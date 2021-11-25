using System;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public sealed class HwndPositionChangedEventArgs : EventArgs
        {
        public IntPtr Handle { get; private set; }

        public Int32 ZIndex { get; private set; }

        public HwndPositionChangedEventArgs(IntPtr handle, Int32 zIndex)
            {
            Handle = handle;
            ZIndex = zIndex;
            }
        }
    }