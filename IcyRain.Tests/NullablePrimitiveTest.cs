using System;
using System.Linq;
using NUnit.Framework;

namespace IcyRain.Tests;

public class NullablePrimitiveTest
{
    #region Bool

    [Test]
    public void BoolTrue()
    {
        bool? value = true;

        foreach (var deepClone in Tests<bool?>.Functions)
        {
            bool? result = deepClone(value);
            Assert.IsTrue(result.HasValue && result.Value);
        }
    }

    [Test]
    public void BoolFalse()
    {
        bool? value = false;

        foreach (var deepClone in Tests<bool?>.Functions)
        {
            bool? result = deepClone(value);
            Assert.IsTrue(result.HasValue && !result.Value);
        }
    }

    [Test]
    public void BoolNull()
    {
        bool? value = null;

        foreach (var deepClone in Tests<bool?>.Functions)
        {
            bool? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    #endregion
    #region Char

    [Test]
    public void Char1()
    {
        char? value = null;

        foreach (var deepClone in Tests<char?>.Functions)
        {
            char? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void Char2()
    {
        char? value = 'д';

        foreach (var deepClone in Tests<char?>.Functions)
        {
            char? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region SByte

    [Test]
    public void SByte1()
    {
        sbyte? value = null;

        foreach (var deepClone in Tests<sbyte?>.Functions)
        {
            sbyte? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void SByte2()
    {
        sbyte? value = 25;

        foreach (var deepClone in Tests<sbyte?>.Functions)
        {
            sbyte? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region Byte

    [Test]
    public void Byte1()
    {
        byte? value = null;

        foreach (var deepClone in Tests<byte?>.Functions)
        {
            byte? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void Byte2()
    {
        byte? value = 25;

        foreach (var deepClone in Tests<byte?>.Functions)
        {
            byte? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region Short

    [Test]
    public void Short1()
    {
        short? value = null;

        foreach (var deepClone in Tests<short?>.Functions)
        {
            short? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void Short2()
    {
        short? value = 25;

        foreach (var deepClone in Tests<short?>.Functions)
        {
            short? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region UShort

    [Test]
    public void UShort1()
    {
        ushort? value = null;

        foreach (var deepClone in Tests<ushort?>.Functions)
        {
            ushort? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void UShort2()
    {
        ushort? value = 25;

        foreach (var deepClone in Tests<ushort?>.Functions)
        {
            ushort? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region Int

    [Test]
    public void Int1()
    {
        int? value = null;

        foreach (var deepClone in Tests<int?>.Functions)
        {
            int? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void Int2()
    {
        int? value = 25;

        foreach (var deepClone in Tests<int?>.Functions)
        {
            int? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region UInt

    [Test]
    public void UInt1()
    {
        uint? value = null;

        foreach (var deepClone in Tests<uint?>.Functions)
        {
            uint? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void UInt2()
    {
        uint? value = 25;

        foreach (var deepClone in Tests<uint?>.Functions)
        {
            uint? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region Long

    [Test]
    public void Long1()
    {
        long? value = null;

        foreach (var deepClone in Tests<long?>.Functions)
        {
            long? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void Long2()
    {
        long? value = 25;

        foreach (var deepClone in Tests<long?>.Functions)
        {
            long? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region ULong

    [Test]
    public void ULong1()
    {
        ulong? value = null;

        foreach (var deepClone in Tests<ulong?>.Functions)
        {
            ulong? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void ULong2()
    {
        ulong? value = 25;

        foreach (var deepClone in Tests<ulong?>.Functions)
        {
            ulong? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region Float

    [Test]
    public void Float1()
    {
        float? value = null;

        foreach (var deepClone in Tests<float?>.Functions)
        {
            float? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void Float2()
    {
        float? value = 25;

        foreach (var deepClone in Tests<float?>.Functions)
        {
            float? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region Double

    [Test]
    public void Double1()
    {
        double? value = null;

        foreach (var deepClone in Tests<double?>.Functions)
        {
            double? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void Double2()
    {
        double? value = 25;

        foreach (var deepClone in Tests<double?>.Functions)
        {
            double? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region Decimal

    [Test]
    public void Decimal1()
    {
        decimal? value = null;

        foreach (var deepClone in Tests<decimal?>.Functions)
        {
            decimal? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void Decimal2()
    {
        decimal? value = 25;

        foreach (var deepClone in Tests<decimal?>.Functions)
        {
            decimal? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region DateTime

    [Test]
    public void DateTime1()
    {
        DateTime? value = null;

        foreach (var deepClone in Tests<DateTime?>.Functions)
        {
            DateTime? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void DateTime2()
    {
        DateTime? value = new DateTime(2021, 1, 26, 23, 1, 0, DateTimeKind.Utc);

        foreach (var deepClone in Tests<DateTime?>.Functions)
        {
            DateTime? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
            Assert.AreEqual(value.Value.Kind, result.Value.Kind);
        }
    }

    #endregion
    #region DateTimeOffset

    [Test]
    public void DateTimeOffset1()
    {
        DateTimeOffset? value = null;

        foreach (var deepClone in Tests<DateTimeOffset?>.Functions)
        {
            DateTimeOffset? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void DateTimeOffset2()
    {
        DateTimeOffset? value = new DateTime(2021, 1, 26, 23, 1, 0, DateTimeKind.Utc);

        foreach (var deepClone in Tests<DateTimeOffset?>.Functions)
        {
            DateTimeOffset? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region Guid

    [Test]
    public void Guid1()
    {
        Guid? value = null;

        foreach (var deepClone in Tests<Guid?>.Functions)
        {
            Guid? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void Guid2()
    {
        Guid? value = Guid.NewGuid();

        foreach (var deepClone in Tests<Guid?>.Functions)
        {
            Guid? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region TimeSpan

    [Test]
    public void TimeSpan1()
    {
        TimeSpan? value = null;

        foreach (var deepClone in Tests<TimeSpan?>.Functions)
        {
            TimeSpan? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void TimeSpan2()
    {
        TimeSpan? value = new TimeSpan(23, 12, 8);

        foreach (var deepClone in Tests<TimeSpan?>.Functions)
        {
            TimeSpan? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }
    }

    #endregion
    #region IntPtr

    [Test]
    public void IntPtr1()
    {
        IntPtr? value = null;

        foreach (var deepClone in Tests<IntPtr?>.Functions)
        {
            IntPtr? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void IntPtr2()
    {
        IntPtr? value = new IntPtr(25);

        foreach (var deepClone in Tests<IntPtr?>.Functions)
        {
            IntPtr? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value.ToInt64() == result.Value.ToInt64());
        }
    }

    #endregion
    #region UIntPtr

    [Test]
    public void UIntPtr1()
    {
        UIntPtr? value = null;

        foreach (var deepClone in Tests<UIntPtr?>.Functions)
        {
            UIntPtr? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void UIntPtr2()
    {
        UIntPtr? value = new UIntPtr(25);

        foreach (var deepClone in Tests<UIntPtr?>.Functions)
        {
            UIntPtr? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value.ToUInt64() == result.Value.ToUInt64());
        }
    }

    #endregion
    #region ArraySegment<byte>

    [Test]
    public void ArraySegment1()
    {
        ArraySegment<byte>? value = null;

        foreach (var deepClone in Tests<ArraySegment<byte>?>.Functions)
        {
            ArraySegment<byte>? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void ArraySegment2()
    {
        ArraySegment<byte>? value = new ArraySegment<byte>(new byte[] { 1, 5, 0, 9 });

        foreach (var deepClone in Tests<ArraySegment<byte>?>.Functions)
        {
            ArraySegment<byte>? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value.Array.SequenceEqual(result.Value));
        }
    }

    #endregion
    #region Memory<byte>

    [Test]
    public void Memory1()
    {
        Memory<byte>? value = null;

        foreach (var deepClone in Tests<Memory<byte>?>.Functions)
        {
            Memory<byte>? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void Memory2()
    {
        Memory<byte>? value = new byte[] { 1, 5, 0, 9 };

        foreach (var deepClone in Tests<Memory<byte>?>.Functions)
        {
            Memory<byte>? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value.ToArray().SequenceEqual(result.Value.ToArray()));
        }
    }

    #endregion
    #region ReadOnlyMemory<byte>

    [Test]
    public void ReadOnlyMemory1()
    {
        ReadOnlyMemory<byte>? value = null;

        foreach (var deepClone in Tests<ReadOnlyMemory<byte>?>.Functions)
        {
            ReadOnlyMemory<byte>? result = deepClone(value);
            Assert.IsFalse(result.HasValue);
        }
    }

    [Test]
    public void ReadOnlyMemory2()
    {
        ReadOnlyMemory<byte>? value = new byte[] { 1, 5, 0, 9 };

        foreach (var deepClone in Tests<ReadOnlyMemory<byte>?>.Functions)
        {
            ReadOnlyMemory<byte>? result = deepClone(value);
            Assert.IsTrue(result.HasValue && value.Value.ToArray().SequenceEqual(result.Value.ToArray()));
        }
    }

    #endregion
}