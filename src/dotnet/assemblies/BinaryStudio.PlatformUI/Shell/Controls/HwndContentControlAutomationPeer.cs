using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    internal class HwndContentControlAutomationPeer : FrameworkElementAutomationPeer
        {
        private Visual VisualSubtree { get; set; }

        internal HwndContentControlAutomationPeer(HwndContentControl owner, Visual visualSubtree)
          : base(owner)
            {
            VisualSubtree = visualSubtree;
            }

        /// <summary>Gets the control type for the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType" />.</summary>
        /// <returns>The <see cref="F:System.Windows.Automation.Peers.AutomationControlType.Custom" /> enumeration value.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            {
            return AutomationControlType.Pane;
            }

        internal IRawElementProviderSimple GetProvider()
            {
            return ProviderFromPeer(this);
            }

        /// <summary>Gets the collection of child elements of the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetChildren" />.</summary>
        /// <returns>A list of child <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> elements.</returns>
        protected override List<AutomationPeer> GetChildrenCore()
            {
            List<AutomationPeer> children = null;
            IterateVisualChildren(VisualSubtree, peer =>
            {
                if (children == null)
                    children = new List<AutomationPeer>();
                children.Add(peer);
            });
            return children;
            }

        private static void IterateVisualChildren(DependencyObject parent, IteratorCallback callback)
            {
            if (parent == null)
                return;
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var childIndex = 0; childIndex < childrenCount; ++childIndex)
                {
                var child = VisualTreeHelper.GetChild(parent, childIndex);
                var element = child as UIElement;
                var uiElement3D = child as UIElement3D;
                AutomationPeer peerForElement1;
                if (element != null && (peerForElement1 = CreatePeerForElement(element)) != null)
                    {
                    callback(peerForElement1);
                    }
                else
                    {
                    AutomationPeer peerForElement2;
                    if (uiElement3D != null && (peerForElement2 = UIElement3DAutomationPeer.CreatePeerForElement((UIElement3D)child)) != null)
                        callback(peerForElement2);
                    else
                        IterateVisualChildren(child, callback);
                    }
                }
            }

        private delegate void IteratorCallback(AutomationPeer peer);
        }
    }