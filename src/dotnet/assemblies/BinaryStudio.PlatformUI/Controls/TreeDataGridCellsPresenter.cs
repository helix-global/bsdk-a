using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridCellsPresenter : DataGridCellsPresenter
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
                cell.CellIndex = ItemContainerGenerator.IndexFromContainer(element);
                }
            }
        }
    }