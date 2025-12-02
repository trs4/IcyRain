using System.Runtime.Serialization;

namespace IcyRain.Grpc.Tests.Data;

[DataContract]
public class UnaryRequest
{
    [DataMember(Order = 1)]
    public string Name { get; set; }
}

[DataContract]
public class UnaryResponse
{
    [DataMember(Order = 1)]
    public string Description { get; set; }

    [DataMember(Order = 2)]
    public int Complete { get; set; }
}

[DataContract]
public class StreamRequest
{
    [DataMember(Order = 1)]
    public string Name { get; set; }
}

[DataContract]
public class StreamResponse
{
    [DataMember(Order = 1)]
    public string Description { get; set; }

    [DataMember(Order = 2)]
    public int Number { get; set; }
}
