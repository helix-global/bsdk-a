using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.PlatformUI.Shell.Controls;

namespace BinaryStudio.PlatformUI.Shell
    {
    internal class DraggedTabInfo
        {
        private List<Rect> tabRects = new List<Rect>();
        private Int32 draggedTabPosition = -1;
        private Int32 groupPosition = -1;
        private Thickness expandTabStripMargin = new Thickness(0.0);
        public const Int32 InvalidTabPosition = -1;
        private Rect tabStripRect;
        private ViewElement sibling;
        private ViewElement nestedGroup;
        private Rect virtualTabRect;
        private SplitterLength groupDockedWidth;
        private SplitterLength groupDockedHeight;
        private Double groupFloatingWidth;
        private Double groupFloatingHeight;
        private DocumentGroupContainer groupContainer;

        public ViewElement DraggedViewElement { get; set; }

        public Rect TabStripRect
            {
            get
                {
                return tabStripRect;
                }
            }

        public List<Rect> TabRects
            {
            get
                {
                return tabRects;
                }
            }

        public ViewElement Sibling
            {
            get
                {
                return sibling;
                }
            set
                {
                sibling = value;
                }
            }

        public ViewElement NestedGroup
            {
            get
                {
                return nestedGroup;
                }
            set
                {
                nestedGroup = value;
                }
            }

        public ViewElement TargetElement
            {
            get
                {
                return nestedGroup ?? sibling;
                }
            }

        public Int32 DraggedTabPosition
            {
            get
                {
                return draggedTabPosition;
                }
            set
                {
                draggedTabPosition = value;
                }
            }

        public Rect VirtualTabRect
            {
            get
                {
                return virtualTabRect;
                }
            }

        public SplitterLength GroupDockedWidth
            {
            get
                {
                return groupDockedWidth;
                }
            }

        public SplitterLength GroupDockedHeight
            {
            get
                {
                return groupDockedHeight;
                }
            }

        public Double GroupFloatingWidth
            {
            get
                {
                return groupFloatingWidth;
                }
            }

        public Double GroupFloatingHeight
            {
            get
                {
                return groupFloatingHeight;
                }
            }

        public Int32 GroupPosition
            {
            get
                {
                return groupPosition;
                }
            }

        public DocumentGroupContainer GroupContainer
            {
            get
                {
                return groupContainer;
                }
            }

        public ReorderTabPanel TabStrip { get; set; }

        public Boolean HasBeenReordered { get; set; }

        static DraggedTabInfo()
            {
            EventManager.RegisterClassHandler(typeof(ReorderTabPanel), ReorderTabPanel.PanelLayoutUpdatedEvent, new RoutedEventHandler(OnPanelLayoutUpdated));
            }

        private static void OnPanelLayoutUpdated(Object sender, RoutedEventArgs args)
            {
            var draggedTabInfo = DockManager.Instance.DraggedTabInfo;
            if (draggedTabInfo == null || draggedTabInfo.TabStrip != sender)
                return;
            draggedTabInfo.MeasureTabStrip();
            }

        public void Initialize(ViewElement view)
            {
            sibling = FindFirstSibling(view);
            var parent = view.Parent;
            nestedGroup = null;
            groupPosition = -1;
            groupFloatingHeight = 0.0;
            groupFloatingWidth = 0.0;
            groupContainer = null;
            if (parent == null)
                return;
            if (parent.Parent != null)
                {
                groupPosition = parent.Parent.Children.IndexOf(parent);
                if (parent != null && parent.Children.Count == 1 && parent.Parent.Children.Count == 1)
                    nestedGroup = parent;
                }
            groupDockedWidth = parent.DockedWidth;
            groupDockedHeight = parent.DockedHeight;
            groupFloatingWidth = parent.FloatingWidth;
            groupFloatingHeight = parent.FloatingHeight;
            groupContainer = parent.Parent as DocumentGroupContainer;
            }

        public Rect GetTabRectAt(Point screenPoint)
            {
            var rect = new Rect(0.0, 0.0, 0.0, 0.0);
            foreach (var tabRect in tabRects)
                {
                if (tabRect.Contains(screenPoint))
                    {
                    rect = tabRect;
                    break;
                    }
                }
            return rect;
            }

        public Int32 GetTabIndexAt(Point screenPoint)
            {
            var num = 0;
            foreach (var tabRect in tabRects)
                {
                if (tabRect.Contains(screenPoint))
                    return num;
                ++num;
                }
            return -1;
            }

        public Rect GetClosestTabRectAt(Point screenPoint)
            {
            var rect = GetTabRectAt(screenPoint);
            if (rect.Width == 0.0)
                {
                var num1 = Double.MaxValue;
                foreach (var tabRect in tabRects)
                    {
                    var num2 = AverageDistance(tabRect, screenPoint);
                    if (num2 < num1)
                        {
                        num1 = num2;
                        rect = tabRect;
                        }
                    }
                }
            return rect;
            }

        public Int32 GetClosestTabIndexAt(Point screenPoint)
            {
            var num1 = GetTabIndexAt(screenPoint);
            if (-1 == num1)
                {
                var num2 = Double.MaxValue;
                for (var index = 0; index < tabRects.Count; ++index)
                    {
                    var num3 = AverageDistance(tabRects[index], screenPoint);
                    if (num3 < num2)
                        {
                        num2 = num3;
                        num1 = index;
                        }
                    }
                }
            return num1;
            }

        private static Double AverageDistance(Rect rect, Point point)
            {
            return (DistanceSquare(rect.TopLeft, point) + DistanceSquare(rect.TopRight, point) + DistanceSquare(rect.BottomLeft, point) + DistanceSquare(rect.BottomRight, point)) / 4.0;
            }

        private static Double DistanceSquare(Point point1, Point point2)
            {
            return Math.Pow(point1.X - point2.X, 2.0) + Math.Pow(point1.Y - point2.Y, 2.0);
            }

        public void MoveTabRect(Int32 from, Int32 to)
            {
            if (from < 0 || from >= tabRects.Count)
                throw new ArgumentOutOfRangeException("from: " + @from + " tabRects.count: " + tabRects.Count);
            if (to < 0 || to >= tabRects.Count)
                throw new ArgumentOutOfRangeException("to: " + to + " tabRects.count: " + tabRects.Count);
            var x = tabRects[0].X;
            var tabRect = tabRects[from];
            tabRects.Remove(tabRect);
            tabRects.Insert(to, tabRect);
            RearrangeTabCoordinates(x);
            }

        public void RemoveTabRect(Int32 position)
            {
            if (position < 0 || position >= tabRects.Count)
                throw new ArgumentOutOfRangeException("position: " + position + " tabRects.Count: " + tabRects.Count);
            var x = tabRects[0].X;
            tabRects.RemoveAt(position);
            if (tabRects.Count <= 0)
                return;
            RearrangeTabCoordinates(x);
            }

        public void SetVirtualTabRect(Int32 position)
            {
            if (position < 0 || position >= tabRects.Count)
                throw new ArgumentOutOfRangeException("position: " + position + " tabRects.count: " + tabRects.Count);
            virtualTabRect = tabRects[position];
            }

        public void ClearVirtualTabRect()
            {
            virtualTabRect.X = 0.0;
            virtualTabRect.Y = 0.0;
            virtualTabRect.Width = 0.0;
            virtualTabRect.Height = 0.0;
            }

        public void ExpandTabStrip()
            {
            expandTabStripMargin = TabStrip.ExpandedTearOffMargin;
            ExpandTabStripCore();
            }

        private void ExpandTabStripCore()
            {
            tabStripRect = tabStripRect.Resize(new Vector(-expandTabStripMargin.Left, -expandTabStripMargin.Top), new Vector(expandTabStripMargin.Left + expandTabStripMargin.Right, expandTabStripMargin.Top + expandTabStripMargin.Bottom), new Size(0.0, 0.0), new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            NormalizeTabHeight();
            }

        public Rect MeasureTabStrip()
            {
            if (TabStrip == null)
                throw new InvalidOperationException("TabStrip must be initialized.");
            draggedTabPosition = -1;
            var topLeft = new Point(Double.MaxValue, Double.MaxValue);
            var bottomRight = new Point(Double.MinValue, Double.MinValue);
            tabRects.Clear();
            foreach (UIElement child in TabStrip.Children)
                {
                var tabChild = child as TabItem;
                if (tabChild != null)
                    MeasureTabItem(tabChild, ref topLeft, ref bottomRight);
                }
            if (topLeft.X == Double.MaxValue || topLeft.Y == Double.MaxValue || (bottomRight.X == Double.MinValue || bottomRight.Y == Double.MinValue))
                {
                topLeft.X = 0.0;
                topLeft.Y = 0.0;
                bottomRight.X = 0.0;
                bottomRight.Y = 0.0;
                tabRects.Clear();
                }
            var ancestor = TabStrip.FindAncestor<DockTarget>();
            if (ancestor != null && ancestor.DockTargetType == DockTargetType.Auto && ancestor.IsConnectedToPresentationSource())
                {
                tabStripRect = new Rect(ancestor.PointToScreen(new Point(0.0, 0.0)), DpiHelper.LogicalToDeviceUnits(new Size(ancestor.ActualWidth, ancestor.ActualHeight)));
                }
            else
                {
                var tabStrip = TabStrip as DocumentTabPanel;
                if (tabStrip != null && tabStrip.IsConnectedToPresentationSource())
                    {
                    var rect = DraggedViewElement.IsPinned() ? tabStrip.PinnedTabsRect : tabStrip.UnpinnedTabsRect;
                    topLeft = tabStrip.PointToScreen(rect.TopLeft);
                    bottomRight = tabStrip.PointToScreen(rect.BottomRight);
                    }
                tabStripRect = new Rect(topLeft, bottomRight);
                }
            ExpandTabStripCore();
            return tabStripRect;
            }

        private void MeasureTabItem(TabItem tabChild, ref Point topLeft, ref Point bottomRight)
            {
            if (tabChild.Visibility != Visibility.Visible || !tabChild.IsConnectedToPresentationSource())
                return;
            var screen = tabChild.PointToScreen(new Point(0.0, 0.0));
            var deviceUnits = DpiHelper.LogicalToDeviceUnits(new Size(tabChild.ActualWidth, tabChild.ActualHeight));
            var rect = new Rect(screen, deviceUnits);
            topLeft.X = Math.Min(topLeft.X, screen.X);
            topLeft.Y = Math.Min(topLeft.Y, screen.Y);
            bottomRight.X = Math.Max(bottomRight.X, screen.X + deviceUnits.Width);
            bottomRight.Y = Math.Max(bottomRight.Y, screen.Y + deviceUnits.Height);
            Int32 index;
            for (index = 0; index < tabRects.Count; ++index)
                {
                var tabRect = tabRects[index];
                var num = (tabRect.Y + tabRect.Bottom) / 2.0;
                if (num > rect.Bottom || num < rect.Bottom && num > rect.Top && rect.X < tabRect.X)
                    break;
                }
            tabRects.Insert(index, rect);
            if (tabChild.DataContext == DraggedViewElement)
                {
                draggedTabPosition = index;
                }
            else
                {
                if (draggedTabPosition < index)
                    return;
                draggedTabPosition = draggedTabPosition + 1;
                }
            }

        private void RearrangeTabCoordinates(Double originalX)
            {
            if (tabRects.Count == 0)
                throw new InvalidOperationException("tabRects is empty.");
            var tabRect1 = tabRects[0];
            tabRect1.X = originalX;
            tabRects[0] = tabRect1;
            var num = (tabRect1.Y + tabRect1.Bottom) / 2.0;
            for (var index = 1; index < tabRects.Count; ++index)
                {
                var tabRect2 = tabRects[index];
                if (tabRect2.Top >= num || tabRect2.Bottom <= num)
                    {
                    var rect = tabRect2;
                    rect.X = originalX;
                    tabRects[index] = rect;
                    num = (rect.Y + rect.Bottom) / 2.0;
                    }
                else
                    {
                    tabRect2.X = tabRects[index - 1].X + Math.Round(tabRects[index - 1].Width, 0) + 1.0;
                    tabRects[index] = tabRect2;
                    }
                }
            }

        private void NormalizeTabHeight()
            {
            var val1_1 = 0.0;
            var val1_2 = 0.0;
            var num1 = Double.MinValue;
            var flag1 = false;
            for (var index1 = 0; index1 < tabRects.Count; ++index1)
                {
                var tabRect1 = tabRects[index1];
                if (num1 <= tabRect1.Top || num1 >= tabRect1.Bottom)
                    {
                    num1 = (tabRect1.Top + tabRect1.Bottom) / 2.0;
                    val1_1 = Double.MinValue;
                    val1_2 = Double.MaxValue;
                    var flag2 = index1 == 0;
                    for (var index2 = index1; index2 < tabRects.Count; ++index2)
                        {
                        var tabRect2 = tabRects[index2];
                        if (num1 > tabRect2.Top && num1 < tabRect2.Bottom)
                            {
                            val1_1 = Math.Max(val1_1, tabRect2.Height);
                            val1_2 = Math.Min(val1_2, tabRect2.Y);
                            flag1 = index2 == tabRects.Count - 1;
                            }
                        else
                            break;
                        }
                    if (flag2)
                        {
                        val1_1 += val1_2 - tabStripRect.Y;
                        val1_2 = tabStripRect.Y;
                        }
                    if (flag1)
                        val1_1 += tabStripRect.Bottom - (val1_2 + val1_1);
                    if (!virtualTabRect.IsEmpty)
                        {
                        var num2 = (virtualTabRect.Top + virtualTabRect.Bottom) / 2.0;
                        if (num2 > tabRect1.Top && num2 < tabRect1.Bottom)
                            {
                            virtualTabRect.Y = val1_2;
                            virtualTabRect.Height = val1_1;
                            }
                        }
                    }
                tabRect1.Height = Math.Max(val1_1, 0.0);
                tabRect1.Y = val1_2;
                tabRects[index1] = tabRect1;
                }
            }

        private static ViewElement FindFirstSibling(ViewElement element)
            {
            var viewElement = (ViewElement)null;
            if (element.Parent != null)
                {
                foreach (var child in element.Parent.Children)
                    {
                    if (child != element && !(child is ViewBookmark))
                        {
                        viewElement = child;
                        break;
                        }
                    }
                }
            return viewElement;
            }
        }
    }