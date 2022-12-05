using Google.Protobuf;

namespace GrpcMini
{
public delegate Task<TResponse> UnaryMethod<TService, TRequest, TResponse>(TService service, TRequest request, ServerCallContext context)
    where TService : class
    where TRequest : IMessage<TRequest>
    where TResponse : IMessage<TResponse>;

public delegate Task<TResponse> ClientStreamingMethod<TService, TRequest, TResponse>(TService service, IAsyncStreamReader<TRequest> reader, ServerCallContext context)
    where TService : class
    where TRequest : IMessage<TRequest>
    where TResponse : IMessage<TResponse>;

public delegate Task ServerStreamingMethod<TService, TRequest, TResponse>(TService service, TRequest request, IServerStreamWriter<TResponse> writer, ServerCallContext context)
    where TService : class
    where TRequest : IMessage<TRequest>
    where TResponse : IMessage<TResponse>;

public delegate Task DuplexStreamingMethod<TService, TRequest, TResponse>(TService service, IAsyncStreamReader<TRequest> reader, IServerStreamWriter<TResponse> writer, ServerCallContext context)
    where TService : class
    where TRequest : IMessage<TRequest>
    where TResponse : IMessage<TResponse>;
}