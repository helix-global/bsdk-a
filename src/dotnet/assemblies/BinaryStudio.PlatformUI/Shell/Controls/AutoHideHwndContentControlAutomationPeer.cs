using System;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    internal class AutoHideHwndContentControlAutomationPeer : HwndContentControlAutomationPeer
        {
        internal AutoHideHwndContentControlAutomationPeer(HwndContentControl owner, Visual visualSubtree)
          : base(owner, visualSubtree)
            {
            }

        /// <summary>Gets the name of the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.</summary>
        /// <returns>An <see cref="F:System.String.Empty" /> string.</returns>
        protected override String GetClassNameCore()
            {
            return "AutoHideWindowContentControl";
            }
        }
    }