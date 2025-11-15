```

BenchmarkDotNet v0.15.6, macOS 26.1 (25B78) [Darwin 25.1.0]
Apple M4 Pro, 1 CPU, 12 logical and 12 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a


```
| Method   | Mean     | Error   | StdDev   | Completed Work Items | Lock Contentions | Gen0   | Allocated |
|--------- |---------:|--------:|---------:|---------------------:|-----------------:|-------:|----------:|
| GrpcSet  | 329.9 μs | 6.58 μs | 10.43 μs |               7.9326 |           0.0005 | 0.4883 |    7352 B |
| GrpcGet  | 325.6 μs | 6.32 μs |  9.84 μs |               7.9302 |           0.0015 | 0.4883 |    7177 B |
| RedisSet | 142.0 μs | 2.80 μs |  3.55 μs |               3.0002 |           0.1467 |      - |     568 B |
| RedisGet | 136.0 μs | 2.65 μs |  4.21 μs |               3.0002 |           0.1399 |      - |     448 B |
