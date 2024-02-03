using System.Runtime.InteropServices;

public class AADecompress {
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    private static extern IntPtr LoadLibraryW([MarshalAs(UnmanagedType.LPWStr)] string libFileName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

    private delegate bool DecompressDelegate(string[] sources, int nSources, [MarshalAs(UnmanagedType.LPStr)] string destination);

    private const int START_OFFSET = 0x00377B18;
    private const int DECOMPRESS_OFFSET = 0x00044080;

    public static bool Decompress(string[] sources, string destination) {
        IntPtr hModule = LoadLibraryW("patcher.dll");
        if (hModule == IntPtr.Zero) {
            Console.Error.WriteLine("LoadLibraryW(patcher.dll) failed.");
            return false;
        }

        IntPtr startAddress = GetProcAddress(hModule, "Start");
        if (startAddress == IntPtr.Zero) {
            Console.Error.WriteLine("GetProcAddress(patcher.dll, Start) failed.");
            return false;
        }

        DecompressDelegate _decompress = Marshal.GetDelegateForFunctionPointer<DecompressDelegate>(startAddress - (START_OFFSET - DECOMPRESS_OFFSET));

        _decompress(sources, sources.Length, destination);

        return true;
    }
}
