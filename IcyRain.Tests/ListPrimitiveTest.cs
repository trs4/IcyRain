using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace IcyRain.Tests
{
    public class ListPrimitiveTest
    {
        #region Bool

        [Test]
        public void BoolList()
        {
            var value = new List<bool> { true, false, true, true };

            foreach (var deepClone in Tests<List<bool>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region Char

        [Test]
        public void CharList()
        {
            var value = new List<char> { 't', 'r', '9', 'ц' };

            foreach (var deepClone in Tests<List<char>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region SByte

        [Test]
        public void SByteList()
        {
            var value = new List<sbyte> { 0, 34, 3, 7 };

            foreach (var deepClone in Tests<List<sbyte>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region Byte

        [Test]
        public void ByteList()
        {
            var value = new List<byte> { 25, 34, 3, 7 };

            foreach (var deepClone in Tests<List<byte>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region Short

        [Test]
        public void ShortList()
        {
            var value = new List<short> { 25, 34, 3, 7 };

            foreach (var deepClone in Tests<List<short>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region UShort

        [Test]
        public void UShortList()
        {
            var value = new List<ushort> { 25, 34, 3, 7 };

            foreach (var deepClone in Tests<List<ushort>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region Int

        [Test]
        public void IntList()
        {
            var value = new List<int> { 25, 34, 3, 7 };

            foreach (var deepClone in Tests<List<int>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region UInt

        [Test]
        public void UIntList()
        {
            var value = new List<uint> { 25, 34, 3, 7 };

            foreach (var deepClone in Tests<List<uint>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region Long

        [Test]
        public void LongList()
        {
            var value = new List<long> { 25, 34, 3, 7 };

            foreach (var deepClone in Tests<List<long>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region ULong

        [Test]
        public void ULongList()
        {
            var value = new List<ulong> { 25, 34, 3, 7 };

            foreach (var deepClone in Tests<List<ulong>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region Float

        [Test]
        public void FloatList()
        {
            var value = new List<float> { 25, 34, 3, 7 };

            foreach (var deepClone in Tests<List<float>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region Double

        [Test]
        public void DoubleList()
        {
            var value = new List<double> { 25, 34, 3, 7 };

            foreach (var deepClone in Tests<List<double>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region Decimal

        [Test]
        public void DecimalList()
        {
            var value = new List<decimal> { 25, 34, 3, 7 };

            foreach (var deepClone in Tests<List<decimal>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region DateTime

        [Test]
        public void DateTimeList()
        {
            var value = new List<DateTime>
            {
                new DateTime(2021, 1, 26, 23, 1, 0, DateTimeKind.Utc),
                new DateTime(2000, 1, 4, 10, 7, 0, DateTimeKind.Utc),
            };

            foreach (var deepClone in Tests<List<DateTime>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region DateTimeOffset

        [Test]
        public void DateTimeOffsetList()
        {
            var value = new List<DateTimeOffset>
            {
                new DateTime(2021, 1, 26, 23, 1, 0, DateTimeKind.Utc),
            };

            foreach (var deepClone in Tests<List<DateTimeOffset>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region String

        [Test]
        public void StringList()
        {
            var value = new List<string> { "25тестtest", null, string.Empty, "t34" };

            foreach (var deepClone in Tests<List<string>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region Guid

        [Test]
        public void GuidList()
        {
            var value = new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            foreach (var deepClone in Tests<List<Guid>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
        #region TimeSpan

        [Test]
        public void TimeSpanList()
        {
            var value = new List<TimeSpan> { new TimeSpan(23, 12, 8), new TimeSpan(10, 4, 8) };

            foreach (var deepClone in Tests<List<TimeSpan>>.Functions)
            {
                var result = deepClone(value);
                Assert.IsTrue(value.SequenceEqual(result));
            }
        }

        #endregion
    }
}