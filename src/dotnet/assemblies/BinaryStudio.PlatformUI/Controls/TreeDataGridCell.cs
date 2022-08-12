using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using BinaryStudio.PlatformUI.Extensions;
using Microsoft.Expression.Interactivity.Layout;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridCell : ContentControl
        {
        static TreeDataGridCell()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeDataGridCell), new FrameworkPropertyMetadata(typeof(TreeDataGridCell)));
            }

        #region P:CellValue:Object
        internal static readonly DependencyProperty CellValueProperty = DependencyProperty.Register("CellValue", typeof(Object), typeof(TreeDataGridCell), new PropertyMetadata(default(Object),OnCellValueChanged));
        private static void OnCellValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
            return;
            }

        internal Object CellValue
            {
            get { return (Object)GetValue(CellValueProperty); }
            set { SetValue(CellValueProperty, value); }
            }
        #endregion
        #region P:Column:TreeDataGridColumn
        private static readonly DependencyPropertyKey ColumnPropertyKey = DependencyProperty.RegisterReadOnly("Column", typeof(TreeDataGridColumn), typeof(TreeDataGridCell), new PropertyMetadata(default(TreeDataGridColumn)));
        public static readonly DependencyProperty ColumnProperty = ColumnPropertyKey.DependencyProperty;
        public TreeDataGridColumn Column
            {
            get { return (TreeDataGridColumn)GetValue(ColumnProperty); }
            internal set { SetValue(ColumnPropertyKey, value); }
            }
        #endregion
        #region P:IsSelected:Boolean
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(Boolean), typeof(TreeDataGridCell), new PropertyMetadata(default(Boolean),OnSelectedChanged));
        private static void OnSelectedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is TreeDataGridCell source) {
                source.OnSelectedChanged();
                }
            }

        private void OnSelectedChanged() {
            var layer = AdornerLayer.GetAdornerLayer(this);
            //if (layer != null) {
            //    if (IsSelected) {
            //        layer.Add(CellAdorner = new AdornerContainer(this));
            //        }
            //    else
            //        {
            //        layer.Remove(CellAdorner);
            //        }
            //    }
            }

        public Boolean IsSelected
            {
            get { return (Boolean)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
            }
        #endregion
        #region P:EditTemplate:DataTemplate
        public static readonly DependencyProperty CellEditingTemplateProperty = DependencyProperty.Register("EditTemplate", typeof(DataTemplate), typeof(TreeDataGridCell), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate CellEditingTemplate
            {
            get { return (DataTemplate)GetValue(CellEditingTemplateProperty); }
            set { SetValue(CellEditingTemplateProperty, value); }
            }
        #endregion
        #region P:CellTemplate:DataTemplate
        public static readonly DependencyProperty CellTemplateProperty = DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(TreeDataGridCell), new PropertyMetadata(default(DataTemplate)));
        public DataTemplate CellTemplate
            {
            get { return (DataTemplate)GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
            }
        #endregion

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs" /> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e) {
            if (Focusable)
                {
                Focus();
                }
            else
                {
                var rowC = this.FindAncestor<TreeDataGridCellsPresenter>();
                if (rowC != null) {
                    var index = rowC.ItemContainerGenerator.IndexFromContainer(this);
                    var container = rowC.ItemContainerGenerator.ContainerFromIndex(index + 1);
                    if (container is IInputElement input) {
                        Keyboard.Focus(input);
                        }
                    }
                }
            e.Handled = true;
            }

        /// <summary>Invoked whenever an unhandled <see cref="E:System.Windows.UIElement.GotFocus" /> event reaches this element in its route.</summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs" /> that contains the event data.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
            {
            base.OnGotFocus(e);
            TreeDataGridRowOwner.IsSelected = true;
            Column.DataGridOwner.CurrentCellContainer = this;
            e.Handled = true;
            }

        /// <summary>Raises the <see cref="E:System.Windows.UIElement.LostFocus" /> routed event by using the event data that is provided. </summary>
        /// <param name="e">A <see cref="T:System.Windows.RoutedEventArgs" /> that contains event data. This event data must contain the identifier for the <see cref="E:System.Windows.UIElement.LostFocus" /> event.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
            {
            base.OnLostFocus(e);
            }

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.LostKeyboardFocus" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs" /> that contains event data.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
            {
            Debug.Print($"OnLostKeyboardFocus:{{{e.NewFocus}}}:{{{e.OldFocus}}}");
            base.OnLostKeyboardFocus(e);
            }

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
            {
            base.OnApplyTemplate();
            //AdornerDecorator = this.FindDescendant<AdornerDecorator>();
            }

        /// <summary>Returns the string representation of a <see cref="T:System.Windows.Controls.Control" /> object. </summary>
        /// <returns>A string that represents the control.</returns>
        public override String ToString()
            {
            return $"ColumnIndex:{Column.Header}:{{{Column.Header}}}:{{{DataContext}}}";
            }

        internal TreeDataGridRow TreeDataGridRowOwner;
        private Adorner CellAdorner;
        }
    }