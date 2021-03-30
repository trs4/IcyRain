using System;
using System.Collections.Generic;
using System.Net;
using IcyRain.Data.Objects;
using IcyRain.Internal;
using NUnit.Framework;

namespace IcyRain.Tests
{
    public class SerializationTest
    {
        [Test]
        public void VersionTest()
        {
            var value = new Version(1, 2, 3, 4);

            foreach (var deepClone in Tests<Version>.Functions)
            {
                var result = deepClone(value);
                Assert.AreEqual(value.Major, result.Major);
                Assert.AreEqual(value.Minor, result.Minor);
                Assert.AreEqual(value.Build, result.Build);
                Assert.AreEqual(value.Revision, result.Revision);
            }
        }

        [Test]
        public void IPAddressTest()
        {
            var value = IPAddress.Parse("1.2.3.4");

            foreach (var deepClone in Tests<IPAddress>.Functions)
            {
                var result = deepClone(value);
                Assert.AreEqual(value.ToString(), result.ToString());
            }
        }

        [Test]
        public void IPEndPointTest()
        {
            var value = new IPEndPoint(IPAddress.Parse("1.2.3.4"), 12345);

            foreach (var deepClone in Tests<IPEndPoint>.Functions)
            {
                var result = deepClone(value);
                Assert.AreEqual(value.Address.ToString(), result.Address.ToString());
                Assert.AreEqual(value.Port, result.Port);
            }
        }

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
                Check(deepClone, data, clone);
            }
        }

        [Test]
        public void ListSealedClass()
        {
            var data = new SealedData
            {
                Property1 = true,
                Property2 = 25,
                Property3 = 4.5,
                Property4 = new DateTime(2021, 5, 1, 5, 8, 7),
                Property5 = "test",
            };

            var list = new List<SealedData>(100);

            for (int i = 0; i < 100; i++)
                list.Add(data);

            foreach (var deepClone in Tests<List<SealedData>>.Functions)
            {
                var cloneList = deepClone(list);

                Assert.IsNotNull(cloneList);
                Assert.AreEqual(list.Count, cloneList.Count);

                for (int i = 0; i < 100; i++)
                {
                    var clone = cloneList[i];
                    Check(deepClone, data, clone);
                }
            }
        }

        private static void Check(Delegate deepClone, TestData data, TestData clone)
        {
            Assert.IsNotNull(clone);
            Assert.AreEqual(data.Property1, clone.Property1);
            Assert.AreEqual(data.Property2, clone.Property2);
            Assert.AreEqual(data.Property3, clone.Property3);
            Check(deepClone, data.Property4, clone.Property4);
            Assert.AreEqual(data.Property5, clone.Property5);
        }

        private static void Check(Delegate deepClone, SealedData data, SealedData clone)
        {
            Assert.IsNotNull(clone);
            Assert.AreEqual(data.Property1, clone.Property1);
            Assert.AreEqual(data.Property2, clone.Property2);
            Assert.AreEqual(data.Property3, clone.Property3);
            Check(deepClone, data.Property4, clone.Property4);
            Assert.AreEqual(data.Property5, clone.Property5);
        }

        private static void Check(Delegate deepClone, DateTime data, DateTime clone)
        {
            if (deepClone.Method.Name.Contains("InUTC"))
            {
                Assert.AreEqual(DateTimeKind.Utc, clone.Kind);
                Assert.AreEqual(data, clone.ToLocalTime());
            }
            else
            {
                Assert.AreEqual(data, clone);
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
                Check(deepClone, data, clone);
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
                Check(deepClone, data.Property32, clone.Property32);
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
                Check(deepClone, data.Property32, clone.Property32);

                Assert.AreEqual(data.Property33, clone.Property33);
                Assert.AreEqual(data.Property34, clone.Property34);

                Assert.AreEqual(data.Property35.GetType(), clone.Property35?.GetType());
                Assert.AreEqual(data.Property35.Property11, clone.Property35.Property11);
                Assert.AreEqual((data.Property35 as TestB3).Property31, (clone.Property35 as TestB3).Property31);
                Check(deepClone, (data.Property35 as TestB3).Property32, (clone.Property35 as TestB3).Property32);
            }
        }

    }
}
