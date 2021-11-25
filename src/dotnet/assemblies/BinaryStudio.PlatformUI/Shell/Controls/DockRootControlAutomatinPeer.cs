using System;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    internal class DockRootControlAutomatinPeer : FrameworkElementAutomationPeer
        {
        internal DockRootControlAutomatinPeer(DockRootControl dockRoot)
          : base(dockRoot)
            {
            }

        /// <summary>Gets the control type for the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType" />.</summary>
        /// <returns>The <see cref="F:System.Windows.Automation.Peers.AutomationControlType.Custom" /> enumeration value.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            {
            return AutomationControlType.Pane;
            }

        /// <summary>Gets the name of the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.</summary>
        /// <returns>An <see cref="F:System.String.Empty" /> string.</returns>
        protected override String GetClassNameCore()
            {
            return "DockRoot";
            }
        }
    }