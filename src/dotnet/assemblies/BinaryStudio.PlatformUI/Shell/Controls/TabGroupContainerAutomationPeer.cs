using System;
using System.Collections.Generic;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    internal class TabGroupContainerAutomationPeer : FrameworkElementAutomationPeer
        {
        private ViewHeaderAutomationPeer m_viewHeaderPeer;

        private TabGroupControlAutomationPeer m_tabGroupPeer;

        private List<AutomationPeer> m_childPeers;

        protected TabGroupControl TabGroupOwner
            {
            get
                {
                return Owner as TabGroupControl;
                }
            }

        internal TabGroupContainerAutomationPeer(TabGroupControl owner) : base(owner)
            {
            }

        /// <summary>Gets the name of the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.</summary>
        /// <returns>An <see cref="F:System.String.Empty" /> string.</returns>
        protected override String GetClassNameCore()
            {
            return "ToolWindowTabGroupContainer";
            }

        /// <summary>Gets the control type for the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType" />.</summary>
        /// <returns>The <see cref="F:System.Windows.Automation.Peers.AutomationControlType.Custom" /> enumeration value.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            {
            return AutomationControlType.Pane;
            }

        /// <summary>Gets the collection of child elements of the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetChildren" />.</summary>
        /// <returns>A list of child <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> elements.</returns>
        protected override List<AutomationPeer> GetChildrenCore()
            {
            var flag = false;
            if (m_tabGroupPeer == null || m_tabGroupPeer.Owner != TabGroupOwner)
                {
                m_tabGroupPeer = new TabGroupControlAutomationPeer(TabGroupOwner);
                flag = true;
                }
            var header = TabGroupOwner.Header;
            if (header != null && (m_viewHeaderPeer == null || m_viewHeaderPeer.Owner != header))
                {
                m_viewHeaderPeer = new ViewHeaderAutomationPeer(header);
                flag = true;
                }
            if (flag)
                {
                m_childPeers = new List<AutomationPeer>();
                if (m_viewHeaderPeer != null)
                    {
                    m_childPeers.Add(m_viewHeaderPeer);
                    }
                if (m_tabGroupPeer != null)
                    {
                    m_childPeers.Add(m_tabGroupPeer);
                    }
                }
            return m_childPeers;
            }

        /// <summary>Gets the text label of the <see cref="T:System.Windows.ContentElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.ContentElementAutomationPeer" />. Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetName" />.</summary>
        /// <returns>The text label of the element that is associated with this automation peer.</returns>
        protected override String GetNameCore()
            {
            var tabGroupControl = Owner as TabGroupControl;
            if (tabGroupControl != null)
                {
                var view = tabGroupControl.SelectedItem as View;
                if (view != null && view.Title != null)
                    {
                    return view.Title.ToString();
                    }
                }
            return base.GetNameCore();
            }
        }
    }