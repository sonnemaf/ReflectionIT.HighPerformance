using System.Diagnostics.CodeAnalysis;

namespace ReflectionIT.HighPerformance.Buffers;

internal sealed class BytesReadOnlySpanOfBytesEqualityComparer : IAlternateEqualityComparer<ReadOnlySpan<byte>, byte[]>, IEqualityComparer<byte[]> {

    public bool Equals(ReadOnlySpan<byte> alternate, byte[] other) => alternate.SequenceEqual(other);

    public int GetHashCode(ReadOnlySpan<byte> alternate) {
        var hc = new HashCode();
        hc.AddBytes(alternate);
        return hc.ToHashCode();
    }

    public byte[] Create(ReadOnlySpan<byte> alternate) => alternate.ToArray();

    public bool Equals(byte[]? x, byte[]? y) => Equals((ReadOnlySpan<byte>)x, y!);

    public int GetHashCode([DisallowNull] byte[] obj) {
        var hc = new HashCode();
        hc.AddBytes(obj);
        return hc.ToHashCode();
    }

}