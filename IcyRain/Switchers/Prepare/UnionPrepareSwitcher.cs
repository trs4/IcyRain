using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;
using IcyRain.Serializers;

namespace IcyRain.Switchers;

internal sealed class UnionPrepareSwitcher<T> : PrepareSwitcher<T>
{
    [MethodImpl(Flags.HotPath)]
    public sealed override void Prepare()
    {
        if (Serializer<UnionResolver, T>.Instance is IErrorSerializer errorSerializer)
            errorSerializer.Throw();
    }

}
