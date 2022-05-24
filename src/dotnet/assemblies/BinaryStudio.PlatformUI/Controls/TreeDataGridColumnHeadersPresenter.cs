using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridColumnHeadersPresenter : ItemsControl
        {
        static TreeDataGridColumnHeadersPresenter()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeDataGridColumnHeadersPresenter), new FrameworkPropertyMetadata(typeof(TreeDataGridColumnHeadersPresenter)));
            }

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
            {
            base.OnApplyTemplate();
            ItemsHost = GetTemplateChild("ItemsHost") as Grid;
            if (ItemsHost != null) {
                ItemsHost.LayoutUpdated += OnItemsHostLayoutUpdated;
                }
            if (TreeDataGridOwner == null) {
                TreeDataGridOwner = this.FindAncestor<TreeDataGrid>();
                ItemsSource = TreeDataGridOwner?.Columns;
                }
                
            if ((ItemsHost != null) && (TreeDataGridOwner != null)) {
                ItemsHost.ColumnDefinitions.Clear();
                foreach (var column in TreeDataGridOwner.Columns) {
                    var target = new ColumnDefinition();
                    ItemsHost.ColumnDefinitions.Add(target);
                    target.SetBinding(ColumnDefinition.MaxWidthProperty, column, TreeDataGridColumn.MaxWidthProperty, BindingMode.OneWay);
                    target.SetBinding(ColumnDefinition.MinWidthProperty, column, TreeDataGridColumn.MinWidthProperty, BindingMode.OneWay);
                    target.SetBinding(ColumnDefinition.WidthProperty, column, TreeDataGridColumn.WidthProperty, BindingMode.OneWay, new TreeDataGridLengthConverter());
                    }
                }
            }

        private void OnItemsHostLayoutUpdated(Object sender, EventArgs e) {
            Debug.Print("OnItemsHostLayoutUpdated");
            if (TreeDataGridOwner != null) {
                var i = 0;
                foreach (var column in ItemsHost.ColumnDefinitions) {
                    TreeDataGridOwner.Columns[i].ActualWidth = column.ActualWidth;
                    i++;
                    }
                }
            }

        /// <summary>Creates or identifies the element that is used to display the given item.</summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride() {
            return new TreeDataGridColumnHeader{
                
                };
            }

        /// <summary>Prepares the specified element to display the specified item. </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, Object item) {
            base.PrepareContainerForItemOverride(element, item);
            var index = ItemContainerGenerator.IndexFromContainer(element);
            if (element is UIElement u) {
                Grid.SetColumn(u, index);
                }
            if (element is TreeDataGridColumnHeader header) {
                header.Column = TreeDataGridOwner.Columns[index];
                }
            }

        internal TreeDataGrid TreeDataGridOwner;
        private Grid ItemsHost;
        }
    }