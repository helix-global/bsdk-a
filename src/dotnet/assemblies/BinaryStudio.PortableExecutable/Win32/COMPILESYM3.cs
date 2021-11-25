using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct COMPILESYM3
        {
        public readonly UInt32 Flags;
        public readonly CV_CPU_TYPE Machine;         // target processor
        public readonly UInt16 VersionFEMajor;  // front end major version #
        public readonly UInt16 VersionFEMinor;  // front end minor version #
        public readonly UInt16 VersionFEBuild;  // front end build version #
        public readonly UInt16 VersionFEQFE;    // front end QFE version #
        public readonly UInt16 VersionMajor;    // back end major version #
        public readonly UInt16 VersionMinor;    // back end minor version #
        public readonly UInt16 VersionBuild;    // back end build version #
        public readonly UInt16 VersionQFE;      // back end QFE version #
        }
    }