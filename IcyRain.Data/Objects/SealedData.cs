using System;
using System.Runtime.Serialization;

namespace IcyRain.Data.Objects
{
    [DataContract]
    public sealed class SealedData
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
}
