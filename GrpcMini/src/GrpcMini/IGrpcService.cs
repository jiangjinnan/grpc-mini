namespace GrpcMini
{
public interface  IGrpcService<TService> where TService:class
{
    static abstract void Bind(IServiceBinder<TService> binder);
}
}
