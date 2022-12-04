namespace GrpcMini
{
public interface IAsyncStreamReader<out T>
{
    T Current { get; }
    Task<bool> MoveNext(CancellationToken cancellationToken = default);
}
}
