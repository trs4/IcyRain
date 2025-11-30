using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Internal;

internal static partial class StreamExtensions
{
    private static readonly Status ReceivedMessageExceedsLimitStatus = new Status(StatusCode.ResourceExhausted,
        "Received message exceeds the maximum configured message size.");

    public static async ValueTask<TResponse?> ReadMessageAsync<TResponse>(
        this Stream responseStream,
        GrpcCall call,
        Func<DeserializationContext, TResponse> deserializer,
        string grpcEncoding,
        bool singleMessage,
        CancellationToken token)
        where TResponse : class
    {
        byte[]? buffer = null;

        try
        {
            token.ThrowIfCancellationRequested();

            // Start with zero-byte read.
            // A zero-byte read avoids renting buffer until the response is ready. Especially useful for long running streaming calls.
            var readCount = await responseStream.ReadAsync(Memory<byte>.Empty, token).ConfigureAwait(false);
            Debug.Assert(readCount == 0);

            // Buffer is used to read header, then message content.
            // This size was randomly chosen to hopefully be big enough for many small messages.
            // If the message is larger then the array will be replaced when the message size is known.
            buffer = ArrayPool<byte>.Shared.Rent(minimumLength: 4096);

            int read;
            var received = 0;

            while ((read = await responseStream.ReadAsync(buffer.AsMemory(received, GrpcProtocolConstants.HeaderSize - received), token).ConfigureAwait(false)) > 0)
            {
                received += read;

                if (received == GrpcProtocolConstants.HeaderSize)
                    break;
            }

            if (received < GrpcProtocolConstants.HeaderSize)
            {
                if (received == 0)
                    return default;

                throw new InvalidDataException("Unexpected end of content while reading the message header.");
            }

            // Read the header first
            // - 1 byte flag for compression
            // - 4 bytes for the content length
            //var compressed = ReadCompressedFlag(buffer[0]);
            var length = ReadMessageLength(buffer.AsSpan(1, 4));

            if (length > 0)
            {
                if (length > call.Channel.ReceiveMaxMessageSize)
                    throw call.CreateRpcException(ReceivedMessageExceedsLimitStatus);

                // Replace buffer if the message doesn't fit
                if (buffer.Length < length)
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                    buffer = ArrayPool<byte>.Shared.Rent(length);
                }

                await ReadMessageContentAsync(responseStream, buffer, length, token).ConfigureAwait(false);
            }

            token.ThrowIfCancellationRequested();
            var payload = new ReadOnlySequence<byte>(buffer, 0, length);
            call.DeserializationContext.SetPayload(payload);
            var message = deserializer(call.DeserializationContext);
            call.DeserializationContext.SetPayload(null);

            if (singleMessage)
            {
                // Check that there is no additional content in the stream for a single message
                // There is no ReadByteAsync on stream. Reuse header array with ReadAsync, we don't need it anymore
                if (await responseStream.ReadAsync(buffer, token).ConfigureAwait(false) > 0)
                    throw new InvalidDataException("Unexpected data after finished reading message.");
            }

            return message;
        }
        catch (ObjectDisposedException) when (token.IsCancellationRequested)
        {
            // When a deadline expires there can be a race between cancellation and Stream.ReadAsync.
            // If ReadAsync is called after the response is disposed then ReadAsync throws ObjectDisposedException.
            // https://github.com/dotnet/runtime/blob/dfbae37e91c4744822018dde10cbd414c661c0b8/src/libraries/System.Net.Http/src/System/Net/Http/SocketsHttpHandler/Http2Stream.cs#L1479-L1482
            //
            // If ObjectDisposedException is caught and cancellation has happened then rethrow as an OCE.
            // This makes gRPC client correctly report a DeadlineExceeded status.
            throw new OperationCanceledException();
        }
        finally
        {
            if (buffer is not null)
                ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private static int ReadMessageLength(Span<byte> header)
    {
        var length = BinaryPrimitives.ReadUInt32BigEndian(header);

        if (length > int.MaxValue)
            throw new InvalidDataException("Message too large");

        return (int)length;
    }

    private static async Task ReadMessageContentAsync(Stream responseStream, Memory<byte> messageData, int length, CancellationToken token)
    {
        // Read message content until content length is reached
        var received = 0;
        int read;

        while ((read = await responseStream.ReadAsync(messageData.Slice(received, length - received), token).ConfigureAwait(false)) > 0)
        {
            received += read;

            if (received == length)
                break;
        }

        if (received < length)
            throw new InvalidDataException("Unexpected end of content while reading the message content");
    }

    //private static bool ReadCompressedFlag(byte flag)
    //{
    //    if (flag == 0)
    //        return false;
    //    else if (flag == 1)
    //        return true;
    //    else
    //        throw new InvalidDataException("Unexpected compressed flag value in message header");
    //}

    public static async Task WriteMessageAsync<TMessage>(
        this Stream stream,
        GrpcCall call,
        TMessage message,
        Action<TMessage, SerializationContext> serializer,
        CallOptions callOptions)
    {
        // Sync relevant changes here with other WriteMessageAsync
        var serializationContext = call.SerializationContext;
        serializationContext.CallOptions = callOptions;
        serializationContext.Initialize();

        try
        {
            // Serialize message first. Need to know size to prefix the length in the header
            serializer(message, serializationContext);

            // Sending the header+content in a single WriteAsync call has significant performance benefits
            // https://github.com/dotnet/runtime/issues/35184#issuecomment-626304981
            await stream.WriteAsync(serializationContext.GetWrittenPayload(), call.CancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (TryCreateCallCompleteException(ex, call, out var statusException))
                throw statusException;

            throw;
        }
        finally
        {
            serializationContext.Reset();
        }
    }

    public static async Task WriteMessageAsync(
        this Stream stream,
        GrpcCall call,
        ReadOnlyMemory<byte> data,
        CancellationToken token)
    {
        try
        {
            // Sending the header+content in a single WriteAsync call has significant performance benefits
            await stream.WriteAsync(data, token).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            if (TryCreateCallCompleteException(ex, call, out var statusException))
                throw statusException;

            throw;
        }
    }

    private static bool IsCancellationException(Exception ex) => ex is OperationCanceledException or ObjectDisposedException;

    private static bool TryCreateCallCompleteException(Exception originalException, GrpcCall call, [NotNullWhen(true)] out Exception? exception)
    {
        // The call may have been completed while WriteAsync was running and caused WriteAsync to throw.
        // In this situation, report the call's completed status.
        //
        // Replace exception with the status error if:
        // 1. The original exception is one Stream.WriteAsync throws if the call was completed during a write, and
        // 2. The call has already been successfully completed.
        if (IsCancellationException(originalException) && call.CallTask.IsCompletedSuccessfully)
        {
            exception = call.CreateFailureStatusException(call.CallTask.Result);
            return true;
        }

        exception = null;
        return false;
    }

}
