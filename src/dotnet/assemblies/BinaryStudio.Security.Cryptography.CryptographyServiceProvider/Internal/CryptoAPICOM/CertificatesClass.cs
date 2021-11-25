using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [ClassInterface(ClassInterfaceType.None), Guid("3605B612-C3CF-4AB4-A426-2D853391DB2E"), TypeLibType(2)]
    [ComImport]
    #if CODE_ANALYSIS
    [SuppressMessage("Design", "CA1010:Generic interface should also be implemented")]
    [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
    #endif
    public class CertificatesClass : ICertificates2, ICCertificates
        {
        [DispId(0)]
        public virtual extern Object this[Int32 Index]
            {
            [DispId(0)]
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
            get;
            }

        [DispId(1)]
        public virtual extern Int32 Count
            {
            [DispId(1)]
            [MethodImpl(MethodImplOptions.InternalCall)]
            get;
            }

        [DispId(-4), TypeLibFunc(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler")]
        public virtual extern IEnumerator GetEnumerator();

        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern ICertificates Find([In] CAPICOM_CERTIFICATE_FIND_TYPE FindType, [MarshalAs(UnmanagedType.Struct)] [In] Object varCriteria = null, [In] Boolean bFindValidOnly = false);

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        public virtual extern ICertificates Select([MarshalAs(UnmanagedType.BStr)] [In] String Title = "", [MarshalAs(UnmanagedType.BStr)] [In] String DisplayString = "", [In] Boolean bMultiSelect = false);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Add([MarshalAs(UnmanagedType.Interface)] [In] ICertificate pVal);

        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Remove([MarshalAs(UnmanagedType.Struct)] [In] Object Index);

        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Clear();

        [DispId(7)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void Save([MarshalAs(UnmanagedType.BStr)] [In] String FileName, [MarshalAs(UnmanagedType.BStr)] [In] String Password = "", [In] CAPICOM_CERTIFICATES_SAVE_AS_TYPE SaveAs = CAPICOM_CERTIFICATES_SAVE_AS_TYPE.CAPICOM_CERTIFICATES_SAVE_AS_PFX, [In] CAPICOM_EXPORT_FLAG ExportFlag = CAPICOM_EXPORT_FLAG.CAPICOM_EXPORT_DEFAULT);

        [TypeLibFunc(1)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        public virtual extern void _ExportToStore([In] IntPtr hCertStore);
        }
    }
