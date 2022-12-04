using Google.Protobuf;
using System.Buffers.Binary;
using System.IO.Pipelines;

namespace GrpcMini
{
    public static class PipeReaderWriterExtensions
    {
public static ValueTask<FlushResult> WriteMessageAsync(this PipeWriter writer, IMessage message)
{
    var length = message.CalculateSize();
    var span = writer.GetSpan(5 + length);
    span[0] = 0;
    BinaryPrimitives.WriteInt32BigEndian(span.Slice(1, 4), length);
    message.WriteTo(span.Slice(5, length));
    writer.Advance(5 + length);
    return writer.FlushAsync();
}

public static async Task<TMessage> ReadSingleMessageAsync<TMessage>(this PipeReader reader, MessageParser<TMessage> parser) where TMessage:IMessage<TMessage>
{
    while (true)
    {
        var result = await reader.ReadAsync();
        var buffer = result.Buffer;
        if (Buffers.TryReadMessage(parser, ref buffer, out var message))
        {
            return message!;
        }
        reader.AdvanceTo(buffer.Start, buffer.End);
        if (result.IsCompleted)
        {
            break;
        }
    }
    throw new IOException("Fails to read message.");
}
    }
}
