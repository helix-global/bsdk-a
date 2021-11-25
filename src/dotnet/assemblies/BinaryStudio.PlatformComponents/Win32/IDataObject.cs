using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("0000010E-0000-0000-C000-000000000046")]
    public interface IDataObject
        {
        [MethodImpl(MethodImplOptions.PreserveSig)] HRESULT GetData(ref FORMATETC pformatetcIn, out STGMEDIUM pRemoteMedium);
        [MethodImpl(MethodImplOptions.InternalCall)] void GetDataHere([In][MarshalAs(UnmanagedType.LPArray)] FORMATETC[] pFormatetc, [In] [Out][MarshalAs(UnmanagedType.LPArray)] STGMEDIUM[] pRemoteMedium);
        [MethodImpl(MethodImplOptions.PreserveSig)] int QueryGetData([In][MarshalAs(UnmanagedType.LPArray)] FORMATETC[] pFormatetc);
        [MethodImpl(MethodImplOptions.PreserveSig)] int GetCanonicalFormatEtc([In][MarshalAs(UnmanagedType.LPArray)] FORMATETC[] pformatectIn, [Out][MarshalAs(UnmanagedType.LPArray)] FORMATETC[] pformatetcOut);
        [MethodImpl(MethodImplOptions.InternalCall)] void SetData([In][MarshalAs(UnmanagedType.LPArray)] FORMATETC[] pFormatetc, [In] [MarshalAs(UnmanagedType.LPArray)] STGMEDIUM[] pmedium, [In] int fRelease);
        [MethodImpl(MethodImplOptions.PreserveSig)] int EnumFormatEtc([In] uint dwDirection, [MarshalAs(UnmanagedType.Interface)] out IEnumFORMATETC ppenumFormatEtc);
        [MethodImpl(MethodImplOptions.PreserveSig)] int DAdvise([In][MarshalAs(UnmanagedType.LPArray)] FORMATETC[] pFormatetc, [In] uint ADVF, [In] [MarshalAs(UnmanagedType.Interface)] IAdviseSink pAdvSink,out uint pdwConnection);
        [MethodImpl(MethodImplOptions.InternalCall)] void DUnadvise([In] uint dwConnection);
        [MethodImpl(MethodImplOptions.PreserveSig)] int EnumDAdvise([MarshalAs(UnmanagedType.Interface)] out IEnumSTATDATA ppenumAdvise);
        }
    }