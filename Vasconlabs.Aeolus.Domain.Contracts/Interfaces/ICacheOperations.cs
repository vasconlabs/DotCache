using Vasconlabs.Aeolus.Domain.Contracts.Cache;

namespace Vasconlabs.Aeolus.Domain.Contracts.Interfaces;

public interface ICacheOperations
{
    public ReadOnlyMemory<byte>? Get(ulong key);
    
    public void Set(ulong key, CacheModel value);
    
    public Task SaveSnapshot();
    
    public Task SaveLogCommit();
}