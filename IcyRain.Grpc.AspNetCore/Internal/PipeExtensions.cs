using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal static partial class PipeExtensions
{
    private const int MessageDelimiterSize = 4; // how many bytes it takes to encode "Message-Length"
    private const int HeaderSize = MessageDelimiterSize + 1; // message length + compression flag

    private static readonly Status MessageCancelledStatus = new Status(StatusCode.Internal, "Incoming message cancelled.");
    private static readonly Status AdditionalDataStatus = new Status(StatusCode.Internal, "Additional data after the message received.");
    private static readonly Status IncompleteMessageStatus = new Status(StatusCode.Internal, "Incomplete message.");
    private static readonly Status ReceivedMessageExceedsLimitStatus = new Status(StatusCode.ResourceExhausted, "Received message exceeds the maximum configured message size.");

    public static async Task WriteSingleMessageAsync<TResponse>(this PipeWriter pipeWriter, TResponse response, HttpContextServerCallContext serverCallContext,
        Action<TResponse, SerializationContext> serializer)
        where TResponse : class
    {
        // Must call StartAsync before the first pipeWriter.GetSpan() in WriteHeader
        var httpResponse = serverCallContext.HttpContext.Response;

        if (!httpResponse.HasStarted)
            await httpResponse.StartAsync();

        var serializationContext = serverCallContext.SerializationContext;
        serializationContext.Reset();
        serializationContext.ResponseBufferWriter = pipeWriter;
        serializer(response, serializationContext);
    }

    public static async Task WriteStreamedMessageAsync<TResponse>(this PipeWriter pipeWriter, TResponse response, HttpContextServerCallContext serverCallContext,
        Action<TResponse, SerializationContext> serializer, CancellationToken token = default)
        where TResponse : class
    {
        // Must call StartAsync before the first pipeWriter.GetSpan() in WriteHeader
        var httpResponse = serverCallContext.HttpContext.Response;

        if (!httpResponse.HasStarted)
            await httpResponse.StartAsync(token);

        var serializationContext = serverCallContext.SerializationContext;
        serializationContext.Reset();
        serializationContext.ResponseBufferWriter = pipeWriter;
        serializer(response, serializationContext);

        // Flush messages unless WriteOptions.Flags has BufferHint set
        var flush = ((serverCallContext.WriteOptions?.Flags ?? default) & WriteFlags.BufferHint) != WriteFlags.BufferHint;

        if (flush)
        {
            var flushResult = await pipeWriter.FlushAsync(token);

            // Workaround bug where FlushAsync doesn't return IsCanceled = true on request abort.
            // https://github.com/dotnet/aspnetcore/issues/40788
            // Also, sometimes the request CT isn't triggered. Also check CT passed into method.
            if (!flushResult.IsCompleted && (serverCallContext.CancellationToken.IsCancellationRequested || token.IsCancellationRequested))
                throw new OperationCanceledException("Request aborted while sending the message.");
        }
    }

    private static int DecodeMessageLength(ReadOnlySpan<byte> buffer)
    {
        var result = BinaryPrimitives.ReadUInt32BigEndian(buffer);

        if (result > int.MaxValue)
            throw new IOException("Message too large: " + result);

        return (int)result;
    }

    private static bool TryReadHeader(in ReadOnlySequence<byte> buffer, out int messageLength)
    {
        if (buffer.Length < HeaderSize)
        {
            messageLength = 0;
            return false;
        }

        if (buffer.First.Length >= HeaderSize)
        {
            var headerData = buffer.First.Span.Slice(0, HeaderSize);
            messageLength = DecodeMessageLength(headerData.Slice(1));
        }
        else
        {
            Span<byte> headerData = stackalloc byte[HeaderSize];
            buffer.Slice(0, HeaderSize).CopyTo(headerData);
            messageLength = DecodeMessageLength(headerData.Slice(1));
        }

        return true;
    }

    private static bool ReadCompressedFlag(byte flag)
    {
        if (flag == 0)
            return false;
        else if (flag == 1)
            return true;
        else
            throw new InvalidDataException("Unexpected compressed flag value in message header.");
    }

    public static async ValueTask<T> ReadSingleMessageAsync<T>(this PipeReader input, HttpContextServerCallContext serverCallContext,
        Func<DeserializationContext, T> deserializer)
        where T : class
    {
        T? request = null;

        while (true)
        {
            var result = await input.ReadAsync();
            var buffer = result.Buffer;

            try
            {
                if (result.IsCanceled)
                    throw new RpcException(MessageCancelledStatus);

                if (!buffer.IsEmpty)
                {
                    if (request is not null)
                        throw new RpcException(AdditionalDataStatus);

                    if (TryReadMessage(ref buffer, serverCallContext, out var data))
                    {
                        serverCallContext.DeserializationContext.SetPayload(data);
                        request = deserializer(serverCallContext.DeserializationContext);
                        serverCallContext.DeserializationContext.SetPayload(null);
                    }
                }

                if (result.IsCompleted)
                {
                    if (request is not null)
                    {
                        if (buffer.Length > 0)
                            throw new RpcException(AdditionalDataStatus);

                        return request;
                    }

                    throw new RpcException(IncompleteMessageStatus);
                }
            }
            finally
            {
                if (request is not null)
                    input.AdvanceTo(buffer.Start);
                else
                    input.AdvanceTo(buffer.Start, buffer.End);
            }
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public static async ValueTask<T?> ReadStreamMessageAsync<T>(this PipeReader input, HttpContextServerCallContext serverCallContext,
        Func<DeserializationContext, T> deserializer, CancellationToken token = default)
        where T : class
    {
        while (true)
        {
            var completeMessage = false;
            var result = await input.ReadAsync(token);
            var buffer = result.Buffer;

            try
            {
                if (result.IsCanceled)
                    throw new RpcException(MessageCancelledStatus);

                if (!buffer.IsEmpty)
                {
                    if (TryReadMessage(ref buffer, serverCallContext, out var data))
                    {
                        completeMessage = true;

                        serverCallContext.DeserializationContext.SetPayload(data);
                        var request = deserializer(serverCallContext.DeserializationContext);
                        serverCallContext.DeserializationContext.SetPayload(null);
                        return request;
                    }
                }

                if (result.IsCompleted)
                {
                    if (buffer.Length == 0)
                        return default;

                    throw new RpcException(IncompleteMessageStatus);
                }
            }
            finally
            {
                if (completeMessage)
                    input.AdvanceTo(buffer.Start);
                else
                    input.AdvanceTo(buffer.Start, buffer.End);
            }
        }
    }

    private static bool TryReadMessage(ref ReadOnlySequence<byte> buffer, HttpContextServerCallContext context, out ReadOnlySequence<byte> message)
    {
        if (!TryReadHeader(buffer, out var messageLength))
        {
            message = default;
            return false;
        }

        if (messageLength > context.Options.MaxReceiveMessageSize)
            throw new RpcException(ReceivedMessageExceedsLimitStatus);

        if (buffer.Length < HeaderSize + messageLength)
        {
            message = default;
            return false;
        }

        // Convert message to byte array
        message = buffer.Slice(HeaderSize, messageLength);
        buffer = buffer.Slice(HeaderSize + messageLength);
        return true;
    }

}
