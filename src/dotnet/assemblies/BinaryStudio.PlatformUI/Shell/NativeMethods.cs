using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace BinaryStudio.PlatformUI
    {
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct LOGFONT
        {
        public Int32 lfHeight;
        public Int32 lfWidth;
        public Int32 lfEscapement;
        public Int32 lfOrientation;
        public Int32 lfWeight;
        public Byte lfItalic;
        public Byte lfUnderline;
        public Byte lfStrikeOut;
        public Byte lfCharSet;
        public Byte lfOutPrecision;
        public Byte lfClipPrecision;
        public Byte lfQuality;
        public Byte lfPitchAndFamily;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public String lfFaceName;
        }


    internal struct NONCLIENTMETRICS
        {
        private readonly Int32 cbSize;
        private readonly Int32 iBorderWidth;
        private readonly Int32 iScrollWidth;
        public readonly Int32 iScrollHeight;
        public Int32 iCaptionWidth;
        private readonly Int32 iCaptionHeight;
        public readonly LOGFONT lfCaptionFont;
        private readonly Int32 iSmCaptionWidth;
        private Int32 iSmCaptionHeight;
        public LOGFONT lfSmCaptionFont;
        public Int32 iMenuWidth;
        public readonly Int32 iMenuHeight;
        private readonly LOGFONT lfMenuFont;
        private readonly LOGFONT lfStatusFont;
        private LOGFONT lfMessageFont;
        private Int32 iPaddedBorderWidth;
        }
    internal static class NativeMethods
        {
        private const Int32 WS_OVERLAPPED       = 0x00000000;
        private const Int32 WS_POPUP            = unchecked((Int32)0x80000000);
        private const Int32 WS_CHILD            = 0x40000000;
        private const Int32 WS_MINIMIZE         = 0x20000000;
        private const Int32 WS_VISIBLE          = 0x10000000;
        private const Int32 WS_DISABLED         = 0x08000000;
        private const Int32 WS_CLIPSIBLINGS     = 0x04000000;
        private const Int32 WS_CLIPCHILDREN     = 0x02000000;
        private const Int32 WS_MAXIMIZE         = 0x01000000;
        private const Int32 WS_CAPTION          = 0x00C00000;     /* WS_BORDER | WS_DLGFRAME  */
        private const Int32 WS_BORDER           = 0x00800000;
        private const Int32 WS_DLGFRAME         = 0x00400000;
        private const Int32 WS_VSCROLL          = 0x00200000;
        private const Int32 WS_HSCROLL          = 0x00100000;
        private const Int32 WS_SYSMENU          = 0x00080000;
        private const Int32 WS_THICKFRAME       = 0x00040000;
        private const Int32 WS_GROUP            = 0x00020000;
        private const Int32 WS_TABSTOP          = 0x00010000;
        private const Int32 WS_MINIMIZEBOX      = 0x00020000;
        private const Int32 WS_MAXIMIZEBOX      = 0x00010000;
        private const Int32 WS_TILED            = WS_OVERLAPPED;
        private const Int32 WS_ICONIC           = WS_MINIMIZE;
        private const Int32 WS_SIZEBOX          = WS_THICKFRAME;
        private const Int32 WS_TILEDWINDOW      = WS_OVERLAPPEDWINDOW;
        private const Int32 WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
        private const Int32 WS_POPUPWINDOW      = (WS_POPUP | WS_BORDER | WS_SYSMENU);
        private const Int32 WS_CHILDWINDOW      = (WS_CHILD);
        private const Int32 WS_EX_DLGMODALFRAME    = 0x00000001;
        private const Int32 WS_EX_NOPARENTNOTIFY    = 0x00000004;
        private const Int32 WS_EX_TOPMOST           = 0x00000008;
        private const Int32 WS_EX_ACCEPTFILES       = 0x00000010;
        private const Int32 WS_EX_TRANSPARENT       = 0x00000020;
        private const Int32 WS_EX_MDICHILD           = 0x00000040;
        private const Int32 WS_EX_TOOLWINDOW        = 0x00000080;
        private const Int32 WS_EX_WINDOWEDGE        = 0x00000100;
        private const Int32 WS_EX_CLIENTEDGE        = 0x00000200;
        private const Int32 WS_EX_CONTEXTHELP       = 0x00000400;
        private const Int32 WS_EX_RIGHT            = 0x00001000;
        private const Int32 WS_EX_LEFT              = 0x00000000;
        private const Int32 WS_EX_RTLREADING        = 0x00002000;
        private const Int32 WS_EX_LTRREADING        = 0x00000000;
        private const Int32 WS_EX_LEFTSCROLLBAR     = 0x00004000;
        private const Int32 WS_EX_RIGHTSCROLLBAR    = 0x00000000;
        private const Int32 WS_EX_CONTROLPARENT     = 0x00010000;
        private const Int32 WS_EX_STATICEDGE        = 0x00020000;
        private const Int32 WS_EX_APPWINDOW         = 0x00040000;
        private const Int32 WS_EX_OVERLAPPEDWINDOW  = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        private const Int32 WS_EX_PALETTEWINDOW     = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        private const Int32 WS_EX_LAYERED           = 0x00080000;
        private const Int32 WS_EX_NOINHERITLAYOUT   = 0x00100000;
        private const Int32 WS_EX_NOREDIRECTIONBITMAP = 0x00200000;
        private const Int32 WS_EX_LAYOUTRTL         = 0x00400000;
        private const Int32 WS_EX_COMPOSITED        = 0x02000000;
        private const Int32 WS_EX_NOACTIVATE        = 0x08000000;


        internal static readonly IntPtr HRGN_NONE = new IntPtr(-1);
        internal static readonly IntPtr HWND_TOP = IntPtr.Zero;
        internal static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        internal static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        internal static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        internal static readonly IntPtr HWND_BROADCAST = new IntPtr(UInt16.MaxValue);
        internal const Int32 MAX_PATH = 260;
        private static Int32 vsmNotifyOwnerActivate;
        private static Int32 vsmProcessUIBackgroundPriorityTaskQueue;
        internal const Int32 RT_CURSOR = 1;
        internal const Int32 RT_BITMAP = 2;
        internal const Int32 RT_ICON = 3;
        internal const Int32 RT_MENU = 4;
        internal const Int32 RT_DIALOG = 5;
        internal const Int32 RT_STRING = 6;
        internal const Int32 RT_FONTDIR = 7;
        internal const Int32 RT_FONT = 8;
        internal const Int32 RT_ACCELERATOR = 9;
        internal const Int32 RT_RCDATA = 10;
        internal const Int32 RT_MESSAGETABLE = 11;
        internal const Int32 RT_GROUP_CURSOR = 12;
        internal const Int32 RT_GROUP_ICON = 14;
        internal const Int32 RT_VERSION = 16;
        internal const Int32 RT_DLGINCLUDE = 17;
        internal const Int32 RT_PLUGPLAY = 19;
        internal const Int32 RT_VXD = 20;
        internal const Int32 RT_ANICURSOR = 21;
        internal const Int32 RT_ANIICON = 22;
        internal const Int32 RT_HTML = 23;
        internal const Int32 RT_MANIFEST = 24;
        private const Byte KeyDown = 128;
        internal const UInt32 MAPVK_VK_TO_VSC = 0;
        internal const UInt32 MAPVK_VSC_TO_VK = 1;
        internal const UInt32 MAPVK_VK_TO_CHAR = 2;
        internal const UInt32 MAPVK_VSC_TO_VK_EX = 3;
        internal const Int32 VK_LBUTTON = 1;
        internal const Int32 VK_RBUTTON = 2;
        internal const Int32 VK_MBUTTON = 4;
        internal const Int32 VK_XBUTTON1 = 5;
        internal const Int32 VK_XBUTTON2 = 6;
        internal const Int32 VK_TAB = 9;
        internal const Int32 VK_SHIFT = 16;
        internal const Int32 VK_CONTROL = 17;
        internal const Int32 VK_MENU = 18;
        internal const Int32 VK_SPACE = 32;
        internal const Int32 VK_LSHIFT = 160;
        internal const Int32 VK_RSHIFT = 161;
        internal const Int32 VK_LCONTROL = 162;
        internal const Int32 VK_RCONTROL = 163;
        internal const Int32 VK_LMENU = 164;
        public const Int32 VK_RMENU = 165;
        public const Int32 VK_LWIN = 91;
        public const Int32 VK_RWIN = 92;
        public const Int32 VK_F1 = 112;
        public const Int32 VK_F4 = 115;
        public const Int32 VK_ESC = 27;
        public const Int32 VK_RETURN = 13;
        public const Int32 MK_CONTROL = 8;
        public const Int32 MK_LBUTTON = 1;
        public const Int32 MK_MBUTTON = 16;
        public const Int32 MK_RBUTTON = 2;
        public const Int32 MK_SHIFT = 4;
        public const Int32 MK_XBUTTON1 = 32;
        public const Int32 MK_XBUTTON2 = 64;
        public const Int32 XBUTTON1 = 1;
        public const Int32 XBUTTON2 = 2;
        public const Int32 RPC_E_SERVERCALL_RETRYLATER = -2147417846;
        public const Int32 RPC_E_SERVERCALL_REJECTED = -2147417845;
        public const Int32 RPC_E_RETRY = -2147417847;
        public const Int32 RPC_E_DISCONNECTED = -2147417848;
        public const Int32 RPC_E_SYS_CALL_FAILED = -2147417856;
        public const Int32 E_SHARING_VIOLATION = -2147024864;
        internal const Int32 ICON_FORMAT_VERSION = 196608;
        internal const Int32 LR_DEFAULTCOLOR = 0;
        internal const Int32 PICTYPE_UNINITIALIZED = -1;
        internal const Int32 PICTYPE_NONE = 0;
        internal const Int32 PICTYPE_BITMAP = 1;
        internal const Int32 PICTYPE_METAFILE = 2;
        internal const Int32 PICTYPE_ICON = 3;
        internal const Int32 PICTYPE_ENHMETAFILE = 4;
        internal const Int32 BITMAPINFO_MAX_COLORSIZE = 256;
        internal const Int32 DIB_RGB_COLORS = 0;
        internal const Int32 DIB_PAL_COLORS = 1;
        internal const Int32 AC_SRC_OVER = 0;
        internal const Int32 AC_SRC_ALPHA = 1;
        internal const Int32 ULW_ALPHA = 2;
        internal const Int32 BI_RGB = 0;
        internal const Int32 BI_RLE8 = 1;
        internal const Int32 BI_RLE4 = 2;
        internal const Int32 BI_BITFIELDS = 3;
        internal const Int32 BI_JPEG = 4;
        internal const Int32 BI_PNG = 5;
        public const Int32 TRUE = 1;
        public const Int32 FALSE = 0;
        public const Int32 DCX_WINDOW = 1;
        public const Int32 DCX_CACHE = 2;
        public const Int32 DCX_NORESETATTRS = 4;
        public const Int32 DCX_CLIPCHILDREN = 8;
        public const Int32 DCX_CLIPSIBLINGS = 16;
        public const Int32 DCX_PARENTCLIP = 32;
        public const Int32 DCX_EXCLUDERGN = 64;
        public const Int32 DCX_INTERSECTRGN = 128;
        public const Int32 DCX_EXCLUDEUPDATE = 256;
        public const Int32 DCX_INTERSECTUPDATE = 512;
        public const Int32 DCX_LOCKWINDOWUPDATE = 1024;
        public const Int32 ILD_NORMAL = 0;
        public const Int32 ILD_TRANSPARENT = 1;
        public const Int32 ILD_MASK = 16;
        public const Int32 ILD_IMAGE = 32;
        public const Int32 ILD_ROP = 64;
        public const Int32 ILD_BLEND25 = 2;
        public const Int32 ILD_BLEND50 = 4;
        public const Int32 ILD_OVERLAYMASK = 3840;
        public const Int32 ILD_SELECTED = 4;
        public const Int32 ILD_FOCUS = 2;
        public const Int32 ILD_BLEND = 4;
        public const Int32 GA_PARENT = 1;
        public const Int32 GA_ROOT = 2;
        public const Int32 GA_ROOTOWNER = 3;
        public const Int32 GW_FIRST = 0;
        public const Int32 GW_LAST = 1;
        public const Int32 GW_HWNDNEXT = 2;
        public const Int32 GW_HWNDPREV = 3;
        public const Int32 GW_OWNER = 4;
        public const Int32 GW_CHILD = 5;
        public const Int32 HTNOWHERE = 0;
        public const Int32 HTCLIENT = 1;
        public const Int32 HTCAPTION = 2;
        public const Int32 HTSYSMENU = 3;
        public const Int32 HTLEFT = 10;
        public const Int32 HTRIGHT = 11;
        public const Int32 HTTOP = 12;
        public const Int32 HTTOPLEFT = 13;
        public const Int32 HTTOPRIGHT = 14;
        public const Int32 HTBOTTOM = 15;
        public const Int32 HTBOTTOMLEFT = 16;
        public const Int32 HTBOTTOMRIGHT = 17;
        public const Int32 ICON_BIG = 1;
        public const Int32 ICON_SMALL = 0;
        public const Int32 LWA_COLORKEY = 1;
        public const Int32 LWA_ALPHA = 2;
        public const Int32 LOGPIXELSX = 88;
        public const Int32 LOGPIXELSY = 90;
        public const Int32 MA_ACTIVATE = 1;
        public const Int32 MA_ACTIVATEANDEAT = 2;
        public const Int32 MA_NOACTIVATE = 3;
        public const Int32 MA_NOACTIVATEANDEAT = 4;
        public const Int32 MONITOR_DEFAULTTONEAREST = 2;
        public const Int32 SW_HIDE = 0;
        public const Int32 SW_SHOWNORMAL = 1;
        public const Int32 SW_NORMAL = 1;
        public const Int32 SW_SHOWMINIMIZED = 2;
        public const Int32 SW_SHOWMAXIMIZED = 3;
        public const Int32 SW_MAXIMIZE = 3;
        public const Int32 SW_SHOWNOACTIVATE = 4;
        public const Int32 SW_SHOW = 5;
        public const Int32 SW_MINIMIZE = 6;
        public const Int32 SW_SHOWMINNOACTIVE = 7;
        public const Int32 SW_SHOWNA = 8;
        public const Int32 SW_RESTORE = 9;
        public const Int32 SW_SHOWDEFAULT = 10;
        public const Int32 SW_FORCEMINIMIZE = 11;
        public const Int32 SW_MAX = 11;
        public const Int32 SW_PARENTCLOSING = 1;
        public const Int32 SW_OTHERZOOM = 2;
        public const Int32 SW_PARENTOPENING = 3;
        public const Int32 SW_OTHERUNZOOM = 4;
        public const Int32 WA_INACTIVE = 0;
        public const Int32 WA_ACTIVE = 1;
        public const Int32 WA_CLICKACTIVE = 2;
        public const Int32 SC_SIZE = 61440;
        public const Int32 SC_MOVE = 61456;
        public const Int32 SC_MINIMIZE = 61472;
        public const Int32 SC_MAXIMIZE = 61488;
        public const Int32 SC_NEXTWINDOW = 61504;
        public const Int32 SC_PREVWINDOW = 61520;
        public const Int32 SC_CLOSE = 61536;
        public const Int32 SC_VSCROLL = 61552;
        public const Int32 SC_HSCROLL = 61568;
        public const Int32 SC_MOUSEMENU = 61584;
        public const Int32 SC_KEYMENU = 61696;
        public const Int32 SC_ARRANGE = 61712;
        public const Int32 SC_RESTORE = 61728;
        public const Int32 SC_TASKLIST = 61744;
        public const Int32 SC_SCREENSAVE = 61760;
        public const Int32 SC_HOTKEY = 61776;
        public const Int32 SC_DEFAULT = 61792;
        public const Int32 SC_MONITORPOWER = 61808;
        public const Int32 SC_CONTEXTHELP = 61824;
        public const Int32 SC_SEPARATOR = 61455;
        public const Int32 SM_SWAPBUTTON = 23;
        public const Int32 SM_MENUDROPALIGNMENT = 40;
        public const Int32 SPI_SETHIGHCONTRAST = 67;
        public const Int32 SPI_GETNONCLIENTMETRICS = 41;
        public const Int32 SPI_SETNONCLIENTMETRICS = 42;
        private const Int32 SWP_NOSIZE = 0x0001;
        private const Int32 SWP_NOMOVE = 0x0002;
        private const Int32 SWP_NOZORDER = 0x0004;
        private const Int32 SWP_NOREDRAW = 0x0008;
        private const Int32 SWP_NOACTIVATE = 0x0010;
        private const Int32 SWP_FRAMECHANGED = 0x0020;
        private const Int32 SWP_SHOWWINDOW = 0x0040;
        private const Int32 SWP_HIDEWINDOW = 0x0080;
        private const Int32 SWP_NOCOPYBITS = 0x0100;
        private const Int32 SWP_NOOWNERZORDER = 0x0200;
        private const Int32 SWP_NOSENDCHANGING = 0x0400;
        private const Int32 SWP_DEFERERASE = 0x2000;
        private const Int32 SWP_ASYNCWINDOWPOS = 0x4000;
        public const UInt32 TPM_LEFTBUTTON = 0;
        public const UInt32 TPM_RIGHTBUTTON = 2;
        public const UInt32 TPM_LEFTALIGN = 0;
        public const UInt32 TPM_CENTERALIGN = 4;
        public const UInt32 TPM_RIGHTALIGN = 8;
        public const UInt32 TPM_TOPALIGN = 0;
        public const UInt32 TPM_VCENTERALIGN = 16;
        public const UInt32 TPM_BOTTOMALIGN = 32;
        public const UInt32 TPM_HORIZONTAL = 0;
        public const UInt32 TPM_VERTICAL = 64;
        public const UInt32 TPM_NONOTIFY = 128;
        public const UInt32 TPM_RETURNCMD = 256;
        public const UInt32 TPM_RECURSE = 1;
        public const UInt32 TPM_HORPOSANIMATION = 1024;
        public const UInt32 TPM_HORNEGANIMATION = 2048;
        public const UInt32 TPM_VERPOSANIMATION = 4096;
        public const UInt32 TPM_VERNEGANIMATION = 8192;
        public const UInt32 TPM_NOANIMATION = 16384;
        public const UInt32 TPM_LAYOUTRTL = 32768;
        public const UInt32 TPM_WORKAREA = 65536;
        public const Int32 WM_NULL = 0;
        public const Int32 WM_CREATE = 1;
        public const Int32 WM_DESTROY = 2;
        public const Int32 WM_MOVE = 3;
        public const Int32 WM_SIZE = 5;
        private const Int32 WM_ACTIVATE = 0x00000006;
        public const Int32 WM_SETFOCUS = 7;
        public const Int32 WM_KILLFOCUS = 8;
        public const Int32 WM_ENABLE = 0xA;
        public const Int32 WM_SETREDRAW = 11;
        private const Int32 WM_SETTEXT = 0x0000000C;
        public const Int32 WM_GETTEXT = 13;
        public const Int32 WM_GETTEXTLENGTH = 14;
        public const Int32 WM_PAINT = 15;
        public const Int32 WM_CLOSE = 16;
        public const Int32 WM_QUERYENDSESSION = 17;
        public const Int32 WM_QUERYOPEN = 19;
        public const Int32 WM_ENDSESSION = 22;
        public const Int32 WM_QUIT = 18;
        public const Int32 WM_ERASEBKGND = 20;
        public const Int32 WM_SYSCOLORCHANGE = 21;
        public const Int32 WM_SHOWWINDOW = 24;
        public const Int32 WM_WININICHANGE = 26;
        public const Int32 WM_SETTINGCHANGE = 26;
        public const Int32 WM_DEVMODECHANGE = 27;
        public const Int32 WM_ACTIVATEAPP = 28;
        public const Int32 WM_FONTCHANGE = 29;
        public const Int32 WM_TIMECHANGE = 30;
        public const Int32 WM_CANCELMODE = 31;
        public const Int32 WM_SETCURSOR = 32;
        public const Int32 WM_MOUSEACTIVATE = 33;
        public const Int32 WM_CHILDACTIVATE = 34;
        public const Int32 WM_QUEUESYNC = 35;
        public const Int32 WM_GETMINMAXINFO = 36;
        public const Int32 WM_PAINTICON = 38;
        public const Int32 WM_ICONERASEBKGND = 39;
        public const Int32 WM_NEXTDLGCTL = 40;
        public const Int32 WM_SPOOLERSTATUS = 42;
        public const Int32 WM_DRAWITEM = 43;
        public const Int32 WM_MEASUREITEM = 44;
        public const Int32 WM_DELETEITEM = 45;
        public const Int32 WM_VKEYTOITEM = 46;
        public const Int32 WM_CHARTOITEM = 47;
        public const Int32 WM_SETFONT = 48;
        public const Int32 WM_GETFONT = 49;
        public const Int32 WM_SETHOTKEY = 50;
        public const Int32 WM_GETHOTKEY = 51;
        public const Int32 WM_QUERYDRAGICON = 55;
        public const Int32 WM_COMPAREITEM = 57;
        public const Int32 WM_GETOBJECT = 61;
        public const Int32 WM_COMPACTING = 65;
        public const Int32 WM_COMMNOTIFY = 68;
        private const Int32 WM_WINDOWPOSCHANGING = 0x00000046;
        private const Int32 WM_WINDOWPOSCHANGED = 0x00000047;
        public const Int32 WM_POWER = 72;
        public const Int32 WM_COPYDATA = 74;
        public const Int32 WM_CANCELJOURNAL = 75;
        public const Int32 WM_NOTIFY = 78;
        public const Int32 WM_INPUTLANGCHANGEREQUEST = 80;
        public const Int32 WM_INPUTLANGCHANGE = 81;
        public const Int32 WM_TCARD = 82;
        public const Int32 WM_HELP = 83;
        public const Int32 WM_USERCHANGED = 84;
        public const Int32 WM_NOTIFYFORMAT = 85;
        public const Int32 WM_CONTEXTMENU = 123;
        public const Int32 WM_STYLECHANGING = 124;
        public const Int32 WM_STYLECHANGED = 125;
        public const Int32 WM_DISPLAYCHANGE = 126;
        public const Int32 WM_GETICON = 127;
        private const Int32 WM_SETICON = 0x00000080;
        public const Int32 WM_NCCREATE = 129;
        public const Int32 WM_NCDESTROY = 130;
        private const Int32 WM_NCCALCSIZE = 0x00000083;
        private const Int32 WM_NCHITTEST = 0x00000084;
        private const Int32 WM_NCPAINT = 0x00000085;
        private const Int32 WM_NCACTIVATE = 0x00000086;
        public const Int32 WM_GETDLGCODE = 135;
        public const Int32 WM_SYNCPAINT = 136;
        public const Int32 WM_NCMOUSEMOVE = 160;
        public const Int32 WM_NCLBUTTONDOWN = 161;
        public const Int32 WM_NCLBUTTONUP = 162;
        public const Int32 WM_NCLBUTTONDBLCLK = 163;
        private const Int32 WM_NCRBUTTONDOWN = 0x000000A4;
        private const Int32 WM_NCRBUTTONUP = 0x000000A5;
        private const Int32 WM_NCRBUTTONDBLCLK = 0x000000A6;
        public const Int32 WM_NCMBUTTONDOWN = 167;
        public const Int32 WM_NCMBUTTONUP = 168;
        public const Int32 WM_NCMBUTTONDBLCLK = 169;
        public const Int32 WM_NCXBUTTONDOWN = 171;
        public const Int32 WM_NCXBUTTONUP = 172;
        public const Int32 WM_NCXBUTTONDBLCLK = 173;
        private const Int32 WM_NCUAHDRAWCAPTION = 0x000000AE;
        private const Int32 WM_NCUAHDRAWFRAME = 0x000000AF;
        public const Int32 WM_INPUT = 255;
        public const Int32 WM_KEYFIRST = 256;
        public const Int32 WM_KEYDOWN = 256;
        public const Int32 WM_KEYUP = 257;
        public const Int32 WM_CHAR = 258;
        public const Int32 WM_DEADCHAR = 259;
        public const Int32 WM_SYSKEYDOWN = 260;
        public const Int32 WM_SYSKEYUP = 261;
        public const Int32 WM_SYSCHAR = 262;
        public const Int32 WM_SYSDEADCHAR = 263;
        public const Int32 WM_UNICHAR = 265;
        public const Int32 WM_KEYLAST = 264;
        public const Int32 WM_IME_STARTCOMPOSITION = 269;
        public const Int32 WM_IME_ENDCOMPOSITION = 270;
        public const Int32 WM_IME_COMPOSITION = 271;
        public const Int32 WM_IME_KEYLAST = 271;
        public const Int32 WM_INITDIALOG = 272;
        public const Int32 WM_COMMAND = 273;
        private const Int32 WM_SYSCOMMAND = 0x00000112;
        public const Int32 WM_TIMER = 275;
        public const Int32 WM_HSCROLL = 276;
        public const Int32 WM_VSCROLL = 277;
        public const Int32 WM_INITMENU = 278;
        public const Int32 WM_INITMENUPOPUP = 279;
        public const Int32 WM_MENUSELECT = 287;
        public const Int32 WM_MENUCHAR = 288;
        public const Int32 WM_ENTERIDLE = 289;
        public const Int32 WM_MENURBUTTONUP = 290;
        public const Int32 WM_MENUDRAG = 291;
        public const Int32 WM_MENUGETOBJECT = 292;
        public const Int32 WM_UNINITMENUPOPUP = 293;
        public const Int32 WM_MENUCOMMAND = 294;
        public const Int32 WM_CHANGEUISTATE = 295;
        public const Int32 WM_UPDATEUISTATE = 296;
        public const Int32 WM_QUERYUISTATE = 297;
        public const Int32 WM_CTLCOLOR = 25;
        public const Int32 WM_CTLCOLORMSGBOX = 306;
        public const Int32 WM_CTLCOLOREDIT = 307;
        public const Int32 WM_CTLCOLORLISTBOX = 308;
        public const Int32 WM_CTLCOLORBTN = 309;
        public const Int32 WM_CTLCOLORDLG = 310;
        public const Int32 WM_CTLCOLORSCROLLBAR = 311;
        public const Int32 WM_CTLCOLORSTATIC = 312;
        public const Int32 WM_MOUSEFIRST = 512;
        public const Int32 WM_MOUSEMOVE = 512;
        public const Int32 WM_LBUTTONDOWN = 513;
        public const Int32 WM_LBUTTONUP = 514;
        public const Int32 WM_LBUTTONDBLCLK = 515;
        public const Int32 WM_RBUTTONDOWN = 516;
        public const Int32 WM_RBUTTONUP = 517;
        public const Int32 WM_RBUTTONDBLCLK = 518;
        public const Int32 WM_MBUTTONDOWN = 519;
        public const Int32 WM_MBUTTONUP = 520;
        public const Int32 WM_MBUTTONDBLCLK = 521;
        public const Int32 WM_MOUSEWHEEL = 522;
        public const Int32 WM_XBUTTONDOWN = 523;
        public const Int32 WM_XBUTTONUP = 524;
        public const Int32 WM_XBUTTONDBLCLK = 525;
        public const Int32 WM_MOUSELAST = 525;
        public const Int32 WM_PARENTNOTIFY = 528;
        public const Int32 WM_ENTERMENULOOP = 529;
        public const Int32 WM_EXITMENULOOP = 530;
        public const Int32 WM_NEXTMENU = 531;
        public const Int32 WM_SIZING = 532;
        public const Int32 WM_CAPTURECHANGED = 533;
        public const Int32 WM_MOVING = 534;
        public const Int32 WM_POWERBROADCAST = 536;
        public const Int32 WM_DEVICECHANGE = 537;
        public const Int32 WM_MDICREATE = 544;
        public const Int32 WM_MDIDESTROY = 545;
        public const Int32 WM_MDIACTIVATE = 546;
        public const Int32 WM_MDIRESTORE = 547;
        public const Int32 WM_MDINEXT = 548;
        public const Int32 WM_MDIMAXIMIZE = 549;
        public const Int32 WM_MDITILE = 550;
        public const Int32 WM_MDICASCADE = 551;
        public const Int32 WM_MDIICONArANGE = 552;
        public const Int32 WM_MDIGETACTIVE = 553;
        public const Int32 WM_MDISETMENU = 560;
        public const Int32 WM_ENTERSIZEMOVE = 561;
        public const Int32 WM_EXITSIZEMOVE = 562;
        public const Int32 WM_DROPFILES = 563;
        public const Int32 WM_MDIREFRESHMENU = 564;
        public const Int32 WM_IME_SETCONTEXT = 641;
        public const Int32 WM_IME_NOTIFY = 642;
        public const Int32 WM_IME_CONTROL = 643;
        public const Int32 WM_IME_COMPOSITIONFULL = 644;
        public const Int32 WM_IME_SELECT = 645;
        public const Int32 WM_IME_CHAR = 646;
        public const Int32 WM_IME_REQUEST = 648;
        public const Int32 WM_IME_KEYDOWN = 656;
        public const Int32 WM_IME_KEYUP = 657;
        public const Int32 WM_MOUSEHOVER = 673;
        public const Int32 WM_MOUSELEAVE = 675;
        public const Int32 WM_NCMOUSELEAVE = 674;
        public const Int32 WM_WTSSESSION_CHANGE = 689;
        public const Int32 WM_TABLET_FIRST = 704;
        public const Int32 WM_TABLET_LAST = 735;
        public const Int32 WM_CUT = 768;
        public const Int32 WM_COPY = 769;
        public const Int32 WM_PASTE = 770;
        public const Int32 WM_CLEAR = 771;
        public const Int32 WM_UNDO = 772;
        public const Int32 WM_RENDERFORMAT = 773;
        public const Int32 WM_RENDERALLFORMATS = 774;
        public const Int32 WM_DESTROYCLIPBOARD = 775;
        public const Int32 WM_DRAWCLIPBOARD = 776;
        public const Int32 WM_PAINTCLIPBOARD = 777;
        public const Int32 WM_VSCROLLCLIPBOARD = 778;
        public const Int32 WM_SIZECLIPBOARD = 779;
        public const Int32 WM_ASKCBFORMATNAME = 780;
        public const Int32 WM_CHANGECBCHAIN = 781;
        public const Int32 WM_HSCROLLCLIPBOARD = 782;
        public const Int32 WM_QUERYNEWPALETTE = 783;
        public const Int32 WM_PALETTEISCHANGING = 784;
        public const Int32 WM_PALETTECHANGED = 785;
        public const Int32 WM_HOTKEY = 786;
        public const Int32 WM_PRINT = 791;
        public const Int32 WM_PRINTCLIENT = 792;
        public const Int32 WM_APPCOMMAND = 793;
        public const Int32 WM_THEMECHANGED = 794;
        public const Int32 WM_HANDHELDFIRST = 856;
        public const Int32 WM_HANDHELDLAST = 863;
        public const Int32 WM_AFXFIRST = 864;
        public const Int32 WM_AFXLAST = 895;
        public const Int32 WM_PENWINFIRST = 896;
        public const Int32 WM_PENWINLAST = 911;
        public const Int32 WM_USER = 1024;
        public const Int32 WM_REFLECT = 8192;
        public const Int32 WM_APP = 32768;
        //public const Int32 WS_OVERLAPPED = 0;
        //public const Int32 WS_POPUP = -2147483648;
        //public const Int32 WS_CHILD = 1073741824;
        //public const Int32 WS_MINIMIZE = 536870912;
        //public const Int32 WS_VISIBLE = 268435456;
        //public const Int32 WS_DISABLED = 134217728;
        //public const Int32 WS_CLIPSIBLINGS = 67108864;
        //public const Int32 WS_CLIPCHILDREN = 33554432;
        //public const Int32 WS_MAXIMIZE = 16777216;
        //public const Int32 WS_CAPTION = 12582912;
        //public const Int32 WS_BORDER = 8388608;
        //public const Int32 WS_DLGFRAME = 4194304;
        //public const Int32 WS_VSCROLL = 2097152;
        //public const Int32 WS_HSCROLL = 1048576;
        //public const Int32 WS_SYSMENU = 524288;
        //public const Int32 WS_THICKFRAME = 262144;
        //public const Int32 WS_GROUP = 131072;
        //public const Int32 WS_TABSTOP = 65536;
        //public const Int32 WS_MINIMIZEBOX = 131072;
        //public const Int32 WS_MAXIMIZEBOX = 65536;
        //public const Int32 WS_TILED = 0;
        //public const Int32 WS_ICONIC = 536870912;
        //public const Int32 WS_SIZEBOX = 262144;
        //public const Int32 WS_TILEDWINDOW = 13565952;
        //public const Int32 WS_OVERLAPPEDWINDOW = 13565952;
        //public const Int32 WS_POPUPWINDOW = -2138570752;
        //public const Int32 WS_CHILDWINDOW = 1073741824;
        //public const Int32 WS_EX_DLGMODALFRAME = 1;
        //public const Int32 WS_EX_NOPARENTNOTIFY = 4;
        //public const Int32 WS_EX_TOPMOST = 8;
        //public const Int32 WS_EX_ACCEPTFILES = 16;
        //public const Int32 WS_EX_TRANSPARENT = 32;
        //public const Int32 WS_EX_MDICHILD = 64;
        //public const Int32 WS_EX_TOOLWINDOW = 128;
        //public const Int32 WS_EX_WINDOWEDGE = 256;
        //public const Int32 WS_EX_CLIENTEDGE = 512;
        //public const Int32 WS_EX_CONTEXTHELP = 1024;
        //public const Int32 WS_EX_RIGHT = 4096;
        //public const Int32 WS_EX_LEFT = 0;
        //public const Int32 WS_EX_RTLREADING = 8192;
        //public const Int32 WS_EX_LTRREADING = 0;
        //public const Int32 WS_EX_LEFTSCROLLBAR = 16384;
        //public const Int32 WS_EX_RIGHTSCROLLBAR = 0;
        //public const Int32 WS_EX_CONTROLPARENT = 65536;
        //public const Int32 WS_EX_STATICEDGE = 131072;
        //public const Int32 WS_EX_APPWINDOW = 262144;
        //public const Int32 WS_EX_OVERLAPPEDWINDOW = 768;
        //public const Int32 WS_EX_PALETTEWINDOW = 392;
        //public const Int32 WS_EX_LAYERED = 524288;
        //public const Int32 WS_EX_NOINHERITLAYOUT = 1048576;
        //public const Int32 WS_EX_LAYOUTRTL = 4194304;
        //public const Int32 WS_EX_COMPOSITED = 33554432;
        //public const Int32 WS_EX_NOACTIVATE = 134217728;
        public const Int32 CBN_ERRSPACE = -1;
        public const Int32 CBN_SELCHANGE = 1;
        public const Int32 CBN_DBLCLK = 2;
        public const Int32 CBN_SETFOCUS = 3;
        public const Int32 CBN_KILLFOCUS = 4;
        public const Int32 CBN_EDITCHANGE = 5;
        public const Int32 CBN_EDITUPDATE = 6;
        public const Int32 CBN_DROPDOWN = 7;
        public const Int32 CBN_CLOSEUP = 8;
        public const Int32 CBN_SELENDOK = 9;
        public const Int32 CBN_SELENDCANCEL = 10;
        public const Int32 UIS_SET = 1;
        public const Int32 UIS_CLEAR = 2;
        public const Int32 UIS_INITIALIZE = 3;
        public const Int32 UISF_HIDEFOCUS = 1;
        public const Int32 UISF_HIDEACCEL = 2;
        public const Int32 UISF_ACTIVE = 4;
        internal const UInt32 CLSCTX_INPROC_SERVER = 1;
        public const Int32 CHILDID_SELF = 0;
        public const UInt32 MF_BYCOMMAND = 0;
        public const UInt32 MF_BYPOSITION = 1024;
        public const UInt32 MF_ENABLED = 0;
        public const UInt32 MF_GRAYED = 1;
        public const UInt32 MF_DISABLED = 2;
        private const Int32 VBM__BASE = 4096;
        public const Int32 VSINPUT_PROCESSING_MSG = 4242;
        internal const Int32 PSN_APPLY = -202;
        internal const Int32 PSN_KILLACTIVE = -201;
        internal const Int32 PSN_RESET = -203;
        internal const Int32 PSN_SETACTIVE = -200;
        internal const Int32 PSN_QUERYCANCEL = -209;
        internal const Int32 QS_KEY = 1;
        internal const Int32 QS_MOUSEMOVE = 2;
        internal const Int32 QS_MOUSEBUTTON = 4;
        internal const Int32 QS_POSTMESSAGE = 8;
        internal const Int32 QS_TIMER = 16;
        internal const Int32 QS_PAINT = 32;
        internal const Int32 QS_SENDMESSAGE = 64;
        internal const Int32 QS_HOTKEY = 128;
        internal const Int32 QS_ALLPOSTMESSAGE = 256;
        internal const Int32 QS_MOUSE = 6;
        internal const Int32 QS_INPUT = 7;
        internal const Int32 QS_ALLEVENTS = 191;
        internal const Int32 QS_ALLINPUT = 255;
        internal const Int32 QS_EVENT = 8192;
        internal const Int32 PM_NOREMOVE = 0;
        internal const Int32 PM_REMOVE = 1;
        internal const Int32 PM_NOYIELD = 2;
        internal const Int32 MWMO_WAITALL = 1;
        internal const Int32 MWMO_ALERTABLE = 2;
        internal const Int32 MWMO_INPUTAVAILABLE = 4;
        internal const Int32 DLGC_WANTARROWS = 1;
        internal const Int32 DLGC_WANTTAB = 2;
        internal const Int32 DLGC_WANTALLKEYS = 4;
        internal const Int32 DLGC_WANTMESSAGE = 4;
        internal const Int32 DLGC_HASSETSEL = 8;
        internal const Int32 DLGC_DEFPUSHBUTTON = 16;
        internal const Int32 DLGC_UNDEFPUSHBUTTON = 32;
        internal const Int32 DLGC_RADIOBUTTON = 64;
        internal const Int32 DLGC_WANTCHARS = 128;
        internal const Int32 DLGC_STATIC = 256;
        internal const Int32 DLGC_BUTTON = 8192;

        public static Int32 NOTIFYOWNERACTIVATE {
            get
                {
                if (vsmNotifyOwnerActivate == 0)
                    vsmNotifyOwnerActivate = RegisterWindowMessage("NOTIFYOWNERACTIVATE{A982313C-756C-4da9-8BD0-0C375A45784B}");
                return vsmNotifyOwnerActivate;
                }
            }

        public static Int32 PROCESSUIBACKGROUNDTASKS {
            get
                {
                if (vsmProcessUIBackgroundPriorityTaskQueue == 0)
                    vsmProcessUIBackgroundPriorityTaskQueue = RegisterWindowMessage("PROCESSUIBACKGROUNDTASKS{A982313C-756C-4da9-8BD0-0C375A45784B}");
                return vsmProcessUIBackgroundPriorityTaskQueue;
                }
            }

        internal static unsafe ModifierKeys ModifierKeys {
            get
                {
                Byte* ptr = stackalloc Byte[256];
                var keyboardState = GetKeyboardState(ptr);
                var modifierKeys = ModifierKeys.None;
                if ((ptr[16] & 128) == 128) {
                    modifierKeys |= ModifierKeys.Shift;
                    }
                if ((ptr[17] & 128) == 128) {
                    modifierKeys |= ModifierKeys.Control;
                    }
                if ((ptr[18] & 128) == 128) {
                    modifierKeys |= ModifierKeys.Alt;
                    }
                if ((ptr[91] & 128) == 128 || (ptr[92] & 128) == 128) {
                    modifierKeys |= ModifierKeys.Windows;
                    }
                return modifierKeys;
                }
            }


        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll")]
        public static extern IntPtr GetCapture();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean ShowOwnedPopups(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] Boolean fShow);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean GetWindowPlacement(IntPtr hwnd, WINDOWPLACEMENT lpwndpl);

        public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hwnd) {
            var lpwndpl = new WINDOWPLACEMENT();
            if (GetWindowPlacement(hwnd, lpwndpl))
                return lpwndpl;
            throw new Win32Exception(Marshal.GetLastWin32Error());
            }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean GetCursorPos(ref POINT point);

        internal static Point GetCursorPos() {
            var point1 = new POINT { x = 0, y = 0 };
            var point2 = new Point();
            if (GetCursorPos(ref point1)) {
                point2.X = point1.x;
                point2.Y = point1.y;
                }
            return point2;
            }

        [DllImport("user32.dll")]
        public static extern Int32 GetSysColor(Int32 nIndex);

        [DllImport("user32.dll")]
        internal static extern Int32 GetSystemMetrics(Int32 index);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean SystemParametersInfo(Int32 uiAction, Int32 uiParam, ref NONCLIENTMETRICS pvParam, Int32 fWinIni);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetSystemMenu(IntPtr hwnd, Boolean bRevert);

        [DllImport("user32.dll")]
        internal static extern Int32 TrackPopupMenuEx(IntPtr hmenu, UInt32 fuFlags, Int32 x, Int32 y, IntPtr hwnd, IntPtr lptpm);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean EnableMenuItem(IntPtr menu, UInt32 uIDEnableItem, UInt32 uEnable);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 RegisterWindowMessage(String lpString);

        [DllImport("user32.dll")]
        internal static extern Int16 GetAsyncKeyState(Int32 vKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 GetMessagePos();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 MsgWaitForMultipleObjectsEx(Int32 nCount, IntPtr[] pHandles, Int32 dwMilliseconds, Int32 dwWakeMask, Int32 dwFlags);

        [DllImport("user32.dll")]
        internal static extern Int16 GetKeyState(Int32 vKey);

        internal static Boolean IsKeyPressed(Int32 vKey) {
            return GetKeyState(vKey) < 0;
            }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibraryEx(String filename, IntPtr hReservedNull, LoadLibraryFlags flags);

        public static IntPtr LoadLibraryEx(String filename, LoadLibraryFlags flags) {
            return LoadLibraryEx(filename, IntPtr.Zero, flags);
            }

        public static IntPtr LoadLibraryAsDataFile(String filename) {
            return LoadLibraryEx(filename, IntPtr.Zero, LoadLibraryFlags.LOAD_LIBRARY_AS_DATAFILE);
            }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean FreeLibrary(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr FindResource(IntPtr hModule, Int32 lpName, Int32 lpType);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindResource(IntPtr hModule, Int32 lpName, String lpType);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindResource(IntPtr hModule, String lpName, Int32 lpType);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr FindResource(IntPtr hModule, String lpName, String lpType);

        public static IntPtr FindResource(IntPtr hModule, NativeResourceIdentifier nameId, NativeResourceIdentifier typeId) {
            if (nameId.IsIntegerId) {
                if (typeId.IsIntegerId)
                    return FindResource(hModule, nameId.IntegerId, typeId.IntegerId);
                return FindResource(hModule, nameId.IntegerId, typeId.StringId);
                }
            if (typeId.IsIntegerId)
                return FindResource(hModule, nameId.StringId, typeId.IntegerId);
            return FindResource(hModule, nameId.StringId, typeId.StringId);
            }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Int32 SizeofResource(IntPtr hModule, IntPtr hResInfo);

        //[DllImport("ComCtl32.dll")]
        //public static extern Boolean SetWindowSubclass(IntPtr hWnd, SubClassProc pfnSubclass, UIntPtr uIdSubclass, UIntPtr dwRefData);
        //[DllImport("ComCtl32.dll")]
        //public static extern Boolean RemoveWindowSubclass(IntPtr hWnd, SubClassProc pfnSubclass, UIntPtr uIdSubclass);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr EnumResourceNames(IntPtr hModule, String type, EnumResourceNameProc callback, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr EnumResourceNames(IntPtr hModule, Int32 id, EnumResourceNameProc callback, IntPtr lParam);

        public static IntPtr EnumResourceNames(IntPtr hModule, NativeResourceIdentifier id, EnumResourceNameProc callback, IntPtr lParam) {
            if (id.IsIntegerId)
                return EnumResourceNames(hModule, id.IntegerId, callback, lParam);
            return EnumResourceNames(hModule, id.StringId, callback, lParam);
            }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr LoadImage(IntPtr hModule, String name, LoadImageType type, Int32 cx, Int32 cy, LoadImageFlags flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LoadImage(IntPtr hModule, Int32 id, LoadImageType type, Int32 cx, Int32 cy, LoadImageFlags flags);

        public static IntPtr LoadImage(IntPtr hModule, NativeResourceIdentifier id, LoadImageType type, Int32 cx, Int32 cy, LoadImageFlags flags) {
            if (id.IsIntegerId)
                return LoadImage(hModule, id.IntegerId, type, cx, cy, flags);
            return LoadImage(hModule, id.StringId, type, cx, cy, flags);
            }

        internal static Boolean IsLeftButtonPressed() {
            return IsKeyPressed(1);
            }

        internal static Boolean IsRightButtonPressed() {
            return IsKeyPressed(2);
            }

        internal static Boolean IsControlPressed() {
            return IsKeyPressed(17);
            }

        internal static Boolean IsShiftPressed() {
            return IsKeyPressed(16);
            }

        internal static Boolean IsAltPressed() {
            return IsKeyPressed(18);
            }

        internal static IntPtr MakeParam(Int32 lowWord, Int32 highWord) {
            return new IntPtr(lowWord & UInt16.MaxValue | highWord << 16);
            }

        internal static Int32 GetXLParam(Int32 lParam) {
            return LoWord(lParam);
            }

        internal static Int32 GetYLParam(Int32 lParam) {
            return HiWord(lParam);
            }

        internal static Int32 HiWord(Int32 value) {
            return (Int16)(value >> 16);
            }

        internal static Int32 LoWord(Int32 value) {
            return (Int16)(value & UInt16.MaxValue);
            }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean ScreenToClient(IntPtr hWnd, ref POINT point);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern unsafe Boolean GetKeyboardState(Byte* lpKeyState);

        [DllImport("user32.dll")]
        internal static extern UInt32 MapVirtualKey(UInt32 uCode, UInt32 uMapType);

        [DllImport("user32.dll")]
        internal static extern IntPtr MonitorFromWindow(IntPtr handle, Int32 flags);

        [DllImport("user32.dll")]
        internal static extern IntPtr MonitorFromPoint(POINT pt, Int32 flags);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern UInt32 GetWindowThreadProcessId(IntPtr hWnd, out UInt32 processId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean ClientToScreen(IntPtr hWnd, ref POINT point);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern IntPtr DefWindowProc(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean EnumThreadWindows(UInt32 dwThreadId, EnumWindowsProc lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SetWindowText(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)] String lpString);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean PostMessage(IntPtr hWnd, Int32 nMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean PostThreadMessage(UInt32 threadId, UInt32 msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean PrintWindow(IntPtr hwnd, IntPtr hDC, UInt32 nFlags);

        internal static Boolean PrintWindow(HandleRef hwnd, HandleRef hDC, UInt32 nFlags) {
            return PrintWindow(hwnd.Handle, hDC.Handle, nFlags);
            }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Int32 GetBitmapBits(IntPtr hbmp, Int32 cbBuffer, Byte[] lpvBits);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Int32 GetBitmapBits(IntPtr hbmp, Int32 cbBuffer, IntPtr lpvBits);

        [DllImport("gdi32.dll")]
        internal static extern Int32 GetDIBits(IntPtr hdc, IntPtr hbmp, UInt32 uStartScan, UInt32 cScanLines, [Out] Byte[] lpvBits, ref BITMAPINFO lpbmi, UInt32 uUsage);

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateBitmap(Int32 nWidth, Int32 nHeight, UInt32 cPlanes, UInt32 cBitsPerPel, IntPtr lpvBits);

        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetDC(IntPtr hWnd);

        internal static IntPtr GetDC(HandleRef hWnd) {
            return GetDC(hWnd.Handle);
            }

        [DllImport("User32.dll")]
        internal static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, Int32 dwFlags);

        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 ReleaseDC(IntPtr hWnd, IntPtr hDC);

        internal static Int32 ReleaseDC(HandleRef hWnd, HandleRef hDC) {
            return ReleaseDC(hWnd.Handle, hDC.Handle);
            }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean DeleteDC(IntPtr hdc);

        internal static Boolean DeleteDC(HandleRef hdc) {
            return DeleteDC(hdc.Handle);
            }

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        internal static IntPtr CreateCompatibleDC(HandleRef hdc) {
            return CreateCompatibleDC(hdc.Handle);
            }

        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, Int32 nWidth, Int32 nHeight);

        internal static IntPtr CreateCompatibleBitmap(HandleRef hdc, Int32 nWidth, Int32 nHeight) {
            return CreateCompatibleBitmap(hdc.Handle, nWidth, nHeight);
            }

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        internal static IntPtr SelectObject(HandleRef hdc, IntPtr hgdiobj) {
            return SelectObject(hdc.Handle, hgdiobj);
            }

        [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 GetDeviceCaps(IntPtr hdc, Int32 nIndex);

        [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr CreateRoundRectRgn(Int32 nLeftRect, Int32 nTopRect, Int32 nRightRect, Int32 nBottomRect, Int32 nWidthEllipse, Int32 nHeightEllipse);

        [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr CreateRectRgn(Int32 nLeftRect, Int32 nTopRect, Int32 nRightRect, Int32 nBottomRect);

        [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr CreateRectRgnIndirect(ref RECT lprc);

        [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern Int32 CombineRgn(IntPtr hrngDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, Int32 fnCombineMode);

        internal static Int32 CombineRgn(IntPtr hrnDest, IntPtr hrgnSrc1, IntPtr hrgnSrc2, CombineMode combineMode) {
            return CombineRgn(hrnDest, hrgnSrc1, hrgnSrc2, (Int32)combineMode);
            }

        [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall)]
        internal static extern Int32 SetWindowRgn(IntPtr hWnd, IntPtr hRgn, [MarshalAs(UnmanagedType.Bool)] Boolean redraw);

        internal static Boolean PostMessage(IntPtr hwnd, Int32 msg) {
            return PostMessage(hwnd, msg, IntPtr.Zero, IntPtr.Zero);
            }

        internal static Boolean PostMessage(IntPtr hwnd, Int32 msg, IntPtr wParam) {
            return PostMessage(hwnd, msg, wParam, IntPtr.Zero);
            }

        [DllImport("user32.dll")]
        internal static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean ReleaseCapture();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, Int32 nMsg, IntPtr wParam, IntPtr lParam);

        internal static IntPtr SendMessage(IntPtr hwnd, Int32 msg) {
            return SendMessage(hwnd, msg, IntPtr.Zero, IntPtr.Zero);
            }

        internal static IntPtr SendMessage(IntPtr hwnd, Int32 msg, IntPtr wParam) {
            return SendMessage(hwnd, msg, wParam, IntPtr.Zero);
            }

        [DllImport("user32.dll")] public static extern IntPtr SetFocus(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hwnd, Int32 nCmd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport("user32.dll")]
        internal static extern Int32 GetMessageTime();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean IsWindowEnabled(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean IsWindowVisible(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean IsIconic(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean IsZoomed(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean IsWindow(IntPtr hWnd);

        [DllImport("ole32.dll")]
        internal static extern Int32 OleSetContainedObject(IntPtr punkObj, [MarshalAs(UnmanagedType.Bool)] Boolean fVisible);

        [DllImport("ole32.dll")]
        internal static extern Int32 OleRun(IntPtr punkObj);

        [DllImport("user32.dll")]
        private static extern Int32 CopyAcceleratorTable(IntPtr hAccelSrc, [Out] ACCEL[] lpAccelDst, Int32 cAccelEntries);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        internal static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean IsChild(IntPtr hWndParent, IntPtr hWnd);

        [DllImport("User32", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, Int32 x, Int32 y, Int32 cx, Int32 cy, Int32 flags);

        [DllImport("Comctl32", CharSet = CharSet.Auto)]
        internal static extern Int32 ImageList_Draw(HandleRef hImageList, Int32 i, HandleRef hdc, Int32 x, Int32 y, Int32 fStyle);

        [DllImport("user32.dll")]
        private static extern Int32 GetWindowLong(IntPtr hWnd, Int32 nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, Int32 nIndex);

        [DllImport("comctl32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean SetWindowSubclass(this IntPtr hwnd, SubclassProc callback, UIntPtr id, IntPtr refData);

        [DllImport("comctl32.dll")]
        internal static extern IntPtr DefSubclassProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam);

        [DllImport("comctl32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean RemoveWindowSubclass(IntPtr hwnd, SubclassProc callback, UIntPtr id);

        internal static IntPtr GetWindowLongPtr(IntPtr hWnd, GWLP nIndex) {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr(hWnd, (Int32)nIndex);
            return new IntPtr(GetWindowLong(hWnd, (Int32)nIndex));
            }

        internal static Int32 GetWindowLong(IntPtr hWnd, GWL nIndex) {
            return GetWindowLong(hWnd, (Int32)nIndex);
            }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean GetClientRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern Int32 SetWindowLong(IntPtr hWnd, Int32 nIndex, Int32 dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLong")]
        internal static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, Int32 nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowLongPtr")]
        internal static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, Int32 nIndex, IntPtr dwNewLong);

        internal static IntPtr SetWindowLong(IntPtr hWnd, Int32 nIndex, IntPtr dwNewLong) {
            if (IntPtr.Size == 4) {
                return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
                }
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, Int32 nIndex, IntPtr dwNewLong);

        public static IntPtr SetWindowLongPtr(IntPtr hWnd, GWLP nIndex, IntPtr dwNewLong) {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr(hWnd, (Int32)nIndex, dwNewLong);
            return new IntPtr(SetWindowLong(hWnd, (Int32)nIndex, dwNewLong.ToInt32()));
            }

        public static IntPtr SetWndProc(IntPtr hWnd, WndProc newWndProc) {
            return SetWindowLongPtr(hWnd, GWLP.WNDPROC, Marshal.GetFunctionPointerForDelegate(newWndProc));
            }

        public static Int32 SetWindowLong(IntPtr hWnd, GWL nIndex, Int32 dwNewLong) {
            return SetWindowLong(hWnd, (Int32)nIndex, dwNewLong);
            }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SetLayeredWindowAttributes(IntPtr hwnd, Int32 crKey, Byte bAlpha, Int32 dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern UInt16 RegisterClassEx(ref WNDCLASSEX lpWndClass);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] public static extern UInt16 RegisterClass(ref WNDCLASS lpWndClass);
        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern Int32 GetClassName(IntPtr window, StringBuilder classname, Int32 size);
        internal static String GetClassName(IntPtr window) {
            var builder = new StringBuilder(2048);
            var sz = GetClassName(window, builder, builder.Capacity);
            return builder.ToString();
            }

        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenEvent(UInt32 dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] Boolean bInheritHandle, String lpName);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SetEvent(IntPtr hEvent);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll")]
        public static extern UInt32 GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        public static extern UInt32 GetCurrentProcessId();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(WindowsHookType hookType, WindowsHookProc hookProc, IntPtr module, UInt32 threadId);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, CbtHookAction code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 VariantClear(ref VARIANT var);

        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode)]
        public static extern Int32 VariantChangeType(out Object pvargDest, [In] ref Object pvarSrc, VARIANTFLAGS wFlags, VARTYPE vt);

        [DllImport("gdi32.dll")]
        public static extern IntPtr GetStockObject(Int32 fnObject);

        public static IntPtr GetStockObject(StockObjects objectID) {
            return GetStockObject((Int32)objectID);
            }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateWindowEx(Int32 dwExStyle, IntPtr classAtom, String lpWindowName, Int32 dwStyle, Int32 x, Int32 y, Int32 nWidth, Int32 nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CreateWindowEx(Int32 dwExStyle, String className, String lpWindowName, Int32 dwStyle, Int32 x, Int32 y, Int32 nWidth, Int32 nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle(String moduleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern Boolean GetFileInformationByHandleEx(IntPtr hFile, UInt32 FileInformationClass, out _FILE_STANDARD_INFO lpFileInformation, UInt32 dwBufferSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Boolean GetFileInformationByHandleEx(IntPtr hFile, UInt32 FileInformationClass, out _FILE_COMPRESSION_INFORMATION lpFileInformation, UInt32 dwBufferSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Boolean GetDiskFreeSpace(String lpRootPathName, out UInt32 lpSectorsPerCluster, out UInt32 lpBytesPerSector, out UInt32 lpNumberOfFreeClusters, out UInt32 lpTotalNumberOfClusters);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Boolean GetVolumeInformation(String RootPathName, StringBuilder VolumeNameBuffer, Int32 VolumeNameSize, out UInt32 VolumeSerialNumber, out UInt32 MaximumComponentLength, out FileSystemFeature FileSystemFlags, StringBuilder FileSystemNameBuffer, Int32 nFileSystemNameSize);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean DeviceIoControl(IntPtr hDevice, Int32 dwIoControlCode, ref Int16 InBuffer, Int32 nInBufferSize, IntPtr OutBuffer, Int32 nOutBufferSize, ref Int32 pBytesReturned, IntPtr lpOverlapped);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean TranslateMessage([In] ref MSG lpMsg);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr DispatchMessage([In] ref MSG lpmsg);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean PeekMessage(out MSG lpMsg, IntPtr hWnd, UInt32 wMsgFilterMin, UInt32 wMsgFilterMax, UInt32 wRemoveMsg);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(Int32 colorref);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean DeleteObject(IntPtr hObject);

        public static Boolean DeleteObject(HandleRef hObject) {
            return DeleteObject(hObject.Handle);
            }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean ShowWindow(IntPtr hwnd, Int32 code);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean MoveWindow(IntPtr hwnd, Int32 x, Int32 y, Int32 width, Int32 height, [MarshalAs(UnmanagedType.Bool)] Boolean repaint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean DestroyWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean UnregisterClass(IntPtr classAtom, IntPtr hInstance);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDest, ref POINT pptDest, ref Win32SIZE psize, IntPtr hdcSrc, ref POINT pptSrc, UInt32 crKey, [In] ref BLENDFUNCTION pblend, UInt32 dwFlags);

        [DllImport("user32.dll")]
        public static extern Boolean RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT pt);

        [DllImport("user32.dll")]
        public static extern IntPtr GetAncestor(IntPtr hWnd, Int32 flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean FillRect(IntPtr hDC, ref RECT rect, IntPtr hbrush);

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SetProp(IntPtr hwnd, String propName, IntPtr value);

        [DllImport("user32.dll")]
        public static extern IntPtr GetProp(IntPtr hwnd, String propName);

        [DllImport("user32.dll")]
        public static extern IntPtr RemoveProp(IntPtr hwnd, String propName);

        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hChild, IntPtr hNewParent);

        [DllImport("Ole32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 RevokeDragDrop(IntPtr hwnd);

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean PathIsNetworkPath(String path);

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public unsafe static extern Int32 ExtractIconEx(String szFileName, Int32 nIconIndex, IntPtr* phiconLarge, IntPtr* phiconSmall, Int32 nIcons);

        public unsafe static SafeIcon ExtractIcon(String filename, Int32 index, Boolean large) {
            IntPtr* ptr = stackalloc IntPtr[1];
            *ptr = IntPtr.Zero;
            if (large) {
                ExtractIconEx(filename, index, ptr, null, 1);
                }
            else {
                ExtractIconEx(filename, index, null, ptr, 1);
                }
            return new SafeIcon(*ptr, true);
            }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr SHGetFileInfo(String pszPath, UInt32 dwFileAttributes, ref SHFILEINFO psfi, UInt32 cbFileInfo, SHGFI uFlags);

        [DllImport("shell32.dll")]
        private static extern void SHAddToRecentDocs(SHARD uFlags, [MarshalAs(UnmanagedType.LPWStr)] String pv);

        internal static void SHAddToRecentDocs(String path) {
            SHAddToRecentDocs(SHARD.PATHW, path);
            }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        public static extern Boolean GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        [DllImport("comctl32.dll")]
        internal static extern IntPtr ImageList_Create(Int32 cx, Int32 cy, ILC flags, Int32 cInitial, Int32 cGrow);

        [DllImport("comctl32.dll")]
        internal static extern IntPtr ImageList_Destroy(IntPtr himl);

        [DllImport("comctl32.dll")]
        internal static extern Int32 ImageList_Add(IntPtr himl, IntPtr hbmImage, IntPtr hbmMask);

        [DllImport("comctl32.dll")]
        internal static extern Int32 ImageList_GetImageCount(IntPtr himl);

        [DllImport("comctl32.dll")]
        internal static extern IntPtr ImageList_GetIcon(IntPtr himl, Int32 i, UInt32 flags);

        [DllImport("comctl32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean ImageList_GetIconSize(IntPtr himl, out Int32 cx, out Int32 cy);

        [DllImport("comctl32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean ImageList_GetImageInfo(IntPtr himl, Int32 i, out IMAGEINFO pImageInfo);

        [DllImport("comctl32.dll")]
        internal static extern Int32 ImageList_GetBkColor(IntPtr himl);

        [DllImport("gdi32.dll")]
        internal static extern Int32 GetObject(IntPtr hGdiObj, Int32 cbBuffer, IntPtr lpvObject);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Int32 GetObject(IntPtr hGdiObj, Int32 cbBuffer, [In, Out] BITMAP bm);

        [DllImport("gdi32.dll")]
        internal static extern GdiObjectType GetObjectType(IntPtr hGdiObj);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean IntersectRect(out RECT lprcDst, [In] ref RECT lprcSrc1, [In] ref RECT lprcSrc2);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean PtInRect([In] ref RECT lprc, POINT pt);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO monitorInfo);

        [DllImport("user32.dll")]
        internal static extern IntPtr CreateIconFromResourceEx([In] Byte[] pbIconBits, Int32 cbIconBits, Boolean fIcon, Int32 dwVersion, Int32 cxDesired, Int32 cyDesired, Int32 flags);

        [DllImport("oleaut32.dll", CharSet = CharSet.Ansi)]
        internal static extern Int32 OleCreatePictureIndirect(ref PictDescBitmap pictdesc, ref Guid iid, Boolean fOwn, [MarshalAs(UnmanagedType.Interface)] out Object ppVoid);

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        internal static extern void CopyMemory(IntPtr Destination, IntPtr Source, UInt32 Length);

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, UInt32 iUsage, out IntPtr ppvBits, IntPtr hSection, UInt32 dwOffset);

        [DllImport("msimg32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean AlphaBlend(IntPtr hdcDest, Int32 xoriginDest, Int32 yoriginDest, Int32 wDest, Int32 hDest, IntPtr hdcSrc, Int32 xoriginSrc, Int32 yoriginSrc, Int32 wSrc, Int32 hSrc, BLENDFUNCTION pfn);

        private static WINDOWPOS LParamToWindowPos(IntPtr lParam) {
            var structure = new WINDOWPOS();
            Marshal.PtrToStructure(lParam, structure);
            return structure;
            }

        public static UInt32 GetWindowPosFlags(IntPtr lParam) {
            return LParamToWindowPos(lParam).flags;
            }

        public static Size GetWindowPosSize(IntPtr lParam) {
            var windowPos = LParamToWindowPos(lParam);
            return new Size(windowPos.cx, windowPos.cy);
            }

        public static Int32 BooleanToNativeBOOL(Boolean value) {
            return !value ? 0 : 1;
            }

        public static Int32 GET_SC_WPARAM(IntPtr wParam) {
            return (Int32)wParam & 65520;
            }

        [DllImport("user32.dll")]
        internal static extern Boolean IsDialogMessage(IntPtr hDlg, [In] ref MSG lpMsg);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetNextDlgTabItem(IntPtr hDlg, IntPtr hCtl, Boolean bPrevious);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetTopWindow(IntPtr parent);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll")]
        internal static extern Boolean SetThreadPriority(IntPtr hThread, ThreadPriority nPriority);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern UInt32 GetPrivateProfileString(String section, String key, String defaultValue, [Out] StringBuilder buffer, UInt32 bufferCharLength, String path);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Int32 GetPrivateProfileInt(String section, String key, Int32 defaultValue, String path);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern Int32 WritePrivateProfileString(String section, String key, String value, String path);

        [Flags]
        public enum LoadLibraryFlags : uint
            {
            DONT_RESOLVE_DLL_REFERENCES = 1,
            LOAD_LIBRARY_AS_DATAFILE = 2,
            LOAD_WITH_ALTERED_SEARCH_PATH = 8,
            LOAD_IGNORE_CODE_AUTHZ_LEVEL = 16,
            LOAD_LIBRARY_AS_IMAGE_RESOURCE = 32,
            LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE = 64,
            LOAD_LIBRARY_REQUIRE_SIGNED_TARGET = 128,
            }

        public delegate Boolean EnumResourceNameProc(IntPtr hModule, IntPtr type, IntPtr name, IntPtr lParam);

        public enum LoadImageType : uint
            {
            IMAGE_BITMAP,
            IMAGE_ICON,
            IMAGE_CURSOR,
            IMAGE_ENHMETAFILE,
            }

        [Flags]
        public enum LoadImageFlags : uint
            {
            LR_DEFAULTCOLOR = 0,
            LR_MONOCHROME = 1,
            LR_COLOR = 2,
            LR_COPYRETURNORG = 4,
            LR_COPYDELETEORG = 8,
            LR_LOADFROMFILE = 16,
            LR_LOADTRANSPARENT = 32,
            LR_DEFAULTSIZE = 64,
            LR_VGACOLOR = 128,
            LR_LOADMAP3DCOLORS = 4096,
            LR_CREATEDIBSECTION = 8192,
            LR_COPYFROMRESOURCE = 16384,
            LR_SHARED = 32768,
            }

        public delegate Boolean EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public delegate IntPtr SubclassProc(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam, UIntPtr id, IntPtr refData);

        public delegate IntPtr WindowsHookProc(CbtHookAction code, IntPtr wParam, IntPtr lParam);

        [Flags]
        public enum SyncObjectAccess
            {
            DELETE = 65536,
            READ_CONTROL = 131072,
            WRITE_DAC = 262144,
            WRITE_OWNER = 524288,
            SYNCHRONIZE = 1048576,
            EVENT_ALL_ACCESS = 2031619,
            EVENT_MODIFY_STATE = 2,
            MUTEX_ALL_ACCESS = 2031617,
            MUTEX_MODIFY_STATE = 1,
            SEMAPHORE_ALL_ACCESS = MUTEX_MODIFY_STATE | EVENT_MODIFY_STATE | SYNCHRONIZE | WRITE_OWNER | WRITE_DAC | READ_CONTROL | DELETE,
            SEMAPHORE_MODIFY_STATE = EVENT_MODIFY_STATE,
            TIMER_ALL_ACCESS = SEMAPHORE_MODIFY_STATE | MUTEX_MODIFY_STATE | SYNCHRONIZE | WRITE_OWNER | WRITE_DAC | READ_CONTROL | DELETE,
            TIMER_MODIFY_STATE = SEMAPHORE_MODIFY_STATE,
            TIMER_QUERY_STATE = MUTEX_MODIFY_STATE,
            }

        public enum WindowsHookType
            {
            WH_JOURNALRECORD,
            WH_JOURNALPLAYBACK,
            WH_KEYBOARD,
            WH_GETMESSAGE,
            WH_CALLWNDPROC,
            WH_CBT,
            WH_SYSMSGFILTER,
            WH_MOUSE,
            WH_HARDWARE,
            WH_DEBUG,
            WH_SHELL,
            WH_FOREGROUNDIDLE,
            WH_CALLWNDPROCRET,
            WH_KEYBOARD_LL,
            WH_MOUSE_LL,
            }

        public enum CbtHookAction
            {
            HCBT_MOVESIZE,
            HCBT_MINMAX,
            HCBT_QS,
            HCBT_CREATEWND,
            HCBT_DESTROYWND,
            HCBT_ACTIVATE,
            HCBT_CLICKSKIPPED,
            HCBT_KEYSKIPPED,
            HCBT_SYSCOMMAND,
            HCBT_SETFOCUS,
            }

        [Flags]
        public enum PrintFlags : uint
            {
            PRF_CHECKVISIBLE = 1,
            PRF_NONCLIENT = 2,
            PRF_CLIENT = 4,
            PRF_ERASEBKGND = 8,
            PRF_CHILDREN = 16,
            PRF_OWNED = 32,
            }

        public enum StockObjects
            {
            WHITE_BRUSH = 0,
            LTGRAY_BRUSH = 1,
            GRAY_BRUSH = 2,
            DKGRAY_BRUSH = 3,
            BLACK_BRUSH = 4,
            HOLLOW_BRUSH = 5,
            NULL_BRUSH = 5,
            WHITE_PEN = 6,
            BLACK_PEN = 7,
            NULL_PEN = 8,
            OEM_FIXED_FONT = 10,
            ANSI_FIXED_FONT = 11,
            ANSI_VAR_FONT = 12,
            SYSTEM_FONT = 13,
            DEVICE_DEFAULT_FONT = 14,
            DEFAULT_PALETTE = 15,
            SYSTEM_FIXED_FONT = 16,
            DEFAULT_GUI_FONT = 17,
            DC_BRUSH = 18,
            DC_PEN = 19,
            }

        public delegate IntPtr WndProc(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam);

        internal struct _FILE_STANDARD_INFO
            {
            private readonly Int64 AllocationSize;
            private Int64 EndOfFile;
            private UInt32 NumberOfLinks;
            private readonly Boolean DeletePending;
            private readonly Boolean Directory;
            }

        internal struct _FILE_COMPRESSION_INFORMATION
            {
            internal Int64 CompressedFileSize;
            internal UInt16 CompressionFormat;
            private readonly Byte CompressionUnitShift;
            private readonly Byte ChunkShift;
            private readonly Byte ClusterShift;
            internal Byte[] Reserved;
            }

        internal enum FILE_INFO_BY_HANDLE_CLASS
            {
            FileBasicInfo,
            FileStandardInfo,
            FileNameInfo,
            FileRenameInfo,
            FileDispositionInfo,
            FileAllocationInfo,
            FileEndOfFileInfo,
            FileStreamInfo,
            FileCompressionInfo,
            FileAttributeTagInfo,
            FileIdBothDirectoryInfo,
            FileIdBothDirectoryRestartInfo,
            FileIoPriorityHintInfo,
            FileRemoteProtocolInfo,
            FileFullDirectoryInfo,
            FileFullDirectoryRestartInfo,
            FileStorageInfo,
            FileAlignmentInfo,
            FileIdInfo,
            FileIdExtdDirectoryInfo,
            FileIdExtdDirectoryRestartInfo,
            MaximumFileInfoByHandlesClass,
            }

        internal enum _FILE_COMPRESSION_INFORMATION_COMPRESSION_FORMAT : ushort
            {
            COMPRESSION_FORMAT_NONE,
            COMPRESSION_FORMAT_DEFAULT,
            COMPRESSION_FORMAT_LZNT1,
            }

        [Flags]
        public enum FileSystemFeature : uint
            {
            CaseSensitiveSearch = 1,
            CasePreservedNames = 2,
            UnicodeOnDisk = 4,
            PersistentACLS = 8,
            FileCompression = 16,
            VolumeQuotas = 32,
            SupportsSparseFiles = 64,
            SupportsReparsePoints = 128,
            VolumeIsCompressed = 32768,
            SupportsObjectIDs = 65536,
            SupportsEncryption = 131072,
            NamedStreams = 262144,
            ReadOnlyVolume = 524288,
            SequentialWriteOnce = 1048576,
            SupportsTransactions = 2097152,
            }

        [Flags]
        public enum RedrawWindowFlags : uint
            {
            Invalidate = 1,
            InternalPaint = 2,
            Erase = 4,
            Validate = 8,
            NoInternalPaint = 16,
            NoErase = 32,
            NoChildren = 64,
            AllChildren = 128,
            UpdateNow = 256,
            EraseNow = 512,
            Frame = 1024,
            NoFrame = 2048,
            }

        private enum SHARD
            {
            PIDL = 1,
            PATHA = 2,
            PATHW = 3,
            APPIDINFO = 4,
            APPIDINFOIDLIST = 5,
            LINK = 6,
            APPIDINFOLINK = 7,
            }

        [StructLayout(LayoutKind.Sequential)]
        public class BITMAP
            {
            public IntPtr bmBits = IntPtr.Zero;
            public Int32 bmType;
            public Int32 bmWidth;
            public Int32 bmHeight;
            public Int32 bmWidthBytes;
            public Int16 bmPlanes;
            public Int16 bmBitsPixel;
            }

        public enum GdiObjectType
            {
            ERROR = 0,
            OBJ_PEN = 1,
            OBJ_BRUSH = 2,
            OBJ_DC = 3,
            OBJ_PAL = 5,
            OBJ_FONT = 6,
            OBJ_BITMAP = 7,
            OBJ_REGION = 8,
            OBJ_MEMDC = 10,
            OBJ_ENHMETADC = 12,
            OBJ_ENHMETAFILE = 13,
            }

        [return: MarshalAs(UnmanagedType.Bool)]
        internal delegate Boolean EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

        internal struct PictDescBitmap
            {
            private Int32 cbSizeOfStruct;
            private Int32 pictureType;
            private IntPtr hBitmap;
            private IntPtr hPalette;
            private readonly Int32 unused;

            internal static PictDescBitmap Default {
                get
                    {
                    return new PictDescBitmap
                    {
                        cbSizeOfStruct = 20,
                        pictureType = 1,
                        hBitmap = IntPtr.Zero,
                        hPalette = IntPtr.Zero
                        };
                    }
                }
            }

        internal struct ICONINFO
            {
            private readonly Boolean fIcon;
            private readonly Int32 xHotspot;
            private readonly Int32 yHotspot;
            private readonly IntPtr hbmMask;
            private readonly IntPtr hbmColor;
            }

        internal struct BITMAPINFO
            {
            internal Int32 biSize;
            internal Int32 biWidth;
            internal Int32 biHeight;
            internal Int16 biPlanes;
            internal Int16 biBitCount;
            internal Int32 biCompression;
            internal Int32 biSizeImage;
            internal Int32 biXPelsPerMeter;
            internal Int32 biYPelsPerMeter;
            internal Int32 biClrUsed;
            internal Int32 biClrImportant;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            internal Byte[] bmiColors;

            internal static BITMAPINFO Default {
                get
                    {
                    return new BITMAPINFO
                    {
                        biSize = 40,
                        biPlanes = 1
                        };
                    }
                }
            }

        internal struct BITMAPINFOHEADER
            {
            internal UInt32 biSize;
            internal Int32 biWidth;
            internal Int32 biHeight;
            internal UInt16 biPlanes;
            internal readonly UInt16 biBitCount;
            private UInt32 biCompression;
            internal readonly UInt32 biSizeImage;
            internal Int32 biXPelsPerMeter;
            private readonly Int32 biYPelsPerMeter;
            internal UInt32 biClrUsed;
            internal UInt32 biClrImportant;

            internal static BITMAPINFOHEADER Default {
                get
                    {
                    return new BITMAPINFOHEADER
                    {
                        biSize = 40,
                        biPlanes = 1
                        };
                    }
                }
            }

        [StructLayout(LayoutKind.Sequential)]
        internal class BITMAPFILEHEADER
            {
            public UInt16 bfType;
            public UInt32 bfSize;
            public UInt16 bfReserved1;
            public UInt16 bfReserved2;
            public UInt32 bfOffBits;

            internal static BITMAPFILEHEADER Default {
                get
                    {
                    return new BITMAPFILEHEADER
                    {
                        bfSize = 14
                        };
                    }
                }
            }

        internal struct BLENDFUNCTION
            {
            public Byte BlendOp;
            public Byte BlendFlags;
            public Byte SourceConstantAlpha;
            public Byte AlphaFormat;
            }

        public enum SizeStyle
            {
            SIZENORMAL = 0,
            SIZE_RESTORED = 0,
            SIZEICONIC = 1,
            SIZE_MINIMIZED = 1,
            SIZEFULLSCREEN = 2,
            SIZE_MAXIMIZED = 2,
            SIZEZOOMSHOW = 3,
            SIZE_MAXSHOW = 3,
            SIZEZOOMHIDE = 4,
            SIZE_MAXHIDE = 4,
            }

        public enum DialogResult
            {
            IDOK = 1,
            IDCANCEL = 2,
            IDABORT = 3,
            IDRETRY = 4,
            IDIGNORE = 5,
            IDYES = 6,
            IDNO = 7,
            IDCLOSE = 8,
            IDHELP = 9,
            IDTRYAGAIN = 10,
            IDCONTINUE = 11,
            }

        public enum GWL
            {
            EXSTYLE = -20,
            STYLE = -16,
            }

        public enum GWLP
            {
            USERDATA = -21,
            ID = -12,
            HWNDPARENT = -8,
            HINSTANCE = -6,
            WNDPROC = -4,
            }

        public enum CombineMode
            {
            RGN_AND = 1,
            RGN_MIN = 1,
            RGN_OR = 2,
            RGN_XOR = 3,
            RGN_DIFF = 4,
            RGN_COPY = 5,
            RGN_MAX = 5,
            }

        internal enum ThreadPriority
            {
            THREAD_PRIORITY_IDLE = -15,
            THREAD_PRIORITY_LOWEST = -2,
            THREAD_PRIORITY_BELOW_NORMAL = -1,
            THREAD_PRIORITY_NORMAL = 0,
            THREAD_PRIORITY_ABOVE_NORMAL = 1,
            THREAD_PRIORITY_HIGHEST = 2,
            THREAD_PRIORITY_TIME_CRITICAL = 15,
            THREAD_MODE_BACKGROUND_BEGIN = 65536,
            THREAD_MODE_BACKGROUND_END = 131072,
            }


        [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern Int32 GetWindowText(IntPtr window, StringBuilder classname, Int32 size);
        internal static String GetWindowText(IntPtr window) {
            var builder = new StringBuilder(2048);
            var sz = GetWindowText(window, builder, builder.Capacity);
            return builder.ToString();
            }
        }

    internal struct NativeResourceIdentifier
        {
        private readonly Int32 _integerId;
        private readonly String _stringId;

        public Object Id {
            get
                {
                if (IsStringId)
                    return _stringId;
                return _integerId;
                }
            }

        public Int32 IntegerId {
            get
                {
                if (!IsIntegerId)
                    throw new InvalidOperationException();
                return _integerId;
                }
            }

        public String StringId {
            get
                {
                if (!IsStringId)
                    throw new InvalidOperationException();
                return _stringId;
                }
            }

        public Boolean IsIntegerId {
            get
                {
                return !IsStringId;
                }
            }

        public Boolean IsStringId {
            get
                {
                return _stringId != null;
                }
            }

        public NativeResourceIdentifier(Int32 id) {
            _integerId = id;
            _stringId = null;
            }

        public NativeResourceIdentifier(String id) {
            ValidateStringId(id);
            _integerId = 0;
            _stringId = id;
            }

        public NativeResourceIdentifier(Object id) {
            if (id is Int32) {
                _integerId = (Int32)id;
                _stringId = null;
                }
            else {
                if ((_stringId = id as String) == null)
                    throw new ArgumentException("id must be a string or an integer", nameof(id));
                ValidateStringId(_stringId);
                _integerId = 0;
                }
            }

        public override String ToString() {
            if (!IsStringId)
                return _integerId.ToString();
            return _stringId;
            }

        private static void ValidateStringId(String id) {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (id.Length == 0)
                throw new ArgumentException("An identifer string cannot be empty", nameof(id));
            }
        }

    internal struct MONITORINFO
        {
        public UInt32 cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public UInt32 dwFlags;
        }

    [Serializable]
    internal struct RECT
        {
        public Int32 Left;
        public Int32 Top;
        public Int32 Right;
        public Int32 Bottom;

        public Point Position {
            get
                {
                return new Point(Left, Top);
                }
            }

        public Size Size {
            get
                {
                return new Size(Width, Height);
                }
            }

        public Int32 Height {
            get
                {
                return Bottom - Top;
                }
            set
                {
                Bottom = Top + value;
                }
            }

        public Int32 Width {
            get
                {
                return Right - Left;
                }
            set
                {
                Right = Left + value;
                }
            }

        public RECT(Int32 left, Int32 top, Int32 right, Int32 bottom) {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
            }

        public RECT(Rect rect) {
            Left = (Int32)rect.Left;
            Top = (Int32)rect.Top;
            Right = (Int32)rect.Right;
            Bottom = (Int32)rect.Bottom;
            }

        public void Offset(Int32 dx, Int32 dy) {
            Left = Left + dx;
            Right = Right + dx;
            Top = Top + dy;
            Bottom = Bottom + dy;
            }

        public Int32Rect ToInt32Rect() {
            return new Int32Rect(Left, Top, Width, Height);
            }
        }


    [StructLayout(LayoutKind.Sequential)]
    internal struct WINDOWPOS
        {
        public IntPtr hwnd;
        public IntPtr hwndInsertAfter;
        public Int32 x;
        public Int32 y;
        public Int32 cx;
        public Int32 cy;
        public UInt32 flags;
        }

    internal struct IMAGEINFO
        {
        public IntPtr hbmImage;
        public IntPtr hbmMask;
        public Int32 Unused1;
        public Int32 Unused2;
        public RECT rcImage;
        }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WNDCLASS
        {
        public UInt32 style;
        public Delegate lpfnWndProc;
        public Int32 cbClsExtra;
        public Int32 cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String lpszClassName;
        }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct CREATESTRUCT
        {
        public IntPtr CreateParams;
        public IntPtr Instance;
        public IntPtr Menu;
        public IntPtr ParentWindow;
        public Int32 Left;
        public Int32 Top;
        public Int32 Width;
        public Int32 Height;
        public Int32 Styles;
        [MarshalAs(UnmanagedType.LPWStr)] public String Name;
        [MarshalAs(UnmanagedType.LPWStr)] public String Class;
        public UInt32 ExtendedStyles;
        }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct WNDCLASSEX
        {
        public UInt32 cbSize;
        public UInt32 style;
        public Delegate lpfnWndProc;
        public Int32 cbClsExtra;
        public Int32 cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String lpszMenuName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String lpszClassName;
        public IntPtr hIconSm;
        }

    internal struct ACCEL
        {
        private readonly Byte fVirt;
        private readonly UInt16 key;
        private readonly UInt16 cmd;
        }

    internal struct WINDOWINFO
        {
        public Int32 cbSize;
        public RECT rcWindow;
        public RECT rcClient;
        public Int32 dwStyle;
        public Int32 dwExStyle;
        public UInt32 dwWindowStatus;
        public UInt32 cxWindowBorders;
        public UInt32 cyWindowBorders;
        public UInt16 atomWindowType;
        public UInt16 wCreatorVersion;
        }

    internal struct POINT
        {
        public Int32 x;
        public Int32 y;
        }

    [StructLayout(LayoutKind.Sequential)]
    internal class WINDOWPLACEMENT
        {
        public Int32 length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
        public Int32 flags;
        public Int32 showCmd;
        public POINT ptMinPosition;
        public POINT ptMaxPosition;
        public RECT rcNormalPosition;
        }


    internal enum VARTYPE : ushort
        {
        VT_EMPTY = 0,
        VT_NULL = 1,
        VT_I2 = 2,
        VT_I4 = 3,
        VT_R4 = 4,
        VT_R8 = 5,
        VT_CY = 6,
        VT_DATE = 7,
        VT_BSTR = 8,
        VT_DISPATCH = 9,
        VT_ERROR = 10,
        VT_BOOL = 11,
        VT_VARIANT = 12,
        VT_UNKNOWN = 13,
        VT_DECIMAL = 14,
        VT_I1 = 16,
        VT_UI1 = 17,
        VT_UI2 = 18,
        VT_UI4 = 19,
        VT_I8 = 20,
        VT_UI8 = 21,
        VT_INT = 22,
        VT_UINT = 23,
        VT_VOID = 24,
        VT_HRESULT = 25,
        VT_PTR = 26,
        VT_SAFEARRAY = 27,
        VT_CARRAY = 28,
        VT_USERDEFINED = 29,
        VT_LPSTR = 30,
        VT_LPWSTR = 31,
        VT_RECORD = 36,
        VT_INT_PTR = 37,
        VT_UINT_PTR = 38,
        VT_FILETIME = 64,
        VT_BLOB = 65,
        VT_STREAM = 66,
        VT_STORAGE = 67,
        VT_STREAMED_OBJECT = 68,
        VT_STORED_OBJECT = 69,
        VT_BLOB_OBJECT = 70,
        VT_CF = 71,
        VT_CLSID = 72,
        VT_VERSIONED_STREAM = 73,
        VT_BSTR_BLOB = 4095,
        VT_ILLEGALMASKED = 4095,
        VT_TYPEMASK = 4095,
        VT_VECTOR = 4096,
        VT_ARRAY = 8192,
        VT_BYREF = 16384,
        VT_RESERVED = 32768,
        VT_ILLEGAL = 65535,
        }
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    internal struct VARIANT
        {
        [FieldOffset(0)]
        public VARTYPE vt;
        [FieldOffset(8)]
        public IntPtr pdispVal;
        [FieldOffset(8)]
        public Byte ui1;
        [FieldOffset(8)]
        public UInt16 ui2;
        [FieldOffset(8)]
        public UInt32 ui4;
        [FieldOffset(8)]
        public UInt64 ui8;
        [FieldOffset(8)]
        public Single r4;
        [FieldOffset(8)]
        public Double r8;
        }

    internal enum VARIANTFLAGS : ushort
        {
        NONE = 0,
        VARIANT_NOVALUEPROP = 1,
        VARIANT_ALPHABOOL = 2,
        VARIANT_NOUSEROVERRIDE = 4,
        VARIANT_LOCALBOOL = 16,
        }

    [Flags]
    internal enum ILC : uint
        {
        ILC_MASK = 1,
        ILC_COLOR = 0,
        ILC_COLORDDB = 254,
        ILC_COLOR4 = 4,
        ILC_COLOR8 = 8,
        ILC_COLOR16 = 16,
        ILC_COLOR24 = ILC_COLOR16 | ILC_COLOR8,
        ILC_COLOR32 = 32,
        ILC_PALETTE = 2048,
        ILC_MIRROR = 8192,
        ILC_PERITEMMIRROR = 32768,
        ILC_ORIGINALSIZE = 65536,
        ILC_HIGHQUALITYSCALE = 131072,
        }

    internal struct Win32SIZE
        {
        public Int32 cx;
        public Int32 cy;
        }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct SHFILEINFO
        {
        public IntPtr hIcon;
        public Int32 iIcon;
        public UInt32 dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public String szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public String szTypeName;
        }

    [Flags]
    internal enum SHGFI : uint
        {
        Icon = 256,
        DisplayName = 512,
        TypeName = 1024,
        Attributes = 2048,
        IconLocation = 4096,
        ExeType = 8192,
        SysIconIndex = 16384,
        LinkOverlay = 32768,
        Selected = 65536,
        Attr_Specified = 131072,
        LargeIcon = 0,
        SmallIcon = 1,
        OpenIcon = 2,
        ShellIconSize = 4,
        PIDL = 8,
        UseFileAttributes = 16,
        AddOverlays = 32,
        OverlayIndex = 64,
        }
    }