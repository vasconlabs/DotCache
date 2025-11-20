namespace Vasconlabs.Aeolus.Domain.Contracts.Cache;

public readonly struct CacheModel
{
    public readonly ReadOnlyMemory<byte> Data;
    
    public CacheModel(ReadOnlyMemory<byte> data) => Data = data;
}