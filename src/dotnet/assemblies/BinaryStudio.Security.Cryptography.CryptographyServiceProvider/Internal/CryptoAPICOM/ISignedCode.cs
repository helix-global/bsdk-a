using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [DefaultMember("FileName"), Guid("84FBCB95-5600-404C-9187-AC25B4CD6E94")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1056:Uri properties should not be strings")]
    [SuppressMessage("Design", "CA1054:Uri parameters should not be strings")]
    #endif
    public interface ISignedCode
        {
        [DispId(0)]
        String FileName
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
        String Description
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
        String DescriptionURL
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
        ISigner Signer
            {
            [DispId(3)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(4)]
        ISigner TimeStamper
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(5)]
        ICertificates Certificates
            {
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Sign([MarshalAs(UnmanagedType.Interface)] [In] ISigner pISigner2 = null);

        [DispId(7)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Timestamp([MarshalAs(UnmanagedType.BStr)] [In] String URL);

        [DispId(8)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Verify([In] Boolean bUIAllowed = false);
        }
    }
