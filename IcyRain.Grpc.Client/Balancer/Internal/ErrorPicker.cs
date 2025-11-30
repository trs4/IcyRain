using System.Diagnostics;
using Grpc.Core;

namespace IcyRain.Grpc.Client.Balancer.Internal;

internal sealed class ErrorPicker : SubchannelPicker
{
    private readonly Status _status;

    public ErrorPicker(Status status)
    {
        Debug.Assert(status.StatusCode != StatusCode.OK, "Error status code must not be OK.");
        _status = status;
    }

    public override PickResult Pick(PickContext context) => PickResult.ForFailure(_status);
}
