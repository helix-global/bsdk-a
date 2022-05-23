using System;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridCell : ContentControl
        {
        static TreeDataGridCell()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeDataGridCell), new FrameworkPropertyMetadata(typeof(TreeDataGridCell)));
            }

        #region P:CellValue:Object
        internal static readonly DependencyProperty CellValueProperty = DependencyProperty.Register("CellValue", typeof(Object), typeof(TreeDataGridCell), new PropertyMetadata(default(Object),OnCellValueChanged));
        private static void OnCellValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
            return;
            }

        internal Object CellValue
            {
            get { return (Object)GetValue(CellValueProperty); }
            set { SetValue(CellValueProperty, value); }
            }
        #endregion
        #region P:Column:TreeDataGridColumn
        private static readonly DependencyPropertyKey ColumnPropertyKey = DependencyProperty.RegisterReadOnly("Column", typeof(TreeDataGridColumn), typeof(TreeDataGridCell), new PropertyMetadata(default(TreeDataGridColumn)));
        public static readonly DependencyProperty ColumnProperty = ColumnPropertyKey.DependencyProperty;
        public TreeDataGridColumn Column
            {
            get { return (TreeDataGridColumn)GetValue(ColumnProperty); }
            internal set { SetValue(ColumnPropertyKey, value); }
            }
        #endregion
        #region P:IsSelected:Boolean
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(Boolean), typeof(TreeDataGridCell), new PropertyMetadata(default(Boolean)));
        public Boolean IsSelected
            {
            get { return (Boolean)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
            }
        #endregion
        #region P:EditTemplate:DataTemplate
        public static readonly DependencyProperty CellEditingTemplateProperty = DependencyProperty.Register("EditTemplate", typeof(DataTemplate), typeof(TreeDataGridCell), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate CellEditingTemplate
            {
            get { return (DataTemplate)GetValue(CellEditingTemplateProperty); }
            set { SetValue(CellEditingTemplateProperty, value); }
            }
        #endregion
        #region P:DisplayTemplate:DataTemplate
        public static readonly DependencyProperty CellTemplateProperty = DependencyProperty.Register("DisplayTemplate", typeof(DataTemplate), typeof(TreeDataGridCell), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate CellTemplate
            {
            get { return (DataTemplate)GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
            }
        #endregion
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