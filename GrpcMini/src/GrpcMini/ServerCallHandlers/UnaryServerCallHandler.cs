using Google.Protobuf;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcMini
{
internal class UnaryServerCallHandler<TService, TRequest, TResponse> : ServerCallHandler<TService, TRequest, TResponse>
    where TService : class
    where TRequest : IMessage<TRequest>
    where TResponse : IMessage<TResponse>
{
    private readonly UnaryServerMethod<TService, TRequest, TResponse> _handler;

    public UnaryServerCallHandler(UnaryServerMethod<TService, TRequest, TResponse> handler, MessageParser<TRequest> requestParser):base(requestParser)
    => _handler = handler;
        protected override async Task HandleCallAsyncCore(ServerCallContext serverCallContext)
    {
        using var scope = serverCallContext.HttpContext.RequestServices.CreateScope();
        var service = ActivatorUtilities.CreateInstance<TService>(scope.ServiceProvider);
        var httpContext = serverCallContext.HttpContext;
        var request = await httpContext.Request.BodyReader.ReadSingleMessageAsync<TRequest>(RequestParser);
        var reply = await _handler(service, request!, serverCallContext);
        await httpContext.Response.BodyWriter.WriteMessageAsync(reply);
    }
}
}
