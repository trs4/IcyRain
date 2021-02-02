using System.Runtime.CompilerServices;
using IcyRain.Internal;
using IcyRain.Resolvers;

namespace IcyRain.Switchers
{
    public abstract class PrepareSwitcher<T>
    {
        public static PrepareSwitcher<T> Instance
        {
            [MethodImpl(Flags.HotPath)]
            get;
        }

        static PrepareSwitcher()
            => Instance = ResolverHelper.IsUnionResolver<T>() ? new UnionPrepareSwitcher<T>() : new DefaultPrepareSwitcher<T>();

        public abstract void Prepare();
    }
}
