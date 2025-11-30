using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Internal;

internal sealed class GrpcCallSerializationContext : SerializationContext, IBufferWriter<byte>
{
    private static readonly Status SendingMessageExceedsLimitStatus = new Status(StatusCode.ResourceExhausted,
        "Sending message exceeds the maximum configured message size.");

    private readonly GrpcCall _call;
    private InternalState _state;
    private int? _payloadLength;

    private bool IsDirectSerializationSupported(out int payloadLength)
    {
        // Message can be written directly to the buffer if:
        // - Its length is known.
        // - There is no compression.
        if (_payloadLength != null)
        {
            payloadLength = _payloadLength.Value;
            return true;
        }

        payloadLength = 0;
        return false;
    }

    private ArrayBufferWriter<byte>? _bufferWriter;
    private byte[]? _buffer;
    private int _bufferPosition;

    public CallOptions CallOptions { get; set; }

    public GrpcCallSerializationContext(GrpcCall call)
        => _call = call;

    private enum InternalState : byte
    {
        Initialized,
        CompleteArray,
        IncompleteBufferWriter,
        CompleteBufferWriter,
    }

    public void Initialize()
    {
        _payloadLength = null;
        _state = InternalState.Initialized;
        _bufferPosition = 0;
    }

    public void Reset()
    {
        // Release writer and buffer.
        // Stream could be long running and we don't want to hold onto large
        // buffer arrays for a long period of time.
        _bufferWriter = null;

        if (_buffer != null)
        {
            ArrayPool<byte>.Shared.Return(_buffer);
            _buffer = null;
            _bufferPosition = 0;
        }
    }

    /// <summary>
    /// Obtains the payload from this operation. Error is thrown if complete hasn't been called.
    /// </summary>
    public ReadOnlyMemory<byte> GetWrittenPayload()
    {
        switch (_state)
        {
            case InternalState.CompleteArray:
            case InternalState.CompleteBufferWriter:
                if (_buffer is not null)
                    return _buffer.AsMemory(0, _bufferPosition);
                else if (_bufferWriter is not null)
                    return _bufferWriter.WrittenMemory;
                break;
        }

        throw new InvalidOperationException("Serialization did not return a payload.");
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

    private static void WriteHeader(Span<byte> headerData, int length, bool compress)
    {
        // Compression flag
        headerData[0] = compress ? (byte)1 : (byte)0;

        // Message length
        BinaryPrimitives.WriteUInt32BigEndian(headerData.Slice(1), (uint)length);
    }

    public override IBufferWriter<byte> GetBufferWriter()
    {
        switch (_state)
        {
            case InternalState.Initialized:
                var bufferWriter = ResolveBufferWriter();

                // When writing directly to the buffer the header with message size needs to be written first
                if (IsDirectSerializationSupported(out var payloadLength))
                {
                    EnsureMessageSizeAllowed(payloadLength);

                    WriteHeader(_buffer, payloadLength, compress: false);
                    _bufferPosition += GrpcProtocolConstants.HeaderSize;
                }

                _state = InternalState.IncompleteBufferWriter;
                return bufferWriter;
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
        {
            _buffer ??= ArrayPool<byte>.Shared.Rent(GrpcProtocolConstants.HeaderSize + payloadLength);
            return this;
        }
        else // Initialize buffer writer with exact length if available. ArrayBufferWriter doesn't allow zero initial length.
            _bufferWriter ??= payloadLength > 0 ? new ArrayBufferWriter<byte>(payloadLength) : new ArrayBufferWriter<byte>();

        return _bufferWriter;
    }

    private void EnsureMessageSizeAllowed(int payloadLength)
    {
        if (payloadLength > _call.Channel.SendMaxMessageSize)
            throw _call.CreateRpcException(SendingMessageExceedsLimitStatus);
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
                    Debug.Assert(_bufferWriter != null, "Buffer writer has been set to get to this state.");

                    var data = _bufferWriter.WrittenSpan;
                    WriteMessage(data);
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
        _buffer = ArrayPool<byte>.Shared.Rent(GrpcProtocolConstants.HeaderSize + data.Length);

        WriteHeader(_buffer, data.Length, compress: false);
        _bufferPosition += GrpcProtocolConstants.HeaderSize;

        data.CopyTo(_buffer.AsSpan(GrpcProtocolConstants.HeaderSize));
        _bufferPosition += data.Length;
    }

    public void Advance(int count)
    {
        if (_buffer != null)
            _bufferPosition += count;
        else
            _bufferWriter!.Advance(count);
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
        => _buffer != null ? _buffer.AsMemory(_bufferPosition) : _bufferWriter!.GetMemory(sizeHint);

    public Span<byte> GetSpan(int sizeHint = 0)
        => _buffer != null ? _buffer.AsSpan(_bufferPosition) : _bufferWriter!.GetSpan(sizeHint);
}
