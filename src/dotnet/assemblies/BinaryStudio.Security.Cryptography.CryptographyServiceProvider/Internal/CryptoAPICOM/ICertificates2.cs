using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Internal.CryptoAPICOM
    {
    [Guid("7B57C04B-1786-4B30-A7B6-36235CD58A14")]
    [TypeLibType(TypeLibTypeFlags.FDispatchable | TypeLibTypeFlags.FDual)]
    [ComImport]
    #if CODE_ANALYSIS
    [SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "<Pending>")]
    [SuppressMessage("Design", "CA1010:Generic interface should also be implemented", Justification = "<Pending>")]
    #endif
    public interface ICertificates2 : ICertificates
        {
        [DispId(2)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ICertificates Find([In] CAPICOM_CERTIFICATE_FIND_TYPE FindType, [MarshalAs(UnmanagedType.Struct)] [In] Object varCriteria = null, [In] Boolean bFindValidOnly = false);

        [DispId(3)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.Interface)]
        ICertificates Select([MarshalAs(UnmanagedType.BStr)] [In] String Title = "", [MarshalAs(UnmanagedType.BStr)] [In] String DisplayString = "", [In] Boolean bMultiSelect = false);

        [DispId(4)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Add([MarshalAs(UnmanagedType.Interface)] [In] ICertificate pVal);

        [DispId(5)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Remove([MarshalAs(UnmanagedType.Struct)] [In] Object Index);

        [DispId(6)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Clear();

        [DispId(7)]
        [MethodImpl(MethodImplOptions.InternalCall)]
        void Save([MarshalAs(UnmanagedType.BStr)] [In] String FileName, [MarshalAs(UnmanagedType.BStr)] [In] String Password = "", [In] CAPICOM_CERTIFICATES_SAVE_AS_TYPE SaveAs = CAPICOM_CERTIFICATES_SAVE_AS_TYPE.CAPICOM_CERTIFICATES_SAVE_AS_PFX, [In] CAPICOM_EXPORT_FLAG ExportFlag = CAPICOM_EXPORT_FLAG.CAPICOM_EXPORT_DEFAULT);
        }
    }
