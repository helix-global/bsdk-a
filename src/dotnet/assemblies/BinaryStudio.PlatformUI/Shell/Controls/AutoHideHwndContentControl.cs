using System;
using System.Windows;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class AutoHideHwndContentControl : PositionManagedHwndContentControl
        {
        private Rect oldRect;

        protected override Int32 ZIndex {
            get
                {
                return 1;
                }
            }

        /// <summary>Creates an <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> for <see cref="T:System.Windows.Interop.HwndHost" /> . </summary>
        /// <returns>The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementation. </returns>
        protected override AutomationPeer OnCreateAutomationPeer() {
            return new AutoHideHwndContentControlAutomationPeer(this, HwndSourcePresenter);
            }

        /// <summary> Called when the hosted window's position changes. </summary>
        /// <param name="rc">The window's position.</param>
        protected override void OnWindowPositionChanged(Rect rc) {
            base.OnWindowPositionChanged(rc);
            if (!rc.Equals(oldRect)) {
                oldRect = rc;
                var e = Content as UIElement;
                if (e != null) {
                    e.InvalidateMeasure();
                    }
                }
            }
        }
    }