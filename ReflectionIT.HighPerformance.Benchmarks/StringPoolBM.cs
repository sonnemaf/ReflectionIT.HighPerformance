using BenchmarkDotNet.Attributes;
using ReflectionIT.HighPerformance.Buffers;

namespace ReflectionIT.HighPerformance.Benchmarks;

//[ShortRunJob]
[HideColumns("Job", "StdDev", "RatioSD")]
[MemoryDiagnoser(displayGenColumns: true)]
public class StringPoolBM {

    private readonly List<char[]> _words = [];

    [GlobalSetup]
    public void GlobalSetup() {
        for (var i = 0; i < 1000; i++) {
            Add("The");
            Add("quick");
            Add("brown");
            Add("fox");
            Add("jumped");
            Add("over");
            Add("the");
            Add("lazy");
            Add("dog");
        }
        void Add(ReadOnlySpan<char> bytes) {
            _words.Add(bytes.ToArray());
        }
    }

    [Benchmark]
    public void StringPool() {
        var pool = new StringPool(20);
        foreach (var item in _words) {
            _ = pool.GetOrAdd(item);
        }
    }

    [Benchmark]
    public void CommunityToolkitStringPool() {
        var pool = new CommunityToolkit.HighPerformance.Buffers.StringPool(20);
        foreach (var item in _words) {
            _ = pool.GetOrAdd(item);
        }
    }

    [Benchmark(Baseline = true)]
    public void NewString() {
        foreach (var item in _words) {
            _ = new string(item);
        }
    }

}
