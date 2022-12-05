using Google.Protobuf;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Linq.Expressions;
using System.Reflection;

namespace GrpcMini
{
public class ServiceBinder<TService> : IServiceBinder<TService> where TService : class
{
    private readonly IEndpointRouteBuilder _routeBuilder;
    public ServiceBinder(IEndpointRouteBuilder routeBuilder) => _routeBuilder = routeBuilder;

    public IServiceBinder<TService> AddUnaryMethod<TRequest, TResponse>(string methodName, Func<TService, Func<TRequest, ServerCallContext, Task<TResponse>>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        Task<TResponse> GetMethod(TService service, TRequest request, ServerCallContext context) => methodAccessor(service)(request, context);
        var callHandler = new UnaryCallHandler<TService, TRequest, TResponse>(GetMethod, parser);
        _routeBuilder.MapPost(ServiceBinder<TService>.GetPath(methodName), callHandler.HandleCallAsync);
        return this;
    }

    public IServiceBinder<TService> AddClientStreamingMethod<TRequest, TResponse>(string methodName, Func<TService, Func<IAsyncStreamReader<TRequest>, ServerCallContext, Task<TResponse>>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        Task<TResponse> GetMethod(TService service, IAsyncStreamReader<TRequest> reader, ServerCallContext context) => methodAccessor(service)(reader, context);
        var callHandler = new ClientStreamingCallHandler<TService, TRequest, TResponse>(GetMethod, parser);
        _routeBuilder.MapPost(ServiceBinder<TService>.GetPath(methodName), callHandler.HandleCallAsync);
        return this;
    }

    public IServiceBinder<TService> AddServerStreamingMethod<TRequest, TResponse>(string methodName, Func<TService, Func<TRequest, IServerStreamWriter<TResponse>, ServerCallContext, Task>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        ServerStreamingMethod<TService, TRequest, TResponse> handler = (service, request, writer, context) => methodAccessor(service)(request, writer, context);
        var callHandler = new ServerStreamingCallHandler<TService, TRequest, TResponse>(handler, parser);
        _routeBuilder.MapPost(ServiceBinder<TService>.GetPath(methodName), callHandler.HandleCallAsync);
        return this;
    }

    public IServiceBinder<TService> AddDuplexStreamingMethod<TRequest, TResponse>(string methodName, Func<TService, Func<IAsyncStreamReader<TRequest>, IServerStreamWriter<TResponse>, ServerCallContext, Task>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        DuplexStreamingMethod<TService, TRequest, TResponse> handler = (service, reader, writer, context) => methodAccessor(service)(reader, writer, context);
        var callHandler = new DuplexStreamingCallHandler<TService, TRequest, TResponse>(handler, parser);
        _routeBuilder.MapPost(ServiceBinder<TService>.GetPath(methodName), callHandler.HandleCallAsync);
        return this;
    }

    private static string GetPath(string methodName)
    {
        var serviceName = typeof(TService).GetCustomAttribute<GrpcServiceAttribute>()?.ServiceName ?? typeof(TService).Name;
        if (methodName.EndsWith("Async"))
        {
            methodName = methodName.Substring(0, methodName.Length - 5);
        }
        return $"{serviceName}/{methodName}";
    }

    public IServiceBinder<TService> AddUnaryMethod<TRequest, TResponse>(Expression<Func<TService, Task<TResponse>>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        var method = CreateDelegate<UnaryMethod<TService, TRequest,TResponse>>(methodAccessor, out var methodName);
        var serviceName = typeof(TService).GetCustomAttribute<GrpcServiceAttribute>()?.ServiceName ?? typeof(TService).Name;
        var callHandler = new UnaryCallHandler<TService, TRequest, TResponse>(method, parser);
        _routeBuilder.MapPost(ServiceBinder<TService>.GetPath(methodName), callHandler.HandleCallAsync);
        return this;
    }

    public IServiceBinder<TService> AddClientStreamingMethod<TRequest, TResponse>( Expression<Func<TService, Task<TResponse>>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        var method = CreateDelegate<ClientStreamingMethod<TService, TRequest, TResponse>>(methodAccessor, out var methodName);
        var serviceName = typeof(TService).GetCustomAttribute<GrpcServiceAttribute>()?.ServiceName ?? typeof(TService).Name;
        var callHandler = new ClientStreamingCallHandler<TService, TRequest, TResponse>(method, parser);
        _routeBuilder.MapPost(ServiceBinder<TService>.GetPath(methodName), callHandler.HandleCallAsync);
        return this;
    }

    public IServiceBinder<TService> AddServerStreamingMethod<TRequest, TResponse>(Expression<Func<TService, Task>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        var method = CreateDelegate<ServerStreamingMethod<TService, TRequest, TResponse>>(methodAccessor, out var methodName);
        var serviceName = typeof(TService).GetCustomAttribute<GrpcServiceAttribute>()?.ServiceName ?? typeof(TService).Name;
        var callHandler = new ServerStreamingCallHandler<TService, TRequest, TResponse>(method, parser);
        _routeBuilder.MapPost(ServiceBinder<TService>.GetPath(methodName), callHandler.HandleCallAsync);
        return this;
    }

    public IServiceBinder<TService> AddDuplexStreamingMethod<TRequest, TResponse>(Expression<Func<TService, Task>> methodAccessor, MessageParser<TRequest> parser)
        where TRequest : IMessage<TRequest>
        where TResponse : IMessage<TResponse>
    {
        var method = CreateDelegate<DuplexStreamingMethod<TService, TRequest, TResponse>>(methodAccessor, out var methodName);
        var serviceName = typeof(TService).GetCustomAttribute<GrpcServiceAttribute>()?.ServiceName ?? typeof(TService).Name;
        var callHandler = new DuplexStreamingCallHandler<TService, TRequest, TResponse>(method, parser);
        _routeBuilder.MapPost(ServiceBinder<TService>.GetPath(methodName), callHandler.HandleCallAsync);
        return this;
    }

    private TDelegate CreateDelegate<TDelegate>(LambdaExpression expression, out string methodName) where TDelegate : Delegate
    {
        var method = ((MethodCallExpression)expression.Body).Method;
        methodName = method.GetCustomAttribute<GrpcMethodAttribute>()?.MethodName ?? method.Name;
        return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), method);
    }
}
}
