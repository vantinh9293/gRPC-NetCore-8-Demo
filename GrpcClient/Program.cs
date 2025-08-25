using System;
using System.Threading.Tasks;
using Grpc.Core;
using System.Runtime.CompilerServices;
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

			// Console.WriteLine("Your name:");
			// var name = Console.ReadLine();

			// var reply = await client.SayHelloAsync(new HelloRequest { Name = name });
			// Console.WriteLine($"Server reply: {reply.Message}");
			// Console.ReadLine();

		// Bi-directional streaming demo
		Console.WriteLine("\nBi-directional streaming demo. Nhập nhiều tên, để trống để kết thúc.");
		using var call = client.Chat();

		// Task nhận phản hồi từ server
		var responseTask = Task.Run(async () =>
		{
			while (await call.ResponseStream.MoveNext())
			{
				var response = call.ResponseStream.Current;
				Console.WriteLine($"[Server]: {response.Message}");
			}
		});

		// Gửi nhiều request lên server
		while (true)
		{
			Console.Write("[You]: ");
			var input = Console.ReadLine();
			if (string.IsNullOrEmpty(input))
				break;
			await call.RequestStream.WriteAsync(new HelloRequest { Name = input });
		}
		await call.RequestStream.CompleteAsync();
		await responseTask;
		}
	}
}
