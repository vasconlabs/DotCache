using System.Buffers;
using System.Buffers.Binary;
using Google.Protobuf;
using Grpc.Core;
using Vasconlabs.Aeolus.Client.Extensions;
using Vasconlabs.Aeolus.Client.Interfaces;
using Vasconlabs.Aeolus.Domain.Contracts.V1;

namespace Vasconlabs.Aeolus.Client.Services;

internal class AeolusCacheService(AeolusCacheGrpcService.AeolusCacheGrpcServiceClient aeolusClient) : IAeolusCacheService
{
    private readonly AsyncDuplexStreamingCall<RawMessage, RawMessage> _setStream = aeolusClient.Set();
    private readonly AsyncDuplexStreamingCall<RawMessage, RawMessage> _getStream = aeolusClient.Get();
    
    public async Task<ReadOnlyMemory<byte>> GetAsync(string key)
    {
        byte[] keyBuffer = ArrayPool<byte>.Shared.Rent(8);

        try
        {
            BinaryPrimitives.WriteUInt64LittleEndian(keyBuffer.AsSpan(0, 8),  XxHash64.ComputeHash(key));

            RawMessage request = new()
            {
                Data = UnsafeByteOperations.UnsafeWrap(keyBuffer.AsMemory(0, 8))
            };

            await _getStream.RequestStream.WriteAsync(request);
            
            if (await _getStream.ResponseStream.MoveNext())
            {
                return _getStream.ResponseStream.Current.Data.Memory;
            }
            
            throw new Exception("Stream finalizado sem resposta");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(keyBuffer);
        }
    }

    public async Task SetAsync(string key, ReadOnlyMemory<byte> value, TimeSpan ttl)
    {
        int totalSize = 16 + value.Length;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(totalSize);

        try
        {
            Span<byte> span = buffer.AsSpan(0, totalSize);

            BinaryPrimitives.WriteUInt64LittleEndian(span[..8], XxHash64.ComputeHash(key));

            BinaryPrimitives.WriteUInt64LittleEndian(span[8..16], (ulong)(DateTime.UtcNow.Ticks + ttl.Ticks));

            value.Span.CopyTo(span[16..]);
            
            RawMessage request = new()
            {
                Data = UnsafeByteOperations.UnsafeWrap(buffer.AsMemory(0, totalSize))
            };
            
            await _setStream.RequestStream.WriteAsync(request);
            
            if (await _setStream.ResponseStream.MoveNext())
            {
                return;
            }
            
            throw new Exception("Stream finalizado sem resposta");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}