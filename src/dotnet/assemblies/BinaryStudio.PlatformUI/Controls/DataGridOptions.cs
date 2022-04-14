using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class DataGridOptions
        {
        #region P:DataGridOptions.ColumnHeaderTemplateSelector:DataTemplateSelector
        public static readonly DependencyProperty ColumnHeaderTemplateSelectorProperty = DependencyProperty.RegisterAttached("ColumnHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridOptions), new PropertyMetadata(default(DataTemplateSelector)));
        public static void SetColumnHeaderTemplateSelector(DependencyObject element, DataTemplateSelector value)
            {
            element.SetValue(ColumnHeaderTemplateSelectorProperty, value);
            }

        public static DataTemplateSelector GetColumnHeaderTemplateSelector(DependencyObject element)
            {
            return (DataTemplateSelector)element.GetValue(ColumnHeaderTemplateSelectorProperty);
            }
        #endregion

        //public static readonly DependencyProperty DefaultDataGridTextColumnElementStyleProperty = DependencyProperty.RegisterAttached("DefaultDataGridTextColumnElementStyle", typeof(Style), typeof(DataGridOptions), new PropertyMetadata(default(Style), OnDefaultDataGridTextColumnElementStyleChanged));
        //private static void OnDefaultDataGridTextColumnElementStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
        //    if (sender is DataGrid source) {
        //        source.AutoGeneratingColumn += OnAutoGeneratingColumn;
        //        }
        //    }
        //public static void SetDefaultDataGridTextColumnElementStyle(DependencyObject element, Style value)
        //    {
        //    element.SetValue(DefaultDataGridTextColumnElementStyleProperty, value);
        //    }

        //public static Style GetDefaultDataGridTextColumnElementStyle(DependencyObject element)
        //    {
        //    return (Style) element.GetValue(DefaultDataGridTextColumnElementStyleProperty);
        //    }

        //private static void OnAutoGeneratingColumn(Object sender, DataGridAutoGeneratingColumnEventArgs e) {
        //    if (sender is DataGrid source) {
        //        var r = new DataGridTemplateColumn();
        //        r.SetBinding(DataGridTemplateColumn.CellTemplateSelectorProperty,source, CellTemplateSelectorProperty,BindingMode.OneWay);
        //        r.Header = e.Column.Header;
        //        e.Cancel = false;
        //        e.Column = r;
        //        }
        //    }

        //#region P:DataGridOptions.CellTemplateSelector:DataTemplateSelector
        //public static readonly DependencyProperty CellTemplateSelectorProperty = DependencyProperty.RegisterAttached("CellTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridOptions), new PropertyMetadata(default(DataTemplateSelector), OnCellTemplateSelectorChanged));
        //private static void OnCellTemplateSelectorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
        //    if (sender is DataGrid source) {
        //        source.AutoGeneratingColumn += OnAutoGeneratingColumn;
        //        }
        //    }

        //private static void OnAutoGeneratingColumn(Object sender, DataGridAutoGeneratingColumnEventArgs e) {
        //    if (sender is DataGrid source) {
        //        var r = new DataGridTemplateColumn();
        //        r.SetBinding(DataGridTemplateColumn.CellTemplateSelectorProperty,source, CellTemplateSelectorProperty,BindingMode.OneWay);
        //        r.Header = e.Column.Header;
        //        e.Cancel = false;
        //        e.Column = r;
        //        }
        //    }

        //public static void SetCellTemplateSelector(DependencyObject element, DataTemplateSelector value)
        //    {
        //    element.SetValue(CellTemplateSelectorProperty, value);
        //    }

        //public static DataTemplateSelector GetCellTemplateSelector(DependencyObject element)
        //    {
        //    return (DataTemplateSelector)element.GetValue(CellTemplateSelectorProperty);
        //    }
        //#endregion
    }
}