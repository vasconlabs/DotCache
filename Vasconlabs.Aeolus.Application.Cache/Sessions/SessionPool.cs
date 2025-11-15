using System.Collections.Concurrent;
using FASTER.core;
using Vasconlabs.Aeolus.Application.Cache.Store;
using Vasconlabs.Aeolus.Domain.Contracts.Cache;

namespace Vasconlabs.Aeolus.Application.Cache.Sessions;

internal class SessionPool
{
    private readonly FasterKV<string, CacheModel> _store;
    private readonly SimpleFunctions<string, CacheModel> _functions;
    private readonly ConcurrentBag<ClientSession<string, CacheModel, CacheModel, CacheModel, Empty, IFunctions<string, CacheModel, CacheModel, CacheModel, Empty>>> _pool;
    
    public SessionPool(CacheStore cacheStore, int initialSessions = 4)
    {
        _store = cacheStore.Store;
        _functions = new SimpleFunctions<string, CacheModel>();
        _pool = new ConcurrentBag<ClientSession<string, CacheModel, CacheModel, CacheModel, Empty, IFunctions<string, CacheModel, CacheModel, CacheModel, Empty>>>();

        for (int i = 0; i < initialSessions; i++)
            _pool.Add(_store.NewSession(_functions));

    }
    
    public ClientSession<string, CacheModel, CacheModel, CacheModel, Empty, IFunctions<string, CacheModel, CacheModel, CacheModel, Empty>> RentSession()
    {
        return _pool.TryTake(out var session) ? session : _store.NewSession(_functions);
    }

    public void ReturnSession(ClientSession<string, CacheModel, CacheModel, CacheModel, Empty, IFunctions<string, CacheModel, CacheModel, CacheModel, Empty>> session)
    {
        _pool.Add(session);
    }
    
    public IEnumerable<ClientSession<string,CacheModel,CacheModel,CacheModel,Empty,IFunctions<string,CacheModel,CacheModel,CacheModel,Empty>>> ReturnAllSessions()
    {
        return _pool.ToArray();
    }
}