using System;
using System.Linq;
using NUnit.Framework;

namespace IcyRain.Tests
{
    public class ArrayPrimitiveTest
    {
        #region Bool

        [Test]
        public void BoolArray()
        {
            bool[] value = new bool[] { true, false, true, true };
            bool[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region Char

        [Test]
        public void CharArray()
        {
            char[] value = new char[] { 't', 'r', '9', 'ц' };
            char[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region SByte

        [Test]
        public void SByteArray()
        {
            sbyte[] value = new sbyte[] { 0, 34, 3, 7 };
            sbyte[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region Byte

        [Test]
        public void ByteArray()
        {
            byte[] value = new byte[] { 25, 34, 3, 7 };
            byte[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region Short

        [Test]
        public void ShortArray()
        {
            short[] value = new short[] { 25, 34, 3, 7 };
            short[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region UShort

        [Test]
        public void UShortArray()
        {
            ushort[] value = new ushort[] { 25, 34, 3, 7 };
            ushort[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region Int

        [Test]
        public void IntArray()
        {
            int[] value = new int[] { 25, 34, 3, 7 };
            int[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region UInt

        [Test]
        public void UIntArray()
        {
            uint[] value = new uint[] { 25, 34, 3, 7 };
            uint[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region Long

        [Test]
        public void LongArray()
        {
            long[] value = new long[] { 25, 34, 3, 7 };
            long[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region ULong

        [Test]
        public void ULongArray()
        {
            ulong[] value = new ulong[] { 25, 34, 3, 7 };
            ulong[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region Float

        [Test]
        public void FloatArray()
        {
            float[] value = new float[] { 25, 34, 3, 7 };
            float[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region Double

        [Test]
        public void DoubleArray()
        {
            double[] value = new double[] { 25, 34, 3, 7 };
            double[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region Decimal

        [Test]
        public void DecimalArray()
        {
            decimal[] value = new decimal[] { 25, 34, 3, 7 };
            decimal[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
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

            DateTime[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
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

            DateTimeOffset[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region String

        [Test]
        public void StringArray()
        {
            string[] value = new string[] { "25тестtest", null, string.Empty, "t34" };
            string[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region Guid

        [Test]
        public void GuidArray()
        {
            Guid[] value = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            Guid[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
        #region TimeSpan

        [Test]
        public void TimeSpanArray()
        {
            TimeSpan[] value = new TimeSpan[] { new TimeSpan(23, 12, 8), new TimeSpan(10, 4, 8) };
            TimeSpan[] result = Serialization.DeepClone(value);
            Assert.IsTrue(value.SequenceEqual(result));
        }

        #endregion
    }
}