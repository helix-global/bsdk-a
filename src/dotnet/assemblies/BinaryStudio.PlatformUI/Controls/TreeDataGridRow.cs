using System;
using System.Windows;
using System.Windows.Controls;
using BinaryStudio.PlatformUI.Controls.Internal;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridRow : TreeViewItem, ITreeDataGridRow
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
                TreeDataGridOwner = TreeDataGridOwner,
                };
            }

        /// <summary>Prepares the specified element to display the specified item. </summary>
        /// <param name="element">The element that displays the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, Object item) {
            base.PrepareContainerForItemOverride(element, item);
            if (element is TreeDataGridRow e) {
                e.Level = Level + 1;
                e.RowIndex = ItemContainerGenerator.IndexFromContainer(element);
                e.ParentRow = this;
                }
            }

        #region P:Level:Int32
        private static readonly DependencyPropertyKey LevelPropertyKey = DependencyProperty.RegisterReadOnly("Level", typeof(Int32), typeof(TreeDataGridRow), new PropertyMetadata(default(Int32)));
        public static readonly DependencyProperty LevelProperty = LevelPropertyKey.DependencyProperty;
        public Int32 Level
            {
            get { return (Int32)GetValue(LevelProperty); }
            internal set { SetValue(LevelPropertyKey, value); }
            }
        #endregion
        #region P:RowIndex:Int32
        private static readonly DependencyPropertyKey RowIndexPropertyKey = DependencyProperty.RegisterReadOnly("RowIndex", typeof(Int32), typeof(TreeDataGridRow), new PropertyMetadata(default(Int32)));
        public static readonly DependencyProperty RowIndexProperty = RowIndexPropertyKey.DependencyProperty;
        public Int32 RowIndex
            {
            get { return (Int32)GetValue(RowIndexProperty); }
            internal set { SetValue(RowIndexPropertyKey, value); }
            }
        #endregion
        

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
            {
            base.OnApplyTemplate();
            }

        /// <summary>Returns the string representation of a <see cref="T:System.Windows.Controls.HeaderedItemsControl" /> object. </summary>
        /// <returns>A string that represents this object.</returns>
        public override String ToString()
            {
            return $"RowIndex:{RowIndex}:{{{DataContext}}}";
            }

        internal TreeDataGrid TreeDataGridOwner;
        internal ITreeDataGridRow ParentRow;
        ITreeDataGridRow ITreeDataGridRow.ParentRow { get { return ParentRow; }}
        }
    }