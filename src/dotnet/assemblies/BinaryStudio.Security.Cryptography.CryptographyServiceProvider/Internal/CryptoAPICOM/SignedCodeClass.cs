using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("FileName"), ClassInterface(ClassInterfaceType.None), Guid("8C3E4934-9FA4-4693-9253-A29A05F99186"), TypeLibType(2)]
    [ComImport]
    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1054:Uri parameters should not be strings")]
    #endif
    public class SignedCodeClass : ISignedCode
        {
        [DispId(0)]
        public virtual extern String FileName
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
            }

        [DispId(1)]
        public virtual extern String Description
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
            }

        [DispId(2)]
        public virtual extern String DescriptionURL
            {
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            [DispId(2)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [param: MarshalAs(UnmanagedType.BStr)]
            set;
            }

        [DispId(3)]
        public virtual extern ISigner Signer
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(4)]
        public virtual extern ISigner TimeStamper
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(5)]
        public virtual extern ICertificates Certificates
            {
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Sign([MarshalAs(UnmanagedType.Interface)] [In] ISigner pISigner2 = null);

        [DispId(7)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Timestamp([MarshalAs(UnmanagedType.BStr)] [In] String URL);

        [DispId(8)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Verify([In] Boolean bUIAllowed = false);
        }
    }
