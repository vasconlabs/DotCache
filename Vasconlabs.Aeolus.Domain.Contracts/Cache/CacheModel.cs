namespace Vasconlabs.Aeolus.Domain.Contracts.Cache;

public class CacheModel
{
    public required string Key { get; init; }
    
    public required byte[] Value { get; init; }
    
    public long Ttl { get; init; }
    
    public ReadOnlyMemory<byte> AsMemory() => Value.AsMemory();
    
    public ReadOnlySpan<byte> AsSpan() => Value.AsSpan();
}