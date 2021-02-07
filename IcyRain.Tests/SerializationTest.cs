using System;
using IcyRain.Data.Objects;
using NUnit.Framework;

namespace IcyRain.Tests
{
    public class SerializationTest
    {
        [Test]
        public void SealedClass()
        {
            var data = new SealedData
            {
                Property1 = true,
                Property2 = 25,
                Property3 = 4.5,
                Property4 = new DateTime(2021, 5, 1, 5, 8, 7),
                Property5 = "test",
            };

            foreach (var deepClone in Tests<SealedData>.Functions)
            {
                var clone = deepClone(data);

                Assert.IsNotNull(clone);
                Assert.AreEqual(data.Property1, clone.Property1);
                Assert.AreEqual(data.Property2, clone.Property2);
                Assert.AreEqual(data.Property3, clone.Property3);
                Assert.AreEqual(data.Property4, clone.Property4);
                Assert.AreEqual(data.Property5, clone.Property5);
            }
        }

        [Test]
        public void DefaultResolver()
        {
            var data = new TestData
            {
                Property1 = true,
                Property2 = 25,
                Property3 = 4.5,
                Property4 = new DateTime(2021, 5, 1, 5, 8, 7),
                Property5 = "test",
            };

            foreach (var deepClone in Tests<TestData>.Functions)
            {
                var clone = deepClone(data);

                Assert.IsNotNull(clone);
                Assert.AreEqual(data.Property1, clone.Property1);
                Assert.AreEqual(data.Property2, clone.Property2);
                Assert.AreEqual(data.Property3, clone.Property3);
                Assert.AreEqual(data.Property4, clone.Property4);
                Assert.AreEqual(data.Property5, clone.Property5);
            }
        }

        [Test]
        public void UnionResolver()
        {
            var data = new TestB3
            {
                Property11 = true,
                Property31 = 7.5,
                Property32 = new DateTime(2010, 5, 1, 5, 8, 7),
            };

            foreach (var deepClone in Tests<TestB1>.Functions)
            {
                var clone = deepClone(data) as TestB3;

                Assert.IsNotNull(clone);
                Assert.AreEqual(data.Property11, clone.Property11);
                Assert.AreEqual(data.Property31, clone.Property31);
                Assert.AreEqual(data.Property32, clone.Property32);
            }
        }

        [Test]
        public void UnionHashResolver()
        {
            var data = new TestA25
            {
                Property11 = true,
                Property31 = 7.5,
                Property32 = new DateTime(2010, 5, 1, 5, 8, 7),
                Property34 = "new IntPtr(8)",
                Property35 = new TestB3
                {
                    Property11 = true,
                    Property31 = 10.5,
                    Property32 = new DateTime(2000, 5, 1, 5, 8, 7),
                },
            };

            foreach (var deepClone in Tests<TestA1>.Functions)
            {
                var clone = deepClone(data) as TestA25;

                Assert.IsNotNull(clone);
                Assert.AreEqual(data.Property11, clone.Property11);
                Assert.AreEqual(data.Property31, clone.Property31);
                Assert.AreEqual(data.Property32, clone.Property32);
                Assert.AreEqual(data.Property33, clone.Property33);
                Assert.AreEqual(data.Property34, clone.Property34);

                Assert.AreEqual(data.Property35.GetType(), clone.Property35?.GetType());
                Assert.AreEqual(data.Property35.Property11, clone.Property35.Property11);
                Assert.AreEqual((data.Property35 as TestB3).Property31, (clone.Property35 as TestB3).Property31);
                Assert.AreEqual((data.Property35 as TestB3).Property32, (clone.Property35 as TestB3).Property32);
            }
        }

    }
}
