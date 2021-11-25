using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    public unsafe delegate Boolean PFN_CMSG_STREAM_OUTPUT(IntPtr arg, Byte* data, UInt32 size, Boolean final);
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct CMSG_STREAM_INFO
        {
        public UInt32 Content;
        public PFN_CMSG_STREAM_OUTPUT StreamOutput;
        public IntPtr Arg;

        public CMSG_STREAM_INFO(UInt32 content, PFN_CMSG_STREAM_OUTPUT streamoutput, IntPtr arg)
            {
            Content = (UInt32)content;
            StreamOutput = streamoutput;
            Arg = arg;
            }

        public CMSG_STREAM_INFO(UInt32 content, PFN_CMSG_STREAM_OUTPUT streamoutput)
            {
            Content = (UInt32)content;
            StreamOutput = streamoutput;
            Arg = IntPtr.Zero;
            }
        }
    }