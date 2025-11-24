namespace Vasconlabs.Aeolus.Domain.Contracts.Cache;

public readonly struct CacheModel
{
    public readonly ReadOnlyMemory<byte> Data;
    
    public readonly ulong Ttl;
    
    public CacheModel(ReadOnlyMemory<byte> data, ulong ttl)
    {
        Data = data;
        Ttl = ttl;
    }
}