using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGrid : TreeView
        {
        static TreeDataGrid()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(typeof(TreeDataGrid)));
            }

        public TreeDataGrid()
            {
            Columns = new ObservableCollection<TreeDataGridColumn>();
            Columns.CollectionChanged += OnColumnsCollectionChanged;
            }

        private void OnColumnsCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
            var i = 0;
            foreach (var column in Columns) {
                column.ColumnIndex = i;
                column.IsLastColumn = i == Columns.Count - 1;
                i++;
                }
            }

        public ObservableCollection<TreeDataGridColumn> Columns { get; }

        /// <summary>Instantiates a new <see cref="T:System.Windows.Controls.DataGridRow" />.</summary>
        /// <returns>The row that is the container.</returns>
        protected override DependencyObject GetContainerForItemOverride()
            {
            return new TreeDataGridRow
                {
                TreeDataGridOwner = this
                };
            }

        /// <summary>Prepares a new row for the specified item.</summary>
        /// <param name="element">The new <see cref="T:System.Windows.Controls.DataGridRow" />.</param>
        /// <param name="item">The data item that the row contains.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, Object item) {
            base.PrepareContainerForItemOverride(element, item);
            //BindingOperations.SetBinding(element, TreeDataGridRow.ItemsSourceProperty, HierarchicalDataBinding);
            }

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
            {
            base.OnApplyTemplate();
            //ItemsHost = GetTemplateChild("ItemsHost") as TreeView;
            //OnItemsSourceChanged();
            }

        //public virtual BindingBase HierarchicalDataBinding { get;set; }

        //private TreeView ItemsHost;
        }
    }
