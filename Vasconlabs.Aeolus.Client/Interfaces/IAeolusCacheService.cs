namespace Vasconlabs.Aeolus.Client.Interfaces;

public interface IAeolusCacheService
{
    public Task<ReadOnlyMemory<byte>> GetAsync(string key);
    
    public Task SetAsync(string key, ReadOnlyMemory<byte> value, TimeSpan ttl);
}