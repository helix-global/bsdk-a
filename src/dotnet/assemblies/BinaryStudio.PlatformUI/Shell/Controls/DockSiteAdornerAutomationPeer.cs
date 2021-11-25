using System;
using System.Windows.Automation.Peers;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DockSiteAdornerAutomationPeer : FrameworkElementAutomationPeer
        {
        internal static readonly String DocumentAutomationName = "Document";
        internal static readonly String MainAutomationName = "Main";

        public DockSiteAdornerAutomationPeer(DockSiteAdorner adorner)
          : base(adorner)
            {
            }

        /// <summary>Gets the name of the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.</summary>
        /// <returns>An <see cref="F:System.String.Empty" /> string.</returns>
        protected override String GetClassNameCore()
            {
            return "DockSiteAdorner";
            }

        /// <summary>Gets the text label of the <see cref="T:System.Windows.ContentElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.ContentElementAutomationPeer" />. Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetName" />.</summary>
        /// <returns>The text label of the element that is associated with this automation peer.</returns>
        protected override String GetNameCore()
            {
            var owner = (DockSiteAdorner)Owner;
            return owner.AdornedDockTarget == null || owner.AdornedDockTarget.TargetElement == null ? MainAutomationName : (owner.AdornedDockTarget.TargetElement is DocumentGroup || owner.AdornedDockTarget.TargetElement is DocumentGroupContainer ? DocumentAutomationName : owner.AdornedDockTarget.TargetElement.GetAutomationPeerCaption());
            }

        /// <summary>Gets the string that uniquely identifies the <see cref="T:System.Windows.FrameworkElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer" />. Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationId" />.</summary>
        /// <returns>The automation identifier for the element associated with the <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer" />, or <see cref="F:System.String.Empty" /> if there isn't an automation identifier.</returns>
        protected override String GetAutomationIdCore()
            {
            var owner = (DockSiteAdorner)Owner;
            var parent = VisualTreeHelper.GetParent(owner);
            return (parent == null || !(parent is DockGroupAdorner) && !(VisualTreeHelper.GetParent(parent) is DockGroupAdorner) ? "Dock_" : "DockGroup_") + owner.DockDirection.ToString();
            }
        }
    }