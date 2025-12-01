using System.Buffers.Binary;
using Google.Protobuf;
using Grpc.Core;
using Vasconlabs.Aeolus.Domain.Contracts.Interfaces;
using Vasconlabs.Aeolus.Domain.Contracts.V1;

namespace Vasconlabs.Aeolus.Interface.Server.Services;

public class AeolusCacheService(ICacheOperations cacheOperations) : AeolusCacheGrpcService.AeolusCacheGrpcServiceBase
{
    public override async Task Get(IAsyncStreamReader<RawMessage> requestStream, IServerStreamWriter<RawMessage> responseStream, ServerCallContext context)
    {
        RawMessage notFoundMsg = new RawMessage 
        {
            Data = UnsafeByteOperations.UnsafeWrap(Array.Empty<byte>())
        }; 

        await foreach (RawMessage msg in requestStream.ReadAllAsync())
        {
            if (msg.Data.Length < 8) 
            {
                await responseStream.WriteAsync(notFoundMsg);

                continue;
            }

            ReadOnlyMemory<byte> result = await cacheOperations.Get(BinaryPrimitives.ReadUInt64LittleEndian(msg.Data.Span));
            
            if (result.Length == 0)
            {
                await responseStream.WriteAsync(notFoundMsg);

                continue;
            }
            
            ulong ttl = BinaryPrimitives.ReadUInt64LittleEndian(result[..8].Span);
            
            if ((ulong)DateTime.UtcNow.Ticks > ttl)
            {
                await responseStream.WriteAsync(notFoundMsg);

                continue;
            }

            await responseStream.WriteAsync(new RawMessage
            {
                Data = UnsafeByteOperations.UnsafeWrap(result[8..])
            });
        }
    }
    
    public override async Task Set(IAsyncStreamReader<RawMessage> requestStream, IServerStreamWriter<RawMessage> responseStream, ServerCallContext context)
    {
        RawMessage okMsg = new RawMessage
        {
            Data = UnsafeByteOperations.UnsafeWrap(Array.Empty<byte>())
        };

        await foreach (RawMessage msg in requestStream.ReadAllAsync())
        {
            if (msg.Data.Length < 16)
            {
                await responseStream.WriteAsync(okMsg);

                continue;
            } 

            ReadOnlyMemory<byte> span = msg.Data.Memory;
            
            await cacheOperations.Set(BinaryPrimitives.ReadUInt64LittleEndian(span.Slice(0, 8).Span), span.Slice(8));
            
            await responseStream.WriteAsync(okMsg);
        }
    }

    public override Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
    {
        return base.Delete(request, context);
    }

    public override Task<TTLResponse> TTL(TTLRequest request, ServerCallContext context)
    {
        return base.TTL(request, context);
    }

    public override Task<FlushAllResponse> FlushAll(FlushAllRequest request, ServerCallContext context)
    {
        return base.FlushAll(request, context);
    }
}
