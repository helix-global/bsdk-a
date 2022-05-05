using System;
using System.Windows;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class DataGridCellTemplate : DependencyObject
        {
        public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register(nameof(DataType), typeof(Type), typeof(DataGridCellTemplate), new PropertyMetadata(default(Type)));
        public Type DataType
            {
            get { return (Type)GetValue(DataTypeProperty); }
            set { SetValue(DataTypeProperty, value); }
            }

        public static readonly DependencyProperty DisplayTemplateProperty = DependencyProperty.Register(nameof(DisplayTemplate), typeof(DataTemplate), typeof(DataGridCellTemplate), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate DisplayTemplate
            {
            get { return (DataTemplate)GetValue(DisplayTemplateProperty); }
            set { SetValue(DisplayTemplateProperty, value); }
            }

        public static readonly DependencyProperty EditTemplateProperty = DependencyProperty.Register(nameof(EditTemplate), typeof(DataTemplate), typeof(DataGridCellTemplate), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate EditTemplate
            {
            get { return (DataTemplate)GetValue(EditTemplateProperty); }
            set { SetValue(EditTemplateProperty, value); }
            }
        }
    }