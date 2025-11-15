using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Grpc.Net.Client;
using StackExchange.Redis;
using Vasconlabs.Aeolus.Domain.Contracts.V1;

namespace Vasconlabs.Aeolus.Test.Benchmark;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class CacheBenchmarks
{
    private AeolusCacheGrpcService.AeolusCacheGrpcServiceClient _grpcClient = null!;
    private IDatabase _redisDb = null!;

    [GlobalSetup]
    public void Setup()
    {
        var channel = GrpcChannel.ForAddress("http://127.0.0.1:8080");
        _grpcClient = new AeolusCacheGrpcService.AeolusCacheGrpcServiceClient(channel);

        // Redis client
        var redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        _redisDb = redis.GetDatabase();
    }

    // ---------------- gRPC ----------------

    [Benchmark]
    public async Task GrpcSet()
    {
        var key = $"key-{Guid.NewGuid()}";
        var request = new SetRequest
        {
            Key = key,
            Value = Google.Protobuf.ByteString.CopyFromUtf8("val"),
            TtlSeconds = 60
        };
        await _grpcClient.SetAsync(request);
    }

    [Benchmark]
    public async Task GrpcGet()
    {
        var key = "key-test";
        var request = new GetRequest { Key = key };
        await _grpcClient.GetAsync(request);
    }

    // ---------------- Redis ----------------

    [Benchmark]
    public async Task RedisSet()
    {
        var key = $"key-{Guid.NewGuid()}";
        await _redisDb.StringSetAsync(key, "val", TimeSpan.FromSeconds(60));
    }

    [Benchmark]
    public async Task RedisGet()
    {
        var key = "key-test";
        await _redisDb.StringGetAsync(key);
    }
}

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<CacheBenchmarks>();
    }
}