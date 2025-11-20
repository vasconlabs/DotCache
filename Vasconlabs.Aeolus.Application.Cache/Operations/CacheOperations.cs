using FASTER.core;
using Vasconlabs.Aeolus.Application.Cache.Sessions;
using Vasconlabs.Aeolus.Application.Cache.Store;
using Vasconlabs.Aeolus.Domain.Contracts.Cache;
using Vasconlabs.Aeolus.Domain.Contracts.Interfaces;

namespace Vasconlabs.Aeolus.Application.Cache.Operations;

internal class CacheOperations(SessionPool sessionPool, CacheStore cacheStore): ICacheOperations
{
    public void Set(ulong key, CacheModel value)
    {
        ClientSession<ulong, CacheModel, CacheModel, CacheModel, Empty, IFunctions<ulong, CacheModel, CacheModel, CacheModel, Empty>> session = sessionPool.RentSession();
        
        try
        {
            session.Upsert(ref key, ref value);
        }
        finally
        {
            sessionPool.ReturnSession(session);
        }
    }

    public ReadOnlyMemory<byte>? Get(ulong key)
    {
        ClientSession<ulong, CacheModel, CacheModel, CacheModel, Empty, IFunctions<ulong, CacheModel, CacheModel, CacheModel, Empty>> session = sessionPool.RentSession();
        
        try
        {
            CacheModel input = default;
            CacheModel output = default;

            Status status = session.Read(ref key, ref input, ref output);

            if (!status.IsPending) return status.Found ? output.Data : null;
            
            FasterKV<ulong, CacheModel>.ReadAsyncResult<CacheModel, CacheModel, Empty> result = session.ReadAsync(ref key, ref input).GetAwaiter().GetResult();
            
            return result.Status.Found ? result.Output.Data : null;

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
    
    public async Task SaveLogCommit()
    {
        await cacheStore.Store.TakeHybridLogCheckpointAsync(CheckpointType.Snapshot);
    }
}