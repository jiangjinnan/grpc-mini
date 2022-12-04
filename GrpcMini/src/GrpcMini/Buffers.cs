using Google.Protobuf;
using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;

namespace GrpcMini
{
internal static class Buffers
{
    public static readonly int HeaderLength = 5;
    public static bool TryReadMessage<TRequest>(MessageParser<TRequest> parser, ref ReadOnlySequence<byte> buffer, out TRequest? message) where TRequest: IMessage<TRequest> 
    {
        if (buffer.Length < HeaderLength)
        {
            message = default;
            return false;
        }

        Span<byte> lengthBytes = stackalloc byte[4];
        buffer.Slice(1, 4).CopyTo(lengthBytes);
        var length = BinaryPrimitives.ReadInt32BigEndian(lengthBytes);
        if (buffer.Length < length + HeaderLength)
        {
            message = default;
            return false;
        }

        message = parser.ParseFrom(buffer.Slice(HeaderLength, length));
        buffer = buffer.Slice(length + HeaderLength);
        return true;
    }
}
}
