using System.Runtime.CompilerServices;
using System.Text;

namespace IcyRain.Internal;

internal static class StringEncoding
{
    public static readonly Encoding UTF8 = new UTF8Encoding(false);

    [MethodImpl(Flags.HotPath)]
    public static int GetSize(string value)
        => value is null || value.Length == 0 ? 4 : 4 + value.Length * 2;
}
