using System;
using System.Buffers;
using System.Linq;
using NUnit.Framework;

namespace IcyRain.Tests;

public class PrimitiveTest
{
    #region Bool

    [Test]
    public void BoolTrue()
    {
        foreach (var deepClone in Tests<bool>.Functions)
        {
            bool result = deepClone(true);
            Assert.That(result);
        }
    }

    [Test]
    public void BoolFalse()
    {
        foreach (var deepClone in Tests<bool>.Functions)
        {
            bool result = deepClone(false);
            Assert.That(!result);
        }
    }

    #endregion
    #region Char

    [Test]
    public void Char1()
    {
        const char value = 't';

        foreach (var deepClone in Tests<char>.Functions)
        {
            char result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void Char2()
    {
        const char value = 'д';

        foreach (var deepClone in Tests<char>.Functions)
        {
            char result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region SByte

    [Test]
    public void SByte1()
    {
        const sbyte value = 0;

        foreach (var deepClone in Tests<sbyte>.Functions)
        {
            sbyte result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void SByte2()
    {
        const sbyte value = 25;

        foreach (var deepClone in Tests<sbyte>.Functions)
        {
            sbyte result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region Byte

    [Test]
    public void Byte1()
    {
        const byte value = 0;

        foreach (var deepClone in Tests<byte>.Functions)
        {
            byte result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void Byte2()
    {
        const byte value = 25;

        foreach (var deepClone in Tests<byte>.Functions)
        {
            byte result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region Short

    [Test]
    public void Short1()
    {
        const short value = 0;

        foreach (var deepClone in Tests<short>.Functions)
        {
            short result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void Short2()
    {
        const short value = 25;

        foreach (var deepClone in Tests<short>.Functions)
        {
            short result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region UShort

    [Test]
    public void UShort1()
    {
        const ushort value = 0;

        foreach (var deepClone in Tests<ushort>.Functions)
        {
            ushort result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void UShort2()
    {
        const ushort value = 25;

        foreach (var deepClone in Tests<ushort>.Functions)
        {
            ushort result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region Int

    [Test]
    public void Int1()
    {
        const int value = 0;

        foreach (var deepClone in Tests<int>.Functions)
        {
            int result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void Int2()
    {
        const int value = 25;

        foreach (var deepClone in Tests<int>.Functions)
        {
            int result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region UInt

    [Test]
    public void UInt1()
    {
        const uint value = 0;

        foreach (var deepClone in Tests<uint>.Functions)
        {
            uint result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void UInt2()
    {
        const uint value = 25;

        foreach (var deepClone in Tests<uint>.Functions)
        {
            uint result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region Long

    [Test]
    public void Long1()
    {
        const long value = 0;

        foreach (var deepClone in Tests<long>.Functions)
        {
            long result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void Long2()
    {
        const long value = 25;

        foreach (var deepClone in Tests<long>.Functions)
        {
            long result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region ULong

    [Test]
    public void ULong1()
    {
        const ulong value = 0;

        foreach (var deepClone in Tests<ulong>.Functions)
        {
            ulong result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void ULong2()
    {
        const ulong value = 25;

        foreach (var deepClone in Tests<ulong>.Functions)
        {
            ulong result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region Float

    [Test]
    public void Float1()
    {
        const float value = 0;

        foreach (var deepClone in Tests<float>.Functions)
        {
            float result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void Float2()
    {
        const float value = 25;

        foreach (var deepClone in Tests<float>.Functions)
        {
            float result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region Double

    [Test]
    public void Double1()
    {
        const double value = 0;

        foreach (var deepClone in Tests<double>.Functions)
        {
            double result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void Double2()
    {
        const double value = 25;

        foreach (var deepClone in Tests<double>.Functions)
        {
            double result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region Decimal

    [Test]
    public void Decimal1()
    {
        const decimal value = 0;

        foreach (var deepClone in Tests<decimal>.Functions)
        {
            decimal result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void Decimal2()
    {
        const decimal value = 25;

        foreach (var deepClone in Tests<decimal>.Functions)
        {
            decimal result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region DateTime

    [Test]
    public void DateTime1()
    {
        DateTime value = DateTime.UtcNow;

        foreach (var deepClone in Tests<DateTime>.Functions)
        {
            DateTime result = deepClone(value);

            Assert.That(value == result);
            Assert.That(value.Kind == result.Kind);
        }
    }

    [Test]
    public void DateTime2()
    {
        DateTime value = DateTime.Now;

        foreach (var deepClone in Tests<DateTime>.Functions)
        {
            DateTime result = deepClone(value);

            if (deepClone.Method.Name.Contains("InUTC"))
            {
                Assert.That(DateTimeKind.Utc == result.Kind);
                result = result.ToLocalTime();
            }
            else
            {
                Assert.That(value.Kind == result.Kind);
            }

            Assert.That(value == result);
        }
    }

    [Test]
    public void DateTime3()
    {
        DateTime value = new DateTime(2021, 1, 26, 23, 1, 0, DateTimeKind.Utc);

        foreach (var deepClone in Tests<DateTime>.Functions)
        {
            DateTime result = deepClone(value);
            Assert.That(value == result);
            Assert.That(value.Kind == result.Kind);
        }
    }

    #endregion
    #region DateTimeOffset

    [Test]
    public void DateTimeOffset1()
    {
        DateTimeOffset value = new DateTimeOffset(DateTime.UtcNow);

        foreach (var deepClone in Tests<DateTimeOffset>.Functions)
        {
            DateTimeOffset result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void DateTimeOffset2()
    {
        DateTimeOffset value = new DateTime(2021, 1, 26, 23, 1, 0, DateTimeKind.Utc);

        foreach (var deepClone in Tests<DateTimeOffset>.Functions)
        {
            DateTimeOffset result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region String

    [Test]
    public void String1()
    {
        string value = string.Empty;

        foreach (var deepClone in Tests<string>.Functions)
        {
            string result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void String2()
    {
        string value = "25тестtest";

        foreach (var deepClone in Tests<string>.Functions)
        {
            string result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region Guid

    [Test]
    public void Guid1()
    {
        Guid value = Guid.Empty;

        foreach (var deepClone in Tests<Guid>.Functions)
        {
            Guid result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void Guid2()
    {
        Guid value = Guid.NewGuid();

        foreach (var deepClone in Tests<Guid>.Functions)
        {
            Guid result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region TimeSpan

    [Test]
    public void TimeSpan1()
    {
        TimeSpan value = TimeSpan.Zero;

        foreach (var deepClone in Tests<TimeSpan>.Functions)
        {
            TimeSpan result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void TimeSpan2()
    {
        TimeSpan value = new TimeSpan(23, 12, 8);

        foreach (var deepClone in Tests<TimeSpan>.Functions)
        {
            TimeSpan result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region IntPtr

    [Test]
    public void IntPtr1()
    {
        IntPtr value = IntPtr.Zero;

        foreach (var deepClone in Tests<IntPtr>.Functions)
        {
            IntPtr result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void IntPtr2()
    {
        IntPtr value = new IntPtr(25);

        foreach (var deepClone in Tests<IntPtr>.Functions)
        {
            IntPtr result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region UIntPtr

    [Test]
    public void UIntPtr1()
    {
        UIntPtr value = UIntPtr.Zero;

        foreach (var deepClone in Tests<UIntPtr>.Functions)
        {
            UIntPtr result = deepClone(value);
            Assert.That(value == result);
        }
    }

    [Test]
    public void UIntPtr2()
    {
        UIntPtr value = new UIntPtr(25);

        foreach (var deepClone in Tests<UIntPtr>.Functions)
        {
            UIntPtr result = deepClone(value);
            Assert.That(value == result);
        }
    }

    #endregion
    #region ArraySegment<byte>

    [Test]
    public void ArraySegment1()
    {
        ArraySegment<byte> value = default;

        foreach (var deepClone in Tests<ArraySegment<byte>>.Functions)
        {
            ArraySegment<byte> result = deepClone(value);
            Assert.That(result.Array is null || result.Array.Length == 0);
        }
    }

    [Test]
    public void ArraySegment2()
    {
        ArraySegment<byte> value = new ArraySegment<byte>([1, 5, 0, 9]);

        foreach (var deepClone in Tests<ArraySegment<byte>>.Functions)
        {
            ArraySegment<byte> result = deepClone(value);
            Assert.That(value.Array.SequenceEqual(result));
        }
    }

    #endregion
    #region Memory<byte>

    [Test]
    public void Memory1()
    {
        Memory<byte> value = default;

        foreach (var deepClone in Tests<Memory<byte>>.Functions)
        {
            Memory<byte> result = deepClone(value);
            Assert.That(result.IsEmpty);
        }
    }

    [Test]
    public void Memory2()
    {
        Memory<byte> value = new byte[] { 1, 5, 0, 9 };

        foreach (var deepClone in Tests<Memory<byte>>.Functions)
        {
            Memory<byte> result = deepClone(value);
            Assert.That(value.ToArray().SequenceEqual(result.ToArray()));
        }
    }

    #endregion
    #region ReadOnlyMemory<byte>

    [Test]
    public void ReadOnlyMemory1()
    {
        ReadOnlyMemory<byte> value = default;

        foreach (var deepClone in Tests<ReadOnlyMemory<byte>>.Functions)
        {
            ReadOnlyMemory<byte> result = deepClone(value);
            Assert.That(result.IsEmpty);
        }
    }

    [Test]
    public void ReadOnlyMemory2()
    {
        ReadOnlyMemory<byte> value = new byte[] { 1, 5, 0, 9 };

        foreach (var deepClone in Tests<ReadOnlyMemory<byte>>.Functions)
        {
            ReadOnlyMemory<byte> result = deepClone(value);
            Assert.That(value.ToArray().SequenceEqual(result.ToArray()));
        }
    }

    #endregion
    #region ReadOnlySequence<byte>

    [Test]
    public void ReadOnlySequence1()
    {
        ReadOnlySequence<byte> value = default;

        foreach (var deepClone in Tests<ReadOnlySequence<byte>>.Functions)
        {
            ReadOnlySequence<byte> result = deepClone(value);
            Assert.That(result.IsEmpty);
        }
    }

    [Test]
    public void ReadOnlySequence2()
    {
        ReadOnlySequence<byte> value = new ReadOnlySequence<byte>([1, 5, 0, 9]);

        foreach (var deepClone in Tests<ReadOnlySequence<byte>>.Functions)
        {
            ReadOnlySequence<byte> result = deepClone(value);
            Assert.That(value.ToArray().SequenceEqual(result.ToArray()));
        }
    }

    #endregion
}