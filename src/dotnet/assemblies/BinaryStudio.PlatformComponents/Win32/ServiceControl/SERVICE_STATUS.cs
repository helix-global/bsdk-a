using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SERVICE_STATUS
        {
        public readonly SERVICE_TYPE  ServiceType;
        public readonly SERVICE_STATE CurrentState;
        public readonly UInt32 dwControlsAccepted;
        public readonly UInt32 dwWin32ExitCode;
        public readonly UInt32 dwServiceSpecificExitCode;
        public readonly UInt32 dwCheckPoint;
        public readonly UInt32 dwWaitHint;
        }
    }