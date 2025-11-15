using Google.Protobuf;
using Grpc.Core;
using Vasconlabs.Aeolus.Domain.Contracts.Cache;
using Vasconlabs.Aeolus.Domain.Contracts.Interfaces;
using Vasconlabs.Aeolus.Domain.Contracts.V1;

namespace Vasconlabs.Aeolus.Interface.Server.Services;

public class AeolusCacheService(ICacheOperations cacheOperations): AeolusCacheGrpcService.AeolusCacheGrpcServiceBase
{
    public override async Task<GetResponse> Get(GetRequest request, ServerCallContext context)
    {
        CacheModel? model = await cacheOperations.Get(request.Key);

        if (model is not null)
        {
            return new GetResponse
            {
                Value = ByteString.CopyFrom(model.Value.AsSpan()),
                Found = true
            };            
        }
        
        return new GetResponse();
    }

    public override async Task<SetResponse> Set(SetRequest request, ServerCallContext context)
    {
        await cacheOperations.Set(request.Key, new CacheModel
        {
            Key = request.Key,
            Value = request.Value.ToByteArray(),
            Ttl = 0
        });
        
        return new SetResponse
        {
            Ok = true
        };
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