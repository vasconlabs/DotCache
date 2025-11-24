using Grpc.Core;

namespace Vasconlabs.Aeolus.Interface.Server.Marshaller;

public static class RawMarshaller
{
    public static readonly Marshaller<ReadOnlyMemory<byte>> Instance = new Marshaller<ReadOnlyMemory<byte>>(
            serializer: (value, ctx) =>
            {
                ctx.SetPayloadLength(value.Length);
                ctx.Complete(value.Span);
            },
            deserializer: ctx =>
            {
                return ctx.PayloadAsReadOnlyMemory();
            });
}