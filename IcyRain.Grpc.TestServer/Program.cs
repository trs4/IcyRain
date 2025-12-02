using IcyRain.Grpc.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Server;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

var app = builder.Build();
app.MapGrpcService<TestServiceImpl>();
//app.MapGrpcService<GreeterService>();

app.Run();