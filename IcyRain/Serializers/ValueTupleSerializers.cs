using System;
using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Serializers
{
    internal sealed class ValueTupleSerializer<TResolver, T1> : Serializer<TResolver, ValueTuple<T1>>
        where TResolver : Resolver
    {
        private readonly int? _size;
        private readonly Serializer<TResolver, T1> _serializer1 = Serializer<TResolver, T1>.Instance;

        public ValueTupleSerializer()
        {
            int? size1 = _serializer1.GetSize();

            if (size1.HasValue)
                _size = size1.Value;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => _size;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(ValueTuple<T1> value)
            => _size ?? _serializer1.GetCapacity(value.Item1);

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, ValueTuple<T1> value)
            => _serializer1.Serialize(ref writer, value.Item1);

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, ValueTuple<T1> value)
            => _serializer1.Serialize(ref writer, value.Item1);

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1> Deserialize(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            return new ValueTuple<T1>(item1);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1> DeserializeInUTC(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            return new ValueTuple<T1>(item1);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1> DeserializeSpot(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            return new ValueTuple<T1>(item1);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1> DeserializeInUTCSpot(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            return new ValueTuple<T1>(item1);
        }

    }

    internal sealed class ValueTupleSerializer<TResolver, T1, T2> : Serializer<TResolver, ValueTuple<T1, T2>>
        where TResolver : Resolver
    {
        private readonly int? _size;
        private readonly Serializer<TResolver, T1> _serializer1 = Serializer<TResolver, T1>.Instance;
        private readonly Serializer<TResolver, T2> _serializer2 = Serializer<TResolver, T2>.Instance;

        public ValueTupleSerializer()
        {
            int? size1 = _serializer1.GetSize();
            int? size2 = _serializer2.GetSize();

            if (size1.HasValue && size2.HasValue)
                _size = size1.Value + size2.Value;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => _size;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(ValueTuple<T1, T2> value)
            => _size ?? (_serializer1.GetCapacity(value.Item1) + _serializer2.GetCapacity(value.Item2));

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, ValueTuple<T1, T2> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, ValueTuple<T1, T2> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2> Deserialize(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            return new ValueTuple<T1, T2>(item1, item2);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2> DeserializeInUTC(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2>(item1, item2);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2> DeserializeSpot(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            return new ValueTuple<T1, T2>(item1, item2);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2> DeserializeInUTCSpot(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2>(item1, item2);
        }

    }

    internal sealed class ValueTupleSerializer<TResolver, T1, T2, T3> : Serializer<TResolver, ValueTuple<T1, T2, T3>>
        where TResolver : Resolver
    {
        private readonly int? _size;
        private readonly Serializer<TResolver, T1> _serializer1 = Serializer<TResolver, T1>.Instance;
        private readonly Serializer<TResolver, T2> _serializer2 = Serializer<TResolver, T2>.Instance;
        private readonly Serializer<TResolver, T3> _serializer3 = Serializer<TResolver, T3>.Instance;

        public ValueTupleSerializer()
        {
            int? size1 = _serializer1.GetSize();
            int? size2 = _serializer2.GetSize();
            int? size3 = _serializer3.GetSize();

            if (size1.HasValue && size2.HasValue && size3.HasValue)
                _size = size1.Value + size2.Value + size3.Value;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => _size;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(ValueTuple<T1, T2, T3> value)
            => _size ?? (_serializer1.GetCapacity(value.Item1) + _serializer2.GetCapacity(value.Item2)
            + _serializer3.GetCapacity(value.Item3));

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, ValueTuple<T1, T2, T3> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
            _serializer3.Serialize(ref writer, value.Item3);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, ValueTuple<T1, T2, T3> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
            _serializer3.Serialize(ref writer, value.Item3);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3> Deserialize(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            var item3 = _serializer3.Deserialize(ref reader);
            return new ValueTuple<T1, T2, T3>(item1, item2, item3);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3> DeserializeInUTC(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            var item3 = _serializer3.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2, T3>(item1, item2, item3);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3> DeserializeSpot(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            var item3 = _serializer3.Deserialize(ref reader);
            return new ValueTuple<T1, T2, T3>(item1, item2, item3);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3> DeserializeInUTCSpot(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            var item3 = _serializer3.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2, T3>(item1, item2, item3);
        }

    }

    internal sealed class ValueTupleSerializer<TResolver, T1, T2, T3, T4> : Serializer<TResolver, ValueTuple<T1, T2, T3, T4>>
        where TResolver : Resolver
    {
        private readonly int? _size;
        private readonly Serializer<TResolver, T1> _serializer1 = Serializer<TResolver, T1>.Instance;
        private readonly Serializer<TResolver, T2> _serializer2 = Serializer<TResolver, T2>.Instance;
        private readonly Serializer<TResolver, T3> _serializer3 = Serializer<TResolver, T3>.Instance;
        private readonly Serializer<TResolver, T4> _serializer4 = Serializer<TResolver, T4>.Instance;

        public ValueTupleSerializer()
        {
            int? size1 = _serializer1.GetSize();
            int? size2 = _serializer2.GetSize();
            int? size3 = _serializer3.GetSize();
            int? size4 = _serializer4.GetSize();

            if (size1.HasValue && size2.HasValue && size3.HasValue && size4.HasValue)
                _size = size1.Value + size2.Value + size3.Value + size4.Value;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => _size;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(ValueTuple<T1, T2, T3, T4> value)
            => _size ?? (_serializer1.GetCapacity(value.Item1) + _serializer2.GetCapacity(value.Item2)
            + _serializer3.GetCapacity(value.Item3) + _serializer4.GetCapacity(value.Item4));

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, ValueTuple<T1, T2, T3, T4> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
            _serializer3.Serialize(ref writer, value.Item3);
            _serializer4.Serialize(ref writer, value.Item4);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, ValueTuple<T1, T2, T3, T4> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
            _serializer3.Serialize(ref writer, value.Item3);
            _serializer4.Serialize(ref writer, value.Item4);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4> Deserialize(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            var item3 = _serializer3.Deserialize(ref reader);
            var item4 = _serializer4.Deserialize(ref reader);
            return new ValueTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4> DeserializeInUTC(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            var item3 = _serializer3.DeserializeInUTC(ref reader);
            var item4 = _serializer4.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4> DeserializeSpot(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            var item3 = _serializer3.Deserialize(ref reader);
            var item4 = _serializer4.Deserialize(ref reader);
            return new ValueTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4> DeserializeInUTCSpot(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            var item3 = _serializer3.DeserializeInUTC(ref reader);
            var item4 = _serializer4.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2, T3, T4>(item1, item2, item3, item4);
        }

    }

    internal sealed class ValueTupleSerializer<TResolver, T1, T2, T3, T4, T5> : Serializer<TResolver, ValueTuple<T1, T2, T3, T4, T5>>
        where TResolver : Resolver
    {
        private readonly int? _size;
        private readonly Serializer<TResolver, T1> _serializer1 = Serializer<TResolver, T1>.Instance;
        private readonly Serializer<TResolver, T2> _serializer2 = Serializer<TResolver, T2>.Instance;
        private readonly Serializer<TResolver, T3> _serializer3 = Serializer<TResolver, T3>.Instance;
        private readonly Serializer<TResolver, T4> _serializer4 = Serializer<TResolver, T4>.Instance;
        private readonly Serializer<TResolver, T5> _serializer5 = Serializer<TResolver, T5>.Instance;

        public ValueTupleSerializer()
        {
            int? size1 = _serializer1.GetSize();
            int? size2 = _serializer2.GetSize();
            int? size3 = _serializer3.GetSize();
            int? size4 = _serializer4.GetSize();
            int? size5 = _serializer5.GetSize();

            if (size1.HasValue && size2.HasValue && size3.HasValue && size4.HasValue && size5.HasValue)
                _size = size1.Value + size2.Value + size3.Value + size4.Value + size5.Value;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => _size;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(ValueTuple<T1, T2, T3, T4, T5> value)
            => _size ?? (_serializer1.GetCapacity(value.Item1) + _serializer2.GetCapacity(value.Item2)
            + _serializer3.GetCapacity(value.Item3) + _serializer4.GetCapacity(value.Item4)
            + _serializer5.GetCapacity(value.Item5));

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, ValueTuple<T1, T2, T3, T4, T5> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
            _serializer3.Serialize(ref writer, value.Item3);
            _serializer4.Serialize(ref writer, value.Item4);
            _serializer5.Serialize(ref writer, value.Item5);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, ValueTuple<T1, T2, T3, T4, T5> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
            _serializer3.Serialize(ref writer, value.Item3);
            _serializer4.Serialize(ref writer, value.Item4);
            _serializer5.Serialize(ref writer, value.Item5);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5> Deserialize(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            var item3 = _serializer3.Deserialize(ref reader);
            var item4 = _serializer4.Deserialize(ref reader);
            var item5 = _serializer5.Deserialize(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5> DeserializeInUTC(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            var item3 = _serializer3.DeserializeInUTC(ref reader);
            var item4 = _serializer4.DeserializeInUTC(ref reader);
            var item5 = _serializer5.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5> DeserializeSpot(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            var item3 = _serializer3.Deserialize(ref reader);
            var item4 = _serializer4.Deserialize(ref reader);
            var item5 = _serializer5.Deserialize(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5> DeserializeInUTCSpot(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            var item3 = _serializer3.DeserializeInUTC(ref reader);
            var item4 = _serializer4.DeserializeInUTC(ref reader);
            var item5 = _serializer5.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);
        }

    }

    internal sealed class ValueTupleSerializer<TResolver, T1, T2, T3, T4, T5, T6> : Serializer<TResolver, ValueTuple<T1, T2, T3, T4, T5, T6>>
        where TResolver : Resolver
    {
        private readonly int? _size;
        private readonly Serializer<TResolver, T1> _serializer1 = Serializer<TResolver, T1>.Instance;
        private readonly Serializer<TResolver, T2> _serializer2 = Serializer<TResolver, T2>.Instance;
        private readonly Serializer<TResolver, T3> _serializer3 = Serializer<TResolver, T3>.Instance;
        private readonly Serializer<TResolver, T4> _serializer4 = Serializer<TResolver, T4>.Instance;
        private readonly Serializer<TResolver, T5> _serializer5 = Serializer<TResolver, T5>.Instance;
        private readonly Serializer<TResolver, T6> _serializer6 = Serializer<TResolver, T6>.Instance;

        public ValueTupleSerializer()
        {
            int? size1 = _serializer1.GetSize();
            int? size2 = _serializer2.GetSize();
            int? size3 = _serializer3.GetSize();
            int? size4 = _serializer4.GetSize();
            int? size5 = _serializer5.GetSize();
            int? size6 = _serializer6.GetSize();

            if (size1.HasValue && size2.HasValue && size3.HasValue && size4.HasValue && size5.HasValue && size6.HasValue)
                _size = size1.Value + size2.Value + size3.Value + size4.Value + size5.Value + size6.Value;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => _size;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(ValueTuple<T1, T2, T3, T4, T5, T6> value)
            => _size ?? (_serializer1.GetCapacity(value.Item1) + _serializer2.GetCapacity(value.Item2)
            + _serializer3.GetCapacity(value.Item3) + _serializer4.GetCapacity(value.Item4)
            + _serializer5.GetCapacity(value.Item5) + _serializer6.GetCapacity(value.Item6));

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
            _serializer3.Serialize(ref writer, value.Item3);
            _serializer4.Serialize(ref writer, value.Item4);
            _serializer5.Serialize(ref writer, value.Item5);
            _serializer6.Serialize(ref writer, value.Item6);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, ValueTuple<T1, T2, T3, T4, T5, T6> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
            _serializer3.Serialize(ref writer, value.Item3);
            _serializer4.Serialize(ref writer, value.Item4);
            _serializer5.Serialize(ref writer, value.Item5);
            _serializer6.Serialize(ref writer, value.Item6);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5, T6> Deserialize(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            var item3 = _serializer3.Deserialize(ref reader);
            var item4 = _serializer4.Deserialize(ref reader);
            var item5 = _serializer5.Deserialize(ref reader);
            var item6 = _serializer6.Deserialize(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5, T6> DeserializeInUTC(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            var item3 = _serializer3.DeserializeInUTC(ref reader);
            var item4 = _serializer4.DeserializeInUTC(ref reader);
            var item5 = _serializer5.DeserializeInUTC(ref reader);
            var item6 = _serializer6.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5, T6> DeserializeSpot(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            var item3 = _serializer3.Deserialize(ref reader);
            var item4 = _serializer4.Deserialize(ref reader);
            var item5 = _serializer5.Deserialize(ref reader);
            var item6 = _serializer6.Deserialize(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5, T6> DeserializeInUTCSpot(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            var item3 = _serializer3.DeserializeInUTC(ref reader);
            var item4 = _serializer4.DeserializeInUTC(ref reader);
            var item5 = _serializer5.DeserializeInUTC(ref reader);
            var item6 = _serializer6.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5, T6>(item1, item2, item3, item4, item5, item6);
        }

    }

    internal sealed class ValueTupleSerializer<TResolver, T1, T2, T3, T4, T5, T6, T7> : Serializer<TResolver, ValueTuple<T1, T2, T3, T4, T5, T6, T7>>
        where TResolver : Resolver
    {
        private readonly int? _size;
        private readonly Serializer<TResolver, T1> _serializer1 = Serializer<TResolver, T1>.Instance;
        private readonly Serializer<TResolver, T2> _serializer2 = Serializer<TResolver, T2>.Instance;
        private readonly Serializer<TResolver, T3> _serializer3 = Serializer<TResolver, T3>.Instance;
        private readonly Serializer<TResolver, T4> _serializer4 = Serializer<TResolver, T4>.Instance;
        private readonly Serializer<TResolver, T5> _serializer5 = Serializer<TResolver, T5>.Instance;
        private readonly Serializer<TResolver, T6> _serializer6 = Serializer<TResolver, T6>.Instance;
        private readonly Serializer<TResolver, T7> _serializer7 = Serializer<TResolver, T7>.Instance;

        public ValueTupleSerializer()
        {
            int? size1 = _serializer1.GetSize();
            int? size2 = _serializer2.GetSize();
            int? size3 = _serializer3.GetSize();
            int? size4 = _serializer4.GetSize();
            int? size5 = _serializer5.GetSize();
            int? size6 = _serializer6.GetSize();
            int? size7 = _serializer7.GetSize();

            if (size1.HasValue && size2.HasValue && size3.HasValue && size4.HasValue && size5.HasValue && size6.HasValue && size7.HasValue)
                _size = size1.Value + size2.Value + size3.Value + size4.Value + size5.Value + size6.Value + size7.Value;
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => _size;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
            => _size ?? (_serializer1.GetCapacity(value.Item1) + _serializer2.GetCapacity(value.Item2)
            + _serializer3.GetCapacity(value.Item3) + _serializer4.GetCapacity(value.Item4)
            + _serializer5.GetCapacity(value.Item5) + _serializer6.GetCapacity(value.Item6)
            + _serializer7.GetCapacity(value.Item7));

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
            _serializer3.Serialize(ref writer, value.Item3);
            _serializer4.Serialize(ref writer, value.Item4);
            _serializer5.Serialize(ref writer, value.Item5);
            _serializer6.Serialize(ref writer, value.Item6);
            _serializer7.Serialize(ref writer, value.Item7);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
            _serializer3.Serialize(ref writer, value.Item3);
            _serializer4.Serialize(ref writer, value.Item4);
            _serializer5.Serialize(ref writer, value.Item5);
            _serializer6.Serialize(ref writer, value.Item6);
            _serializer7.Serialize(ref writer, value.Item7);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5, T6, T7> Deserialize(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            var item3 = _serializer3.Deserialize(ref reader);
            var item4 = _serializer4.Deserialize(ref reader);
            var item5 = _serializer5.Deserialize(ref reader);
            var item6 = _serializer6.Deserialize(ref reader);
            var item7 = _serializer7.Deserialize(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5, T6, T7> DeserializeInUTC(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            var item3 = _serializer3.DeserializeInUTC(ref reader);
            var item4 = _serializer4.DeserializeInUTC(ref reader);
            var item5 = _serializer5.DeserializeInUTC(ref reader);
            var item6 = _serializer6.DeserializeInUTC(ref reader);
            var item7 = _serializer7.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5, T6, T7> DeserializeSpot(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            var item3 = _serializer3.Deserialize(ref reader);
            var item4 = _serializer4.Deserialize(ref reader);
            var item5 = _serializer5.Deserialize(ref reader);
            var item6 = _serializer6.Deserialize(ref reader);
            var item7 = _serializer7.Deserialize(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5, T6, T7> DeserializeInUTCSpot(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            var item3 = _serializer3.DeserializeInUTC(ref reader);
            var item4 = _serializer4.DeserializeInUTC(ref reader);
            var item5 = _serializer5.DeserializeInUTC(ref reader);
            var item6 = _serializer6.DeserializeInUTC(ref reader);
            var item7 = _serializer7.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7>(item1, item2, item3, item4, item5, item6, item7);
        }

    }

    internal sealed class ValueTupleSerializer<TResolver, T1, T2, T3, T4, T5, T6, T7, TRest> : Serializer<TResolver, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>>
        where TResolver : Resolver
        where TRest : struct
    {
        private readonly Serializer<TResolver, T1> _serializer1 = Serializer<TResolver, T1>.Instance;
        private readonly Serializer<TResolver, T2> _serializer2 = Serializer<TResolver, T2>.Instance;
        private readonly Serializer<TResolver, T3> _serializer3 = Serializer<TResolver, T3>.Instance;
        private readonly Serializer<TResolver, T4> _serializer4 = Serializer<TResolver, T4>.Instance;
        private readonly Serializer<TResolver, T5> _serializer5 = Serializer<TResolver, T5>.Instance;
        private readonly Serializer<TResolver, T6> _serializer6 = Serializer<TResolver, T6>.Instance;
        private readonly Serializer<TResolver, T7> _serializer7 = Serializer<TResolver, T7>.Instance;
        private readonly Serializer<TResolver, TRest> _serializerRest = Serializer<TResolver, TRest>.Instance;

        [MethodImpl(Flags.HotPath)]
        public override sealed int? GetSize() => null;

        [MethodImpl(Flags.HotPath)]
        public override sealed int GetCapacity(ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
            => _serializer1.GetCapacity(value.Item1) + _serializer2.GetCapacity(value.Item2)
            + _serializer3.GetCapacity(value.Item3) + _serializer4.GetCapacity(value.Item4)
            + _serializer5.GetCapacity(value.Item5) + _serializer6.GetCapacity(value.Item6)
            + _serializer7.GetCapacity(value.Item7) + _serializerRest.GetCapacity(value.Rest);

        [MethodImpl(Flags.HotPath)]
        public override sealed void Serialize(ref Writer writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
            _serializer3.Serialize(ref writer, value.Item3);
            _serializer4.Serialize(ref writer, value.Item4);
            _serializer5.Serialize(ref writer, value.Item5);
            _serializer6.Serialize(ref writer, value.Item6);
            _serializer7.Serialize(ref writer, value.Item7);
            _serializerRest.Serialize(ref writer, value.Rest);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed void SerializeSpot(ref Writer writer, ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> value)
        {
            _serializer1.Serialize(ref writer, value.Item1);
            _serializer2.Serialize(ref writer, value.Item2);
            _serializer3.Serialize(ref writer, value.Item3);
            _serializer4.Serialize(ref writer, value.Item4);
            _serializer5.Serialize(ref writer, value.Item5);
            _serializer6.Serialize(ref writer, value.Item6);
            _serializer7.Serialize(ref writer, value.Item7);
            _serializerRest.Serialize(ref writer, value.Rest);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> Deserialize(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            var item3 = _serializer3.Deserialize(ref reader);
            var item4 = _serializer4.Deserialize(ref reader);
            var item5 = _serializer5.Deserialize(ref reader);
            var item6 = _serializer6.Deserialize(ref reader);
            var item7 = _serializer7.Deserialize(ref reader);
            var rest = _serializerRest.Deserialize(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, rest);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> DeserializeInUTC(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            var item3 = _serializer3.DeserializeInUTC(ref reader);
            var item4 = _serializer4.DeserializeInUTC(ref reader);
            var item5 = _serializer5.DeserializeInUTC(ref reader);
            var item6 = _serializer6.DeserializeInUTC(ref reader);
            var item7 = _serializer7.DeserializeInUTC(ref reader);
            var rest = _serializerRest.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, rest);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> DeserializeSpot(ref Reader reader)
        {
            var item1 = _serializer1.Deserialize(ref reader);
            var item2 = _serializer2.Deserialize(ref reader);
            var item3 = _serializer3.Deserialize(ref reader);
            var item4 = _serializer4.Deserialize(ref reader);
            var item5 = _serializer5.Deserialize(ref reader);
            var item6 = _serializer6.Deserialize(ref reader);
            var item7 = _serializer7.Deserialize(ref reader);
            var rest = _serializerRest.Deserialize(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, rest);
        }

        [MethodImpl(Flags.HotPath)]
        public override sealed ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest> DeserializeInUTCSpot(ref Reader reader)
        {
            var item1 = _serializer1.DeserializeInUTC(ref reader);
            var item2 = _serializer2.DeserializeInUTC(ref reader);
            var item3 = _serializer3.DeserializeInUTC(ref reader);
            var item4 = _serializer4.DeserializeInUTC(ref reader);
            var item5 = _serializer5.DeserializeInUTC(ref reader);
            var item6 = _serializer6.DeserializeInUTC(ref reader);
            var item7 = _serializer7.DeserializeInUTC(ref reader);
            var rest = _serializerRest.DeserializeInUTC(ref reader);
            return new ValueTuple<T1, T2, T3, T4, T5, T6, T7, TRest>(item1, item2, item3, item4, item5, item6, item7, rest);
        }

    }

}
