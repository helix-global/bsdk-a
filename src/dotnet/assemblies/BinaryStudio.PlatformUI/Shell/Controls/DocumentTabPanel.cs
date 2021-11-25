using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.PlatformUI.Shell.Controls {
    public class DocumentTabPanel : ReorderTabPanel {
        #region P:HasOverflowItems:Boolean
        private static readonly DependencyPropertyKey HasOverflowItemsPropertyKey = DependencyProperty.RegisterReadOnly("HasOverflowItems", typeof(Boolean), typeof(DocumentTabPanel), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty HasOverflowItemsProperty = HasOverflowItemsPropertyKey.DependencyProperty;
        public Boolean HasOverflowItems {
            get { return (Boolean)GetValue(HasOverflowItemsProperty); }
            private set { SetValue(HasOverflowItemsPropertyKey, Boxes.Box(value)); }
            }
        #endregion
        #region P:PinnedTabsRect:Rect
        private static readonly DependencyPropertyKey PinnedTabsRectPropertyKey = DependencyProperty.RegisterReadOnly("PinnedTabsRect", typeof(Rect), typeof(DocumentTabPanel), new PropertyMetadata(Rect.Empty));
        public static readonly DependencyProperty PinnedTabsRectProperty = PinnedTabsRectPropertyKey.DependencyProperty;
        public Rect PinnedTabsRect {
            get { return (Rect)GetValue(PinnedTabsRectProperty); }
            private set { SetValue(PinnedTabsRectPropertyKey, value); }
            }
        #endregion
        #region P:SeparatePinnedTabsFromUnpinnedTabs:Boolean
        private static readonly DependencyProperty SeparatePinnedTabsFromUnpinnedTabsProperty = DependencyProperty.Register("SeparatePinnedTabsFromUnpinnedTabs", typeof(Boolean), typeof(DocumentTabPanel), new FrameworkPropertyMetadata(Boxes.BooleanFalse, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));
        public Boolean SeparatePinnedTabsFromUnpinnedTabs { get {
            return (Boolean)GetValue(SeparatePinnedTabsFromUnpinnedTabsProperty);
            }}
        #endregion
        #region P:StartNewRowAfterPinnedTabs:Boolean
        private static readonly DependencyPropertyKey StartNewRowAfterPinnedTabsPropertyKey = DependencyProperty.RegisterReadOnly("StartNewRowAfterPinnedTabs", typeof(Boolean), typeof(DocumentTabPanel), new PropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty StartNewRowAfterPinnedTabsProperty = StartNewRowAfterPinnedTabsPropertyKey.DependencyProperty;
        public Boolean StartNewRowAfterPinnedTabs {
            get { return (Boolean)GetValue(StartNewRowAfterPinnedTabsProperty); }
            private set { SetValue(StartNewRowAfterPinnedTabsPropertyKey, Boxes.Box(value)); }
            }
        #endregion
        #region P:UnpinnedTabsRect:Rect
        private static readonly DependencyPropertyKey UnpinnedTabsRectPropertyKey = DependencyProperty.RegisterReadOnly("UnpinnedTabsRect", typeof(Rect), typeof(DocumentTabPanel), new PropertyMetadata(Rect.Empty));
        public static readonly DependencyProperty UnpinnedTabsRectProperty = UnpinnedTabsRectPropertyKey.DependencyProperty;
        public Rect UnpinnedTabsRect {
            get { return (Rect)GetValue(UnpinnedTabsRectProperty); }
            private set { SetValue(UnpinnedTabsRectPropertyKey, value); }
            }
        #endregion

        private Boolean CanRaiseSelectedItemHidden {
            get {
                var documentGroup = DocumentGroup;
                if (documentGroup == null || documentGroup.SelectedElement == null)
                    return false;
                return Equals(documentGroup.SelectedElement.Parent, documentGroup);
                }
            }

        private DocumentGroup DocumentGroup {
            get {
                return (DocumentGroup)DataContext;
                }
            }

        private Boolean DependentCollectionChangedDuringMeasure { get; set; }

        private new Boolean MeasureInProgress { get; set; }

        public static event EventHandler<SelectedItemHiddenEventArgs> SelectedItemHidden;

        public DocumentTabPanel() {
            HorizontalAlignment = HorizontalAlignment.Left;
            BindingOperations.SetBinding(this, SeparatePinnedTabsFromUnpinnedTabsProperty, new Binding
            {
                Source = ViewManager.Instance.Preferences,
                Path = new PropertyPath(ViewManagerPreferences.IsPinnedTabPanelSeparateProperty),
                Mode = BindingMode.OneWay
            });
            }

        #region P:DocumentTabPanel.IsAdjacentToDocumentWell:Boolean
        private static readonly DependencyPropertyKey IsAdjacentToDocumentWellPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsAdjacentToDocumentWell", typeof(Boolean), typeof(DocumentTabPanel), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        public static readonly DependencyProperty IsAdjacentToDocumentWellProperty = IsAdjacentToDocumentWellPropertyKey.DependencyProperty;
        public static Boolean GetIsAdjacentToDocumentWell(View view) {
            Validate.IsNotNull(view, "view");
            return (Boolean)view.GetValue(IsAdjacentToDocumentWellProperty);
            }

        private static void SetIsAdjacentToDocumentWell(View view, Boolean value) {
            Validate.IsNotNull(view, "view");
            view.SetValue(IsAdjacentToDocumentWellPropertyKey, Boxes.Box(value));
            }
        #endregion

        /// <summary>When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class. </summary>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        /// <param name="panelConstraint">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size panelConstraint) {
            var size = Size.Empty;
            MeasureInProgress = true;
            try {
                var num = 0;
                do {
                    DependentCollectionChangedDuringMeasure = false;
                    using (LayoutTracer.Indent(1))
                        size = new MeasureHelper(this, panelConstraint).Measure();
                    if (!DependentCollectionChangedDuringMeasure)
                        break;
                    }
                while (num++ < 5);
                }
            finally {
                MeasureInProgress = false;
                }
            return size;
            }

        /// <summary>When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement" /> derived class. </summary>
        /// <returns>The actual size used.</returns>
        /// <param name="panelConstraint">The final area within the parent that this element should use to arrange itself and its children.</param>
        protected override Size ArrangeOverride(Size panelConstraint) {
            using (LayoutTracer.Indent(1))
                new ArrangeHelper(this, panelConstraint).Arrange();
            return panelConstraint;
            }

        protected override void OnDataContextChanged(Object sender, DependencyPropertyChangedEventArgs e) {
            var oldValue = e.OldValue as DocumentGroup;
            if (oldValue != null) { CollectionChangedEventManager.RemoveListener(oldValue.PinnedViews, this); }
            var newValue = e.NewValue as DocumentGroup;
            if (newValue != null) { CollectionChangedEventManager.AddListener(newValue.PinnedViews, this); }
            base.OnDataContextChanged(sender, e);
            }

        protected override void OnDependentCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
            if (MeasureInProgress) { DependentCollectionChangedDuringMeasure = true; }
            base.OnDependentCollectionChanged(sender, e);
            }

        private static Boolean IsLogicallyEmpty(Size size) {
            if (!size.IsEmpty && !size.Width.IsNearlyEqual(0.0))
                return size.Height.IsNearlyEqual(0.0);
            return true;
            }

        private static Boolean HasZeroArea(Rect rect) {
            if (!rect.IsEmpty && !rect.Width.IsNearlyEqual(0.0))
                return rect.Height.IsNearlyEqual(0.0);
            return true;
            }

        private static class LayoutTracer {
            private static BooleanSwitch _outputSwitch = new BooleanSwitch("DocumentTabPanel", "Dumps DocumentTabPanel diagnostic information");
            private static Boolean _needHeader = true;

            private static String IndentString {
                get {
                    return String.Empty;
                    }
                }

            public static Boolean Enabled {
                get { return _outputSwitch.Enabled; }
                set { _outputSwitch.Enabled = value; }
                }

            public static Boolean Verbose { get; } = false;

            [Conditional("DEBUG")]
            public static void WriteLine() {
                _needHeader = true;
                }

            [Conditional("DEBUG")]
            public static void WriteLine(String format, params Object[] args) {
                _needHeader = true;
                }

            [Conditional("DEBUG")]
            public static void WriteLineIf(Boolean condition, String format, params Object[] args) {
                var num = condition ? 1 : 0;
                }

            [Conditional("DEBUG")]
            public static void Write(String format, params Object[] args) {
                var num = Enabled ? 1 : 0;
                }

            [Conditional("DEBUG")]
            public static void WriteIf(Boolean condition, String format, params Object[] args) {
                var num = condition ? 1 : 0;
                }

            [Conditional("DEBUG")]
            private static void MaybeTraceHeader() {
                if (!_needHeader)
                    return;
                _needHeader = false;
                }

            public static IDisposable Indent(Int32 indentCount = 1) {
                return null;
                }
            }

        private abstract class LayoutHelper {
            protected readonly DocumentTabPanel _panel;
            protected readonly Size _panelConstraint;
            protected readonly List<TabItem> _pinnedTabs;
            protected readonly List<TabItem> _unpinnedTabs;
            protected readonly TabItem _previewTab;

            protected Int32 RowIndex { get; private set; }

            protected abstract Double RemainingWidth { get; }

            protected LayoutHelper(DocumentTabPanel panel, Size panelConstraint) {
                Validate.IsNotNull(panel, "panel");
                _panel = panel;
                _panelConstraint = panelConstraint;
                var observableCollection = panel.DocumentGroup != null ? panel.DocumentGroup.PinnedViews : null;
                var count = panel.InternalChildren.Count;
                var capacity1 = observableCollection != null ? observableCollection.Count : 0;
                var capacity2 = Math.Max(count - capacity1, 0);
                _pinnedTabs = new List<TabItem>(capacity1);
                _unpinnedTabs = new List<TabItem>(capacity2);
                foreach (TabItem internalChild in panel.InternalChildren) {
                    var view = GetView(internalChild);
                    /* TODO: */
                    if (view != null) {
                        if (view.IsPinned)
                            _pinnedTabs.Add(internalChild);
                        else if (DocumentGroup.GetIsPreviewView(view))
                            _previewTab = internalChild;
                        else
                            _unpinnedTabs.Add(internalChild);
                        }
                    }
                }

            protected Boolean TabOverflowsCurrentRow(TabItem tab) {
                return tab.DesiredSize.Width.IsSignificantlyGreaterThan(RemainingWidth);
                }

            protected Boolean TabFitsOnCurrentRow(TabItem tab) {
                return !TabOverflowsCurrentRow(tab);
                }

            protected void StartNewRow() {
                RowIndex = RowIndex + 1;
                }

            protected class TabItemTracer : DisposableObject {
                private TabItem _tab;

                public TracedTabItemState State { get; set; }

                public Size CumulativeSize { get; set; }

                public TabItemTracer(TabItem tab, Int32 index) {
                    _tab = tab;
                    }

                protected override void DisposeManagedResources() {
                    if (_tab == null)
                        return;
                    _tab = null;
                    }

                [Conditional("DEBUG")]
                private void TraceTabItemHeader(Int32 index) {
                    if (!LayoutTracer.Enabled)
                        return;
                    _tab.Header.ToString();
                    }

                [Conditional("DEBUG")]
                private void TraceTabItem() {
                    if (!LayoutTracer.Enabled)
                        return;
                    DocumentGroupControl.GetAccessOrder(_tab);
                    var str1 = "size={0,-13} total size={1,-13} access={2}";
                    if (_tab.IsSelected)
                        State = State | TracedTabItemState.Selected;
                    if (_tab.Visibility != Visibility.Visible)
                        State = State | TracedTabItemState.Hidden;
                    var tab = _tab as DocumentTabItem;
                    if (tab != null) {
                        if (tab.TabState == TabState.Minimized)
                            State = State | TracedTabItemState.Minimized;
                        if (tab.View.IsPinned)
                            State = State | TracedTabItemState.Pinned;
                        }
                    if (State != TracedTabItemState.None) {
                        var str2 = str1 + " ({3})";
                        }
                    if (!LayoutTracer.Verbose || _tab == null)
                        return;
                    using (LayoutTracer.Indent(5))
                        TraceVisualTree(_tab);
                    }

                private static void TraceVisualTree(FrameworkElement element) {
                    if (element == null)
                        return;
                    var str1 = "({0}) size={1}";
                    if (!String.IsNullOrWhiteSpace(element.Name)) {
                        var str2 = "{2} " + str1;
                        }
                    using (LayoutTracer.Indent(1)) {
                        for (var childIndex = 0; childIndex < VisualTreeHelper.GetChildrenCount(element); ++childIndex)
                            TraceVisualTree(VisualTreeHelper.GetChild(element, childIndex) as FrameworkElement);
                        }
                    }
                }

            [Flags]
            protected enum TracedTabItemState {
                None = 0,
                Selected = 1,
                Hidden = 2,
                Minimized = 4,
                Pinned = 8,
                StartNewRow = 16,
                }
            }

        private class MeasureHelper : LayoutHelper {
            private readonly Size _tabConstraint;
            private Int32 _currentTabIndex;
            private Int32 _tabsInCurrentRow;
            private Size _cumulativePanelSize;
            private Size _currentRowSize;

            private Boolean CanStartNewRow {
                get {
                    if (!_panel.StartNewRowAfterPinnedTabs)
                        return HavePinnedTabs;
                    return false;
                    }
                }

            private Boolean CanMinimizePreviewTab {
                get {
                    var previewTab = _previewTab as DocumentTabItem;
                    if (previewTab != null)
                        return previewTab.TabState == TabState.Normal;
                    return false;
                    }
                }

            private Boolean HavePinnedTabs {
                get {
                    return (UInt32)_pinnedTabs.Count > 0U;
                    }
                }

            private Size RemainingSize {
                get {
                    return new Size(RemainingWidth, _panelConstraint.Height);
                    }
                }

            protected override Double RemainingWidth {
                get {
                    return Math.Max(_panelConstraint.Width - _currentRowSize.Width, 0.0);
                    }
                }

            public MeasureHelper(DocumentTabPanel panel, Size panelConstraint)
              : base(panel, panelConstraint) {
                _tabConstraint = new Size(Double.PositiveInfinity, _panelConstraint.Height);
                }

            internal Size Measure() {
                MeasurePinnedTabs();
                MeasurePreviewTab();
                MeasureUnpinnedTabs();
                _cumulativePanelSize.Width = _previewTab == null ? Math.Min(_cumulativePanelSize.Width + _panel.UndockingOffset, _panelConstraint.Width) : _panelConstraint.Width;
                return _cumulativePanelSize;
                }

            private void MeasurePinnedTabs() {
                _panel.StartNewRowAfterPinnedTabs = false;
                foreach (var pinnedTab in _pinnedTabs) {
                    var tab = pinnedTab;
                    var num = _currentTabIndex + 1;
                    _currentTabIndex = num;
                    var index = num;
                    using (var tracer = new TabItemTracer(tab, index)) {
                        pinnedTab.Visibility = Visibility.Visible;
                        pinnedTab.Measure(_tabConstraint);
                        if (TabOverflowsCurrentRow(pinnedTab)) {
                            if (_tabsInCurrentRow == 0)
                                pinnedTab.Measure(RemainingSize);
                            else
                                StartNewRow(tracer);
                            }
                        AddToCurrentRow(pinnedTab, tracer);
                        }
                    }
                if (!_panel.StartNewRowAfterPinnedTabs)
                    _panel.StartNewRowAfterPinnedTabs = _panel.SeparatePinnedTabsFromUnpinnedTabs && _tabsInCurrentRow > 0;
                if (!_panel.StartNewRowAfterPinnedTabs)
                    return;
                StartNewRow(null);
                }

            private void MeasurePreviewTab() {
                if (_previewTab == null)
                    return;
                var previewTab1 = _previewTab;
                var num = _currentTabIndex + 1;
                _currentTabIndex = num;
                var index = num;
                using (var tracer = new TabItemTracer(previewTab1, index)) {
                    var previewTab2 = _previewTab as DocumentTabItem;
                    if (previewTab2 != null)
                        previewTab2.TabState = TabState.Normal;
                    _previewTab.Visibility = Visibility.Visible;
                    _previewTab.Measure(_tabConstraint);
                    if (TabOverflowsCurrentRow(_previewTab)) {
                        if (CanStartNewRow)
                            StartNewRow(tracer);
                        if (TabOverflowsCurrentRow(_previewTab))
                            _previewTab.Measure(RemainingSize);
                        }
                    AddToCurrentRow(_previewTab, tracer);
                    }
                }

            private void MeasureUnpinnedTabs() {
                var flag = false;
                var isFirstUnpinnedTab = true;
                foreach (var unpinnedTab in _unpinnedTabs) {
                    var tab = unpinnedTab;
                    var num = _currentTabIndex + 1;
                    _currentTabIndex = num;
                    var index = num;
                    using (var tracer = new TabItemTracer(tab, index)) {
                        if (!flag) {
                            MeasureUnpinnedTab(unpinnedTab, isFirstUnpinnedTab, tracer);
                            isFirstUnpinnedTab = false;
                            }
                        var visibility = Visibility.Visible;
                        if (flag || TabOverflowsCurrentRow(unpinnedTab)) {
                            visibility = Visibility.Collapsed;
                            flag = true;
                            if (GetView(unpinnedTab).IsSelected && _panel.CanRaiseSelectedItemHidden)
                                RaiseSelectedItemHidden(unpinnedTab);
                            }
                        else
                            AddToCurrentRow(unpinnedTab, tracer);
                        unpinnedTab.Visibility = visibility;
                        }
                    }
                _panel.HasOverflowItems = flag;
                _cumulativePanelSize.Height += _currentRowSize.Height;
                }

            private void RaiseSelectedItemHidden(TabItem selected) {
                var viewsToMove = new List<SelectedItemHiddenEventArgs.ViewIndexChange>();
                var source = _unpinnedTabs.TakeWhile(tab => {
                    if (!Equals(tab, selected))
                        return tab.Visibility == Visibility.Visible;
                    return false;
                });
                var newIndex = _pinnedTabs.Count + source.Count();
                viewsToMove.Add(new SelectedItemHiddenEventArgs.ViewIndexChange(GetView(selected), newIndex));
                if (!TabFitsOnCurrentRow(selected)) {
                    foreach (var tabItem in source.OrderBy(tab => DocumentGroupControl.GetAccessOrder(tab))) {
                        var view = GetView(tabItem);
                        if ((view != null ? view.Parent : null) != null) {
                            viewsToMove.Add(new SelectedItemHiddenEventArgs.ViewIndexChange(view, newIndex--));
                            _currentRowSize.Width = Math.Max(_currentRowSize.Width - tabItem.DesiredSize.Width, 0.0);
                            if (TabFitsOnCurrentRow(selected))
                                break;
                            }
                        }
                    }
                // ISSUE: reference to a compiler-generated field
                SelectedItemHidden.RaiseEvent(null, new SelectedItemHiddenEventArgs(viewsToMove));
                }

            private void MeasureUnpinnedTab(TabItem tab, Boolean isFirstUnpinnedTab, TabItemTracer tracer) {
                tab.Visibility = Visibility.Visible;
                tab.Measure(_tabConstraint);
                if (isFirstUnpinnedTab && _panel.StartNewRowAfterPinnedTabs)
                    tracer.State |= TracedTabItemState.StartNewRow;
                if (TabFitsOnCurrentRow(tab))
                    return;
                if (CanMinimizePreviewTab) {
                    var desiredSize1 = _previewTab.DesiredSize;
                    ((DocumentTabItem)_previewTab).TabState = TabState.Minimized;
                    if (DocumentTabItem.GetEffectiveTabState(_previewTab) == TabState.Minimized) {
                        _previewTab.Measure(new Size(Double.MaxValue, _tabConstraint.Height));
                        var desiredSize2 = _previewTab.DesiredSize;
                        if (desiredSize2 != desiredSize1)
                            _currentRowSize.Width = Math.Max(_currentRowSize.Width - desiredSize1.Width + desiredSize2.Width, 0.0);
                        if (TabFitsOnCurrentRow(tab))
                            return;
                        }
                    }
                if (!isFirstUnpinnedTab)
                    return;
                if (CanStartNewRow) {
                    StartNewRow(tracer);
                    if (_previewTab != null)
                        AddToCurrentRow(_previewTab, tracer);
                    if (TabFitsOnCurrentRow(tab))
                        return;
                    }
                tab.Measure(RemainingSize);
                }

            private void StartNewRow(TabItemTracer tracer = null) {
                _cumulativePanelSize.Height += _currentRowSize.Height;
                _currentRowSize.Width = 0.0;
                _currentRowSize.Height = 0.0;
                _tabsInCurrentRow = 0;
                _panel.StartNewRowAfterPinnedTabs = true;
                if (tracer != null) {
                    tracer.State |= TracedTabItemState.StartNewRow;
                    tracer.CumulativeSize = _cumulativePanelSize;
                    }
                base.StartNewRow();
                }

            private void AddToCurrentRow(TabItem tab, TabItemTracer tracer) {
                _currentRowSize.Width += tab.DesiredSize.Width;
                _currentRowSize.Height = Math.Max(_currentRowSize.Height, tab.DesiredSize.Height);
                _tabsInCurrentRow = _tabsInCurrentRow + 1;
                _cumulativePanelSize.Width = Math.Max(_cumulativePanelSize.Width, _currentRowSize.Width);
                tracer.CumulativeSize = _cumulativePanelSize;
                }
            }

        private class ArrangeHelper : LayoutHelper {
            private Point _origin;

            protected override Double RemainingWidth {
                get {
                    return Math.Max(_panelConstraint.Width - _origin.X, 0.0);
                    }
                }

            public ArrangeHelper(DocumentTabPanel panel, Size panelConstraint)
              : base(panel, panelConstraint) {
                }

            internal void Arrange() {
                ArrangePinnedTabs();
                ArrangeUnpinnedTabs();
                ArrangePreviewTab();
                }

            private void ArrangePinnedTabs() {
                var num1 = 0;
                var num2 = 0.0;
                var tabItem = (TabItem)null;
                foreach (var pinnedTab in _pinnedTabs) {
                    if (_origin.X == 0.0)
                        tabItem = pinnedTab;
                    if (TabOverflowsCurrentRow(pinnedTab)) {
                        StartNewRow(num2);
                        num2 = 0.0;
                        num1 = 0;
                        }
                    ArrangeTab(pinnedTab);
                    num2 = Math.Max(num2, pinnedTab.DesiredSize.Height);
                    ++num1;
                    }
                var flag1 = false;
                var flag2 = _panel.StartNewRowAfterPinnedTabs & (_unpinnedTabs.Count > 0 || _previewTab != null);
                foreach (var pinnedTab in _pinnedTabs) {
                    if (Equals(pinnedTab, tabItem) && !flag2)
                        flag1 = true;
                    SetIsAdjacentToDocumentWell(GetView(pinnedTab), flag1);
                    }
                if (_panel.StartNewRowAfterPinnedTabs)
                    StartNewRow(num2);
                var rect = _origin.Y == 0.0 ? new Rect(0.0, 0.0, _origin.X, num2) : new Rect(0.0, 0.0, _panelConstraint.Width, _origin.Y);
                _panel.PinnedTabsRect = HasZeroArea(rect) ? Rect.Empty : rect;
                }

            private void ArrangeUnpinnedTabs() {
                var origin = _origin;
                var num = 0.0;
                foreach (var unpinnedTab in _unpinnedTabs) {
                    ArrangeTab(unpinnedTab);
                    SetIsAdjacentToDocumentWell(GetView(unpinnedTab), true);
                    num = Math.Max(num, unpinnedTab.DesiredSize.Height);
                    }
                var size = new Size(Math.Max(_panelConstraint.Width - origin.X, 0.0), num);
                var rect = new Rect(origin, size);
                _panel.UnpinnedTabsRect = HasZeroArea(rect) ? Rect.Empty : rect;
                }

            private void ArrangePreviewTab() {
                if (_previewTab == null) {
                    return;
                    }
                _origin.X = _panelConstraint.Width - _previewTab.DesiredSize.Width;
                ArrangeTab(_previewTab);
                SetIsAdjacentToDocumentWell(GetView(_previewTab), true);
                }

            private void ArrangeTab(TabItem tab) {
                if (tab.Visibility != Visibility.Collapsed) {
                    tab.Arrange(new Rect(_origin, tab.DesiredSize));
                    _origin.X += tab.DesiredSize.Width;
                    }
                var documentTabItem = tab as DocumentTabItem;
                if (documentTabItem == null)
                    return;
                DocumentTabItem.SetRowIndex(documentTabItem.View, RowIndex);
                }

            private void StartNewRow(Double currentRowHeight) {
                _origin.X = 0.0;
                _origin.Y += currentRowHeight;
                StartNewRow();
                }
            }
        }
    }
