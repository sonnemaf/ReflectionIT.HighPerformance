using System.Runtime.CompilerServices;

namespace ReflectionIT.HighPerformance.Helpers;

/// <summary>
/// Provides extension methods for the <see cref="DefaultInterpolatedStringHandler"/> struct.
/// </summary>
internal static class Extensions {

    /// <summary>
    /// Gets the text from the <see cref="DefaultInterpolatedStringHandler"/>.
    /// </summary>
    /// <param name="this">The <see cref="DefaultInterpolatedStringHandler"/> instance.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> of characters representing the text.</returns>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_Text")]
    public static extern ReadOnlySpan<char> GetText(this ref DefaultInterpolatedStringHandler @this);

    /// <summary>
    /// Clears the content of the <see cref="DefaultInterpolatedStringHandler"/>.
    /// </summary>
    /// <param name="this">The <see cref="DefaultInterpolatedStringHandler"/> instance.</param>
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Clear")]
    public static extern void Clear(this ref DefaultInterpolatedStringHandler @this);
}
