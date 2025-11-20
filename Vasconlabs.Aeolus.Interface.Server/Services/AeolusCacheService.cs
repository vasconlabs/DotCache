using Google.Protobuf;
using Grpc.Core;
using Vasconlabs.Aeolus.Domain.Contracts.Cache;
using Vasconlabs.Aeolus.Domain.Contracts.Interfaces;
using Vasconlabs.Aeolus.Domain.Contracts.V1;

namespace Vasconlabs.Aeolus.Interface.Server.Services;

public class AeolusCacheService(ICacheOperations cacheOperations): AeolusCacheGrpcService.AeolusCacheGrpcServiceBase
{
    public override Task<GetResponse> Get(GetRequest request, ServerCallContext context)
    {
        ReadOnlyMemory<byte>? model = cacheOperations.Get(request.Key);

        if (model is not null)
        {
            return Task.FromResult(new GetResponse
            {
                Value = UnsafeByteOperations.UnsafeWrap(model.Value),
                Found = true
            });            
        }
        
        return NotOkResponse;
    }

    public override Task<SetResponse> Set(CacheEntry request, ServerCallContext context)
    {
        cacheOperations.Set(request.Key, new CacheModel(request.Value.Memory));
        
        return OkResponse; 
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
    
    private static readonly Task<SetResponse> OkResponse = Task.FromResult(new SetResponse { Ok = true });
    private static readonly Task<GetResponse> NotOkResponse = Task.FromResult(new GetResponse());
}