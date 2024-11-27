using ReflectionIT.HighPerformance.Helpers;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;

namespace ReflectionIT.HighPerformance.Buffers;

/// <summary>
/// Represents a thread safe pool of strings that can be reused to reduce memory allocations.
/// </summary>
public sealed class ConcurrentStringPool : IEnumerable<string> {

    /// <summary>
    /// Gets the shared instance of the <see cref="StringPool"/>.
    /// </summary>
    public static StringPool Shared { get; } = new();

    private readonly ConcurrentDictionary<string, string> _pool;
    private readonly ConcurrentDictionary<string, string>.AlternateLookup<ReadOnlySpan<char>> _alternateLookupPool;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringPool"/> class.
    /// </summary>
    public ConcurrentStringPool() {
        _pool = [];
        _alternateLookupPool = _pool.GetAlternateLookup<ReadOnlySpan<char>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringPool"/> class.
    /// </summary>
    /// <param name="capacity">The initial size of the StringPool.</param>
    public ConcurrentStringPool(int capacity) {
        _pool = new(-1, capacity);
        _alternateLookupPool = _pool.GetAlternateLookup<ReadOnlySpan<char>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringPool"/> class with a specified equality comparer.
    /// </summary>
    /// <param name="equalityComparer">The equality comparer to use for comparing strings.</param>
    public ConcurrentStringPool(IEqualityComparer<string> equalityComparer) {
        ArgumentNullException.ThrowIfNull(equalityComparer);
        _pool = new(equalityComparer);
        _alternateLookupPool = _pool.GetAlternateLookup<ReadOnlySpan<char>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringPool"/> class with a specified equality comparer.
    /// </summary>
    /// <param name="equalityComparer">The equality comparer to use for comparing strings.</param>
    /// <param name="capacity">The initial size of the StringPool.</param>
    public ConcurrentStringPool(int capacity, IEqualityComparer<string> equalityComparer) {
        ArgumentNullException.ThrowIfNull(equalityComparer);
        _pool = new(-1, capacity, equalityComparer);
        _alternateLookupPool = _pool.GetAlternateLookup<ReadOnlySpan<char>>();
    }

    /// <summary>
    /// Gets or adds a string to the pool.
    /// </summary>
    /// <param name="key">The string to add.</param>
    /// <returns>The existing or newly added string.</returns>
    public string GetOrAdd(string key) => _pool.TryGetValue(key, out var value)
               ? value : AddAndReturn(key);

    /// <summary>
    /// Gets or adds a string to the pool.
    /// </summary>
    /// <param name="key">The span of characters representing the string to add.</param>
    /// <returns>The existing or newly added string.</returns>
    public string GetOrAdd(ReadOnlySpan<char> key) => _alternateLookupPool.TryGetValue(key, out var value)
               ? value : AddAndReturn(key.ToString());

    /// <summary>
    /// Gets or adds a string to the pool.
    /// </summary>
    /// <param name="bytes">The read-only span of bytes representing the string to add.</param>
    /// <returns>The existing or newly added string.</returns>
    public string GetOrAdd(ReadOnlySpan<byte> bytes) {
        var length = Encoding.UTF8.GetMaxCharCount(bytes.Length);
        char[]? charArray = null;
        try {
            Span<char> chars = length < 32 ? stackalloc char[length] : (charArray = ArrayPool<char>.Shared.Rent(length));
            Utf8.ToUtf16(bytes, chars, out _, out length);
            chars = chars[0..length]; // remove \0
            if (_alternateLookupPool.TryGetValue(chars, out var value)) {
                return value;
            } else {
                value = new string(chars);
                return AddAndReturn(value);
            }
        } finally {
            if (charArray is not null) {
                ArrayPool<char>.Shared.Return(charArray);
            }
        }
    }

    /// <summary>
    /// Gets or adds a string to the pool.
    /// </summary>
    /// <param name="interpolatedStringHandler">The interpolated string handler representing the string to add.</param>
    /// <returns>The existing or newly added string.</returns>
    public string GetOrAdd(DefaultInterpolatedStringHandler interpolatedStringHandler) {
        try {
            var span = interpolatedStringHandler.GetText();
            return this.GetOrAdd(span);
        } finally {
            interpolatedStringHandler.Clear();
        }
    }

    /// <summary>
    /// Adds a string to the pool.
    /// </summary>
    /// <param name="text">The string to add.</param>
    /// <returns>The added string.</returns>
    private string AddAndReturn(string text) {
        _pool.TryAdd(text, text);
        return text;
    }

    /// <summary>
    /// Gets the number of strings in the pool.
    /// </summary>
    public int Count => _pool.Count;

    /// <summary>
    /// Clear the string pool
    /// </summary>
    public void Clear() => _pool.Clear();

    /// <summary>
    /// Returns an enumerator that iterates through the strings in the pool.
    /// </summary>
    /// <returns>An enumerator for the strings in the pool.</returns>
    public IEnumerator<string> GetEnumerator() => _pool.Values.GetEnumerator();

    /// <summary>
    /// Returns an enumerator that iterates through the strings in the pool.
    /// </summary>
    /// <returns>An enumerator for the strings in the pool.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
