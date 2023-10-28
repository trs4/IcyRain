﻿using System;
using System.Linq;
using NUnit.Framework;

namespace IcyRain.Tests;

public class ArrayPrimitiveTest
{
    #region Bool

    [Test]
    public void BoolArray()
    {
        bool[] value = new bool[] { true, false, true, true };

        foreach (var deepClone in Tests<bool[]>.Functions)
        {
            bool[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region Char

    [Test]
    public void CharArray()
    {
        char[] value = new char[] { 't', 'r', '9', 'ц' };

        foreach (var deepClone in Tests<char[]>.Functions)
        {
            char[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region SByte

    [Test]
    public void SByteArray()
    {
        sbyte[] value = new sbyte[] { 0, 34, 3, 7 };

        foreach (var deepClone in Tests<sbyte[]>.Functions)
        {
            sbyte[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region Byte

    [Test]
    public void ByteArray()
    {
        byte[] value = new byte[] { 25, 34, 3, 7 };

        foreach (var deepClone in Tests<byte[]>.Functions)
        {
            byte[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region Short

    [Test]
    public void ShortArray()
    {
        short[] value = new short[] { 25, 34, 3, 7 };

        foreach (var deepClone in Tests<short[]>.Functions)
        {
            short[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region UShort

    [Test]
    public void UShortArray()
    {
        ushort[] value = new ushort[] { 25, 34, 3, 7 };

        foreach (var deepClone in Tests<ushort[]>.Functions)
        {
            ushort[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region Int

    [Test]
    public void IntArray()
    {
        int[] value = new int[] { 25, 34, 3, 7 };

        foreach (var deepClone in Tests<int[]>.Functions)
        {
            int[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region UInt

    [Test]
    public void UIntArray()
    {
        uint[] value = new uint[] { 25, 34, 3, 7 };

        foreach (var deepClone in Tests<uint[]>.Functions)
        {
            uint[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region Long

    [Test]
    public void LongArray()
    {
        long[] value = new long[] { 25, 34, 3, 7 };

        foreach (var deepClone in Tests<long[]>.Functions)
        {
            long[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region ULong

    [Test]
    public void ULongArray()
    {
        ulong[] value = new ulong[] { 25, 34, 3, 7 };

        foreach (var deepClone in Tests<ulong[]>.Functions)
        {
            ulong[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region Float

    [Test]
    public void FloatArray()
    {
        float[] value = new float[] { 25, 34, 3, 7 };

        foreach (var deepClone in Tests<float[]>.Functions)
        {
            float[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region Double

    [Test]
    public void DoubleArray()
    {
        double[] value = new double[] { 25, 34, 3, 7 };

        foreach (var deepClone in Tests<double[]>.Functions)
        {
            double[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region Decimal

    [Test]
    public void DecimalArray()
    {
        decimal[] value = new decimal[] { 25, 34, 3, 7 };

        foreach (var deepClone in Tests<decimal[]>.Functions)
        {
            decimal[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region DateTime

    [Test]
    public void DateTimeArray()
    {
        DateTime[] value = new DateTime[]
        {
            new DateTime(2021, 1, 26, 23, 1, 0, DateTimeKind.Utc),
            new DateTime(2000, 1, 4, 10, 7, 0, DateTimeKind.Utc),
        };

        foreach (var deepClone in Tests<DateTime[]>.Functions)
        {
            DateTime[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region DateTimeOffset

    [Test]
    public void DateTimeOffsetArray()
    {
        DateTimeOffset[] value = new DateTimeOffset[]
        {
            new DateTime(2021, 1, 26, 23, 1, 0, DateTimeKind.Utc),
        };

        foreach (var deepClone in Tests<DateTimeOffset[]>.Functions)
        {
            DateTimeOffset[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region String

    [Test]
    public void StringArray()
    {
        string[] value = new string[] { "25тестtest", null, string.Empty, "t34" };

        foreach (var deepClone in Tests<string[]>.Functions)
        {
            string[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region Guid

    [Test]
    public void GuidArray()
    {
        Guid[] value = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

        foreach (var deepClone in Tests<Guid[]>.Functions)
        {
            Guid[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
    #region TimeSpan

    [Test]
    public void TimeSpanArray()
    {
        TimeSpan[] value = new TimeSpan[] { new TimeSpan(23, 12, 8), new TimeSpan(10, 4, 8) };

        foreach (var deepClone in Tests<TimeSpan[]>.Functions)
        {
            TimeSpan[] result = deepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }
    }

    #endregion
}