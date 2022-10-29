using System;
using System.Runtime.InteropServices;

namespace Penumbra.String.Functions;

/// <summary>
/// Import some standard functions for memory handling.
/// </summary>
public static class MemoryUtility
{
    [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
    private static extern unsafe IntPtr memcpy(void* dest, void* src, int count);

    public static unsafe void MemCpyUnchecked(void* dest, void* src, int count)
        => memcpy(dest, src, count);


    [DllImport("msvcrt.dll", EntryPoint = "memcmp", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
    private static extern unsafe int memcmp(void* b1, void* b2, int count);

    public static unsafe int MemCmpUnchecked(void* ptr1, void* ptr2, int count)
        => memcmp(ptr1, ptr2, count);


    [DllImport("msvcrt.dll", EntryPoint = "_memicmp", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
    private static extern unsafe int memicmp(void* b1, void* b2, int count);

    public static unsafe int MemCmpCaseInsensitiveUnchecked(void* ptr1, void* ptr2, int count)
        => memicmp(ptr1, ptr2, count);

    [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
    private static extern unsafe void* memset(void* dest, int c, int count);

    public static unsafe void* MemSet(void* dest, byte value, int count)
        => memset(dest, value, count);
}
