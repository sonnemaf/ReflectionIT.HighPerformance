using ReflectionIT.HighPerformance.Buffers;
using System.Text;

namespace ReflectionIT.HighPerformance.Test;

public class Utf8StringPoolTests {

    /// <summary>
    /// Tests that GetOrAdd with a byte array key returns the same string.
    /// </summary>
    [Fact]
    public void GetOrAdd_ByteArrayKey_ReturnsSameString() {
        // Arrange
        var pool = new Utf8StringPool();
        var key = "Test😁👌💕€Ġ"u8;

        // Act
        var result = pool.GetOrAdd(key);

        // Assert
        Assert.Equal("Test😁👌💕€Ġ", result);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with a ReadOnlySpan<byte> key returns the same string.
    /// </summary>
    [Fact]
    public void GetOrAdd_ReadOnlySpanByteKey_ReturnsSameString() {
        // Arrange
        var pool = new Utf8StringPool();
        var key1 = "Test😁👌💕"u8;
        var key2 = "Test😁👌💕"u8;

        // Act
        var result1 = pool.GetOrAdd(key1);
        var result2 = pool.GetOrAdd(key2);

        // Assert
        Assert.Equal("Test😁👌💕", result1);
        Assert.Equal("Test😁👌💕", result2);
        Assert.Same(result1, result2);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with the same byte array key twice returns the same instance.
    /// </summary>
    [Fact]
    public void GetOrAdd_SameByteArrayKeyTwice_ReturnsSameInstance() {
        // Arrange
        var pool = new Utf8StringPool();
        var key = Encoding.UTF8.GetBytes("test");

        // Act
        var result1 = pool.GetOrAdd(key);
        var result2 = pool.GetOrAdd(key);

        // Assert
        Assert.Same(result1, result2);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with different byte array keys returns different instances.
    /// </summary>
    [Fact]
    public void GetOrAdd_DifferentByteArrayKeys_ReturnsDifferentInstances() {
        // Arrange
        var pool = new Utf8StringPool();
        var key1 = Encoding.UTF8.GetBytes("test1");
        var key2 = Encoding.UTF8.GetBytes("test2");

        // Act
        var result1 = pool.GetOrAdd(key1);
        var result2 = pool.GetOrAdd(key2);

        // Assert
        Assert.NotSame(result1, result2);
        Assert.Equal(2, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with the same ReadOnlySpan<byte> key twice returns the same instance.
    /// </summary>
    [Fact]
    public void GetOrAdd_SameReadOnlySpanByteKeyTwice_ReturnsSameInstance() {
        // Arrange
        var pool = new Utf8StringPool();
        var key = Encoding.UTF8.GetBytes("test").AsSpan();

        // Act
        var result1 = pool.GetOrAdd(key);
        var result2 = pool.GetOrAdd(key);

        // Assert
        Assert.Same(result1, result2);
        Assert.Equal(1, pool.Count);
    }

    /// <summary>
    /// Tests that GetOrAdd with different ReadOnlySpan<byte> keys returns different instances.
    /// </summary>
    [Fact]
    public void GetOrAdd_DifferentReadOnlySpanByteKeys_ReturnsDifferentInstances() {
        // Arrange
        var pool = new Utf8StringPool();
        var key1 = Encoding.UTF8.GetBytes("test1").AsSpan();
        var key2 = Encoding.UTF8.GetBytes("test2").AsSpan();

        // Act
        var result1 = pool.GetOrAdd(key1);
        var result2 = pool.GetOrAdd(key2);

        // Assert
        Assert.NotSame(result1, result2);
        Assert.Equal(2, pool.Count);
    }


}

