using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Markup;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    [DefaultProperty("Content")]
    [ContentProperty("Content")]
    public class HwndContentControl : FocusableHwndHost
        {
        #region P:Content:Object
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(Object), typeof(HwndContentControl), new PropertyMetadata(default(Object), OnContentChanged));
        public Object Content {
            get { return (Object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
            }
        #endregion
        public HwndSource HwndSource { get; private set; }
        protected ContentPresenter HwndSourcePresenter { get; }

        public HwndContentControl() {
            HwndSourcePresenter = new ContentPresenter();
            PresentationSource.AddSourceChangedHandler(this, OnSourceChanged);
            }

        #region M:BuildWindowCore(HandleRef):HandleRef
        /// <summary>When overridden in a derived class, creates the window to be hosted. </summary>
        /// <returns>The handle to the child Win32 window to create.</returns>
        /// <param name="hwndParent">The window handle of the parent window.</param>
        protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
            HwndSource = CreateHwndSource(hwndParent);
            return new HandleRef(this, HwndSource.Handle);
            }
        #endregion
        #region M:DestroyWindowCore(HandleRef)
        /// <summary>When overridden in a derived class, destroys the hosted window. </summary>
        /// <param name="hwnd">A structure that contains the window handle.</param>
        protected override void DestroyWindowCore(HandleRef hwnd) {
            if (HwndSource != null) {
                HwndSource.Dispose();
                HwndSource = null;
                }
            }
        #endregion
        #region M:WndProc(IntPtr,Int32,IntPtr,IntPtr,Boolean):IntPtr
        /// <summary>When overridden in a derived class, accesses the window process (handle) of the hosted child window. </summary>
        /// <returns>The window handle of the child window.</returns>
        /// <param name="hwnd">The window handle of the hosted window.</param>
        /// <param name="msg">The message to act upon.</param>
        /// <param name="W">Information that may be relevant to handling the message. This is typically used to store small pieces of information, such as flags.</param>
        /// <param name="L">Information that may be relevant to handling the message. This is typically used to reference an object.</param>
        /// <param name="handled">Whether events resulting should be marked handled.</param>
        protected override IntPtr WndProc(IntPtr hwnd, Int32 msg, IntPtr W, IntPtr L, ref Boolean handled) {
            if (msg != WM_GETOBJECT) { return base.WndProc(hwnd, msg, W, L, ref handled); }
            handled = true;
            var peer = UIElementAutomationPeer.CreatePeerForElement(this) as HwndContentControlAutomationPeer;
            if (peer == null) { return IntPtr.Zero; }
            var provider = peer.GetProvider();
            return (provider != null)
                ? AutomationInteropProvider.ReturnRawElementProvider(hwnd, W, L, provider)
                : IntPtr.Zero;
            }
        #endregion
        #region M:CreateHwndSource(HandleRef):HwndSource
        private HwndSource CreateHwndSource(HandleRef parent) {
            var r = new HwndSource(new HwndSourceParameters {
                Width = 0,
                Height = 0,
                WindowStyle = WS_CHILDWINDOW | WS_CLIPCHILDREN | WS_CLIPSIBLINGS,
                ParentWindow = parent.Handle
                });
            r.RootVisual = HwndSourcePresenter;
            AddLogicalChild(HwndSourcePresenter);
            BringWindowToTop(r.Handle);
            return r;
            }
        #endregion
        #region M:OnCreateAutomationPeer:AutomationPeer
        /// <summary>Creates an <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> for <see cref="T:System.Windows.Interop.HwndHost" /> . </summary>
        /// <returns>The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementation. </returns>
        protected override AutomationPeer OnCreateAutomationPeer() {
            return new HwndContentControlAutomationPeer(this, HwndSourcePresenter);
            }
        #endregion
        #region M:OnSourceChanged(Object,SourceChangedEventArgs)
        private void OnSourceChanged(Object sender, SourceChangedEventArgs e) {
            HwndSourcePresenter.Visibility = (e.NewSource == null)
                ? Visibility.Collapsed
                : Visibility.Visible;
            }
        #endregion
        #region M:OnContentChanged(DependencyObject,DependencyPropertyChangedEventArgs)
        private static void OnContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (HwndContentControl)sender;
            source.HwndSourcePresenter.Content = e.NewValue;
            LayoutSynchronizer.Update(source.HwndSourcePresenter);
            }
        #endregion

        #region M:Dispose(Boolean)
        /// <summary>Immediately frees any system resources that the object might hold.</summary>
        /// <param name="disposing">Set to true if called from an explicit disposer and false otherwise.</param>
        protected override void Dispose(Boolean disposing) {
            if (disposing && (HwndSource != null)) {
                HwndSource.Dispose();
                HwndSource = null;
                }
            base.Dispose(disposing);
            }
        #endregion

        private const Int32 WM_GETOBJECT = 0x3D;
        private const Int32 WS_CHILDWINDOW  = 0x40000000;
        private const Int32 WS_CLIPCHILDREN = 0x02000000;
        private const Int32 WS_CLIPSIBLINGS = 0x04000000;

        /// <summary>
        /// Brings the specified window to the top of the Z order. If the window is a top-level window, it is activated. If the window is a child window, the top-level parent window associated with the child window is activated.
        /// </summary>
        /// <param name="window">A handle to the window to bring to the top of the Z order.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.</returns>
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern Boolean BringWindowToTop(IntPtr window);
        }
    }