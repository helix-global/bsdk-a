using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Win32 {
    public sealed class FocusTracker : ComputerBasedTrainingTracker {
        #region P:Instance:FocusTracker
        [ThreadStatic]
        private static FocusTracker instance;
        public static FocusTracker Instance { get {
            return instance ?? (instance = new FocusTracker());
            }}
        #endregion

        private event EventHandler<TrackFocusEventArgs> E;
        public event EventHandler<TrackFocusEventArgs> TrackFocus {
            [MethodImpl(MethodImplOptions.Synchronized)]
            add
                {
                E += value;
                EnsureHandle();
                }
            [MethodImpl(MethodImplOptions.Synchronized)]
            remove
                {
                E -= value;
                TryDestroyHandle();
                }
            }

        private FocusTracker()
            {
            }

        #region M:OnKeyboardFocusReceived(IntPtr,IntPtr)
        protected override void OnKeyboardFocusReceived(IntPtr gainFocusHandle, IntPtr loseFocusHandle) {
            RaiseTrackFocusEvent(new TrackFocusEventArgs(gainFocusHandle, loseFocusHandle));
            }
        #endregion
        #region M:RaiseTrackFocusEvent(TrackFocusEventArgs)
        private void RaiseTrackFocusEvent(TrackFocusEventArgs e) {
            var trackFocusDelegate = E;
            if (trackFocusDelegate == null) { return; }
            trackFocusDelegate(this, e);
            }
        #endregion
        #region M:TryDestroyHandle
        protected override void TryDestroyHandle() {
            if (E == null) {
                base.TryDestroyHandle();
                }
            }
        #endregion
        }
    }