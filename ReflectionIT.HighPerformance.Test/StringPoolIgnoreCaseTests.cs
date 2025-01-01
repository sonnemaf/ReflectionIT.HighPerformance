using ReflectionIT.HighPerformance.Buffers;
using System.Text;

namespace ReflectionIT.HighPerformance.Test;

public class StringPoolIgnoreCaseTests {
    private static StringPool CreatePool() => new StringPool(20, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Tests that GetOrAdd with a string key returns the same string.
    /// </summary>
    [Fact]
    public void GetOrAdd_StringKey_ReturnsSameString() {
        // Arrange
        var pool = CreatePool();
        var key = "test";

        // Act
        var result1 = pool.GetOrAdd(key);
        var result2 = pool.GetOrAdd(key.ToUpper());

        // Assert
        Assert.Equal(key, result1);
        Assert.Equal(key, result2);
        Assert.Same(result1, result2);
        Assert.Equal(1, pool.Count);
    }


    /// <summary>
    /// Tests that GetOrAdd with a ReadOnlySpan<char> key returns the same string.
    /// </summary>
    [Fact]
    public void GetOrAdd_ReadOnlySpanCharKey_ReturnsSameString() {
        // Arrange
        var pool = CreatePool();
        var key1 = "test".AsSpan();
        var key2 = "TEST".AsSpan();

        // Act
        var result1 = pool.GetOrAdd(key1);
        var result2 = pool.GetOrAdd(key2);

        // Assert
        Assert.Equal("test", result1);
        Assert.Equal("test", result2);
        Assert.Same(result1, result2);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with a byte array key returns the same string.
    /// </summary>
    [Fact]
    public void GetOrAdd_ByteArrayKey_ReturnsSameString() {
        // Arrange
        var pool = CreatePool();
        var key1 = Encoding.UTF8.GetBytes("test");
        var key2 = Encoding.UTF8.GetBytes("TEST");

        // Act
        var result1 = pool.GetOrAdd(key1);
        var result2 = pool.GetOrAdd(key2);

        // Assert
        Assert.Equal("test", result1);
        Assert.Equal("test", result2);
        Assert.Same(result1, result2);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with a ReadOnlySpan<byte> key returns the same string.
    /// </summary>
    [Fact]
    public void GetOrAdd_ReadOnlySpanByteKey_ReturnsSameString() {
        // Arrange
        var pool = CreatePool();
        var key1 = "test"u8;
        var key2 = "TEST"u8;

        // Act
        var result1 = pool.GetOrAdd(key1);
        var result2 = pool.GetOrAdd(key2);

        // Assert
        Assert.Equal("test", result1);
        Assert.Equal("test", result2);
        Assert.Same(result1, result2);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with a DefaultInterpolatedStringHandler returns the same string.
    /// </summary>
    [Fact]
    public void GetOrAdd_DefaultInterpolatedStringHandler_ReturnsSameString() {
        // Arrange
        var pool = CreatePool();
        var number = 12;
        // Act

#pragma warning disable CS0618 // Type or member is obsolete
        var result1 = pool.GetOrAdd($"Hello World {number}");
        var result2 = pool.GetOrAdd($"HELLO World {number}");
#pragma warning restore CS0618 // Type or member is obsolete

        // Assert
        Assert.Equal("Hello World 12", result1);
        Assert.Equal("Hello World 12", result2);
        Assert.Same(result1, result2);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with the same string twice returns the same instance.
    /// </summary>
    [Fact]
    public void GetOrAdd_SameStringTwice_ReturnsSameInstance() {
        // Arrange
        var pool = CreatePool();
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
        var pool = CreatePool();
        var key1 = "test1";
        var key2 = "test2";

        // Act
        var result1 = pool.GetOrAdd(key1);
        var result2 = pool.GetOrAdd(key2);

        // Assert
        Assert.NotSame(result1, result2);
        Assert.Equal(2, pool.Count);
    }
}

