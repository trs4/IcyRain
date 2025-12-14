using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using IcyRain.Data.Objects;
using IcyRain.Tables;
using NUnit.Framework;

namespace IcyRain.Tests;

public class SerializationTest
{
    [Test]
    public void VersionTest()
    {
        var value = new Version(1, 2, 3, 4);

        foreach (var deepClone in Tests<Version>.Functions)
        {
            var result = deepClone(value);
            Assert.That(value.Major == result.Major);
            Assert.That(value.Minor == result.Minor);
            Assert.That(value.Build == result.Build);
            Assert.That(value.Revision == result.Revision);
        }
    }

    [Test]
    public void IPAddressTest()
    {
        var value = IPAddress.Parse("1.2.3.4");

        foreach (var deepClone in Tests<IPAddress>.Functions)
        {
            var result = deepClone(value);
            Assert.That(value.ToString() == result.ToString());
        }
    }

    [Test]
    public void IPEndPointTest()
    {
        var value = new IPEndPoint(IPAddress.Parse("1.2.3.4"), 12345);

        foreach (var deepClone in Tests<IPEndPoint>.Functions)
        {
            var result = deepClone(value);
            Assert.That(value.Address.ToString() == result.Address.ToString());
            Assert.That(value.Port == result.Port);
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

            Assert.That(cloneList is not null);
            Assert.That(list.Count == cloneList.Count);

            for (int i = 0; i < 100; i++)
            {
                var clone = cloneList[i];
                Check(deepClone, data, clone);
            }
        }
    }

    [Test]
    public void NotifyClass()
    {
        var data = new NotifyMessage
        {
            Name = "test1",
            DeviceName = "test2",
            Port = 12345,
        };

        var message3 = Serialization.Serialize(data);

        foreach (var deepClone in Tests<SearchMessage>.Functions)
        {
            var clone = deepClone(data);
            Check(deepClone, data, clone);
        }
    }

    private static void Check(Delegate deepClone, NotifyMessage data, SearchMessage clone)
    {
        var notifyClone = clone as NotifyMessage;
        Assert.That(notifyClone is not null);
        Assert.That(data.Name == clone.Name);
        Assert.That(data.DeviceName == clone.DeviceName);
        Assert.That(data.Port == notifyClone.Port);
    }

    private static void Check(Delegate deepClone, TestData data, TestData clone)
    {
        Assert.That(clone is not null);
        Assert.That(data.Property1 == clone.Property1);
        Assert.That(data.Property2 == clone.Property2);
        Assert.That(data.Property3 == clone.Property3);
        Check(deepClone, data.Property4, clone.Property4);
        Assert.That(data.Property5 == clone.Property5);
        Assert.That(data.Property6 == clone.Property6);
    }

    private static void Check(Delegate deepClone, SealedData data, SealedData clone)
    {
        Assert.That(clone is not null);
        Assert.That(data.Property1 == clone.Property1);
        Assert.That(data.Property2 == clone.Property2);
        Assert.That(data.Property3 == clone.Property3);
        Check(deepClone, data.Property4, clone.Property4);
        Assert.That(data.Property5 == clone.Property5);
    }

    private static void Check(Delegate deepClone, DateTime data, DateTime clone)
    {
        if (deepClone.Method.Name.Contains("InUTC"))
        {
            Assert.That(DateTimeKind.Utc == clone.Kind);
            Assert.That(data == clone.ToLocalTime());
        }
        else
        {
            Assert.That(data == clone);
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
            Property6 = 4,
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

            Assert.That(clone is not null);
            Assert.That(data.Property11 == clone.Property11);
            Assert.That(data.Property31 == clone.Property31);
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

            Assert.That(clone is not null);
            Assert.That(data.Property11 == clone.Property11);
            Assert.That(data.Property31 == clone.Property31);
            Check(deepClone, data.Property32, clone.Property32);

            Assert.That(data.Property33 == clone.Property33);
            Assert.That(data.Property34 == clone.Property34);

            Assert.That(data.Property35.GetType() == clone.Property35?.GetType());
            Assert.That(data.Property35.Property11 == clone.Property35.Property11);
            Assert.That((data.Property35 as TestB3).Property31 == (clone.Property35 as TestB3).Property31);
            Check(deepClone, (data.Property35 as TestB3).Property32, (clone.Property35 as TestB3).Property32);
        }
    }

    [Test]
    public void Ticket()
    {
        var tracksDataTable = new DataTable() { RowCapacity = 1, RowCount = 1 };
        tracksDataTable.AddGuidColumn("Guid").Values.Add(Guid.NewGuid());
        tracksDataTable.AddStringColumn("Title").Values.Add("test");
        tracksDataTable.AddNullableInt32Column("Year").Values.Add(2025);
        tracksDataTable.AddTimeSpanColumn("Duration").Values.Add(new TimeSpan(5, 4, 1));
        tracksDataTable.AddByteColumn("Rating").Values.Add(5);
        tracksDataTable.AddStringColumn("Artist").Values.Add("test2");
        tracksDataTable.AddStringColumn("Genre").Values.Add("test3");
        tracksDataTable.AddStringColumn("Album").Values.Add("test4");
        tracksDataTable.AddDateTimeColumn("Created").Values.Add(DateTime.UtcNow);
        tracksDataTable.AddDateTimeColumn("Added").Values.Add(DateTime.UtcNow);
        tracksDataTable.AddInt32Column("Bitrate").Values.Add(320);
        tracksDataTable.AddInt64Column("Size").Values.Add(54_000);
        tracksDataTable.AddDateTimeColumn("LastWrite").Values.Add(DateTime.UtcNow);
        tracksDataTable.AddBooleanColumn("HasPicture").Values.Add(true);

        var data = new UploadTracksData
        {
            Tracks = tracksDataTable,
        };

        foreach (var deepClone in Tests<SubscriptionTicket>.Functions)
        {
            var clone = deepClone(data) as UploadTracksData;

            Assert.That(clone is not null);
            Assert.That(data.Tracks.RowCount == (clone?.Tracks?.RowCount ?? -1));
            Assert.That(data.Tracks.Count == clone.Tracks.Count);

            Assert.That(((GuidDataColumn)data.Tracks["Guid"]).Values.SequenceEqual(((GuidDataColumn)clone.Tracks["Guid"]).Values));
            Assert.That(((StringDataColumn)data.Tracks["Title"]).Values.SequenceEqual(((StringDataColumn)clone.Tracks["Title"]).Values));
            Assert.That(((NullableInt32DataColumn)data.Tracks["Year"]).Values.SequenceEqual(((NullableInt32DataColumn)clone.Tracks["Year"]).Values));
            Assert.That(((TimeSpanDataColumn)data.Tracks["Duration"]).Values.SequenceEqual(((TimeSpanDataColumn)clone.Tracks["Duration"]).Values));
            Assert.That(((ByteDataColumn)data.Tracks["Rating"]).Values.SequenceEqual(((ByteDataColumn)clone.Tracks["Rating"]).Values));
            Assert.That(((StringDataColumn)data.Tracks["Artist"]).Values.SequenceEqual(((StringDataColumn)clone.Tracks["Artist"]).Values));
            Assert.That(((StringDataColumn)data.Tracks["Genre"]).Values.SequenceEqual(((StringDataColumn)clone.Tracks["Genre"]).Values));
            Assert.That(((StringDataColumn)data.Tracks["Album"]).Values.SequenceEqual(((StringDataColumn)clone.Tracks["Album"]).Values));
            Assert.That(((DateTimeDataColumn)data.Tracks["Created"]).Values.SequenceEqual(((DateTimeDataColumn)clone.Tracks["Created"]).Values));
            Assert.That(((DateTimeDataColumn)data.Tracks["Added"]).Values.SequenceEqual(((DateTimeDataColumn)clone.Tracks["Added"]).Values));
            Assert.That(((Int32DataColumn)data.Tracks["Bitrate"]).Values.SequenceEqual(((Int32DataColumn)clone.Tracks["Bitrate"]).Values));
            Assert.That(((Int64DataColumn)data.Tracks["Size"]).Values.SequenceEqual(((Int64DataColumn)clone.Tracks["Size"]).Values));
            Assert.That(((DateTimeDataColumn)data.Tracks["LastWrite"]).Values.SequenceEqual(((DateTimeDataColumn)clone.Tracks["LastWrite"]).Values));
            Assert.That(((BooleanDataColumn)data.Tracks["HasPicture"]).Values.SequenceEqual(((BooleanDataColumn)clone.Tracks["HasPicture"]).Values));
        }
    }

    [Test]
    public void DataTable()
    {
        var data = new DataTable() { RowCapacity = 1, RowCount = 1 };
        data.AddGuidColumn("Guid").Values.Add(Guid.NewGuid());
        data.AddStringColumn("Title").Values.Add("test");
        data.AddNullableInt32Column("Year").Values.Add(2025);
        data.AddTimeSpanColumn("Duration").Values.Add(new TimeSpan(5, 4, 1));
        data.AddByteColumn("Rating").Values.Add(5);
        data.AddStringColumn("Artist").Values.Add("test2");
        data.AddStringColumn("Genre").Values.Add("test3");
        data.AddStringColumn("Album").Values.Add("test4");
        data.AddDateTimeColumn("Created").Values.Add(DateTime.UtcNow);
        data.AddDateTimeColumn("Added").Values.Add(DateTime.UtcNow);
        data.AddInt32Column("Bitrate").Values.Add(320);
        data.AddInt64Column("Size").Values.Add(54_000);
        data.AddDateTimeColumn("LastWrite").Values.Add(DateTime.UtcNow);
        data.AddBooleanColumn("HasPicture").Values.Add(true);

        foreach (var deepClone in Tests<DataTable>.Functions)
        {
            var clone = deepClone(data);

            Assert.That(clone is not null);
            Assert.That(data.RowCount == (clone?.RowCount ?? -1));
            Assert.That(data.Count == clone.Count);

            Assert.That(((GuidDataColumn)data["Guid"]).Values.SequenceEqual(((GuidDataColumn)clone["Guid"]).Values));
            Assert.That(((StringDataColumn)data["Title"]).Values.SequenceEqual(((StringDataColumn)clone["Title"]).Values));
            Assert.That(((NullableInt32DataColumn)data["Year"]).Values.SequenceEqual(((NullableInt32DataColumn)clone["Year"]).Values));
            Assert.That(((TimeSpanDataColumn)data["Duration"]).Values.SequenceEqual(((TimeSpanDataColumn)clone["Duration"]).Values));
            Assert.That(((ByteDataColumn)data["Rating"]).Values.SequenceEqual(((ByteDataColumn)clone["Rating"]).Values));
            Assert.That(((StringDataColumn)data["Artist"]).Values.SequenceEqual(((StringDataColumn)clone["Artist"]).Values));
            Assert.That(((StringDataColumn)data["Genre"]).Values.SequenceEqual(((StringDataColumn)clone["Genre"]).Values));
            Assert.That(((StringDataColumn)data["Album"]).Values.SequenceEqual(((StringDataColumn)clone["Album"]).Values));
            Assert.That(((DateTimeDataColumn)data["Created"]).Values.SequenceEqual(((DateTimeDataColumn)clone["Created"]).Values));
            Assert.That(((DateTimeDataColumn)data["Added"]).Values.SequenceEqual(((DateTimeDataColumn)clone["Added"]).Values));
            Assert.That(((Int32DataColumn)data["Bitrate"]).Values.SequenceEqual(((Int32DataColumn)clone["Bitrate"]).Values));
            Assert.That(((Int64DataColumn)data["Size"]).Values.SequenceEqual(((Int64DataColumn)clone["Size"]).Values));
            Assert.That(((DateTimeDataColumn)data["LastWrite"]).Values.SequenceEqual(((DateTimeDataColumn)clone["LastWrite"]).Values));
            Assert.That(((BooleanDataColumn)data["HasPicture"]).Values.SequenceEqual(((BooleanDataColumn)clone["HasPicture"]).Values));
        }
    }

}
