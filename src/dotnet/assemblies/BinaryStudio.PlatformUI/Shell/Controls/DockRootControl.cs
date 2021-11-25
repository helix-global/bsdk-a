using System.Windows;
using System.Windows.Automation.Peers;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DockRootControl : LayoutSynchronizedItemsControl
        {
        static DockRootControl()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockRootControl), new FrameworkPropertyMetadata(typeof(DockRootControl)));
            }

        /// <summary>Returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementations for the Windows Presentation Foundation (WPF) infrastructure.</summary>
        /// <returns>The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementation.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            {
            return new DockRootControlAutomatinPeer(this);
            }
        }
    }
