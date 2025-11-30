using System;

namespace IcyRain.Grpc.Client.Internal;

internal static class RuntimeHelpers
{
    public static bool QueryRuntimeSettingSwitch(string switchName, bool defaultValue)
    {
        if (AppContext.TryGetSwitch(switchName, out var value))
            return value;

        return defaultValue;
    }

}
