using System;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridRow : DataGridRow
        {
        static TreeDataGridRow()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeDataGridRow), new FrameworkPropertyMetadata(typeof(TreeDataGridRow)));
            }

        #region P:ItemsSource:Object
        internal static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(Object), typeof(TreeDataGridRow), new PropertyMetadata(default(Object)));
        internal Object ItemsSource
            {
            get { return (Object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
            }
        #endregion
        #region P:HasItemsInternal:Boolean
        public static readonly DependencyProperty HasItemsInternalProperty = DependencyProperty.Register("HasItemsInternal", typeof(Boolean), typeof(TreeDataGridRow), new PropertyMetadata(default(Boolean), OnHasItemsInternalChanged));
        private static void OnHasItemsInternalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            if (sender is TreeDataGridRow source) {
                source.OnHasItemsInternalChanged();
                }
            }

        private void OnHasItemsInternalChanged()
            {
            HasItems = HasItemsInternal;
            }

        public Boolean HasItemsInternal
            {
            get { return (Boolean)GetValue(HasItemsInternalProperty); }
            set { SetValue(HasItemsInternalProperty, value); }
            }
        #endregion
        #region P:IsExpanded:Boolean
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(Boolean), typeof(TreeDataGridRow), new PropertyMetadata(default(Boolean)));
        public Boolean IsExpanded
            {
            get { return (Boolean)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
            }
        #endregion
        #region P:HasItems:Boolean
        private static readonly DependencyPropertyKey HasItemsPropertyKey = DependencyProperty.RegisterReadOnly("HasItems", typeof(Boolean), typeof(TreeDataGridRow), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty HasItemsProperty=HasItemsPropertyKey.DependencyProperty;
        public Boolean HasItems
            {
            get { return (Boolean)GetValue(HasItemsProperty); }
            private set { SetValue(HasItemsPropertyKey, value); }
            }
        #endregion
        }
    }