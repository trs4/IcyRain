namespace IcyRain.Grpc.Tools;

internal enum ReadState
{
    Using,
    Namespace,
    Service,
    Operations,
    End,
}
