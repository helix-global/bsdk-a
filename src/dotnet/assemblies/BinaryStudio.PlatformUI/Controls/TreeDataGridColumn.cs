using System;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Controls
    {
    public abstract class TreeDataGridColumn : DependencyObject
        {
        #region P:ActualWidth:Double
        private static readonly DependencyPropertyKey ActualWidthPropertyKey = DependencyProperty.RegisterReadOnly("ActualWidth", typeof(Double), typeof(TreeDataGridColumn), new PropertyMetadata(default(Double)));
        public static readonly DependencyProperty ActualWidthProperty = ActualWidthPropertyKey.DependencyProperty;
        public Double ActualWidth
            {
            get { return (Double)GetValue(ActualWidthProperty); }
            internal set { SetValue(ActualWidthPropertyKey, value); }
            }
        #endregion
        #region P:Header:Object
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(Object), typeof(TreeDataGridColumn), new PropertyMetadata(default(Object)));
        public Object Header
            {
            get { return (Object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
            }
        #endregion
        #region P:HeaderStyle:Style
        public static readonly DependencyProperty HeaderStyleProperty = DependencyProperty.Register("HeaderStyle", typeof(Style), typeof(TreeDataGridColumn), new PropertyMetadata(default(Style)));
        public Style HeaderStyle
            {
            get { return (Style)GetValue(HeaderStyleProperty); }
            set { SetValue(HeaderStyleProperty, value); }
            }
        #endregion
        #region P:HeaderTemplate:DataTemplate
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(TreeDataGridColumn), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate HeaderTemplate
            {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
            }
        #endregion
        #region P:MinWidth:Double
        public static readonly DependencyProperty MinWidthProperty = DependencyProperty.Register("MinWidth", typeof(Double), typeof(TreeDataGridColumn), new PropertyMetadata(20.0));
        public Double MinWidth
            {
            get { return (Double)GetValue(MinWidthProperty); }
            set { SetValue(MinWidthProperty, value); }
            }
        #endregion
        #region P:MaxWidth:Double
        public static readonly DependencyProperty MaxWidthProperty = DependencyProperty.Register("MaxWidth", typeof(Double), typeof(TreeDataGridColumn), new PropertyMetadata(Double.PositiveInfinity));
        public Double MaxWidth
            {
            get { return (Double)GetValue(MaxWidthProperty); }
            set { SetValue(MaxWidthProperty, value); }
            }
        #endregion
        #region P:Width:DataGridLength
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register("Width", typeof(DataGridLength), typeof(TreeDataGridColumn), new PropertyMetadata(default(DataGridLength)));
        public DataGridLength Width
            {
            get { return (DataGridLength)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
            }
        #endregion
        #region P:ColumnIndex:Int32
        private static readonly DependencyPropertyKey IndexPropertyKey = DependencyProperty.RegisterReadOnly("ColumnIndex", typeof(Int32), typeof(TreeDataGridColumn), new PropertyMetadata(default(Int32)));
        public static readonly DependencyProperty ColumnIndexProperty = IndexPropertyKey.DependencyProperty;
        public Int32 ColumnIndex
            {
            get { return (Int32)GetValue(ColumnIndexProperty); }
            internal set { SetValue(IndexPropertyKey, value); }
            }
        #endregion
        #region P:IsLastColumn:Boolean
        private static readonly DependencyPropertyKey IsLastColumnPropertyKey = DependencyProperty.RegisterReadOnly("IsLastColumn", typeof(Boolean), typeof(TreeDataGridColumn), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsLastColumnProperty = IsLastColumnPropertyKey.DependencyProperty;
        public Boolean IsLastColumn
            {
            get { return (Boolean)GetValue(IsLastColumnProperty); }
            internal set { SetValue(IsLastColumnPropertyKey, value); }
            }
        #endregion

        public abstract void PrepareCell(TreeDataGridCell target);
        }
    }