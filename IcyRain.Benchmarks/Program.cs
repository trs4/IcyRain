using System;
using BenchmarkDotNet.Running;

// PowerShell
// [System.Environment]::SetEnvironmentVariable('DOTNET_CLI_TELEMETRY_OPTOUT', 1, 'Machine')

namespace IcyRain.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssemblies(new[] { typeof(Program).Assembly }).Run(args);

        if (args is null || args.Length == 0)
            Console.ReadKey();
    }

}
