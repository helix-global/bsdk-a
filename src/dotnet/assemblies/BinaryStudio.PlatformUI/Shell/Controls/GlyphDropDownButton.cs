using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI.Shell.Controls {
    public class GlyphDropDownButton : GlyphButton {
        public GlyphDropDownButton() {
            ContextMenuOpening += OnContextMenuOpening;
            Click += OnClick;
            SetResourceReference(StyleProperty, typeof(GlyphButton));
            }

        public void ShowDropDown() {
            if (ContextMenu == null || ContextMenu.Items.Count == 0 && (ContextMenu.TemplatedParent == null || !(ContextMenu.TemplatedParent as ItemsControl).HasItems))
                return;
            var itemsSource = ContextMenu.ItemsSource as CollectionView;
            if (itemsSource != null)
                itemsSource.Refresh();
            ContextMenu.Placement = PlacementMode.Bottom;
            ContextMenu.PlacementTarget = this;
            ContextMenu.IsOpen = true;
            }

        private void OnClick(Object sender, RoutedEventArgs e) {
            ShowDropDown();
            }

        private void OnContextMenuOpening(Object sender, ContextMenuEventArgs e) {
            ContextMenu.IsOpen = false;
            e.Handled = true;
            }
        }
    }