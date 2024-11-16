using ReflectionIT.HighPerformance.Helpers;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;

namespace ReflectionIT.HighPerformance.Buffers;

/// <summary>
/// Represents a pool of strings that can be reused to reduce memory allocations.
/// </summary>
public sealed class StringPool : IEnumerable<string> {

    /// <summary>
    /// Gets the shared instance of the <see cref="StringPool"/>.
    /// </summary>
    public static StringPool Shared { get; } = new();

    private readonly HashSet<string> _pool;
    private readonly HashSet<string>.AlternateLookup<ReadOnlySpan<char>> _alternateLookupPool;

    /// <summary>
    /// Initializes a new instance of the <see cref="StringPool"/> class.
    /// </summary>
    public StringPool() {
        _pool = new HashSet<string>();
        _alternateLookupPool = _pool.GetAlternateLookup<ReadOnlySpan<char>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringPool"/> class.
    /// </summary>
    /// <param name="capacity">The initial size of the StringPool.</param>
    public StringPool(int capacity) {
        _pool = new HashSet<string>(capacity);
        _alternateLookupPool = _pool.GetAlternateLookup<ReadOnlySpan<char>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringPool"/> class with a specified equality comparer.
    /// </summary>
    /// <param name="equalityComparer">The equality comparer to use for comparing strings.</param>
    public StringPool(IEqualityComparer<string> equalityComparer) {
        _pool = new HashSet<string>(equalityComparer);
        _alternateLookupPool = _pool.GetAlternateLookup<ReadOnlySpan<char>>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StringPool"/> class with a specified equality comparer.
    /// </summary>
    /// <param name="equalityComparer">The equality comparer to use for comparing strings.</param>
    /// <param name="capacity">The initial size of the StringPool.</param>
    public StringPool(int capacity, IEqualityComparer<string> equalityComparer) {
        _pool = new HashSet<string>(capacity, equalityComparer);
        _alternateLookupPool = _pool.GetAlternateLookup<ReadOnlySpan<char>>();
    }

    /// <summary>
    /// Gets or adds a string to the pool.
    /// </summary>
    /// <param name="key">The string to add.</param>
    /// <returns>The existing or newly added string.</returns>
    public string GetOrAdd(string key) {
        return _pool.TryGetValue(key, out var value) 
               ? value : Add(key);
    }

    /// <summary>
    /// Gets or adds a string to the pool.
    /// </summary>
    /// <param name="key">The span of characters representing the string to add.</param>
    /// <returns>The existing or newly added string.</returns>
    public string GetOrAdd(ReadOnlySpan<char> key) {
        return _alternateLookupPool.TryGetValue(key, out var value) 
               ? value : Add(key.ToString());
    }

    /// <summary>
    /// Gets or adds a string to the pool.
    /// </summary>
    /// <param name="bytes">The read-only span of bytes representing the string to add.</param>
    /// <returns>The existing or newly added string.</returns>
    public string GetOrAdd(ReadOnlySpan<byte> bytes) {
        Span<char> chars = stackalloc char[Encoding.UTF8.GetMaxCharCount(bytes.Length)];
        Utf8.ToUtf16(bytes, chars, out _, out var length);
        chars = chars[0..length]; // remove \0
        return _alternateLookupPool.TryGetValue(chars, out var value) 
               ? value : Add(new string(chars));
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
    private string Add(string text) {
        _pool.Add(text);
        return text;
    }

    /// <summary>
    /// Gets the number of strings in the pool.
    /// </summary>
    public int Count => _pool.Count;

    /// <summary>
    /// Returns an enumerator that iterates through the strings in the pool.
    /// </summary>
    /// <returns>An enumerator for the strings in the pool.</returns>
    public IEnumerator<string> GetEnumerator() {
        return _pool.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the strings in the pool.
    /// </summary>
    /// <returns>An enumerator for the strings in the pool.</returns>
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
