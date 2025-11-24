using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Vasconlabs.Aeolus.Client;
using Vasconlabs.Aeolus.Client.Interfaces;

namespace Vasconlabs.Aeolus.Test.Benchmark;

[MemoryDiagnoser]
[ThreadingDiagnoser]
public class CacheBenchmarks
{
    private IDatabase _redisDb = null!;
    private IAeolusCacheService _cache = null!;

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddAeolusClient(opts =>
        {
            opts.BaseUrl = "https://localhost:7246";
        });

        var provider = services.BuildServiceProvider();

        _cache = provider.GetRequiredService<IAeolusCacheService>();
        
        var redis = ConnectionMultiplexer.Connect("127.0.0.1:6379");
        
        _redisDb = redis.GetDatabase();
    }

    // ---------------- gRPC ----------------
    
    [Benchmark]
    public async Task GrpcSet()
    {
        await _cache.SetAsync("test-key", new byte[] { 1, 2, 3 });
    }

    [Benchmark]
    public async Task GrpcGet()
    {
        await _cache.GetAsync("test-key");
    }
    // ---------------- Redis ----------------

    [Benchmark]
    public async Task RedisSet()
    {
        await _redisDb.StringSetAsync("test-key", new byte[] { 1, 2, 3 }, TimeSpan.FromSeconds(60));
    }
    
    [Benchmark]
    public async Task RedisGet()
    {
        await _redisDb.StringGetAsync("test-key");
    }
}

class Program
{
    static void Main(string[] args)
    {
        var services = new ServiceCollection();
        
        services.AddAeolusClient(opts =>
        {
            opts.BaseUrl = "https://localhost:7246"; 
        });
        
        BenchmarkRunner.Run<CacheBenchmarks>();
    }
}