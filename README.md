# IcyRain
**Fastest C# data serializer and deserializer** for .NET Core, .NET Framework for use in [Grpc.Core C# library](https://github.com/grpc/grpc/tree/master/src/csharp) with SerializationContext base on **IBufferWriter<byte>** and DeserializationContext base on **ReadOnlySequence<byte>** (see [example](https://github.com/trs4/IcyRain/blob/main/IcyRain.Data/GrpcTestService.cs)). And also supports serialization via **byte[]** or **ArraySegment<byte>**. See [performance](https://github.com/trs4/IcyRain#performance)

Install
---
**Package -> [IcyRain](https://www.nuget.org/packages/IcyRain/)** 

[![Nuget](https://img.shields.io/nuget/v/IcyRain.svg)](http://www.nuget.org/packages/IcyRain/)

Quick start
---
**1 Use the contract definition** as in WCF with mark **[DataContract]** on classes and **[DataMember(Order = PropertyNumber)]** on properties with required parameter Order. Order value must be unique on the properties of the class.

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
* Inherited classes must be marked with attributes **[KnownType(typeof(TestData2))]** in the base class.

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

// IBufferWriter<byte> & ReadOnlySequence<byte> serialization
IBufferWriter<byte> buffer = ...;
Serialization.Serialize(buffer, data);
ReadOnlySequence<byte> sequence = ...;
TestData sequenceClone = Serialization.Deserialize<TestData>(sequence);
```

DateTime
---
**DateTime** are serialized in UTC format with time zone and deserialized with time zone. In order to deserialize a **DateTime** in a particular time zone need use **DeserializeOptions**
```csharp
T Deserialize<T>(in ReadOnlySequence<byte> sequence, DeserializeOptions options = null)
```
```csharp
public sealed class DeserializeOptions
{
    public DeserializeOptions(DateTimeKind dateTimeZone)
        => DateTimeZone = dateTimeZone;

    public DateTimeKind DateTimeZone { get; }
}
```

Performance
---
Deep clone = serialize + deserialize

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.14393.4169 (1607/AnniversaryUpdate/Redstone1)
Intel Core i7-4770K CPU 3.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3415997 Hz, Resolution=292.7403 ns, Timer=TSC
  [Host]     : .NET Framework 4.8 (4.8.4311.0), X64 RyuJIT
  Job-SKJJXH : .NET Core 5.0.2 (CoreCLR 5.0.220.61120, CoreFX 5.0.220.61120), X64 RyuJIT

Runtime=.NET Core 5.0  Toolchain=netcoreapp50  

* **Int:**
```csharp
|          Method | Categories |        Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------- |----------- |------------:|---------:|---------:|-------:|------:|------:|----------:|
|         IcyRain |  Serialize |    18.41 ns | 0.286 ns | 0.477 ns | 0.0076 |     - |     - |      32 B |
|   ZeroFormatter |  Serialize |    38.08 ns | 0.151 ns | 0.142 ns | 0.0153 |     - |     - |      64 B |
|    protobuf-net |  Serialize |   132.05 ns | 0.479 ns | 0.374 ns | 0.0362 |     - |     - |     152 B |
| Google.Protobuf |  Serialize |   858.70 ns | 4.957 ns | 4.636 ns | 1.9455 |     - |     - |    8152 B |
|     MessagePack |  Serialize |   901.08 ns | 3.875 ns | 3.435 ns | 1.9379 |     - |     - |    8120 B |
|                 |            |             |          |          |        |       |       |           |
|         IcyRain | Deep clone |    50.81 ns | 0.896 ns | 0.838 ns | 0.0153 |     - |     - |      64 B |
|   ZeroFormatter | Deep clone |    56.62 ns | 0.152 ns | 0.142 ns | 0.0153 |     - |     - |      64 B |
|    protobuf-net | Deep clone |   359.64 ns | 2.009 ns | 1.781 ns | 0.0362 |     - |     - |     152 B |
| Google.Protobuf | Deep clone |   940.40 ns | 6.941 ns | 5.796 ns | 1.9569 |     - |     - |    8184 B |
|     MessagePack | Deep clone | 1,034.78 ns | 5.574 ns | 5.214 ns | 1.9379 |     - |     - |    8120 B |
```

* **:**
```csharp
```

* **:**
```csharp
```







