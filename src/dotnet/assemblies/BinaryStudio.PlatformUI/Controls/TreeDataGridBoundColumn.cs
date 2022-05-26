using System.Windows.Data;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridBoundColumn : TreeDataGridColumn
        {
        public virtual BindingBase Binding { get;set; }

        public override void PrepareCell(TreeDataGridCell target) {
            if (target != null) {
                target.SetBinding(TreeDataGridCell.CellValueProperty, Binding);
                }
            }
        }
    }