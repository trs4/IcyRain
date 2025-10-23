using System.Runtime.Serialization;

namespace IcyRain.Data.Objects;

[DataContract, KnownType(typeof(NotifyMessage))]
public class SearchMessage
{
    [DataMember(Order = 1)]
    public string Name { get; set; }

    [DataMember(Order = 2)]
    public string DeviceName { get; set; }
}

[DataContract]
public class NotifyMessage : SearchMessage
{
    [DataMember(Order = 3)]
    public int Port { get; set; }
}
