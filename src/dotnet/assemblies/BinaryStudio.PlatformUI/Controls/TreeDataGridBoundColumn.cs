using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridBoundColumn : TreeDataGridColumn
        {
        protected internal TreeDataGrid DataGridOwner { get;internal set; }
        public virtual BindingBase Binding { get;set; }

        public override void PrepareCell(TreeDataGridCell target) {
            if (target != null) {
                target.SetBinding(TreeDataGridCell.CellValueProperty, Binding);
                }
            }
        }
    }