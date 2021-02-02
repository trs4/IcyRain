using System;
using System.Linq;
using NUnit.Framework;

namespace IcyRain.Tests
{
    public class NullablePrimitiveTest
    {
        #region Bool

        [Test]
        public void BoolTrue()
        {
            bool? value = true;
            bool? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && result.Value);
        }

        [Test]
        public void BoolFalse()
        {
            bool? value = false;
            bool? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && !result.Value);
        }

        [Test]
        public void BoolNull()
        {
            bool? value = null;
            bool? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        #endregion
        #region Char

        [Test]
        public void Char1()
        {
            char? value = null;
            char? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void Char2()
        {
            char? value = 'д';
            char? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region SByte

        [Test]
        public void SByte1()
        {
            sbyte? value = null;
            sbyte? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void SByte2()
        {
            sbyte? value = 25;
            sbyte? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region Byte

        [Test]
        public void Byte1()
        {
            byte? value = null;
            byte? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void Byte2()
        {
            byte? value = 25;
            byte? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region Short

        [Test]
        public void Short1()
        {
            short? value = null;
            short? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void Short2()
        {
            short? value = 25;
            short? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region UShort

        [Test]
        public void UShort1()
        {
            ushort? value = null;
            ushort? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void UShort2()
        {
            ushort? value = 25;
            ushort? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region Int

        [Test]
        public void Int1()
        {
            int? value = null;
            int? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void Int2()
        {
            int? value = 25;
            int? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region UInt

        [Test]
        public void UInt1()
        {
            uint? value = null;
            uint? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void UInt2()
        {
            uint? value = 25;
            uint? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region Long

        [Test]
        public void Long1()
        {
            long? value = null;
            long? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void Long2()
        {
            long? value = 25;
            long? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region ULong

        [Test]
        public void ULong1()
        {
            ulong? value = null;
            ulong? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void ULong2()
        {
            ulong? value = 25;
            ulong? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region Float

        [Test]
        public void Float1()
        {
            float? value = null;
            float? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void Float2()
        {
            float? value = 25;
            float? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region Double

        [Test]
        public void Double1()
        {
            double? value = null;
            double? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void Double2()
        {
            double? value = 25;
            double? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region Decimal

        [Test]
        public void Decimal1()
        {
            decimal? value = null;
            decimal? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void Decimal2()
        {
            decimal? value = 25;
            decimal? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region DateTime

        [Test]
        public void DateTime1()
        {
            DateTime? value = null;
            DateTime? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void DateTime2()
        {
            DateTime? value = new DateTime(2021, 1, 26, 23, 1, 0, DateTimeKind.Utc);
            DateTime? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
            Assert.AreEqual(value.Value.Kind, result.Value.Kind);
        }

        #endregion
        #region DateTimeOffset

        [Test]
        public void DateTimeOffset1()
        {
            DateTimeOffset? value = null;
            DateTimeOffset? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void DateTimeOffset2()
        {
            DateTimeOffset? value = new DateTime(2021, 1, 26, 23, 1, 0, DateTimeKind.Utc);
            DateTimeOffset? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region Guid

        [Test]
        public void Guid1()
        {
            Guid? value = null;
            Guid? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void Guid2()
        {
            Guid? value = Guid.NewGuid();
            Guid? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region TimeSpan

        [Test]
        public void TimeSpan1()
        {
            TimeSpan? value = null;
            TimeSpan? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void TimeSpan2()
        {
            TimeSpan? value = new TimeSpan(23, 12, 8);
            TimeSpan? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value == result.Value);
        }

        #endregion
        #region IntPtr

        [Test]
        public void IntPtr1()
        {
            IntPtr? value = null;
            IntPtr? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void IntPtr2()
        {
            IntPtr? value = new IntPtr(25);
            IntPtr? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value.ToInt64() == result.Value.ToInt64());
        }

        #endregion
        #region UIntPtr

        [Test]
        public void UIntPtr1()
        {
            UIntPtr? value = null;
            UIntPtr? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void UIntPtr2()
        {
            UIntPtr? value = new UIntPtr(25);
            UIntPtr? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value.ToUInt64() == result.Value.ToUInt64());
        }

        #endregion
        #region ArraySegment<byte>

        [Test]
        public void ArraySegment1()
        {
            ArraySegment<byte>? value = null;
            ArraySegment<byte>? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void ArraySegment2()
        {
            ArraySegment<byte>? value = new ArraySegment<byte>(new byte[] { 1, 5, 0, 9 });
            ArraySegment<byte>? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value.Array.SequenceEqual(result.Value));
        }

        #endregion
        #region Memory<byte>

        [Test]
        public void Memory1()
        {
            Memory<byte>? value = null;
            Memory<byte>? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void Memory2()
        {
            Memory<byte>? value = new byte[] { 1, 5, 0, 9 };
            Memory<byte>? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value.ToArray().SequenceEqual(result.Value.ToArray()));
        }

        #endregion
        #region ReadOnlyMemory<byte>

        [Test]
        public void ReadOnlyMemory1()
        {
            ReadOnlyMemory<byte>? value = null;
            ReadOnlyMemory<byte>? result = Serialization.DeepClone(value);
            Assert.IsFalse(result.HasValue);
        }

        [Test]
        public void ReadOnlyMemory2()
        {
            ReadOnlyMemory<byte>? value = new byte[] { 1, 5, 0, 9 };
            ReadOnlyMemory<byte>? result = Serialization.DeepClone(value);
            Assert.IsTrue(result.HasValue && value.Value.ToArray().SequenceEqual(result.Value.ToArray()));
        }

        #endregion
    }
}