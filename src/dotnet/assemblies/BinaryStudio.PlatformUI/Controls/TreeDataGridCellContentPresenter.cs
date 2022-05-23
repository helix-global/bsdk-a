using System;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridCellContentPresenter : Control
        {
        #region P:DisplayTemplate:DataTemplate
        public static readonly DependencyProperty DisplayTemplateProperty = DependencyProperty.Register("DisplayTemplate", typeof(DataTemplate), typeof(TreeDataGridCellContentPresenter), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate DisplayTemplate
            {
            get { return (DataTemplate)GetValue(DisplayTemplateProperty); }
            set { SetValue(DisplayTemplateProperty, value); }
            }
        #endregion
        #region P:EditTemplate:DataTemplate
        public static readonly DependencyProperty EditTemplateProperty = DependencyProperty.Register("EditTemplate", typeof(DataTemplate), typeof(TreeDataGridCellContentPresenter), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate EditTemplate
            {
            get { return (DataTemplate)GetValue(EditTemplateProperty); }
            set { SetValue(EditTemplateProperty, value); }
            }
        #endregion
        #region P:IsEditing:Boolean
        private static readonly DependencyPropertyKey IsEditingPropertyKey = DependencyProperty.RegisterReadOnly("IsEditing", typeof(Boolean), typeof(TreeDataGridCellContentPresenter), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;
        public Boolean IsEditing
            {
            get { return (Boolean)GetValue(IsEditingProperty); }
            private set { SetValue(IsEditingPropertyKey, value); }
            }
        #endregion
        #region P:Content:Object
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(Object), typeof(TreeDataGridCellContentPresenter), new PropertyMetadata(default(Object)));
        public Object Content
            {
            get { return (Object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
            }
        #endregion
        }
    }