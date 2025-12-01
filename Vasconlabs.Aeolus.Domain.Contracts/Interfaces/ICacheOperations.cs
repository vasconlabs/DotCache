namespace Vasconlabs.Aeolus.Domain.Contracts.Interfaces;

public interface ICacheOperations
{
    public Task<ReadOnlyMemory<byte>> Get(ulong key);

    public Task Set(ulong key, ReadOnlyMemory<byte> value);

    public Task SaveSnapshot();
    
    public Task SaveLogCommit();
}