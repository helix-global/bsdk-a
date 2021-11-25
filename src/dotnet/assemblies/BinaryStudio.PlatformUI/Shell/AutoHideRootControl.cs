using System;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class AutoHideRootControl : LayoutSynchronizedItemsControl
        {
        static AutoHideRootControl()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideRootControl), new FrameworkPropertyMetadata(typeof(AutoHideRootControl)));
            }

        #region P:DockRoot:FrameworkElement
        private static readonly DependencyPropertyKey DockRootPropertyKey = DependencyProperty.RegisterReadOnly("DockRoot", typeof(FrameworkElement), typeof(AutoHideRootControl), new PropertyMetadata(default(FrameworkElement)));
        public static readonly DependencyProperty DockRootProperty = DockRootPropertyKey.DependencyProperty;
        public FrameworkElement DockRoot {
            get { return (FrameworkElement) GetValue(DockRootProperty); }
            protected set { SetValue(DockRootPropertyKey, value); }
            }
        #endregion

        #region M:PrepareContainerForItemOverride(DependencyObject,Object)
        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, Object item) {
            base.PrepareContainerForItemOverride(element, item);
            var u = element as UIElement;
            var i = item as DependencyObject;
            if (u != null) {
                if (item is DockRoot) {
                    Panel.SetZIndex(u, 0);
                    Grid.SetColumn(u, 1);
                    Grid.SetRow(u, 1);
                    DockRoot = u as FrameworkElement;
                    return;
                    }
                Panel.SetZIndex(u, 1);
                if (i != null) {
                    switch ((Dock)i.GetValue(DockPanel.DockProperty)) {
                        case Dock.Left:
                            {
                            Grid.SetColumn(u, 0);
                            Grid.SetRow(u, 1);
                            }
                            return;
                        case Dock.Top:
                            {
                            Grid.SetColumn(u, 1);
                            Grid.SetRow(u, 0);
                            }
                            return;
                        case Dock.Right:
                            {
                            Grid.SetColumn(u, 2);
                            Grid.SetRow(u, 1);
                            }
                            return;
                        case Dock.Bottom:
                            {
                            Grid.SetColumn(u, 1);
                            Grid.SetRow(u, 2);
                            }
                            return;
                        default: return;
                        }
                    }
                }
            }
        #endregion
        }
    }
