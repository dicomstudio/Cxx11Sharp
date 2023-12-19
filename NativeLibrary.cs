using System.Runtime.InteropServices;

namespace DicomStudio.Cxx11Sharp;

internal static class NativeLibrary
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal delegate int ReadDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal delegate int WriteDelegate(int count);

    [DllImport("Cxx11Sharp")]
    private static extern StreamBufHandle create_managed_streambuf(IntPtr fillBuffer, IntPtr flushBuffer,
        [In] byte[] buffer, [In] int buffering);

    internal static StreamBufHandle create_managed_streambuf(ReadDelegate read, WriteDelegate write, byte[] buffer)
    {
        var fillPtr = Marshal.GetFunctionPointerForDelegate(read);
        var flushPtr = Marshal.GetFunctionPointerForDelegate(write);
        return create_managed_streambuf(fillPtr, flushPtr, buffer, buffer.Length);
    }

    [DllImport("Cxx11Sharp")]
    internal static extern void delete_managed_streambuf(IntPtr handle);
}