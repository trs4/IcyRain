using System.Threading.Tasks;
using Grpc.Core;
using IcyRain.Grpc.Tests;
using IcyRain.Grpc.Tests.Data;

namespace Server;

public class TestServiceImpl : TestService.Server
{
    public override Task<UnaryResponse> UnaryOp(UnaryRequest request, ServerCallContext context)
        => Task.FromResult(new UnaryResponse { Description = "Hello " + request.Name });

    public override async Task<StreamResponse> ClientStreamOp(IAsyncStreamReader<StreamRequest> request, ServerCallContext context)
    {
        string last = null;

        while (await request.MoveNext())
            last = request.Current.Name;

        return new StreamResponse() { Description = "Hello " + last };
    }

    public override async Task ServerStreamOp(StreamRequest request, IServerStreamWriter<StreamResponse> response, ServerCallContext context)
    {
        await response.WriteAsync(new StreamResponse() { Description = "Hello " + request.Name });
        await response.WriteAsync(new StreamResponse() { Description = "Hello2 " + request.Name });
    }

    public override async Task DuplexStreamOp(IAsyncStreamReader<StreamRequest> request, IServerStreamWriter<StreamResponse> response, ServerCallContext context)
    {
        string last = null;

        while (await request.MoveNext())
            last = request.Current.Name;

        await response.WriteAsync(new StreamResponse() { Description = "Hello " + last });
    }

}
