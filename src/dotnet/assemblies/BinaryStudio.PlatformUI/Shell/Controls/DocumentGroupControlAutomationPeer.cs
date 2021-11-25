using System;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    internal class DocumentGroupControlAutomationPeer : TabControlAutomationPeer
        {
        private DocumentGroupControl DocumentGroupOwner
            {
            get
                {
                return (DocumentGroupControl)Owner;
                }
            }

        internal DocumentGroupControlAutomationPeer(DocumentGroupControl documentGroup)
          : base(documentGroup)
            {
            }

        /// <summary>Gets the name of the <see cref="T:System.Windows.Controls.TabItem" /> that is associated with the new <see cref="T:System.Windows.Automation.Peers.TabItemAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.</summary>
        /// <returns>A string that contains "TabControl".</returns>
        protected override String GetClassNameCore()
            {
            return "DocumentGroup";
            }

        /// <summary>Sets the keyboard input focus on the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.SetFocus" />.</summary>
        protected override void SetFocusCore()
            {
            var selectedItem = DocumentGroupOwner.SelectedItem as View;
            if (selectedItem != null)
                {
                selectedItem.ShowInFront();
                selectedItem.IsActive = true;
                }
            else
                base.SetFocusCore();
            }
        }
    }