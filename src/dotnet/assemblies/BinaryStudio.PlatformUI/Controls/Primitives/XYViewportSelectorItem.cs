using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    public class XYViewportSelectorItem : ContentControl
        {
        static XYViewportSelectorItem()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XYViewportSelectorItem), new FrameworkPropertyMetadata(typeof(XYViewportSelectorItem)));
            }

        public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(XYViewportSelectorItem), new FrameworkPropertyMetadata(default(Boolean),FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, new PropertyChangedCallback(OnIsSelectedChanged)));
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
            }

        public Boolean IsSelected
            {
            get { return (Boolean)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
            }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) {
                var i = !IsSelected;
                if (i)
                    {
                    Owner.SelectItem(this);
                    }
                else
                    {
                    Owner.UnselectItem(this);
                    }
                }
            else
                {
                Owner.UnselectAll();
                Owner.SelectItem(this);
                }
            e.Handled = true;
            }

        public override void OnApplyTemplate()
            {
            base.OnApplyTemplate();
            Owner = this.FindAncestor<XYViewportSelector>();
            }

        private XYViewportSelector Owner;
        }
    }
