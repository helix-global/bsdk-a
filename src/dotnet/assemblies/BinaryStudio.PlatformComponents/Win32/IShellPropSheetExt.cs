using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] public delegate Boolean LPFNSVADDPROPSHEETPAGE(IntPtr hPropSheetPage, IntPtr lParam);

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214E9-0000-0000-C000-000000000046")]
    public interface IShellPropSheetExt
        {
        [PreserveSig] HRESULT AddPages([In] IntPtr pfnAddPage,[In] IntPtr lParam);
        [PreserveSig] HRESULT ReplacePage([In] IntPtr pageid,[In] IntPtr pfnReplaceWith,[In] IntPtr lParam);
        }
    }