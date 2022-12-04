using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using System.IO.Pipelines;

namespace GrpcMini
{
public class HttpContextStreamWriter<T> : IServerStreamWriter<T> where T : IMessage<T>
{
    private readonly PipeWriter _writer;
    public HttpContextStreamWriter(HttpContext httpContext) => _writer = httpContext.Response.BodyWriter;
    public Task WriteAsync(T message, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return _writer.WriteMessageAsync(message).AsTask();
    }
}
}