using System;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DragUndockHeaderAutomationPeer : FrameworkElementAutomationPeer
        {
        public ViewElement ViewElement
            {
            get
                {
                return ((DragUndockHeader)Owner).ViewElement;
                }
            }

        public DragUndockHeaderAutomationPeer(DragUndockHeader header)
          : base(header)
            {
            }

        protected override AutomationControlType GetAutomationControlTypeCore()
            {
            return AutomationControlType.TitleBar;
            }

        protected override String GetNameCore()
            {
            if (ViewElement == null)
                return "DragUndockHeader";
            var rootElement = ViewElement.FindRootElement(ViewElement) as FloatSite;
            if (rootElement != null)
                {
                var floatingWindow = ViewManager.Instance.FloatingWindowManager.TryGetFloatingWindow(rootElement);
                if (floatingWindow != null)
                    return floatingWindow.Title;
                }
            return ViewElement.ToString();
            }

        protected override String GetClassNameCore()
            {
            return "DragUndockHeaderTitleBar";
            }
        }
    }