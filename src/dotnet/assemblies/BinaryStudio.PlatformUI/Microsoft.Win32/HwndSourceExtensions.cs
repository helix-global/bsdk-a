using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Microsoft.Win32
    {
    internal static class HwndSourceExtensions
        {
        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static Boolean SetWindowPosition(this HwndSource source, Double x, Double y, Double cx, Double cy, WindowSizingAndPositioningFlags flags) {
            return SetWindowPosition(source,IntPtr.Zero, (Int32)x, (Int32)y, (Int32)cx, (Int32)cy, flags);
            }


        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="point"></param>
        /// <param name="size"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static Boolean SetWindowPosition(this HwndSource source, Point point, Size size, WindowSizingAndPositioningFlags flags) {
            return SetWindowPosition(source, IntPtr.Zero, 
                (Int32)point.X, (Int32)point.Y,
                (Int32)size.Width, (Int32)size.Height,
                flags | WindowSizingAndPositioningFlags.RetainsTheCurrentZOrder);
            }

        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="hwndInsertAfter"></param>
        /// <param name="point"></param>
        /// <param name="size"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static Boolean SetWindowPosition(this HwndSource source, IntPtr hwndInsertAfter, Point point, Size size, WindowSizingAndPositioningFlags flags) {
            return SetWindowPosition(source, hwndInsertAfter,
                (Int32)point.X, (Int32)point.Y,
                (Int32)size.Width, (Int32)size.Height, flags);
            }

        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="hwndInsertAfter"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static Boolean SetWindowPosition(this HwndSource source, IntPtr hwndInsertAfter, Int32 x, Int32 y, Int32 cx, Int32 cy, UInt32 flags) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return SetWindowPosition(source.Handle, hwndInsertAfter, x, y, cx, cy, flags);
            }

        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="hwndInsertAfter"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static Boolean SetWindowPosition(this HwndSource source, IntPtr hwndInsertAfter, Int32 x, Int32 y, Int32 cx, Int32 cy, WindowSizingAndPositioningFlags flags) {
            return SetWindowPosition(source, hwndInsertAfter, x, y, cx, cy, (UInt32)flags);
            }

        [DllImport("User32", CharSet = CharSet.Auto, EntryPoint = "SetWindowPos")] [return: MarshalAs(UnmanagedType.Bool)] private static extern Boolean SetWindowPosition(IntPtr hWnd, IntPtr hWndInsertAfter, Int32 x, Int32 y, Int32 cx, Int32 cy, UInt32 flags);
        }
    }