using System;
using System.Windows;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class MainWindowTitleBarAutomationPeer : FrameworkElementAutomationPeer
        {
        public MainWindowTitleBarAutomationPeer(WindowTitleBar owner) : base(owner)
            {
            }

        protected override AutomationControlType GetAutomationControlTypeCore()
            {
            return AutomationControlType.TitleBar;
            }

        protected override String GetNameCore()
            {
            var presentationSource = PresentationSource.FromVisual(base.Owner);
            if (presentationSource != null) {
                var window = presentationSource.RootVisual as Window;
                if (window != null)
                    {
                    return window.Title;
                    }
                }
            return "TitleBar";
            }

        protected override String GetAutomationIdCore()
            {
            return "TitleBar";
            }
        }
    }