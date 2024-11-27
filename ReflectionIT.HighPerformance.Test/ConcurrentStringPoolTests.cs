using ReflectionIT.HighPerformance.Buffers;
using System.Text;

namespace ReflectionIT.HighPerformance.Test;

public class ConcurrentStringPoolTests {

    private static ConcurrentStringPool CreateConcurrentPool() => new ConcurrentStringPool(20);

    /// <summary>
    /// Tests that GetOrAdd with a string key returns the same string.
    /// </summary>
    [Fact]
    public void GetOrAdd_StringKey_ReturnsSameString() {
        // Arrange
        var pool = CreateConcurrentPool();
        var key = "test";

        // Act
        var result = pool.GetOrAdd(key);

        // Assert
        Assert.Equal(key, result);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with a ReadOnlySpan<char> key returns the same string.
    /// </summary>
    [Fact]
    public void GetOrAdd_ReadOnlySpanCharKey_ReturnsSameString() {
        // Arrange
        var pool = CreateConcurrentPool();
        var key = "test".AsSpan();

        // Act
        var result = pool.GetOrAdd(key);

        // Assert
        Assert.Equal("test", result);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with a byte array key returns the same string.
    /// </summary>
    [Fact]
    public void GetOrAdd_ByteArrayKey_ReturnsSameString() {
        // Arrange
        var pool = CreateConcurrentPool();
        var key = Encoding.UTF8.GetBytes("test");

        // Act
        var result = pool.GetOrAdd(key);

        // Assert
        Assert.Equal("test", result);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with a ReadOnlySpan<byte> key returns the same string.
    /// </summary>
    [Fact]
    public void GetOrAdd_ReadOnlySpanByteKey_ReturnsSameString() {
        // Arrange
        var pool = CreateConcurrentPool();
        var key = "test"u8;

        // Act
        var result = pool.GetOrAdd(key);

        // Assert
        Assert.Equal("test", result);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with a DefaultInterpolatedStringHandler returns the same string.
    /// </summary>
    [Fact]
    public void GetOrAdd_DefaultInterpolatedStringHandler_ReturnsSameString() {
        // Arrange
        var pool = CreateConcurrentPool();
        var number = 12;
        // Act

        var result = pool.GetOrAdd($"Hello World {number}");

        // Assert
        Assert.Equal("Hello World 12", result);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with the same string twice returns the same instance.
    /// </summary>
    [Fact]
    public void GetOrAdd_SameStringTwice_ReturnsSameInstance() {
        // Arrange
        var pool = CreateConcurrentPool();
        var key = "test";

        // Act
        var result1 = pool.GetOrAdd(key);
        var result2 = pool.GetOrAdd(key);

        // Assert
        Assert.Same(result1, result2);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with different strings returns different instances.
    /// </summary>
    [Fact]
    public void GetOrAdd_DifferentStrings_ReturnsDifferentInstances() {
        // Arrange
        var pool = CreateConcurrentPool();
        var key1 = "test1";
        var key2 = "test2";

        // Act
        var result1 = pool.GetOrAdd(key1);
        var result2 = pool.GetOrAdd(key2);

        // Assert
        Assert.NotSame(result1, result2);
        Assert.Equal(2, pool.Count);
    }

    /// <summary>
    /// Tests if it is really Concurrent, should not throw an exception
    /// </summary>
    [Fact]
    public void Concurrent_GetOrAdd_DifferentReadOnlySpanByteKeys_ReturnsDifferentInstances() {
        // Arrange
        var pool = CreateConcurrentPool();
        //var pool = new StringPool();

        Parallel.For(0, 2000, i => {
            var key1 = Encoding.UTF8.GetBytes($"test{i}").AsSpan();
            var key2 = Encoding.UTF8.GetBytes($"test{i + 1}").AsSpan();

            // Act
            var result1 = pool.GetOrAdd(key1);
            var result2 = pool.GetOrAdd(key2);

            // Assert
            Assert.NotSame(result1, result2);
        });
    }
}
