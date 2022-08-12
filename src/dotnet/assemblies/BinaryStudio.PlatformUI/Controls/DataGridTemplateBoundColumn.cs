using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class DataGridTemplateBoundColumn : DataGridTemplateColumn
        {
        private BindingBase _binding;
        public virtual BindingBase Binding
            {
            get { return _binding; }
            set
                {
                if (_binding != value)
                    {
                    BindingBase binding = _binding;
                    _binding = value;
                    CoerceValue(IsReadOnlyProperty);
                    CoerceValue(SortMemberPathProperty);
                    OnBindingChanged(binding, _binding);
                    }
                }
            }

        protected virtual void OnBindingChanged(BindingBase oldBinding, BindingBase newBinding)
            {
            NotifyPropertyChanged("Binding");
            }

        /// <summary>Gets an element defined by the <see cref="P:System.Windows.Controls.DataGridTemplateColumn.DisplayTemplate"/> that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding"/> property value.</summary>
        /// <returns>A new, read-only element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding"/> property value.</returns>
        /// <param name="cell">The cell that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
            {
            return LoadTemplateContent(CellTemplate, dataItem, cell);
            }

        private FrameworkElement LoadTemplateContent(DataTemplate template, Object dataItem, DataGridCell cell)
            {
            ContentPresenter contentPresenter = new ContentPresenter();
            BindingOperations.SetBinding(contentPresenter, ContentPresenter.ContentProperty, Binding);
            contentPresenter.ContentTemplate = template;
            return contentPresenter;
            }

        /// <summary>Gets an element defined by the <see cref="P:System.Windows.Controls.DataGridTemplateColumn.EditTemplate"/> that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding"/> property value.</summary>
        /// <returns>A new editing element that is bound to the column's <see cref="P:System.Windows.Controls.DataGridBoundColumn.Binding"/> property value.</returns>
        /// <param name="cell">The cell that will contain the generated element.</param>
        /// <param name="dataItem">The data item represented by the row that contains the intended cell.</param>
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
            {
            return LoadTemplateContent(CellEditingTemplate, dataItem, cell);
            }
        }
    }