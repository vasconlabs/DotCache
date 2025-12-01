using FASTER.core;
using Vasconlabs.Aeolus.Application.Cache.Sessions;
using Vasconlabs.Aeolus.Application.Cache.Store;
using Vasconlabs.Aeolus.Domain.Contracts.Interfaces;

namespace Vasconlabs.Aeolus.Application.Cache.Operations;

internal class CacheOperations(SessionPool sessionPool, CacheStore cacheStore): ICacheOperations
{
    public async Task Set(ulong key, ReadOnlyMemory<byte> value)
    {
        AeolusSession session = sessionPool.RentSession();

        try
        {
            byte[] buffer = value.ToArray();

            await session.UpsertAsync(ref key, ref buffer);
        }
        finally
        {
            sessionPool.ReturnSession(session);
        }
    }

    public async Task<ReadOnlyMemory<byte>> Get(ulong key)
    {
        AeolusSession session = sessionPool.RentSession();

        try
        {
            var result = await session.ReadAsync(ref key);

            if (result.Status.Found) return new ReadOnlyMemory<byte>(result.Output);

            return ReadOnlyMemory<byte>.Empty;
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
    
    private static ReadOnlyMemory<byte> CopyToNewArray(SpanByteAndMemory output)
    {
        ReadOnlySpan<byte> src = output.IsSpanByte
            ? output.SpanByte.AsReadOnlySpan()
            : output.Memory.Memory.Span;

        byte[] arr = new byte[src.Length];
        src.CopyTo(arr);
        return arr;
    }
}