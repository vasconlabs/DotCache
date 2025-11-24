using System.Collections.Concurrent;
using FASTER.core;
using Vasconlabs.Aeolus.Application.Cache.Store;

namespace Vasconlabs.Aeolus.Application.Cache.Sessions;

using AeolusSession = ClientSession<byte[], SpanByte, SpanByte, SpanByteAndMemory, Empty, IFunctions<byte[], SpanByte, SpanByte, SpanByteAndMemory, Empty>>;

internal class SessionPool
{
    private readonly FasterKV<byte[], SpanByte> _store;
    private readonly SpanByteFunctions<byte[], SpanByteAndMemory, Empty>  _functions;
    private readonly ConcurrentBag<AeolusSession> _pool;

    public SessionPool(CacheStore cacheStore, int initialSessions = 4)
    {
        _store = cacheStore.Store;
        _functions = new SpanByteFunctions<byte[], SpanByteAndMemory, Empty> ();
        _pool = new ConcurrentBag<AeolusSession>();

        for (int i = 0; i < initialSessions; i++)
            _pool.Add(_store.NewSession(_functions));
    }

    public AeolusSession RentSession()
    {
        return _pool.TryTake(out var s) ? s : _store.NewSession(_functions);
    }

    public void ReturnSession(AeolusSession session)
    {
        _pool.Add(session);
    }

    public IEnumerable<AeolusSession> ReturnAllSessions()
    {
        return _pool.ToArray();
    }
}