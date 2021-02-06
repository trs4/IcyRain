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

Serialization use IBufferWriter<byte> and ReadOnlySequence<byte>

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

* **List of int:**
```csharp
|        Method | Categories |        Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |----------- |------------:|----------:|----------:|-------:|------:|------:|----------:|
|       IcyRain |  Serialize |    461.2 ns |   2.64 ns |   2.20 ns | 0.9708 |     - |     - |   3.97 KB |
| ZeroFormatter |  Serialize |  6,008.9 ns |  60.85 ns |  53.94 ns | 1.9302 |     - |     - |   7.91 KB |
|   MessagePack |  Serialize | 10,425.3 ns | 108.92 ns | 101.88 ns | 1.9684 |     - |     - |   8.05 KB |
|  protobuf-net |  Serialize | 27,303.2 ns | 256.27 ns | 239.72 ns | 1.3733 |     - |     - |   5.67 KB |
|               |            |             |           |           |        |       |       |           |
|       IcyRain | Deep clone |  2,048.2 ns |  30.79 ns |  28.80 ns | 3.8872 |     - |     - |  15.89 KB |
| ZeroFormatter | Deep clone | 11,086.4 ns |  84.81 ns |  79.33 ns | 2.8992 |     - |     - |  11.87 KB |
|  protobuf-net | Deep clone | 28,310.6 ns | 145.89 ns | 121.82 ns | 1.4038 |     - |     - |   5.78 KB |
|   MessagePack | Deep clone | 29,403.0 ns | 158.36 ns | 140.38 ns | 2.9297 |     - |     - |  12.02 KB |
```

* **List of string:**
```csharp
|        Method | Categories |      Mean |     Error |    StdDev |   Gen 0 |  Gen 1 | Gen 2 | Allocated |
|-------------- |----------- |----------:|----------:|----------:|--------:|-------:|------:|----------:|
|       IcyRain |  Serialize |  5.275 μs | 0.0935 μs | 0.0874 μs |  3.5706 |      - |     - |   14952 B |
|   MessagePack |  Serialize | 18.517 μs | 0.3334 μs | 0.5384 μs |  2.7161 |      - |     - |   11416 B |
| ZeroFormatter |  Serialize | 25.112 μs | 0.3311 μs | 0.3097 μs | 12.8174 |      - |     - |   53640 B |
|  protobuf-net |  Serialize | 47.067 μs | 0.4627 μs | 0.4102 μs |  2.9907 |      - |     - |   12648 B |
|               |            |           |           |           |         |        |       |           |
|       IcyRain | Deep clone | 22.053 μs | 0.4278 μs | 0.6270 μs | 17.7002 | 1.7700 |     - |   74304 B |
|  protobuf-net | Deep clone | 50.857 μs | 0.9976 μs | 1.8980 μs |  3.0518 |      - |     - |   12864 B |
| ZeroFormatter | Deep clone | 51.441 μs | 0.4688 μs | 0.3915 μs | 19.5313 |      - |     - |   81744 B |
|   MessagePack | Deep clone |        NA |        NA |        NA |       - |      - |     - |         - |
```

* **List of DateTime:**
```csharp
|        Method | Categories |      Mean |     Error |    StdDev |    Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |----------- |----------:|----------:|----------:|----------:|-------:|------:|------:|----------:|
|       IcyRain |  Serialize |  3.437 μs | 0.0117 μs | 0.0104 μs |  3.438 μs | 1.5640 |     - |     - |    6560 B |
| ZeroFormatter |  Serialize |  6.503 μs | 0.0345 μs | 0.0306 μs |  6.507 μs | 2.8839 |     - |     - |   12096 B |
|   MessagePack |  Serialize |  7.279 μs | 0.0153 μs | 0.0143 μs |  7.280 μs | 1.9684 |     - |     - |    8248 B |
|  protobuf-net |  Serialize | 83.675 μs | 1.6433 μs | 2.9632 μs | 81.573 μs | 1.4648 |     - |     - |    6416 B |
|               |            |           |           |           |           |        |       |       |           |
|       IcyRain | Deep clone |  8.117 μs | 0.0343 μs | 0.0321 μs |  8.110 μs | 5.0964 |     - |     - |   21360 B |
| ZeroFormatter | Deep clone | 15.392 μs | 0.0495 μs | 0.0413 μs | 15.374 μs | 3.8452 |     - |     - |   16152 B |
|   MessagePack | Deep clone | 21.686 μs | 0.0701 μs | 0.0621 μs | 21.673 μs | 2.9297 |     - |     - |   12304 B |
|  protobuf-net | Deep clone |        NA |        NA |        NA |        NA |      - |     - |     - |         - |
```

* **List of Double:**
```csharp
|        Method | Categories |        Mean |     Error |    StdDev |      Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |----------- |------------:|----------:|----------:|------------:|-------:|------:|------:|----------:|
|       IcyRain |  Serialize |    467.7 ns |   3.92 ns |   3.47 ns |    467.1 ns | 0.9708 |     - |     - |   3.97 KB |
| ZeroFormatter |  Serialize |  5,482.3 ns |  27.28 ns |  24.18 ns |  5,481.5 ns | 1.9302 |     - |     - |   7.91 KB |
|   MessagePack |  Serialize |  5,767.1 ns | 109.46 ns | 126.05 ns |  5,723.5 ns | 1.9684 |     - |     - |   8.05 KB |
|  protobuf-net |  Serialize | 10,966.8 ns |  98.73 ns |  92.35 ns | 10,929.8 ns | 1.3123 |     - |     - |   5.38 KB |
|               |            |             |           |           |             |        |       |       |           |
|       IcyRain | Deep clone |  2,089.6 ns |  32.40 ns |  27.05 ns |  2,077.7 ns | 3.8872 |     - |     - |  15.89 KB |
| ZeroFormatter | Deep clone | 10,768.0 ns |  43.22 ns |  36.09 ns | 10,760.1 ns | 2.8992 |     - |     - |  11.87 KB |
|  protobuf-net | Deep clone | 12,049.9 ns | 213.28 ns | 291.94 ns | 11,890.3 ns | 1.3428 |     - |     - |   5.54 KB |
|   MessagePack | Deep clone | 12,960.6 ns |  47.61 ns |  42.21 ns | 12,955.8 ns | 2.9297 |     - |     - |  12.02 KB |
```

* **Class with DataContract:**
```csharp
on net5.0:
|        Method |        Job |       Runtime |    Toolchain | Categories |       Mean |    Error |  StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |----------- |-------------- |------------- |----------- |-----------:|---------:|--------:|------:|--------:|-------:|------:|------:|----------:|
|       IcyRain | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 |  Serialize |   157.7 ns |  0.49 ns | 0.46 ns |  0.73 |    0.00 | 0.0248 |     - |     - |     104 B |
|  protobuf-net | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 |  Serialize |   432.0 ns |  1.60 ns | 1.50 ns |  0.76 |    0.00 | 0.0439 |     - |     - |     184 B |
|   MessagePack | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 |  Serialize |   866.7 ns |  2.53 ns | 2.11 ns |  0.84 |    0.01 | 1.9684 |     - |     - |    8248 B |
|       IcyRain | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 | Deep clone |   383.6 ns |  1.23 ns | 1.09 ns |  0.67 |    0.00 | 0.0744 |     - |     - |     312 B |
|  protobuf-net | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 | Deep clone |   944.4 ns |  3.41 ns | 3.02 ns |  0.65 |    0.00 | 0.0629 |     - |     - |     264 B |
|   MessagePack | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 | Deep clone | 1,166.0 ns |  4.41 ns | 3.68 ns |  0.73 |    0.00 | 1.9875 |     - |     - |    8328 B |

on net472:
|        Method | Categories |       Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |----------- |-----------:|---------:|---------:|-------:|------:|------:|----------:|
|       IcyRain |  Serialize |   207.5 ns |  3.31 ns |  3.68 ns | 0.0248 |     - |     - |     104 B |
| ZeroFormatter |  Serialize |   312.4 ns |  4.75 ns |  4.45 ns | 0.1259 |     - |     - |     530 B |
|  protobuf-net |  Serialize |   551.2 ns |  4.06 ns |  3.60 ns | 0.0439 |     - |     - |     185 B |
|   MessagePack |  Serialize |   992.8 ns | 13.87 ns | 11.58 ns | 1.9684 |     - |     - |    8272 B |
|               |            |            |          |          |        |       |       |           |
|       IcyRain | Deep clone |   545.8 ns |  7.29 ns |  6.82 ns | 0.0839 |     - |     - |     353 B |
| ZeroFormatter | Deep clone |   691.6 ns |  7.17 ns |  6.71 ns | 0.2003 |     - |     - |     842 B |
|   MessagePack | Deep clone | 1,559.8 ns | 15.50 ns | 13.74 ns | 1.9913 |     - |     - |    8371 B |
|  protobuf-net | Deep clone | 1,593.7 ns | 31.19 ns | 34.67 ns | 0.0648 |     - |     - |     273 B |
```

* **HashSet of int:**
```csharp
|        Method | Categories |      Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |----------- |----------:|----------:|----------:|-------:|------:|------:|----------:|
|       IcyRain |  Serialize |  1.013 μs | 0.0164 μs | 0.0154 μs | 0.2060 |     - |     - |     864 B |
| ZeroFormatter |  Serialize |  2.459 μs | 0.0468 μs | 0.0592 μs | 0.4120 |     - |     - |    1736 B |
|   MessagePack |  Serialize |  3.493 μs | 0.0157 μs | 0.0139 μs | 1.9684 |     - |     - |    8248 B |
|  protobuf-net |  Serialize |  5.892 μs | 0.0267 μs | 0.0236 μs | 0.2670 |     - |     - |    1136 B |
|               |            |           |           |           |        |       |       |           |
|       IcyRain | Deep clone |  4.373 μs | 0.0174 μs | 0.0145 μs | 1.5945 |     - |     - |    6696 B |
|  protobuf-net | Deep clone |  6.502 μs | 0.0200 μs | 0.0177 μs | 0.3128 |     - |     - |    1312 B |
| ZeroFormatter | Deep clone |  7.430 μs | 0.0530 μs | 0.0470 μs | 3.5095 |     - |     - |   14697 B |
|   MessagePack | Deep clone | 12.236 μs | 0.0399 μs | 0.0354 μs | 5.0659 |     - |     - |   21208 B |
```

* **Dictionary with int key and string value:**
```csharp
|        Method |        Job |       Runtime |    Toolchain | Categories |      Mean |     Error |    StdDev | Ratio | RatioSD |   Gen 0 |  Gen 1 | Gen 2 | Allocated |
|-------------- |----------- |-------------- |------------- |----------- |----------:|----------:|----------:|------:|--------:|--------:|-------:|------:|----------:|
|  protobuf-net | Job-LBIENB |    .NET 4.7.2 |       net472 | Deep clone |        NA |        NA |        NA |     ? |       ? |       - |      - |     - |         - |
|  protobuf-net | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 | Deep clone | 57.991 μs | 1.1252 μs | 1.5773 μs |     ? |       ? |  1.0376 |      - |     - |    4584 B |
|               |            |               |              |            |           |           |           |       |         |         |        |       |           |
|       IcyRain | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 |  Serialize |  4.781 μs | 0.0867 μs | 0.1216 μs |  0.66 |    0.02 |  1.2360 |      - |     - |    5200 B |
|       IcyRain | Job-LBIENB |    .NET 4.7.2 |       net472 |  Serialize |  7.353 μs | 0.0509 μs | 0.0397 μs |  1.00 |    0.00 |  1.2436 |      - |     - |    5243 B |
|               |            |               |              |            |           |           |           |       |         |         |        |       |           |
|       IcyRain | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 | Deep clone | 18.962 μs | 0.3733 μs | 0.6539 μs |  0.74 |    0.03 |  7.9956 | 0.5188 |     - |   33456 B |
|       IcyRain | Job-LBIENB |    .NET 4.7.2 |       net472 | Deep clone | 25.558 μs | 0.1681 μs | 0.1572 μs |  1.00 |    0.00 | 10.6201 |      - |     - |   44574 B |
|               |            |               |              |            |           |           |           |       |         |         |        |       |           |
| ZeroFormatter | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 |  Serialize | 13.338 μs | 0.2632 μs | 0.4251 μs |  0.74 |    0.03 |  3.5706 |      - |     - |   14936 B |
| ZeroFormatter | Job-LBIENB |    .NET 4.7.2 |       net472 |  Serialize | 18.476 μs | 0.3302 μs | 0.2927 μs |  1.00 |    0.00 |  3.5706 |      - |     - |   14983 B |
|               |            |               |              |            |           |           |           |       |         |         |        |       |           |
| ZeroFormatter | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 | Deep clone | 28.635 μs | 0.1558 μs | 0.1381 μs |  0.69 |    0.01 |  7.1106 |      - |     - |   29808 B |
| ZeroFormatter | Job-LBIENB |    .NET 4.7.2 |       net472 | Deep clone | 41.562 μs | 0.5594 μs | 0.5233 μs |  1.00 |    0.00 |  7.4463 |      - |     - |   31509 B |
|               |            |               |              |            |           |           |           |       |         |         |        |       |           |
|   MessagePack | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 |  Serialize |  9.861 μs | 0.0561 μs | 0.0438 μs |  0.49 |    0.00 |  1.9684 |      - |     - |    8248 B |
|   MessagePack | Job-LBIENB |    .NET 4.7.2 |       net472 |  Serialize | 19.951 μs | 0.0919 μs | 0.0718 μs |  1.00 |    0.00 |  1.9531 |      - |     - |    8272 B |
|               |            |               |              |            |           |           |           |       |         |         |        |       |           |
|   MessagePack | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 | Deep clone | 26.304 μs | 0.2359 μs | 0.2092 μs |  0.50 |    0.01 |  5.4932 | 0.0305 |     - |   23072 B |
|   MessagePack | Job-LBIENB |    .NET 4.7.2 |       net472 | Deep clone | 52.847 μs | 0.3498 μs | 0.3101 μs |  1.00 |    0.00 |  5.8594 |      - |     - |   24752 B |
|               |            |               |              |            |           |           |           |       |         |         |        |       |           |
|  protobuf-net | Job-SKJJXH | .NET Core 5.0 | netcoreapp50 |  Serialize | 56.799 μs | 0.3132 μs | 0.2777 μs |  0.71 |    0.01 |  0.9766 |      - |     - |    4328 B |
|  protobuf-net | Job-LBIENB |    .NET 4.7.2 |       net472 |  Serialize | 79.973 μs | 0.6766 μs | 0.6329 μs |  1.00 |    0.00 |  0.9766 |      - |     - |    4189 B |
```

* **Bool:**
```csharp
|          Method | Categories |        Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------- |----------- |------------:|----------:|----------:|-------:|------:|------:|----------:|
|         IcyRain |  Serialize |    19.54 ns |  0.106 ns |  0.094 ns | 0.0076 |     - |     - |      32 B |
|   ZeroFormatter |  Serialize |    37.42 ns |  0.202 ns |  0.189 ns | 0.0153 |     - |     - |      64 B |
|    protobuf-net |  Serialize |   136.33 ns |  2.436 ns |  3.080 ns | 0.0362 |     - |     - |     152 B |
| Google.Protobuf |  Serialize |   889.98 ns | 10.456 ns |  9.780 ns | 1.9455 |     - |     - |    8152 B |
|     MessagePack |  Serialize |   945.22 ns | 17.413 ns | 16.288 ns | 1.9379 |     - |     - |    8120 B |
|                 |            |             |           |           |        |       |       |           |
|         IcyRain | Deep clone |    40.86 ns |  0.363 ns |  0.303 ns | 0.0153 |     - |     - |      64 B |
|   ZeroFormatter | Deep clone |    55.53 ns |  0.680 ns |  0.568 ns | 0.0153 |     - |     - |      64 B |
|    protobuf-net | Deep clone |   361.86 ns |  4.269 ns |  3.784 ns | 0.0362 |     - |     - |     152 B |
| Google.Protobuf | Deep clone |   987.89 ns | 15.617 ns | 21.893 ns | 1.9569 |     - |     - |    8184 B |
|     MessagePack | Deep clone | 1,035.23 ns |  9.534 ns |  7.961 ns | 1.9379 |     - |     - |    8120 B |
```

* **Byte array:**
```csharp
|          Method | Categories |       Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------- |----------- |-----------:|---------:|---------:|-------:|------:|------:|----------:|
|         IcyRain |  Serialize |   138.9 ns |  0.87 ns |  0.81 ns | 0.2580 |     - |     - |    1080 B |
|   ZeroFormatter |  Serialize |   296.0 ns |  2.74 ns |  2.56 ns | 0.5121 |     - |     - |    2144 B |
|    protobuf-net |  Serialize |   566.7 ns | 11.37 ns | 17.36 ns | 0.3347 |     - |     - |    1400 B |
|     MessagePack |  Serialize | 1,024.9 ns |  8.44 ns |  7.48 ns | 1.9684 |     - |     - |    8248 B |
| Google.Protobuf |  Serialize | 1,090.5 ns | 12.45 ns | 11.04 ns | 2.2106 |     - |     - |    9256 B |
|                 |            |            |          |          |        |       |       |           |
|    protobuf-net | Deep clone |         NA |       NA |       NA |      - |     - |     - |         - |
|         IcyRain | Deep clone |   145.3 ns |  2.06 ns |  1.93 ns | 0.2580 |     - |     - |    1080 B |
|   ZeroFormatter | Deep clone |   459.3 ns |  5.77 ns |  5.40 ns | 0.7629 |     - |     - |    3192 B |
| Google.Protobuf | Deep clone | 1,329.6 ns | 10.73 ns | 10.03 ns | 2.4738 |     - |     - |   10360 B |
|     MessagePack | Deep clone | 1,372.9 ns |  8.93 ns |  7.91 ns | 2.2221 |     - |     - |    9296 B |
```
