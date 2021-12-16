using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Explicit, Pack = 2)]
    public struct PEB64
        {
        [FieldOffset(002)] public readonly Byte   BeingDebugged;
        [FieldOffset(024)] public readonly UInt64 Ldr;
        [FieldOffset(032)] public readonly UInt64 ProcessParameters;
        [FieldOffset(064)] public readonly UInt64 AtlThunkSListPtr;
        [FieldOffset(100)] public readonly UInt32 AtlThunkSListPtr32;
        [FieldOffset(560)] public readonly UInt64 PostProcessInitRoutine;
        [FieldOffset(704)] public readonly UInt32 SessionId;
        }
    }