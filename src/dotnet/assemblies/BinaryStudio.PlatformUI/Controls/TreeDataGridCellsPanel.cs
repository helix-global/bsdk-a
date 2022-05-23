using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class TreeDataGridCellsPanel : VirtualizingPanel
        {
        private List<UIElement> _realizedChildren;
        private TreeDataGrid _parentDataGrid;

        private Boolean IsVirtualizing { get;set; }
        private Boolean InRecyclingMode { get;set; }

        private ItemsControl ParentPresenter { get {
            return (TemplatedParent is FrameworkElement frameworkElement)
                    ? frameworkElement.TemplatedParent as ItemsControl
                    : null;
            }}

        private Boolean MeasureDuringArrange { get {
            var pi = typeof(VirtualizingPanel).GetProperty("MeasureDuringArrange", BindingFlags.Instance|BindingFlags.NonPublic);
            return (pi != null) && (Boolean)pi.GetValue(this, null);
            }}

        /// <summary>When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class. </summary>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availableSize)
            {
            Size size = default(Size);
            
            return size;
            }

        private IList RealizedChildren
            {
            get
                {
                if (IsVirtualizing && InRecyclingMode)
                    {
                    return _realizedChildren;
                    }
                return base.InternalChildren;
                }
            }

        private void DisconnectRecycledContainers()
            {
            int num = 0;
            UIElement uIElement = (_realizedChildren.Count > 0) ? _realizedChildren[0] : null;
            UIElementCollection internalChildren = base.InternalChildren;
            int num2 = -1;
            int num3 = 0;
            for (int i = 0; i < internalChildren.Count; i++)
                {
                UIElement uIElement2 = internalChildren[i];
                if (uIElement2 == uIElement)
                    {
                    if (num3 > 0)
                        {
                        RemoveInternalChildRange(num2, num3);
                        i -= num3;
                        num3 = 0;
                        num2 = -1;
                        }
                    num++;
                    uIElement = ((num >= _realizedChildren.Count) ? null : _realizedChildren[num]);
                    }
                else
                    {
                    if (num2 == -1)
                        {
                        num2 = i;
                        }
                    num3++;
                    }
                }
            if (num3 > 0)
                {
                RemoveInternalChildRange(num2, num3);
                }
            }

        private void EnsureRealizedChildren() {
            if (IsVirtualizing && InRecyclingMode) {
                if (_realizedChildren == null)
                    {
                    UIElementCollection internalChildren = base.InternalChildren;
                    _realizedChildren = new List<UIElement>(internalChildren.Count);
                    for (int i = 0; i < internalChildren.Count; i++)
                        {
                        _realizedChildren.Add(internalChildren[i]);
                        }
                    }
                }
            else
                {
                _realizedChildren = null;
                }
            }

        private void DetermineVirtualizationState()
            {
            IsVirtualizing = true;
            InRecyclingMode = true;
            }

        private TreeDataGrid ParentDataGrid { get {
            if (_parentDataGrid == null)
                {
                if (ParentPresenter is TreeDataGridCellsPresenter dataGridCellsPresenter)
                    {
                    TreeDataGridRow dataGridRowOwner = dataGridCellsPresenter.TreeDataGridRowOwner;
                    if (dataGridRowOwner != null)
                        {
                        _parentDataGrid = dataGridRowOwner.TreeDataGridOwner;
                        }
                    }
                else
                    {
                    if (ParentPresenter is TreeDataGridColumnHeadersPresenter dataGridColumnHeadersPresenter)
                        {
                        _parentDataGrid = dataGridColumnHeadersPresenter.TreeDataGridOwner;
                        }
                    }
                }
            return _parentDataGrid;
            }}

        //private Size GenerateAndMeasureChildrenForRealizedColumns(Size constraint)
        //    {
        //    double num = 0.0;
        //    double num2 = 0.0;
        //    TreeDataGrid parentDataGrid = ParentDataGrid;
        //    double averageColumnWidth = parentDataGrid.InternalColumns.AverageColumnWidth;
        //    IItemContainerGenerator itemContainerGenerator = base.ItemContainerGenerator;
        //    List<RealizedColumnsBlock> realizedColumnsBlockList = RealizedColumnsBlockList;
        //    VirtualizeChildren(realizedColumnsBlockList, itemContainerGenerator);
        //    if (realizedColumnsBlockList.Count > 0)
        //        {
        //        int i = 0;
        //        for (int count = realizedColumnsBlockList.Count; i < count; i++)
        //            {
        //            RealizedColumnsBlock realizedColumnsBlock = realizedColumnsBlockList[i];
        //            Size size = GenerateChildren(itemContainerGenerator, realizedColumnsBlock.StartIndex, realizedColumnsBlock.EndIndex, constraint);
        //            num += size.Width;
        //            num2 = Math.Max(num2, size.Height);
        //            if (i != count - 1)
        //                {
        //                num += GetColumnEstimatedMeasureWidthSum(endIndex: realizedColumnsBlockList[i + 1].StartIndex - 1, startIndex: realizedColumnsBlock.EndIndex + 1, averageColumnWidth: averageColumnWidth);
        //                }
        //            }
        //        num += GetColumnEstimatedMeasureWidthSum(0, realizedColumnsBlockList[0].StartIndex - 1, averageColumnWidth);
        //        num += GetColumnEstimatedMeasureWidthSum(realizedColumnsBlockList[realizedColumnsBlockList.Count - 1].EndIndex + 1, parentDataGrid.Columns.Count - 1, averageColumnWidth);
        //        }
        //    else
        //        {
        //        num = 0.0;
        //        }
        //    return new Size(num, num2);
        //    }
        }
    }