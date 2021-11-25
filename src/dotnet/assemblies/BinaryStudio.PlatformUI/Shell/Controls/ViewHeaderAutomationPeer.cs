using System;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    internal class ViewHeaderAutomationPeer : FrameworkElementAutomationPeer
        {
        protected View OwnerView
            {
            get
                {
                return ((ViewHeader)Owner).View;
                }
            }

        internal ViewHeaderAutomationPeer(ViewHeader header) : base(header)
            {
            }

        /// <summary>Gets the control type for the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType" />.</summary>
        /// <returns>The <see cref="F:System.Windows.Automation.Peers.AutomationControlType.Custom" /> enumeration value.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            {
            return AutomationControlType.TitleBar;
            }

        /// <summary>Gets the text label of the <see cref="T:System.Windows.ContentElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.ContentElementAutomationPeer" />. Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetName" />.</summary>
        /// <returns>The text label of the element that is associated with this automation peer.</returns>
        protected override String GetNameCore()
            {
            if (OwnerView == null)
                {
                return base.GetNameCore();
                }
            if (OwnerView.Title != null)
                {
                return OwnerView.Title.ToString();
                }
            return String.Empty;
            }

        /// <summary>Gets the name of the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.</summary>
        /// <returns>An <see cref="F:System.String.Empty" /> string.</returns>
        protected override String GetClassNameCore()
            {
            return "ToolWindowTitleBar";
            }
        }
    }