using System;
using System.Threading.Tasks;

namespace IcyRain.Data;

internal class Program
{
    private static async Task Main()
    {
        await GrpcTestService.StartAsync().ConfigureAwait(false);
        Console.ReadLine();
    }

}
