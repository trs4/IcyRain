using IcyRain.Grpc.AspNetCore;
using IcyRain.Grpc.AspNetCore.Web;
using Microsoft.AspNetCore.Builder;
using Server;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

var app = builder.Build();
app.UseGrpcWeb();
app.MapGrpcService<TestServiceImpl>();
//app.MapGrpcService<GreeterService>();

app.Run("https://*:21452");