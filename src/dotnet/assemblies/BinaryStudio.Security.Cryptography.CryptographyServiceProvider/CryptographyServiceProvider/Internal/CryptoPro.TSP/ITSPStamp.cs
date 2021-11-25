using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.CryptoAPICOM;

namespace CryptoPro.TSP
    {
    [Guid("F59A43D1-B906-47E0-8AF7-55C726AEC81D")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable|TypeLibTypeFlags.FNonExtensible|TypeLibTypeFlags.FDual)]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComImport]
    public interface ITSPStamp
        {
        [DispId(4)]
        ICertificates Certificates
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(5)]
        ICertificate TSACertificate
            {
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(6)]
        Int32 FailInfo
            {
            [DispId(6)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(7)]
        String StatusString
            {
            [DispId(7)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(8)]
        Int32 Status
            {
            [DispId(8)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(11)]
        String PolicyID
            {
            [DispId(11)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(12)]
        String SerialNumber
            {
            [DispId(12)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(13)]
        Boolean Ordering
            {
            [DispId(13)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(14)]
        String TSAName
            {
            [DispId(14)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(15)]
        Int32 Accuracy
            {
            [DispId(15)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(16)]
        DateTime Time
            {
            [DispId(16)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(18)]
        String HashValue
            {
            [DispId(18)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(19)]
        IOID HashAlgorithm
            {
            [DispId(19)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(20)]
        Boolean HasNonce
            {
            [DispId(20)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(21)]
        Int32 DefaultAccuracy
            {
            [DispId(21)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            [DispId(21)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            set;
            }

        [DispId(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Import([MarshalAs(UnmanagedType.BStr)] [In] String strStamp, [MarshalAs(UnmanagedType.Struct)] [In] Object Request = null);

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        String Export();

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Int32 Compare([MarshalAs(UnmanagedType.Interface)] [In] ITSPStamp Stamp);

        [DispId(9)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Int32 VerifyStamp([MarshalAs(UnmanagedType.Interface)] [In] ICertContext TSACertificate);

        [DispId(10)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Int32 VerifyCertificate([MarshalAs(UnmanagedType.Interface)] [In] ICertContext TSACertificate);

        [DispId(17)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        Int32 TimePart([MarshalAs(UnmanagedType.BStr)] String Interval);

        [DispId(22)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Display([In] Int32 hwndParent = 0, [MarshalAs(UnmanagedType.BStr)] [In] String Title = "");
        }
    }
