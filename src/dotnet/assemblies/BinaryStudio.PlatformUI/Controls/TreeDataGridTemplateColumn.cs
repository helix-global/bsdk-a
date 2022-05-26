using BinaryStudio.PlatformUI.Extensions;
using System.Windows;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridTemplateColumn : TreeDataGridColumn
        {
        #region P:CellTemplate:DataTemplate
        public static readonly DependencyProperty CellTemplateProperty = DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(TreeDataGridTemplateColumn), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate CellTemplate
            {
            get { return (DataTemplate)GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
            }
        #endregion

        public override void PrepareCell(TreeDataGridCell target)
            {
            if (target != null) {
                target.SetBinding(TreeDataGridCell.CellValueProperty, target, FrameworkElement.DataContextProperty, BindingMode.OneWay);
                }
            }
        }
    }