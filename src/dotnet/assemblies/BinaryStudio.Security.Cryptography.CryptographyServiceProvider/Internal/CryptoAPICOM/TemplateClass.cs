using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [ClassInterface(ClassInterfaceType.None), Guid("61F0D2BD-373E-4F3C-962E-59B7C42C1B22")]
    [ComImport]
    public class TemplateClass : ITemplate
        {
        [DispId(1)]
        public virtual extern Boolean IsPresent
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(2)]
        public virtual extern Boolean IsCritical
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(3)]
        public virtual extern String Name
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(4)]
        public virtual extern IOID OID
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(5)]
        public virtual extern Int32 MajorVersion
            {
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(6)]
        public virtual extern Int32 MinorVersion
            {
            [DispId(6)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }
        }
    }
