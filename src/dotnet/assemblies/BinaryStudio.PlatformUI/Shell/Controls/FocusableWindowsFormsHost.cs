using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
        internal class FocusableWindowsFormsHost : WindowsFormsHost, IInputElement {
        public FocusableWindowsFormsHost(HwndSource topLevelHwndSource){
            TopLevelHwndSource = topLevelHwndSource;
            PresentationSource.AddSourceChangedHandler(this, OnSourceChanged);
            }

        internal IntPtr LastFocusedHwnd { get; private set; }
        #region P:CanFocus:Boolean
        private Boolean CanFocus { get {
            return NativeMethods.IsWindowVisible(Handle) & NativeMethods.IsWindowEnabled(Handle);
            }}
        #endregion

        #region M:HitTestCore(PointHitTestParameters):HitTestResult
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) {
            return (IsArrangeValid && (new Rect(0.0, 0.0, ActualWidth, ActualHeight)).Contains(hitTestParameters.HitPoint))
                ? new PointHitTestResult(this, hitTestParameters.HitPoint)
                : base.HitTestCore(hitTestParameters);
            }
        #endregion
        #region M:OnGotKeyboardFocus(KeyboardFocusChangedEventArgs)
        /// <summary>
        /// Invoked when an unhandled <see cref="E:Keyboard.GotKeyboardFocus"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            base.OnGotKeyboardFocus(e);
            if (e.Handled || !Equals(e.NewFocus, this)) { return; }
            if (IsTakingFrameworkFocus) { return; }
            if (CanFocus) {
                IsTakingNativeFocus = true;
                SetFocus(Handle);
                IsTakingNativeFocus = false;
                }
            else if (PendingSetFocusOperation == null)
                {
                PendingSetFocusOperation = SchedulePendingSetFocusOperation(PendingFocusHelper.FocusPriority);
                }
            e.Handled = true;
            }
        #endregion
        #region M:OnSourceChanged(Object,SourceChangedEventArgs)
        private void OnSourceChanged(Object sender, SourceChangedEventArgs e) {
            HwndSource = e.NewSource as HwndSource;
            }
        #endregion
        #region M:OnTrackFocus(Object,TrackFocusEventArgs)
        private void OnTrackFocus(Object sender, TrackFocusEventArgs e) {
            if (FocusableHwndHost.IsSelfOrDescendentWindow(Handle, e.HwndGainFocus)) {
                if (!IsTakingNativeFocus && !Equals(Keyboard.FocusedElement, this) &&
                    !FocusableHwndHost.IsSelfOrDescendentWindow(Handle, e.HwndLoseFocus)) {
                    SetFocusToHwndHost(false, e.HwndGainFocus);
                    }
                }
            else
                {
                if (FocusableHwndHost.IsSelfOrDescendentWindow(Handle, e.HwndLoseFocus)) {
                    Debug.Print(@"FocusableWindowsFormsHost.OnTrackFocus:Gain=""{0}"",Lose=""{1}""", ToString(e.HwndGainFocus), ToString(e.HwndLoseFocus));
                    if ((e.HwndGainFocus != IntPtr.Zero) && !IsTakingFrameworkFocus) {
                        if ((HwndSource != null) && (e.HwndGainFocus == HwndSource.Handle) && (HwndSource.RootVisual != null)) {
                            FocusManager.SetFocusedElement(HwndSource.RootVisual, null);
                            }
                        else
                            {
                            RaiseEvent(new RoutedEventArgs(LostFocusEvent,this));
                            }
                        }
                    }
                }
            }
        #endregion
        #region M:Dispose(Boolean)
        protected override void Dispose(Boolean disposing) {
            if (disposing) {
                FocusTracker.Instance.TrackFocus -= OnTrackFocus;
                PresentationSource.RemoveSourceChangedHandler(this, OnSourceChanged);
                }
            base.Dispose(disposing);
            }
        #endregion
        #region M:SchedulePendingSetFocusOperation(DispatcherPriority):DispatcherOperation
        private DispatcherOperation SchedulePendingSetFocusOperation(DispatcherPriority priority) {
            return Dispatcher.BeginInvoke(priority, new DispatcherOperationCallback(delegate {
                DispatcherOperation pendingSetFocusOperation = null;
                if (Equals(Keyboard.FocusedElement, this)) {
                    if (CanFocus) {
                        IsTakingNativeFocus = true;
                        SetFocus(Handle);
                        IsTakingNativeFocus = false;
                        }
                    else
                        {
                        pendingSetFocusOperation = SchedulePendingSetFocusOperation(DispatcherPriority.Input);
                        }
                    }
                PendingSetFocusOperation = pendingSetFocusOperation;
                return null;
                }));
            }
        #endregion
        #region M:SetFocusToHwndHost(Boolean,IntPtr)
        private void SetFocusToHwndHost(Boolean allowFocusToDelegateToHostedWindow, IntPtr hwndGainFocus) {
            Debug.Print("FocusableWindowsFormsHost.SetFocusToHwndHost");
            if ((HwndSource) != null && (HwndSource.RootVisual != null) && (GetFocus() != HwndSource.Handle)) {
                FocusManager.SetFocusedElement(HwndSource.RootVisual, null);
                }
            IsTakingFrameworkFocus = !allowFocusToDelegateToHostedWindow;
            LastFocusedHwnd = hwndGainFocus;
            Keyboard.Focus(this);
            LastFocusedHwnd = IntPtr.Zero;
            IsTakingFrameworkFocus = false;
            }
        #endregion
        #region M:WndProc(IntPtr,Int32,IntPtr,IntPtr,Boolean):IntPtr
        protected override IntPtr WndProc(IntPtr hwnd, Int32 msg, IntPtr W, IntPtr L, ref Boolean handled) {
            if (msg == WM_SHOWWINDOW) {
                if (W == IntPtr.Zero)
                    {
                    FocusTracker.Instance.TrackFocus -= OnTrackFocus;
                    }
                else
                    {
                    FocusTracker.Instance.TrackFocus += OnTrackFocus;
                    }
                }
            return base.WndProc(hwnd, msg, W, L, ref handled);
            }
        #endregion

        //protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
        //    var r = base.BuildWindowCore(hwndParent);
        //    NativeMethods.SetParent(r.Handle, TopLevelHwndSource.Handle);
        //    return r;
        //    }

        private static String ToString(IntPtr handle) {
            if (handle != IntPtr.Zero) {
                var builder = new StringBuilder();
                builder.AppendFormat("{0:X8}", (UInt64)handle);
                builder.AppendFormat(@" ""{0}""", NativeMethods.GetWindowText(handle));
                builder.AppendFormat(@" {{{0}}}", NativeMethods.GetClassName(handle));
                return builder.ToString();
                }
            return null;
            }

        public event EventHandler PreviewKeyboardFocusLost;
        private Boolean IsTakingFrameworkFocus;
        private Boolean IsTakingNativeFocus;
        private HwndSource HwndSource;
        private HwndSource TopLevelHwndSource;
        private DispatcherOperation PendingSetFocusOperation;

        private const Int32 WM_SHOWWINDOW = 0x0018;
        [DllImport("user32.dll")] private static extern IntPtr GetFocus();
        [DllImport("user32.dll")] private static extern IntPtr SetFocus(IntPtr window);
        }
    }