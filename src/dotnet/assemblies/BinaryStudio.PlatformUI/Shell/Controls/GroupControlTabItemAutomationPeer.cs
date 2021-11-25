using System;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class GroupControlTabItemAutomationPeer : TabItemWrapperAutomationPeer
        {
        private GroupControlTabItem TabItemOwner
            {
            get
                {
                return (GroupControlTabItem)Owner;
                }
            }

        public GroupControlTabItemAutomationPeer(GroupControlTabItem owner)
          : base(owner)
            {
            }

        /// <summary>Gets a value that indicates whether the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" /> can accept keyboard focus. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.IsKeyboardFocusable" />.</summary>
        /// <returns>true if the element is focusable by the keyboard; otherwise false.</returns>
        protected override Boolean IsKeyboardFocusableCore()
            {
            return true;
            }

        /// <summary>Sets the keyboard input focus on the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.SetFocus" />.</summary>
        protected override void SetFocusCore()
            {
            var dataContext = TabItemOwner.DataContext as View;
            if (dataContext != null)
                {
                dataContext.ShowInFront();
                dataContext.IsActive = true;
                }
            else
                base.SetFocusCore();
            }

        /// <summary>Gets the control type for the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetAutomationControlType" />.</summary>
        /// <returns>The <see cref="F:System.Windows.Automation.Peers.AutomationControlType.Custom" /> enumeration value.</returns>
        protected override AutomationControlType GetAutomationControlTypeCore()
            {
            return AutomationControlType.TabItem;
            }
        }
    }