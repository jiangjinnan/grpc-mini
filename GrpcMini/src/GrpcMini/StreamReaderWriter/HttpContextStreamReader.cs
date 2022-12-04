using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using System.Buffers;
using System.IO.Pipelines;

namespace GrpcMini
{
public class HttpContextStreamReader<T> : IAsyncStreamReader<T> where T : IMessage<T>
{
    private readonly PipeReader _reader;
    private readonly MessageParser<T> _parser;
    private ReadOnlySequence<byte> _buffer;
    public HttpContextStreamReader(HttpContext httpContext, MessageParser<T> parser)
    {
        _reader = httpContext.Request.BodyReader;
        _parser = parser;
    }
    public T Current { get; private set; } = default!;
    public async Task<bool> MoveNext(CancellationToken cancellationToken)
    {
        var completed = false;
        if (_buffer.IsEmpty)
        {
            var result = await _reader.ReadAsync(cancellationToken);
            _buffer = result.Buffer;
            completed = result.IsCompleted;
        }
        if (Buffers.TryReadMessage(_parser, ref _buffer, out var mssage))
        {
            Current = mssage!;
            _reader.AdvanceTo(_buffer.Start, _buffer.End);
            return true;
        }
        _reader.AdvanceTo(_buffer.Start, _buffer.End);
        _buffer = default;
        return !completed && await MoveNext(cancellationToken);
    }
}
}
