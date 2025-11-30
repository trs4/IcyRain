using System;
using Greet;
using IcyRain.Grpc.Client;

using var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new Greeter.GreeterClient(channel);

var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
Console.WriteLine("Greeting: " + reply.Message);

Console.WriteLine("Shutting down");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();