using System.Buffers;
using FASTER.core;
using Vasconlabs.Aeolus.Application.Cache.Sessions;
using Vasconlabs.Aeolus.Application.Cache.Store;
using Vasconlabs.Aeolus.Domain.Contracts.Interfaces;

namespace Vasconlabs.Aeolus.Application.Cache.Operations;

using AeolusSession = ClientSession<byte[], SpanByte, SpanByte, SpanByteAndMemory, Empty, IFunctions<byte[], SpanByte, SpanByte, SpanByteAndMemory, Empty>>;

internal class CacheOperations(SessionPool sessionPool, CacheStore cacheStore): ICacheOperations
{
    public void Set(ReadOnlySpan<byte> key, ReadOnlySpan<byte> value)
    {
        AeolusSession session = sessionPool.RentSession();
        
        byte[] keyBuffer = ArrayPool<byte>.Shared.Rent(8);
        byte[] valBuf = ArrayPool<byte>.Shared.Rent(value.Length);
        
        try
        {
            
            key.CopyTo(keyBuffer);
            value.CopyTo(valBuf);

            SpanByte sb = SpanByte.FromPinnedMemory(valBuf);
            sb.Length = value.Length;

            session.Upsert(ref keyBuffer, ref sb);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(keyBuffer);
            ArrayPool<byte>.Shared.Return(valBuf);
            sessionPool.ReturnSession(session);
        }
    }

    public ReadOnlyMemory<byte> Get(ReadOnlySpan<byte> key)
    {
        AeolusSession session = sessionPool.RentSession();
        byte[] keyBuffer = ArrayPool<byte>.Shared.Rent(8);

        try
        {
            key.CopyTo(keyBuffer);

            SpanByte input = default;
            SpanByteAndMemory output = default;

            Status status = session.Read(ref keyBuffer, ref input, ref output);

            if (!status.Found) return ReadOnlyMemory<byte>.Empty;

            ReadOnlySpan<byte> src = output.IsSpanByte ?
                output.SpanByte.AsReadOnlySpan() :
                output.Memory.Memory.Span;

            byte[] result = ArrayPool<byte>.Shared.Rent(src.Length);
            src.CopyTo(result);

            return result.AsMemory(0, src.Length);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(keyBuffer);
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