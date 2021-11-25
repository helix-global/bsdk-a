using System;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214e4-0000-0000-c000-000000000046")]
    public interface IContextMenu
        {
        [PreserveSig] HRESULT QueryContextMenu([In] IntPtr hmenu,[In] Int32 indexMenu,[In] Int32 idCmdFirst,[In] Int32 idCmdLast,[In] Int32 uFlags);
        [PreserveSig] HRESULT InvokeCommand([In] ref CMINVOKECOMMANDINFO pici);
        [PreserveSig] HRESULT GetCommandString([In] IntPtr idCmd,[In] Int32 uType,IntPtr pReserved,[In,Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder pszName,[In] Int32 cchMax);
        }
    }