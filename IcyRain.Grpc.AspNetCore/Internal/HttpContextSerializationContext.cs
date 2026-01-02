using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using Grpc.Core;

namespace IcyRain.Grpc.AspNetCore.Internal;

internal sealed class HttpContextSerializationContext : SerializationContext
{
    private static readonly Status SendingMessageExceedsLimitStatus = new Status(StatusCode.ResourceExhausted,
        "Sending message exceeds the maximum configured message size");

    private readonly HttpContextServerCallContext _serverCallContext;
    private InternalState _state;
    private int? _payloadLength;
    private ArrayBufferWriter<byte>? _bufferWriter;

    public PipeWriter ResponseBufferWriter { get; set; } = default!;

    private bool IsDirectSerializationSupported(out int payloadLength)
    {
        if (_payloadLength is not null)
        {
            payloadLength = _payloadLength.Value;
            return true;
        }

        payloadLength = 0;
        return false;
    }

    public HttpContextSerializationContext(HttpContextServerCallContext serverCallContext)
    {
        _serverCallContext = serverCallContext;
    }

    private enum InternalState : byte
    {
        Initialized,
        CompleteArray,
        IncompleteBufferWriter,
        CompleteBufferWriter,
    }

    public void Reset()
    {
        _payloadLength = null;
        _bufferWriter?.Clear();
        _state = InternalState.Initialized;
    }

    public override void SetPayloadLength(int payloadLength)
    {
        switch (_state)
        {
            case InternalState.Initialized:
                _payloadLength = payloadLength;
                break;
            default:
                ThrowInvalidState(_state);
                break;
        }
    }

    public override void Complete(byte[] payload)
    {
        switch (_state)
        {
            case InternalState.Initialized:
                _state = InternalState.CompleteArray;
                WriteMessage(payload);
                break;
            default:
                ThrowInvalidState(_state);
                break;
        }
    }

    private static void WriteHeader(PipeWriter pipeWriter, int length, bool compress)
    {
        const int MessageDelimiterSize = 4; // how many bytes it takes to encode "Message-Length"
        const int HeaderSize = MessageDelimiterSize + 1; // message length + compression flag

        var headerData = pipeWriter.GetSpan(HeaderSize);

        // Compression flag
        headerData[0] = compress ? (byte)1 : (byte)0;

        // Message length
        BinaryPrimitives.WriteUInt32BigEndian(headerData.Slice(1), (uint)length);

        pipeWriter.Advance(HeaderSize);
    }

    public override IBufferWriter<byte> GetBufferWriter()
    {
        switch (_state)
        {
            case InternalState.Initialized:
                // When writing directly to the buffer the header with message size needs to be written first
                if (IsDirectSerializationSupported(out var payloadLength))
                {
                    EnsureMessageSizeAllowed(payloadLength);

                    WriteHeader(ResponseBufferWriter, payloadLength, compress: false);
                }

                _state = InternalState.IncompleteBufferWriter;
                return ResolveBufferWriter();
            case InternalState.IncompleteBufferWriter:
                return ResolveBufferWriter();
            default:
                ThrowInvalidState(_state);
                return default!;
        }
    }

    private IBufferWriter<byte> ResolveBufferWriter()
    {
        if (IsDirectSerializationSupported(out var payloadLength))
            return ResponseBufferWriter;
        else
            _bufferWriter ??= payloadLength > 0 ? new ArrayBufferWriter<byte>(payloadLength) : new ArrayBufferWriter<byte>();

        return _bufferWriter;
    }

    private void EnsureMessageSizeAllowed(int payloadLength)
    {
        if (payloadLength > _serverCallContext.Options.MaxSendMessageSize)
            throw new RpcException(SendingMessageExceedsLimitStatus);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowInvalidState(InternalState state)
        => throw new InvalidOperationException("Not valid in the current state: " + state.ToString());

    public override void Complete()
    {
        switch (_state)
        {
            case InternalState.IncompleteBufferWriter:
                _state = InternalState.CompleteBufferWriter;

                if (!IsDirectSerializationSupported(out _))
                {
                    Debug.Assert(_bufferWriter is not null, "Buffer writer has been set to get to this state.");
                    WriteMessage(_bufferWriter.WrittenSpan);
                }
                break;
            default:
                ThrowInvalidState(_state);
                break;
        }
    }

    private void WriteMessage(ReadOnlySpan<byte> data)
    {
        EnsureMessageSizeAllowed(data.Length);
        WriteHeader(ResponseBufferWriter, data.Length, compress: false);
        ResponseBufferWriter.Write(data);
    }

}
