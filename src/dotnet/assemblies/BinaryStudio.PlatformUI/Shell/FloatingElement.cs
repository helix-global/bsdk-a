using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace BinaryStudio.PlatformUI.Shell.Controls {
    public class FloatingElement : CustomChromeWindow
        {
        private Boolean closedInternally;

        protected Boolean IsClosing { get; private set; }

        public FloatingElement() {
            IsVisibleChanged += OnIsVisibleChanged;
            }

        public void ForceClose() {
            closedInternally = true;
            if (IsClosing)
                return;
            Close();
            }

        /// <summary>Raises the <see cref="E:System.Windows.Window.Closing" /> event.</summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e) {
            if (closedInternally) {
                TryActivateOwner();
                }
            else {
                IsClosing = true;
                try {
                    base.OnClosing(e);
                    if (e.Cancel)
                        return;
                    TryActivateOwner();
                    }
                finally {
                    IsClosing = false;
                    }
                }
            }

        private void TryActivateOwner() {
            var windowInteropHelper = new WindowInteropHelper(this);
            if (!(windowInteropHelper.Owner != IntPtr.Zero)) { return; }
            var window = GetWindow(windowInteropHelper.Owner);
            if (window != null)
                {
                window.Focus();
                }
            else
                {
                NativeMethods.SetActiveWindow(windowInteropHelper.Owner);
                }
            }

        private Window GetWindow(IntPtr hwnd) {
            var hwndSource = HwndSource.FromHwnd(hwnd);
            if (hwndSource != null)
                return hwndSource.RootVisual as Window;
            return null;
            }

        private void OnIsVisibleChanged(Object sender, DependencyPropertyChangedEventArgs args) {
            if (IsVisible)
                DockManager.Instance.RegisterSite(this);
            else
                DockManager.Instance.UnregisterSite(this);
            }

        /// <summary>Called when the <see cref="P:System.Windows.Controls.ContentControl.Content" /> property changes.</summary>
        /// <param name="oldContent">A reference to the root of the old content tree.</param>
        /// <param name="newContent">A reference to the root of the new content tree.</param>
        protected override void OnContentChanged(Object oldContent, Object newContent) {
            base.OnContentChanged(oldContent, newContent);
            LayoutSynchronizer.Update(this);
            }
        }
    }