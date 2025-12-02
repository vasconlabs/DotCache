using FASTER.core;
using System.Collections.Concurrent;
using Vasconlabs.Aeolus.Application.Cache.Store;

namespace Vasconlabs.Aeolus.Application.Cache.Sessions;

internal class SessionPool
{
    private readonly TimeSpan _timeout;
    private readonly FasterKV<ulong, byte[]> _store;
    private readonly SessionCacheFunctions _functions;
    private readonly ConcurrentDictionary<Guid, SessionInfo> _pool;

    public int CreatedSessions { get; private set; }
    public int DisposedSessions { get; private set; }

    public SessionPool(CacheStore cacheStore, int initialSessions = 4)
    {
        _store = cacheStore.Store;
        _functions = new SessionCacheFunctions();
        _pool = new ConcurrentDictionary<Guid, SessionInfo>();

        for (int i = 0; i < initialSessions; i++)
        {
            _pool.TryAdd(Guid.CreateVersion7(), new SessionInfo(_store.NewSession(_functions)));

            CreatedSessions++;
        }
    }

    public AeolusSession RentSession()
    {
        foreach (Guid key in _pool.Keys)
        {
            if (_pool.TryRemove(key, out SessionInfo? sessionInfo))
            {
                sessionInfo.UpdateActivity();

                return sessionInfo.Session;
            }
        }

        SessionInfo newSessionInfo = new SessionInfo(_store.NewSession(_functions));

        _pool.TryAdd(Guid.CreateVersion7(), newSessionInfo);

        CreatedSessions++;

        return newSessionInfo.Session;
    }

    public void ReturnSession(AeolusSession session)
    {
        _pool.TryAdd(Guid.CreateVersion7(), new SessionInfo(session));
    }

    public ICollection<SessionInfo> ReturnAllSessions()
    {
        return _pool.Values;
    }

    private void CleanupInactiveSessions()
    {
        DateTime now = DateTime.UtcNow;
        KeyValuePair<Guid, SessionInfo>[] snapshot = _pool.ToArray();

        foreach ((Guid id, SessionInfo? info) in snapshot)
        {
            if (now - info.LastActivity > _timeout)
            {
                if (_pool.TryRemove(id, out var removed))
                {
                    removed.Session.Dispose();
                    DisposedSessions++;
                    Console.WriteLine($"[POOL] Sessão {id} descartada por inatividade.");
                }
            }
        }
    }

}