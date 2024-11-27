using BenchmarkDotNet.Running;
using ReflectionIT.HighPerformance.Benchmarks;

BenchmarkRunner.Run<Utf8StringPoolBM>();
BenchmarkRunner.Run<StringPoolBM>();
