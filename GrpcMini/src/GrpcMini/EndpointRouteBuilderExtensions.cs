using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace GrpcMini
{
public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapGrpcService2<TService>(this IEndpointRouteBuilder routeBuilder) 
        where TService : class, IGrpcService<TService>
    { 

        var binder = new ServiceBinder<TService>(routeBuilder);
        TService.Bind(binder);
        return routeBuilder;
    }
}
}
