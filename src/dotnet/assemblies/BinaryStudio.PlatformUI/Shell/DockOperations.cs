using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.PlatformUI.Shell.Controls;
using BinaryStudio.DataProcessing;
using Orientation = System.Windows.Controls.Orientation;

namespace BinaryStudio.PlatformUI.Shell {
    public static class DockOperations {
        public const Int32 InvalidInsertPosition = -1;
        private const Int32 VerticalUndockHeaderTopPadding = 8;
        private const Int32 HorizontalUndockHeaderRightPadding = 50;

        public static Boolean IsDockPositionChanging {
            get {
                return DockEventRaiser.dockEventsInProgress > 0;
                }
            }

        public static DockAction LastDockAction {
            get {
                return DockEventRaiser.lastDockAction;
                }
            }

        public static Boolean IsTogglePinStatusPrevented {
            get {
                return PreventTogglePinStatusScope.IsTogglePinStatusPrevented;
                }
            }

        public static event EventHandler<DockEventArgs> DockPositionChanging;
        public static event EventHandler<DockEventArgs> DockPositionChanged;

        public static void Undock(this ViewElement element, WindowProfile windowProfile, Point undockingPoint, UndockMode undockMode, Rect currentUndockingRect) {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            //if (windowProfile == null)
            //  throw new ArgumentNullException("windowProfile");
            using (new DockEventRaiser(new DockEventArgs(DockAction.Undock, element, true), false)) {
                using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                    using (ViewManager.DeferActiveViewChanges()) {
                        var result = element;
                        var parent = element.Parent;
                        if (IsBookmarkable(element))
                            ReplaceWithBookmark(element, out result, windowProfile);
                        else
                            element.Detach();
                        result = result.GetFloatingStructure(parent);
                        var floatSite = FloatSite.Create();
                        floatSite.Display = result.Display;
                        floatSite.FloatingLeft = result.FloatingLeft;
                        floatSite.FloatingTop = result.FloatingTop;
                        floatSite.FloatingWidth = result.FloatingWidth;
                        floatSite.FloatingHeight = result.FloatingHeight;
                        CalculateUndockPosition(floatSite, undockingPoint, currentUndockingRect);
                        floatSite.Children.Add(result);
                        foreach (var view in result.FindAll<View>())
                            ClearBookmark(view, windowProfile, ViewBookmarkType.Raft);
                        using (DockManager.Instance.CreateUndockingScope(result, undockingPoint, undockMode)) {
                            if (windowProfile != null) {
                                windowProfile.Children.Add(floatSite);
                                }
                            }
                        //floatSite.IsVisible = true;
                        //ViewManager.Instance.FloatingWindowManager.AddFloat(floatSite);
                        }
                    }
                }
            }

        public static void UndockMultiSelection(this DocumentGroup group, WindowProfile windowProfile, Point undockingPoint, Rect currentUndockingRect) {
            if (group == null)
                throw new ArgumentNullException(nameof(@group));
            if (windowProfile == null)
                throw new ArgumentNullException(nameof(windowProfile));
            using (new DockEventRaiser(new DockEventArgs(DockAction.Undock, @group, true), true)) {
                using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                    using (ViewManager.DeferActiveViewChanges()) {
                        var viewElementList = new List<ViewElement>(group.MultiSelectedElements);
                        var selectedElement = group.SelectedElement;
                        var oldParent = selectedElement != null ? selectedElement.Parent : null;
                        foreach (var element in viewElementList) {
                            if (IsBookmarkable(element)) {
                                ViewElement result;
                                ReplaceWithBookmark(element, out result, windowProfile);
                                }
                            else
                                element.Detach();
                            }
                        var documentGroup = DocumentGroup.Create();
                        var floatingStructure = documentGroup.GetFloatingStructure(oldParent);
                        var floatSite = FloatSite.Create();
                        floatSite.FloatingLeft = selectedElement.FloatingLeft;
                        floatSite.FloatingTop = selectedElement.FloatingTop;
                        floatSite.FloatingWidth = selectedElement.FloatingWidth;
                        floatSite.FloatingHeight = selectedElement.FloatingHeight;
                        CalculateUndockPosition(floatSite, undockingPoint, currentUndockingRect);
                        foreach (var viewElement in viewElementList)
                            documentGroup.Children.Add(viewElement);
                        documentGroup.SelectedElement = selectedElement;
                        floatSite.Children.Add(floatingStructure);
                        foreach (var view in floatingStructure.FindAll<View>())
                            ClearBookmark(view, windowProfile, ViewBookmarkType.Raft);
                        using (DockManager.Instance.CreateUndockingScope(floatingStructure, undockingPoint, UndockMode.Normal))
                            windowProfile.Children.Add(floatSite);
                        }
                    }
                }
            }

        public static void Float(this ViewElement element, WindowProfile windowProfile) {
            if (element == null) { throw new ArgumentNullException(nameof(element)); }
            if (windowProfile == null) { throw new ArgumentNullException(nameof(windowProfile)); }
            var view1 = element as View;
            if (view1 != null && DocumentGroup.IsTabbedDocument(element) && FloatSite.IsFloating(element))
                ClearBookmark(view1, windowProfile, ViewBookmarkType.Raft);
            var flag = true;
            foreach (var view2 in element.FindAll<View>()) {
                if (FindBookmark(view2.Name, windowProfile, ViewBookmarkType.Raft) == null) {
                    flag = false;
                    break;
                    }
                }
            if (flag) {
                using (new DockEventRaiser(new DockEventArgs(DockAction.Float, element, true), false)) {
                    using (LayoutSynchronizer.BeginLayoutSynchronization())
                        SnapToBookmark(element, windowProfile, ViewBookmarkType.Raft);
                    }
                }
            else {
                using (new DockEventRaiser(new DockEventArgs(DockAction.Float, element, true), false)) {
                    using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                        var result = element;
                        var floatingWidth = result.FloatingWidth;
                        var floatingHeight = result.FloatingHeight;
                        var viewGroup = result as ViewGroup;
                        if (viewGroup != null && viewGroup.SelectedElement != null) {
                            floatingWidth = viewGroup.SelectedElement.FloatingWidth;
                            floatingHeight = viewGroup.SelectedElement.FloatingHeight;
                            }
                        using (ViewManager.DeferActiveViewChanges()) {
                            var rootElement = ViewElement.FindRootElement(result) as FloatSite;
                            var parent = element.Parent;
                            if (IsBookmarkable(element))
                                ReplaceWithBookmark(element, out result, windowProfile);
                            else
                                element.Detach();
                            result = result.GetFloatingStructure(parent);
                            var point = new Point(result.FloatingLeft, result.FloatingTop);
                            if (rootElement != null)
                                point = UtilityMethods.CalculateFloatingPosition(rootElement, result, windowProfile);
                            var floatSite = FloatSite.Create();
                            floatSite.FloatingWidth = floatingWidth;
                            floatSite.FloatingHeight = floatingHeight;
                            floatSite.FloatingLeft = point.X;
                            floatSite.FloatingTop = point.Y;
                            floatSite.Children.Add(result);
                            windowProfile.Children.Add(floatSite);
                            }
                        foreach (var view2 in result.FindAll<View>())
                            ClearBookmark(view2, windowProfile, ViewBookmarkType.Raft);
                        }
                    }
                }
            }

        public static void FloatMultiSelection(this DocumentGroup group, WindowProfile windowProfile) {
            if (group == null) { throw new ArgumentNullException(nameof(group)); }
            if (windowProfile == null) { throw new ArgumentNullException(nameof(windowProfile)); }
            using (new DockEventRaiser(new DockEventArgs(DockAction.Float, group, true), true)) {
                using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                    using (ViewManager.DeferActiveViewChanges()) {
                        var rootElement = ViewElement.FindRootElement(group) as FloatSite;
                        var selectedElement = group.SelectedElement;
                        var viewElementList = new List<ViewElement>(group.MultiSelectedElements);
                        foreach (var element in viewElementList) {
                            if (IsBookmarkable(element)) {
                                ViewElement result;
                                ReplaceWithBookmark(element, out result, windowProfile);
                                }
                            else
                                element.Detach();
                            }
                        var documentGroup = DocumentGroup.Create();
                        var floatingStructure = documentGroup.GetFloatingStructure(null);
                        var point = new Point(selectedElement.FloatingLeft, selectedElement.FloatingTop);
                        if (rootElement != null)
                            point = UtilityMethods.CalculateFloatingPosition(rootElement, floatingStructure, windowProfile);
                        var floatSite = FloatSite.Create();
                        floatSite.FloatingLeft = point.X;
                        floatSite.FloatingTop = point.Y;
                        floatSite.FloatingWidth = selectedElement.FloatingWidth;
                        floatSite.FloatingHeight = selectedElement.FloatingHeight;
                        foreach (var viewElement in viewElementList)
                            documentGroup.Children.Add(viewElement);
                        documentGroup.SelectedElement = selectedElement;
                        floatSite.Children.Add(floatingStructure);
                        windowProfile.Children.Add(floatSite);
                        foreach (var view in floatingStructure.FindAll<View>())
                            ClearBookmark(view, windowProfile, ViewBookmarkType.Raft);
                        }
                    }
                }
            }

        public static Boolean IsDockingViewValid(ViewElement dockingView) {
            return dockingView is DockGroup || dockingView is TabGroup || dockingView is View || dockingView is DocumentGroup && dockingView.FindAncestor<AutoHideRoot>() != null;
            }

        private static Boolean IsDockTargetValid(ViewElement targetView) {
            if (!(targetView is AutoHideGroup) && !(targetView is AutoHideChannel) && !(targetView is AutoHideRoot))
                return !(targetView is ViewSite);
            return false;
            }

        public static void Dock(this ViewElement targetView, ViewElement dockingView, DockDirection dockDirection) {
            if (targetView == null)
                throw new ArgumentNullException(nameof(targetView));
            if (dockingView == null)
                throw new ArgumentNullException(nameof(dockingView));
            if (dockingView == targetView || dockingView.IsAncestorOf(targetView))
                throw new ArgumentException("targetView and dockingView must not be the same instance, and dockingView cannot be an ancestor of targetView.");
            if (!IsDockingViewValid(dockingView))
                throw new ArgumentException("dockingView is not a dockable ViewElement");
            if (!IsDockTargetValid(targetView))
                throw new ArgumentException("targetView is not a valid target for docking");
            if (dockDirection < DockDirection.Fill || dockDirection > DockDirection.Bottom)
                throw new InvalidEnumArgumentException(nameof(dockDirection), (Int32)dockDirection, typeof(DockDirection));
            if (!AreDockRestrictionsFulfilled(dockingView, targetView))
                throw new ArgumentException("dockingView can not be docked in targetView due to restrictions present in dockingView");
            using (new DockEventRaiser(new DockEventArgs(DockAction.Dock, dockingView, true), false)) {
                using (targetView.PreventCollapse()) {
                    using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                        if (IsBookmarkable(dockingView))
                            ReplaceWithBookmark(dockingView, out dockingView, dockingView.WindowProfile);
                        else
                            dockingView.Detach();
                        using (ViewManager.DeferActiveViewChanges()) {
                            if (dockDirection == DockDirection.Fill)
                                DockNested(GetEffectiveDockTarget(targetView), dockingView);
                            else
                                DockBeside(GetEffectiveDockTarget(targetView), dockingView, dockDirection);
                            }
                        }
                    }
                }
            }

        private static ViewElement GetEffectiveDockTarget(ViewElement targetElement) {
            var view = targetElement as View;
            if (view != null && AutoHideChannel.IsAutoHidden(view)) {
                var bookmark = FindBookmark(view, view.WindowProfile);
                if (bookmark == null) {
                    bookmark = ViewBookmark.Create(view);
                    DockBeside(GetAutoHideCenter(view), bookmark, DockDirection.Bottom);
                    }
                return GetEffectiveDockTarget(bookmark);
                }
            if (targetElement.Parent is NestedGroup)
                return targetElement.Parent;
            return targetElement;
            }

        public static void AutoHide(this ViewElement viewElement) {
            if (viewElement == null)
                throw new ArgumentNullException(nameof(viewElement));
            if (!CanAutoHide(viewElement))
                return;
            using (new DockEventRaiser(new DockEventArgs(DockAction.AutoHide, viewElement, true), false)) {
                var autoHideChannel = FindAutoHideChannel(viewElement);
                ViewElement result;
                var viewElement1 = ReplaceWithBookmark(viewElement, out result, viewElement.WindowProfile);
                var viewList = new List<View>(result.FindAll<View>());
                if (viewList.Count <= 0)
                    return;
                var originalLocation = viewElement1 is ViewBookmark ? viewElement1.Parent : (ViewGroup)viewElement1;
                var existingAutoHideGroup = FindExistingAutoHideGroup(autoHideChannel, originalLocation);
                if (existingAutoHideGroup == null) {
                    existingAutoHideGroup = AutoHideGroup.Create();
                    autoHideChannel.Children.Add(existingAutoHideGroup);
                    }
                foreach (var view in viewList) {
                    view.IsSelected = false;
                    existingAutoHideGroup.Children.Add(view);
                    }
                }
            }

        public static void AutoHideAll(this WindowProfile profile) {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));
            foreach (ViewElement viewElement in new List<View>(profile.FindAll<View>(v => v.IsVisible)))
                viewElement.AutoHide();
            }

        public static void DockViewElementOrGroup(ViewElement element, Boolean autoHideOnlyActiveView) {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (!AutoHideChannel.IsAutoHidden(element))
                throw new ArgumentException("element must be autohidden");
            using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                if (!autoHideOnlyActiveView) {
                    foreach (var element1 in new List<ViewElement>((element.Parent as AutoHideGroup).Children)) {
                        if (element1 != element)
                            element1.SnapToBookmark();
                        }
                    }
                element.SnapToBookmark();
                }
            }

        public static void DockViewElementOrGroup(ViewElement element) {
            DockViewElementOrGroup(element, ViewManager.Instance.Preferences.AutoHideOnlyActiveView);
            }

        public static void AutoHideViewElementOrGroup(ViewElement element, Boolean autoHideOnlyActiveView) {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (!(ViewElement.FindRootElement(element) is ViewSite))
                throw new ArgumentException("element must be docked in a ViewSite.");
            var parent = element.Parent as TabGroup;
            if (!autoHideOnlyActiveView && parent != null) {
                foreach (var viewElement in new List<ViewElement>(parent.Children))
                    viewElement.AutoHide();
                }
            else
                element.AutoHide();
            }

        public static void AutoHideViewElementOrGroup(ViewElement element) {
            AutoHideViewElementOrGroup(element, ViewManager.Instance.Preferences.AutoHideOnlyActiveView);
            }

        private static AutoHideGroup FindExistingAutoHideGroup(AutoHideChannel channel, ViewGroup originalLocation) {
            if (!(originalLocation is TabGroup)) { return null; }
            if (channel == null) { return null; }
            var autoHideGroup = (AutoHideGroup)null;
            foreach (AutoHideGroup child in channel.Children) {
                if (child.OriginalGroup == originalLocation) {
                    autoHideGroup = child;
                    break;
                    }
                }
            return autoHideGroup;
            }

        internal static Boolean CanAutoHide(ViewElement element) {
            var rootElement = ViewElement.FindRootElement(element) as FloatSite;
            if (rootElement != null && !rootElement.HasDocumentGroupContainer || (AutoHideChannel.IsAutoHidden(element) || DocumentGroup.IsTabbedDocument(element)))
                return false;
            if (!(element is View))
                return element is TabGroup;
            return true;
            }

        private static ViewElement GetAutoHideCenter(ViewElement e) {
            var site = ViewElement.FindRootElement(e) as ViewSite;
            if (site != null) {
                return site.Find(AutoHideRoot.GetIsAutoHideCenter, false) ??
                       site.Find<AutoHideRoot>();
                }
            return null;
            }

        private static AutoHideChannel FindAutoHideChannel(ViewElement element) {
            var autoHideCenter = GetAutoHideCenter(element);
            var commonAncestor = element.FindCommonAncestor(autoHideCenter, e => (ViewElement)e.Parent);
            if (commonAncestor == null)
                return null;
            var dockGroup = commonAncestor as DockGroup;
            if (dockGroup == null)
                return null;
            var subtreeIndex1 = FindSubtreeIndex(dockGroup, autoHideCenter);
            var subtreeIndex2 = FindSubtreeIndex(dockGroup, element);
            var dock = (dockGroup.Orientation != Orientation.Horizontal)
                ? (subtreeIndex2 >= subtreeIndex1
                    ? System.Windows.Controls.Dock.Bottom
                    : System.Windows.Controls.Dock.Top)
                : (subtreeIndex2 >= subtreeIndex1
                    ? System.Windows.Controls.Dock.Right
                    : System.Windows.Controls.Dock.Left);
            return GetAutoHideChannel(ViewElement.FindRootElement(element) as ViewSite, dock);
            }

        private static AutoHideChannel GetAutoHideChannel(ViewSite site, Dock dock) {
            foreach (var autoHideChannel in site.FindAll<AutoHideChannel>()) {
                if ((Dock)autoHideChannel.GetValue(DockPanel.DockProperty) == dock)
                    return autoHideChannel;
                }
            return null;
            }

        private static Int32 FindSubtreeIndex(ViewGroup rootElement, ViewElement subtreeElement) {
            while (subtreeElement.Parent != rootElement)
                subtreeElement = subtreeElement.Parent;
            return rootElement.Children.IndexOf(subtreeElement);
            }

        public static void DockAt(this ViewElement targetView, ViewElement dockingView, Int32 dockPosition) {
            if (targetView == null)
                throw new ArgumentNullException(nameof(targetView));
            if (dockingView == null)
                throw new ArgumentNullException(nameof(dockingView));
            if (dockingView == targetView || dockingView.IsAncestorOf(targetView))
                throw new ArgumentException("targetView and dockingView must not be the same instance, and dockingView cannot be an ancestor of targetView.");
            if (!IsDockingViewValid(dockingView))
                throw new ArgumentException("dockingView is not a dockable ViewElement");
            if (!IsDockTargetValid(targetView))
                throw new ArgumentException("targetView is not a valid target for docking");
            if (!AreDockRestrictionsFulfilled(dockingView, targetView))
                throw new ArgumentException("dockingView can not be docked in targetView due to restrictions present in dockingView");
            using (new DockEventRaiser(new DockEventArgs(DockAction.Dock, dockingView, true), false)) {
                using (targetView.PreventCollapse()) {
                    using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                        var result = dockingView;
                        if (IsBookmarkable(dockingView))
                            ReplaceWithBookmark(dockingView, out result, dockingView.WindowProfile);
                        else
                            dockingView.Detach();
                        using (ViewManager.DeferActiveViewChanges())
                            DockNested(GetEffectiveDockTarget(targetView), result, dockPosition);
                        }
                    }
                }
            }

        public static DocumentGroup GetDocumentGroup(DocumentGroupContainer container) {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            foreach (DocumentGroup child in container.Children) {
                if (child.IsVisible)
                    return child;
                }
            return container.Children.FirstOrDefault() as DocumentGroup;
            }

        public static DocumentGroup GetPrimaryDocumentGroup(WindowProfile profile) {
            if (profile == null)
                throw new ArgumentNullException(nameof(profile));
            var documentGroup = (DocumentGroup)null;
            var mainSite1 = profile.Find<MainSite>();
            var viewSite1 = (ViewSite)null;
            if (ViewManager.Instance.ActiveView != null && ViewManager.Instance.ActiveView.WindowProfile == profile)
                viewSite1 = ViewElement.FindRootElement(ViewManager.Instance.ActiveView) as ViewSite;
            if (viewSite1 != null) {
                var viewSite2 = viewSite1;
                var num = 0;
                documentGroup = viewSite2.Find<DocumentGroup>(e => e.IsVisible, num != 0);
                }
            if (documentGroup == null && mainSite1 != null) {
                var mainSite2 = mainSite1;
                var num = 0;
                documentGroup = mainSite2.Find<DocumentGroup>(e => e.IsVisible, num != 0);
                }
            if (documentGroup == null)
                documentGroup = profile.Find<DocumentGroup>(e => e.IsVisible);
            if (documentGroup == null && viewSite1 != null)
                documentGroup = viewSite1.Find<DocumentGroup>(false);
            if (documentGroup == null && mainSite1 != null)
                documentGroup = mainSite1.Find<DocumentGroup>(false);
            if (documentGroup == null)
                documentGroup = profile.Find<DocumentGroup>();
            return documentGroup;
            }

        public static DocumentGroup GetMainSiteDocumentGroup(WindowProfile windowProfile) {
            var mainSite = windowProfile.Find<MainSite>();
            if (mainSite == null)
                return null;
            return mainSite.Find<DocumentGroup>(false);
            }

        private static void DockNested(ViewElement targetView, ViewElement dockingView) {
            DockNested(targetView, dockingView, -1);
            }

        private static void DockNested(ViewElement targetView, ViewElement dockingView, Int32 dockPosition) {
            var stackedParent = GetStackedParent(targetView);
            if (-1 != dockPosition && (dockPosition < 0 || dockPosition > stackedParent.VisibleChildren.Count))
                throw new ArgumentOutOfRangeException("dockPosition is out of range. parent.visiblechildren: " + stackedParent.VisibleChildren.Count + ", dockPosition: " + dockPosition);
            using (stackedParent.PreventCollapse()) {
                var viewList = new List<View>(dockingView.FindAll<View>());
                foreach (var view in viewList)
                    ClearBookmark(view, targetView.WindowProfile, targetView.GetBookmarkType());
                if ((!(stackedParent is DocumentGroup) ? ViewManager.Instance.Preferences.TabDockPreference : ViewManager.Instance.Preferences.DocumentDockPreference) == DockPreference.DockAtBeginning && dockPosition == -1)
                    dockPosition = 0;
                var view1 = (View)null;
                var view2 = (View)null;
                var view3 = (View)null;
                var index = stackedParent.ChildIndexFromVisibleChildIndex(dockPosition);
                foreach (var view4 in viewList) {
                    if (view4 == ViewManager.Instance.ActiveView)
                        view3 = view4;
                    else if (view4.IsSelected)
                        view2 = view4;
                    else if (view4.IsVisible)
                        view1 = view4;
                    view4.Detach();
                    if (-1 != dockPosition) {
                        stackedParent.Children.Insert(index, view4);
                        ++index;
                        }
                    else
                        stackedParent.Children.Add(view4);
                    }
                var nestedGroup = stackedParent;
                var viewElement = (ViewElement)view3;
                if (viewElement == null) {
                    var view4 = view2;
                    if (view4 == null) {
                        var view5 = view1;
                        viewElement = view5 != null ? view5 : stackedParent.SelectedElement;
                        }
                    else
                        viewElement = view4;
                    }
                nestedGroup.SelectedElement = viewElement;
                }
            }

        public static void MoveTab(ViewElement tab, Int32 newIndex, Boolean selectAfterMoving = true) {
            Validate.IsNotNull(tab, "tab");
            if (!(tab is View))
                throw new ArgumentException("Tab must be a View");
            var parent = tab.Parent;
            if (parent == null)
                throw new ArgumentException("tab should have a parent");
            if (!(parent is NestedGroup))
                throw new ArgumentException("Parent of tab must be a NestedGroup.");
            if (newIndex < 0 || newIndex > parent.VisibleChildren.Count - 1)
                throw new ArgumentOutOfRangeException("newIndex: " + newIndex + " parent.visiblechildren: " + parent.VisibleChildren.Count);
            var visiblePosition = parent.VisibleChildren.IndexOf(tab);
            if (newIndex == visiblePosition)
                return;
            var oldIndex = parent.ChildIndexFromVisibleChildIndex(visiblePosition);
            newIndex = parent.ChildIndexFromVisibleChildIndex(newIndex);
            if (!parent.CanMoveTab(oldIndex, newIndex))
                return;
            using (new DockEventRaiser(new DockEventArgs(DockAction.ReorderTab, tab, true), false)) {
                using (parent.PreventCollapse()) {
                    using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                        using (ViewManager.DeferActiveViewChanges()) {
                            var documentGroup = parent as DocumentGroup;
                            if (documentGroup != null && documentGroup.PreviewView == tab)
                                documentGroup.PreviewView = null;
                            parent.Children.Move(oldIndex, newIndex);
                            if (!selectAfterMoving)
                                return;
                            parent.SelectedElement = tab;
                            }
                        }
                    }
                }
            }

        public static void MovePinnedTab(this ViewElement tab, Int32 position) {
            if (tab == null)
                throw new ArgumentNullException(nameof(tab));
            var view = tab as View;
            if (view == null)
                throw new ArgumentException("Tab must be a View.");
            if (!view.IsPinned)
                throw new ArgumentException("Tab must be pinned.");
            var parent = tab.Parent as DocumentGroup;
            if (parent == null)
                throw new ArgumentException("tab should have a DocumentGroup parent.");
            if (position == parent.PinnedViews.IndexOf(tab))
                return;
            if (position < 0 || position > parent.PinnedViews.Count - 1)
                throw new ArgumentOutOfRangeException("position: " + position + " group.PinnedViews: " + parent.PinnedViews.Count);
            using (new DockEventRaiser(new DockEventArgs(DockAction.ReorderTab, tab, true), false)) {
                using (parent.PreventCollapse()) {
                    using (PreventTogglePinStatus()) {
                        using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                            using (ViewManager.DeferActiveViewChanges()) {
                                parent.MovePinnedView(tab, position);
                                parent.SelectedElement = tab;
                                }
                            }
                        }
                    }
                }
            }

        private static TabType GetSelectedElementGroupTabType(NestedGroup group) {
            var selectedElement = group.SelectedElement as View;
            if (selectedElement != null && selectedElement.IsPinned)
                return TabType.Pinned;
            var documentGroup = group as DocumentGroup;
            return documentGroup != null && documentGroup.SelectedElement == documentGroup.PreviewView ? TabType.Preview : TabType.Default;
            }

        public static void SelectNextVisibleTab(NestedGroup group) {
            if (group == null)
                throw new ArgumentNullException(nameof(@group));
            var view = (View)null;
            var group1 = group as DocumentGroup;
            switch (GetSelectedElementGroupTabType(group)) {
                case TabType.Default:
                view = GetNextVisibleRegularTab(group) ?? GetVisiblePreviewTab(group1) ?? GetFirstVisiblePinnedTab(group1) ?? GetFirstVisibleRegularTab(group);
                break;
                case TabType.Pinned:
                view = GetNextVisiblePinnedTab(group1) ?? GetFirstVisibleRegularTab(group) ?? GetVisiblePreviewTab(group1) ?? GetFirstVisiblePinnedTab(group1);
                break;
                case TabType.Preview:
                view = GetFirstVisiblePinnedTab(group1) ?? GetFirstVisibleRegularTab(group);
                break;
                }
            if (view == null)
                return;
            view.IsSelected = true;
            }

        public static void SelectPreviousVisibleTab(NestedGroup group) {
            if (group == null)
                throw new ArgumentNullException(nameof(@group));
            var view = (View)null;
            var group1 = group as DocumentGroup;
            switch (GetSelectedElementGroupTabType(group)) {
                case TabType.Default:
                view = GetPreviousVisibleRegularTab(group) ?? GetLastVisiblePinnedTab(group1) ?? GetVisiblePreviewTab(group1) ?? GetLastVisibleRegularTab(group);
                break;
                case TabType.Pinned:
                view = GetPreviousVisiblePinnedTab(group1) ?? GetVisiblePreviewTab(group1) ?? GetLastVisibleRegularTab(group) ?? GetLastVisiblePinnedTab(group1);
                break;
                case TabType.Preview:
                view = GetLastVisibleRegularTab(group) ?? GetLastVisiblePinnedTab(group1);
                break;
                }
            if (view == null)
                return;
            view.IsSelected = true;
            }

        private static Boolean IsRegularTab(View view) {
            if (view != null && !view.IsPinned)
                return !DocumentGroup.GetIsPreviewView(view);
            return false;
            }

        private static View GetNextVisibleView(IObservableCollection<ViewElement> elements, View current, IsContainedInElements isContained = null) {
            if (elements.Count <= 1)
                return null;
            var index = elements.IndexOf(current) + 1;
            if (index < 0 || index >= elements.Count)
                return null;
            View element;
            for (element = elements[index] as View; element == null || !DocumentTabItem.GetIsTabVisible(element) || isContained != null && !isContained(element); element = elements[index] as View) {
                ++index;
                if (index >= elements.Count)
                    return null;
                }
            return element;
            }

        private static View GetPreviousVisibleView(IObservableCollection<ViewElement> elements, View current, IsContainedInElements isContained = null) {
            if (elements.Count <= 1)
                return null;
            var index = elements.IndexOf(current) - 1;
            if (index < 0 || index >= elements.Count)
                return null;
            View element;
            for (element = elements[index] as View; element == null || !DocumentTabItem.GetIsTabVisible(element) || isContained != null && !isContained(element); element = elements[index] as View) {
                --index;
                if (index < 0)
                    return null;
                }
            return element;
            }

        private static View GetFirstVisibleView(IObservableCollection<ViewElement> elements, IsContainedInElements isContained = null) {
            if (elements.Count > 0) {
                var index = 0;
                View view;
                for (view = elements[index] as View; view == null || !DocumentTabItem.GetIsTabVisible(view) || isContained != null && !isContained(view); view = elements[index] as View) {
                    ++index;
                    if (index >= elements.Count) {
                        view = null;
                        break;
                        }
                    }
                if (view != null && DocumentTabItem.GetIsTabVisible(view))
                    return view;
                }
            return null;
            }

        private static View GetLastVisibleView(IObservableCollection<ViewElement> elements, IsContainedInElements isContained = null) {
            if (elements.Count > 0) {
                var index = elements.Count - 1;
                View view;
                for (view = elements[index] as View; view == null || !DocumentTabItem.GetIsTabVisible(view) || isContained != null && !isContained(view); view = elements[index] as View) {
                    --index;
                    if (index < 0) {
                        view = null;
                        break;
                        }
                    }
                if (view != null && DocumentTabItem.GetIsTabVisible(view))
                    return view;
                }
            return null;
            }

        private static View GetFirstVisibleRegularTab(NestedGroup group) {
            return GetFirstVisibleView(group.VisibleChildren, IsRegularTab);
            }

        private static View GetLastVisibleRegularTab(NestedGroup group) {
            return GetLastVisibleView(group.VisibleChildren, IsRegularTab);
            }

        private static View GetNextVisibleRegularTab(NestedGroup group) {
            return GetNextVisibleView(group.VisibleChildren, group.SelectedElement as View, IsRegularTab);
            }

        private static View GetPreviousVisibleRegularTab(NestedGroup group) {
            return GetPreviousVisibleView(group.VisibleChildren, group.SelectedElement as View, IsRegularTab);
            }

        private static View GetFirstVisiblePinnedTab(DocumentGroup group) {
            if (group == null)
                return null;
            return GetFirstVisibleView(group.PinnedViews, null);
            }

        private static View GetLastVisiblePinnedTab(DocumentGroup group) {
            if (group == null)
                return null;
            return GetLastVisibleView(group.PinnedViews, null);
            }

        private static View GetNextVisiblePinnedTab(DocumentGroup group) {
            if (group == null)
                return null;
            return GetNextVisibleView(group.PinnedViews, group.SelectedElement as View, null);
            }

        private static View GetPreviousVisiblePinnedTab(DocumentGroup group) {
            if (group == null)
                return null;
            return GetPreviousVisibleView(group.PinnedViews, group.SelectedElement as View, null);
            }

        private static View GetVisiblePreviewTab(DocumentGroup group) {
            if (group == null)
                return null;
            return group.PreviewView;
            }

        public static DocumentGroup CreateDocumentGroupAt(DocumentGroupContainer container, Int32 position) {
            if (container == null)
                throw new ArgumentNullException(nameof(container));
            if (position < 0 || position > container.Children.Count)
                throw new ArgumentOutOfRangeException("position: " + position);
            var documentGroup = DocumentGroup.Create();
            container.Children.Insert(position, documentGroup);
            return documentGroup;
            }

        public static Boolean AreDockRestrictionsFulfilled(ViewElement element, ViewElement target) {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            return AreDockRestrictionsFulfilled(element.DockRestriction, target);
            }

        public static Boolean AreDockRestrictionsFulfilled(DockRestrictionType restriction, ViewElement target) {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (restriction == DockRestrictionType.AlwaysFloating)
                return false;
            var view = target as View;
            if (view != null && DocumentGroup.GetIsPreviewView(view))
                return false;
            if (restriction == DockRestrictionType.None || (restriction & DockRestrictionType.DocumentGroup) == DockRestrictionType.DocumentGroup && (target is DocumentGroup || target is DocumentGroupContainer) || (restriction & DockRestrictionType.Document) == DockRestrictionType.Document && DocumentGroup.IsTabbedDocument(target))
                return true;
            if ((restriction & DockRestrictionType.OutsideView) == DockRestrictionType.OutsideView) {
                var rootElement = ViewElement.FindRootElement(target) as FloatSite;
                if (rootElement != null && rootElement.Find<DocumentGroupContainer>(false) == null)
                    return true;
                }
            return false;
            }

        private static NestedGroup GetStackedParent(ViewElement view) {
            var container = view as DocumentGroupContainer;
            NestedGroup nestedGroup;
            if (container != null) {
                if (container.Children.Count == 0)
                    throw new InvalidOperationException("A DocumentGroupContainer must always have at least one child");
                nestedGroup = GetDocumentGroup(container);
                }
            else {
                nestedGroup = view as NestedGroup;
                if (nestedGroup == null) {
                    nestedGroup = view.Parent as NestedGroup;
                    if (nestedGroup == null) {
                        nestedGroup = TabGroup.Create();
                        CopySize(view, nestedGroup);
                        view.InsertNewParent(nestedGroup);
                        }
                    }
                }
            return nestedGroup;
            }

        private static void CopySize(ViewElement source, ViewElement target) {
            target.DockedWidth = source.DockedWidth;
            target.DockedHeight = source.DockedHeight;
            }

        private static void DockBeside(ViewElement targetView, ViewElement dockingView, DockDirection dockDirection) {
            var dockParent = GetDockParent(targetView, dockDirection);
            var index = dockParent.Children.IndexOf(targetView);
            Dock(dockParent, dockingView, index, dockDirection);
            }

        private static DockGroup GetDockParent(ViewElement view, DockDirection dockDirection) {
            var parent = view.Parent as DockGroup;
            if (parent == null) {
                parent = DockGroup.Create();
                CopySize(view, parent);
                parent.Orientation = dockDirection == DockDirection.Left || dockDirection == DockDirection.Right ? Orientation.Horizontal : Orientation.Vertical;
                view.InsertNewParent(parent);
                }
            return parent;
            }

        private static void Dock(DockGroup parent, ViewElement view, Int32 index, DockDirection dockDirection) {
            if (parent.Orientation == Orientation.Horizontal) {
                switch (dockDirection) {
                    case DockDirection.Left:
                    DockSameOrientation(parent, view, index);
                    break;
                    case DockDirection.Top:
                    DockCounterOrientation(parent, view, index, 0);
                    break;
                    case DockDirection.Right:
                    DockSameOrientation(parent, view, index + 1);
                    break;
                    case DockDirection.Bottom:
                    DockCounterOrientation(parent, view, index, 1);
                    break;
                    }
                }
            else {
                switch (dockDirection) {
                    case DockDirection.Left:
                    DockCounterOrientation(parent, view, index, 0);
                    break;
                    case DockDirection.Top:
                    DockSameOrientation(parent, view, index);
                    break;
                    case DockDirection.Right:
                    DockCounterOrientation(parent, view, index, 1);
                    break;
                    case DockDirection.Bottom:
                    DockSameOrientation(parent, view, index + 1);
                    break;
                    }
                }
            foreach (var view1 in view.FindAll<View>())
                ClearBookmark(view1, view1.WindowProfile, view1.GetBookmarkType());
            }

        private static void DockSameOrientation(DockGroup parent, ViewElement view, Int32 index) {
            DockSameOrientation(parent.Children, view, parent.Orientation, index, true, v => v);
            }

        private static void DockCounterOrientation(DockGroup parent, ViewElement view, Int32 index, Int32 counterIndex) {
            DockSameOrientation(NestDockAt(parent, index), view, counterIndex);
            }

        private static DockGroup NestDockAt(DockGroup parent, Int32 index) {
            var child = parent.Children[index];
            var dockGroup = DockGroup.Create();
            dockGroup.Orientation = parent.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
            CopySize(child, dockGroup);
            child.InsertNewParent(dockGroup);
            return dockGroup;
            }

        internal static void DockSameOrientation<TChild>(IList<TChild> collection, ViewElement dockingView, Orientation orientation, Int32 targetIndex, Boolean modifySourceObjects, Func<ViewElement, TChild> viewToT) {
            DockGroup dockGroup;
            for (dockGroup = dockingView as DockGroup; dockGroup != null && dockGroup.Children.Count == 1; dockGroup = dockingView as DockGroup) {
                dockingView = dockGroup.Children[0];
                if (modifySourceObjects)
                    dockGroup.Children.Clear();
                }
            if (dockGroup != null && dockGroup.Orientation == orientation) {
                var childList = new List<TChild>();
                foreach (var child in dockGroup.Children)
                    childList.Add(viewToT(child));
                if (modifySourceObjects)
                    dockGroup.Children.Clear();
                foreach (var child in childList) {
                    collection.Insert(targetIndex, child);
                    ++targetIndex;
                    }
                }
            else
                collection.Insert(targetIndex, viewToT(dockingView));
            }

        public static void DockOutside(this ViewGroup targetView, ViewElement dockingView, DockDirection dockDirection) {
            if (targetView == null)
                throw new ArgumentNullException(nameof(targetView));
            if (dockingView == null)
                throw new ArgumentNullException(nameof(dockingView));
            if (dockDirection == DockDirection.Fill)
                throw new ArgumentException("DockDirection cannot be Fill in DockOutside");
            if (dockDirection < DockDirection.Fill || dockDirection > DockDirection.Bottom)
                throw new InvalidEnumArgumentException(nameof(dockDirection), (Int32)dockDirection, typeof(DockDirection));
            if (dockingView == targetView || dockingView.IsAncestorOf(targetView))
                throw new ArgumentException("targetView and dockingView must not be the same instance, and dockingView cannot be an ancestor of targetView.");
            if (!IsDockingViewValid(dockingView))
                throw new ArgumentException("dockingView is not a dockable ViewElement");
            if (!IsDockTargetValid(targetView))
                throw new ArgumentException("targetView is not a valid target for docking");
            if (targetView.Children.Count > 1)
                throw new ArgumentException("targetView of DockOutside cannot have more than 1 child");
            using (new DockEventRaiser(new DockEventArgs(DockAction.Dock, dockingView, true), false)) {
                using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                    if (IsBookmarkable(dockingView))
                        ReplaceWithBookmark(dockingView, out dockingView, dockingView.WindowProfile);
                    else
                        dockingView.Detach();
                    if (targetView.Children.Count == 0) {
                        targetView.Children.Add(dockingView);
                        }
                    else {
                        var child = targetView.Children[0];
                        var documentGroupContainer = child as DocumentGroupContainer;
                        var dockGroup = child as DockGroup;
                        var orientation = dockDirection == DockDirection.Left || dockDirection == DockDirection.Right ? Orientation.Horizontal : Orientation.Vertical;
                        if (dockGroup == null || documentGroupContainer != null)
                            dockGroup = NestDock(targetView, orientation);
                        var index = 0;
                        var parent = dockGroup;
                        if (dockGroup.Orientation == Orientation.Horizontal) {
                            switch (dockDirection) {
                                case DockDirection.Top:
                                case DockDirection.Bottom:
                                parent = NestDock(targetView, dockGroup.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal);
                                break;
                                case DockDirection.Right:
                                index = dockGroup.Children.Count - 1;
                                break;
                                }
                            }
                        else {
                            switch (dockDirection) {
                                case DockDirection.Left:
                                case DockDirection.Right:
                                parent = NestDock(targetView, dockGroup.Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal);
                                break;
                                case DockDirection.Bottom:
                                index = dockGroup.Children.Count - 1;
                                break;
                                }
                            }
                        Dock(parent, dockingView, index, dockDirection);
                        }
                    }
                }
            }

        private static DockGroup NestDock(ViewGroup site, Orientation orientation) {
            var dockGroup = DockGroup.Create();
            dockGroup.Orientation = orientation;
            if (site.Children.Count == 0) {
                site.Children.Add(dockGroup);
                }
            else {
                var child = site.Children[0];
                var splitterLength = child.DockedWidth.IsFill ? new SplitterLength(1.0, SplitterUnitType.Fill) : new SplitterLength(100.0, SplitterUnitType.Stretch);
                dockGroup.DockedWidth = splitterLength;
                dockGroup.DockedHeight = splitterLength;
                child.InsertNewParent(dockGroup);
                }
            return dockGroup;
            }

        public static IDisposable PreventTogglePinStatus() {
            return new PreventTogglePinStatusScope();
            }

        public static IDisposable PreventCollapse(this IEnumerable<ViewElement> elements) {
            return new PreventCollapseScope(elements);
            }

        private static void ReplaceElement(ViewElement currentElement, ViewElement newElement) {
            var index = currentElement.Parent.Children.IndexOf(currentElement);
            currentElement.Parent.Children[index] = newElement;
            }

        public static ViewElement ReplaceWithBookmark(ViewElement element, out ViewElement result, WindowProfile windowProfile) {
            //if (windowProfile == null)
            //  throw new ArgumentNullException("windowProfile");
            result = element;
            if (DockManager.Instance.IsDragging)
                return null;
            var view = element as View;
            if (view != null)
                return ReplaceWithBookmark(view, windowProfile);
            var tabGroup = element as TabGroup;
            if (tabGroup != null)
                return ReplaceWithBookmark(tabGroup, out result, windowProfile);
            var dockGroup = element as DockGroup;
            if (dockGroup != null)
                return ReplaceWithBookmark(dockGroup, out result, windowProfile);
            throw new ArgumentException("Only TabGroup, DockGroup and View elements are supported for bookmark replacement.");
            }

        private static ViewBookmark ReplaceWithBookmark(View view, WindowProfile windowProfile) {
            var bookmarkType = view.GetBookmarkType();
            ClearBookmark(view, windowProfile, bookmarkType);
            RenumberBookmarkAccessOrder(view.Name, windowProfile);
            var viewBookmark = ViewBookmark.Create(view);
            ReplaceElement(view, viewBookmark);
            return viewBookmark;
            }

        private static TabGroup ReplaceWithBookmark(TabGroup tabGroup, out ViewElement result, WindowProfile windowProfile) {
            return ReplaceWithBookmark(tabGroup, out result, windowProfile, true);
            }

        private static TabGroup ReplaceWithBookmark(TabGroup tabGroup, out ViewElement result, WindowProfile windowProfile, Boolean replaceGroup) {
            result = tabGroup;
            using (tabGroup.PreventCollapse()) {
                var tabGroup1 = TabGroup.Create();
                foreach (var viewElement in new List<ViewElement>(tabGroup.Children)) {
                    var viewBookmark = viewElement as ViewBookmark;
                    if (viewBookmark == null) {
                        var view = (View)viewElement;
                        ClearBookmark(view, windowProfile, view.GetBookmarkType());
                        RenumberBookmarkAccessOrder(view.Name, windowProfile);
                        viewBookmark = ViewBookmark.Create(view);
                        }
                    tabGroup1.Children.Add(viewBookmark);
                    }
                if (replaceGroup)
                    ReplaceElement(tabGroup, tabGroup1);
                if (tabGroup.Children.Count == 1) {
                    result = tabGroup.Children[0];
                    result.Detach();
                    }
                return tabGroup1;
                }
            }

        private static DockGroup ReplaceWithBookmark(DockGroup dockGroup, out ViewElement result, WindowProfile windowProfile) {
            return ReplaceWithBookmark(dockGroup, out result, windowProfile, true);
            }

        private static DockGroup ReplaceWithBookmark(DockGroup dockGroup, out ViewElement result, WindowProfile windowProfile, Boolean replaceGroup) {
            result = dockGroup;
            using (dockGroup.PreventCollapse()) {
                var dockGroup1 = DockGroup.Create();
                dockGroup1.Orientation = dockGroup.Orientation;
                foreach (var currentElement in new List<ViewElement>(dockGroup.Children)) {
                    var result1 = (ViewElement)null;
                    var viewElement = (ViewElement)null;
                    using (currentElement.PreventCollapse()) {
                        var tabGroup = currentElement as TabGroup;
                        if (tabGroup != null) {
                            viewElement = ReplaceWithBookmark(tabGroup, out result1, windowProfile, false);
                            }
                        else {
                            var dockGroup2 = currentElement as DockGroup;
                            if (dockGroup2 != null)
                                viewElement = ReplaceWithBookmark(dockGroup2, out result1, windowProfile, false);
                            }
                        if (viewElement != null) {
                            dockGroup1.Children.Add(viewElement);
                            if (currentElement != result1)
                                ReplaceElement(currentElement, result1);
                            }
                        else {
                            var viewBookmark = currentElement as ViewBookmark;
                            if (viewBookmark == null) {
                                var view = (View)currentElement;
                                ClearBookmark(view, windowProfile, view.GetBookmarkType());
                                RenumberBookmarkAccessOrder(view.Name, windowProfile);
                                viewBookmark = ViewBookmark.Create(view);
                                }
                            dockGroup1.Children.Add(viewBookmark);
                            }
                        }
                    }
                if (replaceGroup)
                    ReplaceElement(dockGroup, dockGroup1);
                if (dockGroup.Children.Count == 1) {
                    result = dockGroup.Children[0];
                    result.Detach();
                    }
                return dockGroup1;
                }
            }

        public static void SnapToBookmark(this ViewElement element) {
            if (element == null) { throw new ArgumentNullException(nameof(element)); }
            if (element.DockRestriction == DockRestrictionType.AlwaysFloating) { return; }
            using (ViewManager.DeferActiveViewChanges())
                SnapToBookmark(element, element.WindowProfile, ViewBookmarkType.All, DefaultBookmarkResolver.Instance);
            }

        public static void SnapToBookmark(this ViewElement element, ViewBookmarkType type) {
            if (element == null) { throw new ArgumentNullException(nameof(element)); }
            using (ViewManager.DeferActiveViewChanges())
                SnapToBookmark(element, element.WindowProfile, type, DefaultBookmarkResolver.Instance);
            }

        private static void SnapToBookmark(ViewElement element, WindowProfile windowProfile, ViewBookmarkType type) {
            using (ViewManager.DeferActiveViewChanges())
                SnapToBookmark(element, windowProfile, type, DefaultBookmarkResolver.Instance);
            }

        public static void SnapToBookmark(this ViewElement element, ViewBookmarkType type, IBookmarkResolver resolver) {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));
            using (ViewManager.DeferActiveViewChanges())
                SnapToBookmark(element, element.WindowProfile, type, resolver);
            }

        private static void SnapToBookmark(View view, WindowProfile windowProfile, ViewBookmarkType type, IBookmarkResolver resolver) {
            ClearBookmarksInOtherSitesIfNecessary(view, windowProfile, type);
            var bookmarkType = view.GetBookmarkType();
            var bookmark = FindBookmark(view.Name, windowProfile, type);
            var viewBookmark = bookmarkType != type ? FindBookmark(view, windowProfile, bookmarkType) : null;
            var args = new DockEventArgs(DockAction.SnapToBookmark, view, true);
            using (new DockEventRaiser(args, false)) {
                using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                    var bookmarkLocation = (ViewElement)null;
                    if (bookmark == null)
                        bookmarkLocation = resolver.SelectBookmarkLocation(view, windowProfile, type);
                    if (bookmarkType != type && viewBookmark == null && IsBookmarkable(view))
                        ReplaceWithBookmark(view, windowProfile);
                    if (bookmark == null) {
                        args.IsActionSuccessful = false;
                        resolver.DockToBookmarkLocation(bookmarkLocation, view, windowProfile, type);
                        }
                    else {
                        ReplaceElement(bookmark, view);
                        view.IsSelected = true;
                        }
                    }
                }
            }

        private static void ClearBookmarksInOtherSitesIfNecessary(View view, WindowProfile windowProfile, ViewBookmarkType type) {
            var site = view.FindRootElement() as FloatSite;
            if (site == null)
                return;
            var flag = false;
            if (type != ViewBookmarkType.DocumentWell) {
                if (type == ViewBookmarkType.Raft && site.HasDocumentGroupContainer)
                    flag = true;
                }
            else
                flag = true;
            if (!flag)
                return;
            ClearBookmarksIf(view, windowProfile, type, bm => bm.FindRootElement() != site);
            }

        private static List<View> SnapToBookmarkIf(IEnumerable<View> views, WindowProfile windowProfile, ViewBookmarkType type, IBookmarkResolver resolver, Predicate<View> include) {
            var viewList = new List<View>();
            foreach (var view in views) {
                if (include(view))
                    SnapToBookmark(view, windowProfile, type, resolver);
                else
                    viewList.Add(view);
                }
            return viewList;
            }

        private static void SnapToBookmark(ViewGroup group, WindowProfile windowProfile, ViewBookmarkType type, IBookmarkResolver resolver) {
            using (new DockEventRaiser(new DockEventArgs(DockAction.SnapToBookmark, @group, true), false)) {
                var viewList = new List<View>(group.FindAll<View>());
                using (LayoutSynchronizer.BeginLayoutSynchronization()) {
                    var bookmarkIf = SnapToBookmarkIf(SnapToBookmarkIf(viewList, windowProfile, type, resolver, v => {
                        if (!DocumentGroup.IsTabbedDocument(v))
                            return !v.IsSelected;
                        return false;
                    }), windowProfile, type, resolver, v => !DocumentGroup.IsTabbedDocument(v));
                    var flag = false;
                    if (group.OnScreenViewCardinality == OnScreenViewCardinality.Many)
                        flag = SnapContainerToMainSite(group.Find<DocumentGroupContainer>(false), windowProfile);
                    if (flag)
                        return;
                    foreach (var view in SnapToBookmarkIf(bookmarkIf, windowProfile, type, resolver, v => !v.IsSelected))
                        SnapToBookmark(view, windowProfile, type, resolver);
                    }
                }
            }

        private static void SnapToBookmark(ViewElement element, WindowProfile windowProfile, ViewBookmarkType type, IBookmarkResolver resolver) {
            var view = element as View;
            if (view != null) {
                SnapToBookmark(view, windowProfile, type, resolver);
                }
            else {
                var group = element as ViewGroup;
                if (group == null)
                    throw new InvalidOperationException("Only TabGroup and View elements are supported for bookmark replacement.");
                SnapToBookmark(group, windowProfile, type, resolver);
                }
            }

        private static Boolean SnapContainerToMainSite(DocumentGroupContainer sourceContainer, WindowProfile windowProfile) {
            var mainSite = windowProfile.Find<MainSite>();
            if (mainSite == null)
                return false;
            var documentGroupAt = mainSite.Find<DocumentGroup>(false);
            if (documentGroupAt == null)
                return false;
            var parent = documentGroupAt.Parent as DocumentGroupContainer;
            if (parent.VisibleChildren.Count <= 1)
                parent.Orientation = sourceContainer.Orientation;
            var documentGroupList = new List<DocumentGroup>(sourceContainer.FindAll<DocumentGroup>());
            using (sourceContainer.PreventCollapse()) {
                foreach (var documentGroup in documentGroupList) {
                    if (documentGroupAt.VisibleChildren.Count > 0)
                        documentGroupAt = CreateDocumentGroupAt(parent, parent.Children.Count);
                    documentGroupAt.Dock(documentGroup, DockDirection.Fill);
                    }
                var ancestor = sourceContainer.FindAncestor<FloatSite>();
                if (ancestor != null)
                    ancestor.RemoveDocumentGroupContainer(windowProfile);
                }
            return true;
            }

        public static ViewBookmark FindBookmark(View view, WindowProfile windowProfile) {
            return FindBookmark(view.Name, windowProfile);
            }

        public static ViewBookmark FindBookmark(String name, WindowProfile windowProfile) {
            return FindBookmark(name, windowProfile, ViewBookmarkType.All);
            }

        public static ViewBookmark FindBookmark(View view, WindowProfile windowProfile, ViewBookmarkType type) {
            return FindBookmark(view.Name, windowProfile, type);
            }

        public static ViewBookmark FindBookmark(String name, WindowProfile windowProfile, ViewBookmarkType type) {
            var viewBookmark = (ViewBookmark)null;
            var viewBookmarkList = new List<ViewBookmark>(FindBookmarks(name, windowProfile, type));
            if (viewBookmarkList.Count > 0)
                viewBookmark = viewBookmarkList[0];
            return viewBookmark;
            }

        public static IEnumerable<ViewBookmark> FindBookmarks(View view, WindowProfile profile, ViewBookmarkType type) {
            return FindBookmarks(view.Name, profile, type);
            }

        public static IEnumerable<ViewBookmark> FindBookmarks(String name, WindowProfile profile, ViewBookmarkType type) {
            if (profile == null) { return new ViewBookmark[0]; }
            var r = new List<ViewBookmark>(profile.FindAll<ViewBookmark>(i => (i.Name == name) && ((i.ViewBookmarkType == type) || ViewBookmarkType.All == type)));
            r.Sort(CompareBookmarksByAccessOrder);
            return r;
            }

        public static void ClearBookmark(View view, WindowProfile windowProfile, ViewBookmarkType type) {
            foreach (var bookmark in FindBookmarks(view.Name, windowProfile, type))
                bookmark.Detach();
            }

        public static void ClearBookmarksIf(View view, WindowProfile windowProfile, ViewBookmarkType type, Predicate<ViewBookmark> shouldClear) {
            foreach (var bookmark in FindBookmarks(view.Name, windowProfile, type)) {
                if (shouldClear(bookmark))
                    bookmark.Detach();
                }
            }

        internal static void ClearViewBookmarks(List<ViewElement> elements) {
            if (elements == null)
                return;
            foreach (var element in elements) {
                var parent = (ViewElement)element.Parent;
                if (parent != null && !FloatSite.IsFloating(parent)) {
                    var windowProfile = parent.WindowProfile;
                    var bookmarkType = parent.GetBookmarkType();
                    var view = element as View;
                    if (view != null)
                        ClearBookmark(view, windowProfile, bookmarkType);
                    }
                }
            }

        private static Boolean IsBookmarkable(ViewElement element) {
            if (!(element is View) && !(element is TabGroup) && !(element is DockGroup))
                return false;
            var rootElement = ViewElement.FindRootElement(element);
            if (!(rootElement is MainSite)) {
                var floatSite = rootElement as FloatSite;
                if (floatSite == null || floatSite.OnScreenViewCardinality != OnScreenViewCardinality.Many && floatSite.Find<ViewBookmark>(false) == null && !(floatSite.Child is TabGroup))
                    return false;
                }
            return element.Parent is DockGroup || element.Parent is TabGroup || (element.Parent is DocumentGroup || element.Parent is FloatSite);
            }

        public static void RenumberBookmarkAccessOrder(String name, WindowProfile profile) {
            var viewBookmarkList = new List<ViewBookmark>(FindBookmarks(name, profile, ViewBookmarkType.All));
            var num = 1;
            foreach (var viewBookmark in viewBookmarkList) {
                viewBookmark.AccessOrder = num;
                ++num;
                }
            }

        private static Int32 CompareBookmarksByAccessOrder(ViewBookmark bookmark1, ViewBookmark bookmark2) {
            return bookmark1.AccessOrder.CompareTo(bookmark2.AccessOrder);
            }

        private static void CalculateUndockPosition(ViewElement element, Point cursorDevicePoint, Rect currentUndockingDeviceRect) {
            var width = element.FloatingWidth;
            var height = element.FloatingHeight;
            var logicalUnits1 = cursorDevicePoint.DeviceToLogicalUnits();
            var logicalUnits2 = currentUndockingDeviceRect.DeviceToLogicalUnits();
            Double left;
            Double top;
            if (!logicalUnits2.IsEmpty) {
                width = logicalUnits2.Width;
                height = logicalUnits2.Height;
                left = Math.Max(logicalUnits2.Left, logicalUnits1.X + 50.0 - width);
                top = logicalUnits1.Y - 8.0;
                }
            else {
                left = logicalUnits1.X - width / 2.0;
                top = logicalUnits1.Y - 8.0;
                }
            Int32 display;
            Rect relativePosition;
            Screen.AbsoluteRectToRelativeRect(left, top, width, height, out display, out relativePosition);
            element.Display = display;
            element.FloatingLeft = relativePosition.Left;
            element.FloatingTop = relativePosition.Top;
            element.FloatingWidth = relativePosition.Width;
            element.FloatingHeight = relativePosition.Height;
            }

        private class DockEventRaiser : DisposableObject {
            private List<View> changingViews;
            internal static Int32 dockEventsInProgress;
            internal static DockAction lastDockAction;

            private DockEventArgs EventArgs { get; }

            public DockEventRaiser(DockEventArgs args, Boolean onlyMultiSelection = false) {
                if (args == null)
                    throw new ArgumentNullException(nameof(args));
                EventArgs = args;
                ++dockEventsInProgress;
                lastDockAction = args.Action;
                // ISSUE: reference to a compiler-generated field
                DockPositionChanging.RaiseEvent(null, args);
                changingViews = new List<View>(EventArgs.ChangingElement.FindAll<View>(v => {
                    if (onlyMultiSelection)
                        return DocumentGroup.GetIsMultiSelected(v);
                    return true;
                }));
                foreach (var changingView in changingViews)
                    changingView.RaiseDockPositionChanging(EventArgs.Action);
                }

            protected override void DisposeManagedResources() {
                --dockEventsInProgress;
                // ISSUE: reference to a compiler-generated field
                DockPositionChanged.RaiseEvent(null, EventArgs);
                foreach (var changingView in changingViews)
                    changingView.RaiseDockPositionChanged(EventArgs.Action, EventArgs.IsActionSuccessful);
                var view = changingViews.FirstOrDefault();
                if (view == null)
                    return;
                UtilityMethods.RestoreWindow(view);
                }
            }

        private enum TabType {
            Default,
            Pinned,
            Preview,
            }

        private delegate Boolean IsContainedInElements(View element);

        private sealed class PreventCollapseScope : DisposableObject {
            private IEnumerable<IDisposable> nestedDisposables;

            public PreventCollapseScope(IEnumerable<ViewElement> elements) {
                var disposableList = new List<IDisposable>();
                foreach (var element in elements)
                    disposableList.Add(element.PreventCollapse());
                nestedDisposables = disposableList;
                }

            protected override void DisposeManagedResources() {
                foreach (var nestedDisposable in nestedDisposables)
                    nestedDisposable.Dispose();
                }
            }

        private sealed class PreventTogglePinStatusScope : DisposableObject {
            private static Int32 preventCount;

            public static Boolean IsTogglePinStatusPrevented {
                get {
                    return preventCount > 0;
                    }
                }

            public PreventTogglePinStatusScope() {
                ++preventCount;
                }

            protected override void DisposeManagedResources() {
                base.DisposeManagedResources();
                --preventCount;
                }
            }

        private sealed class DefaultBookmarkResolver : IBookmarkResolver {
            private static DefaultBookmarkResolver _instance;

            public static DefaultBookmarkResolver Instance {
                get {
                    return _instance ?? (_instance = new DefaultBookmarkResolver());
                    }
                }

            private DefaultBookmarkResolver() {
                }

            public ViewElement SelectBookmarkLocation(View view, WindowProfile windowProfile, ViewBookmarkType bookmarkType) {
                return GetPrimaryDocumentGroup(windowProfile);
                }

            public void DockToBookmarkLocation(ViewElement bookmarkLocation, View view, WindowProfile windowProfile, ViewBookmarkType bookmarkType) {
                var targetView = bookmarkLocation as DocumentGroup;
                if (targetView == null)
                    return;
                if (bookmarkType == ViewBookmarkType.DocumentWell || (view.DockRestriction & DockRestrictionType.DocumentGroup) != DockRestrictionType.None)
                    targetView.Dock(view, DockDirection.Fill);
                else
                    targetView.Parent.Dock(view, DockDirection.Bottom);
                view.IsSelected = true;
                }
            }
        }
    }