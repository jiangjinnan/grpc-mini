using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcMini
{
internal class ServerStreamingCallHandler<TService, TRequest, TResponse> : ServerCallHandler<TService, TRequest, TResponse>
    where TService : class
    where TRequest : IMessage<TRequest>
    where TResponse : IMessage<TResponse>
{
    private readonly ServerStreamingMethod<TService, TRequest, TResponse> _handler;
    public ServerStreamingCallHandler(ServerStreamingMethod<TService, TRequest, TResponse> handler, MessageParser<TRequest> requestParser):base(requestParser)
        => _handler = handler;
    protected override async Task HandleCallAsyncCore(ServerCallContext serverCallContext)
    {
        using var scope = serverCallContext.HttpContext.RequestServices.CreateScope();
        var service = ActivatorUtilities.CreateInstance<TService>(scope.ServiceProvider);   
        var httpContext = serverCallContext.HttpContext;
        var streamWriter = new HttpContextStreamWriter<TResponse>(httpContext);
        var request = await httpContext.Request.BodyReader.ReadSingleMessageAsync(RequestParser);
        await _handler(service, request, streamWriter, serverCallContext);
    }
}
}