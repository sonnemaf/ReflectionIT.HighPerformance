using System.Collections;
using System.Text;

namespace ReflectionIT.HighPerformance.Buffers;

/// <summary>
/// Represents a pool of UTF-8 encoded strings that can be reused to reduce memory allocations.
/// </summary>
public sealed class Utf8StringPool : IEnumerable<string> {

    /// <summary>
    /// Gets the shared instance of the <see cref="Utf8StringPool"/>.
    /// </summary>
    public static Utf8StringPool Shared { get; } = new();

    private readonly Dictionary<byte[], string> _pool = new Dictionary<byte[], string>(new BytesReadOnlySpanOfBytesEqualityComparer());
    private readonly Dictionary<byte[], string>.AlternateLookup<ReadOnlySpan<byte>> _alternateLookupPool;

    /// <summary>
    /// Initializes a new instance of the <see cref="Utf8StringPool"/> class.
    /// </summary>
    public Utf8StringPool() {
        _alternateLookupPool = _pool.GetAlternateLookup<ReadOnlySpan<byte>>();
    }

    /// <summary>
    /// Gets or adds a UTF-8 encoded string to the pool.
    /// </summary>
    /// <param name="key">The byte array representing the UTF-8 encoded string to add.</param>
    /// <returns>The existing or newly added string.</returns>
    public string GetOrAdd(byte[] key) {
        return _pool.TryGetValue(key, out var value)
               ? value : this.Add(key, value: Encoding.UTF8.GetString(key));
    }

    /// <summary>
    /// Gets or adds a UTF-8 encoded string to the pool.
    /// </summary>
    /// <param name="key">The read-only span of bytes representing the UTF-8 encoded string to add.</param>
    /// <returns>The existing or newly added string.</returns>
    public string GetOrAdd(ReadOnlySpan<byte> key) {
        return _alternateLookupPool.TryGetValue(key, out var value)
               ? value : Add(key.ToArray(), Encoding.UTF8.GetString(key));
    }

    /// <summary>
    /// Adds a UTF-8 encoded string to the pool.
    /// </summary>
    /// <param name="key">The byte array representing the UTF-8 encoded string to add.</param>
    /// <param name="value">The string to add.</param>
    /// <returns>The added string.</returns>
    private string Add(byte[] key, string value) {
        _pool.Add(key, value);
        return value;
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
        return _pool.Values.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the strings in the pool.
    /// </summary>
    /// <returns>An enumerator for the strings in the pool.</returns>
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
