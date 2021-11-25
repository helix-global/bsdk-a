using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using BinaryStudio.PlatformUI.Controls;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DockManager
        {
        public static readonly RoutedEvent FloatingElementDockedEvent = EventManager.RegisterRoutedEvent("FloatingElementDocked", RoutingStrategy.Direct, typeof(EventHandler<FloatingElementDockedEventArgs>), typeof(DockManager));
        private readonly Dictionary<Visual, DockSite> visualToSite = new Dictionary<Visual, DockSite>();
        private readonly Dictionary<IntPtr, DockSite> hwndToSite = new Dictionary<IntPtr, DockSite>();
        private List<DependencyObject> currentlyDragWithin = new List<DependencyObject>();
        private static DockManager instance;
        private List<View> draggedViews;
        private IDockPreviewWindow dockPreviewWindow;

        public UndockingScope UndockingInformation { get; private set; }

        internal DraggedTabInfo DraggedTabInfo { get; set; }
        internal Boolean IsDragging { get; set; }
        internal Double PreviewDockLength { get; private set; }
        internal Orientation PreviewDockOrientation { get; private set; }
        internal Double UndockedTabItemOffset { get; set; }
        internal Double UndockedTabItemLength { get; set; }

        internal List<View> DraggedViews {
            get
                {
                if (draggedViews == null)
                    draggedViews = new List<View>();
                return draggedViews;
                }
            }

        internal ViewSite DraggedViewSite {
            get
                {
                var view = DraggedViews.FirstOrDefault();
                if (view != null)
                    return ViewElement.FindRootElement(view) as ViewSite;
                return null;
                }
            }

        internal Window DraggedWindow {
            get
                {
                var draggedViewSite = DraggedViewSite as FloatSite;
                if (draggedViewSite == null)
                    return null;
                return ViewManager.Instance.FloatingWindowManager.TryGetFloatingWindow(draggedViewSite);
                }
            }

        internal IntPtr DraggedWindowHandle {
            get
                {
                var draggedWindow = DraggedWindow;
                if (draggedWindow == null)
                    return IntPtr.Zero;
                return new WindowInteropHelper(draggedWindow).Handle;
                }
            }

        internal Boolean IsFloatingOverDockAdorner {
            get
                {
                var flag = false;
                foreach (var dependencyObject in currentlyDragWithin) {
                    var dockAdornerWindow = dependencyObject as DockAdornerWindow;
                    if (dockAdornerWindow != null && dockAdornerWindow.IsVisible) {
                        flag = true;
                        break;
                        }
                    }
                return flag;
                }
            }

        private IDockPreviewWindow DockPreviewWindow {
            get
                {
                if (dockPreviewWindow == null)
                    dockPreviewWindow = CreateDockPreviewWindow();
                return dockPreviewWindow;
                }
            }

        public static DockManager Instance {
            get
                {
                return instance ?? (instance = new DockManager());
                }
            internal set
                {
                instance = value;
                }
            }

        internal void ComputeTabItemLengths(TabItem tabItem) {
            if (tabItem == null)
                throw new ArgumentNullException(nameof(tabItem));
            if (tabItem.DataContext is View && tabItem.IsConnectedToPresentationSource()) {
                var visualOrLogicalParent = tabItem.GetVisualOrLogicalParent() as ReorderTabPanel;
                if (visualOrLogicalParent != null) {
                    var point = visualOrLogicalParent.TransformToDescendant(tabItem).Transform(new Point(0.0, 0.0));
                    Double num1;
                    Double num2;
                    if (visualOrLogicalParent.IsVerticallyOriented) {
                        num1 = -point.Y;
                        num2 = tabItem.ActualHeight;
                        }
                    else {
                        num1 = -point.X;
                        num2 = tabItem.ActualWidth;
                        }
                    UndockedTabItemOffset = num1;
                    UndockedTabItemLength = num2;
                    return;
                    }
                }
            UndockedTabItemOffset = 0.0;
            UndockedTabItemLength = 0.0;
            }

        public UndockingScope CreateUndockingScope(ViewElement element, Point undockingPoint, UndockMode undockMode) {
            if (UndockingInformation != null)
                throw new InvalidOperationException("Only one undocking operation can happen at a time.");
            return new UndockingScope(element, undockingPoint, undockMode);
            }

        public void PerformDrop(DragAbsoluteEventArgs args) {
            var originalSource = args.OriginalSource as DragUndockHeader;
            var floatingWindow = originalSource.FindAncestor<FloatingWindow>();
            var hitElement = FindHitElement(args.ScreenPoint, s => !Equals(s.Visual, floatingWindow));
            if (hitElement != null) {
                var ancestorOrSelf1 = hitElement.VisualHit.FindAncestorOrSelf<DockSiteAdorner>();
                var ancestorOrSelf2 = hitElement.VisualHit.FindAncestorOrSelf<DockAdornerWindow>();
                var dockTarget = hitElement.VisualHit.FindAncestorOrSelf<DockTarget>();
                var dockDirection = DockDirection.Fill;
                var flag = false;
                var createDocumentGroup = false;
                if (floatingWindow != null && IsValidFillPreviewOperation(dockTarget, originalSource.ViewElement)) {
                    dockDirection = DockDirection.Fill;
                    flag = true;
                    }
                if (ancestorOrSelf1 != null && ancestorOrSelf2 != null && ancestorOrSelf2.AdornedElement != null) {
                    dockDirection = ancestorOrSelf1.DockDirection;
                    dockTarget = ancestorOrSelf2.AdornedElement as DockTarget;
                    if (DockOperations.AreDockRestrictionsFulfilled(originalSource.ViewElement, dockTarget.TargetElement)) {
                        flag = true;
                        createDocumentGroup = ancestorOrSelf1.CreatesDocumentGroup;
                        }
                    }
                var viewElement = originalSource.ViewElement;
                var num = 1;
                var content = viewElement.Find(ve => DockOperations.IsDockingViewValid(ve), num != 0);
                if (content == null)
                    flag = false;
                if (flag)
                    dockTarget.RaiseEvent(new FloatingElementDockedEventArgs(FloatingElementDockedEvent, content, dockDirection, createDocumentGroup, DockPreviewWindow.InsertPosition));
                }
            ClearAdorners();
            }

        public void UpdateTargets(DragAbsoluteEventArgs args) {
            var ancestor = ((Visual)args.OriginalSource).FindAncestor<FloatingWindow>();
            UpdateAdorners(args, ancestor);
            UpdateDockPreview(args, ancestor);
            UpdateIsFloatingWindowDragWithin(args, ancestor);
            }

        internal virtual DraggedTabInfo GetAutodockTarget(DragAbsoluteEventArgs args) {
            var draggedTabInfo = (DraggedTabInfo)null;
            var floatingElement = (args.OriginalSource as DragUndockHeader).FindAncestor<FloatingElement>();
            if (DraggedTabInfo != null && DraggedTabInfo.TabStripRect.Contains(args.ScreenPoint)) {
                draggedTabInfo = DraggedTabInfo;
                }
            else {
                var hitElement = FindHitElement(args.ScreenPoint, s =>
                {
                    if (!Equals(s.Visual, floatingElement))
                        return !(s.Visual is DockAdornerWindow);
                    return false;
                });
                if (hitElement != null) {
                    var reorderTabPanel = (ReorderTabPanel)null;
                    for (var sourceElement = (DependencyObject)hitElement.VisualHit; sourceElement != null && reorderTabPanel == null; sourceElement = sourceElement.GetVisualOrLogicalParent()) {
                        var dockTarget = sourceElement as DockTarget;
                        if (dockTarget != null && dockTarget.DockTargetType == DockTargetType.Auto && dockTarget.Visibility == Visibility.Visible)
                            reorderTabPanel = dockTarget.FindDescendant<ReorderTabPanel>();
                        }
                    if (reorderTabPanel != null) {
                        draggedTabInfo = new DraggedTabInfo();
                        draggedTabInfo.TabStrip = reorderTabPanel;
                        draggedTabInfo.MeasureTabStrip();
                        var dataContext = reorderTabPanel.DataContext as ViewGroup;
                        if (dataContext == null)
                            throw new InvalidOperationException("Reorder tab panel should always have ViewGroup as its DataContext.");
                        draggedTabInfo.NestedGroup = dataContext;
                        if (dataContext.VisibleChildren.Count > 0)
                            draggedTabInfo.Sibling = dataContext.VisibleChildren[0];
                        }
                    }
                }
            return draggedTabInfo;
            }

        public void ClearAdorners() {
            ClearAdorners(s => true);
            CloseDockPreview();
            }

        internal void SetDraggedViewElements(ViewElement dragged) {
            if (dragged == null)
                throw new ArgumentNullException(nameof(dragged));
            DraggedViews.Clear();
            DraggedViews.AddRange(dragged.FindAll<View>());
            }

        private void UpdateIsFloatingWindowDragWithin(DragAbsoluteEventArgs args, FloatingWindow floatingWindow) {
            FloatingWindow.SetIsFloatingWindowDragWithin(currentlyDragWithin, false);
            var hitElement = FindHitElement(args.ScreenPoint, s =>
            {
                if (!Equals(s.Visual, floatingWindow))
                    return s.Visual is DockAdornerWindow;
                return false;
            });
            if (hitElement != null) {
                currentlyDragWithin = new List<DependencyObject>(GetHierarchy(hitElement.VisualHit, null));
                FloatingWindow.SetIsFloatingWindowDragWithin(currentlyDragWithin, true);
                }
            else
                currentlyDragWithin.Clear();
            }

        protected virtual IDockPreviewWindow CreateDockPreviewWindow() {
            return new DockPreviewWindow();
            }

        private void HideDockPreview() {
            if (dockPreviewWindow == null)
                return;
            dockPreviewWindow.Hide();
            }

        private void CloseDockPreview() {
            if (dockPreviewWindow != null)
                dockPreviewWindow.Close();
            dockPreviewWindow = null;
            }

        protected virtual void UpdateAdorners(DragAbsoluteEventArgs args, FloatingElement floatingElement, IList<DockSiteType> types, ViewElement element) {
            var validHitElement = FindValidHitElement(args.ScreenPoint, element, types, s =>
            {
                if (!Equals(s.Visual, floatingElement))
                    return !(s.Visual is DockAdornerWindow);
                return false;
            });
            var addedAdorners = (ICollection<DockAdornerWindow>)new List<DockAdornerWindow>();
            if (validHitElement != null) {
                addedAdorners = AddAdorners(validHitElement.DockSite, validHitElement.VisualHit, types, element);
                var ancestorOrSelf = validHitElement.DockSite.Visual.FindAncestorOrSelf<Window>();
                if (ancestorOrSelf != null)
                    AutoZOrderManager.CurrentDragOverWindow = ancestorOrSelf;
                }
            ClearAdorners(visual => !addedAdorners.Contains(visual as DockAdornerWindow));
            }

        private void UpdateAdorners(DragAbsoluteEventArgs args, FloatingWindow floatingWindow) {
            var dataContext = floatingWindow.DataContext as FloatSite;
            if (dataContext == null)
                return;
            var dockSiteTypeList = new List<DockSiteType>();
            dockSiteTypeList.Add(DockSiteType.NonDraggable);
            var child = dataContext.Child;
            if (child == null)
                return;
            if ((child.DockRestriction & DockRestrictionType.Document) != DockRestrictionType.Document && (child.DockRestriction & DockRestrictionType.DocumentGroup) != DockRestrictionType.DocumentGroup)
                dockSiteTypeList.Add(DockSiteType.Default);
            UpdateAdorners(args, floatingWindow, dockSiteTypeList, child);
            }

        private void UpdateDockPreview(DragAbsoluteEventArgs args, FloatingElement floatingElement) {
            var hitElement = FindHitElement(args.ScreenPoint, s => !Equals(s.Visual, floatingElement));
            if (hitElement != null) {
                var ancestorOrSelf1 = hitElement.VisualHit.FindAncestorOrSelf<DockSiteAdorner>();
                var ancestorOrSelf2 = hitElement.VisualHit.FindAncestorOrSelf<DockAdornerWindow>();
                var ancestorOrSelf3 = hitElement.VisualHit.FindAncestorOrSelf<DockTarget>();
                var content = floatingElement.Content as FloatSite;
                var dockDirection = DockDirection.Fill;
                var adornedElement = (FrameworkElement)null;
                if (content == null)
                    throw new InvalidOperationException("Dragging element must be a FloatSite");
                if (content.Child == null)
                    throw new InvalidOperationException("floatSite must have at least one child.");
                var child = content.Child;
                if (IsValidFillPreviewOperation(ancestorOrSelf3, child)) {
                    dockDirection = DockDirection.Fill;
                    adornedElement = ancestorOrSelf3.AdornmentTarget == null ? ancestorOrSelf3 : ancestorOrSelf3.AdornmentTarget;
                    }
                if (ancestorOrSelf1 != null && ancestorOrSelf2 != null && ancestorOrSelf2.AdornedElement != null) {
                    dockDirection = ancestorOrSelf1.DockDirection;
                    adornedElement = ancestorOrSelf2.AdornedElement;
                    if (!ancestorOrSelf1.CreatesDocumentGroup && dockDirection != DockDirection.Fill && adornedElement.DataContext is DocumentGroup)
                        adornedElement = adornedElement.FindAncestor<DocumentGroupContainerControl>();
                    }
                if (adornedElement != null) {
                    DockPreviewWindow.SetupDockPreview(new SetupDockPreviewArgs
                    {
                        previewRect = GetDockPreviewRect(dockDirection, adornedElement, child),
                        dockTargetType = ancestorOrSelf3 != null ? ancestorOrSelf3.DockTargetType : DockTargetType.Outside,
                        screenPoint = args.ScreenPoint,
                        dockDirection = dockDirection,
                        adornedElement = adornedElement,
                        floatingElement = child
                        });
                    DockPreviewWindow.Show(hitElement.DockSite.Handle);
                    }
                else
                    HideDockPreview();
                }
            else
                HideDockPreview();
            }

        protected virtual Boolean IsValidFillPreviewOperation(DockTarget dockTarget, ViewElement dockingView) {
            var flag = false;
            if (dockTarget != null && (dockTarget.DockTargetType == DockTargetType.InsertTabPreview || dockTarget.DockTargetType == DockTargetType.FillPreview && (dockingView.DockRestriction & DockRestrictionType.OutsideView) != DockRestrictionType.OutsideView) && (DockOperations.AreDockRestrictionsFulfilled(dockingView, dockTarget.TargetElement) && dockTarget.TargetElement.AreDockTargetsEnabled))
                flag = true;
            return flag;
            }

        protected void ClearAdorners(Predicate<Visual> includeAdorner) {
            foreach (DockAdornerWindow dockAdornerWindow in new List<DockSite>(visualToSite.Values).Where(s =>
            {
                if (s.Visual is DockAdornerWindow)
                    return includeAdorner(s.Visual);
                return false;
            }).Select(s => s.Visual)) {
                var ancestorOrSelf = dockAdornerWindow.AdornedElement.FindAncestorOrSelf<Window>();
                if (ancestorOrSelf != null)
                    AutoZOrderManager.AdornersCleared(ancestorOrSelf);
                dockAdornerWindow.PrepareAndHide();
                }
            }

        private Rect GetDockPreviewRect(DockDirection dockDirection, FrameworkElement adornedElement, ViewElement element) {
            Rect result;
            var orientation = Orientation.Horizontal;
            var viewElement = adornedElement.DataContext as ViewElement;
            var itemLength = default(SplitterLength);
            if (dockDirection == DockDirection.Top || dockDirection == DockDirection.Bottom) {
                orientation = Orientation.Vertical;
                itemLength = new SplitterLength(element.FloatingHeight);
                }
            else if (dockDirection == DockDirection.Left || dockDirection == DockDirection.Right) {
                orientation = Orientation.Horizontal;
                itemLength = new SplitterLength(element.FloatingWidth);
                }
            if (dockDirection != DockDirection.Fill) {
                SplitterPanel splitterPanel;
                var originalIndex = -1;
                GetPreviewSplitterPanel(out splitterPanel, out originalIndex, dockDirection, viewElement, adornedElement, orientation);
                if (splitterPanel != null && orientation == splitterPanel.Orientation) {
                    result = PreviewDockSameOrientation(dockDirection, splitterPanel, viewElement, itemLength, orientation, originalIndex);
                    }
                else {
                    result = PreviewDockCounterOrientation(dockDirection, adornedElement, viewElement, itemLength, orientation);
                    }
                }
            else {
                result = PreviewDockFill(adornedElement);
                }
            return result;
            }

        private void GetPreviewSplitterPanel(out SplitterPanel panel, out Int32 targetIndex, DockDirection dockDirection, ViewElement viewElement, FrameworkElement adornedElement, Orientation orientation) {
            targetIndex = -1;
            panel = adornedElement.FindAncestor<SplitterPanel>();
            if (panel != null) {
                var ancestor = adornedElement.FindAncestor<SplitterItem>();
                targetIndex = SplitterPanel.GetIndex(ancestor);
                }
            var mainSite = viewElement as MainSite;
            if (mainSite == null)
                return;
            var child = mainSite.Child as DockGroup;
            if (child == null || child.Orientation != orientation)
                return;
            panel = null;
            var reference = (DependencyObject)adornedElement;
            while (panel == null && reference != null) {
                reference = VisualTreeHelper.GetChild(reference, 0);
                panel = reference as SplitterPanel;
                }
            if (panel == null)
                return;
            if (dockDirection == DockDirection.Left || dockDirection == DockDirection.Top)
                targetIndex = 0;
            else
                targetIndex = panel.Children.Count - 1;
            }

        private Rect PreviewDockSameOrientation(DockDirection dockDirection, SplitterPanel panel, ViewElement viewElement, SplitterLength itemLength, Orientation orientation, Int32 originalIndex) {
            var list = new List<UIElement>();
            var splitterItem = new SplitterItem();
            var availableSize = default(Size);
            availableSize.Width = panel.ActualWidth;
            availableSize.Height = panel.ActualHeight;
            SplitterItem splitterItem2 = null;
            foreach (SplitterItem splitterItem3 in panel.Children) {
                list.Add(splitterItem3);
                if (splitterItem3.Content == viewElement) {
                    splitterItem2 = splitterItem3;
                    var num = panel.Children.IndexOf(splitterItem2);
                    }
                }
            Int32 num2;
            if (splitterItem2 == null) {
                num2 = originalIndex;
                }
            else {
                num2 = list.IndexOf(splitterItem2);
                }
            if (dockDirection == DockDirection.Right || dockDirection == DockDirection.Bottom) {
                num2++;
                }
            list.Insert(num2, splitterItem);
            SplitterPanel.SetMaximumLength(splitterItem, (orientation == Orientation.Horizontal) ? (availableSize.Width / 2.0) : (availableSize.Height / 2.0));
            SplitterPanel.SetSplitterLength(splitterItem, itemLength);
            var point = panel.PointToScreen(new Point(0.0, 0.0));
            var list2 = SplitterMeasureData.FromElements(list);
            var splitterMeasureData = list2[num2];
            SplitterPanel.Measure(availableSize, orientation, list2.ToArray(), false);
            PreviewDockOrientation = orientation;
            PreviewDockLength = splitterMeasureData.AttachedLength.Value;
            var result = list2[num2].MeasuredBounds.LogicalToDeviceUnits();
            result.Offset(point.X, point.Y);
            return result;
            }

        private Rect PreviewDockCounterOrientation(DockDirection dockDirection, FrameworkElement adornedElement, ViewElement viewElement, SplitterLength itemLength, Orientation orientation) {
            var list = new List<UIElement>();
            var splitterItem = new SplitterItem();
            var index = 0;
            var availableSize = new Size(adornedElement.ActualWidth, adornedElement.ActualHeight);
            var splitterItem2 = new SplitterItem();
            list.Add(splitterItem2);
            if (dockDirection == DockDirection.Right || dockDirection == DockDirection.Bottom) {
                index = 1;
                }
            list.Insert(index, splitterItem);
            SplitterPanel.SetMaximumLength(splitterItem, (orientation == Orientation.Horizontal) ? (availableSize.Width / 2.0) : (availableSize.Height / 2.0));
            SplitterLength value;
            if (viewElement is MainSite) {
                value = new SplitterLength(1.0, SplitterUnitType.Fill);
                }
            else {
                value = ((orientation == Orientation.Horizontal) ? viewElement.DockedWidth : viewElement.DockedHeight);
                }
            SplitterPanel.SetSplitterLength(splitterItem2, value);
            SplitterPanel.SetSplitterLength(splitterItem, itemLength);
            var point = adornedElement.PointToScreen(new Point(0.0, 0.0));
            var list2 = SplitterMeasureData.FromElements(list);
            var splitterMeasureData = list2[index];
            SplitterPanel.Measure(availableSize, orientation, list2.ToArray(), false);
            PreviewDockOrientation = orientation;
            PreviewDockLength = splitterMeasureData.AttachedLength.Value;
            var result = list2[index].MeasuredBounds.LogicalToDeviceUnits();
            result.Offset(point.X, point.Y);
            return result;
            }

        private Rect PreviewDockFill(FrameworkElement adornedElement) {
            var screen = adornedElement.PointToScreen(new Point(0.0, 0.0));
            var size = new Size();
            if (adornedElement.ActualHeight.IsNonreal() || adornedElement.ActualWidth.IsNonreal()) {
                size = adornedElement.DesiredSize;
                }
            else {
                size.Width = adornedElement.ActualWidth;
                size.Height = adornedElement.ActualHeight;
                }
            return new Rect(screen, DpiHelper.LogicalToDeviceUnits(size));
            }

        private IEnumerable<DependencyObject> GetHierarchy(DependencyObject sourceElement, DependencyObject commonAncestor) {
            while (sourceElement != null) {
                yield return sourceElement;
                sourceElement = sourceElement.GetVisualOrLogicalParent();
                if (Equals(sourceElement, commonAncestor))
                    break;
                }
            }

        protected virtual Boolean IsValidDockTarget(DockTarget target, IList<DockSiteType> types, ViewElement floatingElement) {
            if (target != null && (target.TargetElement == null || target.TargetElement.AreDockTargetsEnabled) && types.Contains(target.DockSiteType))
                return DockOperations.AreDockRestrictionsFulfilled(floatingElement.DockRestriction, target.TargetElement);
            return false;
            }

        protected virtual ICollection<DockAdornerWindow> PrepareAndShowAdornerLayers(IDictionary<DockDirection, DockTarget> targets, DockSite site, ViewElement floatingElement) {
            var dockAdornerWindowSet = new HashSet<DockAdornerWindow>();
            foreach (var target in targets) {
                var adornerLayer = site.GetAdornerLayer(target.Key);
                dockAdornerWindowSet.Add(adornerLayer);
                adornerLayer.AdornedElement = target.Value;
                adornerLayer.DockDirection = target.Key;
                adornerLayer.Orientation = GetTargetOrientation(target.Value);
                adornerLayer.AreOuterTargetsEnabled = floatingElement.DockRestriction == DockRestrictionType.None;
                adornerLayer.AreInnerTargetsEnabled = !IsDocumentGroupContainerTarget(target.Value);
                adornerLayer.IsInnerCenterTargetEnabled = target.Value.DockTargetType != DockTargetType.SidesOnly;
                adornerLayer.AreInnerSideTargetsEnabled = target.Value.DockTargetType != DockTargetType.CenterOnly;
                adornerLayer.PrepareAndShow();
                }
            return dockAdornerWindowSet;
            }

        private ICollection<DockAdornerWindow> AddAdorners(DockSite site, Visual ownerVisual, IList<DockSiteType> types, ViewElement floatingElement) {
            var dictionary = new Dictionary<DockDirection, DockTarget>();
            for (var sourceElement = (DependencyObject)ownerVisual; sourceElement != null; sourceElement = sourceElement.GetVisualOrLogicalParent()) {
                var target = sourceElement as DockTarget;
                if (IsValidDockTarget(target, types, floatingElement)) {
                    // ReSharper disable once PossibleNullReferenceException
                    if (target.DockTargetType == DockTargetType.Outside) {
                        dictionary[DockDirection.Left] = target;
                        dictionary[DockDirection.Right] = target;
                        dictionary[DockDirection.Top] = target;
                        dictionary[DockDirection.Bottom] = target;
                        }
                    else {
                        DockTarget dockTarget;
                        if ((target.DockTargetType == DockTargetType.Inside || target.DockTargetType == DockTargetType.SidesOnly || target.DockTargetType == DockTargetType.CenterOnly) && !dictionary.TryGetValue(DockDirection.Fill, out dockTarget))
                            dictionary[DockDirection.Fill] = target;
                        }
                    }
                }
            return PrepareAndShowAdornerLayers(dictionary, site, floatingElement);
            }

        protected Orientation? GetTargetOrientation(DockTarget target) {
            var nullable = new Orientation?();
            if (target.DockSiteType == DockSiteType.NonDraggable) {
                var documentGroupContainer = target.TargetElement as DocumentGroupContainer;
                if (documentGroupContainer == null) {
                    var targetElement = target.TargetElement as DocumentGroup;
                    if (targetElement != null)
                        documentGroupContainer = targetElement.Parent as DocumentGroupContainer;
                    }
                if (documentGroupContainer != null && documentGroupContainer.VisibleChildren.Count > 1)
                    nullable = documentGroupContainer.Orientation;
                }
            return nullable;
            }

        protected Boolean IsDocumentGroupContainerTarget(DockTarget target) {
            if (target.DockSiteType == DockSiteType.NonDraggable)
                return target.TargetElement is DocumentGroupContainer;
            return false;
            }

        public void RegisterSite(Window window) {
            var hwndSource = PresentationSource.FromDependencyObject(window) as HwndSource;
            if (hwndSource == null)
                window.SourceInitialized += OnSourceInitialized;
            else
                RegisterSite(window, hwndSource.Handle);
            }

        public void RegisterSite(Visual visual, IntPtr hWnd) {
            if (visualToSite.ContainsKey(visual)) {
                if (visualToSite[visual].Handle != hWnd)
                    throw new InvalidOperationException("Visual cannot be used in RegisterSite with two different window handles");
                }
            else {
                var dockSite = new DockSite
                {
                    Handle = hWnd,
                    Visual = visual
                    };
                visualToSite[visual] = dockSite;
                hwndToSite[hWnd] = dockSite;
                }
            }

        private void OnSourceInitialized(Object sender, EventArgs args) {
            var window = (Window)sender;
            window.SourceInitialized -= OnSourceInitialized;
            RegisterSite(window);
            }

        public void UnregisterSite(Visual visual) {
            DockSite dockSite;
            if (!visualToSite.TryGetValue(visual, out dockSite))
                return;
            visualToSite.Remove(visual);
            hwndToSite.Remove(dockSite.Handle);
            }

        protected DockSiteHitTestResult FindHitElement(Point point, Predicate<DockSite> includedSites) {
            using (var enumerator = FindHitElements(point, includedSites).GetEnumerator()) {
                if (enumerator.MoveNext())
                    return enumerator.Current;
                }
            return null;
            }

        private DockSiteHitTestResult FindValidHitElement(Point point, ViewElement viewElement, IList<DockSiteType> types, Predicate<DockSite> includedSites) {
            foreach (var hitElement in FindHitElements(point, includedSites)) {
                for (var sourceElement = (DependencyObject)hitElement.VisualHit; sourceElement != null; sourceElement = sourceElement.GetVisualOrLogicalParent()) {
                    if (IsValidDockTarget(sourceElement as DockTarget, types, viewElement))
                        return hitElement;
                    }
                }
            return null;
            }

        private List<DockSite> GetSortedDockSites() {
            var sortedSites = new List<DockSite>();
            NativeMethods.EnumThreadWindows(NativeMethods.GetCurrentThreadId(), (hWnd, lParam) =>
            {
                NativeMethods.EnumChildWindows(hWnd, (hWndChild, lParamChild) =>
                {
                    AddDockSiteFromHwnd(sortedSites, hWndChild);
                    return true;
                }, IntPtr.Zero);
                AddDockSiteFromHwnd(sortedSites, hWnd);
                return true;
            }, IntPtr.Zero);
            return sortedSites;
            }

        private void AddDockSiteFromHwnd(List<DockSite> sortedSites, IntPtr hWnd) {
            DockSite dockSite;
            if (!hwndToSite.TryGetValue(hWnd, out dockSite))
                return;
            sortedSites.Add(dockSite);
            }

        private IEnumerable<DockSiteHitTestResult> FindHitElements(Point point, Predicate<DockSite> includedSites) {
            var sortedDockSites = GetSortedDockSites();
            var results = new List<DockSiteHitTestResult>();
            foreach (var dockSite in sortedDockSites.Where(w => includedSites(w))) {
                var site = dockSite;
                if (PresentationSource.FromDependencyObject(site.Visual) != null)
                    UtilityMethods.HitTestVisibleElements(site.Visual, result =>
                    {
                        results.Add(new DockSiteHitTestResult(site, (Visual)result.VisualHit));
                        return HitTestResultBehavior.Stop;
                    }, new PointHitTestParameters(site.Visual.PointFromScreen(point)));
                }
            return results;
            }

        public IList<DockAdornerWindow> GetAdornerWindows() {
            var dockAdornerWindowList = new List<DockAdornerWindow>();
            foreach (var dockSite in visualToSite.Values) {
                var visual = dockSite.Visual as DockAdornerWindow;
                if (visual != null)
                    dockAdornerWindowList.Add(visual);
                }
            return dockAdornerWindowList;
            }

        protected class DockSite
            {
            private readonly Dictionary<DockDirection, DockAdornerWindow> adorners = new Dictionary<DockDirection, DockAdornerWindow>();

            public IntPtr Handle { get; set; }

            public Visual Visual { get; set; }

            public DockAdornerWindow GetAdornerLayer(DockDirection type) {
                DockAdornerWindow dockAdornerWindow;
                if (!adorners.TryGetValue(type, out dockAdornerWindow)) {
                    dockAdornerWindow = new DockAdornerWindow(Handle);
                    adorners[type] = dockAdornerWindow;
                    }
                return dockAdornerWindow;
                }
            }

        protected class DockSiteHitTestResult
            {
            public DockSite DockSite { get; }

            public Visual VisualHit { get; }

            public DockSiteHitTestResult(DockSite site, Visual visualHit) {
                DockSite = site;
                VisualHit = visualHit;
                }
            }

        public sealed class UndockingScope : IDisposable
            {
            public ViewElement Element { get; }

            public Point Location { get; }

            public UndockMode UndockMode { get; }

            public UndockingScope(ViewElement element, Point undockingPoint, UndockMode undockMode) {
                Element = element;
                Location = undockingPoint;
                UndockMode = undockMode;
                Instance.UndockingInformation = this;
                }

            public void Dispose() {
                Instance.UndockingInformation = null;
                }
            }
        }
    }