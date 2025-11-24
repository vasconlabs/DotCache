using System.Buffers.Binary;
using FASTER.core;
using Vasconlabs.Aeolus.Domain.Contracts.Cache;

namespace Vasconlabs.Aeolus.Application.Cache.Serializer;

public sealed class CacheModelSerializer : IObjectSerializer<CacheModel>
{
    private const int MaxAllowedValueSize = 100 * 1024 * 1024;
    
    private Stream? _serializeStream;
    private Stream? _deserializeStream;

    public void BeginSerialize(Stream toStream) => _serializeStream = toStream;
    public void BeginDeserialize(Stream fromStream) => _deserializeStream = fromStream;
    public void EndSerialize() => _serializeStream = null;
    public void EndDeserialize() => _deserializeStream = null;

    public void Serialize(ref CacheModel obj)
    {
        ReadOnlySpan<byte> data = obj.Data.Span;
        
        if (data.Length > MaxAllowedValueSize)
            throw new InvalidDataException($"Value too large: {data.Length} bytes");
        
        Span<byte> header = stackalloc byte[12];
        
        BinaryPrimitives.WriteInt32LittleEndian(header, data.Length);
        
        BinaryPrimitives.WriteUInt64LittleEndian(header[4..], obj.Ttl);
        
        _serializeStream!.Write(header);
        
        if (!data.IsEmpty)
            _serializeStream.Write(data);
    }

    public void Deserialize(out CacheModel obj)
    {
        Span<byte> header = stackalloc byte[12];
        
        if (_deserializeStream!.Read(header) < 12) throw new EndOfStreamException();
        
        int len = BinaryPrimitives.ReadInt32LittleEndian(header[..4]);
        ulong ttl = BinaryPrimitives.ReadUInt64LittleEndian(header[4..12]);
        
        if (len is < 0 or > MaxAllowedValueSize)
            throw new InvalidDataException($"Invalid length: {len}");

        byte[] buffer = new byte[len];

        if (len > 0)
        {
            int read = _deserializeStream.Read(buffer, 0, len);
            if (read != len)
                throw new EndOfStreamException();
        }

        obj = new CacheModel(buffer, ttl);
    }
}