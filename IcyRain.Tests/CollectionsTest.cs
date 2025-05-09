using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace IcyRain.Tests;

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
            { 4, "7" },
            { 42, "5" },
            { 43, "9" },
            { 44, "10" },

            { 45, "11" },
            { 46, "12" },
            { 47, "13" },
            { 48, "14" },
            { 49, "15" },
            { 410, "16" },
            { 411, "17" },
            { 412, "18" },
            { 413, "19" },
            { 414, "20" },
        };

        foreach (var deepClone in Tests<Dictionary<int, string>>.Functions)
        {
            var clone = deepClone(data);

            Assert.That(clone is not null);
            Assert.That(data.Count == clone.Count);
            Assert.That(clone.TryGetValue(2, out string value) && value == "1");
            Assert.That(clone.TryGetValue(5, out value) && value == "2");
            Assert.That(clone.TryGetValue(10, out value) && value == "3");
            Assert.That(clone.TryGetValue(9, out value) && value == "4");
            Assert.That(clone.TryGetValue(3, out value) && value == "5");
            Assert.That(clone.TryGetValue(4, out value) && value == "7");
        }
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

        foreach (var deepClone in Tests<ReadOnlyDictionary<int, string>>.Functions)
        {
            var clone = deepClone(data);

            Assert.That(clone is not null);
            Assert.That(data.Count == clone.Count);
            Assert.That(clone.TryGetValue(2, out string value) && value == "1");
            Assert.That(clone.TryGetValue(5, out value) && value == "2");
            Assert.That(clone.TryGetValue(10, out value) && value == "3");
            Assert.That(clone.TryGetValue(9, out value) && value == "4");
            Assert.That(clone.TryGetValue(3, out value) && value == "5");
            Assert.That(clone.TryGetValue(4, out value) && value == "6");
        }
    }

    [Test]
    public void HashSetInt()
    {
        var data = new HashSet<int>() { 2, 7, 8, 3, 4 };

        foreach (var deepClone in Tests<HashSet<int>>.Functions)
        {
            var clone = deepClone(data);

            Assert.That(clone is not null);
            Assert.That(data.Count == clone.Count);
            Assert.That(clone.Contains(2));
            Assert.That(clone.Contains(7));
            Assert.That(clone.Contains(8));
            Assert.That(clone.Contains(3));
            Assert.That(clone.Contains(4));
        }
    }

    [Test]
    public void ReadOnlyCollectionInt()
    {
        var list = new List<int> { 2, 7, 8, 3, 4 };
        var data = new ReadOnlyCollection<int>(list);

        foreach (var deepClone in Tests<ReadOnlyCollection<int>>.Functions)
        {
            var clone = deepClone(data);

            Assert.That(clone is not null);
            Assert.That(data.Count == clone.Count);
            Assert.That(clone.Contains(2));
            Assert.That(clone.Contains(7));
            Assert.That(clone.Contains(8));
            Assert.That(clone.Contains(3));
            Assert.That(clone.Contains(4));
        }
    }

    [Test]
    public void KeyValuePairIntString()
    {
        var data = new KeyValuePair<int, string>(2, "1");

        foreach (var deepClone in Tests<KeyValuePair<int, string>>.Functions)
        {
            var clone = deepClone(data);

            Assert.That(data.Key == clone.Key);
            Assert.That(data.Value == clone.Value);
        }
    }

}
