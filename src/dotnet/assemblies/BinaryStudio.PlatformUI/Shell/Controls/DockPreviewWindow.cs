using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell.Controls {
    [TemplatePart(Name = "PART_DocumentGroupControl", Type = typeof(TabControl))]
    [TemplatePart(Name = "PART_InsertedDocumentTabsControl", Type = typeof(TabControl))]
    [TemplatePart(Name = "PART_TabGroupControl", Type = typeof(TabControl))]
    public sealed class DockPreviewWindow : ContentControl, IDockPreviewWindow {
        internal const Double DefaultTabHeight = 25.0;
        internal const Double DefaultTabWidth = 100.0;
        private NestedGroup adornedGroup;
        private ReorderTabPanel previewPanel;
        private ReorderTabPanel insertedTabsPanel;
        private ViewElement floatingElement;
        private Int32 selectedTab;
        private Point screenPoint;
        private DockTargetType dockTargetType;
        private DraggedTabInfo tabInfo;
        private DraggedTabInfo insertedTabInfo;
        private Int32 floatingViewCount;
        private HwndSource hwndWrapper;

        #region P:DeviceTop:Double
        public static readonly DependencyProperty DeviceTopProperty = DependencyProperty.Register("DeviceTop", typeof(Double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(OnPropertyChanged));
        public Double DeviceTop {
            get { return (Double)GetValue(DeviceTopProperty); }
            set { SetValue(DeviceTopProperty, value); }
            }
        #endregion
        #region P:DeviceLeft:Double
        public static readonly DependencyProperty DeviceLeftProperty = DependencyProperty.Register("DeviceLeft", typeof(Double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(OnPropertyChanged));
        public Double DeviceLeft {
            get { return (Double)GetValue(DeviceLeftProperty); }
            set { SetValue(DeviceLeftProperty, value); }
            }
        #endregion
        #region P:ShowTopTab:Boolean
        public static readonly DependencyProperty ShowTopTabProperty = DependencyProperty.Register("ShowTopTab", typeof(Boolean), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnPropertyChanged));
        public Boolean ShowTopTab {
            get { return (Boolean)GetValue(ShowTopTabProperty); }
            set { SetValue(ShowTopTabProperty, Boxes.Box(value)); }
            }
        #endregion
        #region P:ShowBottomTab:Boolean
        public static readonly DependencyProperty ShowBottomTabProperty = DependencyProperty.Register("ShowBottomTab", typeof(Boolean), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnPropertyChanged));
        public Boolean ShowBottomTab {
            get { return (Boolean)GetValue(ShowBottomTabProperty); }
            set { SetValue(ShowBottomTabProperty, Boxes.Box(value)); }
            }
        #endregion
        #region P:TabHeight:Double
        public static readonly DependencyProperty TabHeightProperty = DependencyProperty.Register("TabHeight", typeof(Double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(25.0));
        public Double TabHeight {
            get { return (Double)GetValue(TabHeightProperty); }
            set { SetValue(TabHeightProperty, value); }
            }
        #endregion
        #region P:TabWidth:Double
        public static readonly DependencyProperty TabWidthProperty = DependencyProperty.Register("TabWidth", typeof(Double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(100.0));
        public Double TabWidth {
            get { return (Double)GetValue(TabWidthProperty); }
            set { SetValue(TabWidthProperty, value); }
            }
        #endregion
        #region P:HorizontalTabOffset:Double
        public static readonly DependencyProperty HorizontalTabOffsetProperty = DependencyProperty.Register("HorizontalTabOffset", typeof(Double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(Boxes.DoubleZero));
        public Double HorizontalTabOffset {
            get { return (Double)GetValue(HorizontalTabOffsetProperty); }
            set { SetValue(HorizontalTabOffsetProperty, value); }
            }
        #endregion
        #region P:DeviceWidth:Double
        public static readonly DependencyProperty DeviceWidthProperty = DependencyProperty.Register("DeviceWidth", typeof(Double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(Boxes.DoubleZero, OnDeviceWidthChanged));
        private static void OnDeviceWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = sender as DockPreviewWindow;
            source.Width = DpiHelper.DeviceToLogicalUnitsScalingFactorX * (Double)e.NewValue;
            source.IsChanged = true;
            }
        public Double DeviceWidth {
            get { return (Double)GetValue(DeviceWidthProperty); }
            private set { SetValue(DeviceWidthProperty, value); }
            }
        #endregion
        #region P:DeviceHeight:Double
        public static readonly DependencyProperty DeviceHeightProperty = DependencyProperty.Register("DeviceHeight", typeof(Double), typeof(DockPreviewWindow), new FrameworkPropertyMetadata(Boxes.DoubleZero, OnDeviceHeightChanged));
        private static void OnDeviceHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = sender as DockPreviewWindow;
            source.Height = DpiHelper.DeviceToLogicalUnitsScalingFactorY * (Double)e.NewValue;
            source.IsChanged = true;
            }
        public Double DeviceHeight {
            get { return (Double)GetValue(DeviceHeightProperty); }
            private set { SetValue(DeviceHeightProperty, value); }
            }
        #endregion

        private Boolean IsChanged { get; set; }
        public Int32 InsertPosition { get; private set; }

        static DockPreviewWindow() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockPreviewWindow), new FrameworkPropertyMetadata(typeof(DockPreviewWindow)));
            WidthProperty.OverrideMetadata(typeof(DockPreviewWindow), new FrameworkPropertyMetadata(OnPropertyChanged));
            HeightProperty.OverrideMetadata(typeof(DockPreviewWindow), new FrameworkPropertyMetadata(OnPropertyChanged));
            EventManager.RegisterClassHandler(typeof(ReorderTabPanel), ReorderTabPanel.PanelLayoutUpdatedEvent, new RoutedEventHandler(OnPanelLayoutUpdated));
            }

        private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            (obj as DockPreviewWindow).IsChanged = true;
            }

        private void CreateWindow(IntPtr owner) {
            var parameters = new HwndSourceParameters("DockPreviewWindow");
            var num = -2013265880;
            parameters.Width = (Int32)DeviceWidth;
            parameters.Height = (Int32)DeviceHeight;
            parameters.PositionX = (Int32)DeviceLeft;
            parameters.PositionY = (Int32)DeviceTop;
            parameters.WindowStyle = num;
            parameters.ParentWindow = owner;
            parameters.UsesPerPixelOpacity = true;
            hwndWrapper = new HwndSource(parameters);
            hwndWrapper.ContentRendered += OnContentRendered;
            hwndWrapper.SizeToContent = SizeToContent.Manual;
            hwndWrapper.RootVisual = this;
            TabWidth = 0.0;
            HorizontalTabOffset = 0.0;
            InsertPosition = -1;
            tabInfo = null;
            insertedTabInfo = null;
            IsChanged = false;
            }

        public void Show(IntPtr owner) {
            if (!IsChanged) { return; }
            if (hwndWrapper != null) { Hide(); }
            CreateWindow(owner);
            NativeMethods.SetWindowPos(hwndWrapper.Handle, IntPtr.Zero, (Int32)DeviceLeft, (Int32)DeviceTop, (Int32)DeviceWidth, (Int32)DeviceHeight, 84);
            }

        public void Hide() {
            if (hwndWrapper != null) {
                hwndWrapper.Dispose();
                hwndWrapper = null;
                }
            IsChanged = true;
            InsertPosition = -1;
            tabInfo = null;
            insertedTabInfo = null;
            }

        public void Close() {
            Hide();
            }

        public void SetupDockPreview(SetupDockPreviewArgs args) {
            DeviceLeft = args.previewRect.Left;
            DeviceTop = args.previewRect.Top;
            DeviceWidth = args.previewRect.Width;
            DeviceHeight = args.previewRect.Height;
            screenPoint = args.screenPoint;
            dockTargetType = args.dockTargetType;
            if (DockTargetType.InsertTabPreview == dockTargetType && tabInfo != null) {
                Int32 insertPosition;
                Double tabPosition;
                CalculateInsertAndTabPosition(out insertPosition, out tabPosition);
                if (InsertPosition != insertPosition)
                    MeasurePreviewTab();
                }
            if (args.dockDirection == DockDirection.Fill) {
                floatingElement = args.floatingElement;
                selectedTab = 0;
                if (args.adornedElement.DataContext is DocumentGroup || args.adornedElement.DataContext is DocumentGroupContainer) {
                    ShowTopTab = true;
                    ShowBottomTab = false;
                    }
                else {
                    ShowTopTab = false;
                    ShowBottomTab = true;
                    }
                var ancestor = args.adornedElement.FindAncestor<GroupControl>();
                ReorderTabPanel reorderTabPanel = null;
                if (ancestor != null) {
                    reorderTabPanel = ancestor.GetHeaderPanel() as ReorderTabPanel;
                    adornedGroup = ancestor.DataContext as NestedGroup;
                    selectedTab = adornedGroup.VisibleChildren.IndexOf(adornedGroup.SelectedElement);
                    }
                if (reorderTabPanel != null) {
                    TabHeight = reorderTabPanel.ActualHeight;
                    }
                else {
                    TabWidth = 100.0;
                    TabHeight = 25.0;
                    }
                }
            else {
                ShowTopTab = false;
                ShowBottomTab = false;
                }
            }

        private void MeasurePreviewTab() {
            if (floatingElement == null)
                return;
            previewPanel = null;
            insertedTabsPanel = null;
            TabControl tabControl1 = null;
            TabControl tabControl2 = null;
            if (ShowTopTab) {
                tabControl1 = GetTemplateChild("PART_DocumentGroupControl") as TabControl;
                tabControl2 = GetTemplateChild("PART_InsertedDocumentTabsControl") as TabControl;
                }
            if (ShowBottomTab)
                tabControl1 = GetTemplateChild("PART_TabGroupControl") as TabControl;
            var viewList1 = new List<View>(10);
            var viewList2 = new List<View>(10);
            var viewList3 = new List<View>(floatingElement.FindAll<View>());
            floatingViewCount = viewList3.Count;
            var viewList4 = tabControl2 == null ? viewList1 : viewList2;
            foreach (var view in viewList3)
                viewList4.Add(view);
            if (adornedGroup != null) {
                viewList1.AddRange(adornedGroup.VisibleChildren.OfType<View>());
                }
            if (tabControl1 != null) {
                tabControl1.ItemsSource = viewList1;
                tabControl1.SelectedIndex = selectedTab;
                previewPanel = tabControl1.FindDescendant<ReorderTabPanel>();
                if (previewPanel != null)
                    previewPanel.IsNotificationNeeded = true;
                }
            if (tabControl2 == null)
                return;
            tabControl2.ItemsSource = viewList2;
            insertedTabsPanel = tabControl2.FindDescendant<ReorderTabPanel>();
            if (insertedTabsPanel == null)
                return;
            insertedTabsPanel.IsNotificationNeeded = true;
            }

        private void CalculateInsertAndTabPosition(out Int32 insertPosition, out Double tabPosition) {
            insertPosition = -1;
            tabPosition = 0.0;
            if (!previewPanel.IsConnectedToPresentationSource())
                return;
            if (tabInfo.TabRects.Count == 0) {
                insertPosition = 0;
                tabPosition = previewPanel.PointToScreen(new Point(0.0, 0.0)).X;
                }
            else if (DockTargetType.InsertTabPreview != dockTargetType) {
                if ((!ShowTopTab ? ViewManager.Instance.Preferences.TabDockPreference : ViewManager.Instance.Preferences.DocumentDockPreference) == DockPreference.DockAtBeginning) {
                    insertPosition = 0;
                    tabPosition = previewPanel.PointToScreen(new Point(0.0, 0.0)).X;
                    }
                else {
                    insertPosition = tabInfo.TabRects.Count;
                    tabPosition = tabInfo.TabRects[tabInfo.TabRects.Count - 1].Right;
                    }
                }
            else if (tabInfo.TabRects.Count > 0 && screenPoint.X > tabInfo.TabRects[tabInfo.TabRects.Count - 1].Right) {
                insertPosition = tabInfo.TabRects.Count;
                tabPosition = tabInfo.TabRects[tabInfo.TabRects.Count - 1].Right;
                }
            else {
                insertPosition = tabInfo.GetClosestTabIndexAt(screenPoint);
                tabPosition = tabInfo.TabRects[insertPosition].Left;
                }
            }

        private void OnPanelLayoutUpdated(Object sender) {
            if (previewPanel != null && Equals(previewPanel, sender) && previewPanel.Children.Count > 0) {
                tabInfo = new DraggedTabInfo();
                tabInfo.TabStrip = previewPanel;
                tabInfo.MeasureTabStrip();
                if (insertedTabsPanel == null) {
                    var num = 0.0;
                    for (var index = 0; index < floatingViewCount && tabInfo.TabRects.Count > 0; ++index) {
                        num += tabInfo.TabRects[0].Width;
                        tabInfo.RemoveTabRect(0);
                        }
                    TabWidth = num;
                    }
                }
            if (insertedTabsPanel != null && insertedTabsPanel == sender && insertedTabsPanel.Children.Count > 0) {
                insertedTabInfo = new DraggedTabInfo();
                insertedTabInfo.TabStrip = insertedTabsPanel;
                insertedTabInfo.MeasureTabStrip();
                var num = 0.0;
                for (var index = 0; index < insertedTabInfo.TabRects.Count; ++index)
                    num += insertedTabInfo.TabRects[index].Width;
                TabWidth = num;
                }
            if ((insertedTabsPanel == null || tabInfo == null || insertedTabInfo == null) && (insertedTabsPanel != null || tabInfo == null) || !previewPanel.IsConnectedToPresentationSource())
                return;
            Int32 insertPosition;
            Double tabPosition;
            CalculateInsertAndTabPosition(out insertPosition, out tabPosition);
            InsertPosition = insertPosition;
            var screen = previewPanel.PointToScreen(new Point(0.0, 0.0));
            HorizontalTabOffset = DpiHelper.DeviceToLogicalUnitsScalingFactorX * (tabPosition - screen.X);
            }

        private void OnContentRendered(Object sender, EventArgs e) {
            MeasurePreviewTab();
            }

        private static void OnPanelLayoutUpdated(Object sender, RoutedEventArgs args) {
            var reorderTabPanel = sender as ReorderTabPanel;
            if (reorderTabPanel == null)
                return;
            var ancestor = reorderTabPanel.FindAncestor<DockPreviewWindow>();
            if (ancestor == null)
                return;
            ancestor.OnPanelLayoutUpdated(sender);
            }
        }
    }