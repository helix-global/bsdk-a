using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214e8-0000-0000-c000-000000000046")]
    public interface IShellExtInit
        {
        [SuppressUnmanagedCodeSecurity]
        [PreserveSig] HRESULT Initialize ([In]IntPtr pidlFolder, [In] IDataObject lpdobj, [In]IntPtr /*HKEY*/ hKeyProgID);
        }
    }