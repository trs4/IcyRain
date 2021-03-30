using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace IcyRain.Internal
{
    [StructLayout(LayoutKind.Auto)]
    public ref struct Reader
    {
        #region Reader

        private int _offset;
        private ReadOnlyMemory<byte> _currentMemory;
        private ReadOnlySpan<byte> _currentSpan;
        private readonly ReadOnlySequence<byte>.Enumerator _sequenceEnumerator;

        [MethodImpl(Flags.HotPath)]
        internal Reader(in ReadOnlySequence<byte> sequence)
        {
            _offset = 0;

            if (sequence.IsSingleSegment)
            {
                _currentMemory = sequence.First;
                _currentSpan = _currentMemory.Span;
                _sequenceEnumerator = default;
            }
            else
            {
                _sequenceEnumerator = sequence.GetEnumerator();
                _sequenceEnumerator.MoveNext();
                _currentMemory = _sequenceEnumerator.Current;
                _currentSpan = _currentMemory.Span;
            }
        }

        [MethodImpl(Flags.HotPath)]
        internal Reader(in ReadOnlyMemory<byte> memory)
        {
            _offset = 0;
            _currentMemory = memory;
            _currentSpan = memory.Span;
            _sequenceEnumerator = default;
        }

#pragma warning disable CA1801 // Review unused parameters
        [MethodImpl(Flags.HotPath)]
        internal Reader(in ReadOnlySequence<byte> sequence, bool forCompress)
        {
            _offset = 1;

            if (sequence.IsSingleSegment)
            {
                _currentMemory = sequence.First;
                _currentSpan = _currentMemory.Span;
                _sequenceEnumerator = default;
            }
            else
            {
                _sequenceEnumerator = sequence.GetEnumerator();
                _sequenceEnumerator.MoveNext();
                _currentMemory = _sequenceEnumerator.Current;
                _currentSpan = _currentMemory.Span;
            }
        }

        [MethodImpl(Flags.HotPath)]
        internal Reader(in ReadOnlyMemory<byte> memory, bool forCompress)
        {
            _offset = 1;
            _currentMemory = memory;
            _currentSpan = memory.Span;
            _sequenceEnumerator = default;
        }
#pragma warning restore CA1801 // Review unused parameters

        #endregion
        #region Memory

        [MethodImpl(Flags.HotPath)]
        public ReadOnlyMemory<byte> GetMemory(int size)
        {
            int remainingSpace = _currentSpan.Length - _offset - size;

            if (remainingSpace > 0)
            {
                var memory = _currentMemory.Slice(_offset, size);
                _offset += size;
                return memory;
            }

            return ReadNextMemory(remainingSpace, size);
        }

        [MethodImpl(Flags.HotPath)]
        public Memory<byte> ReadMemory(int size)
        {
            Memory<byte> memory = new byte[size];
            GetMemory(size).TryCopyTo(memory);
            return memory;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private ReadOnlyMemory<byte> ReadNextMemory(int remainingSpace, int size)
        {
            if (remainingSpace == 0)
            {
                var memory = _currentMemory.Slice(_offset);
                _currentMemory = _sequenceEnumerator.MoveNext() ? _sequenceEnumerator.Current : default;
                _currentSpan = _currentMemory.Span;
                _offset = 0;
                return memory;
            }

            byte[] result = new byte[size];
            _currentSpan.Slice(_offset).TryCopyTo(result);
            ReadMultiSegment(result, remainingSpace + size, -remainingSpace);
            return result;
        }

        #endregion
        #region Span

        [MethodImpl(Flags.HotPath)]
        private ReadOnlySpan<byte> GetSpan(int size)
        {
            int remainingSpace = _currentSpan.Length - _offset - size;

            if (remainingSpace > 0)
            {
                var span = _currentSpan.Slice(_offset, size);
                _offset += size;
                return span;
            }

            return ReadNextSpan(remainingSpace, size);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private ReadOnlySpan<byte> ReadNextSpan(int remainingSpace, int size)
        {
            if (remainingSpace == 0)
            {
                var span = _currentSpan.Slice(_offset);
                _currentMemory = _sequenceEnumerator.MoveNext() ? _sequenceEnumerator.Current : default;
                _currentSpan = _currentMemory.Span;
                _offset = 0;
                return span;
            }

            byte[] result = new byte[size];
            _currentSpan.Slice(_offset).TryCopyTo(result);
            ReadMultiSegment(result, remainingSpace + size, -remainingSpace);
            return result;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ReadMultiSegment(byte[] result, int resultOffset, int size)
        {
            if (!_sequenceEnumerator.MoveNext())
                throw new InvalidOperationException("Empty space");

            _currentMemory = _sequenceEnumerator.Current;
            _currentSpan = _currentMemory.Span;

            if (size > _currentSpan.Length)
            {
                _currentSpan.Slice(0, size).TryCopyTo(result.AsSpan(resultOffset));
                _offset = size;
            }
            else if (size == _currentSpan.Length)
            {
                _currentSpan.TryCopyTo(result.AsSpan(resultOffset));
                _currentMemory = _sequenceEnumerator.MoveNext() ? _sequenceEnumerator.Current : default;
                _currentSpan = _currentMemory.Span;
                _offset = 0;
            }
            else
            {
                _currentSpan.TryCopyTo(result.AsSpan(resultOffset));
                ReadMultiSegment(result, resultOffset + _currentSpan.Length, size - _currentSpan.Length);
            }
        }

        [MethodImpl(Flags.HotPath)]
        private byte GetSpanByte()
        {
            int remainingSpace = _currentSpan.Length - _offset - 1;
            return remainingSpace >= 0 ? _currentSpan[_offset++] : ReadSpanByte();
        }

        private byte ReadSpanByte()
        {
            if (!_sequenceEnumerator.MoveNext())
                throw new InvalidOperationException("Empty space");

            _currentMemory = _sequenceEnumerator.Current;
            _currentSpan = _currentMemory.Span;
            _offset = 1;
            return _currentSpan.Length > 0 ? _currentSpan[0] : ReadSpanByte();
        }

        #endregion
        #region Types

        #region Bool

        [MethodImpl(Flags.HotPath)]
        public bool ReadBool() => GetSpanByte() != 0;

        [MethodImpl(Flags.HotPath)]
        public unsafe bool[] ReadBoolArray(int length)
        {
            bool[] value = new bool[length];

            fixed (byte* ptr = GetSpan(length))
            fixed (bool* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)length);

            return value;
        }

        #endregion
        #region Char

        [MethodImpl(Flags.HotPath)]
        public char ReadChar()
            => (char)ReadUShort();

        [MethodImpl(Flags.HotPath)]
        public unsafe char[] ReadCharArray(int length)
        {
            char[] value = new char[length];
            int size = length * 2;

            fixed (byte* ptr = GetSpan(size))
            fixed (char* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)size);

            return value;
        }

        #endregion
        #region SByte

        [MethodImpl(Flags.HotPath)]
        public sbyte ReadSByte() => (sbyte)GetSpanByte();

        [MethodImpl(Flags.HotPath)]
        public unsafe sbyte[] ReadSByteArray(int length)
        {
            sbyte[] value = new sbyte[length];

            fixed (byte* ptr = GetSpan(length))
            fixed (sbyte* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)length);

            return value;
        }

        #endregion
        #region Byte

        [MethodImpl(Flags.HotPath)]
        public byte ReadByte() => GetSpanByte();

        [MethodImpl(Flags.HotPath)]
        public unsafe byte[] ReadByteArray(int length)
        {
            byte[] value = new byte[length];

            fixed (byte* ptr = GetSpan(length))
            fixed (byte* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)length);

            return value;
        }

        #endregion
        #region Short

        [MethodImpl(Flags.HotPath)]
        public unsafe short ReadShort()
        {
            fixed (byte* ptr = GetSpan(2))
                return *(short*)ptr;
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe short[] ReadShortArray(int length)
        {
            short[] value = new short[length];
            int size = length * 2;

            fixed (byte* ptr = GetSpan(size))
            fixed (short* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)size);

            return value;
        }

        #endregion
        #region UShort

        [MethodImpl(Flags.HotPath)]
        public unsafe ushort ReadUShort()
        {
            fixed (byte* ptr = GetSpan(2))
                return *(ushort*)ptr;
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe ushort[] ReadUShortArray(int length)
        {
            ushort[] value = new ushort[length];
            int size = length * 2;

            fixed (byte* ptr = GetSpan(size))
            fixed (ushort* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)size);

            return value;
        }

        #endregion
        #region Int

        [MethodImpl(Flags.HotPath)]
        public unsafe int ReadInt()
        {
            fixed (byte* ptr = GetSpan(4))
                return *(int*)ptr;
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe int[] ReadIntArray(int length)
        {
            int[] value = new int[length];
            int size = length * 4;

            fixed (byte* ptr = GetSpan(size))
            fixed (int* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)size);

            return value;
        }

        #endregion
        #region UInt

        [MethodImpl(Flags.HotPath)]
        public unsafe uint ReadUInt()
        {
            fixed (byte* ptr = GetSpan(4))
                return *(uint*)ptr;
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe uint[] ReadUIntArray(int length)
        {
            uint[] value = new uint[length];
            int size = length * 4;

            fixed (byte* ptr = GetSpan(size))
            fixed (uint* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)size);

            return value;
        }

        #endregion
        #region Long

        [MethodImpl(Flags.HotPath)]
        public unsafe long ReadLong()
        {
            fixed (byte* ptr = GetSpan(8))
                return *(long*)ptr;
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe long[] ReadLongArray(int length)
        {
            long[] value = new long[length];
            int size = length * 8;

            fixed (byte* ptr = GetSpan(size))
            fixed (long* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)size);

            return value;
        }

        #endregion
        #region ULong

        [MethodImpl(Flags.HotPath)]
        public unsafe ulong ReadULong()
        {
            fixed (byte* ptr = GetSpan(8))
                return *(ulong*)ptr;
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe ulong[] ReadULongArray(int length)
        {
            ulong[] value = new ulong[length];
            int size = length * 8;

            fixed (byte* ptr = GetSpan(size))
            fixed (ulong* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)size);

            return value;
        }

        #endregion
        #region Float

        [MethodImpl(Flags.HotPath)]
        public unsafe float ReadFloat()
        {
            fixed (byte* ptr = GetSpan(4))
                return *(float*)ptr;
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe float[] ReadFloatArray(int length)
        {
            float[] value = new float[length];
            int size = length * 4;

            fixed (byte* ptr = GetSpan(size))
            fixed (float* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)size);

            return value;
        }

        #endregion
        #region Double

        [MethodImpl(Flags.HotPath)]
        public unsafe double ReadDouble()
        {
            fixed (byte* ptr = GetSpan(8))
                return *(double*)ptr;
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe double[] ReadDoubleArray(int length)
        {
            double[] value = new double[length];
            int size = length * 8;

            fixed (byte* ptr = GetSpan(size))
            fixed (double* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)size);

            return value;
        }

        #endregion
        #region Decimal

        [MethodImpl(Flags.HotPath)]
        public unsafe decimal ReadDecimal()
        {
            fixed (byte* ptr = GetSpan(16))
                return *(decimal*)ptr;
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe decimal[] ReadDecimalArray(int length)
        {
            decimal[] value = new decimal[length];
            int size = length * 16;

            fixed (byte* ptr = GetSpan(size))
            fixed (decimal* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)size);

            return value;
        }

        #endregion
        #region DateTime

        [MethodImpl(Flags.HotPath)]
        public DateTime ReadDateTimeWithoutZone()
            => DateTime.FromBinary(ReadLong());

        [MethodImpl(Flags.HotPath)]
        public unsafe DateTime ReadDateTime()
        {
            DateTime value;
            bool isUtcTimeZone;

            fixed (byte* ptr = GetSpan(9))
            {
                value = DateTime.FromBinary(*(long*)ptr);
                isUtcTimeZone = *(ptr + 8) == 1;
            }

            return isUtcTimeZone ? value : value.ToLocalTime();
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe DateTime ReadDateTimeInUTC()
        {
            fixed (byte* ptr = GetSpan(9))
                return DateTime.FromBinary(*(long*)ptr);
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe void AppendDateTime(ref DateTime value)
        {
            bool isUtcTimeZone;

            fixed (byte* ptr = GetSpan(9))
            {
                value = DateTime.FromBinary(*(long*)ptr);
                isUtcTimeZone = *(ptr + 8) == 1;
            }

            if (!isUtcTimeZone)
                value = value.ToLocalTime();
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe void AppendDateTimeInUTC(ref DateTime value)
        {
            fixed (byte* ptr = GetSpan(9))
                value = DateTime.FromBinary(*(long*)ptr);
        }

        #endregion
        #region String

        [MethodImpl(Flags.HotPath)]
        public unsafe string ReadString()
        {
            int length = ReadInt();

            if (length > 0)
            {
                int stringSize = length * 2;
#if NETFRAMEWORK
                char[] chars = new char[length];
#else
                Span<char> chars = stackalloc char[length];
#endif
                fixed (char* charsPtr = chars)
                fixed (byte* ptr = GetSpan(stringSize))
                    Unsafe.CopyBlock(charsPtr, ptr, (uint)stringSize);

                return new string(chars);
            }

            return length == 0 ? string.Empty : null;
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe string ReadNotNullString()
        {
            int length = ReadInt();

            if (length > 0)
            {
                int stringSize = length * 2;
#if NETFRAMEWORK
                char[] chars = new char[length];
#else
                Span<char> chars = stackalloc char[length];
#endif
                fixed (char* charsPtr = chars)
                fixed (byte* ptr = GetSpan(stringSize))
                    Unsafe.CopyBlock(charsPtr, ptr, (uint)stringSize);

                return new string(chars);
            }

            return string.Empty;
        }

        #endregion
        #region Guid

        [MethodImpl(Flags.HotPath)]
        public unsafe Guid ReadGuid()
        {
            fixed (byte* ptr = GetSpan(16))
                return *(Guid*)ptr;
        }

        [MethodImpl(Flags.HotPath)]
        public unsafe Guid[] ReadGuidArray(int length)
        {
            Guid[] value = new Guid[length];
            int size = length * 16;

            fixed (byte* ptr = GetSpan(size))
            fixed (Guid* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)size);

            return value;
        }

        #endregion
        #region TimeSpan

        [MethodImpl(Flags.HotPath)]
        public TimeSpan ReadTimeSpan() => new TimeSpan(ReadLong());

        [MethodImpl(Flags.HotPath)]
        public unsafe TimeSpan[] ReadTimeSpanArray(int length)
        {
            TimeSpan[] value = new TimeSpan[length];
            int size = length * 8;

            fixed (byte* ptr = GetSpan(size))
            fixed (TimeSpan* ptrValue = value)
                Unsafe.CopyBlock(ptrValue, ptr, (uint)size);

            return value;
        }

        #endregion
        #region ReadOnlySequence

        [MethodImpl(Flags.HotPath)]
        public ReadOnlySequence<byte> ReadReadOnlySequence(int length)
            => new ReadOnlySequence<byte>(GetMemory(length));

        #endregion

        #endregion
    }
}
