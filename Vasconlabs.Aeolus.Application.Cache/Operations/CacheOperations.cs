using FASTER.core;
using Vasconlabs.Aeolus.Application.Cache.Sessions;
using Vasconlabs.Aeolus.Application.Cache.Store;
using Vasconlabs.Aeolus.Domain.Contracts.Cache;
using Vasconlabs.Aeolus.Domain.Contracts.Interfaces;

namespace Vasconlabs.Aeolus.Application.Cache.Operations;

internal class CacheOperations(SessionPool sessionPool, CacheStore cacheStore): ICacheOperations
{
    public async Task Set(string key, CacheModel value)
    {
        ClientSession<string, CacheModel, CacheModel, CacheModel, Empty, IFunctions<string, CacheModel, CacheModel, CacheModel, Empty>> session = sessionPool.RentSession();
        
        try
        {
            await session.UpsertAsync(ref key, ref value);
        }
        finally
        {
            sessionPool.ReturnSession(session);
        }
    }

    public async Task<CacheModel?> Get(string key)
    {
        ClientSession<string, CacheModel, CacheModel, CacheModel, Empty, IFunctions<string, CacheModel, CacheModel, CacheModel, Empty>> session = sessionPool.RentSession();
        
        try
        {
            FasterKV<string, CacheModel>.ReadAsyncResult<CacheModel, CacheModel, Empty> result = await session.ReadAsync(ref key);
            
            return result.Status.Found ? result.Output : null;
        }
        finally
        {
            sessionPool.ReturnSession(session);
        }
    }

    public async Task SaveSnapshot()
    {
        foreach (var session in sessionPool.ReturnAllSessions())
            await session.CompletePendingAsync();
        
        await cacheStore.Store.TakeFullCheckpointAsync(CheckpointType.FoldOver);
    }
}