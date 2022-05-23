using System;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridRow : TreeViewItem
        {
        static TreeDataGridRow()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeDataGridRow), new FrameworkPropertyMetadata(typeof(TreeDataGridRow)));
            }

        /// <summary>Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement" /> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)" />.</summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
            {
            base.OnPropertyChanged(e);
            }

        /// <summary>Creates a new <see cref="T:System.Windows.Controls.TreeViewItem" /> to use to display the object.</summary>
        /// <returns>A new <see cref="T:System.Windows.Controls.TreeViewItem" />.</returns>
        protected override DependencyObject GetContainerForItemOverride() {
            return new TreeDataGridRow{
                TreeDataGridOwner = TreeDataGridOwner
                };
            }

        //#region P:ItemsSource:Object
        //internal static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(Object), typeof(TreeDataGridRow), new PropertyMetadata(default(Object)));
        //internal Object ItemsSource
        //    {
        //    get { return (Object)GetValue(ItemsSourceProperty); }
        //    set { SetValue(ItemsSourceProperty, value); }
        //    }
        //#endregion
        //#region P:HasItemsInternal:Boolean
        //public static readonly DependencyProperty HasItemsInternalProperty = DependencyProperty.Register("HasItemsInternal", typeof(Boolean), typeof(TreeDataGridRow), new PropertyMetadata(default(Boolean), OnHasItemsInternalChanged));
        //private static void OnHasItemsInternalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //    {
        //    if (sender is TreeDataGridRow source) {
        //        source.OnHasItemsInternalChanged();
        //        }
        //    }

        //private void OnHasItemsInternalChanged()
        //    {
        //    HasItems = HasItemsInternal;
        //    }

        //public Boolean HasItemsInternal
        //    {
        //    get { return (Boolean)GetValue(HasItemsInternalProperty); }
        //    set { SetValue(HasItemsInternalProperty, value); }
        //    }
        //#endregion
        //#region P:IsExpanded:Boolean
        //public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(Boolean), typeof(TreeDataGridRow), new PropertyMetadata(default(Boolean)));
        //public Boolean IsExpanded
        //    {
        //    get { return (Boolean)GetValue(IsExpandedProperty); }
        //    set { SetValue(IsExpandedProperty, value); }
        //    }
        //#endregion
        //#region P:HasItems:Boolean
        //private static readonly DependencyPropertyKey HasItemsPropertyKey = DependencyProperty.RegisterReadOnly("HasItems", typeof(Boolean), typeof(TreeDataGridRow), new PropertyMetadata(default(Boolean)));
        //public static readonly DependencyProperty HasItemsProperty=HasItemsPropertyKey.DependencyProperty;
        //public Boolean HasItems
        //    {
        //    get { return (Boolean)GetValue(HasItemsProperty); }
        //    private set { SetValue(HasItemsPropertyKey, value); }
        //    }
        //#endregion

        internal TreeDataGrid TreeDataGridOwner;
        }
    }