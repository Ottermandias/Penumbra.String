namespace Penumbra.String.Functions;

/// <summary>
/// Import some standard functions for memory handling.
/// </summary>
public static partial class MemoryUtility
{
    /// <summary> Copies <paramref name="count"/> bytes from <paramref name="src"/> to <paramref name="dest"/>. </summary>
    public static unsafe void MemCpyUnchecked(void* dest, void* src, int count)
    {
        var span   = new Span<byte>(src,  count);
        var target = new Span<byte>(dest, count);
        span.CopyTo(target);
    }

    /// <summary> Compares <paramref name="count"/> bytes from <paramref name="ptr1"/> with <paramref name="ptr2"/> lexicographically. </summary>
    public static unsafe int MemCmpUnchecked(void* ptr1, void* ptr2, int count)
    {
        var lhs = new Span<byte>(ptr1, count);
        var rhs = new Span<byte>(ptr2, count);
        return lhs.SequenceCompareTo(rhs);
    }

    [LibraryImport("msvcrt.dll", EntryPoint = "_memicmp", SetLastError = false)]
    private static unsafe partial int memicmp(void* b1, void* b2, ulong count);

    /// <summary> Compares <paramref name="count"/> bytes from <paramref name="ptr1"/> with <paramref name="ptr2"/> lexicographically and disregarding ascii-case. </summary>
    /// <remarks>Call memicmp from msvcrt.dll.</remarks>
    public static unsafe int MemCmpCaseInsensitiveUnchecked(void* ptr1, void* ptr2, int count)
        => memicmp(ptr1, ptr2, (ulong)count);

    /// <summary> Sets <paramref name="count"/> bytes from <paramref name="dest"/> on to <paramref name="value"/>. </summary>
    public static unsafe void* MemSet(void* dest, byte value, int count)
    {
        var span = new Span<byte>(dest, count);
        span.Fill(value);
        return dest;
    }
}
