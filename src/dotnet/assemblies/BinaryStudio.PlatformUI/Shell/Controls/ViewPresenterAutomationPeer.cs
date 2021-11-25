using System;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class ViewPresenterAutomationPeer : FrameworkElementAutomationPeer
        {
        protected View OwnerView
            {
            get
                {
                return ((ViewPresenter)Owner).View;
                }
            }

        public ViewPresenterAutomationPeer(ViewPresenter viewPresenter)
          : base(viewPresenter)
            {
            }

        /// <summary>Gets the string that uniquely identifies the <see cref="T:System.Windows.FrameworkElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer" />. Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationId" />.</summary>
        /// <returns>The automation identifier for the element associated with the <see cref="T:System.Windows.Automation.Peers.FrameworkElementAutomationPeer" />, or <see cref="F:System.String.Empty" /> if there isn't an automation identifier.</returns>
        protected override String GetAutomationIdCore()
            {
            if (OwnerView == null)
                return base.GetAutomationIdCore();
            return OwnerView.Name;
            }

        /// <summary>Gets the control type for the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType" />.</summary>
        /// <returns>The <see cref="F:System.Windows.Automation.Peers.AutomationControlType.Custom" /> enumeration value.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            {
            return AutomationControlType.Pane;
            }

        /// <summary>Gets the text label of the <see cref="T:System.Windows.ContentElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.ContentElementAutomationPeer" />. Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetName" />.</summary>
        /// <returns>The text label of the element that is associated with this automation peer.</returns>
        protected override String GetNameCore()
            {
            if (OwnerView == null)
                return base.GetAutomationIdCore();
            var title = OwnerView.Title;
            if (title != null)
                return title.ToString();
            return String.Empty;
            }

        /// <summary>Gets the name of the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.</summary>
        /// <returns>An <see cref="F:System.String.Empty" /> string.</returns>
        protected override String GetClassNameCore()
            {
            return "ViewPresenter";
            }
        }
    }