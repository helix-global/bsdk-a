using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct PEB32_LDR_DATA
        {
        [FieldOffset(20)] public readonly LIST_ENTRY32 InMemoryOrderModuleList;
        }
    }