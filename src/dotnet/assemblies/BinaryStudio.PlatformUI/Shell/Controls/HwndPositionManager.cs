using System;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public sealed class HwndPositionManager
        {
        private static HwndPositionManager current;

        public static HwndPositionManager Current
            {
            get
                {
                return current ?? (current = new HwndPositionManager());
                }
            }

        public event EventHandler<HwndPositionChangedEventArgs> PositionChanged;

        public void ReportPositionChange(IntPtr hWnd, Int32 zIndex)
            {
            // ISSUE: reference to a compiler-generated field
            var positionChanged = PositionChanged;
            if (positionChanged == null)
                return;
            positionChanged(this, new HwndPositionChangedEventArgs(hWnd, zIndex));
            }
        }
    }