using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct PEB32
        {
        [FieldOffset(  2)] public readonly Byte BeingDebugged;
        [FieldOffset( 12)] public readonly UInt32 Ldr;
        [FieldOffset( 16)] public readonly UInt32 ProcessParameters;
        [FieldOffset( 32)] public readonly UInt32 AtlThunkSListPtr;
        [FieldOffset( 52)] public readonly UInt32 AtlThunkSListPtr32;
        [FieldOffset(332)] public readonly UInt32 PostProcessInitRoutine;
        [FieldOffset(468)] public readonly UInt32 SessionId;
        }
    }