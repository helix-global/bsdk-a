using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
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
using BinaryStudio.PlatformUI.Controls.Internal;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGrid : TreeView, ITreeDataGridRow
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
                column.DataGridOwner = this;
                column.ColumnIndex = i;
                column.IsLastColumn = i == Columns.Count - 1;
                i++;
                }
            }

        public ObservableCollection<TreeDataGridColumn> Columns { get; }
        #region P:CurrentCellContainer:TreeDataGridCell
        private static readonly DependencyPropertyKey SelectedCellPropertyKey = DependencyProperty.RegisterReadOnly("CurrentCellContainer", typeof(TreeDataGridCell), typeof(TreeDataGrid), new PropertyMetadata(default(TreeDataGridCell), OnSelectedCellChanged));
        public static readonly DependencyProperty CurrentCellContainerProperty = SelectedCellPropertyKey.DependencyProperty;
        private static void OnSelectedCellChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is TreeDataGrid source) {
                source.OnSelectedCellChanged(
                    (TreeDataGridCell)e.OldValue,
                    (TreeDataGridCell)e.NewValue);
                }
            }

        private void OnSelectedCellChanged(TreeDataGridCell o, TreeDataGridCell n)
            {
            if (o != null)
                {
                o.IsSelected = false;
                }
            if (n != null)
                {
                n.IsSelected = true;
                }
            }

        public TreeDataGridCell CurrentCellContainer
            {
            get { return (TreeDataGridCell)GetValue(CurrentCellContainerProperty); }
            internal set { SetValue(SelectedCellPropertyKey, value); }
            }
        #endregion

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
            if (element is TreeDataGridRow e) {
                e.RowIndex = ItemContainerGenerator.IndexFromContainer(element);
                e.ParentRow = this;
                }
            }

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
            {
            base.OnApplyTemplate();
            //ItemsHost = GetTemplateChild("ItemsHost") as TreeView;
            //OnItemsSourceChanged();
            }

        private static TreeDataGridCell GetTailCell(TreeDataGridRow rowD) {
            if (rowD.IsExpanded) {
                return GetTailCell((TreeDataGridRow)rowD.
                    ItemContainerGenerator.
                    ContainerFromIndex(rowD.Items.Count - 1));
                }
            return GetLastCellInRow(rowD);
            }

        private static TreeDataGridCell GetLastCellInRow(TreeDataGridRow rowD) {
            var rowC = rowD.FindDescendant<TreeDataGridCellsPresenter>();
            return (rowC != null)
                ? (TreeDataGridCell)rowC.ItemContainerGenerator.ContainerFromIndex(rowC.Items.Count - 1)
                : null;
            }

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            var FocusedElement = Keyboard.FocusedElement as UIElement;
            var FocusedContainer = FocusedElement.FindAncestorOrSelf<TreeDataGridCell>();
            var columnsCount = Columns.Count;
            switch (e.Key) {
                case Key.Tab:
                    break;
                case Key.Return:
                    break;
                #region Key.Left
                case Key.Left:
                    {
                    if (FocusedContainer != null) {
                        var rowC = FocusedContainer.FindAncestor<TreeDataGridCellsPresenter>();
                        if (rowC != null) {
                            e.Handled = true;
                            var index = rowC.ItemContainerGenerator.IndexFromContainer(FocusedContainer);
                            if (index > 1) {
                                var container = rowC.ItemContainerGenerator.ContainerFromIndex(index - 1);
                                if (container is IInputElement input) {
                                    Keyboard.Focus(input);
                                    }
                                }
                            else if (index == 1) {
                                DependencyObject target = null;
                                var rowD = rowC.FindAncestor<TreeDataGridRow>();
                                if (rowD.Level == 0) {
                                    var rowP = rowD.ParentRow;
                                    var rowI = rowP.ItemContainerGenerator.IndexFromContainer(rowD);
                                    if (rowI != 0)
                                        {
                                        target = GetTailCell((TreeDataGridRow)rowP.ItemContainerGenerator.ContainerFromIndex(rowI - 1));
                                        }
                                    }
                                else
                                    {
                                    var rowP = (TreeDataGridRow)rowD.ParentRow;
                                    var rowI = rowP.ItemContainerGenerator.IndexFromContainer(rowD);
                                    if (rowI != 0)
                                        {
                                        target = GetTailCell((TreeDataGridRow)rowP.ItemContainerGenerator.ContainerFromIndex(rowI - 1));
                                        }
                                    else
                                        {
                                        target = GetLastCellInRow(rowP);
                                        }
                                    }
                                if (target is IInputElement input) {
                                    Keyboard.Focus(input);
                                    }
                                }
                            }
                        }
                    }
                    break;
                #endregion
                #region Key.Right
                case Key.Right:
                    {
                    if (FocusedContainer != null) {
                        var rowC = FocusedContainer.FindAncestor<TreeDataGridCellsPresenter>();
                        if (rowC != null) {
                            e.Handled = true;
                            DependencyObject target = null;
                            var index = rowC.ItemContainerGenerator.IndexFromContainer(FocusedContainer);
                            if (index < columnsCount - 1) {
                                target = rowC.ItemContainerGenerator.ContainerFromIndex(index + 1);
                                }
                            else
                                {
                                var rowD = rowC.FindAncestor<TreeDataGridRow>();
                                var rowP = rowD.ParentRow;
                                if (rowD.IsExpanded) {
                                    var rowT = rowD.ItemContainerGenerator.ContainerFromIndex(0);
                                    if (rowT != null) {
                                        target = rowT.Descendants().OfType<TreeDataGridCell>().FirstOrDefault(i => i.Focusable);
                                        }
                                    }
                                else
                                    {
                                    var rowI = rowP.ItemContainerGenerator.IndexFromContainer(rowD);
                                    var rowT = rowP.ItemContainerGenerator.ContainerFromIndex(rowI + 1);
                                    if (rowT != null) {
                                        target = rowT.Descendants().OfType<TreeDataGridCell>().FirstOrDefault(i => i.Focusable);
                                        }
                                    else
                                        {
                                        while (rowP.ParentRow != null) {
                                            var rowU = rowP.ParentRow;
                                            if (rowU == null) { break; }
                                            rowT = rowU.ItemContainerGenerator.ContainerFromIndex(rowU.ItemContainerGenerator.IndexFromContainer((DependencyObject)rowP) + 1);
                                            if (rowT != null)
                                                {
                                                target = rowT.Descendants().OfType<TreeDataGridCell>().FirstOrDefault(i => i.Focusable);
                                                break;
                                                }
                                            else
                                                {
                                                rowP = rowU;
                                                }
                                            }
                                        }
                                    }
                                }
                            if (target is IInputElement input) {
                                Keyboard.Focus(input);
                                }
                            }
                        }
                    }
                    break;
                #endregion
                case Key.Up:
                case Key.Down:
                    break;
                case Key.End:
                case Key.Home:
                    break;
                case Key.Prior:
                case Key.Next:
                    break;
                }
            }

        /// <summary>Provides class handling for the <see cref="E:System.Windows.UIElement.KeyDown" /> event for a <see cref="T:System.Windows.Controls.TreeView" />.</summary>
        /// <param name="e">The event data.</param>
        protected override void OnKeyDown(KeyEventArgs e) {
            switch (e.Key) {
                case Key.Tab:
                    OnTabKeyDown(e);
                    break;
                case Key.Return:
                    OnEnterKeyDown(e);
                    break;
                case Key.Left:
                case Key.Up:
                case Key.Right:
                case Key.Down:
                    OnArrowKeyDown(e);
                    break;
                case Key.End:
                case Key.Home:
                    OnHomeOrEndKeyDown(e);
                    break;
                case Key.Prior:
                case Key.Next:
                    OnPageUpOrDownKeyDown(e);
                    break;
                }
            if (!e.Handled) { base.OnKeyDown(e); }
            }

        private void OnPageUpOrDownKeyDown(KeyEventArgs e)
            {
            }

        private void OnHomeOrEndKeyDown(KeyEventArgs e)
            {
            }

        private void OnArrowKeyDown(KeyEventArgs e)
            {
            var FocusedElement = Keyboard.FocusedElement as UIElement;
            if (FocusedElement == null) { return; }
            var request = new TraversalRequest(KeyToTraversalDirection(e.Key));
            var r = FocusedElement.MoveFocus(request);
            Debug.Print($"FocusedElement:{r}");
            e.Handled = r;
            }

        private void OnEnterKeyDown(KeyEventArgs e)
            {
            }

        private void OnTabKeyDown(KeyEventArgs e)
            {
            }

        private static FocusNavigationDirection KeyToTraversalDirection(Key key) {
            switch (key) {
                case Key.Left:  return FocusNavigationDirection.Left;
                case Key.Right: return FocusNavigationDirection.Right;
                case Key.Up:    return FocusNavigationDirection.Up;
                default:        return FocusNavigationDirection.Down;
                }
            }

        ITreeDataGridRow ITreeDataGridRow.ParentRow { get { return null; }}
        }
    }
