using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public abstract class FocusableHwndHost : HwndHost, IKeyboardInputSink
        {
        private Boolean IsTakingFrameworkFocus;
        private Boolean IsTakingNativeFocus;
        private DispatcherOperation PendingSetFocusOperation;
        private HwndSource HwndSource;

        internal IntPtr LastFocusedHwnd { get; private set; }

        #region P:CanFocus:Boolean
        private Boolean CanFocus { get {
            return NativeMethods.IsWindowVisible(Handle) & NativeMethods.IsWindowEnabled(Handle);
            }}
        #endregion

        protected FocusableHwndHost() {
            FocusTracker.Instance.TrackFocus += OnTrackFocus;
            PresentationSource.AddSourceChangedHandler(this, OnSourceChanged);
            InputMethod.SetIsInputMethodSuspended(this, true);
            }

        #region M:Dispose(Boolean)
        /// <summary>
        /// Immediately frees any system resources that the object might hold.
        /// </summary>
        /// <param name="disposing">Set to true if called from an explicit disposer and false otherwise.</param>
        protected override void Dispose(Boolean disposing) {
            if (disposing) {
                FocusTracker.Instance.TrackFocus -= OnTrackFocus;
                PresentationSource.RemoveSourceChangedHandler(this, OnSourceChanged);
                var sink = (IKeyboardInputSink)this;
                if (sink.KeyboardInputSite != null) {
                    sink.KeyboardInputSite.Unregister();
                    }
                }
            base.Dispose(disposing);
            }
        #endregion
        #region M:HitTestCore(PointHitTestParameters):HitTestResult
        /// <summary> Implements <see cref="M:System.Windows.Media.Visual.HitTestCore(System.Windows.Media.PointHitTestParameters)" /> to supply base element hit testing behavior (returning <see cref="T:System.Windows.Media.HitTestResult" />). </summary>
        /// <returns>Results of the test, including the evaluated point.</returns>
        /// <param name="hitTestParameters">Describes the hit test to perform, including the initial hit point.</param>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) {
            return (IsArrangeValid && (new Rect(0.0, 0.0, ActualWidth, ActualHeight)).Contains(hitTestParameters.HitPoint))
                ? new PointHitTestResult(this, hitTestParameters.HitPoint)
                : base.HitTestCore(hitTestParameters);
            }
        #endregion
        #region M:IsSelfOrDescendentWindow(IntPtr):Boolean
        private Boolean IsSelfOrDescendentWindow(IntPtr window) {
            return IsSelfOrDescendentWindow(Handle, window);
            }
        #endregion
        #region M:IsSelfOrDescendentWindow(IntPtr,IntPtr):Boolean
        internal static Boolean IsSelfOrDescendentWindow(IntPtr source, IntPtr window) {
            if (!(source != IntPtr.Zero) || !(window != IntPtr.Zero)) { return false; }
            return (source == window) || IsChild(source, window);
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
            if (IsSelfOrDescendentWindow(e.HwndGainFocus)) {
                if (!IsTakingNativeFocus && !Equals(Keyboard.FocusedElement, this) &&
                    !IsSelfOrDescendentWindow(e.HwndLoseFocus)) {
                    SetFocusToHwndHost(false, e.HwndGainFocus);
                    }
                }
            else
                {
                if ((e.HwndGainFocus != IntPtr.Zero) &&
                    IsSelfOrDescendentWindow(e.HwndLoseFocus) &&
                    !IsTakingFrameworkFocus && (HwndSource != null) &&
                    (e.HwndGainFocus == HwndSource.Handle) &&
                    (HwndSource.RootVisual != null)) {
                    FocusManager.SetFocusedElement(HwndSource.RootVisual, null);
                    }
                }
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
        #region M:IKeyboardInputSink.TabInto(TraversalRequest):Boolean
        Boolean IKeyboardInputSink.TabInto(TraversalRequest request) {
            SetFocusToHwndHost(true, IntPtr.Zero);
            return true;
            }
        #endregion

        [DllImport("user32.dll")] private static extern IntPtr SetFocus(IntPtr window);
        [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)] private static extern Boolean IsChild(IntPtr parent, IntPtr window);
        [DllImport("user32.dll")] private static extern IntPtr GetFocus();
        }
    }