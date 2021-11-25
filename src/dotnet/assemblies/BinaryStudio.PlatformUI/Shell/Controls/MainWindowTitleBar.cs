using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class MainWindowTitleBar : Border, INonClientArea
        {
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
            {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
            }

        protected override AutomationPeer OnCreateAutomationPeer()
            {
            return new MainWindowTitleBarAutomationPeer(this);
            }

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
            {
            if (!e.Handled)
                {
                HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
                if (hwndSource != null)
                    {
                    CustomChromeWindow.ShowWindowMenu(hwndSource, this, Mouse.GetPosition(this), base.RenderSize);
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