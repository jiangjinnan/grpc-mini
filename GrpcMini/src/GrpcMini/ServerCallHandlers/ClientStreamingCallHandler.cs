using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcMini
{
internal class ClientStreamingCallHandler<TService, TRequest, TResponse> : ServerCallHandler<TService, TRequest, TResponse>
    where TService : class
    where TRequest : IMessage<TRequest>
    where TResponse : IMessage<TResponse>
{
    private readonly ClientStreamingMethod<TService, TRequest, TResponse> _handler;
    public ClientStreamingCallHandler(ClientStreamingMethod<TService, TRequest, TResponse> handler, MessageParser<TRequest> requestParser)
        :base(requestParser) 
    {
        _handler = handler;
    }
    protected override async Task HandleCallAsyncCore(ServerCallContext serverCallContext)
    {
        using var scope = serverCallContext.HttpContext.RequestServices.CreateScope();
        var service = ActivatorUtilities.CreateInstance<TService>(scope.ServiceProvider);
        var reader = serverCallContext.HttpContext.Request.BodyReader;
        var writer = serverCallContext.HttpContext.Response.BodyWriter;
        var streamReader = new HttpContextStreamReader<TRequest>(serverCallContext.HttpContext, RequestParser);
        var response = await _handler(service, streamReader, serverCallContext);
        await writer.WriteMessageAsync(response);
    }
}
}
