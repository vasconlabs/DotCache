namespace Vasconlabs.Aeolus.Domain.Contracts.Interfaces;

public interface ICacheOperations
{
    public ReadOnlyMemory<byte> Get(ReadOnlySpan<byte> key);
    
    public void Set(ReadOnlySpan<byte> key, ReadOnlySpan<byte> value);
    
    public Task SaveSnapshot();
    
    public Task SaveLogCommit();
}