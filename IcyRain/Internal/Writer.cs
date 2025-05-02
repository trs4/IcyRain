using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IcyRain.Compression.LZ4;

namespace IcyRain.Internal;

[StructLayout(LayoutKind.Auto)]
public ref struct Writer
{
    #region Writer

    private int _offset;
    private readonly Span<byte> _span;

    [MethodImpl(Flags.HotPath)]
    internal Writer(in Span<byte> span)
    {
        _offset = 0;
        _span = span;
    }

#pragma warning disable CA1801 // Review unused parameters
    [MethodImpl(Flags.HotPath)]
    internal Writer(in Span<byte> span, bool forCompress)
    {
        span[0] = 0;
        _offset = 1;
        _span = span;
    }
#pragma warning restore CA1801 // Review unused parameters

    internal readonly int Size
    {
        [MethodImpl(Flags.HotPath)]
        get => _offset;
    }

    [MethodImpl(Flags.HotPath)]
    internal void CompressLZ4() => LZ4Codec.Encode(_span, ref _offset);

    #endregion
    #region Types
#pragma warning disable CA1062 // Validate arguments of public methods

    #region Bool

    /// <summary>bool size:1</summary>
    [MethodImpl(Flags.HotPath)]
    public void WriteBool(bool value)
        => _span[_offset++] = (byte)(value ? 1 : 0);

    /// <summary>bool size:1</summary>
    [MethodImpl(Flags.HotPath)]
    public void WriteBoolFalse()
        => _span[_offset++] = 0;

    /// <summary>bool size:1</summary>
    [MethodImpl(Flags.HotPath)]
    public void WriteBoolTrue()
        => _span[_offset++] = 1;

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteBoolArray(bool[] value)
    {
        fixed (byte* ptr = _span)
        fixed (bool* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)value.Length);

        _offset += value.Length;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteBoolArray(bool[] value, int count)
    {
        fixed (byte* ptr = _span)
        fixed (bool* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region Char

    /// <summary>char size:2</summary>
    [MethodImpl(Flags.HotPath)]
    public void WriteChar(char value)
        => WriteUShort(value);

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteCharArray(char[] value)
    {
        int size = value.Length * 2;

        fixed (byte* ptr = _span)
        fixed (char* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)size);

        _offset += size;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteCharArray(char[] value, int count)
    {
        count *= 2;

        fixed (byte* ptr = _span)
        fixed (char* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region SByte

    /// <summary>sbyte size:1</summary>
    [MethodImpl(Flags.HotPath)]
    public void WriteSByte(sbyte value)
        => _span[_offset++] = (byte)value;

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteSByteArray(sbyte[] value)
    {
        fixed (byte* ptr = _span)
        fixed (sbyte* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)value.Length);

        _offset += value.Length;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteSByteArray(sbyte[] value, int count)
    {
        fixed (byte* ptr = _span)
        fixed (sbyte* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region Byte

    /// <summary>byte size:1</summary>
    [MethodImpl(Flags.HotPath)]
    public void WriteByte(byte value)
        => _span[_offset++] = value;

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteByteArray(byte[] value)
    {
        fixed (byte* ptr = _span)
        fixed (byte* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)value.Length);

        _offset += value.Length;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteByteArray(byte[] value, int count)
    {
        fixed (byte* ptr = _span)
        fixed (byte* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteByteArray(byte[] value, int offset, int count)
    {
        fixed (byte* ptr = _span)
        fixed (byte* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue + offset, (uint)count);

        _offset += count;
    }

    #endregion
    #region Short

    /// <summary>short size:2</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteShort(short value)
    {
        fixed (byte* ptr = _span)
            *(short*)(ptr + _offset) = value;

        _offset += 2;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteShortArray(short[] value)
    {
        int size = value.Length * 2;

        fixed (byte* ptr = _span)
        fixed (short* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)size);

        _offset += size;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteShortArray(short[] value, int count)
    {
        count *= 2;

        fixed (byte* ptr = _span)
        fixed (short* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region UShort

    /// <summary>ushort size:2</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteUShort(ushort value)
    {
        fixed (byte* ptr = _span)
            *(ushort*)(ptr + _offset) = value;

        _offset += 2;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteUShortArray(ushort[] value)
    {
        int size = value.Length * 2;

        fixed (byte* ptr = _span)
        fixed (ushort* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)size);

        _offset += size;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteUShortArray(ushort[] value, int count)
    {
        count *= 2;

        fixed (byte* ptr = _span)
        fixed (ushort* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region Int

    /// <summary>int size:4</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteInt(int value)
    {
        fixed (byte* ptr = _span)
            *(int*)(ptr + _offset) = value;

        _offset += 4;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteIntArray(int[] value)
    {
        int size = value.Length * 4;

        fixed (byte* ptr = _span)
        fixed (int* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)size);

        _offset += size;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteIntArray(int[] value, int count)
    {
        count *= 4;

        fixed (byte* ptr = _span)
        fixed (int* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region UInt

    /// <summary>uint size:4</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteUInt(uint value)
    {
        fixed (byte* ptr = _span)
            *(uint*)(ptr + _offset) = value;

        _offset += 4;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteUIntArray(uint[] value)
    {
        int size = value.Length * 4;

        fixed (byte* ptr = _span)
        fixed (uint* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)size);

        _offset += size;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteUIntArray(uint[] value, int count)
    {
        count *= 4;

        fixed (byte* ptr = _span)
        fixed (uint* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region Long

    /// <summary>long size:8</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteLong(long value)
    {
        fixed (byte* ptr = _span)
            *(long*)(ptr + _offset) = value;

        _offset += 8;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteLongArray(long[] value)
    {
        int size = value.Length * 8;

        fixed (byte* ptr = _span)
        fixed (long* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)size);

        _offset += size;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteLongArray(long[] value, int count)
    {
        count *= 8;

        fixed (byte* ptr = _span)
        fixed (long* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region ULong

    /// <summary>ulong size:8</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteULong(ulong value)
    {
        fixed (byte* ptr = _span)
            *(ulong*)(ptr + _offset) = value;

        _offset += 8;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteULongArray(ulong[] value)
    {
        int size = value.Length * 8;

        fixed (byte* ptr = _span)
        fixed (ulong* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)size);

        _offset += size;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteULongArray(ulong[] value, int count)
    {
        count *= 8;

        fixed (byte* ptr = _span)
        fixed (ulong* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region Float

    /// <summary>float size:4</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteFloat(float value)
    {
        fixed (byte* ptr = _span)
            *(float*)(ptr + _offset) = value;

        _offset += 4;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteFloatArray(float[] value)
    {
        int size = value.Length * 4;

        fixed (byte* ptr = _span)
        fixed (float* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)size);

        _offset += size;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteFloatArray(float[] value, int count)
    {
        count *= 4;

        fixed (byte* ptr = _span)
        fixed (float* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region Double

    /// <summary>double size:8</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteDouble(double value)
    {
        fixed (byte* ptr = _span)
            *(double*)(ptr + _offset) = value;

        _offset += 8;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteDoubleArray(double[] value)
    {
        int size = value.Length * 8;

        fixed (byte* ptr = _span)
        fixed (double* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)size);

        _offset += size;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteDoubleArray(double[] value, int count)
    {
        count *= 8;

        fixed (byte* ptr = _span)
        fixed (double* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region Decimal

    /// <summary>decimal size:16</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteDecimal(decimal value)
    {
        fixed (byte* ptr = _span)
            *(decimal*)(ptr + _offset) = value;

        _offset += 16;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteDecimalArray(decimal[] value)
    {
        int size = value.Length * 16;

        fixed (byte* ptr = _span)
        fixed (decimal* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)size);

        _offset += size;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteDecimalArray(decimal[] value, int count)
    {
        count *= 16;

        fixed (byte* ptr = _span)
        fixed (decimal* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region DateTime

    /// <summary>datetime size:8</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteDateTimeWithoutZone(in DateTime value)
    {
        long binaryValue = value.Kind == DateTimeKind.Utc
            ? value.ToBinary()
            : TimeZoneInfo.ConvertTime(value, TimeZoneInfo.Local, TimeZoneInfo.Utc).ToBinary();

        fixed (byte* ptr = _span)
            *(long*)(ptr + _offset) = binaryValue;

        _offset += 8;
    }

    /// <summary>datetime size:9</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteDateTime(in DateTime value)
    {
        bool isUtcTimeZone = value.Kind == DateTimeKind.Utc;

        long binaryValue = isUtcTimeZone
            ? value.ToBinary()
            : TimeZoneInfo.ConvertTime(value, TimeZoneInfo.Local, TimeZoneInfo.Utc).ToBinary();

        fixed (byte* ptr = _span)
        {
            *(long*)(ptr + _offset) = binaryValue;
            *(ptr + _offset + 8) = isUtcTimeZone ? (byte)1 : (byte)0;
        }

        _offset += 9;
    }

    #endregion
    #region String

    /// <summary>string size:calculated</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteString(string value)
    {
        int length = value is null ? -1 : value.Length;
        WriteInt(length);

        if (length > 0)
        {
            int stringSize = length * 2;

            fixed (char* ptrString = value)
            fixed (byte* ptr = _span)
            {
                Unsafe.CopyBlock(ptr + _offset, ptrString, (uint)stringSize);
                _offset += stringSize;
            }
        }
    }

    /// <summary>string size:calculated</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteNotNullString(string value)
    {
        WriteInt(value.Length);

        if (value.Length > 0)
        {
            int stringSize = value.Length * 2;

            fixed (char* ptrString = value)
            fixed (byte* ptr = _span)
            {
                Unsafe.CopyBlock(ptr + _offset, ptrString, (uint)stringSize);
                _offset += stringSize;
            }
        }
    }

    #endregion
    #region Guid

    /// <summary>guid size:16</summary>
    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteGuid(in Guid value)
    {
        fixed (byte* ptr = _span)
            *(Guid*)(ptr + _offset) = value;

        _offset += 16;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteGuidArray(Guid[] value)
    {
        int size = value.Length * 16;

        fixed (byte* ptr = _span)
        fixed (Guid* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)size);

        _offset += size;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteGuidArray(Guid[] value, int count)
    {
        count *= 16;

        fixed (byte* ptr = _span)
        fixed (Guid* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region TimeSpan

    /// <summary>timespan size:8</summary>
    [MethodImpl(Flags.HotPath)]
    public void WriteTimeSpan(in TimeSpan value)
        => WriteLong(value.Ticks);

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteTimeSpanArray(TimeSpan[] value)
    {
        int size = value.Length * 8;

        fixed (byte* ptr = _span)
        fixed (TimeSpan* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)size);

        _offset += size;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteTimeSpanArray(TimeSpan[] value, int count)
    {
        count *= 8;

        fixed (byte* ptr = _span)
        fixed (TimeSpan* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)count);

        _offset += count;
    }

    #endregion
    #region Span

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteSpan(Span<byte> value)
    {
        fixed (byte* ptr = _span)
        fixed (byte* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)value.Length);

        _offset += value.Length;
    }

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteSpan(ReadOnlySpan<byte> value)
    {
        fixed (byte* ptr = _span)
        fixed (byte* ptrValue = value)
            Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)value.Length);

        _offset += value.Length;
    }

    #endregion
    #region ReadOnlySequence

    [MethodImpl(Flags.HotPath)]
    public unsafe void WriteReadOnlySequence(in ReadOnlySequence<byte> value)
    {
        if (value.IsSingleSegment)
        {
            int length = (int)value.Length;

            fixed (byte* ptr = _span)
            fixed (byte* ptrValue = value.First.Span)
                Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)length);

            _offset += length;
        }
        else
        {
            WriteReadOnlySequenceMultiple(in value);
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private unsafe void WriteReadOnlySequenceMultiple(in ReadOnlySequence<byte> value)
    {
        var position = value.Start;

        fixed (byte* ptr = _span)
        {
            while (value.TryGet(ref position, out var memory))
            {
                var span = memory.Span;

                fixed (byte* ptrValue = span)
                    Unsafe.CopyBlock(ptr + _offset, ptrValue, (uint)span.Length);

                _offset += span.Length;

                if (position.GetObject() is null)
                    break;
            }
        }
    }

    #endregion

#pragma warning restore CA1062 // Validate arguments of public methods
    #endregion
}
