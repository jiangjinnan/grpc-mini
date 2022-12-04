namespace GrpcMini
{
public interface IAsyncStreamWriter<in T>
{
    Task WriteAsync(T message, CancellationToken cancellationToken = default);
}
public interface IServerStreamWriter<in T> : IAsyncStreamWriter<T>
{
}
public interface IClientStreamWriter<in T> : IAsyncStreamWriter<T>
{
    Task CompleteAsync();
}
}
