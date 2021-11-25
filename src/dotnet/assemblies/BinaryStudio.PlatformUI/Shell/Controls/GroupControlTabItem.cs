using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class GroupControlTabItem : TabItem
        {
        #region P:GroupControlTabItem.CornerRadius:CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(GroupControlTabItem), new FrameworkPropertyMetadata(new CornerRadius(0.0)));
        public static CornerRadius GetCornerRadius(GroupControlTabItem element)
            {
            if (element == null) { throw new ArgumentNullException(nameof(element)); }
            return (CornerRadius)element.GetValue(CornerRadiusProperty);
            }

        public static void SetCornerRadius(GroupControlTabItem element, CornerRadius value)
            {
            if (element == null) { throw new ArgumentNullException(nameof(element)); }
            element.SetValue(CornerRadiusProperty, value);
            }
        #endregion
        #region M:OnCreateAutomationPeer:AutomationPeer
        /// <summary>Provides an appropriate <see cref="T:System.Windows.Automation.Peers.TabItemAutomationPeer" /> implementation for this control, as part of the WPF automation infrastructure.</summary>
        /// <returns>The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementation.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            {
            return new GroupControlTabItemAutomationPeer(this);
            }
        #endregion

        protected override void OnSelected(RoutedEventArgs e)
            {
            base.OnSelected(e);
            }
        }
    }
