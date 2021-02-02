using System;
using System.Linq;
using System.Runtime.Serialization;

namespace IcyRain.Internal
{
    internal sealed class ContractData
    {
        public readonly bool HasContract;
        public readonly bool HasCollectionContract;
        public readonly KnownTypeAttribute[] KnownTypes;

        public ContractData(Type type)
        {
            object[] attributes = type.GetCustomAttributes(false);

            HasContract = attributes.OfType<DataContractAttribute>().Any();
            HasCollectionContract = attributes.OfType<CollectionDataContractAttribute>().Any();
            KnownTypes = attributes.OfType<KnownTypeAttribute>().ToArray();
        }

    }
}
