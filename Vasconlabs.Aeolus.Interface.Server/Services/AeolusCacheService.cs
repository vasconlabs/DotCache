using System.Buffers;
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
        RawMessage notFoundMsg = new RawMessage(); 

        await foreach (RawMessage msg in requestStream.ReadAllAsync())
        {
            if (msg.Data.Length < 8) continue;
            
            ReadOnlyMemory<byte> result = cacheOperations.Get(msg.Data.Span);
            
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
            
            byte[] buffer = ArrayPool<byte>.Shared.Rent(result.Length);
                
            try
            {
                result.CopyTo(buffer);
                
                await responseStream.WriteAsync(new RawMessage
                {
                    Data = UnsafeByteOperations.UnsafeWrap(buffer)
                });
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
    
    public override async Task Set(IAsyncStreamReader<RawMessage> requestStream, IServerStreamWriter<RawMessage> responseStream, ServerCallContext context)
    {
        RawMessage okMsg = new RawMessage();
        
        await foreach (RawMessage msg in requestStream.ReadAllAsync())
        {
            if (msg.Data.Length < 16) continue;

            ReadOnlySpan<byte> span = msg.Data.Span;
            
            cacheOperations.Set(span.Slice(0, 8), span.Slice(8));
            
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
