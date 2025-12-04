namespace IcyRain.Grpc.Tools;

internal sealed class OperationData
{
    public OperationData(Operation operation, string methodName,
        string requestType, string requestMarshaller, string responseType, string responseMarshaller)
    {
        Operation = operation;
        MethodName = methodName;
        RequestType = requestType;
        RequestMarshaller = requestMarshaller;
        ResponseType = responseType;
        ResponseMarshaller = responseMarshaller;
    }

    public Operation Operation { get; }

    public string MethodName { get; }

    public string RequestType { get; }

    public string RequestMarshaller { get; }

    public string ResponseType { get; }

    public string ResponseMarshaller { get; }
}
