using BinaryStudio.PlatformUI.Shell.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Interop;
using System.Windows.Media;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.PlatformUI.Shell
    {
    public class ViewManager
        {
        private ContentControl mainWindowContent;
        private HwndSource mainWindowHwndSource;
        private WindowProfile windowProfile;
        private Boolean isWindowProfileChanging;
        private ResourceDictionary theme;
        private View activeView;
        private View pendingActiveView;
        private static ViewManager instance_;
        private FloatingWindowManager floatingWindowManager;
        private AutoHideWindowManager autoHideWindowManager;
        private ViewManagerPreferences preferences;
        private Boolean tabGroupSelectionChanged;
        private DragUndockHeader currentDragUndockHeader;

        public String Name { get; }

        #region P:MainWindowHwndSource:HwndSource
        private HwndSource MainWindowHwndSource {
            get { return mainWindowHwndSource; }
            set
                {
                if (mainWindowHwndSource == value) return;
                mainWindowHwndSource = value;
                FloatingWindowManager.OwnerWindow = MainWindowHandle;
                DockManager.Instance.UnregisterSite(mainWindowContent);
                DockManager.Instance.RegisterSite(mainWindowContent, MainWindowHandle);
                BroadcastMessageMonitor.Instance.HwndSource = mainWindowHwndSource;
                }
            }
        #endregion
        #region P:MainWindowHandle:IntPtr
        private IntPtr MainWindowHandle { get {
            if (MainWindowHwndSource != null) return MainWindowHwndSource.Handle;
            return IntPtr.Zero;
            }}
        #endregion
        #region P:MainWindowContent:ContentControl
        internal ContentControl MainWindowContent { get {
            return mainWindowContent;
            }}
        #endregion
        #region P:FloatingWindowManager:FloatingWindowManager
        public FloatingWindowManager FloatingWindowManager { get {
            if (floatingWindowManager == null)
                floatingWindowManager = CreateFloatingWindowManager();
            return floatingWindowManager;
            }}
        #endregion
        #region P:AutoHideWindowManager:AutoHideWindowManager
        internal AutoHideWindowManager AutoHideWindowManager { get {
            if (autoHideWindowManager == null)
                autoHideWindowManager = new AutoHideWindowManager();
            return autoHideWindowManager;
            }}
        #endregion
        #region P:CurrentAutoHideView:View
        public View CurrentAutoHideView { get {
            return AutoHideWindowManager.AutoHideWindowElement;
            }}
        #endregion
        #region P:IsInitialized:Boolean
        public Boolean IsInitialized { get {
            return mainWindowContent != null;
            }}
        #endregion
        #region P:Preferences:ViewManagerPreferences
        public ViewManagerPreferences Preferences { get {
            if (preferences == null) { preferences = new ViewManagerPreferences(); }
            return preferences;
            }}
        #endregion
        #region P:WindowProfile:WindowProfile
        public WindowProfile WindowProfile {
            get
                {
                return windowProfile;
                }
            set
                {
                if (value == null) { throw new ArgumentNullException(nameof(value)); }
                if (isWindowProfileChanging) { throw new InvalidOperationException("ViewManager does not support reentrant calls to set_WindowProfile."); }
                isWindowProfileChanging = true;
                MultiSelectionManager.Instance.Clear();
                try {
                    if (Equals(value, windowProfile)) { return; }
                    using (DeferActiveViewChanges()) {
                        using (FloatingWindowManager.DeferFloatingVisibilityChanges()) {
                            WindowProfileChanging.RaiseEvent(this, new WindowProfileChangingEventArgs(windowProfile, value));
                            SetActiveView(null, ActivationType.Default);
                            if (windowProfile != null) {
                                FloatingWindowManager.RemoveAllFloats(windowProfile);
                                windowProfile.Children.CollectionChanged -= OnViewSitesChanged;
                                }
                            MainSite mainSite1 = null;
                            var floatSiteList = new List<FloatSite>();
                            foreach (var child in value.Children) {
                                var mainSite2 = child as MainSite;
                                if (mainSite2 != null)
                                    {
                                    mainSite1 = mainSite2;
                                    }
                                else
                                    {
                                    floatSiteList.Add(child as FloatSite);
                                    }
                                }
                            mainWindowContent.Content = mainSite1;
                            value.Children.CollectionChanged += OnViewSitesChanged;
                            foreach (var floatSite in floatSiteList)
                                {
                                FloatingWindowManager.AddFloat(floatSite);
                                }

                            if (windowProfile != null)
                                {
                                windowProfile.ViewManager = null;
                                }
                            windowProfile = value;
                            if (windowProfile != null)
                                {
                                windowProfile.ViewManager = this;
                                }
                            }
                        }
                    WindowProfileChanged.RaiseEvent(this);
                    }
                finally
                    {
                    isWindowProfileChanging = false;
                    }
                }
            }
        #endregion
        #region P:Theme:ResourceDictionary
        public ResourceDictionary Theme {
            get
                {
                return theme;
                }
            set
                {
                if (theme != null) {
                    Application.Current.Resources.MergedDictionaries.Remove(theme);
                    if (mainWindowContent != null)
                        mainWindowContent.Resources.MergedDictionaries.Remove(theme);
                    }
                theme = value;
                if (theme == null)
                    return;
                Application.Current.Resources.MergedDictionaries.Add(theme);
                if (mainWindowContent == null)
                    return;
                mainWindowContent.Resources.MergedDictionaries.Add(theme);
                }
            }
        #endregion
        #region P:Instance:ViewManager
        public static ViewManager Instance {
            get {
                return instance_ ?? (instance_ = new ViewManager("Default"));
            }
            internal set {
                instance_ = value;
            }
        }
        #endregion

        public Boolean IsPendingActiveView { get; private set; }

        #region P:ActiveView:View
        public View ActiveView {
            get { return activeView; }
            }
        #endregion
        #region P:CurrentDragUndockHeader:DragUndockHeader
        internal DragUndockHeader CurrentDragUndockHeader {
            get
                {
                return currentDragUndockHeader;
                }
            set
                {
                if (currentDragUndockHeader != null)
                    PresentationSource.RemoveSourceChangedHandler(currentDragUndockHeader, OnViewHeaderPresentationSourceChanged);
                currentDragUndockHeader = value;
                if (currentDragUndockHeader != null)
                    PresentationSource.AddSourceChangedHandler(currentDragUndockHeader, OnViewHeaderPresentationSourceChanged);
                KeyboardStateManager.CurrentDragUndockHeader = currentDragUndockHeader;
                }
            }
        #endregion

        public event EventHandler<WindowProfileChangingEventArgs> WindowProfileChanging;
        public event EventHandler WindowProfileChanged;
        public event EventHandler<ActiveViewChangedEventArgs> ActiveViewChanged;
        public event EventHandler<CancelEventArgs> ActivatingWindow;

        static ViewManager() {
            EventManager.RegisterClassHandler(typeof(DragUndockHeader), DragUndockHeader.DragStartedEvent, new EventHandler<DragAbsoluteEventArgs>(OnViewHeaderDragStarted));
            EventManager.RegisterClassHandler(typeof(DragUndockHeader), DragUndockHeader.DragAbsoluteEvent, new EventHandler<DragAbsoluteEventArgs>(OnViewHeaderDragAbsolute));
            EventManager.RegisterClassHandler(typeof(DragUndockHeader), DragUndockHeader.DragCompletedAbsoluteEvent, new EventHandler<DragAbsoluteCompletedEventArgs>(OnViewHeaderDragCompleted));
            EventManager.RegisterClassHandler(typeof(DragUndockHeader), DragUndockHeader.DragHeaderClickedEvent, new RoutedEventHandler(OnViewHeaderClicked));
            EventManager.RegisterClassHandler(typeof(DockTarget), DockManager.FloatingElementDockedEvent, new EventHandler<FloatingElementDockedEventArgs>(OnFloatingElementDocked));
            EventManager.RegisterClassHandler(typeof(AutoHideTabItem), MouseHover.MouseHoverEvent, new RoutedEventHandler(OnMouseHoverOverAutoHideTabItem));
            CommandManager.RegisterClassCommandBinding(typeof(AutoHideTabItem), new CommandBinding(ViewCommands.ShowAutoHiddenView, OnShowAutoHiddenView));
            CommandManager.RegisterClassCommandBinding(typeof(AutoHideTabItem), new CommandBinding(ViewCommands.ShowAndActivateAutoHiddenView, OnShowAndActivateAutoHiddenView));
            CommandManager.RegisterClassCommandBinding(typeof(AutoHideTabItem), new CommandBinding(ViewCommands.HideAutoHiddenView, OnHideAutoHiddenView));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.HideViewCommand, OnHideView));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.AutoHideViewCommand, OnAutoHideView));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.NewHorizontalTabGroupCommand, OnNewHorizontalTabGroup));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.NewVerticalTabGroupCommand, OnNewVerticalTabGroup));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.MoveToNextTabGroupCommand, OnMoveToNextTabGroup));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.MoveToPreviousTabGroupCommand, OnMoveToPreviousTabGroup));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.MoveAllToNextTabGroupCommand, OnMoveAllToNextTabGroup));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.MoveAllToPreviousTabGroupCommand, OnMoveAllToPreviousTabGroup));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.ActivateDocumentViewCommand, OnActivateDocumentView));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.ToggleDocked, OnToggleDocked));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.PromoteCommand, Promote));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.MultiSelectCommand, OnMultiSelect));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.RightSelectCommand, OnRightSelect));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.CancelMultiSelectionCommand, OnCancelMultiSelection));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.TogglePinStatusCommand, OnTogglePinStatus));
            EventManager.RegisterClassHandler(typeof(ViewPresenter), UIElement.PreviewGotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(OnViewPreviewGotKeyboardFocus));
            EventManager.RegisterClassHandler(typeof(ViewPresenter), UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(OnViewMouseDown));
            EventManager.RegisterClassHandler(typeof(DocumentGroupControl), UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(OnTabControlMouseDown));
            EventManager.RegisterClassHandler(typeof(DocumentGroupControl), Selector.SelectionChangedEvent, new SelectionChangedEventHandler(OnTabControlSelectionChanged));
            EventManager.RegisterClassHandler(typeof(TabGroupControl), UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(OnTabControlMouseDown));
            EventManager.RegisterClassHandler(typeof(TabGroupControl), Selector.SelectionChangedEvent, new SelectionChangedEventHandler(OnTabControlSelectionChanged));
            EventManager.RegisterClassHandler(typeof(TabItem), UIElement.PreviewMouseDownEvent, new MouseButtonEventHandler(OnTabItemMouseDown));
            EventManager.RegisterClassHandler(typeof(TabItem), UIElement.PreviewMouseUpEvent, new MouseButtonEventHandler(OnTabItemMouseUp));
            EventManager.RegisterClassHandler(typeof(FloatingWindow), FrameworkElement.SizeChangedEvent, new SizeChangedEventHandler(OnFloatingWindowSizeChanged));
            EventManager.RegisterClassHandler(typeof(FloatingWindow), FloatingWindow.LocationChangedEvent, new RoutedEventHandler(OnFloatingWindowLocationChanged));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.ToggleMaximizeRestoreWindow, OnToggleMaximizeRestoreWindow));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.MinimizeWindow, OnMinimizeWindow));
            CommandManager.RegisterClassCommandBinding(typeof(UIElement), new CommandBinding(ViewCommands.CloseWindow, OnCloseWindow));
            DockOperations.DockPositionChanged += OnDockPositionChanged;
            DocumentTabPanel.SelectedItemHidden += OnSelectedItemHidden;
            var assembly = typeof(Behavior).Assembly;
            }

        #region M:CreateFloatingWindowManager:FloatingWindowManager
        protected virtual FloatingWindowManager CreateFloatingWindowManager()
            {
            return new FloatingWindowManager();
            }
        #endregion
        #region M:Initialize(ContentControl)
        public void Initialize(ContentControl content) {
            if (content == null) { throw new ArgumentNullException(nameof(content)); }
            mainWindowContent = content;
            SetViewManager(content, this);
            //MergeResources();
            InitializePresentationSource(content);
            }
        #endregion
        #region M:InitializePresentationSource(UIElement)
        private void InitializePresentationSource(UIElement content)
            {
            PresentationSource.AddSourceChangedHandler(content, OnPresentationSourceChanged);
            UpdateMainWindowHandle(content);
            }
        #endregion
        #region M:UpdateMainWindowHandle(UIElement)
        private void UpdateMainWindowHandle(UIElement content)
            {
            MainWindowHwndSource = UtilityMethods.FindTopLevelHwndSource(content);
            }
        #endregion
        #region M:OnPresentationSourceChanged(Object,SourceChangedEventArgs)
        private void OnPresentationSourceChanged(Object sender, SourceChangedEventArgs e)
            {
            UpdateMainWindowHandle(mainWindowContent);
            }
        #endregion
        #region M:CanShowAutoHiddenView(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanShowAutoHiddenView(ExecutedRoutedEventArgs e)
            {
            return (e.Parameter is View) && e.OriginalSource is AutoHideTabItem;
            }
        #endregion
        #region M:OnShowAutoHiddenView(Object,ExecutedRoutedEventArgs)
        private static void OnShowAutoHiddenView(Object sender, ExecutedRoutedEventArgs e)
            {
            if (!CanShowAutoHiddenView(e)) { return; }
            var originalSource = e.OriginalSource as AutoHideTabItem;
            var parameter = (View)e.Parameter;
            instance_.AutoHideWindowManager.ShowAutoHideWindow(originalSource, parameter);
            }
        #endregion
        #region M:CanShowAndActivateAutoHiddenView(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanShowAndActivateAutoHiddenView(ExecutedRoutedEventArgs e)
            {
            return (e.Parameter is View) && e.OriginalSource is AutoHideTabItem;
            }
        #endregion
        #region M:OnShowAndActivateAutoHiddenView(Object,ExecutedRoutedEventArgs)
        private static void OnShowAndActivateAutoHiddenView(Object sender, ExecutedRoutedEventArgs e)
            {
            if (!CanShowAndActivateAutoHiddenView(e)) { return; }
            var originalSource = e.OriginalSource as AutoHideTabItem;
            var parameter = (View)e.Parameter;
            if (!Equals(parameter.WindowProfile, instance_.WindowProfile)) { return; }
            instance_.SetActiveView(parameter, ActivationType.Default);
            if (!AutoHideChannel.IsAutoHidden(parameter)) { return; }
            instance_.AutoHideWindowManager.ShowAutoHideWindow(originalSource, parameter);
            }
        #endregion
        #region M:OnHideAutoHiddenView(Object,ExecutedRoutedEventArgs)
        private static void OnHideAutoHiddenView(Object sender, ExecutedRoutedEventArgs e)
            {
            ((ViewElement)e.Parameter).ViewManager.AutoHideWindowManager.CloseAutoHideWindow();
            }
        #endregion
        #region M:CanHideView(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanHideView(ExecutedRoutedEventArgs e)
            {
            return e.Parameter is ViewElement;
            }
        #endregion
        #region M:ShouldHideTabGroup(ViewElement,Boolean):Boolean
        private static Boolean ShouldHideTabGroup(ViewElement element, Boolean hideOnlyActiveView) {
            if (!(element.Parent is TabGroup)) { return false; }
            return !hideOnlyActiveView;
            }
        #endregion
        #region M:OnHideViewCore(ViewElement,Boolean)
        private static void OnHideViewCore(ViewElement closingViewElement, Boolean hideOnlyActiveView)
            {
            if (ShouldHideTabGroup(closingViewElement, hideOnlyActiveView))
                closingViewElement = closingViewElement.Parent;
            foreach (var view in new List<View>(closingViewElement.FindAll<View>()))
                view.Hide();
            }
        #endregion
        #region M:OnHideView(Object,ExecutedRoutedEventArgs)
        private static void OnHideView(Object sender, ExecutedRoutedEventArgs e) {
            if (!CanHideView(e)) { return; }
            var M = ((ViewElement)e.Parameter).ViewManager;
            var hideOnlyActiveView = M.Preferences.HideOnlyActiveView;
            if (NativeMethods.IsKeyPressed(16))
                hideOnlyActiveView = !hideOnlyActiveView;
            var element = sender as UIElement;
            if (element != null && FloatingWindow.GetCloseAllViews(element))
                hideOnlyActiveView = false;
            OnHideViewCore((ViewElement)e.Parameter, hideOnlyActiveView);
            }
        #endregion
        #region M:CanAutoHideView(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanAutoHideView(ExecutedRoutedEventArgs e) {
            var parameter = e.Parameter as ViewElement;
            if (parameter == null) { return false; }
            return (parameter.Parent is AutoHideGroup) || DockOperations.CanAutoHide(parameter);
            }
        #endregion
        #region M:OnAutoHideViewCore(ViewElement,Boolean)
        private static void OnAutoHideViewCore(ViewElement autoHidingElement, Boolean autoHideOnlyActiveView) {
            if (autoHidingElement.Parent is AutoHideGroup) {
                DockOperations.DockViewElementOrGroup(autoHidingElement, autoHideOnlyActiveView);
                }
            else {
                if (autoHideOnlyActiveView) {
                    var tabGroup = autoHidingElement as TabGroup;
                    if (tabGroup != null)
                        autoHidingElement = tabGroup.SelectedElement;
                    }
                autoHidingElement.AutoHide();
                }
            }
        #endregion
        #region M:OnAutoHideView(Object,ExecutedRoutedEventArgs)
        private static void OnAutoHideView(Object sender, ExecutedRoutedEventArgs e) {
            if (!CanAutoHideView(e)) { return; }
            var view = (ViewElement)e.Parameter;
            var M = view.ViewManager;
            var autoHideOnlyActiveView = M.Preferences.AutoHideOnlyActiveView;
            if (NativeMethods.IsKeyPressed(16))
                autoHideOnlyActiveView = !autoHideOnlyActiveView;
            OnAutoHideViewCore((ViewElement)e.Parameter, autoHideOnlyActiveView);
            }
        #endregion
        #region M:CanToggleDocked(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanToggleDocked(ExecutedRoutedEventArgs e)
            {
            return e.Parameter is ViewElement;
            }
        #endregion
        #region M:OnToggleDocked(Object,ExecutedRoutedEventArgs)
        private static void OnToggleDocked(Object sender, ExecutedRoutedEventArgs e)
            {
            if (!CanToggleDocked(e)) { return; }
            var parameter = (ViewElement)e.Parameter;
            ViewBookmarkType bookmarkType;
            if (GetToggleDockAction(parameter, out bookmarkType) == DockAction.Dock)
                parameter.SnapToBookmark(bookmarkType, ToggleDockedBookmarkResolver.Instance);
            else
                parameter.Float(parameter.WindowProfile);
            }
        #endregion
        #region M:GetToggleDockAction(ViewElement,out ViewBookmarkType):DockAction
        private static DockAction GetToggleDockAction(ViewElement toggledElement, out ViewBookmarkType bookmarkType) {
            bookmarkType = ViewBookmarkType.All;
            if (toggledElement is FloatSite) { return DockAction.Dock; }
            var rootElement = toggledElement.FindRootElement() as FloatSite;
            if (rootElement == null) { return DockAction.Float; }
            if (!rootElement.HasDocumentGroupContainer) { return DockAction.Dock; }
            if (rootElement.HasAutohiddenViews || rootElement.OnScreenViewCardinality == OnScreenViewCardinality.Many || toggledElement.FindAncestor<DocumentGroup>().VisibleChildren.Count > 1)
                return DockAction.Float;
            bookmarkType = ViewBookmarkType.DocumentWell;
            return DockAction.Dock;
            }
        #endregion
        #region M:CanPromote(ExecutedRoutedEventArgs)
        private static Boolean CanPromote(ExecutedRoutedEventArgs e) {
            var parameter = e.Parameter as View;
            if (parameter == null) { return false; }
            var parent = parameter.Parent as DocumentGroup;
            return parent != null && Equals(parent.PreviewView, parameter);
            }
        #endregion
        #region M:Promote(Object,ExecutedRoutedEventArgs)
        private static void Promote(Object sender, ExecutedRoutedEventArgs e)
            {
            if (!CanPromote(e)) { return; }
            ((DocumentGroup)((ViewElement)e.Parameter).Parent).PreviewView = null;
            }
        #endregion
        #region M:CanMultiSelect(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanMultiSelect(ExecutedRoutedEventArgs e) {
            if (e.Parameter is View)
                return ((ViewElement)e.Parameter).Parent is DocumentGroup;
            return false;
            }
        #endregion
        #region M:OnMultiSelect(Object,ExecutedRoutedEventArgs)
        private static void OnMultiSelect(Object sender, ExecutedRoutedEventArgs args) {
            if (!CanMultiSelect(args)) { return; }
            var parameter = (View)args.Parameter;
            var instance = MultiSelectionManager.Instance;
            if (DocumentGroup.GetIsPreviewView(parameter))
                return;
            if (instance.Contains(parameter)) {
                instance.Remove(parameter);
                }
            else {
                View view = null;
                if (parameter.Parent != null)
                    view = parameter.Parent.PreviousSelectedElement as View;
                if (parameter.ViewManager.tabGroupSelectionChanged && view != null)
                    instance.Add(view);
                if (instance.Contains(parameter) || instance.SelectedElementCount <= 0)
                    return;
                instance.Add(parameter);
                }
            }
        #endregion
        #region M:CanRightSelect(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanRightSelect(ExecutedRoutedEventArgs e)
            {
            return e.Parameter is View;
            }
        #endregion
        #region M:OnRightSelect(Object,ExecutedRoutedEventArgs)
        private static void OnRightSelect(Object sender, ExecutedRoutedEventArgs args)
            {
            if (!CanRightSelect(args) || MultiSelectionManager.Instance.Contains((ViewElement)args.Parameter)) { return; }
            MultiSelectionManager.Instance.Clear();
            }
        #endregion
        #region M:OnCancelMultiSelection(Object,ExecutedRoutedEventArgs)
        private static void OnCancelMultiSelection(Object sender, ExecutedRoutedEventArgs args)
            {
            MultiSelectionManager.Instance.Clear();
            }
        #endregion
        #region M:CanTogglePinStatus(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanTogglePinStatus(ExecutedRoutedEventArgs args)
            {
            return (args.Parameter is View) && ((ViewElement)args.Parameter).Parent is DocumentGroup;
            }
        #endregion
        #region M:OnTogglePinStatus(Object,ExecutedRoutedEventArgs)
        private static void OnTogglePinStatus(Object sender, ExecutedRoutedEventArgs args)
            {
            if (!CanTogglePinStatus(args)) { return; }
            TogglePinStatus((View)args.Parameter, false);
            }
        #endregion
        #region M:TogglePinStatus(View,Boolean)
        public static void TogglePinStatus(View view, Boolean toggleAllSelected)
            {
            Validate.IsNotNull(view, nameof(view));
            var state = !view.IsPinned;
            if (toggleAllSelected)
                {
                ForEachSelectedView(view, v => v.IsPinned = state);
                }
            else
                {
                view.IsPinned = state;
                }
            }
        #endregion
        #region M:IsViewContainedInDocumentGroupContainer(Object):Boolean
        private static Boolean IsViewContainedInDocumentGroupContainer(Object parameter)
            {
            var view = parameter as View;
            if (view != null) {
                var parent = view.Parent as DocumentGroup;
                if (parent != null)
                    return parent.Parent is DocumentGroupContainer;
                }
            return false;
            }
        #endregion
        #region M:CanExecuteNewHorizontalTabGroup(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanExecuteNewHorizontalTabGroup(ExecutedRoutedEventArgs args)
            {
            return IsViewContainedInDocumentGroupContainer(args.Parameter);
            }
        #endregion
        #region M:OnNewHorizontalTabGroup(Object,ExecutedRoutedEventArgs)
        private static void OnNewHorizontalTabGroup(Object sender, ExecutedRoutedEventArgs e)
            {
            if (!CanExecuteNewHorizontalTabGroup(e)) { return; }
            var parameter = (View)e.Parameter;
            using (DeferActiveViewChanges()) {
                parameter.ViewManager.CreateDocumentGroup(parameter, Orientation.Vertical);
                ForEachSelectedView(parameter, v => MoveToTabGroup(v, 1));
                parameter.ViewManager.SetActiveView(parameter, ActivationType.Default);
                }
            }
        #endregion
        #region M:CanExecuteNewVerticalTabGroup(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanExecuteNewVerticalTabGroup(ExecutedRoutedEventArgs args)
            {
            return IsViewContainedInDocumentGroupContainer(args.Parameter);
            }
        #endregion
        #region M:OnNewVerticalTabGroup(Object,ExecutedRoutedEventArgs)
        private static void OnNewVerticalTabGroup(Object sender, ExecutedRoutedEventArgs e)
            {
            if (!CanExecuteNewVerticalTabGroup(e))
                return;
            var parameter = (View)e.Parameter;
            using (DeferActiveViewChanges()) {
                parameter.ViewManager.CreateDocumentGroup(parameter, Orientation.Horizontal);
                ForEachSelectedView(parameter, v => MoveToTabGroup(v, 1));
                parameter.ViewManager.SetActiveView(parameter, ActivationType.Default);
                }
            }
        #endregion
        #region M:CanMoveToNextTabGroup(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanMoveToNextTabGroup(ExecutedRoutedEventArgs args)
            {
            return IsViewContainedInDocumentGroupContainer(args.Parameter);
            }
        #endregion
        #region M:OnMoveToNextTabGroup(Object,ExecutedRoutedEventArgs)
        private static void OnMoveToNextTabGroup(Object sender, ExecutedRoutedEventArgs e)
            {
            if (!CanMoveToNextTabGroup(e)) { return; }
            ((View)e.Parameter).ViewManager.MoveToVisibleTabGroup((View)e.Parameter, 1);
            }
        #endregion
        #region M:CanMoveToPreviousTabGroup(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanMoveToPreviousTabGroup(ExecutedRoutedEventArgs args)
            {
            return IsViewContainedInDocumentGroupContainer(args.Parameter);
            }
        #endregion
        #region M:OnMoveToPreviousTabGroup(Object,ExecutedRoutedEventArgs)
        private static void OnMoveToPreviousTabGroup(Object sender, ExecutedRoutedEventArgs e) {
            if (!CanMoveToPreviousTabGroup(e)) { return; }
            ((View)e.Parameter).ViewManager.MoveToVisibleTabGroup((View)e.Parameter, -1);
            }
        #endregion
        #region M:ForEachSelectedView(View,Action<View>)
        private static void ForEachSelectedView(View referenceView, Action<View> action) {
            var instance = MultiSelectionManager.Instance;
            if (instance.SelectedElementCount > 1 && instance.Contains(referenceView)) {
                foreach (var viewElement in new List<ViewElement>(instance.MultiSelectedElements)) {
                    var view = viewElement as View;
                    if (view != null && view != referenceView)
                        action(view);
                    }
                }
            action(referenceView);
            }
        #endregion
        #region M:CanMoveAllToNextTabGroup(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanMoveAllToNextTabGroup(ExecutedRoutedEventArgs args)
            {
            return IsViewContainedInDocumentGroupContainer(args.Parameter);
            }
        #endregion
        #region M:OnMoveAllToNextTabGroup(Object,ExecutedRoutedEventArgs)
        private static void OnMoveAllToNextTabGroup(Object sender, ExecutedRoutedEventArgs e) {
            if (!CanMoveAllToNextTabGroup(e)) { return; }
            var parameter = (View)e.Parameter;
            using (DeferActiveViewChanges()) {
                parameter.ViewManager.MoveAllToTabGroup(parameter, 1);
                parameter.ViewManager.SetActiveView(parameter, ActivationType.Default);
                }
            }
        #endregion
        #region M:CanMoveAllToPreviousTabGroup(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanMoveAllToPreviousTabGroup(ExecutedRoutedEventArgs args)
            {
            return IsViewContainedInDocumentGroupContainer(args.Parameter);
            }
        #endregion
        #region M:OnMoveAllToPreviousTabGroup(Object,ExecutedRoutedEventArgs)
        private static void OnMoveAllToPreviousTabGroup(Object sender, ExecutedRoutedEventArgs e) {
            if (!CanMoveAllToPreviousTabGroup(e)) { return; }
            var parameter = (View)e.Parameter;
            using (DeferActiveViewChanges()) {
                parameter.ViewManager.MoveAllToTabGroup(parameter, -1);
                parameter.ViewManager.SetActiveView(parameter, ActivationType.Default);
                }
            }
        #endregion
        #region M:MoveToVisibleTabGroup(View,Int32)
        private void MoveToVisibleTabGroup(View view, Int32 visibleOffset) {
            var parent1 = (DocumentGroup)view.Parent;
            var parent2 = (DocumentGroupContainer)parent1.Parent;
            var visiblePosition = parent2.VisibleChildren.IndexOf(parent1);
            var num1 = parent2.ChildIndexFromVisibleChildIndex(visiblePosition);
            var num2 = parent2.ChildIndexFromVisibleChildIndex(visiblePosition + visibleOffset);
            if (-1 == num2)
                throw new InvalidOperationException("No next visible group found");
            var offset = num2 - num1;
            using (DeferActiveViewChanges()) {
                ForEachSelectedView(view, v => MoveToTabGroup(v, offset));
                SetActiveView(view, ActivationType.Default);
                }
            }
        #endregion
        #region M:MoveToTabGroup(ViewElement,Int32)
        private static void MoveToTabGroup(ViewElement view, Int32 tabGroupOffset)
            {
            var parent1 = view.Parent as DocumentGroup;
            if (parent1 == null)
                throw new InvalidOperationException("View that is being moved must be child of a DocumentGroup.");
            var parent2 = parent1.Parent as DocumentGroupContainer;
            if (parent2 == null)
                throw new InvalidOperationException("DocumentGroup must be child of a DocumentGroupContainer");
            var documentGroupList = new List<DocumentGroup>(parent2.FindAll<DocumentGroup>());
            var num = documentGroupList.IndexOf(parent1);
            if (num + tabGroupOffset < 0 || num + tabGroupOffset >= documentGroupList.Count)
                return;
            var targetView = documentGroupList[num + tabGroupOffset];
            view.Detach();
            targetView.Dock(view, DockDirection.Fill);
            targetView.SelectedElement = view;
            }
        #endregion
        #region M:MoveAllToTabGroup(ViewElement,Int32)
        private void MoveAllToTabGroup(ViewElement view, Int32 tabGroupOffset) {
            var parent1 = view.Parent as DocumentGroup;
            if (parent1 == null)
                throw new InvalidOperationException("View that is being moved must be child of a DocumentGroup.");
            var parent2 = parent1.Parent as DocumentGroupContainer;
            if (parent2 == null)
                throw new InvalidOperationException("DocumentGroup must be child of a DocumentGroupContainer");
            var documentGroupList = new List<DocumentGroup>(parent2.FindAll<DocumentGroup>());
            var viewElementList = new List<ViewElement>(parent1.VisibleChildren);
            var num = documentGroupList.IndexOf(parent1);
            if (num + tabGroupOffset < 0 || num + tabGroupOffset >= documentGroupList.Count)
                return;
            var documentGroup = documentGroupList[num + tabGroupOffset];
            var index = 0;
            foreach (var view1 in viewElementList) {
                view1.Detach();
                if (Preferences.DocumentDockPreference == DockPreference.DockAtBeginning) {
                    documentGroup.Children.Insert(index, view1);
                    ++index;
                    }
                else
                    documentGroup.Children.Add(view1);
                }
            documentGroup.SelectedElement = view;
            }
        #endregion
        #region M:CanActivateDocumentView(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanActivateDocumentView(ExecutedRoutedEventArgs args)
            {
            return args.Parameter is View;
            }
        #endregion
        #region M:OnActivateDocumentView(Object,ExecutedRoutedEventArgs)
        private static void OnActivateDocumentView(Object sender, ExecutedRoutedEventArgs args)
            {
            if (!CanActivateDocumentView(args))
                return;
            ((ViewElement)args.Parameter).IsSelected = true;
            }
        #endregion
        #region M:CreateDocumentGroup(ViewElement,Orientation)
        private void CreateDocumentGroup(ViewElement view, Orientation orientation) {
            if (view == null) { throw new ArgumentNullException(nameof(view)); }
            var parent1 = view.Parent as DocumentGroup;
            if (parent1 == null) { throw new InvalidOperationException("View that is being moved must be child of a DocumentGroup."); }
            var parent2 = parent1.Parent as DocumentGroupContainer;
            if (parent2 == null) { throw new InvalidOperationException("DocumentGroup must be child of a DocumentGroupContainer"); }
            var documentGroup = DocumentGroup.Create();
            documentGroup.FloatingHeight = 200.0;
            documentGroup.FloatingWidth = 300.0;
            documentGroup.IsVisible = true;
            if (parent2.VisibleChildren.Count <= 1)
                parent2.Orientation = orientation;
            var num = parent2.Children.IndexOf(parent1);
            parent2.Children.Insert(num + 1, documentGroup);
            }
        #endregion
        #region M:SetActiveView(View,ActivationType)
        public void SetActiveView(View view, ActivationType type) {
            if (view != null)
                {
                var manager = GetViewManager(view);
                }

            if (IsPendingActiveView) {
                pendingActiveView = view;
                }
            else {
                if (Equals(activeView, view)) { return; }
                if (view != null && !Equals(view.WindowProfile, WindowProfile)) { throw new ArgumentException("The ViewManager.ActiveView must be a View contained within the ViewManager.WindowProfile."); }
                var s1 = activeView == null ? String.Empty : activeView.Name;
                var s2 = view == null ? String.Empty : view.Name;
                var byteCount = Encoding.Unicode.GetByteCount(s1);
                var numArray = new Byte[byteCount + Encoding.Unicode.GetByteCount(s2) + 4];
                Encoding.Unicode.GetBytes(s1, 0, s1.Length, numArray, 0);
                Encoding.Unicode.GetBytes(s2, 0, s2.Length, numArray, byteCount + 2);
                using (new CodeMarkerExStartEnd(18115, 18116, numArray, false)) {
                    if (activeView != null)
                        activeView.IsActive = false;
                    activeView = view;
                    if (activeView != null) {
                        activeView.IsActive = true;
                        UtilityMethods.RestoreWindow(activeView);
                        var args = new CancelEventArgs(false);
                        // ISSUE: reference to a compiler-generated field
                        ActivatingWindow.RaiseEvent(this, args);
                        if (!args.Cancel)
                            FloatingWindowManager.ActivateFloatingControl(activeView);
                        }
                    // ISSUE: reference to a compiler-generated field
                    ActiveViewChanged.RaiseEvent(this, new ActiveViewChangedEventArgs(type));
                    }
                }
            }
        #endregion

        public static IDisposable DeferActiveViewChanges()
            {
            return new ActiveViewDeferrer();
            }

        #region M:CanBringFloatingWindowsToFront:Boolean
        public Boolean CanBringFloatingWindowsToFront()
            {
            return Preferences.EnableIndependentFloatingDocumentGroups && FloatingWindowManager.FloatingWindows.Any();
            }
        #endregion
        #region M:BringFloatingWindowsToFront
        public void BringFloatingWindowsToFront()
            {
            if (!CanBringFloatingWindowsToFront()) { return; }
            FloatingWindowManager.BringFloatingWindowsToFront();
            }
        #endregion
        #region M:OnMouseHoverOverAutoHideTabItem(Object,RoutedEventArgs)
        private static void OnMouseHoverOverAutoHideTabItem(Object sender, RoutedEventArgs e) {
            var originalSource = e.OriginalSource as AutoHideTabItem;
            if (originalSource == null) { return; }
            var dataContext = originalSource.DataContext as View;
            if (dataContext == null) { return; }
            dataContext.IsSelected = true;
            }
        #endregion
        #region M:ShouldActivateFromFocusChange(RoutedEventArgs):Boolean
        private static Boolean ShouldActivateFromFocusChange(RoutedEventArgs e) {
            var originalsource = e.OriginalSource as FocusableHwndHost;
            if (originalsource == null) { return true; }
            var r = originalsource.LastFocusedHwnd;
            return (r == IntPtr.Zero) || r == originalsource.Handle;
            }
        #endregion
        #region M:ShouldActivateFromClick(DependencyObject,MouseButtonEventArgs)
        private static Boolean ShouldActivateFromClick(DependencyObject activationElement, MouseButtonEventArgs e) {
            switch (e.ChangedButton)
                {
                case MouseButton.Left:   { return ShouldActivateFromClick(activationElement, e.OriginalSource, ViewPresenter.CanActivateFromLeftClickProperty);   }
                case MouseButton.Middle: { return ShouldActivateFromClick(activationElement, e.OriginalSource, ViewPresenter.CanActivateFromMiddleClickProperty); }
                default: { return true; }
                }
            }
        #endregion
        #region M:IsClickWithinTabItem(MouseButtonEventArgs):Boolean
        private static Boolean IsClickWithinTabItem(MouseButtonEventArgs e) {
            var originalSource = e.OriginalSource as Visual;
            if (originalSource == null)
                return false;
            return originalSource.FindAncestorOrSelf<TabItem>() != null;
            }
        #endregion
        #region M:ShouldActivateFromClick(DependencyObject,Object,DependencyProperty):Boolean
        private static Boolean ShouldActivateFromClick(DependencyObject activationElement, Object originalSource, DependencyProperty canActivateFromClickProperty) {
            for (var sourceElement = originalSource as DependencyObject; sourceElement != null && !Equals(sourceElement, activationElement); sourceElement = sourceElement.GetVisualOrLogicalParent()) {
                if (!(Boolean)sourceElement.GetValue(canActivateFromClickProperty))
                    return false;
                }
            return true;
            }
        #endregion

        protected virtual void ActivateViewOnGotFocus(ViewPresenter presenter)
            {
            ActivateViewFromPresenter(presenter, ActivationType.Default);
            }

        #region M:OnViewPreviewGotKeyboardFocus(Object,KeyboardFocusChangedEventArgs)
        private static void OnViewPreviewGotKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e) {
            if (!ShouldActivateFromFocusChange(e)) { return; }
            var presenter = sender as ViewPresenter;
            if (presenter != null) {
                Instance.ActivateViewFromPresenter(sender as ViewPresenter, ActivationType.Default);
                }
            }
        #endregion
        #region M:OnViewMouseDown(Object,MouseButtonEventArgs)
        private static void OnViewMouseDown(Object sender, MouseButtonEventArgs e) {
            var presenter = sender as ViewPresenter;
            if (presenter == null || !ShouldActivateFromClick(presenter, e)) { return; }
            Instance.ActivateViewFromPresenter(presenter, ActivationType.MouseActivate);
            }
        #endregion
        #region M:ActivateViewFromPresenter(ViewPresenter,ActivationType)
        protected virtual void ActivateViewFromPresenter(ViewPresenter presenter, ActivationType type) {
            if (presenter == null) { return; }
            var dataContext = presenter.DataContext as View;
            if (dataContext == null || dataContext == ActiveView || dataContext.WindowProfile != WindowProfile)
                return;
            SetActiveView(dataContext, type);
            }
        #endregion
        #region M:OnTabItemMouseDown(Object,MouseButtonEventArgs)
        private static void OnTabItemMouseDown(Object sender, MouseButtonEventArgs e) {
            var tabItem = sender as TabItem;
            if (tabItem == null || !ShouldActivateFromClick(tabItem, e)) { return; }
            var dataContext = tabItem.DataContext as View;
            if (dataContext == null) { return; }
            var isSelected = tabItem.IsSelected;
            tabItem.IsSelected = true;
            var manager = dataContext.ViewManager;
            Debug.Assert(manager != null);
            manager.tabGroupSelectionChanged = isSelected != tabItem.IsSelected;
            if (!tabItem.IsSelected) { return; }
            manager.SetActiveView(dataContext, ActivationType.MouseActivate);
            }
        #endregion
        #region M:OnTabItemMouseUp(Object,MouseButtonEventArgs)
        private static void OnTabItemMouseUp(Object sender, MouseButtonEventArgs e)
            {
            Instance.tabGroupSelectionChanged = false;
            }
        #endregion
        #region M:OnTabControlMouseDown(Object,MouseButtonEventArgs)
        private static void OnTabControlMouseDown(Object sender, MouseButtonEventArgs e) {
            var c = sender as TabControl;
            if (c == null || !ShouldActivateFromClick(c, e) || IsClickWithinTabItem(e)) { return; }
            var source = ViewManager.GetViewManager(c);
            Debug.Assert(source != null);
            source.ActivateViewFromTabControl(c, ActivationType.MouseActivate);
            }
        #endregion
        #region M:OnTabControlSelectionChanged(Object,SelectionChangedEventArgs)
        private static void OnTabControlSelectionChanged(Object sender, SelectionChangedEventArgs e) {
            var c = (TabControl)sender;
            if (DoesTabControlContainActiveView(c)) {
                var manager = (c.DataContext as ViewElement).ViewManager;
                manager.ActivateViewFromTabControl(c, ActivationType.Default);
                }
            }
        #endregion
        #region M:DoesTabControlContainActiveView(TabControl):Boolean
        private static Boolean DoesTabControlContainActiveView(TabControl tabControl)
            {
            var r = tabControl.DataContext as ViewGroup;
            return (r != null) && r.Children.Contains(r.ViewManager.ActiveView);
            }
        #endregion
        #region M:ActivateViewFromTabControl(TabControl,ActivationType)
        private void ActivateViewFromTabControl(TabControl tabControl, ActivationType type) {
            var dataContext = tabControl.DataContext as ViewGroup;
            if (dataContext == null) { return; }
            var selectedElement = dataContext.SelectedElement as View;
            if (selectedElement == null || Equals(selectedElement, ActiveView) || !Equals(selectedElement.WindowProfile, WindowProfile))
                return;
            SetActiveView(selectedElement, type);
            }
        #endregion
        #region M:IsAutoDockAllowed(ReorderTabPanel)
        private Boolean IsAutoDockAllowed(ReorderTabPanel panel) {
            if (panel is DocumentTabPanel)
                return Preferences.AllowDocumentTabAutoDocking;
            return Preferences.AllowTabGroupTabAutoDocking;
            }
        #endregion
        #region M:OnViewHeaderDragStarted(Object,DragAbsoluteEventArgs)
        private static void OnViewHeaderDragStarted(Object sender, DragAbsoluteEventArgs e) {
            var originalSource = (DragUndockHeader)e.OriginalSource;
            var M = originalSource?.ViewElement.ViewManager;
            M.CurrentDragUndockHeader = originalSource;
            if (originalSource.ViewElement != null && DockManager.Instance.DraggedViews.Count == 0)
                DockManager.Instance.SetDraggedViewElements(originalSource.ViewElement);
            if (!originalSource.IsWindowTitleBar && originalSource.ViewElement != null) {
                originalSource.CancelDrag();
                if (DockManager.Instance.DraggedTabInfo != null && M.IsAutoDockAllowed(DockManager.Instance.DraggedTabInfo.TabStrip)) {
                    if (-1 != DockManager.Instance.DraggedTabInfo.DraggedTabPosition)
                        DockManager.Instance.DraggedTabInfo.RemoveTabRect(DockManager.Instance.DraggedTabInfo.DraggedTabPosition);
                    DockManager.Instance.DraggedTabInfo.Initialize(originalSource.ViewElement);
                    DockManager.Instance.DraggedTabInfo.DraggedTabPosition = -1;
                    }
                else
                    DockManager.Instance.DraggedTabInfo = null;
                var undockedPosition = Rect.Empty;
                var frameworkElement = originalSource.ViewFrameworkElement;
                if (frameworkElement != null && frameworkElement.IsConnectedToPresentationSource())
                    undockedPosition = new Rect(frameworkElement.PointToScreen(new Point(0.0, 0.0)), DpiHelper.LogicalToDeviceUnits(frameworkElement.RenderSize));
                EventHandler<FloatingWindowEventArgs> eventHandler = (localSender, localArgs) =>
                {
                    var logicalUnits = DpiHelper.DeviceToLogicalUnits(undockedPosition.Size);
                    localArgs.Window.Width += logicalUnits.Width - frameworkElement.RenderSize.Width;
                    localArgs.Window.Height += logicalUnits.Height - frameworkElement.RenderSize.Height;
                };
                try {
                    if (undockedPosition != Rect.Empty)
                        FloatingWindowManager.FloatingWindowShown += eventHandler;
                    if (MultiSelectionManager.Instance.Contains(originalSource.ViewElement)) {
                        var parent = originalSource.ViewElement.Parent as DocumentGroup;
                        parent.UndockMultiSelection(parent.WindowProfile, e.ScreenPoint, undockedPosition);
                        }
                    else {
                        var undockMode = originalSource.IsInTabItem ? UndockMode.Tab : UndockMode.Normal;
                        originalSource.ViewElement.Undock(originalSource.ViewElement.WindowProfile, e.ScreenPoint, undockMode, undockedPosition);
                        }
                    }
                finally {
                    FloatingWindowManager.FloatingWindowShown -= eventHandler;
                    }
                }
            DockManager.Instance.IsDragging = true;
            }
        #endregion
        #region M:OnViewHeaderDragAbsolute(Object,DragAbsoluteEventArgs)
        private static void OnViewHeaderDragAbsolute(Object sender, DragAbsoluteEventArgs e) {
            var originalSource = (DragUndockHeader)e.OriginalSource;
            if (originalSource.IsWindowTitleBar) {
                HandleDragAbsoluteFloatingWindow(originalSource, e);
                }
            else {
                if (DockManager.Instance.DraggedTabInfo == null) { return; }
                HandleDragAbsoluteMoveTabInPlace(originalSource, e);
                }
            }
        #endregion
        #region M:OnViewHeaderDragCompleted(Object,DragAbsoluteCompletedEventArgs)
        private static void OnViewHeaderDragCompleted(Object sender, DragAbsoluteCompletedEventArgs e) {
            if (((DragUndockHeader)e.OriginalSource).IsWindowTitleBar && e.IsCompleted && !NativeMethods.IsKeyPressed(17))
                DockManager.Instance.PerformDrop(e);
            else
                DockManager.Instance.ClearAdorners();
            if (!DraggedTabScope.IsDraggingTab) {
                DockManager.Instance.DraggedTabInfo = null;
                DockManager.Instance.IsDragging = false;
                DockManager.Instance.DraggedViews.Clear();
                }
            var source = ((sender as FrameworkElement).DataContext as View).ViewManager;
            source.CurrentDragUndockHeader = null;
            }
        #endregion
        #region M:OnViewHeaderClicked(Object,RoutedEventArgs)
        private static void OnViewHeaderClicked(Object sender, RoutedEventArgs e) {
            var originalSource = e.OriginalSource as DragUndockHeader;
            var ancestor = originalSource.FindAncestor<ReorderTabPanel>();
            if (ancestor != null) {
                DockManager.Instance.DraggedTabInfo = new DraggedTabInfo();
                DockManager.Instance.DraggedTabInfo.TabStrip = ancestor;
                DockManager.Instance.DraggedTabInfo.DraggedViewElement = originalSource.ViewElement;
                DockManager.Instance.DraggedTabInfo.MeasureTabStrip();
                }
            else
                {
                DockManager.Instance.DraggedTabInfo = null;
                }
            }
        #endregion
        #region M:OnViewHeaderPresentationSourceChanged(Object,SourceChangedEventArgs)
        private void OnViewHeaderPresentationSourceChanged(Object sender, SourceChangedEventArgs e)
            {
            if (e.NewSource != null || CurrentDragUndockHeader == null) { return; }
            DockManager.Instance.ClearAdorners();
            DockManager.Instance.DraggedTabInfo = null;
            DockManager.Instance.IsDragging = false;
            DockManager.Instance.DraggedViews.Clear();
            CurrentDragUndockHeader = null;
            }
        #endregion
        #region M:HandleDragAbsoluteFloatingWindow(DragUndockHeader,DragAbsoluteEventArgs)
        private static void HandleDragAbsoluteFloatingWindow(DragUndockHeader header, DragAbsoluteEventArgs e) {
            if (!NativeMethods.IsKeyPressed(17)) {
                DockManager.Instance.UpdateTargets(e);
                var autodockTarget = DockManager.Instance.GetAutodockTarget(e);
                var flag = true;
                var viewElement1 = header.ViewElement as ViewGroup;
                if (viewElement1 != null)
                    flag = viewElement1.VisibleChildren.Count == 1;
                if (!(autodockTarget != null & flag) || DockManager.Instance.IsFloatingOverDockAdorner)
                    return;
                var viewElement2 = header.ViewElement;
                HandleDockIntoTabStrip(autodockTarget, header, e);
                DockManager.Instance.DraggedTabInfo = autodockTarget;
                DockManager.Instance.DraggedTabInfo.DraggedViewElement = viewElement2;
                }
            else
                DockManager.Instance.ClearAdorners();
            }
        #endregion
        #region M:HandleDragAbsoluteMoveTabInPlace(DragUndockHeader,DragAbsoluteEventArgs)
        private static void HandleDragAbsoluteMoveTabInPlace(DragUndockHeader header, DragAbsoluteEventArgs e) {
            using (new DraggedTabScope()) {
                var draggedTabInfo = DockManager.Instance.DraggedTabInfo;
                var tabIndexAt = draggedTabInfo.GetTabIndexAt(e.ScreenPoint);
                var flag = tabIndexAt == draggedTabInfo.DraggedTabPosition;
                if (flag)
                    draggedTabInfo.ClearVirtualTabRect();
                if (!draggedTabInfo.TabStripRect.Contains(e.ScreenPoint) || (draggedTabInfo.VirtualTabRect.Contains(e.ScreenPoint) || flag))
                    return;
                if (-1 != tabIndexAt) {
                    draggedTabInfo.SetVirtualTabRect(tabIndexAt);
                    var viewElement = header.ViewElement;
                    if (viewElement.IsPinned()) {
                        var parent = viewElement.Parent as DocumentGroup;
                        if (tabIndexAt < parent.PinnedViews.Count)
                            viewElement.MovePinnedTab(tabIndexAt);
                        }
                    else
                        DockOperations.MoveTab(viewElement, tabIndexAt, true);
                    draggedTabInfo.TabStrip.IsNotificationNeeded = true;
                    draggedTabInfo.DraggedTabPosition = tabIndexAt;
                    }
                if (draggedTabInfo.HasBeenReordered)
                    return;
                draggedTabInfo.HasBeenReordered = true;
                draggedTabInfo.ExpandTabStrip();
                }
            }
        #endregion
        #region M:HandleDockIntoTabStrip(DraggedTabInfo,DragUndockHeader,DragAbsoluteEventArgs)
        private static void HandleDockIntoTabStrip(DraggedTabInfo tabInfo, DragUndockHeader header, DragAbsoluteEventArgs e) {
            var dockPosition = tabInfo.GetClosestTabIndexAt(e.ScreenPoint);
            if (-1 == dockPosition)
                return;
            var viewElement = tabInfo.TargetElement;
            if (viewElement == null && tabInfo.GroupContainer != null) {
                viewElement = DockOperations.CreateDocumentGroupAt(tabInfo.GroupContainer, tabInfo.GroupPosition);
                viewElement.DockedHeight = tabInfo.GroupDockedHeight;
                viewElement.DockedWidth = tabInfo.GroupDockedWidth;
                viewElement.FloatingHeight = tabInfo.GroupFloatingHeight;
                viewElement.FloatingWidth = tabInfo.GroupFloatingWidth;
                }
            if (!DockOperations.AreDockRestrictionsFulfilled(header.ViewElement, viewElement))
                return;
            var flag = false;
            var nestedGroup = tabInfo.NestedGroup as ViewGroup;
            if (nestedGroup != null)
                flag = nestedGroup.Children.Contains(header.ViewElement);
            if (!flag && tabInfo.TabRects.Count > 0 && e.ScreenPoint.X > tabInfo.TabRects[tabInfo.TabRects.Count - 1].Right)
                dockPosition = tabInfo.TabRects.Count;
            if (tabInfo.TabRects.Count == 0)
                dockPosition = 0;
            if (DockManager.Instance.DraggedTabInfo != null && -1 != DockManager.Instance.DraggedTabInfo.DraggedTabPosition)
                DockManager.Instance.DraggedTabInfo.RemoveTabRect(DockManager.Instance.DraggedTabInfo.DraggedTabPosition);
            viewElement.DockAt(header.ViewElement, dockPosition);
            tabInfo.TabStrip.IsNotificationNeeded = true;
            tabInfo.DraggedTabPosition = dockPosition;
            tabInfo.ClearVirtualTabRect();
            DockManager.Instance.ClearAdorners();
            }
        #endregion
        #region M:OnFloatingElementDocked(Object,FloatingElementDockedEventArgs)
        private static void OnFloatingElementDocked(Object sender, FloatingElementDockedEventArgs e) {
            if (e.Content == null) { return; }
            DockManager.Instance.IsDragging = false;
            if (e.CreateDocumentGroup) {
                var target = sender as DockTarget;
                var documentGroup = target.DataContext as DocumentGroup;
                var container = target.DataContext as DocumentGroupContainer;
                if (documentGroup == null && container == null) {
                    DockDocumentGroupContainer(target, e);
                    }
                else {
                    if (documentGroup == null)
                        documentGroup = container.Children[0] as DocumentGroup;
                    if (container == null)
                        container = documentGroup.Parent as DocumentGroupContainer;
                    var orientation = container.Orientation;
                    var position = container.Children.IndexOf(documentGroup);
                    switch (e.DockDirection) {
                        case DockDirection.Left:
                            orientation = Orientation.Horizontal;
                            break;
                        case DockDirection.Top:
                            orientation = Orientation.Vertical;
                            break;
                        case DockDirection.Right:
                            orientation = Orientation.Horizontal;
                            ++position;
                            break;
                        case DockDirection.Bottom:
                            orientation = Orientation.Vertical;
                            ++position;
                            break;
                        }
                    container.Orientation = orientation;
                    UpdateFloatingViewsDockSize(e.Content);
                    var content = e.Content as DocumentGroupContainer;
                    if (content != null) {
                        var viewElementList = new List<ViewElement>(content.VisibleChildren);
                        for (var index = 0; index < viewElementList.Count; ++index) {
                            var dockLength = DockManager.Instance.PreviewDockLength / viewElementList.Count;
                            DockInNewDocumentGroup(container, position + index, viewElementList[index], dockLength);
                            }
                        }
                    else
                        DockInNewDocumentGroup(container, position, e.Content, DockManager.Instance.PreviewDockLength);
                    }
                }
            else {
                var dockTarget = sender as DockTarget;
                if (dockTarget.DockTargetType == DockTargetType.Inside || dockTarget.DockTargetType == DockTargetType.CenterOnly || (dockTarget.DockTargetType == DockTargetType.SidesOnly || dockTarget.DockTargetType == DockTargetType.FillPreview) || dockTarget.DockTargetType == DockTargetType.InsertTabPreview) {
                    var targetView = dockTarget.DataContext as ViewElement;
                    var documentGroup = targetView as DocumentGroup;
                    if (documentGroup != null && e.DockDirection != DockDirection.Fill)
                        targetView = documentGroup.Parent;
                    if (-1 != e.InsertPosition && dockTarget.DockTargetType == DockTargetType.InsertTabPreview) {
                        targetView.DockAt(e.Content, e.InsertPosition);
                        }
                    else {
                        UpdateFloatingViewsDockSize(e.Content);
                        targetView.Dock(e.Content, e.DockDirection);
                        }
                    }
                else {
                    UpdateFloatingViewsDockSize(e.Content);
                    (dockTarget.DataContext as ViewGroup).DockOutside(e.Content, e.DockDirection);
                    }
                }
            }
        #endregion
        #region M:DockDocumentGroupContainer(DockTarget,FloatingElementDockedEventArgs)
        private static void DockDocumentGroupContainer(DockTarget target, FloatingElementDockedEventArgs e) {
            var element = target.DataContext as ViewElement;
            var rootElement1 = ViewElement.FindRootElement(element) as FloatSite;
            var rootElement2 = ViewElement.FindRootElement(e.Content) as FloatSite;
            if (rootElement1 == null || rootElement2 == null)
                return;
            var defaultViewSiteContent = WindowProfile.CreateDefaultViewSiteContent();
            var dockRoot = defaultViewSiteContent.Find<DockRoot>(false);
            var parent = element.Parent;
            if (parent is TabGroup) {
                element = parent;
                parent = element.Parent;
                }
            var index = parent.Children.IndexOf(element);
            using (dockRoot.PreventCollapse()) {
                using (rootElement1.PreventCollapse()) {
                    using (parent.PreventCollapse()) {
                        defaultViewSiteContent.Find<DocumentGroupContainer>(false).Detach();
                        var ancestorOrSelf = e.Content.FindAncestorOrSelf<DocumentGroupContainer>();
                        ancestorOrSelf.Detach();
                        var dockGroup = DockGroup.Create();
                        dockGroup.Children.Add(ancestorOrSelf);
                        switch (e.DockDirection) {
                            case DockDirection.Left:
                                dockGroup.Orientation = Orientation.Horizontal;
                                dockGroup.Children.Add(element);
                                break;
                            case DockDirection.Top:
                                dockGroup.Orientation = Orientation.Vertical;
                                dockGroup.Children.Add(element);
                                break;
                            case DockDirection.Right:
                                dockGroup.Orientation = Orientation.Horizontal;
                                dockGroup.Children.Insert(0, element);
                                break;
                            case DockDirection.Bottom:
                                dockGroup.Orientation = Orientation.Vertical;
                                dockGroup.Children.Insert(0, element);
                                break;
                            default:
                                throw new ArgumentException("A DocumentWell can only be docked to the sides of the target ViewElement.");
                            }
                        parent.Children.Insert(index, dockGroup);
                        dockRoot.Children.Add(rootElement1.Child);
                        rootElement1.Child = defaultViewSiteContent;
                        var num = rootElement1.FloatingWidth * rootElement1.FloatingHeight;
                        if (rootElement2.FloatingWidth * rootElement2.FloatingHeight > num) {
                            rootElement1.FloatingWidth = rootElement2.FloatingWidth;
                            rootElement1.FloatingHeight = rootElement2.FloatingHeight;
                            }
                        switch (e.DockDirection) {
                            case DockDirection.Left:
                            case DockDirection.Right:
                                element.DockedWidth = new SplitterLength(dockGroup.FloatingWidth * 0.25);
                                ancestorOrSelf.DockedWidth = new SplitterLength(1.0, 0);
                                break;
                            case DockDirection.Top:
                            case DockDirection.Bottom:
                                element.DockedHeight = new SplitterLength(dockGroup.FloatingHeight * 0.25);
                                ancestorOrSelf.DockedHeight = new SplitterLength(1.0, 0);
                                break;
                            }
                        }
                    }
                }
            }
        #endregion
        #region M:DockInNewDocumentGroup(DocumentGroupContainer,Int32,ViewElement,Double)
        private static void DockInNewDocumentGroup(DocumentGroupContainer container, Int32 position, ViewElement dockingView, Double dockLength) {
            var documentGroupAt = DockOperations.CreateDocumentGroupAt(container, position);
            if (DockManager.Instance.PreviewDockOrientation == Orientation.Horizontal) {
                documentGroupAt.DockedWidth = new SplitterLength(dockLength);
                documentGroupAt.DockedHeight = new SplitterLength(dockingView.FloatingHeight);
                }
            else {
                documentGroupAt.DockedWidth = new SplitterLength(dockingView.FloatingWidth);
                documentGroupAt.DockedHeight = new SplitterLength(dockLength);
                }
            documentGroupAt.Dock(dockingView, DockDirection.Fill);
            }
        #endregion
        #region M:UpdateFloatingViewsDockSize(ViewElement)
        private static void UpdateFloatingViewsDockSize(ViewElement floatingContent) {
            var floatingWidth = floatingContent.FloatingWidth;
            var floatingHeight = floatingContent.FloatingHeight;
            Double num1;
            Double num2;
            if (DockManager.Instance.PreviewDockOrientation == Orientation.Horizontal) {
                num1 = DockManager.Instance.PreviewDockLength;
                num2 = floatingContent.FloatingHeight;
                }
            else {
                num1 = floatingContent.FloatingWidth;
                num2 = DockManager.Instance.PreviewDockLength;
                }
            foreach (var view in floatingContent.FindAll<View>(v => v.IsVisible)) {
                var onScreenContent = GetOnScreenContent(view);
                if (onScreenContent != null) {
                    var frameworkElement = (onScreenContent.FindAncestor<ViewFrame>() ?? (FrameworkElement)onScreenContent.FindAncestor<TabGroupControl>()) ?? onScreenContent.FindAncestor<FloatingWindow>();
                    view.DockedWidth = new SplitterLength(frameworkElement.ActualWidth * num1 / floatingWidth);
                    view.DockedHeight = new SplitterLength(frameworkElement.ActualHeight * num2 / floatingHeight);
                    if (view.Parent != null) {
                        view.Parent.DockedWidth = new SplitterLength(frameworkElement.ActualWidth * num1 / floatingWidth);
                        view.Parent.DockedHeight = new SplitterLength(frameworkElement.ActualHeight * num2 / floatingHeight);
                        }
                    }
                }
            }
        #endregion
        #region M:GetOnScreenContent(View):UIElement
        private static UIElement GetOnScreenContent(View view) {
            if (view.IsOnScreen)
                return view.Content as UIElement;
            var parent = view.Parent as NestedGroup;
            if (parent != null) {
                var selectedElement = parent.SelectedElement as View;
                if (selectedElement != null)
                    return selectedElement.Content as UIElement;
                }
            return null;
            }
        #endregion
        #region M:CanToggleMaximizeRestoreWindow(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanToggleMaximizeRestoreWindow(ExecutedRoutedEventArgs e)
            {
            return e.Parameter is Window;
            }
        #endregion
        #region M:OnToggleMaximizeRestoreWindow(Object,ExecutedRoutedEventArgs)
        private static void OnToggleMaximizeRestoreWindow(Object sender, ExecutedRoutedEventArgs e)
            {
            if (!CanToggleMaximizeRestoreWindow(e)) { return; }
            var parameter = (Window)e.Parameter;
            parameter.WindowState = (parameter.WindowState == WindowState.Maximized)
                ? WindowState.Normal
                : WindowState.Maximized;
            }
        #endregion
        #region M:CanMinimizeWindow(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanMinimizeWindow(ExecutedRoutedEventArgs e)
            {
            return e.Parameter is Window;
            }
        #endregion
        #region M:OnMinimizeWindow(Object,ExecutedRoutedEventArgs)
        private static void OnMinimizeWindow(Object sender, ExecutedRoutedEventArgs args) {
            if (!CanMinimizeWindow(args)) { return; }
            ((Window)args.Parameter).WindowState = WindowState.Minimized;
            }
        #endregion
        #region M:CanCloseWindow(ExecutedRoutedEventArgs):Boolean
        private static Boolean CanCloseWindow(ExecutedRoutedEventArgs e)
            {
            return e.Parameter is Window;
            }
        #endregion
        #region M:OnCloseWindow(Object,ExecutedRoutedEventArgs)
        private static void OnCloseWindow(Object sender, ExecutedRoutedEventArgs e) {
            if (!CanCloseWindow(e)) { return; }
            ((Window)e.Parameter).Close();
            }
        #endregion
        #region M:OnFloatingWindowSizeChanged(Object,SizeChangedEventArgs)
        private static void OnFloatingWindowSizeChanged(Object sender, SizeChangedEventArgs e) {
            var floatingWindow = (FloatingWindow)sender;
            var site = floatingWindow.DataContext as FloatSite;
            if (site == null || floatingWindow.WindowState == WindowState.Minimized) { return; }
            foreach (var viewElement in site.FindAll(element => (element.IsVisible) && !Equals(element, site)))
                {
                viewElement.Display = site.Display;
                viewElement.FloatingWidth = site.FloatingWidth;
                viewElement.FloatingHeight = site.FloatingHeight;
                }
            }
        #endregion
        #region M:OnFloatingWindowLocationChanged(Object,RoutedEventArgs)
        private static void OnFloatingWindowLocationChanged(Object sender, RoutedEventArgs e) {
            var floatingWindow = (FloatingWindow)sender;
            var dataContext = floatingWindow.DataContext as FloatSite;
            if (dataContext == null || floatingWindow.WindowState == WindowState.Minimized)
                return;
            foreach (var viewElement in dataContext.FindAll(element => (element.IsVisible) && !(element is FloatSite)))
                {
                viewElement.Display = dataContext.Display;
                viewElement.FloatingLeft = dataContext.FloatingLeft;
                viewElement.FloatingTop = dataContext.FloatingTop;
                }
            }
        #endregion
        #region M:OnDockPositionChanged(Object,DockEventArgs)
        private static void OnDockPositionChanged(Object sender, DockEventArgs e) {
            if (e.Action == DockAction.ReorderTab) { return; }
            DockManager.Instance.DraggedTabInfo = null;
            DockManager.Instance.IsDragging = false;
            DockManager.Instance.DraggedViews.Clear();
            Instance.CurrentDragUndockHeader = null;
            }
        #endregion
        #region M:OnSelectedItemHidden(Object,SelectedItemHiddenEventArgs)
        private static void OnSelectedItemHidden(Object sender, SelectedItemHiddenEventArgs e)
            {
            foreach (var viewIndexChange in e.ViewsToMove)
                {
                DockOperations.MoveTab(viewIndexChange.View, viewIndexChange.NewIndex, false);
                }
            }
        #endregion

        protected virtual void MergeResources() {
            if (Application.Current == null) {
                var application = new Application();
                }
            try
            {
                Application.Current.Resources.MergedDictionaries.Add(LoadResourceValue<ResourceDictionary>("Templates.xaml"));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw new Exception(e.Message, e);
            }
            
            }

        internal static T LoadResourceValue<T>(String xamlName) {
            return (T)Application.LoadComponent(new Uri(Assembly.GetExecutingAssembly().GetName().Name + ";component/" + xamlName, UriKind.Relative));
            }

        #region M:OnViewSitesChanged(Object,NotifyCollectionChangedEventArgs)
        private void OnViewSitesChanged(Object sender, NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null) {
                foreach (var oldItem in e.OldItems) {
                    var floatSite = oldItem as FloatSite;
                    if (floatSite != null)
                        FloatingWindowManager.RemoveFloat(floatSite);
                    }
                }
            if (e.NewItems == null) { return; }
            foreach (var newItem in e.NewItems) {
                var floatSite = newItem as FloatSite;
                if (floatSite != null) { FloatingWindowManager.AddFloat(floatSite); }
                }
            }
        #endregion

        private class ToggleDockedBookmarkResolver : IBookmarkResolver
            {
            private static ToggleDockedBookmarkResolver _instance;

            public static ToggleDockedBookmarkResolver Instance {
                get
                    {
                    return _instance ?? (_instance = new ToggleDockedBookmarkResolver());
                    }
                }

            private ToggleDockedBookmarkResolver() {
                }

            public ViewElement SelectBookmarkLocation(View view, WindowProfile windowProfile, ViewBookmarkType bookmarkType) {
                var mainSite = windowProfile.Find<MainSite>();
                if (mainSite == null)
                    return null;
                return mainSite.Find<DocumentGroup>(false) ?? null;
                }

            public void DockToBookmarkLocation(ViewElement bookmarkLocation, View view, WindowProfile windowProfile, ViewBookmarkType bookmarkType) {
                var targetView = bookmarkLocation as DocumentGroup;
                if (bookmarkLocation == null)
                    return;
                if (DocumentGroup.IsTabbedDocument(view) || (view.DockRestriction & DockRestrictionType.DocumentGroup) != DockRestrictionType.None)
                    targetView.Dock(view, DockDirection.Fill);
                else
                    targetView.Parent.Dock(view, DockDirection.Bottom);
                view.IsSelected = true;
                }
            }

        internal class ActiveViewDeferrer : DisposableObject
            {
            private static Int32 refCount;

            public ActiveViewDeferrer() {
                if (refCount == 0) {
                    instance_.pendingActiveView = instance_.ActiveView;
                    instance_.IsPendingActiveView = true;
                    }
                ++refCount;
                }

            protected override void DisposeManagedResources() {
                base.DisposeManagedResources();
                --refCount;
                if (refCount != 0)
                    return;
                instance_.IsPendingActiveView = false;
                instance_.SetActiveView(instance_.pendingActiveView, ActivationType.Default);
                instance_.pendingActiveView = null;
                }
            }

        private class DraggedTabScope : DisposableObject
            {
            private static Int32 refCount;

            public static Boolean IsDraggingTab {
                get
                    {
                    return refCount > 0;
                    }
                }

            public DraggedTabScope() {
                ++refCount;
                }

            protected override void DisposeManagedResources() {
                --refCount;
                }
            }

        #region P:ViewManager.ViewManager:ViewManager
        public static readonly DependencyProperty ViewManagerProperty = DependencyProperty.RegisterAttached("ViewManager", typeof(ViewManager), typeof(ViewManager), new PropertyMetadata(default(ViewManager)));
        public static void SetViewManager(DependencyObject source, ViewManager value) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            source.SetValue(ViewManagerProperty, value);
            }

        public static ViewManager GetViewManager(DependencyObject source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return (ViewManager)source.GetValue(ViewManagerProperty);
            }
        #endregion
        #region M:BindViewManager(DependencyObject,DependencyObject)
        internal static void BindViewManager(DependencyObject target, DependencyObject source) {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            BindingOperations.SetBinding(target, ViewManagerProperty, new Binding {
                Path = new PropertyPath(ViewManagerProperty),
                Source = source,
                Mode = BindingMode.OneWay
                });
            }
        #endregion
        #region M:ClearViewManager(DependencyObject)
        internal static void ClearViewManager(DependencyObject target) {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            BindingOperations.ClearBinding(target, ViewManagerProperty);
            }
        #endregion

        internal ViewManager(String name)
            {
            Name = name;
            }

        public override String ToString()
            {
            return Name;
            }
        }
    }
