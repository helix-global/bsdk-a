using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformComponents.Win32
    {
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] public delegate IntPtr DLGPROC(IntPtr hWnd, WindowMessage nMsg, IntPtr wParam, IntPtr lParam);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)] public delegate Int32 LPFNPSPCALLBACKW(IntPtr hwnd, PSPCB_ACTION uMsg, ref PROPSHEETPAGEW_V4 psp);

    [StructLayout(LayoutKind.Sequential)]
    public struct PROPSHEETPAGEW_V4
        {
        public Int32 dwSize;
        public PSP_FLAGS dwFlags;
        public IntPtr hInstance;
        public IntPtr pszTemplate;
        public IntPtr hIcon;
        public IntPtr pszTitle;
        public IntPtr pfnDlgProc;
        public IntPtr lParam;
        public IntPtr pfnCallback;
        public IntPtr pcRefParent;
        public IntPtr pszHeaderTitle;
        public IntPtr pszHeaderSubTitle;
        public IntPtr hActCtx;
        public IntPtr pszbmHeader;
        }
    }