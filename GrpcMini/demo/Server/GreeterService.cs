using Google.Protobuf.WellKnownTypes;
using GrpcMini;

namespace Server
{
[GrpcService(ServiceName = "Greeter")]
public class GreeterService: IGrpcService<GreeterService>
{
    public Task<HelloReply> SayHelloUnaryAsync(HelloRequest request, ServerCallContext context)
    => Task.FromResult(new HelloReply { Message = $"Hello, {request.Name}" });

    public async Task<HelloReply> SayHelloClientStreamingAsync(IAsyncStreamReader<HelloRequest> reader, ServerCallContext context)
    {
        var list = new List<string>();
        while (await reader.MoveNext(CancellationToken.None))
        {
            list.Add(reader.Current.Name);
        }
        return new HelloReply { Message = $"Hello, {string.Join(",", list)}" };
    }

    public  async Task SayHelloServerStreamingAsync(Empty request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        await responseStream.WriteAsync(new HelloReply { Message = "Hello, Foo!" });
        await Task.Delay(1000);
        await responseStream.WriteAsync(new HelloReply { Message = "Hello, Bar!" });
        await Task.Delay(1000);
        await responseStream.WriteAsync(new HelloReply { Message = "Hello, Baz!" });
    }

    public async Task SayHelloDuplexStreamingAsync(IAsyncStreamReader<HelloRequest> reader, IServerStreamWriter<HelloReply> writer, ServerCallContext context)
    {
        while (await reader.MoveNext())
        {
            await writer.WriteAsync(new HelloReply { Message = $"Hello {reader.Current.Name}" });
        }
    }

        public static void Bind(IServiceBinder<GreeterService> binder)
        {
            binder.AddUnaryMethod<HelloRequest, HelloReply>(it =>it.SayHelloUnaryAsync(default!,default!), HelloRequest.Parser);
            binder.AddClientStreamingMethod<HelloRequest, HelloReply>(it => it.SayHelloClientStreamingAsync(default!, default!), HelloRequest.Parser);
            //binder.AddServerStreamingMethod<Empty, HelloReply>(it => it.SayHelloServerStreamingAsync(default!,default!, default!), Empty.Parser);
            //binder.AddDuplexStreamingMethod<HelloRequest, HelloReply>(it => it.SayHelloDuplexStreamingAsync(default!, default!,default!), HelloRequest.Parser);

            // OR

            //binder.AddUnaryMethod<HelloRequest, HelloReply>(nameof(SayHelloAsync), it => it.SayHelloAsync, HelloRequest.Parser);
            //binder.AddClientStreamingMethod<HelloRequest, HelloReply>(nameof(SayHelloClientStreamingAsync), it => it.SayHelloClientStreamingAsync, HelloRequest.Parser);
            binder.AddServerStreamingMethod<Empty, HelloReply>(nameof(SayHelloServerStreamingAsync), it => it.SayHelloServerStreamingAsync, Empty.Parser);
            binder.AddDuplexStreamingMethod<HelloRequest, HelloReply>(nameof(SayHelloDuplexStreamingAsync), it => it.SayHelloDuplexStreamingAsync, HelloRequest.Parser);
        }      
    }
}
