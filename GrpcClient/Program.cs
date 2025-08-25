using System;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcServer;

namespace GrpcClient
{
	class Program
	{
		static async Task Main(string[] args)
		{
			// Địa chỉ server gRPC
			using var channel = GrpcChannel.ForAddress("http://localhost:5140");
			var client = new Greeter.GreeterClient(channel);

			Console.WriteLine("Your name:");
			var name = Console.ReadLine();

			var reply = await client.SayHelloAsync(new HelloRequest { Name = name });
			Console.WriteLine($"Server reply: {reply.Message}");
			Console.ReadLine();
		}
	}
}
