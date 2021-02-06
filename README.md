# IcyRain
**Fastest C# data serializer and deserializer** for .NET Core, .NET Framework for use in [Grpc.Core C# library](https://github.com/grpc/grpc/tree/master/src/csharp) with SerializationContext base on **IBufferWriter<byte>** and DeserializationContext base on **ReadOnlySequence<byte>** (see [example](https://github.com/trs4/IcyRain/blob/main/IcyRain.Data/GrpcTestService.cs)). And also supports serialization via **byte[]** or **ArraySegment<byte>**.

Install
---
Package -> [IcyRain](https://www.nuget.org/packages/IcyRain/)

Quick start
---
**1 Use the contract definition** as in WCF with mark *[DataContract]* on classes and *[DataMember(Order = PropertyNumber)]* on properties with required parameter Order. Order value must be unique on the properties of the class.

```csharp
[DataContract]
public class TestData
{
    [DataMember(Order = 1)]
    public bool Property1 { get; set; }

    [DataMember(Order = 2)]
    public int Property2 { get; set; }

    [DataMember(Order = 3)]
    public double Property3 { get; set; }

    [DataMember(Order = 4)]
    public DateTime Property4 { get; set; }

    [DataMember(Order = 5)]
    public string Property5 { get; set; }
}
```
**2 Serialize / Deserialize data**

```csharp
var data = new TestData
{
    Property1 = true,
    Property2 = 25,
    Property3 = 7.0,
    Property4 = DateTime.Now,
    Property5 = "test"
};

// byte array serialization
byte[] bytes = Serialization.Serialize(data);
TestData bytesClone = Serialization.Deserialize<TestData>(bytes);

// array segment serialization
ArraySegment<byte> segment = Serialization.SerializeSegment(data);
TestData segmentClone = Serialization.DeserializeSegment<TestData>(segment);

// IBufferWriter<byte> & ReadOnlySequence<byte>
IBufferWriter<byte> buffer = null;
Serialization.Serialize(buffer, data);
ReadOnlySequence<byte> sequence = default;
TestData sequenceClone = Serialization.Deserialize<TestData>(sequence);
```

