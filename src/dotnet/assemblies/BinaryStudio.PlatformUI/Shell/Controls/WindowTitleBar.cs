using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class WindowTitleBar : Border, INonClientArea
        {
        /// <summary> Implements <see cref="M:System.Windows.Media.Visual.HitTestCore(System.Windows.Media.PointHitTestParameters)"/> to supply base element hit testing behavior (returning <see cref="T:System.Windows.Media.HitTestResult"/>).</summary>
        /// <returns>Results of the test, including the evaluated point.</returns>
        /// <param name="hitTestParameters">Describes the hit test to perform, including the initial hit point.</param>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
            {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
            }

        /// <summary>Returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementations for the Windows Presentation Foundation (WPF) infrastructure.</summary>
        /// <returns>The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer"/> implementation.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            {
            return new MainWindowTitleBarAutomationPeer(this);
            }

        /// <summary> Invoked whenever an unhandled <see cref="E:System.Windows.FrameworkElement.ContextMenuOpening"/> routed event reaches this class in its route. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e) {
            if (!e.Handled) {
                if (PresentationSource.FromVisual(this) is HwndSource hwndSource) {
                    CustomChromeWindow.ShowWindowMenu(hwndSource, this, Mouse.GetPosition(this), RenderSize);
                    }
                e.Handled = true;
                }
            }

        Int32 INonClientArea.HitTest(Point point)
            {
            return 2;
            }
        }
    }