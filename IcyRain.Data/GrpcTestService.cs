using System;
using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Data.Objects;

namespace IcyRain.Data
{
    public static class GrpcTestService
    {
        private static readonly DeserializeOptions _options = new(DateTimeKind.Utc);
        private static readonly Marshaller<TestData> _requestMarshaller = new(SerializeData, DeserializeData<TestData>);
        private static readonly Marshaller<SealedData> _responseMarshaller = new(SerializeData, DeserializeData<SealedData>);
        internal static readonly Method<TestData, SealedData> Method = new(MethodType.Unary, "TestService", "TestMethod", _requestMarshaller, _responseMarshaller);

        private static void SerializeData<T>(T obj, SerializationContext context)
        {
            var buffer = context.GetBufferWriter();
            Serialization.Serialize(buffer, obj);
            context.Complete();
        }

        private static T DeserializeData<T>(DeserializationContext context)
            => Serialization.Deserialize<T>(context.PayloadAsReadOnlySequence(), _options);

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
            var request = new TestData { Property5 = "test" };
            var response = await client.TestMethod(request).ResponseAsync.ConfigureAwait(false);
            System.Diagnostics.Debug.WriteLine($"Response: {response?.Property5}");

            await server.ShutdownAsync().ConfigureAwait(false);
        }

    }

    public abstract class TestServiceServer
    {
        public static ServerServiceDefinition BindService(TestServiceServer server)
        {
            return ServerServiceDefinition.CreateBuilder()
                .AddMethod(GrpcTestService.Method, server.TestMethod).Build();
        }

        public abstract Task<SealedData> TestMethod(TestData request, ServerCallContext context);
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

    }

    public sealed class TestServiceClient : ClientBase<TestServiceClient>
    {
        public TestServiceClient(Channel channel) : base(channel) { }

        private TestServiceClient(ClientBaseConfiguration configuration) : base(configuration) { }

        protected override TestServiceClient NewInstance(ClientBaseConfiguration configuration)
            => new TestServiceClient(configuration);

        public AsyncUnaryCall<SealedData> TestMethod(TestData request, CallOptions options = default)
            => CallInvoker.AsyncUnaryCall(GrpcTestService.Method, null, options, request);
    }

}
