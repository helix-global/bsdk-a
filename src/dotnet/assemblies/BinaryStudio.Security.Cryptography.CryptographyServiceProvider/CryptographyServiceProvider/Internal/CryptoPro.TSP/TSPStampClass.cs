using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Internal.CryptoAPICOM;

namespace CryptoPro.TSP
    {
    [ClassInterface(ClassInterfaceType.None), Guid("CC4DA861-67F1-486A-B062-E27A1C36734B")]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [ComImport]
    public class TSPStampClass : ITSPStamp
        {
        [DispId(4)]
        public virtual extern ICertificates Certificates
            {
            [DispId(4)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(5)]
        public virtual extern ICertificate TSACertificate
            {
            [DispId(5)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(6)]
        public virtual extern Int32 FailInfo
            {
            [DispId(6)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(7)]
        public virtual extern String StatusString
            {
            [DispId(7)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(8)]
        public virtual extern Int32 Status
            {
            [DispId(8)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(11)]
        public virtual extern String PolicyID
            {
            [DispId(11)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(12)]
        public virtual extern String SerialNumber
            {
            [DispId(12)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(13)]
        public virtual extern Boolean Ordering
            {
            [DispId(13)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(14)]
        public virtual extern String TSAName
            {
            [DispId(14)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(15)]
        public virtual extern Int32 Accuracy
            {
            [DispId(15)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(16)]
        public virtual extern DateTime Time
            {
            [DispId(16)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(18)]
        public virtual extern String HashValue
            {
            [DispId(18)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.BStr)]
            get;
            }

        [DispId(19)]
        public virtual extern IOID HashAlgorithm
            {
            [DispId(19)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            [return: MarshalAs(UnmanagedType.Interface)]
            get;
            }

        [DispId(20)]
        public virtual extern Boolean HasNonce
            {
            [DispId(20)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(21)]
        public virtual extern Int32 DefaultAccuracy
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
        public virtual extern void Import([MarshalAs(UnmanagedType.BStr)] [In] String strStamp, [MarshalAs(UnmanagedType.Struct)] [In] Object Request = null);

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public virtual extern String Export();

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern Int32 Compare([MarshalAs(UnmanagedType.Interface)] [In] ITSPStamp Stamp);

        [DispId(9)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern Int32 VerifyStamp([MarshalAs(UnmanagedType.Interface)] [In] ICertContext TSACertificate);

        [DispId(10)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern Int32 VerifyCertificate([MarshalAs(UnmanagedType.Interface)] [In] ICertContext TSACertificate);

        [DispId(17)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern Int32 TimePart([MarshalAs(UnmanagedType.BStr)] String Interval);

        [DispId(22)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Display([In] Int32 hwndParent = 0, [MarshalAs(UnmanagedType.BStr)] [In] String Title = "");
        }
    }
