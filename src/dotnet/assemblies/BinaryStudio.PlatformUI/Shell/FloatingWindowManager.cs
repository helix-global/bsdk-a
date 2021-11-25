using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.PlatformUI.Shell.Controls;
using BinaryStudio.DataProcessing;
using Application = System.Windows.Application;

namespace BinaryStudio.PlatformUI.Shell
    {
    public class FloatingWindowManager
        {
        private readonly Dictionary<FloatSite, FloatingWindow> floatingWindows = new Dictionary<FloatSite, FloatingWindow>();
        private readonly List<IntPtr> floatingWindowHandles = new List<IntPtr>();
        private readonly HashSet<FloatSite> delayShownElements = new HashSet<FloatSite>();
        private readonly HashSet<FloatSite> delayHiddenElements = new HashSet<FloatSite>();
        private IntPtr ownerWindow;
        private Int32 deferFloatingVisibilityChangesRefCount;
        private Boolean isProcessingVisibilityChanges;
        private ImageSource floatingWindowIcon;

        public IntPtr OwnerWindow
            {
            get
                {
                return ownerWindow;
                }
            set
                {
                if (!(ownerWindow != value))
                    return;
                ownerWindow = value;
                UpdateFloatingOwners();
                TryProcessingPendingVisibilityChanges();
                }
            }

        public ImageSource FloatingWindowIcon
            {
            get
                {
                return floatingWindowIcon;
                }
            set
                {
                if (floatingWindowIcon != null)
                    return;
                floatingWindowIcon = value;
                foreach (var window in floatingWindows.Values)
                    window.Icon = FloatingWindowIcon;
                }
            }

        private Boolean CanChangeFloatingVisibility
            {
            get
                {
                if (OwnerWindow != IntPtr.Zero)
                    return deferFloatingVisibilityChangesRefCount == 0;
                return false;
                }
            }

        public IEnumerable<FloatingWindow> FloatingWindows
            {
            get
                {
                return floatingWindows.Values;
                }
            }

        public static event EventHandler<FloatingWindowEventArgs> FloatingWindowCreated;
        public static event EventHandler<FloatingWindowEventArgs> FloatingWindowShown;
        public static event EventHandler<FloatingWindowEventArgs> FloatingWindowClosed;

        private void DelayShowElement(FloatSite element)
            {
            if (delayHiddenElements.Contains(element))
                delayHiddenElements.Remove(element);
            delayShownElements.Add(element);
            }

        private void DelayHideElement(FloatSite element)
            {
            if (delayShownElements.Contains(element))
                delayShownElements.Remove(element);
            delayHiddenElements.Add(element);
            }

        private void TryProcessingPendingVisibilityChanges()
            {
            if (!CanChangeFloatingVisibility)
                return;
            isProcessingVisibilityChanges = true;
            try
                {
                foreach (var delayShownElement in delayShownElements)
                    ShowFloat(delayShownElement, false);
                foreach (var delayHiddenElement in delayHiddenElements)
                    CloseFloat(delayHiddenElement);
                delayShownElements.Clear();
                delayHiddenElements.Clear();
                }
            finally
                {
                isProcessingVisibilityChanges = false;
                }
            }

        public FloatingWindow TryGetFloatingWindow(FloatSite site)
            {
            if (site == null)
                throw new ArgumentNullException(nameof(site));
            FloatingWindow floatingWindow;
            if (!floatingWindows.TryGetValue(site, out floatingWindow))
                floatingWindow = null;
            return floatingWindow;
            }

        public void RemoveAllFloats(WindowProfile profile)
            {
            foreach (var child in profile.Children)
                {
                var floatSite = child as FloatSite;
                if (floatSite != null)
                    RemoveFloat(floatSite);
                }
            }

        public void AddFloat(FloatSite floatSite)
            {
            floatSite.IsVisibleChanged += OnFloatSiteIsVisibleChanged;
            floatSite.HasDocumentGroupContainerChanged += OnFloatSiteHasDocumentGroupContainerChanged;
            if (!floatSite.IsVisible)
                return;
            ShowFloat(floatSite, true);
            }

        public void RemoveFloat(FloatSite floatSite)
            {
            floatSite.IsVisibleChanged -= OnFloatSiteIsVisibleChanged;
            floatSite.HasDocumentGroupContainerChanged -= OnFloatSiteHasDocumentGroupContainerChanged;
            CloseFloat(floatSite);
            }

        public IDisposable DeferFloatingVisibilityChanges()
            {
            return new DeferFloatingChangesScope(this);
            }

        internal void BringFloatingWindowsToFront()
            {
            var floatingWindowList = new List<FloatingWindow>(floatingWindows.Values);
            foreach (var floatingWindow in floatingWindows.Values)
                {
                var windowZorder = GetWindowZOrder(floatingWindow, false);
                if (windowZorder != floatingWindowList.IndexOf(floatingWindow))
                    {
                    floatingWindowList.Remove(floatingWindow);
                    floatingWindowList.Insert(windowZorder, floatingWindow);
                    }
                }
            floatingWindowList.Reverse();
            foreach (var floatingWindow in floatingWindowList)
                {
                if (new WindowInteropHelper(floatingWindow).Owner == IntPtr.Zero)
                    {
                    if (floatingWindow.WindowState == WindowState.Minimized)
                        floatingWindow.WindowState = WindowState.Normal;
                    floatingWindow.Activate();
                    }
                }
            }

        internal Int32 GetWindowZOrder(Window window)
            {
            return GetWindowZOrder(window, true);
            }

        private Int32 GetWindowZOrder(Window window, Boolean includeMainWindow)
            {
            var hwnd = new WindowInteropHelper(window).Handle;
            var num1 = 0;
            var num2 = !includeMainWindow ? new IntPtr(-1) : new WindowInteropHelper(Application.Current.MainWindow).Handle;
            while (hwnd != IntPtr.Zero)
                {
                hwnd = NativeMethods.GetWindow(hwnd, 3);
                if (hwnd != IntPtr.Zero && (num2 == hwnd || floatingWindowHandles.Contains(hwnd)))
                    ++num1;
                }
            return num1;
            }

        private void ShowFloat(FloatSite floatSite, Boolean showActivated)
            {
            if (CanChangeFloatingVisibility || isProcessingVisibilityChanges)
                {
                EnsureCorrectPosition(floatSite);
                var floatingControl = GetFloatingControl(floatSite);
                floatingControl.DataContext = floatSite;
                floatingControl.Icon = FloatingWindowIcon;
                if (floatSite.FloatingWindowState == WindowState.Minimized)
                    floatingControl.WindowState = floatSite.FloatingWindowState;
                var absolutePosition = Screen.RelativePositionToAbsolutePosition(floatSite.Display, floatSite.FloatingLeft, floatSite.FloatingTop);
                floatingControl.Left = absolutePosition.X;
                floatingControl.Top = absolutePosition.Y;
                floatingControl.Width = floatSite.FloatingWidth;
                floatingControl.Height = floatSite.FloatingHeight;
                floatingControl.ShowActivated = floatingControl.WindowState == WindowState.Maximized || showActivated;
                floatingControl.Show();
                floatingWindowHandles.Add(new WindowInteropHelper(floatingControl).Handle);
                // ISSUE: reference to a compiler-generated field
                FloatingWindowShown.RaiseEvent(this, new FloatingWindowEventArgs(floatingControl));
                }
            else
                DelayShowElement(floatSite);
            }

        private void CloseFloat(FloatSite floatSite)
            {
            if (CanChangeFloatingVisibility || isProcessingVisibilityChanges)
                {
                FloatingWindow window;
                if (!floatingWindows.TryGetValue(floatSite, out window))
                    return;
                var handle = new WindowInteropHelper(window).Handle;
                window.ForceClose();
                floatingWindows.Remove(floatSite);
                floatingWindowHandles.Remove(handle);
                // ISSUE: reference to a compiler-generated field
                FloatingWindowClosed.RaiseEvent(this, new FloatingWindowEventArgs(window));
                }
            else
                DelayHideElement(floatSite);
            }

        protected virtual FloatingWindow CreateFloatingWindow()
            {
            var window = new FloatingWindow();
            // ISSUE: reference to a compiler-generated field
            FloatingWindowCreated.RaiseEvent(this, new FloatingWindowEventArgs(window));
            return window;
            }

        private FloatingWindow GetFloatingControl(FloatSite floatSite)
            {
            FloatingWindow floatingWindow;
            if (!floatingWindows.TryGetValue(floatSite, out floatingWindow))
                {
                floatingWindow = CreateFloatingWindow();
                floatingWindow.Closing += OnFloatingControlClosing;
                SetWindowOwner(floatingWindow, floatSite);
                floatingWindows[floatSite] = floatingWindow;
                ViewManager.SetViewManager(floatingWindow, floatSite.ViewManager);
                }
            return floatingWindow;
            }

        private void SetWindowOwner(CustomChromeWindow window, FloatSite floatSite)
            {
            if (floatSite.HasDocumentGroupContainer && ViewManager.Instance.Preferences.EnableIndependentFloatingDocumentGroups || !floatSite.HasDocumentGroupContainer && ViewManager.Instance.Preferences.EnableIndependentFloatingToolwindows)
                window.ChangeOwner(IntPtr.Zero);
            else
                window.ChangeOwner(OwnerWindow);
            window.ChangeOwnerForActivate(OwnerWindow);
            }

        private void OnFloatSiteIsVisibleChanged(Object sender, EventArgs args)
            {
            var floatSite = (FloatSite)sender;
            if (floatSite.IsVisible)
                ShowFloat(floatSite, true);
            else
                CloseFloat(floatSite);
            }

        private void OnFloatSiteHasDocumentGroupContainerChanged(Object sender, EventArgs args)
            {
            var floatSite = (FloatSite)sender;
            FloatingWindow floatingWindow;
            if (!floatingWindows.TryGetValue(floatSite, out floatingWindow))
                return;
            SetWindowOwner(floatingWindow, floatSite);
            }

        public void ActivateFloatingControl(ViewElement element)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            var rootElement = ViewElement.FindRootElement(element) as FloatSite;
            FloatingWindow floatingWindow;
            if (rootElement == null || !floatingWindows.TryGetValue(rootElement, out floatingWindow))
                return;
            floatingWindow.Activate();
            }

        private void OnFloatingControlClosing(Object sender, CancelEventArgs args)
            {
            var dataContext = ((FrameworkElement)sender).DataContext as FloatSite;
            if (dataContext == null)
                return;
            foreach (var view in new List<View>(dataContext.FindAll<View>()))
                view.Hide();
            args.Cancel = dataContext.IsVisible;
            }

        internal void UpdateFloatingOwners()
            {
            foreach (var floatingWindow in floatingWindows)
                SetWindowOwner(floatingWindow.Value, floatingWindow.Key);
            }

        private void EnsureCorrectPosition(FloatSite site)
            {
            var profile = site.WindowProfile ?? ViewManager.Instance.WindowProfile;
            var floatingPosition = UtilityMethods.CalculateFloatingPosition(site, profile);
            site.FloatingLeft = floatingPosition.X;
            site.FloatingTop = floatingPosition.Y;
            site.EnsureOnScreen();
            }

        private class DeferFloatingChangesScope : DisposableObject
            {
            private FloatingWindowManager Manager { get; }

            public DeferFloatingChangesScope(FloatingWindowManager manager)
                {
                Manager = manager;
                ++Manager.deferFloatingVisibilityChangesRefCount;
                }

            protected override void DisposeManagedResources()
                {
                --Manager.deferFloatingVisibilityChangesRefCount;
                if (Manager.deferFloatingVisibilityChangesRefCount != 0)
                    return;
                Manager.TryProcessingPendingVisibilityChanges();
                }
            }
        }

    }