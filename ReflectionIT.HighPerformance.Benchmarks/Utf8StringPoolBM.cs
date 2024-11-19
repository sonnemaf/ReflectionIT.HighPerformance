using BenchmarkDotNet.Attributes;
using ReflectionIT.HighPerformance.Buffers;
using System.Text;

namespace ReflectionIT.HighPerformance.Benchmarks;

//[ShortRunJob]
[HideColumns("Job", "StdDev", "RatioSD")]
[MemoryDiagnoser(displayGenColumns: true)]
public class Utf8StringPoolBM {

    private readonly List<byte[]> _words = [];

    [GlobalSetup]
    public void GlobalSetup() {
        for (var i = 0; i < 100; i++) {
            Add("The"u8);
            Add("quick"u8);
            Add("brown"u8);
            Add("fox"u8);
            Add("jumped"u8);
            Add("over"u8);
            Add("the"u8);
            Add("lazy"u8);
            Add("dog"u8);
        }
        void Add(ReadOnlySpan<byte> bytes) {
            _words.Add(bytes.ToArray());
        }
    }

    [Benchmark(Baseline = true)]
    public void Utf8StringPool() {
        var pool = new Utf8StringPool();
        foreach (var item in _words) {
            _ = pool.GetOrAdd(item);
        }
    }

    [Benchmark()]
    public void ConcurrentUtf8StringPool() {
        var pool = new ConcurrentUtf8StringPool();
        foreach (var item in _words) {
            _ = pool.GetOrAdd(item);
        }
    }

    [Benchmark]
    public void StringPool() {
        var pool = new StringPool();
        foreach (var item in _words) {
            _ = pool.GetOrAdd(item);
        }
    }

    [Benchmark]
    public void Utf8Encode() {
        foreach (var item in _words) {
            _ = Encoding.UTF8.GetString(item);
        }
    }

}
