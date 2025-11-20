using System.Buffers.Binary;
using FASTER.core;
using Vasconlabs.Aeolus.Domain.Contracts.Cache;

namespace Vasconlabs.Aeolus.Application.Cache.Serializer;

public sealed class CacheModelSerializer : IObjectSerializer<CacheModel>
{
    private Stream? _serializeStream;
    private Stream? _deserializeStream;

    public void BeginSerialize(Stream toStream) => _serializeStream = toStream;
    public void BeginDeserialize(Stream fromStream) => _deserializeStream = fromStream;
    public void EndSerialize() => _serializeStream = null;
    public void EndDeserialize() => _deserializeStream = null;

    public void Serialize(ref CacheModel obj)
    {
        var data = obj.Data.Span;
        Span<byte> lenBuf = stackalloc byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(lenBuf, data.Length);
        _serializeStream!.Write(lenBuf);
        
        if (!data.IsEmpty)
            _serializeStream.Write(data);
    }

    public void Deserialize(out CacheModel obj)
    {
        Span<byte> lenBuf = stackalloc byte[4];
        if (_deserializeStream!.Read(lenBuf) < 4) throw new EndOfStreamException();

        int len = BinaryPrimitives.ReadInt32LittleEndian(lenBuf);
        
        if (len is < 0 or > 100 * 1024 * 1024) 
            throw new InvalidDataException($"Invalid length: {len}");

        byte[] buffer = new byte[len];
        if (len > 0)
        {
            int read = _deserializeStream.Read(buffer, 0, len);
            if (read != len) throw new EndOfStreamException();
        }

        obj = new CacheModel(buffer);
    }
}