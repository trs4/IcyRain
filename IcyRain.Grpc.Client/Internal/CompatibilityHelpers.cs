using System.Threading.Tasks;

namespace IcyRain.Grpc.Client.Internal;

internal static class CompatibilityHelpers
{
    public static Task<T> AwaitWithYieldAsync<T>(Task<T> callTask)
    {
        // A completed task doesn't need to yield because code after it isn't run in a continuation.
        if (callTask.IsCompleted)
            return callTask;

        return AwaitWithYieldAsyncCore(callTask);

        static async Task<T> AwaitWithYieldAsyncCore(Task<T> callTask)
            => await callTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);
    }

}
