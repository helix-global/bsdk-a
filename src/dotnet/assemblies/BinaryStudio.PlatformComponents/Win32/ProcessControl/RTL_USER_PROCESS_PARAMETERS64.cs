using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct RTL_USER_PROCESS_PARAMETERS64
        {
        [FieldOffset( 96)] public readonly UNICODE_STRING64 ImagePathName;
        [FieldOffset(108)] public readonly UNICODE_STRING64 CommandLine;
        }
    }