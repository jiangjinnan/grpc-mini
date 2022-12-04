using Google.Protobuf;
using Microsoft.AspNetCore.Http;

namespace GrpcMini
{
public abstract class ServerCallHandlerBase
{
    public async Task HandleCallAsync(HttpContext httpContext)
    {
        try
        {
            var serverCallContext = new ServerCallContext(httpContext);
            var response = httpContext.Response;
            response.ContentType = "application/grpc";
            await HandleCallAsyncCore(serverCallContext);
            SetStatus(serverCallContext.StatusCode);
        }
        catch
        {
            SetStatus(StatusCode.Unknown);
        }

        void SetStatus(StatusCode statusCode)
        {
            httpContext.Response.AppendTrailer("grpc-status", ((int)statusCode).ToString());
        }
    }
    protected abstract Task HandleCallAsyncCore(ServerCallContext serverCallContext);
}

public abstract class ServerCallHandler<TService, TRequest, TResponse> : ServerCallHandlerBase
    where TService : class
    where TRequest : IMessage<TRequest>
    where TResponse : IMessage<TResponse>
{
    protected ServerCallHandler(MessageParser<TRequest> requestParser)=> RequestParser = requestParser;
    public MessageParser<TRequest> RequestParser { get; }
}
}