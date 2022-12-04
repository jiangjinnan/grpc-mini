namespace GrpcMini
{
[AttributeUsage(AttributeTargets.Class)]
public class GrpcServiceAttribute: Attribute
{
    public string? ServiceName { get; set; }
}

[AttributeUsage(AttributeTargets.Method)]
public class GrpcMethodAttribute : Attribute
{
    public string? MethodName { get; set; }
}
}
