# ReflectionIT.HighPerformance

A collection of helpers for working in high-performance scenarios. 
It includes APIs such as ``Uf8StringPool``, ``StringPool``, ``ConcurrentUf8StringPool`` and ``ConcurrentStringPool`` types. 
It utilizes the new performance improvements of .NET 9 like AlternateLookup.

## NuGet packages

| Package | Version |
| ------ | ------ |
| ReflectionIT.HighPerformance | [![NuGet](https://img.shields.io/nuget/v/ReflectionIT.HighPerformance)](https://www.nuget.org/packages/ReflectionIT.HighPerformance/) |    

## Get Started

ReflectionIT.HighPerformance can be installed using the Nuget package manager or the `dotnet` CLI.

```
dotnet add package ReflectionIT.HighPerformance
```

## Usage examples

### Utf8StringPool

```cs
using ReflectionIT.HighPerformance.Buffers;

ReadOnlySpan<byte> text = "abc,def,xyz,abc,def,xyz,abc,def,xyz"u8;

foreach (Range range in text.Split((byte)',')) {
    ReadOnlySpan<byte> bytes = text[range];

    string word = Utf8StringPool.Shared.GetOrAdd(bytes);

    Console.WriteLine(word);
}

Console.WriteLine(Utf8StringPool.Shared.Count); // 3
```

### StringPool

Case sensitive StringPool

```cs
using ReflectionIT.HighPerformance.Buffers;

ReadOnlySpan<char> text = "abc,def,xyz,abc,def,xyz,abc,def,xyz";

foreach (Range range in text.Split(',')) {
    ReadOnlySpan<char> chars = text[range];

    string word = StringPool.Shared.GetOrAdd(chars);

    Console.WriteLine(word);
}

Console.WriteLine(StringPool.Shared.Count); // 3
```

Case insensitive StringPool with an initial capacity of 20

```cs
using ReflectionIT.HighPerformance.Buffers;

ReadOnlySpan<char> text = "abc,def,xyz,ABC,DEF,XYZ,Abc,Def,Xyz";

StringPool pool = new StringPool(capacity: 20, StringComparer.OrdinalIgnoreCase);

foreach (Range range in text.Split(',')) {
    ReadOnlySpan<char> chars = text[range];

    string word = pool.GetOrAdd(chars);

    Console.WriteLine(word);
}

Console.WriteLine(pool.Count); // 3
```


Using string interpolation

```cs
using ReflectionIT.HighPerformance.Buffers;

for (int i = 0; i < 100; i++) {
    for (int j = 0; j < 10; j++) {
        string s = StringPool.Shared.GetOrAdd($"Hello World {j}");
    }
}
Console.WriteLine(StringPool.Shared.Count); // 10
```

## Benchmarks

This solution also contains [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) benchmarks for the ``Utf8StringPool`` and the ``StringPool`` classes.

### Utf8StringPool

Utf8Encode using ``Encoding.UTF8.GetString()`` is the fastest but also allocates the most memory. 
The ``StringPool`` is fater than the ``Utf8StringPool`` but also uses more memory.

[Utf8StringPoolBM.cs](https://github.com/sonnemaf/ReflectionIT.HighPerformance/blob/master/ReflectionIT.HighPerformance.Benchmarks/Utf8StringPoolBM.cs)

```
BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4541/23H2/2023Update/SunValley3)
Intel Core i7-10875H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=DefaultJob  
```
| Method                   | Mean     | Error    | Ratio | Gen0   | Allocated | Alloc Ratio |
|------------------------- |---------:|---------:|------:|-------:|----------:|------------:|
| Utf8StringPool           | 16.32 μs | 0.313 μs |  1.00 | 0.1526 |   1.28 KB |        1.00 |
| ConcurrentUtf8StringPool | 15.87 μs | 0.308 μs |  0.97 | 0.1831 |   1.72 KB |        1.34 |
| StringPool               | 20.40 μs | 0.022 μs |  1.25 | 0.1221 |   1.06 KB |        0.83 |
| Utf8Encode               | 13.07 μs | 0.240 μs |  0.80 | 3.5248 |  28.91 KB |       22.56 |

### StringPool

Creating new strings is the fastest but also allocates the most memory. 
The ``StringPool`` is fater than the ``CommunityToolkit.HighPerformance.Buffers.StringPool``.


[StringPoolBM.cs](https://github.com/sonnemaf/ReflectionIT.HighPerformance/blob/master/ReflectionIT.HighPerformance.Benchmarks/StringPoolBM.cs)

```
BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4541/23H2/2023Update/SunValley3)
Intel Core i7-10875H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]   : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  ShortRun : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1  
WarmupCount=3  
```
| Method                     | Mean      | Error     | Ratio | Gen0   | Allocated | Alloc Ratio |
|--------------------------- |----------:|----------:|------:|-------:|----------:|------------:|
| StringPool                 |  9.917 μs | 0.1990 μs |  1.00 | 0.0916 |     872 B |        1.00 |
| ConcurrentStringPool       | 10.643 μs | 0.6412 μs |  1.07 | 0.2747 |    2408 B |        2.76 |
| CommunityToolkitStringPool | 33.811 μs | 1.3866 μs |  3.41 | 0.1831 |    1920 B |        2.20 |
| NewString                  |  6.317 μs | 1.6826 μs |  0.64 | 3.5324 |   29600 B |       33.94 |

## Contributing

If you find an issue or have suggestions file an issue on the this repository.

### Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).

For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## License

MIT