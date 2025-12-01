```

BenchmarkDotNet v0.15.6, Windows 11 (10.0.26200.7171)
13th Gen Intel Core i5-1345U 1.60GHz, 1 CPU, 12 logical and 10 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3
  DefaultJob : .NET 10.0.0 (10.0.0, 10.0.25.52411), X64 RyuJIT x86-64-v3


```
| Method  | Mean     | Error   | StdDev   | Completed Work Items | Lock Contentions | Allocated |
|-------- |---------:|--------:|---------:|---------------------:|-----------------:|----------:|
| GrpcSet | 343.2 μs | 6.14 μs |  7.76 μs |               6.0254 |           0.0439 |    2.6 KB |
| GrpcGet | 260.5 μs | 7.27 μs | 20.26 μs |               6.0068 |           0.0264 |    2.6 KB |
