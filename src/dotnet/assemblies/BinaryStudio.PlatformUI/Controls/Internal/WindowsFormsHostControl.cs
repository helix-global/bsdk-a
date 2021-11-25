using System;
using System.Diagnostics;
using System.Windows.Interop;
using BinaryStudio.PlatformUI.Shell.Controls;
using Microsoft.Win32;

namespace BinaryStudio.PlatformUI.Controls.Internal
    {
    internal class WindowsFormsHostControl : FocusableWindowsFormsHost {
        private static String ToString(IntPtr source) {
            return String.Format("{0:X8}", (Int64)source);
            }
        protected override IntPtr WndProc(IntPtr hwnd, Int32 msg, IntPtr W, IntPtr L, ref Boolean handled) {
            Debug.Print("{4}:WndProc:HWND={1},WPARAM={2},LPARAM={3},AHWND:{5},MSG={0}", (NativeWindowMessage)msg, ToString(hwnd), ToString(W), ToString(L), ToString(Handle), ToString(NativeMethods.GetActiveWindow()));
            return base.WndProc(hwnd, msg, W, L, ref handled);
            }

        public WindowsFormsHostControl(HwndSource topLevelHwndSource) : base(topLevelHwndSource)
            {
            }
        }
    }