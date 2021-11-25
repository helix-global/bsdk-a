using System;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    internal class DockAdornerWindowAutomationPeer : FrameworkElementAutomationPeer
        {
        internal DockAdornerWindowAutomationPeer(DockAdornerWindow owner)
          : base(owner)
            {
            }

        /// <summary>Gets the name of the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.</summary>
        /// <returns>An <see cref="F:System.String.Empty" /> string.</returns>
        protected override String GetClassNameCore()
            {
            return "DockAdornerWindow";
            }

        /// <summary>Gets the string that uniquely identifies the <see cref="T:System.Windows.FrameworkElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer" />. Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationId" />.</summary>
        /// <returns>The automation identifier for the element associated with the <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer" />, or <see cref="F:System.String.Empty" /> if there isn't an automation identifier.</returns>
        protected override String GetAutomationIdCore()
            {
            return "DockAdornerWindow_" + ((DockAdornerWindow)Owner).DockDirection;
            }
        }
    }