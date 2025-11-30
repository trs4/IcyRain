using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace IcyRain.Grpc.Client.Internal;

internal interface IOperatingSystem
{
    bool IsBrowser { get; }

    bool IsAndroid { get; }

    bool IsWindows { get; }

    bool IsWindowsServer { get; }

    Version OSVersion { get; }
}

internal sealed class OperatingSystem : IOperatingSystem
{
    public static readonly OperatingSystem Instance = new();

    private readonly Lazy<bool> _isWindowsServer;

    public bool IsBrowser { get; }

    public bool IsAndroid { get; }

    public bool IsWindows { get; }

    public bool IsWindowsServer => _isWindowsServer.Value;

    public Version OSVersion { get; }

    private OperatingSystem()
    {
        IsAndroid = System.OperatingSystem.IsAndroid();
        IsWindows = System.OperatingSystem.IsWindows();
        IsBrowser = System.OperatingSystem.IsBrowser();
        OSVersion = Environment.OSVersion.Version;

        // Windows Server detection requires a P/Invoke call to RtlGetVersion.
        // Get the value lazily so that it is only called if needed.
        _isWindowsServer = new Lazy<bool>(() =>
        {
            // RtlGetVersion is not available on UWP. Check it first.
            if (IsWindows && !Native.IsUwp(RuntimeInformation.FrameworkDescription, Environment.OSVersion.Version))
            {
                Native.DetectWindowsVersion(out _, out var isWindowsServer);
                return isWindowsServer;
            }

            return false;
        }, LazyThreadSafetyMode.ExecutionAndPublication);
    }

}
