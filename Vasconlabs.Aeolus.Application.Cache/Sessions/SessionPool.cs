using FASTER.core;
using System.Collections.Concurrent;
using Vasconlabs.Aeolus.Application.Cache.Store;

namespace Vasconlabs.Aeolus.Application.Cache.Sessions;

internal class SessionPool
{
    private readonly FasterKV<ulong, byte[]> _store;
    private readonly CacheFunctions _functions;
    private readonly ConcurrentBag<AeolusSession> _pool;

    public SessionPool(CacheStore cacheStore, int initialSessions = 4)
    {
        _store = cacheStore.Store;
        _functions = new CacheFunctions();
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