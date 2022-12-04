using GrpcMini;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Server;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(kestrel => kestrel.ConfigureEndpointDefaults(options => options.Protocols = HttpProtocols.Http2));
var app = builder.Build();
app.MapGrpcService2<GreeterService>();
app.Run();

