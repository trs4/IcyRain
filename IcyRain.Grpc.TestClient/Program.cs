using System;
using Greet;
using IcyRain.Grpc.Client;
using IcyRain.Grpc.Tests;
using IcyRain.Grpc.Tests.Data;

using var channel = GrpcChannel.ForAddress("https://localhost:5001");

var client = new TestService.Client(channel);
var request = await client.UnaryOp(new UnaryRequest { Name = "IcyRainClient" });
Console.WriteLine("Response: " + request.Description);

//var client = new Greeter.GreeterClient(channel);
//var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
//Console.WriteLine("Greeting: " + reply.Message);

Console.WriteLine("Shutting down");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();