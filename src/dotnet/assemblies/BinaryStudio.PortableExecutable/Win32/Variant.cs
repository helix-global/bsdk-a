using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable
    {
    [StructLayout(LayoutKind.Explicit,Pack = 1)]
    internal struct VARIANT
        {
        [FieldOffset(0)] public VARTYPE Type;
        [FieldOffset(6)] public Int32   Int32;
        [FieldOffset(6)] public Int16   Int16;
        [FieldOffset(6)] public Int64   Int64;
        [FieldOffset(6)] public UInt16  UInt16;
        [FieldOffset(6)] public UInt32  UInt32;
        [FieldOffset(6)] public UInt64  UInt64;
        [FieldOffset(6)] public Byte    Byte;
        [FieldOffset(6)] public SByte   SByte;
        [FieldOffset(6)] public Single  Single;
        [FieldOffset(6)] public Double  Double;
        [FieldOffset(6)] public IntPtr  IntPtr;
        [FieldOffset(6)] public UIntPtr UIntPtr;
        }
    }