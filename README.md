# ReflectionIT.HighPerformance

A collection of helpers for working in high-performance scenarios. 
It includes APIs such as ``Uf8StringPool`` and ``StringPool`` types. 
It utilizes the new performance improvements of .NET 9 like AlternateLookup.

# NuGet packages

| Package | Version |
| ------ | ------ |
| ReflectionIT.HighPerformance | [![NuGet](https://img.shields.io/nuget/v/ReflectionIT.HighPerformance)](https://www.nuget.org/packages/ReflectionIT.HighPerformance/) |    


# Usage examples

## Utf8StringPool

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

## StringPool

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

# Benchmarks

This solution also contains [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) benchmarks for the ``Utf8StringPool`` and the ``StringPool`` classes.

## Utf8StringPool

Utf8Encode using ``Encoding.UTF8.GetString()`` is the fastest but also allocates the most memory. 
The ``StringPool`` is fater than the ``Utf8StringPool`` but also uses more memory.

[Utf8StringPool.cs](https://github.com/sonnemaf/ReflectionIT.HighPerformance/blob/master/ReflectionIT.HighPerformance.Benchmarks/UtfStringPoolBM.cs)

```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
Intel Core i7-10875H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=DefaultJob  

```
| Method         | Mean     | Error    | Ratio | Gen0   | Allocated | Alloc Ratio |
|--------------- |---------:|---------:|------:|-------:|----------:|------------:|
| Utf8StringPool | 16.27 μs | 0.055 μs |  1.00 | 0.1526 |   1.28 KB |        1.00 |
| StringPool     | 20.04 μs | 0.240 μs |  1.23 | 0.1221 |   1.06 KB |        0.83 |
| Utf8Encode     | 13.16 μs | 0.186 μs |  0.81 | 3.5248 |  28.91 KB |       22.56 |


## StringPool

Creating new strings is the fastest but also allocates the most memory. 
The ``StringPool`` is fater than the ``CommunityToolkit.HighPerformance.Buffers.StringPool``.


[StringPool.cs](https://github.com/sonnemaf/ReflectionIT.HighPerformance/blob/master/ReflectionIT.HighPerformance.Benchmarks/StringPoolBM.cs)

```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.22631.4460/23H2/2023Update/SunValley3)
Intel Core i7-10875H CPU 2.30GHz, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.100
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX2

Job=DefaultJob  

```
| Method                     | Mean      | Error     | Ratio | Gen0   | Allocated | Alloc Ratio |
|--------------------------- |----------:|----------:|------:|-------:|----------:|------------:|
| StringPool                 |  9.644 μs | 0.1712 μs |  1.00 | 0.0916 |     872 B |        1.00 |
| CommunityToolkitStringPool | 33.457 μs | 0.5034 μs |  3.47 | 0.1831 |    1920 B |        2.20 |
| NewString                  |  6.316 μs | 0.0828 μs |  0.66 | 3.5324 |   29600 B |       33.94 |
