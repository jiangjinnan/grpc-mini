using Google.Protobuf;
using System.Linq.Expressions;

namespace GrpcMini
{
public interface IServiceBinder<TService> where TService : class
{
    IServiceBinder<TService> AddUnaryMethod<TRequest, TResponse>(string methodName, Func<TService, Func<TRequest, ServerCallContext, Task<TResponse>>> methodAccessor, MessageParser<TRequest> parser)            
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>;

    IServiceBinder<TService> AddClientStreamingMethod<TRequest, TResponse>(string methodName, Func<TService, Func<IAsyncStreamReader<TRequest>, ServerCallContext, Task<TResponse>>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>;

    IServiceBinder<TService> AddServerStreamingMethod<TRequest, TResponse>(string methodName, Func<TService, Func<TRequest, IServerStreamWriter<TResponse>, ServerCallContext, Task>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>;

    IServiceBinder<TService> AddDuplexStreamingMethod<TRequest, TResponse>(string methodName, Func<TService, Func<IAsyncStreamReader<TRequest>, IServerStreamWriter<TResponse>, ServerCallContext, Task>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>;


    IServiceBinder<TService> AddUnaryMethod<TRequest, TResponse>(Expression<Func<TService, Task<TResponse>>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>;
    IServiceBinder<TService> AddClientStreamingMethod<TRequest, TResponse>( Expression<Func<TService, Task<TResponse>>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>;

    IServiceBinder<TService> AddServerStreamingMethod<TRequest, TResponse>( Expression<Func<TService, Task>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>;

    IServiceBinder<TService> AddDuplexStreamingMethod<TRequest, TResponse>( Expression<Func<TService, Task>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>;
}
}
