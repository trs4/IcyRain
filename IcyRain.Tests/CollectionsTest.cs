﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace IcyRain.Tests
{
    public class CollectionsTest
    {
        [Test]
        public void DictionaryIntString()
        {
            var data = new Dictionary<int, string>()
            {
                { 2, "1" },
                { 5, "2" },
                { 10, "3" },
                { 9, "4" },
                { 3, "5" },
                { 4, "6" },
            };

            var clone = Serialization.DeepClone(data);

            Assert.IsNotNull(clone);
            Assert.AreEqual(data.Count, clone.Count);
            Assert.IsTrue(clone.TryGetValue(2, out string value) && value == "1");
            Assert.IsTrue(clone.TryGetValue(5, out value) && value == "2");
            Assert.IsTrue(clone.TryGetValue(10, out value) && value == "3");
            Assert.IsTrue(clone.TryGetValue(9, out value) && value == "4");
            Assert.IsTrue(clone.TryGetValue(3, out value) && value == "5");
            Assert.IsTrue(clone.TryGetValue(4, out value) && value == "6");
        }

        [Test]
        public void ReadOnlyDictionaryIntString()
        {
            var dictionary = new Dictionary<int, string>()
            {
                { 2, "1" },
                { 5, "2" },
                { 10, "3" },
                { 9, "4" },
                { 3, "5" },
                { 4, "6" },
            };

            var data = new ReadOnlyDictionary<int, string>(dictionary);
            var clone = Serialization.DeepClone(data);

            Assert.IsNotNull(clone);
            Assert.AreEqual(data.Count, clone.Count);
            Assert.IsTrue(clone.TryGetValue(2, out string value) && value == "1");
            Assert.IsTrue(clone.TryGetValue(5, out value) && value == "2");
            Assert.IsTrue(clone.TryGetValue(10, out value) && value == "3");
            Assert.IsTrue(clone.TryGetValue(9, out value) && value == "4");
            Assert.IsTrue(clone.TryGetValue(3, out value) && value == "5");
            Assert.IsTrue(clone.TryGetValue(4, out value) && value == "6");
        }

        [Test]
        public void HashSetInt()
        {
            var data = new HashSet<int>() { 2, 7, 8, 3, 4 };

            var clone = Serialization.DeepClone(data);

            Assert.IsNotNull(clone);
            Assert.AreEqual(data.Count, clone.Count);
            Assert.IsTrue(clone.Contains(2));
            Assert.IsTrue(clone.Contains(7));
            Assert.IsTrue(clone.Contains(8));
            Assert.IsTrue(clone.Contains(3));
            Assert.IsTrue(clone.Contains(4));
        }

        [Test]
        public void ReadOnlyCollectionInt()
        {
            var list = new List<int> { 2, 7, 8, 3, 4 };
            var data = new ReadOnlyCollection<int>(list);

            var clone = Serialization.DeepClone(data);

            Assert.IsNotNull(clone);
            Assert.AreEqual(data.Count, clone.Count);
            Assert.IsTrue(clone.Contains(2));
            Assert.IsTrue(clone.Contains(7));
            Assert.IsTrue(clone.Contains(8));
            Assert.IsTrue(clone.Contains(3));
            Assert.IsTrue(clone.Contains(4));
        }

        [Test]
        public void KeyValuePairIntString()
        {
            var data = new KeyValuePair<int, string>(2, "1");

            var clone = Serialization.DeepClone(data);

            Assert.IsNotNull(clone);
            Assert.AreEqual(data.Key, clone.Key);
            Assert.AreEqual(data.Value, clone.Value);
        }

    }
}
