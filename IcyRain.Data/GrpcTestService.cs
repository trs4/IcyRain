using System;
using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Data.Objects;
using IcyRain.Data.Streams;
using IcyRain.Internal;
using IcyRain.Streams;

namespace IcyRain.Data;

public static class GrpcTestService
{
    private const string ServiceName = "TestService";

    #region Data

    private static readonly Marshaller<TestData> _requestMarshaller = new(SerializeData, DeserializeData<TestData>);
    private static readonly Marshaller<SealedData> _responseMarshaller = new(SerializeData, DeserializeData<SealedData>);
    private static readonly Marshaller<Empty> _emptyMarshaller = new(SerializeData, DeserializeData<Empty>);
    internal static readonly Method<TestData, SealedData> Method = new(MethodType.Unary, ServiceName, "TestMethod", _requestMarshaller, _responseMarshaller);

    private static void SerializeData<T>(T obj, SerializationContext context)
    {
        var buffer = context.GetBufferWriter();
        Serialization.Serialize(buffer, obj);
        context.Complete();
    }

    private static T DeserializeData<T>(DeserializationContext context)
        => Serialization.Deserialize<T>(context.PayloadAsReadOnlySequence());

    #endregion
    #region Stream

    private static readonly Marshaller<StreamPart> _streamPartMarshaller = new(SerializeStreamPart, DeserializeStreamPart);
    private static readonly Marshaller<StreamDataPart<SealedData>> _streamDataPartMarshaller = new(SerializeStreamDataPart, DeserializeStreamDataPart);

    private static void SerializeStreamPart(StreamPart obj, SerializationContext context)
    {
        var buffer = context.GetBufferWriter();
        Serialization.Streams.Serialize(buffer, obj);
        context.Complete();
    }

    private static StreamPart DeserializeStreamPart(DeserializationContext context)
        => Serialization.Streams.Deserialize(context.PayloadAsReadOnlySequence());

    private static void SerializeStreamDataPart(StreamDataPart<SealedData> obj, SerializationContext context)
    {
        var buffer = context.GetBufferWriter();
        Serialization.Streams.Serialize(buffer, obj);
        context.Complete();
    }

    private static StreamDataPart<SealedData> DeserializeStreamDataPart(DeserializationContext context)
        => Serialization.Streams.DeserializeData<SealedData>(context.PayloadAsReadOnlySequence());

    internal static readonly Method<StreamPart, Empty> RequestStreamMethod
        = new(MethodType.ClientStreaming, ServiceName, "RequestStreamMethod", _streamPartMarshaller, _emptyMarshaller);

    internal static readonly Method<StreamDataPart<SealedData>, Empty> RequestStreamDataMethod
        = new(MethodType.ClientStreaming, ServiceName, "RequestStreamDataMethod", _streamDataPartMarshaller, _emptyMarshaller);


    internal static readonly Method<Empty, StreamPart> ResponseStreamMethod
        = new(MethodType.ServerStreaming, ServiceName, "ResponseStreamMethod", _emptyMarshaller, _streamPartMarshaller);

    internal static readonly Method<Empty, StreamDataPart<SealedData>> ResponseStreamDataMethod
        = new(MethodType.ServerStreaming, ServiceName, "ResponseStreamDataMethod", _emptyMarshaller, _streamDataPartMarshaller);


    internal static readonly Method<StreamPart, StreamPart> DuplexStreamMethod
        = new(MethodType.DuplexStreaming, ServiceName, "DuplexStreamMethod", _streamPartMarshaller, _streamPartMarshaller);

    internal static readonly Method<StreamDataPart<SealedData>, StreamDataPart<SealedData>> DuplexStreamDataMethod
        = new(MethodType.DuplexStreaming, ServiceName, "DuplexStreamDataMethod", _streamDataPartMarshaller, _streamDataPartMarshaller);

    #endregion

    public static async Task StartAsync()
    {
        const int port = 5400;

        var server = new Server
        {
            Ports = { new ServerPort("[::]", port, ServerCredentials.Insecure) },
            Services = { TestServiceServer.BindService(new TestServiceServerImpl()) }
        };

        server.Start();

        var client = new TestServiceClient(new Channel($"localhost:{port}", ChannelCredentials.Insecure));

        // Unary
        var request = new TestData { Property5 = "test" };
        var response = await client.TestMethod(request).ResponseAsync.ConfigureAwait(false);
        System.Diagnostics.Debug.WriteLine($"Response: {response?.Property5}");


        // Client streaming with Stream
        using var stream2 = GenerateStream();
        var response2 = await client.GetRequestMessage(stream2).ConfigureAwait(false);

        // Client streaming with Stream and data
        var data = new SealedData() { Property5 = "test4" };
        using var stream3 = GenerateStream();
        var response3 = await client.GetDataRequestMessage(data, stream3).ConfigureAwait(false);


        // Server streaming with Stream
        using var stream4 = client.GetResponseMessage();
        using var destinationStream4 = new MemoryStream();
        await stream4.CopyToAsync(destinationStream4).ConfigureAwait(false);

        // Server streaming with Stream and data
        var stream5 = await client.GetDataResponseMessage().ConfigureAwait(false);
        using var destinationStream5 = new MemoryStream();
        await stream5.CopyToAsync(destinationStream5).ConfigureAwait(false);
        var data5 = stream5.Data;


        // Duplex streaming with Stream
        using var stream6 = GenerateStream();
        var responseStream6 = await client.GetDuplexMessage(stream6).ConfigureAwait(false);
        using var destinationStream6 = new MemoryStream();
        await responseStream6.CopyToAsync(destinationStream6).ConfigureAwait(false);

        // Duplex streaming with Stream and data
        var data7 = new SealedData() { Property5 = "test7" };
        using var stream7 = GenerateStream();
        var responseStream7 = await client.GetDataDuplexMessage(data7, stream7).ConfigureAwait(false);
        using var destinationStream7 = new MemoryStream();
        await responseStream7.CopyToAsync(destinationStream7).ConfigureAwait(false);
        var dataResponse7 = responseStream7.Data;


        await server.ShutdownAsync().ConfigureAwait(false);
    }

    public static MemoryStream GenerateStream()
    {
        var random = new Random();
        byte[] buffer = new byte[Buffers.StreamPartSize * 2];
        random.NextBytes(buffer);
        return new MemoryStream(buffer);
    }

}

public abstract class TestServiceServer
{
    public static ServerServiceDefinition BindService(TestServiceServer server)
    {
        return ServerServiceDefinition.CreateBuilder()
            .AddMethod(GrpcTestService.Method, server.TestMethod)

            .AddMethod(GrpcTestService.RequestStreamMethod, server.RequestStreamMethod)
            .AddMethod(GrpcTestService.RequestStreamDataMethod, server.RequestStreamDataMethod)

            .AddMethod(GrpcTestService.ResponseStreamMethod, server.ResponseStreamMethod)
            .AddMethod(GrpcTestService.ResponseStreamDataMethod, server.ResponseStreamDataMethod)

            .AddMethod(GrpcTestService.DuplexStreamMethod, server.DuplexStreamMethod)
            .AddMethod(GrpcTestService.DuplexStreamDataMethod, server.DuplexStreamDataMethod)
            .Build();
    }

    public abstract Task<SealedData> TestMethod(TestData request, ServerCallContext context);


    public abstract Task<Empty> RequestStreamMethod(IAsyncStreamReader<StreamPart> requestStream, ServerCallContext context);

    public abstract Task<Empty> RequestStreamDataMethod(IAsyncStreamReader<StreamDataPart<SealedData>> requestStream, ServerCallContext context);


    public abstract Task ResponseStreamMethod(Empty parameters, IServerStreamWriter<StreamPart> responseStream, ServerCallContext context);

    public abstract Task ResponseStreamDataMethod(Empty parameters, IServerStreamWriter<StreamDataPart<SealedData>> responseStream, ServerCallContext context);


    public abstract Task DuplexStreamMethod(IAsyncStreamReader<StreamPart> requestStream,
        IServerStreamWriter<StreamPart> responseStream, ServerCallContext context);

    public abstract Task DuplexStreamDataMethod(IAsyncStreamReader<StreamDataPart<SealedData>> requestStream,
        IServerStreamWriter<StreamDataPart<SealedData>> responseStream, ServerCallContext context);
}

public sealed class TestServiceServerImpl : TestServiceServer
{
    public override Task<SealedData> TestMethod(TestData request, ServerCallContext context)
    {
        var result = new SealedData()
        {
            Property5 = "ok",
        };

        return Task.FromResult(result);
    }


    public override async Task<Empty> RequestStreamMethod(IAsyncStreamReader<StreamPart> requestStream, ServerCallContext context)
    {
        var reader = new GrpcStreamReader(requestStream);
        using var stream = TransferStream.Create(reader);

        using var destinationStream = new MemoryStream();
        await stream.CopyToAsync(destinationStream, -1, context.CancellationToken).ConfigureAwait(false);
        return new Empty();
    }

    public override async Task<Empty> RequestStreamDataMethod(IAsyncStreamReader<StreamDataPart<SealedData>> requestStream, ServerCallContext context)
    {
        var reader = new GrpcStreamDataReader<SealedData>(requestStream);
        using var stream = await TransferDataReaderStream<SealedData>.CreateWithDataAsync(reader).ConfigureAwait(false);
        var data = stream.Data;

        using var destinationStream = new MemoryStream();
        await stream.CopyToAsync(destinationStream, -1, context.CancellationToken).ConfigureAwait(false);
        return new Empty();
    }


    public override async Task ResponseStreamMethod(Empty parameters, IServerStreamWriter<StreamPart> responseStream, ServerCallContext context)
    {
        using var stream = GrpcTestService.GenerateStream();

        var writer = new GrpcStreamWriter(responseStream);
        await StreamTransmitter.SendAsync(writer, stream, cancellationToken: context.CancellationToken).ConfigureAwait(false);
    }

    public override async Task ResponseStreamDataMethod(Empty parameters, IServerStreamWriter<StreamDataPart<SealedData>> responseStream, ServerCallContext context)
    {
        var data = new SealedData() { Property5 = "Response 1" };
        using var stream = GrpcTestService.GenerateStream();

        var writer = new GrpcStreamDataWriter(responseStream);
        await StreamTransmitter.SendAsync(writer, data, stream, cancellationToken: context.CancellationToken).ConfigureAwait(false);
    }


    public override async Task DuplexStreamMethod(IAsyncStreamReader<StreamPart> requestStream,
        IServerStreamWriter<StreamPart> responseStream, ServerCallContext context)
    {
        var reader = new GrpcStreamReader(requestStream);
        using var stream = TransferStream.Create(reader);

        var writer = new GrpcStreamWriter(responseStream);
        await StreamTransmitter.SendAsync(writer, stream, cancellationToken: context.CancellationToken).ConfigureAwait(false);
    }

    public override async Task DuplexStreamDataMethod(IAsyncStreamReader<StreamDataPart<SealedData>> requestStream,
        IServerStreamWriter<StreamDataPart<SealedData>> responseStream, ServerCallContext context)
    {
        var reader = new GrpcStreamDataReader<SealedData>(requestStream);
        using var stream = await TransferDataReaderStream<SealedData>.CreateWithDataAsync(reader).ConfigureAwait(false);
        var data = stream.Data;

        var writer = new GrpcStreamDataWriter(responseStream);
        await StreamTransmitter.SendAsync(writer, data, stream, cancellationToken: context.CancellationToken).ConfigureAwait(false);
    }

}

public sealed class TestServiceClient : ClientBase<TestServiceClient>
{
    public TestServiceClient(Channel channel) : base(channel) { }

    private TestServiceClient(ClientBaseConfiguration configuration) : base(configuration) { }

    protected override TestServiceClient NewInstance(ClientBaseConfiguration configuration)
        => new TestServiceClient(configuration);

    public AsyncUnaryCall<SealedData> TestMethod(TestData request, CallOptions options = default)
        => CallInvoker.AsyncUnaryCall(GrpcTestService.Method, null, options, request);

    #region Request stream

    public async ValueTask<Empty> GetRequestMessage(Stream stream, CallOptions options = default)
    {
        using var call = CallInvoker.AsyncClientStreamingCall(GrpcTestService.RequestStreamMethod, null, options);

        var writer = new GrpcStreamWriter(call.RequestStream);
        await StreamTransmitter.SendAsync(writer, stream, cancellationToken: options.CancellationToken).ConfigureAwait(false);

        await call.RequestStream.CompleteAsync().ConfigureAwait(false);
        return await call.ResponseAsync.ConfigureAwait(false);
    }

    public async ValueTask<Empty> GetDataRequestMessage(SealedData data, Stream stream, CallOptions options = default)
    {
        using var call = CallInvoker.AsyncClientStreamingCall(GrpcTestService.RequestStreamDataMethod, null, options);

        var writer = new GrpcStreamDataWriter(call.RequestStream);
        await StreamTransmitter.SendAsync(writer, data, stream, cancellationToken: options.CancellationToken).ConfigureAwait(false);

        await call.RequestStream.CompleteAsync().ConfigureAwait(false);
        return await call.ResponseAsync.ConfigureAwait(false);
    }

    #endregion
    #region Response stream

    public TransferStream GetResponseMessage(CallOptions options = default)
    {
        var call = CallInvoker.AsyncServerStreamingCall(GrpcTestService.ResponseStreamMethod, null, options, new Empty());

        var reader = new GrpcStreamReader(call.ResponseStream);
        return TransferStream.Create(reader, () => call.Dispose());
    }

    public async ValueTask<TransferDataStream<SealedData>> GetDataResponseMessage(CallOptions options = default)
    {
        var call = CallInvoker.AsyncServerStreamingCall(GrpcTestService.ResponseStreamDataMethod, null, options, new Empty());

        var reader = new GrpcStreamDataReader<SealedData>(call.ResponseStream);
        return await TransferDataReaderStream<SealedData>.CreateWithDataAsync(reader, () => call.Dispose()).ConfigureAwait(false);
    }

    #endregion
    #region Duplex stream

    public async ValueTask<TransferStream> GetDuplexMessage(Stream stream, CallOptions options = default)
    {
        var call = CallInvoker.AsyncDuplexStreamingCall(GrpcTestService.DuplexStreamMethod, null, options);

        var writer = new GrpcStreamWriter(call.RequestStream);
        await StreamTransmitter.SendAsync(writer, stream, cancellationToken: options.CancellationToken).ConfigureAwait(false);

        await call.RequestStream.CompleteAsync().ConfigureAwait(false);

        var reader = new GrpcStreamReader(call.ResponseStream);
        return TransferStream.Create(reader, () => call.Dispose());
    }

    public async ValueTask<TransferDataStream<SealedData>> GetDataDuplexMessage(SealedData data, Stream stream, CallOptions options = default)
    {
        var call = CallInvoker.AsyncDuplexStreamingCall(GrpcTestService.DuplexStreamDataMethod, null, options);

        var writer = new GrpcStreamDataWriter(call.RequestStream);
        await StreamTransmitter.SendAsync(writer, data, stream, cancellationToken: options.CancellationToken).ConfigureAwait(false);

        await call.RequestStream.CompleteAsync().ConfigureAwait(false);

        var reader = new GrpcStreamDataReader<SealedData>(call.ResponseStream);
        return await TransferDataReaderStream<SealedData>.CreateWithDataAsync(reader, () => call.Dispose()).ConfigureAwait(false);
    }

    #endregion
}
