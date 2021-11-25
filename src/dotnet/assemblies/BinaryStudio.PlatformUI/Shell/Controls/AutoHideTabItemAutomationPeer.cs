using System;
using System.Windows;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class AutoHideTabItemAutomationPeer : ButtonAutomationPeer
        {
        private View OwnerView
            {
            get
                {
                return ((FrameworkElement)Owner).DataContext as View;
                }
            }

        public AutoHideTabItemAutomationPeer(AutoHideTabItem element)
          : base(element)
            {
            }

        /// <summary>Gets a human-readable string that contains the item type that the <see cref="T:System.Windows.UIElement" /> for this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" /> represents. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetItemType" />.</summary>
        /// <returns>The string that contains the <see cref="P:System.Windows.Automation.AutomationProperties.ItemType" /> that is returned by <see cref="M:System.Windows.Automation.AutomationProperties.GetItemType(System.Windows.DependencyObject)" />. </returns>
        protected override String GetItemTypeCore()
            {
            return "AutoHideTabItem";
            }

        /// <summary>Gets the <see cref="P:System.Windows.Automation.AutomationProperties.AutomationId" /> for the element associated with this <see cref="T:System.Windows.Automation.Peers.ButtonBaseAutomationPeer" />. Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationId" />.</summary>
        /// <returns>The string that contains the <see cref="P:System.Windows.Automation.AutomationProperties.AutomationId" />.</returns>
        protected override String GetAutomationIdCore()
            {
            var ownerView = OwnerView;
            if (ownerView != null) { return "AUTOHIDE_" + ownerView.Name; }
            return base.GetAutomationIdCore();
            }

        /// <summary>Gets the name of the class of the element associated with this <see cref="T:System.Windows.Automation.Peers.ButtonBaseAutomationPeer" />. Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetName" />.</summary>
        /// <returns>A string that contains the class name, minus the accelerator key.</returns>
        protected override String GetNameCore()
            {
            var ownerView = OwnerView;
            if ((ownerView != null) && (ownerView.Title != null)) { return ownerView.Title.ToString(); }
            return base.GetNameCore();
            }
        }
    }