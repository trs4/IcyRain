namespace IcyRain.Grpc.Client.Balancer.Internal;

internal sealed class EmptyPicker : SubchannelPicker
{
    public static readonly EmptyPicker Instance = new();

    private readonly PickResult _pickResult;

    private EmptyPicker()
        => _pickResult = PickResult.ForQueue();

    public override PickResult Pick(PickContext context) => _pickResult;
}
