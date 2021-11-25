using System;
using System.Windows;
using System.Windows.Automation.Peers;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DockSiteAdorner : DockAdorner
        {
        public DockTarget AdornedDockTarget
            {
            get
                {
                var ancestor = this.FindAncestor<DockAdornerWindow>();
                if (ancestor == null)
                    return null;
                if (ancestor.AdornedElement == null)
                    return null;
                return ancestor.AdornedElement as DockTarget;
                }
            }

        #region P:CreatesDocumentGroup:Boolean
        public static readonly DependencyProperty CreatesDocumentGroupProperty = DependencyProperty.Register("CreatesDocumentGroup", typeof(Boolean), typeof(DockSiteAdorner), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        public Boolean CreatesDocumentGroup
            {
            get { return (Boolean)GetValue(CreatesDocumentGroupProperty); }
            set { SetValue(CreatesDocumentGroupProperty, Boxes.Box(value)); }
            }
        #endregion
        #region P:IsHighlighted:Boolean
        public static readonly DependencyProperty IsHighlightedProperty = DependencyProperty.Register("IsHighlighted", typeof(Boolean), typeof(DockSiteAdorner), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        public Boolean IsHighlighted
            {
            get { return (Boolean)GetValue(IsHighlightedProperty); }
            set { SetValue(IsHighlightedProperty, Boxes.Box(value)); }
            }
        #endregion

        static DockSiteAdorner()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockSiteAdorner), new FrameworkPropertyMetadata(typeof(DockSiteAdorner)));
            }

        /// <summary>Returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementations for the Windows Presentation Foundation (WPF) infrastructure.</summary>
        /// <returns>The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementation.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
            {
            return new DockSiteAdornerAutomationPeer(this);
            }
        }
    }