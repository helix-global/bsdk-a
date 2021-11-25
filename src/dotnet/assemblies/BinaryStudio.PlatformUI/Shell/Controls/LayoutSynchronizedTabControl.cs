using System;
using System.Windows;
using System.Windows.Controls;
using BinaryStudio.PlatformUI.Shell;

namespace BinaryStudio.PlatformUI
    {
    public class LayoutSynchronizedTabControl : TabControl
        {
        static LayoutSynchronizedTabControl()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LayoutSynchronizedTabControl), new FrameworkPropertyMetadata(typeof(LayoutSynchronizedTabControl)));
            }

        protected override void PrepareContainerForItemOverride(DependencyObject element, Object item)
            {
            base.PrepareContainerForItemOverride(element, item);
            var d = item as DependencyObject;
            if (d != null) {
                ViewManager.BindViewManager(element, d);
                }
            }
        }
    }
