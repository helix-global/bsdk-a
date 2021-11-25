using System;
using System.Windows;
using System.Windows.Automation.Peers;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class AutoHideTabItem : RoutedCommandButton
        {
        #region P:IsAutoHideWindowShown:Boolean
        public static readonly DependencyProperty IsAutoHideWindowShownProperty = DependencyProperty.Register("IsAutoHideWindowShown", typeof(Boolean), typeof(AutoHideTabItem), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnIsAutoHideWindowShownChanged));
        public Boolean IsAutoHideWindowShown
            {
            get { return (Boolean)GetValue(IsAutoHideWindowShownProperty); }
            set { SetValue(IsAutoHideWindowShownProperty, Boxes.Box(value)); }
            }
        #endregion

        static AutoHideTabItem()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideTabItem), new FrameworkPropertyMetadata(typeof(AutoHideTabItem)));
            }

        /// <summary>Creates an appropriate <see cref="T:System.Windows.Automation.Peers.ButtonAutomationPeer" /> for this control as part of the WPF infrastructure.</summary>
        /// <returns>A <see cref="T:System.Windows.Automation.Peers.ButtonAutomationPeer" /> for this control.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            {
            return new AutoHideTabItemAutomationPeer(this);
            }

        private static void OnIsAutoHideWindowShownChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
            {
            var autoHideTabItem = (AutoHideTabItem)obj;
            if (!autoHideTabItem.IsConnectedToPresentationSource() || !autoHideTabItem.IsAutoHideWindowShown) { return; }
            ViewCommands.ShowAutoHiddenView.Execute(autoHideTabItem.DataContext, autoHideTabItem);
            }
        }
    }
