using System;
using System.Windows;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    internal class FloatingWindowAutomationPeer : WindowAutomationPeer
        {
        private FloatSite OwnerFloatSite
            {
            get
                {
                return ((FrameworkElement)Owner).DataContext as FloatSite;
                }
            }

        private ViewElement OwnerViewElement
            {
            get
                {
                var ownerFloatSite = OwnerFloatSite;
                if (ownerFloatSite != null)
                    return ownerFloatSite.Child;
                return null;
                }
            }

        internal FloatingWindowAutomationPeer(FloatingWindow floatingWindow)
          : base(floatingWindow)
            {
            }

        /// <summary>Gets the name of the <see cref="T:System.Windows.Window" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.WindowAutomationPeer" />. Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.</summary>
        /// <returns>A string that contains the word "Window".</returns>
        protected override String GetClassNameCore()
            {
            return "FloatingWindow";
            }

        /// <summary>Gets the text label of the <see cref="T:System.Windows.Window" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.ContentElementAutomationPeer" />. Called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetName" />.</summary>
        /// <returns>A string that contains the <see cref="P:System.Windows.Automation.AutomationProperties.Name" /> or  <see cref="P:System.Windows.FrameworkElement.Name" /> of the <see cref="T:System.Windows.Window" />, or <see cref="F:System.String.Empty" /> if the name is null.</returns>
        protected override String GetNameCore()
            {
            var str = String.Empty;
            var ownerViewElement = OwnerViewElement;
            if (ownerViewElement != null)
                str = ownerViewElement.GetAutomationPeerCaption();
            if (String.IsNullOrEmpty(str))
                str = base.GetNameCore();
            return str;
            }
        }
    }