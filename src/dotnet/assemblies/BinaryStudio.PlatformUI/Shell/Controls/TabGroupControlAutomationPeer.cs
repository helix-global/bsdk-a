using System;
using System.Windows;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    internal class TabGroupControlAutomationPeer : TabControlAutomationPeer
        {
        protected TabGroupControl TabGroupOwner
            {
            get
                {
                return Owner as TabGroupControl;
                }
            }

        internal TabGroupControlAutomationPeer(TabGroupControl owner) : base(owner)
            {
            }

        /// <summary>Gets the <see cref="T:System.Windows.Rect" /> that represents the bounding rectangle of the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetBoundingRectangle" />.</summary>
        /// <returns>The <see cref="T:System.Windows.Rect" /> that contains the coordinates of the element. Optionally, if the element is not both a <see cref="T:System.Windows.Interop.HwndSource" /> and a <see cref="T:System.Windows.PresentationSource" />, this method returns <see cref="P:System.Windows.Rect.Empty" />.</returns>
        protected override Rect GetBoundingRectangleCore()
            {
            var num = (TabGroupOwner.Header != null) ? TabGroupOwner.Header.RenderSize.Height : 0.0;
            var boundingRectangleCore = base.GetBoundingRectangleCore();
            return new Rect(boundingRectangleCore.X, boundingRectangleCore.Y + num, boundingRectangleCore.Width, boundingRectangleCore.Height - num);
            }

        /// <summary>Gets the name of the <see cref="T:System.Windows.Controls.TabItem" /> that is associated with the new <see cref="T:System.Windows.Automation.Peers.TabItemAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.</summary>
        /// <returns>A string that contains "TabControl".</returns>
        protected override String GetClassNameCore()
            {
            return "ToolWindowTabGroup";
            }

        /// <summary>Sets the keyboard input focus on the <see cref="T:System.Windows.UIElement" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.UIElementAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.SetFocus" />.</summary>
        protected override void SetFocusCore()
            {
            var view = TabGroupOwner.SelectedItem as View;
            if (view != null)
                {
                view.ShowInFront();
                view.IsActive = true;
                return;
                }
            base.SetFocusCore();
            }
        }
    }