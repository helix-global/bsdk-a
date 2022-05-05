using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    [Guid("9BCD06C3-F4AE-4FA0-9B02-45211D7BD0D0")]
    public class ToolWindow : CustomChromeWindow
        {
        private Boolean IsDragging;
        private Point DraggingPoint;

        //private Tracker tracker;
        //private WS_EX_STYLES xstyles;
        //private WS_STYLES    nstyles;

        static ToolWindow()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolWindow), new FrameworkPropertyMetadata(typeof(ToolWindow)));
            }

        public ToolWindow()
            {
            //tracker = new Tracker(this);
            CommandManager.AddCanExecuteHandler(this, OnCanExecuteCommand);
            CommandManager.AddExecutedHandler(this, OnExecutedCommand);
            }

        //private sealed class Tracker : ComputerBasedTrainingTracker
        //    {
        //    private readonly ToolWindow Host;
        //    public Tracker(ToolWindow host)
        //        :base()
        //        {
        //        Host = host;
        //        EnsureHandle();
        //        }

        //    internal override unsafe void OnCreateWindow(IntPtr handle, CBT_CREATEWND* parameters)
        //        {
        //        base.OnCreateWindow(handle, parameters);
        //        var clss = Marshal.PtrToStringAnsi(parameters->cs->lpszClass);
        //        var name = Marshal.PtrToStringAnsi(parameters->cs->lpszName);
        //        if ((name == "Hidden Window") && (clss != null)) {
        //            if (clss.StartsWith("HwndWrapper[")) {
        //                //Host.nstyles = parameters->cs->style;
        //                //Host.xstyles = parameters->cs->ExStyle;
        //                //NativeMethods.SetWindowLongPtr(handle,NativeMethods.GWLP.GWL_STYLE,IntPtr.Zero);
        //                //NativeMethods.SetWindowLongPtr(handle, NativeMethods.GWLP.GWL_EXSTYLE, IntPtr.Zero);
        //                //parameters->cs->style = 0;
        //                ////parameters->cs->style &= ~(WS_STYLES.WS_THICKFRAME);
        //                ////parameters->cs->style &= ~(WS_STYLES.WS_MAXIMIZEBOX);
        //                ////parameters->cs->style &= ~(WS_STYLES.WS_SYSMENU);
        //                ////parameters->cs->style &= ~(WS_STYLES.WS_CAPTION);
        //                //parameters->cs->ExStyle = 0;
        //                }
        //            }
        //        }
        //    }

        /// <summary>Raises the <see cref="E:System.Windows.Window.SourceInitialized" /> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
            {
            //HwndSource.FromHwnd(new WindowInteropHelper(this).Handle).AddHook(HwndSourceHook);
            base.OnSourceInitialized(e);
            }

        //private IntPtr HwndSourceHook(IntPtr h, Int32 m, IntPtr w, IntPtr l, ref Boolean r)
        //    {
        //    return HwndSourceHook(h, (WindowMessage)m, w, l, ref r);
        //    }

        //protected void OnNcPaint(MessageEventArgs e)
        //    {
        //    e.Handled = true;
        //    }

        //protected void OnEraseBkgnd(MessageEventArgs e)
        //    {
        //    e.Handled = true;
        //    }

        //private unsafe void OnNcCalcSize(ref Boolean r, NCCALCSIZE_PARAMS* ncsize) {
        //    if (ncsize != null) {
        //        var flags = ncsize->Position->flags;
        //        }
        //    r = false;
        //    }

        //private unsafe IntPtr HwndSourceHook(IntPtr h, WindowMessage m, IntPtr w, IntPtr l, ref Boolean r) {
        //    switch (m) {
        //        case WindowMessage.WM_NCPAINT:
        //            {
        //            var e = new MessageEventArgs
        //                {
        //                Handled = false
        //                };
        //            OnNcPaint(e);
        //            r = e.Handled;
        //            }
        //            break;
        //        case WindowMessage.WM_ERASEBKGND:
        //            {
        //            var e = new MessageEventArgs
        //                {
        //                Handled = false
        //                };
        //            OnEraseBkgnd(e);
        //            r = e.Handled;
        //            }
        //            break;
        //        case WindowMessage.WM_NCCALCSIZE:
        //            {
        //            if (w == IntPtr.Zero) {
        //                return IntPtr.Zero;
        //                }
        //            else
        //                {
        //                OnNcCalcSize(ref r, (NCCALCSIZE_PARAMS*)l);
        //                }
        //            }
        //            break;
        //        case WindowMessage.WM_NULL:
        //        case WindowMessage.WM_CREATE:
        //        case WindowMessage.WM_DESTROY:
        //        case WindowMessage.WM_MOVE:
        //        case WindowMessage.WM_SIZE:
        //        case WindowMessage.WM_ACTIVATE:
        //        case WindowMessage.WM_SETFOCUS:
        //        case WindowMessage.WM_KILLFOCUS:
        //        case WindowMessage.WM_ENABLE:
        //        case WindowMessage.WM_SETREDRAW:
        //        case WindowMessage.WM_SETTEXT:
        //        case WindowMessage.WM_GETTEXT:
        //        case WindowMessage.WM_GETTEXTLENGTH:
        //        case WindowMessage.WM_PAINT:
        //        case WindowMessage.WM_CLOSE:
        //        case WindowMessage.WM_QUERYENDSESSION:
        //        case WindowMessage.WM_QUERYOPEN:
        //        case WindowMessage.WM_ENDSESSION:
        //        case WindowMessage.WM_QUIT:
        //        case WindowMessage.WM_SYSCOLORCHANGE:
        //        case WindowMessage.WM_SHOWWINDOW:
        //        case WindowMessage.WM_WININICHANGE:
        //        case WindowMessage.WM_DEVMODECHANGE:
        //        case WindowMessage.WM_ACTIVATEAPP:
        //        case WindowMessage.WM_FONTCHANGE:
        //        case WindowMessage.WM_TIMECHANGE:
        //        case WindowMessage.WM_CANCELMODE:
        //        case WindowMessage.WM_SETCURSOR:
        //        case WindowMessage.WM_MOUSEACTIVATE:
        //        case WindowMessage.WM_CHILDACTIVATE:
        //        case WindowMessage.WM_QUEUESYNC:
        //        case WindowMessage.WM_GETMINMAXINFO:
        //        case WindowMessage.WM_PAINTICON:
        //        case WindowMessage.WM_ICONERASEBKGND:
        //        case WindowMessage.WM_NEXTDLGCTL:
        //        case WindowMessage.WM_SPOOLERSTATUS:
        //        case WindowMessage.WM_DRAWITEM:
        //        case WindowMessage.WM_MEASUREITEM:
        //        case WindowMessage.WM_DELETEITEM:
        //        case WindowMessage.WM_VKEYTOITEM:
        //        case WindowMessage.WM_CHARTOITEM:
        //        case WindowMessage.WM_SETFONT:
        //        case WindowMessage.WM_GETFONT:
        //        case WindowMessage.WM_SETHOTKEY:
        //        case WindowMessage.WM_GETHOTKEY:
        //        case WindowMessage.WM_QUERYDRAGICON:
        //        case WindowMessage.WM_COMPAREITEM:
        //        case WindowMessage.WM_GETOBJECT:
        //        case WindowMessage.WM_COMPACTING:
        //        case WindowMessage.WM_COMMNOTIFY:
        //        case WindowMessage.WM_WINDOWPOSCHANGING:
        //        case WindowMessage.WM_WINDOWPOSCHANGED:
        //        case WindowMessage.WM_POWER:
        //        case WindowMessage.WM_COPYDATA:
        //        case WindowMessage.WM_CANCELJOURNAL:
        //        case WindowMessage.WM_NOTIFY:
        //        case WindowMessage.WM_INPUTLANGCHANGEREQUEST:
        //        case WindowMessage.WM_INPUTLANGCHANGE:
        //        case WindowMessage.WM_TCARD:
        //        case WindowMessage.WM_HELP:
        //        case WindowMessage.WM_USERCHANGED:
        //        case WindowMessage.WM_NOTIFYFORMAT:
        //        case WindowMessage.WM_CONTEXTMENU:
        //        case WindowMessage.WM_STYLECHANGING:
        //        case WindowMessage.WM_STYLECHANGED:
        //        case WindowMessage.WM_DISPLAYCHANGE:
        //        case WindowMessage.WM_GETICON:
        //        case WindowMessage.WM_SETICON:
        //        case WindowMessage.WM_NCCREATE:
        //        case WindowMessage.WM_NCDESTROY:
        //        case WindowMessage.WM_NCHITTEST:
        //        case WindowMessage.WM_NCACTIVATE:
        //        case WindowMessage.WM_GETDLGCODE:
        //        case WindowMessage.WM_SYNCPAINT:
        //        case WindowMessage.WM_NCMOUSEMOVE:
        //        case WindowMessage.WM_NCLBUTTONDOWN:
        //        case WindowMessage.WM_NCLBUTTONUP:
        //        case WindowMessage.WM_NCLBUTTONDBLCLK:
        //        case WindowMessage.WM_NCRBUTTONDOWN:
        //        case WindowMessage.WM_NCRBUTTONUP:
        //        case WindowMessage.WM_NCRBUTTONDBLCLK:
        //        case WindowMessage.WM_NCMBUTTONDOWN:
        //        case WindowMessage.WM_NCMBUTTONUP:
        //        case WindowMessage.WM_NCMBUTTONDBLCLK:
        //        case WindowMessage.WM_NCXBUTTONDOWN:
        //        case WindowMessage.WM_NCXBUTTONUP:
        //        case WindowMessage.WM_NCXBUTTONDBLCLK:
        //        case WindowMessage.WM_INPUT_DEVICE_CHANGE:
        //        case WindowMessage.WM_INPUT:
        //        case WindowMessage.WM_KEYFIRST:
        //        case WindowMessage.WM_KEYUP:
        //        case WindowMessage.WM_CHAR:
        //        case WindowMessage.WM_DEADCHAR:
        //        case WindowMessage.WM_SYSKEYDOWN:
        //        case WindowMessage.WM_SYSKEYUP:
        //        case WindowMessage.WM_SYSCHAR:
        //        case WindowMessage.WM_SYSDEADCHAR:
        //        case WindowMessage.WM_UNICHAR:
        //        case WindowMessage.WM_IME_STARTCOMPOSITION:
        //        case WindowMessage.WM_IME_ENDCOMPOSITION:
        //        case WindowMessage.WM_IME_COMPOSITION:
        //        case WindowMessage.WM_INITDIALOG:
        //        case WindowMessage.WM_COMMAND:
        //        case WindowMessage.WM_SYSCOMMAND:
        //        case WindowMessage.WM_TIMER:
        //        case WindowMessage.WM_HSCROLL:
        //        case WindowMessage.WM_VSCROLL:
        //        case WindowMessage.WM_INITMENU:
        //        case WindowMessage.WM_INITMENUPOPUP:
        //        case WindowMessage.WM_GESTURE:
        //        case WindowMessage.WM_GESTURENOTIFY:
        //        case WindowMessage.WM_MENUSELECT:
        //        case WindowMessage.WM_MENUCHAR:
        //        case WindowMessage.WM_ENTERIDLE:
        //        case WindowMessage.WM_MENURBUTTONUP:
        //        case WindowMessage.WM_MENUDRAG:
        //        case WindowMessage.WM_MENUGETOBJECT:
        //        case WindowMessage.WM_UNINITMENUPOPUP:
        //        case WindowMessage.WM_MENUCOMMAND:
        //        case WindowMessage.WM_CHANGEUISTATE:
        //        case WindowMessage.WM_UPDATEUISTATE:
        //        case WindowMessage.WM_QUERYUISTATE:
        //        case WindowMessage.WM_CTLCOLORMSGBOX:
        //        case WindowMessage.WM_CTLCOLOREDIT:
        //        case WindowMessage.WM_CTLCOLORLISTBOX:
        //        case WindowMessage.WM_CTLCOLORBTN:
        //        case WindowMessage.WM_CTLCOLORDLG:
        //        case WindowMessage.WM_CTLCOLORSCROLLBAR:
        //        case WindowMessage.WM_CTLCOLORSTATIC:
        //        case WindowMessage.MN_GETHMENU:
        //        case WindowMessage.WM_MOUSEFIRST:
        //        case WindowMessage.WM_LBUTTONDOWN:
        //        case WindowMessage.WM_LBUTTONUP:
        //        case WindowMessage.WM_LBUTTONDBLCLK:
        //        case WindowMessage.WM_RBUTTONDOWN:
        //        case WindowMessage.WM_RBUTTONUP:
        //        case WindowMessage.WM_RBUTTONDBLCLK:
        //        case WindowMessage.WM_MBUTTONDOWN:
        //        case WindowMessage.WM_MBUTTONUP:
        //        case WindowMessage.WM_MBUTTONDBLCLK:
        //        case WindowMessage.WM_MOUSEWHEEL:
        //        case WindowMessage.WM_XBUTTONDOWN:
        //        case WindowMessage.WM_XBUTTONUP:
        //        case WindowMessage.WM_XBUTTONDBLCLK:
        //        case WindowMessage.WM_MOUSEHWHEEL:
        //        case WindowMessage.WM_PARENTNOTIFY:
        //        case WindowMessage.WM_ENTERMENULOOP:
        //        case WindowMessage.WM_EXITMENULOOP:
        //        case WindowMessage.WM_NEXTMENU:
        //        case WindowMessage.WM_SIZING:
        //        case WindowMessage.WM_CAPTURECHANGED:
        //        case WindowMessage.WM_MOVING:
        //        case WindowMessage.WM_POWERBROADCAST:
        //        case WindowMessage.WM_DEVICECHANGE:
        //        case WindowMessage.WM_MDICREATE:
        //        case WindowMessage.WM_MDIDESTROY:
        //        case WindowMessage.WM_MDIACTIVATE:
        //        case WindowMessage.WM_MDIRESTORE:
        //        case WindowMessage.WM_MDINEXT:
        //        case WindowMessage.WM_MDIMAXIMIZE:
        //        case WindowMessage.WM_MDITILE:
        //        case WindowMessage.WM_MDICASCADE:
        //        case WindowMessage.WM_MDIICONARRANGE:
        //        case WindowMessage.WM_MDIGETACTIVE:
        //        case WindowMessage.WM_MDISETMENU:
        //        case WindowMessage.WM_ENTERSIZEMOVE:
        //        case WindowMessage.WM_EXITSIZEMOVE:
        //        case WindowMessage.WM_DROPFILES:
        //        case WindowMessage.WM_MDIREFRESHMENU:
        //        case WindowMessage.WM_POINTERDEVICECHANGE:
        //        case WindowMessage.WM_POINTERDEVICEINRANGE:
        //        case WindowMessage.WM_POINTERDEVICEOUTOFRANGE:
        //        case WindowMessage.WM_TOUCH:
        //        case WindowMessage.WM_NCPOINTERUPDATE:
        //        case WindowMessage.WM_NCPOINTERDOWN:
        //        case WindowMessage.WM_NCPOINTERUP:
        //        case WindowMessage.WM_POINTERUPDATE:
        //        case WindowMessage.WM_POINTERDOWN:
        //        case WindowMessage.WM_POINTERUP:
        //        case WindowMessage.WM_POINTERENTER:
        //        case WindowMessage.WM_POINTERLEAVE:
        //        case WindowMessage.WM_POINTERACTIVATE:
        //        case WindowMessage.WM_POINTERCAPTURECHANGED:
        //        case WindowMessage.WM_TOUCHHITTESTING:
        //        case WindowMessage.WM_POINTERWHEEL:
        //        case WindowMessage.WM_POINTERHWHEEL:
        //        case WindowMessage.DM_POINTERHITTEST:
        //        case WindowMessage.WM_POINTERROUTEDTO:
        //        case WindowMessage.WM_POINTERROUTEDAWAY:
        //        case WindowMessage.WM_POINTERROUTEDRELEASED:
        //        case WindowMessage.WM_IME_SETCONTEXT:
        //        case WindowMessage.WM_IME_NOTIFY:
        //        case WindowMessage.WM_IME_CONTROL:
        //        case WindowMessage.WM_IME_COMPOSITIONFULL:
        //        case WindowMessage.WM_IME_SELECT:
        //        case WindowMessage.WM_IME_CHAR:
        //        case WindowMessage.WM_IME_REQUEST:
        //        case WindowMessage.WM_IME_KEYDOWN:
        //        case WindowMessage.WM_IME_KEYUP:
        //        case WindowMessage.WM_MOUSEHOVER:
        //        case WindowMessage.WM_MOUSELEAVE:
        //        case WindowMessage.WM_NCMOUSEHOVER:
        //        case WindowMessage.WM_NCMOUSELEAVE:
        //        case WindowMessage.WM_WTSSESSION_CHANGE:
        //        case WindowMessage.WM_TABLET_FIRST:
        //        case WindowMessage.WM_TABLET_LAST:
        //        case WindowMessage.WM_DPICHANGED:
        //        case WindowMessage.WM_DPICHANGED_BEFOREPARENT:
        //        case WindowMessage.WM_DPICHANGED_AFTERPARENT:
        //        case WindowMessage.WM_GETDPISCALEDSIZE:
        //        case WindowMessage.WM_CUT:
        //        case WindowMessage.WM_COPY:
        //        case WindowMessage.WM_PASTE:
        //        case WindowMessage.WM_CLEAR:
        //        case WindowMessage.WM_UNDO:
        //        case WindowMessage.WM_RENDERFORMAT:
        //        case WindowMessage.WM_RENDERALLFORMATS:
        //        case WindowMessage.WM_DESTROYCLIPBOARD:
        //        case WindowMessage.WM_DRAWCLIPBOARD:
        //        case WindowMessage.WM_PAINTCLIPBOARD:
        //        case WindowMessage.WM_VSCROLLCLIPBOARD:
        //        case WindowMessage.WM_SIZECLIPBOARD:
        //        case WindowMessage.WM_ASKCBFORMATNAME:
        //        case WindowMessage.WM_CHANGECBCHAIN:
        //        case WindowMessage.WM_HSCROLLCLIPBOARD:
        //        case WindowMessage.WM_QUERYNEWPALETTE:
        //        case WindowMessage.WM_PALETTEISCHANGING:
        //        case WindowMessage.WM_PALETTECHANGED:
        //        case WindowMessage.WM_HOTKEY:
        //        case WindowMessage.WM_PRINT:
        //        case WindowMessage.WM_PRINTCLIENT:
        //        case WindowMessage.WM_APPCOMMAND:
        //        case WindowMessage.WM_THEMECHANGED:
        //        case WindowMessage.WM_CLIPBOARDUPDATE:
        //        case WindowMessage.WM_DWMCOMPOSITIONCHANGED:
        //        case WindowMessage.WM_DWMNCRENDERINGCHANGED:
        //        case WindowMessage.WM_DWMCOLORIZATIONCOLORCHANGED:
        //        case WindowMessage.WM_DWMWINDOWMAXIMIZEDCHANGE:
        //        case WindowMessage.WM_DWMSENDICONICTHUMBNAIL:
        //        case WindowMessage.WM_DWMSENDICONICLIVEPREVIEWBITMAP:
        //        case WindowMessage.WM_GETTITLEBARINFOEX:
        //        case WindowMessage.WM_HANDHELDFIRST:
        //        case WindowMessage.WM_HANDHELDLAST:
        //        case WindowMessage.WM_AFXFIRST:
        //        case WindowMessage.WM_AFXLAST:
        //        case WindowMessage.WM_PENWINFIRST:
        //        case WindowMessage.WM_PENWINLAST:
        //        case WindowMessage.WM_APP:
        //        case WindowMessage.WM_USER:
        //        default:
        //            {
        //            Debug.Print($"HwndSourceHook:{m}");
        //            }
        //            break;
        //        }
        //    return IntPtr.Zero;
        //}
        #region M:OnCanExecuteCommand(Object,CanExecuteRoutedEventArgs)
        protected internal void OnCanExecuteCommand(Object sender, CanExecuteRoutedEventArgs e) {
            if (!e.Handled) {
                if (ReferenceEquals(e.Command, ApplicationCommands.Close)) {
                    e.Handled = true;
                    e.CanExecute = true;
                    }
                }
            }
        #endregion
        #region M:OnExecutedCommand(ExecutedRoutedEventArgs)
        protected internal void OnExecutedCommand(Object sender, ExecutedRoutedEventArgs e) {
            if (!e.Handled) {
                if (ReferenceEquals(e.Command, ApplicationCommands.Close)) {
                    e.Handled = true;
                    Close();
                    }
                }
            }
        #endregion
        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonDown" /> routed event is raised on this element. Implement this method to add class handling for this event. </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
            {
            //base.OnMouseLeftButtonDown(e);
            //IsDragging = true;
            //DraggingPoint = e.GetPosition(this);
            //CaptureMouse();
            }

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.UIElement.MouseLeftButtonUp" /> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
            {
            //base.OnMouseLeftButtonUp(e);
            //IsDragging = false;
            //ReleaseMouseCapture();
            //Opacity = 1;
            }

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseMove" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs" /> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
            {
            base.OnMouseMove(e);
            if (IsDragging)
                {
                var pt = e.GetPosition(this);
                var dX = DraggingPoint.X - pt.X;
                var dY = DraggingPoint.Y - pt.Y;
                Left -= dX;
                Top  -= dY;
                Opacity = 0.8;
                }
            }
        }
    }