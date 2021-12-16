using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct PEB64_LDR_DATA
        {
        [FieldOffset(32)] public readonly LIST_ENTRY64 InMemoryOrderModuleList;
        }
    }