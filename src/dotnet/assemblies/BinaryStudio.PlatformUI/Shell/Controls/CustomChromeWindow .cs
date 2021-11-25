using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.DataProcessing;
using Microsoft.Win32;

namespace BinaryStudio.PlatformUI.Shell.Controls {
    public class CustomChromeWindow : Window
        {
        private readonly GlowWindow[] _glowWindows = new GlowWindow[4];
        private Rect logicalSizeForRestore = Rect.Empty;
        private const Int32 MinimizeAnimationDurationMilliseconds = 200;
        private Int32 lastWindowPlacement;
        private Int32 _deferGlowChangesCount;
        private Boolean _isGlowVisible;
        private DispatcherTimer _makeGlowVisibleTimer;
        private Boolean _isNonClientStripVisible;
        private IntPtr OwnerForActivate;
        private Boolean useLogicalSizeForRestore;
        private Boolean updatingZOrder;

        #region P:ActiveGlowColor:Color
        public static readonly DependencyProperty ActiveGlowColorProperty = DependencyProperty.Register("ActiveGlowColor", typeof(Color), typeof(CustomChromeWindow), new PropertyMetadata(Colors.Transparent, OnGlowColorChanged));
        public Color ActiveGlowColor {
            get { return (Color)GetValue(ActiveGlowColorProperty); }
            set { SetValue(ActiveGlowColorProperty, value); }
            }
        private static void OnGlowColorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ((CustomChromeWindow)sender).UpdateGlowColors();
            }
        #endregion
        #region P:InactiveGlowColor:Color
        public static readonly DependencyProperty InactiveGlowColorProperty = DependencyProperty.Register("InactiveGlowColor", typeof(Color), typeof(CustomChromeWindow), new PropertyMetadata(Colors.Transparent,OnGlowColorChanged));
        public Color InactiveGlowColor {
            get { return (Color)GetValue(InactiveGlowColorProperty); }
            set { SetValue(InactiveGlowColorProperty, value); }
            }
        #endregion
        #region P:NonClientFillColor:Color
        public static readonly DependencyProperty NonClientFillColorProperty = DependencyProperty.Register("NonClientFillColor", typeof(Color), typeof(CustomChromeWindow), new PropertyMetadata(SystemColors.InactiveBorderColor));
        public Color NonClientFillColor {
            get { return (Color)GetValue(NonClientFillColorProperty); }
            set { SetValue(NonClientFillColorProperty, value); }
            }
        #endregion
        #region P:CornerRadius:Int32
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(Int32), typeof(CustomChromeWindow), new PropertyMetadata(default(Int32), OnCornerRadiusChanged));
        [DefaultValue(0)]
        public Int32 CornerRadius {
            get { return (Int32)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
            }
        private static void OnCornerRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ((CustomChromeWindow)sender).UpdateClipRegion(ClipRegionChangeType.FromPropertyChange);
            }
        #endregion

        private static Int32 PressedMouseButtons {
            get {
                var num = 0;
                if (NativeMethods.IsKeyPressed(1))
                    num |= 1;
                if (NativeMethods.IsKeyPressed(2))
                    num |= 2;
                if (NativeMethods.IsKeyPressed(4))
                    num |= 16;
                if (NativeMethods.IsKeyPressed(5))
                    num |= 32;
                if (NativeMethods.IsKeyPressed(6))
                    num |= 64;
                return num;
                }
            }

        private Boolean IsGlowVisible {
            get {
                return _isGlowVisible;
                }
            set {
                if (_isGlowVisible == value) { return; }
                _isGlowVisible = value;
                for (var direction = 0; direction < _glowWindows.Length; ++direction)
                    GetOrCreateGlowWindow(direction).IsVisible = value;
                }
            }

        private IEnumerable<GlowWindow> LoadedGlowWindows {
            get {
                return _glowWindows.Where(w => w != null);
                }
            }

        protected virtual Boolean ShouldShowGlow {
            get {
                var handle = new WindowInteropHelper(this).Handle;
                if (NativeMethods.IsWindowVisible(handle) && !NativeMethods.IsIconic(handle) && !NativeMethods.IsZoomed(handle))
                    return (UInt32)ResizeMode > 0U;
                return false;
                }
            }

        static CustomChromeWindow()
            {
            ResizeModeProperty.OverrideMetadata(typeof(CustomChromeWindow), new FrameworkPropertyMetadata(OnResizeModeChanged));
            }

        public CustomChromeWindow()
            {
            CommandManager.AddCanExecuteHandler(this, OnCanExecuteCommand);
            CommandManager.AddExecutedHandler(this, OnExecutedCommand);
            }

        #region M:OnActivated(EventArgs)
        protected override void OnActivated(EventArgs e) {
            UpdateGlowActiveState();
            base.OnActivated(e);
            }
        #endregion
        #region M:OnDeactivated(EventArgs)
        protected override void OnDeactivated(EventArgs e) {
            UpdateGlowActiveState();
            base.OnDeactivated(e);
            }
        #endregion
        #region M:OnResizeModeChanged(DependencyObject,DependencyPropertyChangedEventArgs)
        private static void OnResizeModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            ((CustomChromeWindow)sender).UpdateGlowVisibility(false);
            }
        #endregion

        /// <summary>Raises the <see cref="E:System.Windows.Window.SourceInitialized" /> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnSourceInitialized(EventArgs e) {
            HwndSource.FromHwnd(new WindowInteropHelper(this).Handle).AddHook(HwndSourceHook);
            CreateGlowWindowHandles();
            base.OnSourceInitialized(e);
            }

        private void CreateGlowWindowHandles() {
            for (var direction = 0; direction < _glowWindows.Length; ++direction)
                GetOrCreateGlowWindow(direction).EnsureHandle();
            }


        protected const Int32 WM_NULL                             = 0x00000000;
        protected const Int32 WM_CREATE                           = 0x00000001;
        protected const Int32 WM_DESTROY                          = 0x00000002;
        protected const Int32 WM_MOVE                             = 0x00000003;
        protected const Int32 WM_SIZE                             = 0x00000005;
        protected const Int32 WM_ACTIVATE                         = 0x00000006;
        protected const Int32 WM_SETFOCUS                         = 0x00000007;
        protected const Int32 WM_KILLFOCUS                        = 0x00000008;
        protected const Int32 WM_ENABLE                           = 0x0000000A;
        protected const Int32 WM_SETREDRAW                        = 0x0000000B;
        protected const Int32 WM_SETTEXT                          = 0x0000000C;
        protected const Int32 WM_GETTEXT                          = 0x0000000D;
        protected const Int32 WM_GETTEXTLENGTH                    = 0x0000000E;
        protected const Int32 WM_PAINT                            = 0x0000000F;
        protected const Int32 WM_CLOSE                            = 0x00000010;
        protected const Int32 WM_QUERYENDSESSION                  = 0x00000011;
        protected const Int32 WM_QUERYOPEN                        = 0x00000013;
        protected const Int32 WM_ENDSESSION                       = 0x00000016;
        protected const Int32 WM_QUIT                             = 0x00000012;
        protected const Int32 WM_ERASEBKGND                       = 0x00000014;
        protected const Int32 WM_SYSCOLORCHANGE                   = 0x00000015;
        protected const Int32 WM_SHOWWINDOW                       = 0x00000018;
        protected const Int32 WM_WININICHANGE                     = 0x0000001A;
        protected const Int32 WM_SETTINGCHANGE                    = WM_WININICHANGE;
        protected const Int32 WM_DEVMODECHANGE                    = 0x0000001B;
        protected const Int32 WM_ACTIVATEAPP                      = 0x0000001C;
        protected const Int32 WM_FONTCHANGE                       = 0x0000001D;
        protected const Int32 WM_TIMECHANGE                       = 0x0000001E;
        protected const Int32 WM_CANCELMODE                       = 0x0000001F;
        protected const Int32 WM_SETCURSOR                        = 0x00000020;
        protected const Int32 WM_MOUSEACTIVATE                    = 0x00000021;
        protected const Int32 WM_CHILDACTIVATE                    = 0x00000022;
        protected const Int32 WM_QUEUESYNC                        = 0x00000023;
        protected const Int32 WM_GETMINMAXINFO                    = 0x00000024;
        protected const Int32 WM_PAINTICON                        = 0x00000026;
        protected const Int32 WM_ICONERASEBKGND                   = 0x00000027;
        protected const Int32 WM_NEXTDLGCTL                       = 0x00000028;
        protected const Int32 WM_SPOOLERSTATUS                    = 0x0000002A;
        protected const Int32 WM_DRAWITEM                         = 0x0000002B;
        protected const Int32 WM_MEASUREITEM                      = 0x0000002C;
        protected const Int32 WM_DELETEITEM                       = 0x0000002D;
        protected const Int32 WM_VKEYTOITEM                       = 0x0000002E;
        protected const Int32 WM_CHARTOITEM                       = 0x0000002F;
        protected const Int32 WM_SETFONT                          = 0x00000030;
        protected const Int32 WM_GETFONT                          = 0x00000031;
        protected const Int32 WM_SETHOTKEY                        = 0x00000032;
        protected const Int32 WM_GETHOTKEY                        = 0x00000033;
        protected const Int32 WM_QUERYDRAGICON                    = 0x00000037;
        protected const Int32 WM_COMPAREITEM                      = 0x00000039;
        protected const Int32 WM_GETOBJECT                        = 0x0000003D;
        protected const Int32 WM_COMPACTING                       = 0x00000041;
        protected const Int32 WM_COMMNOTIFY                       = 0x00000044;  /* no longer suported */
        protected const Int32 WM_WINDOWPOSCHANGING                = 0x00000046;
        protected const Int32 WM_WINDOWPOSCHANGED                 = 0x00000047;
        protected const Int32 WM_POWER                            = 0x00000048;
        protected const Int32 WM_COPYDATA                         = 0x0000004A;
        protected const Int32 WM_CANCELJOURNAL                    = 0x0000004B;
        protected const Int32 WM_NOTIFY                           = 0x0000004E;
        protected const Int32 WM_INPUTLANGCHANGEREQUEST           = 0x00000050;
        protected const Int32 WM_INPUTLANGCHANGE                  = 0x00000051;
        protected const Int32 WM_TCARD                            = 0x00000052;
        protected const Int32 WM_HELP                             = 0x00000053;
        protected const Int32 WM_USERCHANGED                      = 0x00000054;
        protected const Int32 WM_NOTIFYFORMAT                     = 0x00000055;
        protected const Int32 WM_CONTEXTMENU                      = 0x0000007B;
        protected const Int32 WM_STYLECHANGING                    = 0x0000007C;
        protected const Int32 WM_STYLECHANGED                     = 0x0000007D;
        protected const Int32 WM_DISPLAYCHANGE                    = 0x0000007E;
        protected const Int32 WM_GETICON                          = 0x0000007F;
        protected const Int32 WM_SETICON                          = 0x00000080;
        protected const Int32 WM_NCCREATE                         = 0x00000081;
        protected const Int32 WM_NCDESTROY                        = 0x00000082;
        protected const Int32 WM_NCCALCSIZE                       = 0x00000083;
        protected const Int32 WM_NCHITTEST                        = 0x00000084;
        protected const Int32 WM_NCPAINT                          = 0x00000085;
        protected const Int32 WM_NCACTIVATE                       = 0x00000086;
        protected const Int32 WM_GETDLGCODE                       = 0x00000087;
        protected const Int32 WM_SYNCPAINT                        = 0x00000088;
        protected const Int32 WM_NCMOUSEMOVE                      = 0x000000A0;
        protected const Int32 WM_NCLBUTTONDOWN                    = 0x000000A1;
        protected const Int32 WM_NCLBUTTONUP                      = 0x000000A2;
        protected const Int32 WM_NCLBUTTONDBLCLK                  = 0x000000A3;
        protected const Int32 WM_NCRBUTTONDOWN                    = 0x000000A4;
        protected const Int32 WM_NCRBUTTONUP                      = 0x000000A5;
        protected const Int32 WM_NCRBUTTONDBLCLK                  = 0x000000A6;
        protected const Int32 WM_NCMBUTTONDOWN                    = 0x000000A7;
        protected const Int32 WM_NCMBUTTONUP                      = 0x000000A8;
        protected const Int32 WM_NCMBUTTONDBLCLK                  = 0x000000A9;
        protected const Int32 WM_NCXBUTTONDOWN                    = 0x000000AB;
        protected const Int32 WM_NCXBUTTONUP                      = 0x000000AC;
        protected const Int32 WM_NCXBUTTONDBLCLK                  = 0x000000AD;
        protected const Int32 WM_INPUT_DEVICE_CHANGE              = 0x000000FE;
        protected const Int32 WM_INPUT                            = 0x000000FF;
        protected const Int32 WM_KEYFIRST                         = 0x00000100;
        protected const Int32 WM_KEYDOWN                          = 0x00000100;
        protected const Int32 WM_KEYUP                            = 0x00000101;
        protected const Int32 WM_CHAR                             = 0x00000102;
        protected const Int32 WM_DEADCHAR                         = 0x00000103;
        protected const Int32 WM_SYSKEYDOWN                       = 0x00000104;
        protected const Int32 WM_SYSKEYUP                         = 0x00000105;
        protected const Int32 WM_SYSCHAR                          = 0x00000106;
        protected const Int32 WM_SYSDEADCHAR                      = 0x00000107;
        protected const Int32 WM_UNICHAR                          = 0x00000109;
        protected const Int32 WM_KEYLAST                          = 0x00000109;
        protected const Int32 WM_IME_STARTCOMPOSITION             = 0x0000010D;
        protected const Int32 WM_IME_ENDCOMPOSITION               = 0x0000010E;
        protected const Int32 WM_IME_COMPOSITION                  = 0x0000010F;
        protected const Int32 WM_IME_KEYLAST                      = 0x0000010F;
        protected const Int32 WM_INITDIALOG                       = 0x00000110;
        protected const Int32 WM_COMMAND                          = 0x00000111;
        protected const Int32 WM_SYSCOMMAND                       = 0x00000112;
        protected const Int32 WM_TIMER                            = 0x00000113;
        protected const Int32 WM_HSCROLL                          = 0x00000114;
        protected const Int32 WM_VSCROLL                          = 0x00000115;
        protected const Int32 WM_INITMENU                         = 0x00000116;
        protected const Int32 WM_INITMENUPOPUP                    = 0x00000117;
        protected const Int32 WM_GESTURE                          = 0x00000119;
        protected const Int32 WM_GESTURENOTIFY                    = 0x0000011A;
        protected const Int32 WM_MENUSELECT                       = 0x0000011F;
        protected const Int32 WM_MENUCHAR                         = 0x00000120;
        protected const Int32 WM_ENTERIDLE                        = 0x00000121;
        protected const Int32 WM_MENURBUTTONUP                    = 0x00000122;
        protected const Int32 WM_MENUDRAG                         = 0x00000123;
        protected const Int32 WM_MENUGETOBJECT                    = 0x00000124;
        protected const Int32 WM_UNINITMENUPOPUP                  = 0x00000125;
        protected const Int32 WM_MENUCOMMAND                      = 0x00000126;
        protected const Int32 WM_CHANGEUISTATE                    = 0x00000127;
        protected const Int32 WM_UPDATEUISTATE                    = 0x00000128;
        protected const Int32 WM_QUERYUISTATE                     = 0x00000129;
        protected const Int32 WM_CTLCOLORMSGBOX                   = 0x00000132;
        protected const Int32 WM_CTLCOLOREDIT                     = 0x00000133;
        protected const Int32 WM_CTLCOLORLISTBOX                  = 0x00000134;
        protected const Int32 WM_CTLCOLORBTN                      = 0x00000135;
        protected const Int32 WM_CTLCOLORDLG                      = 0x00000136;
        protected const Int32 WM_CTLCOLORSCROLLBAR                = 0x00000137;
        protected const Int32 WM_CTLCOLORSTATIC                   = 0x00000138;
        protected const Int32 MN_GETHMENU                         = 0x000001E1;
        protected const Int32 WM_MOUSEFIRST                       = 0x00000200;
        protected const Int32 WM_MOUSEMOVE                        = 0x00000200;
        protected const Int32 WM_LBUTTONDOWN                      = 0x00000201;
        protected const Int32 WM_LBUTTONUP                        = 0x00000202;
        protected const Int32 WM_LBUTTONDBLCLK                    = 0x00000203;
        protected const Int32 WM_RBUTTONDOWN                      = 0x00000204;
        protected const Int32 WM_RBUTTONUP                        = 0x00000205;
        protected const Int32 WM_RBUTTONDBLCLK                    = 0x00000206;
        protected const Int32 WM_MBUTTONDOWN                      = 0x00000207;
        protected const Int32 WM_MBUTTONUP                        = 0x00000208;
        protected const Int32 WM_MBUTTONDBLCLK                    = 0x00000209;
        protected const Int32 WM_MOUSEWHEEL                       = 0x0000020A;
        protected const Int32 WM_XBUTTONDOWN                      = 0x0000020B;
        protected const Int32 WM_XBUTTONUP                        = 0x0000020C;
        protected const Int32 WM_XBUTTONDBLCLK                    = 0x0000020D;
        protected const Int32 WM_MOUSEHWHEEL                      = 0x0000020E;
        protected const Int32 WM_MOUSELAST                        = 0x0000020E;
        protected const Int32 WM_PARENTNOTIFY                     = 0x00000210;
        protected const Int32 WM_ENTERMENULOOP                    = 0x00000211;
        protected const Int32 WM_EXITMENULOOP                     = 0x00000212;
        protected const Int32 WM_NEXTMENU                         = 0x00000213;
        protected const Int32 WM_SIZING                           = 0x00000214;
        protected const Int32 WM_CAPTURECHANGED                   = 0x00000215;
        protected const Int32 WM_MOVING                           = 0x00000216;
        protected const Int32 WM_POWERBROADCAST                   = 0x00000218;
        protected const Int32 WM_DEVICECHANGE                     = 0x00000219;
        protected const Int32 WM_MDICREATE                        = 0x00000220;
        protected const Int32 WM_MDIDESTROY                       = 0x00000221;
        protected const Int32 WM_MDIACTIVATE                      = 0x00000222;
        protected const Int32 WM_MDIRESTORE                       = 0x00000223;
        protected const Int32 WM_MDINEXT                          = 0x00000224;
        protected const Int32 WM_MDIMAXIMIZE                      = 0x00000225;
        protected const Int32 WM_MDITILE                          = 0x00000226;
        protected const Int32 WM_MDICASCADE                       = 0x00000227;
        protected const Int32 WM_MDIICONARRANGE                   = 0x00000228;
        protected const Int32 WM_MDIGETACTIVE                     = 0x00000229;
        protected const Int32 WM_MDISETMENU                       = 0x00000230;
        protected const Int32 WM_ENTERSIZEMOVE                    = 0x00000231;
        protected const Int32 WM_EXITSIZEMOVE                     = 0x00000232;
        protected const Int32 WM_DROPFILES                        = 0x00000233;
        protected const Int32 WM_MDIREFRESHMENU                   = 0x00000234;
        protected const Int32 WM_POINTERDEVICECHANGE              = 0x00000238;
        protected const Int32 WM_POINTERDEVICEINRANGE             = 0x00000239;
        protected const Int32 WM_POINTERDEVICEOUTOFRANGE          = 0x0000023A;
        protected const Int32 WM_TOUCH                            = 0x00000240;
        protected const Int32 WM_NCPOINTERUPDATE                  = 0x00000241;
        protected const Int32 WM_NCPOINTERDOWN                    = 0x00000242;
        protected const Int32 WM_NCPOINTERUP                      = 0x00000243;
        protected const Int32 WM_POINTERUPDATE                    = 0x00000245;
        protected const Int32 WM_POINTERDOWN                      = 0x00000246;
        protected const Int32 WM_POINTERUP                        = 0x00000247;
        protected const Int32 WM_POINTERENTER                     = 0x00000249;
        protected const Int32 WM_POINTERLEAVE                     = 0x0000024A;
        protected const Int32 WM_POINTERACTIVATE                  = 0x0000024B;
        protected const Int32 WM_POINTERCAPTURECHANGED            = 0x0000024C;
        protected const Int32 WM_TOUCHHITTESTING                  = 0x0000024D;
        protected const Int32 WM_POINTERWHEEL                     = 0x0000024E;
        protected const Int32 WM_POINTERHWHEEL                    = 0x0000024F;
        protected const Int32 DM_POINTERHITTEST                   = 0x00000250;
        protected const Int32 WM_POINTERROUTEDTO                  = 0x00000251;
        protected const Int32 WM_POINTERROUTEDAWAY                = 0x00000252;
        protected const Int32 WM_POINTERROUTEDRELEASED            = 0x00000253;
        protected const Int32 WM_IME_SETCONTEXT                   = 0x00000281;
        protected const Int32 WM_IME_NOTIFY                       = 0x00000282;
        protected const Int32 WM_IME_CONTROL                      = 0x00000283;
        protected const Int32 WM_IME_COMPOSITIONFULL              = 0x00000284;
        protected const Int32 WM_IME_SELECT                       = 0x00000285;
        protected const Int32 WM_IME_CHAR                         = 0x00000286;
        protected const Int32 WM_IME_REQUEST                      = 0x00000288;
        protected const Int32 WM_IME_KEYDOWN                      = 0x00000290;
        protected const Int32 WM_IME_KEYUP                        = 0x00000291;
        protected const Int32 WM_MOUSEHOVER                       = 0x000002A1;
        protected const Int32 WM_MOUSELEAVE                       = 0x000002A3;
        protected const Int32 WM_NCMOUSEHOVER                     = 0x000002A0;
        protected const Int32 WM_NCMOUSELEAVE                     = 0x000002A2;
        protected const Int32 WM_WTSSESSION_CHANGE                = 0x000002B1;
        protected const Int32 WM_TABLET_FIRST                     = 0x000002c0;
        protected const Int32 WM_TABLET_LAST                      = 0x000002df;
        protected const Int32 WM_DPICHANGED                       = 0x000002E0;
        protected const Int32 WM_DPICHANGED_BEFOREPARENT          = 0x000002E2;
        protected const Int32 WM_DPICHANGED_AFTERPARENT           = 0x000002E3;
        protected const Int32 WM_GETDPISCALEDSIZE                 = 0x000002E4;
        protected const Int32 WM_CUT                              = 0x00000300;
        protected const Int32 WM_COPY                             = 0x00000301;
        protected const Int32 WM_PASTE                            = 0x00000302;
        protected const Int32 WM_CLEAR                            = 0x00000303;
        protected const Int32 WM_UNDO                             = 0x00000304;
        protected const Int32 WM_RENDERFORMAT                     = 0x00000305;
        protected const Int32 WM_RENDERALLFORMATS                 = 0x00000306;
        protected const Int32 WM_DESTROYCLIPBOARD                 = 0x00000307;
        protected const Int32 WM_DRAWCLIPBOARD                    = 0x00000308;
        protected const Int32 WM_PAINTCLIPBOARD                   = 0x00000309;
        protected const Int32 WM_VSCROLLCLIPBOARD                 = 0x0000030A;
        protected const Int32 WM_SIZECLIPBOARD                    = 0x0000030B;
        protected const Int32 WM_ASKCBFORMATNAME                  = 0x0000030C;
        protected const Int32 WM_CHANGECBCHAIN                    = 0x0000030D;
        protected const Int32 WM_HSCROLLCLIPBOARD                 = 0x0000030E;
        protected const Int32 WM_QUERYNEWPALETTE                  = 0x0000030F;
        protected const Int32 WM_PALETTEISCHANGING                = 0x00000310;
        protected const Int32 WM_PALETTECHANGED                   = 0x00000311;
        protected const Int32 WM_HOTKEY                           = 0x00000312;
        protected const Int32 WM_PRINT                            = 0x00000317;
        protected const Int32 WM_PRINTCLIENT                      = 0x00000318;
        protected const Int32 WM_APPCOMMAND                       = 0x00000319;
        protected const Int32 WM_THEMECHANGED                     = 0x0000031A;
        protected const Int32 WM_CLIPBOARDUPDATE                  = 0x0000031D;
        protected const Int32 WM_DWMCOMPOSITIONCHANGED            = 0x0000031E;
        protected const Int32 WM_DWMNCRENDERINGCHANGED            = 0x0000031F;
        protected const Int32 WM_DWMCOLORIZATIONCOLORCHANGED      = 0x00000320;
        protected const Int32 WM_DWMWINDOWMAXIMIZEDCHANGE         = 0x00000321;
        protected const Int32 WM_DWMSENDICONICTHUMBNAIL           = 0x00000323;
        protected const Int32 WM_DWMSENDICONICLIVEPREVIEWBITMAP   = 0x00000326;
        protected const Int32 WM_GETTITLEBARINFOEX                = 0x0000033F;
        protected const Int32 WM_HANDHELDFIRST                    = 0x00000358;
        protected const Int32 WM_HANDHELDLAST                     = 0x0000035F;
        protected const Int32 WM_AFXFIRST                         = 0x00000360;
        protected const Int32 WM_AFXLAST                          = 0x0000037F;
        protected const Int32 WM_PENWINFIRST                      = 0x00000380;
        protected const Int32 WM_PENWINLAST                       = 0x0000038F;
        protected const Int32 WM_APP                              = 0x00008000;
        protected const Int32 WM_USER                             = 0x00000400;
        protected const Int32 WM_NCUAHDRAWCAPTION                 = 0x000000AE;
        protected const Int32 WM_NCUAHDRAWFRAME                   = 0x000000AF;

        protected virtual unsafe IntPtr HwndSourceHook(IntPtr h, Int32 m, IntPtr w, IntPtr l, ref Boolean r) {
            try
            {
            switch (m) {
                case WM_NCUAHDRAWCAPTION:
                case WM_NCUAHDRAWFRAME:
                    r = true;
                    break;
                case WM_SYSCOMMAND:
                    WmSysCommand(h, w, l);
                    break;
                case WM_SETICON:
                case WM_SETTEXT:
                    return CallDefWindowProcWithoutVisibleStyle(h, m, w, l, ref r);
                case WM_NCCALCSIZE:
                    return WmNcCalcSize(h, w, l, ref r);
                case WM_NCHITTEST:
                    return WmNcHitTest(h, l, ref r);
                case WM_NCPAINT:
                    return WmNcPaint(h, w, l, ref r);
                case WM_NCACTIVATE:
                    return WmNcActivate(h, w, l, ref r);
                case WM_NCRBUTTONDOWN:
                case WM_NCRBUTTONUP:
                case WM_NCRBUTTONDBLCLK:
                    RaiseNonClientMouseMessageAsClient(h, m, w, l);
                    r = true;
                    break;
                case WM_WINDOWPOSCHANGING:
                    WmWindowPosChanging(h, l);
                    break;
                case WM_WINDOWPOSCHANGED:
                    OnWindowPosChanged(h, l);
                    break;
                case WM_ACTIVATE: { OnActivate(w, l); } break;
                }
            
            return IntPtr.Zero;
            }
            finally
            {
               // Debug.Print("CustomChromeWindow.HwndSourceHook:{0},{1}", h, (NativeWindowMessage)m);
            }
            }

        private static void RaiseNonClientMouseMessageAsClient(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam) {
            var point = new POINT
            {
                x = NativeMethods.GetXLParam(lParam.ToInt32()),
                y = NativeMethods.GetYLParam(lParam.ToInt32())
                };
            NativeMethods.ScreenToClient(hWnd, ref point);
            NativeMethods.SendMessage(hWnd, msg + 513 - 161, new IntPtr(PressedMouseButtons), NativeMethods.MakeParam(point.x, point.y));
            }

        private IntPtr CallDefWindowProcWithoutVisibleStyle(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled) {
            Debug.Print("CallDefWindowProcWithoutVisibleStyle");
            var flag = UtilityMethods.ModifyStyle(hWnd, 268435456, 0);
            var num = NativeMethods.DefWindowProc(hWnd, msg, wParam, lParam);
            if (flag)
                UtilityMethods.ModifyStyle(hWnd, 0, 268435456);
            handled = true;
            return num;
            }

        private void OnActivate(IntPtr wParam, IntPtr lParam) {
            if (OwnerForActivate != IntPtr.Zero) {
                NativeMethods.SendMessage(OwnerForActivate, NativeMethods.NOTIFYOWNERACTIVATE, wParam, lParam);
                }
            }

        private IntPtr WmNcActivate(IntPtr hWnd, IntPtr wParam, IntPtr lParam, ref Boolean handled) {
            handled = true;
            return NativeMethods.DefWindowProc(hWnd, 134, wParam, NativeMethods.HRGN_NONE);
            }

        private IntPtr WmNcPaint(IntPtr hWnd, IntPtr wParam, IntPtr lParam, ref Boolean handled) {
            if (_isNonClientStripVisible) {
                var hrgnClip = wParam == new IntPtr(1) ? IntPtr.Zero : wParam;
                var dcEx = NativeMethods.GetDCEx(hWnd, hrgnClip, 155);
                if (dcEx != IntPtr.Zero) {
                    try {
                        var nonClientFillColor = NonClientFillColor;
                        var solidBrush = NativeMethods.CreateSolidBrush(nonClientFillColor.B << 16 | nonClientFillColor.G << 8 | nonClientFillColor.R);
                        try {
                            var relativeToWindowRect = GetClientRectRelativeToWindowRect(hWnd);
                            relativeToWindowRect.Top = relativeToWindowRect.Bottom;
                            relativeToWindowRect.Bottom = relativeToWindowRect.Top + 1;
                            NativeMethods.FillRect(dcEx, ref relativeToWindowRect, solidBrush);
                            }
                        finally {
                            NativeMethods.DeleteObject(solidBrush);
                            }
                        }
                    finally {
                        NativeMethods.ReleaseDC(hWnd, dcEx);
                        }
                    }
                }
            handled = true;
            return IntPtr.Zero;
            }

        private static RECT GetClientRectRelativeToWindowRect(IntPtr hWnd) {
            RECT lpRect1;
            NativeMethods.GetWindowRect(hWnd, out lpRect1);
            RECT lpRect2;
            NativeMethods.GetClientRect(hWnd, out lpRect2);
            var point = new POINT
            {
                x = 0,
                y = 0
                };
            NativeMethods.ClientToScreen(hWnd, ref point);
            lpRect2.Offset(point.x - lpRect1.Left, point.y - lpRect1.Top);
            return lpRect2;
            }

        private IntPtr WmNcCalcSize(IntPtr hWnd, IntPtr wParam, IntPtr lParam, ref Boolean handled) {
            _isNonClientStripVisible = false;
            if (NativeMethods.GetWindowPlacement(hWnd).showCmd == 3) {
                var structure1 = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                NativeMethods.DefWindowProc(hWnd, 131, wParam, lParam);
                var structure2 = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                var monitorinfo = MonitorInfoFromWindow(hWnd);
                if (monitorinfo.rcMonitor.Height == monitorinfo.rcWork.Height && monitorinfo.rcMonitor.Width == monitorinfo.rcWork.Width) {
                    _isNonClientStripVisible = true;
                    --structure2.Bottom;
                    }
                structure2.Top = structure1.Top + (Int32)GetWindowInfo(hWnd).cyWindowBorders;
                Marshal.StructureToPtr((Object)structure2, lParam, true);
                }
            handled = true;
            return IntPtr.Zero;
            }

        private IntPtr WmNcHitTest(IntPtr hWnd, IntPtr lParam, ref Boolean handled) {
            if (!this.IsConnectedToPresentationSource())
                return new IntPtr(0);
            var point1 = new Point(NativeMethods.GetXLParam(lParam.ToInt32()), NativeMethods.GetYLParam(lParam.ToInt32()));
            var point2 = PointFromScreen(point1);
            DependencyObject visualHit = null;
            UtilityMethods.HitTestVisibleElements(this, target => {
                visualHit = target.VisualHit;
                return HitTestResultBehavior.Stop;
            }, new PointHitTestParameters(point2));
            var num = 0;
            for (; visualHit != null; visualHit = visualHit.GetVisualOrLogicalParent()) {
                var nonClientArea = visualHit as INonClientArea;
                if (nonClientArea != null) {
                    num = nonClientArea.HitTest(point1);
                    if (num != 0)
                        break;
                    }
                }
            if (num == 0)
                num = 1;
            handled = true;
            return new IntPtr(num);
            }

        private void WmSysCommand(IntPtr hWnd, IntPtr wParam, IntPtr lParam) {
            var scWparam = NativeMethods.GET_SC_WPARAM(wParam);
            if (scWparam == 61456)
                NativeMethods.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, NativeMethods.RedrawWindowFlags.Invalidate | NativeMethods.RedrawWindowFlags.NoChildren | NativeMethods.RedrawWindowFlags.UpdateNow | NativeMethods.RedrawWindowFlags.Frame);
            if ((scWparam == 61488 || scWparam == 61472 || (scWparam == 61456 || scWparam == 61440)) && (WindowState == WindowState.Normal && !IsAeroSnappedToMonitor(hWnd)))
                logicalSizeForRestore = new Rect(Left, Top, Width, Height);
            if (scWparam == 61456 && WindowState == WindowState.Maximized && logicalSizeForRestore == Rect.Empty)
                logicalSizeForRestore = new Rect(Left, Top, Width, Height);
            if (scWparam != 61728 || WindowState == WindowState.Minimized || (logicalSizeForRestore.Width <= 0.0 || logicalSizeForRestore.Height <= 0.0))
                return;
            Left = logicalSizeForRestore.Left;
            Top = logicalSizeForRestore.Top;
            Width = logicalSizeForRestore.Width;
            Height = logicalSizeForRestore.Height;
            useLogicalSizeForRestore = true;
            }

        private const UInt32 SWP_NOSIZE         = 0x0001;
        private const UInt32 SWP_NOMOVE         = 0x0002;
        private const UInt32 SWP_NOZORDER       = 0x0004;
        private const UInt32 SWP_NOREDRAW       = 0x0008;
        private const UInt32 SWP_NOACTIVATE     = 0x0010;
        private const UInt32 SWP_FRAMECHANGED   = 0x0020;
        private const UInt32 SWP_SHOWWINDOW     = 0x0040;
        private const UInt32 SWP_HIDEWINDOW     = 0x0080;
        private const UInt32 SWP_NOCOPYBITS     = 0x0100;
        private const UInt32 SWP_NOOWNERZORDER  = 0x0200;
        private const UInt32 SWP_NOSENDCHANGING = 0x0400;
        private const UInt32 SWP_DEFERERASE     = 0x2000;
        private const UInt32 SWP_ASYNCWINDOWPOS = 0x4000;

        private Boolean IsAeroSnappedToMonitor(IntPtr hWnd) {
            var monitorinfo = MonitorInfoFromWindow(hWnd);
            var deviceUnits = new Rect(Left, Top, Width, Height).LogicalToDeviceUnits();
            return monitorinfo.rcWork.Height == deviceUnits.Height && monitorinfo.rcWork.Top == deviceUnits.Top;
            }

        private void WmWindowPosChanging(IntPtr hwnd, IntPtr lParam) {
            var structure = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
            if (((Int32)structure.flags & 2) != 0 || ((Int32)structure.flags & 1) != 0 || (structure.cx <= 0 || structure.cy <= 0))
                return;
            var floatRect = new Rect(structure.x, structure.y, structure.cx, structure.cy).DeviceToLogicalUnits();
            if (useLogicalSizeForRestore) {
                floatRect = logicalSizeForRestore;
                logicalSizeForRestore = Rect.Empty;
                useLogicalSizeForRestore = false;
                }
            var deviceUnits = ViewSite.GetOnScreenPosition(floatRect).LogicalToDeviceUnits();
            structure.x = (Int32)deviceUnits.X;
            structure.y = (Int32)deviceUnits.Y;
            Marshal.StructureToPtr((Object)structure, lParam, true);
            }

        /// <summary>
        /// The framework calls this member function when the size, position, or Z-order has changed as a result of a call to the SetWindowPos member function or another window-management function.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lParam"></param>
        private void OnWindowPosChanged(IntPtr hWnd, IntPtr lParam) {
            try
                {
                var structure = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                var windowPlacement = NativeMethods.GetWindowPlacement(hWnd);
                var currentBounds = new RECT(structure.x, structure.y, structure.x + structure.cx, structure.y + structure.cy);
                     if ((structure.flags & SWP_NOSIZE) != SWP_NOSIZE) { UpdateClipRegion(hWnd, windowPlacement, ClipRegionChangeType.FromSize, currentBounds); }
                else if ((structure.flags & SWP_NOMOVE) != SWP_NOMOVE) { UpdateClipRegion(hWnd, windowPlacement, ClipRegionChangeType.FromPosition, currentBounds); }
                OnWindowPosChanged(hWnd, windowPlacement.showCmd, windowPlacement.rcNormalPosition.ToInt32Rect());
                UpdateGlowWindowPositions(((Int32)structure.flags & SWP_SHOWWINDOW) == 0);
                UpdateZOrderOfThisAndOwner();
                }
            catch (Win32Exception)
                {
                }
            }

        private void UpdateZOrderOfThisAndOwner() {
            if (updatingZOrder)
                return;
            try {
                updatingZOrder = true;
                var windowInteropHelper = new WindowInteropHelper(this);
                var handle = windowInteropHelper.Handle;
                foreach (var loadedGlowWindow in LoadedGlowWindows) {
                    if (NativeMethods.GetWindow(loadedGlowWindow.Handle, 3) != handle)
                        NativeMethods.SetWindowPos(loadedGlowWindow.Handle, handle, 0, 0, 0, 0, 19);
                    handle = loadedGlowWindow.Handle;
                    }
                var owner = windowInteropHelper.Owner;
                if (!(owner != IntPtr.Zero))
                    return;
                UpdateZOrderOfOwner(owner);
                }
            finally {
                updatingZOrder = false;
                }
            }

        private void UpdateZOrderOfOwner(IntPtr hwndOwner) {
            var lastOwnedWindow = IntPtr.Zero;
            NativeMethods.EnumThreadWindows(NativeMethods.GetCurrentThreadId(), (hwnd, lParam) => {
                if (NativeMethods.GetWindow(hwnd, 4) == hwndOwner)
                    lastOwnedWindow = hwnd;
                return true;
            }, IntPtr.Zero);
            if (!(lastOwnedWindow != IntPtr.Zero) || !(NativeMethods.GetWindow(hwndOwner, 3) != lastOwnedWindow))
                return;
            NativeMethods.SetWindowPos(hwndOwner, lastOwnedWindow, 0, 0, 0, 0, 19);
            }

        protected virtual void OnWindowPosChanged(IntPtr hWnd, Int32 showCmd, Int32Rect rcNormalPosition) {
            }

        protected void UpdateClipRegion(ClipRegionChangeType regionChangeType = ClipRegionChangeType.FromPropertyChange) {
            var hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            if (hwndSource == null)
                return;
            RECT lpRect;
            NativeMethods.GetWindowRect(hwndSource.Handle, out lpRect);
            var windowPlacement = NativeMethods.GetWindowPlacement(hwndSource.Handle);
            UpdateClipRegion(hwndSource.Handle, windowPlacement, regionChangeType, lpRect);
            }

        private void UpdateClipRegion(IntPtr hWnd, WINDOWPLACEMENT placement, ClipRegionChangeType changeType, RECT currentBounds) {
            UpdateClipRegionCore(hWnd, placement.showCmd, changeType, currentBounds.ToInt32Rect());
            lastWindowPlacement = placement.showCmd;
            }

        protected virtual Boolean UpdateClipRegionCore(IntPtr hWnd, Int32 showCmd, ClipRegionChangeType changeType, Int32Rect currentBounds) {
            if (showCmd == 3) {
                UpdateMaximizedClipRegion(hWnd);
                return true;
                }
            if (changeType != ClipRegionChangeType.FromSize && changeType != ClipRegionChangeType.FromPropertyChange && lastWindowPlacement == showCmd)
                return false;
            if (CornerRadius < 0)
                ClearClipRegion(hWnd);
            else
                SetRoundRect(hWnd, currentBounds.Width, currentBounds.Height);
            return true;
            }

        private WINDOWINFO GetWindowInfo(IntPtr hWnd) {
            var pwi = new WINDOWINFO();
            pwi.cbSize = Marshal.SizeOf((Object)pwi);
            NativeMethods.GetWindowInfo(hWnd, ref pwi);
            return pwi;
            }

        private void UpdateMaximizedClipRegion(IntPtr hWnd) {
            var relativeToWindowRect = GetClientRectRelativeToWindowRect(hWnd);
            if (_isNonClientStripVisible)
                ++relativeToWindowRect.Bottom;
            var rectRgnIndirect = NativeMethods.CreateRectRgnIndirect(ref relativeToWindowRect);
            NativeMethods.SetWindowRgn(hWnd, rectRgnIndirect, NativeMethods.IsWindowVisible(hWnd));
            }

        private static MONITORINFO MonitorInfoFromWindow(IntPtr hWnd) {
            var hMonitor = NativeMethods.MonitorFromWindow(hWnd, 2);
            var monitorInfo = new MONITORINFO();
            monitorInfo.cbSize = (UInt32)Marshal.SizeOf(typeof(MONITORINFO));
            NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);
            return monitorInfo;
            }

        private void ClearClipRegion(IntPtr hWnd) {
            NativeMethods.SetWindowRgn(hWnd, IntPtr.Zero, NativeMethods.IsWindowVisible(hWnd));
            }

        protected void SetRoundRect(IntPtr hWnd, Int32 width, Int32 height) {
            var roundRectRegion = ComputeRoundRectRegion(0, 0, width, height, CornerRadius);
            NativeMethods.SetWindowRgn(hWnd, roundRectRegion, NativeMethods.IsWindowVisible(hWnd));
            }

        private IntPtr ComputeRoundRectRegion(Int32 left, Int32 top, Int32 width, Int32 height, Int32 cornerRadius) {
            var nWidthEllipse = (Int32)(2 * cornerRadius * DpiHelper.LogicalToDeviceUnitsScalingFactorX);
            var nHeightEllipse = (Int32)(2 * cornerRadius * DpiHelper.LogicalToDeviceUnitsScalingFactorY);
            return NativeMethods.CreateRoundRectRgn(left, top, left + width + 1, top + height + 1, nWidthEllipse, nHeightEllipse);
            }

        protected IntPtr ComputeCornerRadiusRectRegion(Int32Rect rect, CornerRadius cornerRadius) {
            if (cornerRadius.TopLeft == cornerRadius.TopRight && cornerRadius.TopLeft == cornerRadius.BottomLeft && cornerRadius.BottomLeft == cornerRadius.BottomRight)
                return ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (Int32)cornerRadius.TopLeft);
            var num1 = IntPtr.Zero;
            var num2 = IntPtr.Zero;
            var num3 = IntPtr.Zero;
            var num4 = IntPtr.Zero;
            var num5 = IntPtr.Zero;
            var num6 = IntPtr.Zero;
            var num7 = IntPtr.Zero;
            var num8 = IntPtr.Zero;
            var num9 = IntPtr.Zero;
            var num10 = IntPtr.Zero;
            try {
                num1 = ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (Int32)cornerRadius.TopLeft);
                num2 = ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (Int32)cornerRadius.TopRight);
                num3 = ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (Int32)cornerRadius.BottomLeft);
                num4 = ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (Int32)cornerRadius.BottomRight);
                var point = new POINT
                {
                    x = rect.X + rect.Width / 2,
                    y = rect.Y + rect.Height / 2
                    };
                num5 = NativeMethods.CreateRectRgn(rect.X, rect.Y, point.x + 1, point.y + 1);
                num6 = NativeMethods.CreateRectRgn(point.x - 1, rect.Y, rect.X + rect.Width, point.y + 1);
                num7 = NativeMethods.CreateRectRgn(rect.X, point.y - 1, point.x + 1, rect.Y + rect.Height);
                num8 = NativeMethods.CreateRectRgn(point.x - 1, point.y - 1, rect.X + rect.Width, rect.Y + rect.Height);
                num9 = NativeMethods.CreateRectRgn(0, 0, 1, 1);
                num10 = NativeMethods.CreateRectRgn(0, 0, 1, 1);
                NativeMethods.CombineRgn(num10, num1, num5, NativeMethods.CombineMode.RGN_AND);
                NativeMethods.CombineRgn(num9, num2, num6, NativeMethods.CombineMode.RGN_AND);
                NativeMethods.CombineRgn(num10, num10, num9, NativeMethods.CombineMode.RGN_OR);
                NativeMethods.CombineRgn(num9, num3, num7, NativeMethods.CombineMode.RGN_AND);
                NativeMethods.CombineRgn(num10, num10, num9, NativeMethods.CombineMode.RGN_OR);
                NativeMethods.CombineRgn(num9, num4, num8, NativeMethods.CombineMode.RGN_AND);
                NativeMethods.CombineRgn(num10, num10, num9, NativeMethods.CombineMode.RGN_OR);
                }
            finally {
                if (num1 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num1);
                if (num2 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num2);
                if (num3 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num3);
                if (num4 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num4);
                if (num5 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num5);
                if (num6 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num6);
                if (num7 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num7);
                if (num8 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num8);
                if (num9 != IntPtr.Zero)
                    NativeMethods.DeleteObject(num9);
                }
            return num10;
            }

        public static void ShowWindowMenu(HwndSource source, Visual element, Point elementPoint, Size elementSize) {
            if (elementPoint.X < 0.0 || elementPoint.X > elementSize.Width || (elementPoint.Y < 0.0 || elementPoint.Y > elementSize.Height))
                return;
            var screen = element.PointToScreen(elementPoint);
            ShowWindowMenu(source, screen, true);
            }

        protected static void ShowWindowMenu(HwndSource source, Point screenPoint, Boolean canMinimize) {
            var systemMetrics = NativeMethods.GetSystemMetrics(40);
            var systemMenu = NativeMethods.GetSystemMenu(source.Handle, false);
            var windowPlacement = NativeMethods.GetWindowPlacement(source.Handle);
            var flag = UtilityMethods.ModifyStyle(source.Handle, 268435456, 0);
            var num1 = canMinimize ? 0U : 1U;
            if (windowPlacement.showCmd == 1) {
                NativeMethods.EnableMenuItem(systemMenu, 61728U, 1U);
                NativeMethods.EnableMenuItem(systemMenu, 61456U, 0U);
                NativeMethods.EnableMenuItem(systemMenu, 61440U, 0U);
                NativeMethods.EnableMenuItem(systemMenu, 61488U, 0U);
                NativeMethods.EnableMenuItem(systemMenu, 61472U, 0U | num1);
                NativeMethods.EnableMenuItem(systemMenu, 61536U, 0U);
                }
            else if (windowPlacement.showCmd == 3) {
                NativeMethods.EnableMenuItem(systemMenu, 61728U, 0U);
                NativeMethods.EnableMenuItem(systemMenu, 61456U, 1U);
                NativeMethods.EnableMenuItem(systemMenu, 61440U, 1U);
                NativeMethods.EnableMenuItem(systemMenu, 61488U, 1U);
                NativeMethods.EnableMenuItem(systemMenu, 61472U, 0U | num1);
                NativeMethods.EnableMenuItem(systemMenu, 61536U, 0U);
                }
            if (flag)
                UtilityMethods.ModifyStyle(source.Handle, 0, 268435456);
            var fuFlags = (UInt32)(systemMetrics | 256 | 128 | 2);
            var num2 = NativeMethods.TrackPopupMenuEx(systemMenu, fuFlags, (Int32)screenPoint.X, (Int32)screenPoint.Y, source.Handle, IntPtr.Zero);
            if (num2 == 0)
                return;
            NativeMethods.PostMessage(source.Handle, 274, new IntPtr(num2), IntPtr.Zero);
            }

        protected override void OnClosed(EventArgs e) {
            StopTimer();
            DestroyGlowWindows();
            base.OnClosed(e);
            }

        private GlowWindow GetOrCreateGlowWindow(Int32 direction) {
            if (_glowWindows[direction] == null) {
                _glowWindows[direction] = new GlowWindow(this, (Dock)direction) {
                    ActiveGlowColor = ActiveGlowColor,
                    InactiveGlowColor = InactiveGlowColor,
                    IsActive = IsActive
                    };
                }
            return _glowWindows[direction];
            }

        private void DestroyGlowWindows() {
            for (var index = 0; index < _glowWindows.Length; ++index) {
                using (_glowWindows[index])
                    _glowWindows[index] = null;
                }
            }

        private void UpdateGlowWindowPositions(Boolean delayIfNecessary) {
            using (DeferGlowChanges()) {
                UpdateGlowVisibility(delayIfNecessary);
                foreach (var loadedGlowWindow in LoadedGlowWindows)
                    loadedGlowWindow.UpdateWindowPos();
                }
            }

        private void UpdateGlowActiveState() {
            using (DeferGlowChanges()) {
                foreach (var loadedGlowWindow in LoadedGlowWindows)
                    loadedGlowWindow.IsActive = IsActive;
                }
            }

        #region M:ChangeOwnerForActivate(IntPtr)
        public void ChangeOwnerForActivate(IntPtr newOwner) {
            OwnerForActivate = newOwner;
            }
        #endregion

        public void ChangeOwner(IntPtr newOwner) {
            new WindowInteropHelper(this).Owner = newOwner;
            foreach (var loadedGlowWindow in LoadedGlowWindows)
                loadedGlowWindow.ChangeOwner(newOwner);
            UpdateZOrderOfThisAndOwner();
            }

        private void UpdateGlowVisibility(Boolean delayIfNecessary) {
            var shouldShowGlow = ShouldShowGlow;
            if (shouldShowGlow == IsGlowVisible)
                return;
            if (SystemParameters.MinimizeAnimation & shouldShowGlow & delayIfNecessary) {
                if (_makeGlowVisibleTimer != null) {
                    _makeGlowVisibleTimer.Stop();
                    }
                else {
                    _makeGlowVisibleTimer = new DispatcherTimer();
                    _makeGlowVisibleTimer.Interval = TimeSpan.FromMilliseconds(200.0);
                    _makeGlowVisibleTimer.Tick += OnDelayedVisibilityTimerTick;
                    }
                _makeGlowVisibleTimer.Start();
                }
            else {
                StopTimer();
                IsGlowVisible = shouldShowGlow;
                }
            }

        private void StopTimer() {
            if (_makeGlowVisibleTimer == null)
                return;
            _makeGlowVisibleTimer.Stop();
            _makeGlowVisibleTimer.Tick -= OnDelayedVisibilityTimerTick;
            _makeGlowVisibleTimer = null;
            }

        private void OnDelayedVisibilityTimerTick(Object sender, EventArgs e) {
            Debug.Print("{0}.{1}",GetType().Name,new StackTrace().GetFrame(0).GetMethod().Name);
            StopTimer();
            UpdateGlowWindowPositions(false);
            }

        private void UpdateGlowColors() {
            using (DeferGlowChanges()) {
                foreach (var loadedGlowWindow in LoadedGlowWindows) {
                    loadedGlowWindow.ActiveGlowColor = ActiveGlowColor;
                    loadedGlowWindow.InactiveGlowColor = InactiveGlowColor;
                    }
                }
            }

        private IDisposable DeferGlowChanges() {
            return new ChangeScope(this);
            }

        private void EndDeferGlowChanges() {
            foreach (var loadedGlowWindow in LoadedGlowWindows)
                loadedGlowWindow.CommitChanges();
            }

        protected enum ClipRegionChangeType {
            FromSize,
            FromPosition,
            FromPropertyChange,
            FromUndockSingleTab,
            }

        private class ChangeScope : DisposableObject {
            private readonly CustomChromeWindow _window;

            public ChangeScope(CustomChromeWindow window) {
                _window = window;
                ++_window._deferGlowChangesCount;
                }

            protected override void DisposeManagedResources() {
                --_window._deferGlowChangesCount;
                if (_window._deferGlowChangesCount != 0)
                    return;
                _window.EndDeferGlowChanges();
                }
            }

        private sealed class GlowBitmap : DisposableObject {
            private static readonly CachedBitmapInfo[] _transparencyMasks = new CachedBitmapInfo[16];
            public const Int32 GlowBitmapPartCount = 16;
            private const Int32 BytesPerPixelBgra32 = 4;
            private readonly IntPtr _pbits;
            private readonly NativeMethods.BITMAPINFO _bitmapInfo;

            public IntPtr Handle { get; }

            public IntPtr DIBits {
                get {
                    return _pbits;
                    }
                }

            public Int32 Width {
                get {
                    return _bitmapInfo.biWidth;
                    }
                }

            public Int32 Height {
                get {
                    return -_bitmapInfo.biHeight;
                    }
                }

            public GlowBitmap(IntPtr hdcScreen, Int32 width, Int32 height) {
                _bitmapInfo.biSize = Marshal.SizeOf(typeof(NativeMethods.BITMAPINFOHEADER));
                _bitmapInfo.biPlanes = 1;
                _bitmapInfo.biBitCount = 32;
                _bitmapInfo.biCompression = 0;
                _bitmapInfo.biXPelsPerMeter = 0;
                _bitmapInfo.biYPelsPerMeter = 0;
                _bitmapInfo.biWidth = width;
                _bitmapInfo.biHeight = -height;
                Handle = NativeMethods.CreateDIBSection(hdcScreen, ref _bitmapInfo, 0U, out _pbits, IntPtr.Zero, 0U);
                }

            protected override void DisposeNativeResources() {
                NativeMethods.DeleteObject(Handle);
                }

            private static Byte PremultiplyAlpha(Byte channel, Byte alpha) {
                return (Byte)(channel * alpha / (Double)Byte.MaxValue);
                }

            public static GlowBitmap Create(GlowDrawingContext drawingContext, GlowBitmapPart bitmapPart, Color color) {
                var alphaMask = GetOrCreateAlphaMask(bitmapPart);
                var glowBitmap = new GlowBitmap(drawingContext.ScreenDC, alphaMask.Width, alphaMask.Height);
                var ofs = 0;
                while (ofs < alphaMask.DIBits.Length) {
                    var diBit = alphaMask.DIBits[ofs + 3];
                    var r = PremultiplyAlpha(color.R, diBit);
                    var g = PremultiplyAlpha(color.G, diBit);
                    var b = PremultiplyAlpha(color.B, diBit);
                    Marshal.WriteByte(glowBitmap.DIBits, ofs, b);
                    Marshal.WriteByte(glowBitmap.DIBits, ofs + 1, g);
                    Marshal.WriteByte(glowBitmap.DIBits, ofs + 2, r);
                    Marshal.WriteByte(glowBitmap.DIBits, ofs + 3, diBit);
                    ofs += 4;
                    }
                return glowBitmap;
                }

            private static CachedBitmapInfo GetOrCreateAlphaMask(GlowBitmapPart bitmapPart) {
                var index = (Int32)bitmapPart;
                if (_transparencyMasks[index] == null) {
                    var bitmapImage = new BitmapImage(CommonUtilities.MakePackUri(typeof(GlowBitmap).Assembly, "Resources/" + bitmapPart.ToString() + ".png"));
                    var diBits = new Byte[4 * bitmapImage.PixelWidth * bitmapImage.PixelHeight];
                    var stride = 4 * bitmapImage.PixelWidth;
                    bitmapImage.CopyPixels(diBits, stride, 0);
                    _transparencyMasks[index] = new CachedBitmapInfo(diBits, bitmapImage.PixelWidth, bitmapImage.PixelHeight);
                    }
                return _transparencyMasks[index];
                }

            private sealed class CachedBitmapInfo {
                public readonly Int32 Width;
                public readonly Int32 Height;
                public readonly Byte[] DIBits;

                public CachedBitmapInfo(Byte[] diBits, Int32 width, Int32 height) {
                    Width = width;
                    Height = height;
                    DIBits = diBits;
                    }
                }
            }

        private enum GlowBitmapPart {
            CornerTopLeft,
            CornerTopRight,
            CornerBottomLeft,
            CornerBottomRight,
            TopLeft,
            Top,
            TopRight,
            LeftTop,
            Left,
            LeftBottom,
            BottomLeft,
            Bottom,
            BottomRight,
            RightTop,
            Right,
            RightBottom,
            }

        private sealed class GlowDrawingContext : DisposableObject {
            public NativeMethods.BLENDFUNCTION Blend;
            private readonly GlowBitmap windowBitmap;

            public Boolean IsInitialized {
                get {
                    if (ScreenDC != IntPtr.Zero && WindowDC != IntPtr.Zero && BackgroundDC != IntPtr.Zero)
                        return windowBitmap != null;
                    return false;
                    }
                }

            public IntPtr ScreenDC { get; }
            public IntPtr WindowDC { get; }
            public IntPtr BackgroundDC { get; }

            public Int32 Width {
                get {
                    return windowBitmap.Width;
                    }
                }

            public Int32 Height {
                get {
                    return windowBitmap.Height;
                    }
                }

            public GlowDrawingContext(Int32 width, Int32 height) {
                ScreenDC = NativeMethods.GetDC(IntPtr.Zero);
                if (ScreenDC == IntPtr.Zero)
                    return;
                WindowDC = NativeMethods.CreateCompatibleDC(ScreenDC);
                if (WindowDC == IntPtr.Zero)
                    return;
                BackgroundDC = NativeMethods.CreateCompatibleDC(ScreenDC);
                if (BackgroundDC == IntPtr.Zero)
                    return;
                Blend.BlendOp = 0;
                Blend.BlendFlags = 0;
                Blend.SourceConstantAlpha = Byte.MaxValue;
                Blend.AlphaFormat = 1;
                windowBitmap = new GlowBitmap(ScreenDC, width, height);
                NativeMethods.SelectObject(WindowDC, windowBitmap.Handle);
                }

            protected override void DisposeManagedResources() {
                windowBitmap.Dispose();
                }

            protected override void DisposeNativeResources() {
                if (ScreenDC != IntPtr.Zero)
                    NativeMethods.ReleaseDC(IntPtr.Zero, ScreenDC);
                if (WindowDC != IntPtr.Zero)
                    NativeMethods.DeleteDC(WindowDC);
                if (!(BackgroundDC != IntPtr.Zero))
                    return;
                NativeMethods.DeleteDC(BackgroundDC);
                }
            }

        private sealed class GlowWindow : HwndWrapper {
            private readonly GlowBitmap[] _activeGlowBitmaps = new GlowBitmap[16];
            private readonly GlowBitmap[] _inactiveGlowBitmaps = new GlowBitmap[16];
            private Color _activeGlowColor = Colors.Transparent;
            private Color _inactiveGlowColor = Colors.Transparent;
            private const String GlowWindowClassName = "VisualStudioGlowWindow";
            private const Int32 GlowDepth = 9;
            private const Int32 CornerGripThickness = 18;
            private readonly CustomChromeWindow _targetWindow;
            private readonly Dock _orientation;
            private static UInt16 _sharedWindowClassAtom;
            private static NativeMethods.WndProc _sharedWndProc;
            private static Int64 _createdGlowWindows;
            private static Int64 _disposedGlowWindows;
            private Int32 _left;
            private Int32 _top;
            private Int32 _width;
            private Int32 _height;
            private Boolean _isVisible;
            private Boolean _isActive;
            private FieldInvalidationTypes _invalidatedValues;
            private Boolean _pendingDelayRender;

            private Boolean IsDeferringChanges {
                get {
                    return _targetWindow._deferGlowChangesCount > 0;
                    }
                }

            private static UInt16 SharedWindowClassAtom {
                get {
                    if (_sharedWindowClassAtom == 0) {
                        var lpWndClass = new WNDCLASS
                        {
                            cbClsExtra = 0,
                            cbWndExtra = 0,
                            hbrBackground = IntPtr.Zero,
                            hCursor = IntPtr.Zero,
                            hIcon = IntPtr.Zero,
                            lpfnWndProc = _sharedWndProc = NativeMethods.DefWindowProc,
                            lpszClassName = "VisualStudioGlowWindow",
                            lpszMenuName = null,
                            style = 0U
                            };
                        _sharedWindowClassAtom = NativeMethods.RegisterClass(ref lpWndClass);
                        }
                    return _sharedWindowClassAtom;
                    }
                }

            #region P:IsVisible:Boolean
            public Boolean IsVisible {
                get { return _isVisible; }
                set { UpdateProperty(ref _isVisible, value, FieldInvalidationTypes.Render | FieldInvalidationTypes.Visibility); }
                }
            #endregion
            #region P:Left:Int32
            public Int32 Left {
                get { return _left; }
                set { UpdateProperty(ref _left, value, FieldInvalidationTypes.Location); }
                }
            #endregion
            #region P:Top:Int32
            public Int32 Top {
                get { return _top; }
                set { UpdateProperty(ref _top, value, FieldInvalidationTypes.Location); }
                }
            #endregion
            #region P:Width:Int32
            public Int32 Width {
                get { return _width; }
                set { UpdateProperty(ref _width, value, FieldInvalidationTypes.Size | FieldInvalidationTypes.Render); }
                }
            #endregion
            #region P:Height:Int32
            public Int32 Height {
                get { return _height; }
                set { UpdateProperty(ref _height, value, FieldInvalidationTypes.Size | FieldInvalidationTypes.Render); }
                }
            #endregion
            #region P:IsActive:Boolean
            public Boolean IsActive {
                get { return _isActive; }
                set { UpdateProperty(ref _isActive, value, FieldInvalidationTypes.Render); }
                }
            #endregion
            #region P:ActiveGlowColor:Color
            public Color ActiveGlowColor {
                get { return _activeGlowColor; }
                set { UpdateProperty(ref _activeGlowColor, value, FieldInvalidationTypes.ActiveColor | FieldInvalidationTypes.Render); }
                }
            #endregion
            #region P:InactiveGlowColor:Color
            public Color InactiveGlowColor {
                get { return _inactiveGlowColor; }
                set { UpdateProperty(ref _inactiveGlowColor, value, FieldInvalidationTypes.InactiveColor | FieldInvalidationTypes.Render); }
                }
            #endregion

            private IntPtr TargetWindowHandle {
                get {
                    return new WindowInteropHelper(_targetWindow).Handle;
                    }
                }

            protected override Boolean IsWindowSubclassed {
                get {
                    return true;
                    }
                }

            private Boolean IsPositionValid {
                get {
                    return !InvalidatedValuesHasFlag(FieldInvalidationTypes.Location | FieldInvalidationTypes.Size | FieldInvalidationTypes.Visibility);
                    }
                }

            public GlowWindow(CustomChromeWindow owner, Dock orientation) {
                Validate.IsNotNull(owner, "owner");
                _targetWindow = owner;
                _orientation = orientation;
                ++_createdGlowWindows;
                }

            private void UpdateProperty<T>(ref T field, T value, FieldInvalidationTypes invalidatedValues) where T : struct {
                if (field.Equals(value))
                    return;
                field = value;
                _invalidatedValues = _invalidatedValues | invalidatedValues;
                if (IsDeferringChanges)
                    return;
                CommitChanges();
                }

            protected override UInt16 CreateWindowClassCore() {
                return SharedWindowClassAtom;
                }

            protected override void DestroyWindowClassCore() {
                }

            protected override IntPtr CreateWindowCore() {
                return NativeMethods.CreateWindowEx(524416, new IntPtr(WindowClassAtom), String.Empty, -2046820352, 0, 0, 0, 0, new WindowInteropHelper(_targetWindow).Owner, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
                }

            public void ChangeOwner(IntPtr newOwner) {
                NativeMethods.SetWindowLongPtr(Handle, NativeMethods.GWLP.HWNDPARENT, newOwner);
                }

            protected override IntPtr WndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam) {
                //Debug.Print("GlowWindow.WndProc:{0},{1}", hwnd, (NativeWindowMessage)msg);
                switch (msg) {
                    case WM_DISPLAYCHANGE:
                    if (IsVisible) { RenderLayeredWindow(); }
                    break;
                    case WM_NCHITTEST:
                    return new IntPtr(WmNcHitTest(lParam));
                    case WM_NCLBUTTONDOWN:
                    case WM_NCLBUTTONDBLCLK:
                    case WM_NCRBUTTONDOWN:
                    case WM_NCRBUTTONDBLCLK:
                    case WM_NCMBUTTONDOWN:
                    case WM_NCMBUTTONDBLCLK:
                    case WM_NCXBUTTONDOWN:
                    case WM_NCXBUTTONDBLCLK:
                    var targetWindowHandle = TargetWindowHandle;
                    NativeMethods.SendMessage(targetWindowHandle, 6, new IntPtr(2), IntPtr.Zero);
                    NativeMethods.SendMessage(targetWindowHandle, msg, wParam, IntPtr.Zero);
                    return IntPtr.Zero;
                    case WM_ACTIVATE:
                    return IntPtr.Zero;
                    case WM_WINDOWPOSCHANGING:
                        var structure = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                        structure.flags |= SWP_NOACTIVATE;
                        Marshal.StructureToPtr((Object)structure, lParam, true);
                    break;
                    }
                return base.WndProc(hwnd, msg, wParam, lParam);
                }

            private Int32 WmNcHitTest(IntPtr lParam) {
                var xlParam = NativeMethods.GetXLParam(lParam.ToInt32());
                var ylParam = NativeMethods.GetYLParam(lParam.ToInt32());
                RECT lpRect;
                NativeMethods.GetWindowRect(Handle, out lpRect);
                switch (_orientation) {
                    case Dock.Left:
                    if (ylParam - 18 < lpRect.Top)
                        return 13;
                    return ylParam + 18 > lpRect.Bottom ? 16 : 10;
                    case Dock.Top:
                    if (xlParam - 18 < lpRect.Left)
                        return 13;
                    return xlParam + 18 > lpRect.Right ? 14 : 12;
                    case Dock.Right:
                    if (ylParam - 18 < lpRect.Top)
                        return 14;
                    return ylParam + 18 > lpRect.Bottom ? 17 : 11;
                    default:
                    if (xlParam - 18 < lpRect.Left)
                        return 16;
                    return xlParam + 18 > lpRect.Right ? 17 : 15;
                    }
                }

            public void CommitChanges() {
                InvalidateCachedBitmaps();
                UpdateWindowPosCore();
                UpdateLayeredWindowCore();
                _invalidatedValues = FieldInvalidationTypes.None;
                }

            private Boolean InvalidatedValuesHasFlag(FieldInvalidationTypes flag) {
                return (UInt32)(_invalidatedValues & flag) > 0U;
                }

            private void InvalidateCachedBitmaps() {
                if (InvalidatedValuesHasFlag(FieldInvalidationTypes.ActiveColor))
                    ClearCache(_activeGlowBitmaps);
                if (!InvalidatedValuesHasFlag(FieldInvalidationTypes.InactiveColor))
                    return;
                ClearCache(_inactiveGlowBitmaps);
                }

            private void UpdateWindowPosCore() {
                if (!InvalidatedValuesHasFlag(FieldInvalidationTypes.Location | FieldInvalidationTypes.Size | FieldInvalidationTypes.Visibility))
                    return;
                var flags = 532;
                if (InvalidatedValuesHasFlag(FieldInvalidationTypes.Visibility)) {
                    if (IsVisible)
                        flags |= 64;
                    else
                        flags |= 131;
                    }
                if (!InvalidatedValuesHasFlag(FieldInvalidationTypes.Location))
                    flags |= 2;
                if (!InvalidatedValuesHasFlag(FieldInvalidationTypes.Size))
                    flags |= 1;
                NativeMethods.SetWindowPos(Handle, IntPtr.Zero, Left, Top, Width, Height, flags);
                }

            private void UpdateLayeredWindowCore() {
                if (!IsVisible || !InvalidatedValuesHasFlag(FieldInvalidationTypes.Render))
                    return;
                if (IsPositionValid) {
                    BeginDelayedRender();
                    }
                else {
                    CancelDelayedRender();
                    RenderLayeredWindow();
                    }
                }

            private void BeginDelayedRender() {
                if (_pendingDelayRender)
                    return;
                _pendingDelayRender = true;
                CompositionTarget.Rendering += CommitDelayedRender;
                }

            private void CancelDelayedRender() {
                if (!_pendingDelayRender)
                    return;
                _pendingDelayRender = false;
                CompositionTarget.Rendering -= CommitDelayedRender;
                }

            private void CommitDelayedRender(Object sender, EventArgs e) {
                CancelDelayedRender();
                if (!IsVisible)
                    return;
                RenderLayeredWindow();
                }

            private void RenderLayeredWindow() {
                using (var drawingContext = new GlowDrawingContext(Width, Height)) {
                    if (!drawingContext.IsInitialized)
                        return;
                    switch (_orientation) {
                        case Dock.Left:
                        DrawLeft(drawingContext);
                        break;
                        case Dock.Top:
                        DrawTop(drawingContext);
                        break;
                        case Dock.Right:
                        DrawRight(drawingContext);
                        break;
                        default:
                        DrawBottom(drawingContext);
                        break;
                        }
                    var pptDest = new POINT
                    {
                        x = Left,
                        y = Top
                        };
                    var psize = new Win32SIZE
                    {
                        cx = Width,
                        cy = Height
                        };
                    var pptSrc = new POINT
                    {
                        x = 0,
                        y = 0
                        };
                    NativeMethods.UpdateLayeredWindow(Handle, drawingContext.ScreenDC, ref pptDest, ref psize, drawingContext.WindowDC, ref pptSrc, 0U, ref drawingContext.Blend, 2U);
                    }
                }

            private GlowBitmap GetOrCreateBitmap(GlowDrawingContext drawingContext, GlowBitmapPart bitmapPart) {
                GlowBitmap[] glowBitmapArray;
                Color color;
                if (IsActive) {
                    glowBitmapArray = _activeGlowBitmaps;
                    color = ActiveGlowColor;
                    }
                else {
                    glowBitmapArray = _inactiveGlowBitmaps;
                    color = InactiveGlowColor;
                    }
                var index = (Int32)bitmapPart;
                if (glowBitmapArray[index] == null)
                    glowBitmapArray[index] = GlowBitmap.Create(drawingContext, bitmapPart, color);
                return glowBitmapArray[index];
                }

            private void ClearCache(GlowBitmap[] cache) {
                for (var index = 0; index < cache.Length; ++index) {
                    using (cache[index])
                        cache[index] = null;
                    }
                }

            protected override void DisposeManagedResources() {
                ClearCache(_activeGlowBitmaps);
                ClearCache(_inactiveGlowBitmaps);
                }

            protected override void DisposeNativeResources() {
                base.DisposeNativeResources();
                ++_disposedGlowWindows;
                }

            private void DrawLeft(GlowDrawingContext drawingContext) {
                var bitmap1 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.CornerTopLeft);
                var bitmap2 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.LeftTop);
                var bitmap3 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.Left);
                var bitmap4 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.LeftBottom);
                var bitmap5 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.CornerBottomLeft);
                var height = bitmap1.Height;
                var yoriginDest1 = height + bitmap2.Height;
                var yoriginDest2 = drawingContext.Height - bitmap5.Height;
                var yoriginDest3 = yoriginDest2 - bitmap4.Height;
                var hDest = yoriginDest3 - yoriginDest1;
                NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap1.Handle);
                NativeMethods.AlphaBlend(drawingContext.WindowDC, 0, 0, bitmap1.Width, bitmap1.Height, drawingContext.BackgroundDC, 0, 0, bitmap1.Width, bitmap1.Height, drawingContext.Blend);
                NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap2.Handle);
                NativeMethods.AlphaBlend(drawingContext.WindowDC, 0, height, bitmap2.Width, bitmap2.Height, drawingContext.BackgroundDC, 0, 0, bitmap2.Width, bitmap2.Height, drawingContext.Blend);
                if (hDest > 0) {
                    NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap3.Handle);
                    NativeMethods.AlphaBlend(drawingContext.WindowDC, 0, yoriginDest1, bitmap3.Width, hDest, drawingContext.BackgroundDC, 0, 0, bitmap3.Width, bitmap3.Height, drawingContext.Blend);
                    }
                NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap4.Handle);
                NativeMethods.AlphaBlend(drawingContext.WindowDC, 0, yoriginDest3, bitmap4.Width, bitmap4.Height, drawingContext.BackgroundDC, 0, 0, bitmap4.Width, bitmap4.Height, drawingContext.Blend);
                NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap5.Handle);
                NativeMethods.AlphaBlend(drawingContext.WindowDC, 0, yoriginDest2, bitmap5.Width, bitmap5.Height, drawingContext.BackgroundDC, 0, 0, bitmap5.Width, bitmap5.Height, drawingContext.Blend);
                }

            private void DrawRight(GlowDrawingContext drawingContext) {
                var bitmap1 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.CornerTopRight);
                var bitmap2 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.RightTop);
                var bitmap3 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.Right);
                var bitmap4 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.RightBottom);
                var bitmap5 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.CornerBottomRight);
                var height = bitmap1.Height;
                var yoriginDest1 = height + bitmap2.Height;
                var yoriginDest2 = drawingContext.Height - bitmap5.Height;
                var yoriginDest3 = yoriginDest2 - bitmap4.Height;
                var hDest = yoriginDest3 - yoriginDest1;
                NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap1.Handle);
                NativeMethods.AlphaBlend(drawingContext.WindowDC, 0, 0, bitmap1.Width, bitmap1.Height, drawingContext.BackgroundDC, 0, 0, bitmap1.Width, bitmap1.Height, drawingContext.Blend);
                NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap2.Handle);
                NativeMethods.AlphaBlend(drawingContext.WindowDC, 0, height, bitmap2.Width, bitmap2.Height, drawingContext.BackgroundDC, 0, 0, bitmap2.Width, bitmap2.Height, drawingContext.Blend);
                if (hDest > 0) {
                    NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap3.Handle);
                    NativeMethods.AlphaBlend(drawingContext.WindowDC, 0, yoriginDest1, bitmap3.Width, hDest, drawingContext.BackgroundDC, 0, 0, bitmap3.Width, bitmap3.Height, drawingContext.Blend);
                    }
                NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap4.Handle);
                NativeMethods.AlphaBlend(drawingContext.WindowDC, 0, yoriginDest3, bitmap4.Width, bitmap4.Height, drawingContext.BackgroundDC, 0, 0, bitmap4.Width, bitmap4.Height, drawingContext.Blend);
                NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap5.Handle);
                NativeMethods.AlphaBlend(drawingContext.WindowDC, 0, yoriginDest2, bitmap5.Width, bitmap5.Height, drawingContext.BackgroundDC, 0, 0, bitmap5.Width, bitmap5.Height, drawingContext.Blend);
                }

            private void DrawTop(GlowDrawingContext drawingContext) {
                var bitmap1 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.TopLeft);
                var bitmap2 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.Top);
                var bitmap3 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.TopRight);
                var xoriginDest1 = 9;
                var xoriginDest2 = xoriginDest1 + bitmap1.Width;
                var xoriginDest3 = drawingContext.Width - 9 - bitmap3.Width;
                var wDest = xoriginDest3 - xoriginDest2;
                NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap1.Handle);
                NativeMethods.AlphaBlend(drawingContext.WindowDC, xoriginDest1, 0, bitmap1.Width, bitmap1.Height, drawingContext.BackgroundDC, 0, 0, bitmap1.Width, bitmap1.Height, drawingContext.Blend);
                if (wDest > 0) {
                    NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap2.Handle);
                    NativeMethods.AlphaBlend(drawingContext.WindowDC, xoriginDest2, 0, wDest, bitmap2.Height, drawingContext.BackgroundDC, 0, 0, bitmap2.Width, bitmap2.Height, drawingContext.Blend);
                    }
                NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap3.Handle);
                NativeMethods.AlphaBlend(drawingContext.WindowDC, xoriginDest3, 0, bitmap3.Width, bitmap3.Height, drawingContext.BackgroundDC, 0, 0, bitmap3.Width, bitmap3.Height, drawingContext.Blend);
                }

            private void DrawBottom(GlowDrawingContext drawingContext) {
                var bitmap1 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.BottomLeft);
                var bitmap2 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.Bottom);
                var bitmap3 = GetOrCreateBitmap(drawingContext, GlowBitmapPart.BottomRight);
                var xoriginDest1 = 9;
                var xoriginDest2 = xoriginDest1 + bitmap1.Width;
                var xoriginDest3 = drawingContext.Width - 9 - bitmap3.Width;
                var wDest = xoriginDest3 - xoriginDest2;
                NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap1.Handle);
                NativeMethods.AlphaBlend(drawingContext.WindowDC, xoriginDest1, 0, bitmap1.Width, bitmap1.Height, drawingContext.BackgroundDC, 0, 0, bitmap1.Width, bitmap1.Height, drawingContext.Blend);
                if (wDest > 0) {
                    NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap2.Handle);
                    NativeMethods.AlphaBlend(drawingContext.WindowDC, xoriginDest2, 0, wDest, bitmap2.Height, drawingContext.BackgroundDC, 0, 0, bitmap2.Width, bitmap2.Height, drawingContext.Blend);
                    }
                NativeMethods.SelectObject(drawingContext.BackgroundDC, bitmap3.Handle);
                NativeMethods.AlphaBlend(drawingContext.WindowDC, xoriginDest3, 0, bitmap3.Width, bitmap3.Height, drawingContext.BackgroundDC, 0, 0, bitmap3.Width, bitmap3.Height, drawingContext.Blend);
                }

            public void UpdateWindowPos() {
                var targetWindowHandle = TargetWindowHandle;
                RECT lpRect;
                NativeMethods.GetWindowRect(targetWindowHandle, out lpRect);
                NativeMethods.GetWindowPlacement(targetWindowHandle);
                if (!IsVisible)
                    return;
                switch (_orientation) {
                    case Dock.Left:
                    Left = lpRect.Left - 9;
                    Top = lpRect.Top - 9;
                    Width = 9;
                    Height = lpRect.Height + 18;
                    break;
                    case Dock.Top:
                    Left = lpRect.Left - 9;
                    Top = lpRect.Top - 9;
                    Width = lpRect.Width + 18;
                    Height = 9;
                    break;
                    case Dock.Right:
                    Left = lpRect.Right;
                    Top = lpRect.Top - 9;
                    Width = 9;
                    Height = lpRect.Height + 18;
                    break;
                    default:
                    Left = lpRect.Left - 9;
                    Top = lpRect.Bottom;
                    Width = lpRect.Width + 18;
                    Height = 9;
                    break;
                    }
                }

            [Flags]
            private enum FieldInvalidationTypes {
                None = 0,
                Location = 1,
                Size = 2,
                ActiveColor = 4,
                InactiveColor = 8,
                Render = 16,
                Visibility = 32,
                }
            }

        public static RoutedCommand CloseCommand    = new RoutedCommand("Close", typeof(CustomChromeWindow));
        public static RoutedCommand MinimizeCommand = new RoutedCommand("Minimize", typeof(CustomChromeWindow));
        public static RoutedCommand MaximizeCommand = new RoutedCommand("Maximize", typeof(CustomChromeWindow));
        public static RoutedCommand RestoreCommand  = new RoutedCommand("Restore", typeof(CustomChromeWindow));

        #region M:OnCanExecuteCommand(Object,CanExecuteRoutedEventArgs)
        protected internal virtual void OnCanExecuteCommand(Object sender, CanExecuteRoutedEventArgs e) {
            if (!e.Handled) {
                if (ReferenceEquals(e.Command, MinimizeCommand))
                    {
                    e.Handled = true;
                    e.CanExecute = WindowState != WindowState.Minimized;
                    }
                else if (ReferenceEquals(e.Command, MaximizeCommand))
                    {
                    e.Handled = true;
                    e.CanExecute = WindowState != WindowState.Maximized;
                    }
                else if (ReferenceEquals(e.Command, RestoreCommand))
                    {
                    e.Handled = true;
                    e.CanExecute = WindowState == WindowState.Maximized;
                    }
                else if (ReferenceEquals(e.Command, CloseCommand))
                    {
                    e.Handled = true;
                    e.CanExecute = true;
                    }
                }
            }
        #endregion
        #region M:OnExecutedCommand(ExecutedRoutedEventArgs)
        protected internal virtual void OnExecutedCommand(Object sender, ExecutedRoutedEventArgs e) {
            if (!e.Handled) {
                if (ReferenceEquals(e.Command, CloseCommand))
                    {
                    e.Handled = true;
                    Close();
                    }
                else if (ReferenceEquals(e.Command, MinimizeCommand))
                    {
                    e.Handled = true;
                    WindowState = WindowState.Minimized;
                    }
                else if (ReferenceEquals(e.Command, MaximizeCommand))
                    {
                    e.Handled = true;
                    WindowState = WindowState.Maximized;
                    }
                else if (ReferenceEquals(e.Command, RestoreCommand))
                    {
                    e.Handled = true;
                    WindowState = WindowState.Normal;
                    }
                }
            }
        #endregion
        }
    }
