using System;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridCell : DataGridCell
        {
        static TreeDataGridCell()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeDataGridCell), new FrameworkPropertyMetadata(typeof(TreeDataGridCell)));
            }

        #region P:CellIndex:Int32
        public static readonly DependencyProperty CellIndexProperty = DependencyProperty.Register("CellIndex", typeof(Int32), typeof(TreeDataGridCell), new PropertyMetadata(default(Int32), OnCellIndexChanged));
        private static void OnCellIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            if (sender is TreeDataGridCell source)
                {
                source.OnCellIndexChanged();
                }
            }

        private void OnCellIndexChanged()
            {
            }

        public Int32 CellIndex
            {
            get { return (Int32)GetValue(CellIndexProperty); }
            set { SetValue(CellIndexProperty, value); }
            }
        #endregion
        }
    }