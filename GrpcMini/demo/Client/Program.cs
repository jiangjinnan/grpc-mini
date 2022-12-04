using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using static Greeter;

var channel = GrpcChannel.ForAddress("http://localhost:5000");
var client = new GreeterClient(channel);

Console.WriteLine("Unary");
await UnaryCallAsync();
Console.WriteLine("\nServer Streaming");
await ServerStreamingCallAsync();
Console.WriteLine("\nClient Streaming");
await ClientStreamingCallAsync();
Console.WriteLine("\nDuplex Streaming");
await DuplexStreamingCallAsync();

Console.ReadLine();

async Task UnaryCallAsync()
{
    var request = new HelloRequest { Name = "foobar" };
    var reply = await client.SayHelloUnaryAsync(request);
    Console.WriteLine(reply.Message);
}

async Task ServerStreamingCallAsync()
{
    var streamingCall = client.SayHelloServerStreaming(new Empty());
    var reader = streamingCall.ResponseStream;
    while (await reader.MoveNext(CancellationToken.None))
    {
        Console.WriteLine(reader.Current.Message);
    }
}

async Task ClientStreamingCallAsync()
{
    var streamingCall = client.SayHelloClientStreaming();
    var writer = streamingCall.RequestStream;

    await writer.WriteAsync(new HelloRequest { Name = "Foo" });
    await Task.Delay(1000);
    await writer.WriteAsync(new HelloRequest { Name = "Bar" });
    await Task.Delay(1000);
    await writer.WriteAsync(new HelloRequest { Name = "Baz" });
    await writer.CompleteAsync();

    var reply = await streamingCall.ResponseAsync;
    Console.WriteLine(reply.Message);
}
async Task DuplexStreamingCallAsync()
{
    var streamingCall = client.SayHelloDuplexStreaming();
    var writer = streamingCall.RequestStream;
    var reader = streamingCall.ResponseStream;
    _ = Task.Run(async () =>
    {
        await writer.WriteAsync(new HelloRequest { Name = "Foo" });
        await Task.Delay(1000);
        await writer.WriteAsync(new HelloRequest { Name = "Bar" });
        await Task.Delay(1000);
        await writer.WriteAsync(new HelloRequest { Name = "Baz" });
        await writer.CompleteAsync();
    });
    await foreach (var reply in reader.ReadAllAsync())
    {
        Console.WriteLine(reply.Message);
    }
}


