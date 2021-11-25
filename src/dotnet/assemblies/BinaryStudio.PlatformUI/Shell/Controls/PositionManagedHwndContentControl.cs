using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public abstract class PositionManagedHwndContentControl : HwndContentControl
        {
        protected abstract Int32 ZIndex {
            get;
            }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent) {
            HwndPositionManager.Current.PositionChanged += OnHwndPositionChanged;
            return base.BuildWindowCore(hwndParent);
            }

        protected override void DestroyWindowCore(HandleRef hwnd) {
            HwndPositionManager.Current.PositionChanged -= OnHwndPositionChanged;
            base.DestroyWindowCore(hwnd);
            }

        protected override IntPtr WndProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled) {
            if (msg == 71) {
                HwndPositionManager.Current.ReportPositionChange(Handle, ZIndex);
                }
            return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
            }

        private void OnHwndPositionChanged(Object sender, HwndPositionChangedEventArgs e) {
            if (e.ZIndex < ZIndex) {
                NativeMethods.BringWindowToTop(HwndSource.Handle);
                }
            }
        }
    }