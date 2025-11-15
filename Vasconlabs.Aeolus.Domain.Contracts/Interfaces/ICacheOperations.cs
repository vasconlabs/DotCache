using Vasconlabs.Aeolus.Domain.Contracts.Cache;

namespace Vasconlabs.Aeolus.Domain.Contracts.Interfaces;

public interface ICacheOperations
{
    public Task<CacheModel?> Get(string key);
    
    public Task Set(string key, CacheModel value);
    
    public Task SaveSnapshot();
}