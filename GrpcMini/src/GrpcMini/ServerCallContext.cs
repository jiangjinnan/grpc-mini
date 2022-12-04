using Microsoft.AspNetCore.Http;

namespace GrpcMini
{
public class ServerCallContext
{
    public StatusCode StatusCode { get; set; } = StatusCode.OK;
    public HttpContext HttpContext { get; }
    public ServerCallContext(HttpContext httpContext)=> HttpContext = httpContext;
}
}