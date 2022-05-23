using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridCellsPresenter : ItemsControl
        {
        static TreeDataGridCellsPresenter()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeDataGridCellsPresenter), new FrameworkPropertyMetadata(typeof(TreeDataGridCellsPresenter)));
            }

        /// <summary>Returns a new <see cref="T:System.Windows.Controls.DataGridCell"/> instance to contain a cell value.</summary>
        /// <returns>A new <see cref="T:System.Windows.Controls.DataGridCell"/> instance.</returns>
        protected override DependencyObject GetContainerForItemOverride()
            {
            return new TreeDataGridCell();
            }

        /// <summary>Prepares the cell to display the specified item.</summary>
        /// <param name="element">The cell to prepare.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, Object item) {
            base.PrepareContainerForItemOverride(element, item);
            if (element is TreeDataGridCell cell) {
                var index = ItemContainerGenerator.IndexFromContainer(element);
                cell.Column = TreeDataGridRowOwner?.TreeDataGridOwner?.Columns[index];
                cell.CellIndex = index;
                cell.Column?.PrepareCell(cell);
                }
            }

        /// <summary>Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement" /> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)" />.</summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
            {
            base.OnPropertyChanged(e);
            if (ReferenceEquals(e.Property,DataContextProperty)) {
                OnDataContextChanged();
                }
            }

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            if (TreeDataGridRowOwner == null) {
                TreeDataGridRowOwner = this.FindAncestor<TreeDataGridRow>();
                OnDataContextChanged();
                }
            }

        private void OnDataContextChanged()
            {
            ItemsSource = (TreeDataGridRowOwner != null)
                ? TreeDataGridRowOwner.TreeDataGridOwner.Columns.Select(i => DataContext)
                : null;
            }

        internal TreeDataGridRow TreeDataGridRowOwner;
        }
    }