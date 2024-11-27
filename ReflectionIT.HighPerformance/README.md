# ReflectionIT.HighPerformance

A collection of helpers for working in high-performance scenarios. 
It includes APIs such as ``Uf8StringPool``, ``StringPool``, ``ConcurrentUf8StringPool`` and ``ConcurrentStringPool`` types. 
It utilizes the new performance improvements of .NET 9 like AlternateLookup.

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
