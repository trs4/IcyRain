using System;
using System.Buffers;
using System.Linq;
using NUnit.Framework;

namespace IcyRain.Tests
{
    public class PrimitiveTest
    {
        #region Bool

        [Test]
        public void BoolTrue()
        {
            bool result = Serialization.DeepClone(true);
            Assert.IsTrue(result);
        }

        [Test]
        public void BoolFalse()
        {
            bool result = Serialization.DeepClone(false);
            Assert.IsFalse(result);
        }

        #endregion
        #region Char

        [Test]
        public void Char1()
        {
            char value = 't';
            char result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void Char2()
        {
            char value = 'д';
            char result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region SByte

        [Test]
        public void SByte1()
        {
            sbyte value = 0;
            sbyte result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void SByte2()
        {
            sbyte value = 25;
            sbyte result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region Byte

        [Test]
        public void Byte1()
        {
            byte value = 0;
            byte result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void Byte2()
        {
            byte value = 25;
            byte result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region Short

        [Test]
        public void Short1()
        {
            short value = 0;
            short result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void Short2()
        {
            short value = 25;
            short result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region UShort

        [Test]
        public void UShort1()
        {
            ushort value = 0;
            ushort result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void UShort2()
        {
            ushort value = 25;
            ushort result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region Int

        [Test]
        public void Int1()
        {
            int value = 0;
            int result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void Int2()
        {
            int value = 25;
            int result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region UInt

        [Test]
        public void UInt1()
        {
            uint value = 0;
            uint result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void UInt2()
        {
            uint value = 25;
            uint result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region Long

        [Test]
        public void Long1()
        {
            long value = 0;
            long result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void Long2()
        {
            long value = 25;
            long result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region ULong

        [Test]
        public void ULong1()
        {
            ulong value = 0;
            ulong result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void ULong2()
        {
            ulong value = 25;
            ulong result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region Float

        [Test]
        public void Float1()
        {
            float value = 0;
            float result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void Float2()
        {
            float value = 25;
            float result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region Double

        [Test]
        public void Double1()
        {
            double value = 0;
            double result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void Double2()
        {
            double value = 25;
            double result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region Decimal

        [Test]
        public void Decimal1()
        {
            decimal value = 0;
            decimal result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void Decimal2()
        {
            decimal value = 25;
            decimal result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region DateTime

        [Test]
        public void DateTime1()
        {
            DateTime value = DateTime.UtcNow;
            DateTime result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
            Assert.AreEqual(value.Kind, result.Kind);
        }

        [Test]
        public void DateTime2()
        {
            DateTime value = DateTime.Now;
            DateTime result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
            Assert.AreEqual(value.Kind, result.Kind);
        }

        [Test]
        public void DateTime3()
        {
            DateTime value = new DateTime(2021, 1, 26, 23, 1, 0, DateTimeKind.Utc);
            DateTime result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
            Assert.AreEqual(value.Kind, result.Kind);
        }

        #endregion
        #region DateTimeOffset

        [Test]
        public void DateTimeOffset1()
        {
            DateTimeOffset value = new DateTimeOffset(DateTime.UtcNow);
            DateTimeOffset result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void DateTimeOffset2()
        {
            DateTimeOffset value = new DateTime(2021, 1, 26, 23, 1, 0, DateTimeKind.Utc);
            DateTimeOffset result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region String

        [Test]
        public void String1()
        {
            string value = string.Empty;
            string result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void String2()
        {
            string value = "25тестtest";
            string result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region Guid

        [Test]
        public void Guid1()
        {
            Guid value = Guid.Empty;
            Guid result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void Guid2()
        {
            Guid value = Guid.NewGuid();
            Guid result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region TimeSpan

        [Test]
        public void TimeSpan1()
        {
            TimeSpan value = TimeSpan.Zero;
            TimeSpan result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void TimeSpan2()
        {
            TimeSpan value = new TimeSpan(23, 12, 8);
            TimeSpan result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region IntPtr

        [Test]
        public void IntPtr1()
        {
            IntPtr value = IntPtr.Zero;
            IntPtr result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void IntPtr2()
        {
            IntPtr value = new IntPtr(25);
            IntPtr result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region UIntPtr

        [Test]
        public void UIntPtr1()
        {
            UIntPtr value = UIntPtr.Zero;
            UIntPtr result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        [Test]
        public void UIntPtr2()
        {
            UIntPtr value = new UIntPtr(25);
            UIntPtr result = Serialization.DeepClone(value);
            Assert.AreEqual(value, result);
        }

        #endregion
        #region ArraySegment<byte>

        [Test]
        public void ArraySegment1()
        {
            ArraySegment<byte> value = default;
            ArraySegment<byte> result = Serialization.DeepClone(value);
            Assert.IsTrue(result.Array is null || result.Array.Length == 0);
        }

        [Test]
        public void ArraySegment2()
        {
            ArraySegment<byte> value = new ArraySegment<byte>(new byte[] { 1, 5, 0, 9 });
            ArraySegment<byte> result = Serialization.DeepClone(value);
            Assert.IsTrue(value.Array.SequenceEqual(result));
        }

        #endregion
        #region Memory<byte>

        [Test]
        public void Memory1()
        {
            Memory<byte> value = default;
            Memory<byte> result = Serialization.DeepClone(value);
            Assert.IsTrue(result.IsEmpty);
        }

        [Test]
        public void Memory2()
        {
            Memory<byte> value = new byte[] { 1, 5, 0, 9 };
            Memory<byte> result = Serialization.DeepClone(value);
            Assert.IsTrue(value.ToArray().SequenceEqual(result.ToArray()));
        }

        #endregion
        #region ReadOnlyMemory<byte>

        [Test]
        public void ReadOnlyMemory1()
        {
            ReadOnlyMemory<byte> value = default;
            ReadOnlyMemory<byte> result = Serialization.DeepClone(value);
            Assert.IsTrue(result.IsEmpty);
        }

        [Test]
        public void ReadOnlyMemory2()
        {
            ReadOnlyMemory<byte> value = new byte[] { 1, 5, 0, 9 };
            ReadOnlyMemory<byte> result = Serialization.DeepClone(value);
            Assert.IsTrue(value.ToArray().SequenceEqual(result.ToArray()));
        }

        #endregion
        #region ReadOnlySequence<byte>

        [Test]
        public void ReadOnlySequence1()
        {
            ReadOnlySequence<byte> value = default;
            ReadOnlySequence<byte> result = Serialization.DeepClone(value);
            Assert.IsTrue(result.IsEmpty);
        }

        [Test]
        public void ReadOnlySequence2()
        {
            ReadOnlySequence<byte> value = new ReadOnlySequence<byte>(new byte[] { 1, 5, 0, 9 });
            ReadOnlySequence<byte> result = Serialization.DeepClone(value);
            Assert.IsTrue(value.ToArray().SequenceEqual(result.ToArray()));
        }

        #endregion
    }
}