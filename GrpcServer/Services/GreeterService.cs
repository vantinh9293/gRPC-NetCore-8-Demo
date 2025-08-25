using Grpc.Core;
using GrpcServer;

namespace GrpcServer.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;
    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloReply
        {
            Message = "Hello " + request.Name
        });
    }
    // Bi-directional streaming Chat
    public override async Task Chat(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        // Tạo một task gửi tin nhắn chủ động từ server mỗi 5 giây
        var sendSystemMessage = Task.Run(async () =>
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(5), context.CancellationToken);
                var sysMsg = new HelloReply { Message = $"[Server System]: {DateTime.Now:HH:mm:ss} - This is message from server." };
                await responseStream.WriteAsync(sysMsg);
            }
        }, context.CancellationToken);

        try
        {
            await foreach (var request in requestStream.ReadAllAsync(context.CancellationToken))
            {
                var reply = new HelloReply { Message = $"Echo: {request.Name}" };
                await responseStream.WriteAsync(reply);
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            // Kết thúc task gửi tin nhắn hệ thống khi client ngắt kết nối
            // (Task sẽ tự động dừng do CancellationToken)
        }
    }
}
