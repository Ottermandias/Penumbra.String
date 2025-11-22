using System.Text.Unicode;
using Penumbra.String.Functions;

namespace Penumbra.String;

public sealed unsafe partial class CiByteString : IEquatable<CiByteString>, IComparable<CiByteString>
{
    /// <param name="other">The string to compare with.</param>
    /// <returns>Whether this string and <paramref name="other"/> are equal ignoring (ASCII) case.</returns>
    public bool Equals(CiByteString? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return EqualsInternal(other);
    }

    /// <returns>Whether this string and the object <paramref name="obj"/> are equal. </returns>
    public override bool Equals(object? obj)
        => ReferenceEquals(this, obj) || obj is CiByteString other && EqualsInternal(other);

    /// <param name="other">The string to compare with.</param>
    /// <returns>Whether this string and <paramref name="other"/> are equal including (ASCII) case.</returns>
    public bool EqualsCs(CiByteString? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return ByteStringFunctions.Equals(_path, Length, other._path, other.Length);
    }

    /// <param name="other">The string to compare with.</param>
    /// <returns>Whether this string is lexicographically smaller (less than 0), bigger (greater than 0) or equal (0) to <paramref name="other"/> ignoring (ASCII) case.</returns>
    public int CompareTo(CiByteString? other)
    {
        if (ReferenceEquals(this, other))
            return 0;

        if (ReferenceEquals(null, other))
            return 1;

        return StringCompareCi(other);
    }

    /// <param name="other">The string to compare with.</param>
    /// <returns>Whether this string is lexicographically smaller (less than 0), bigger (greater than 0) or equal (0) to <paramref name="other"/> including (ASCII) case.</returns>
    public int CompareToCs(CiByteString? other)
    {
        if (ReferenceEquals(null, other))
            return 0;

        if (ReferenceEquals(this, other))
            return 1;

        return ByteStringFunctions.Compare(_path, Length, other._path, other.Length);
    }

    /// <param name="other">The prefix to check for.</param>
    /// <returns>Whether this string has <paramref name="other"/> as a prefix.</returns>
    public bool StartsWith(CiByteString other)
    {
        var otherLength = other.Length;
        return otherLength <= Length && ByteStringFunctions.Equals(other.Path, otherLength, Path, otherLength);
    }

    /// <param name="other">The suffix to check for.</param>
    /// <returns>Whether this string has <paramref name="other"/> as a suffix.</returns>
    public bool EndsWith(CiByteString other)
    {
        var otherLength = other.Length;
        var offset      = Length - otherLength;
        return offset >= 0 && ByteStringFunctions.Equals(other.Path, otherLength, Path + offset, otherLength);
    }

    /// <inheritdoc cref="StartsWith(CiByteString)"/>
    public bool StartsWith(ReadOnlySpan<byte> chars)
    {
        if (chars.Length > Length)
            return false;

        return chars.SequenceEqual(new ReadOnlySpan<byte>(_path, chars.Length));
    }

    /// <inheritdoc cref="StartsWith(CiByteString)"/>
    public bool EndsWith(ReadOnlySpan<byte> chars)
    {
        if (chars.Length > Length)
            return false;

        var ptr = _path + Length - chars.Length;
        return chars.SequenceEqual(new ReadOnlySpan<byte>(ptr, chars.Length));
    }

    /// <summary> Check whether this string contains a substring disregarding case anywhere. </summary>
    /// <param name="other">The substring to check for.</param>
    public bool Contains(CiByteString other)
    {
        var length    = Length;
        var subLength = other.Length;
        if (subLength > length)
            return false;
        if (subLength == 0)
            return true;

        var isLower = IsAsciiLowerInternal is true;
        if (subLength == 1)
            return ContainsLength1SpecialCase(this, other.Span, isLower);

        var otherIsLower = other.IsAsciiLowerInternal is true;
        if (isLower && otherIsLower)
            return Span.IndexOf(other.Span) >= 0;

        return ContainsDefaultCase(_path, other._path, _path + length - subLength, subLength, isLower);
    }


    /// <inheritdoc cref="Contains(CiByteString)"/>
    public bool Contains(ReadOnlySpan<byte> other)
    {
        var length    = Length;
        var subLength = other.Length;
        if (subLength > length)
            return false;
        if (subLength == 0)
            return true;

        var isLower = IsAsciiLowerInternal is true;
        if (subLength == 1)
            return ContainsLength1SpecialCase(this, other, isLower);

        fixed (byte* ptr = other)
        {
            return ContainsDefaultCase(_path, ptr, _path + length - subLength, subLength, isLower);
        }
    }

    /// <summary> Check whether this string contains an exact substring anywhere. </summary>
    /// <param name="other">The substring to check for.</param>
    public bool ContainsCs(CiByteString other)
        => ContainsCs(other.Span);


    /// <inheritdoc cref="ContainsCs(CiByteString)"/>
    public bool ContainsCs(ReadOnlySpan<byte> other)
    {
        if (other.Length > Length)
            return false;


        return Span.IndexOf(other) >= 0;
    }

    /// <summary>
    /// Find the first occurrence of <paramref name="b"/> in this string.
    /// </summary>
    /// <param name="b">The needle.</param>
    /// <param name="from">An optional starting index in this string.</param>
    /// <returns>The index of the first occurrence of <paramref name="b"/> after <paramref name="from"/> or -1 if it is not found.</returns>
    public int IndexOf(byte b, int from = 0)
    {
        var end = _path + Length;
        for (var tmp = _path + from; tmp < end; ++tmp)
        {
            if (*tmp == b)
                return (int)(tmp - _path);
        }

        return -1;
    }

    /// <summary>
    /// Find the last occurrence of <paramref name="b"/> in this string.
    /// </summary>
    /// <param name="b">The needle.</param>
    /// <param name="to">An optional stopping index in this string.</param>
    /// <returns>The index of the last occurrence of <paramref name="b"/> before <paramref name="to"/> or -1 if it is not found.</returns>
    public int LastIndexOf(byte b, int to = 0)
    {
        var end = _path + to;
        for (var tmp = _path + Length - 1; tmp >= end; --tmp)
        {
            if (*tmp == b)
                return (int)(tmp - _path);
        }

        return -1;
    }

    /// <returns>Whether two strings are equal.</returns>
    public static bool operator ==(CiByteString lhs, CiByteString? rhs)
        => lhs.Equals(rhs);

    /// <returns>Whether two strings are different.</returns>
    public static bool operator !=(CiByteString lhs, CiByteString rhs)
        => !lhs.Equals(rhs);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool EqualsInternal(CiByteString other)
        => CompareCiCrc32(other) && StringEqualsCi(other);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool CompareCiCrc32(CiByteString other)
        => !_flags.HasFlag(Flags.HasCiCrc32) || !other._flags.HasFlag(Flags.HasCiCrc32) || _ciCrc32 == other._ciCrc32;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private bool StringEqualsCi(CiByteString other)
    {
        // Fast path when both known lower-case
        if ((IsAsciiLowerInternal ?? false) && (other.IsAsciiLowerInternal ?? false))
            return ByteStringFunctions.Equals(_path, Length, other._path, other.Length);

        // If either contains wildcard, perform managed regex matching (case-insensitive)
        if (IndexOf((byte)'*') >= 0 || other.IndexOf((byte)'*') >= 0)
        {
            var s1 = ToString();
            var s2 = other.ToString();
            return ByteStringFunctions.EqualsCiWildcard(s1, s2);
        }

        return ByteStringFunctions.AsciiCaselessEquals(_path, Length, other._path, other.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private int StringCompareCi(CiByteString other)
        => (IsAsciiLowerInternal ?? false) && (other.IsAsciiLowerInternal ?? false)
            ? ByteStringFunctions.Compare(_path, Length, other._path, other.Length)
            : ByteStringFunctions.AsciiCaselessCompare(_path, Length, other._path, other.Length);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool ContainsLength1SpecialCase(CiByteString hayStack, ReadOnlySpan<byte> other, bool isLower)
    {
        var needle = ByteStringFunctions.AsciiToLower(other[0]);
        if (isLower)
            return hayStack.Contains(needle);

        foreach (var c in hayStack)
        {
            if (ByteStringFunctions.AsciiToLower(c) == needle)
                return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool ContainsDefaultCase(byte* hayStack, byte* other, byte* end, int subLength, bool isLower)
    {
        var start = ByteStringFunctions.AsciiToLower(other[0]);
        ++other;
        --subLength;
        if (isLower)
            for (; hayStack < end; ++hayStack)
            {
                if (*hayStack != start && MemoryUtility.MemCmpCaseInsensitiveUnchecked(hayStack + 1, other, subLength) == 0)
                    return true;
            }
        else
            for (; hayStack < end; ++hayStack)
            {
                if (ByteStringFunctions.AsciiToLower(*hayStack) != start
                 && MemoryUtility.MemCmpCaseInsensitiveUnchecked(hayStack + 1, other, subLength) == 0)
                    return true;
            }

        return false;
    }
}
