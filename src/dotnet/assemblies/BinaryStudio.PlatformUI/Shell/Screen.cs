using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.PlatformUI.Shell.Controls;

namespace BinaryStudio.PlatformUI.Shell
    {
    public static class Screen
        {
        private static List<MONITORINFO> _displays = new List<MONITORINFO>();

        public static Int32 DisplayCount
            {
            get
                {
                return _displays.Count;
                }
            }

        public static event EventHandler DisplayConfigChanged;

        static Screen()
            {
            BroadcastMessageMonitor.Instance.DisplayChange += OnDisplayChange;
            UpdateDisplays();
            }

        internal static void FindMaximumSingleMonitorRectangle(RECT windowRect, out RECT screenSubRect, out RECT monitorRect)
            {
            var displayForWindowRect = FindDisplayForWindowRect(new Rect(windowRect.Left, windowRect.Top, windowRect.Width, windowRect.Height));
            screenSubRect = new RECT
                {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
                };
            monitorRect = new RECT
                {
                Left = 0,
                Right = 0,
                Top = 0,
                Bottom = 0
                };
            if (-1 == displayForWindowRect)
                return;
            var display = _displays[displayForWindowRect];
            var rcWork = display.rcWork;
            RECT lprcDst;
            NativeMethods.IntersectRect(out lprcDst, ref rcWork, ref windowRect);
            screenSubRect = lprcDst;
            monitorRect = display.rcWork;
            }

        internal static void FindMaximumSingleMonitorRectangle(Rect windowRect, out Rect screenSubRect, out Rect monitorRect)
            {
            RECT screenSubRect1;
            RECT monitorRect1;
            FindMaximumSingleMonitorRectangle(new RECT(windowRect), out screenSubRect1, out monitorRect1);
            screenSubRect = new Rect(screenSubRect1.Position, screenSubRect1.Size);
            monitorRect = new Rect(monitorRect1.Position, monitorRect1.Size);
            }

        internal static void FindMonitorRectsFromPoint(Point point, out Rect monitorRect, out Rect workAreaRect)
            {
            var hMonitor = NativeMethods.MonitorFromPoint(new POINT
                {
                x = (Int32)point.X,
                y = (Int32)point.Y
                }, 2);
            monitorRect = new Rect(0.0, 0.0, 0.0, 0.0);
            workAreaRect = new Rect(0.0, 0.0, 0.0, 0.0);
            if (!(hMonitor != IntPtr.Zero))
                return;
            var monitorInfo = new MONITORINFO();
            monitorInfo.cbSize = (UInt32)Marshal.SizeOf(typeof(MONITORINFO));
            NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);
            monitorRect = new Rect(monitorInfo.rcMonitor.Position, monitorInfo.rcMonitor.Size);
            workAreaRect = new Rect(monitorInfo.rcWork.Position, monitorInfo.rcWork.Size);
            }

        public static Int32 FindDisplayForWindowRect(Rect windowRect)
            {
            var num1 = -1;
            var lprcSrc2 = new RECT(windowRect);
            Int64 num2 = 0;
            for (var index = 0; index < _displays.Count; ++index)
                {
                var rcWork = _displays[index].rcWork;
                RECT lprcDst;
                NativeMethods.IntersectRect(out lprcDst, ref rcWork, ref lprcSrc2);
                var num3 = (Int64)(lprcDst.Width * lprcDst.Height);
                if (num3 > num2)
                    {
                    num1 = index;
                    num2 = num3;
                    }
                }
            if (-1 == num1)
                {
                var num3 = Double.MaxValue;
                for (var index = 0; index < _displays.Count; ++index)
                    {
                    var num4 = Distance(_displays[index].rcMonitor, lprcSrc2);
                    if (num4 < num3)
                        {
                        num1 = index;
                        num3 = num4;
                        }
                    }
                }
            return num1;
            }

        public static Int32 FindDisplayForAbsolutePosition(Point absolutePosition)
            {
            for (var index = 0; index < _displays.Count; ++index)
                {
                var rcMonitor = _displays[index].rcMonitor;
                if (rcMonitor.Left <= absolutePosition.X && rcMonitor.Right >= absolutePosition.X && (rcMonitor.Top <= absolutePosition.Y && rcMonitor.Bottom >= absolutePosition.Y))
                    return index;
                }
            var num1 = -1;
            var num2 = Double.MaxValue;
            for (var index = 0; index < _displays.Count; ++index)
                {
                var num3 = Distance(absolutePosition, _displays[index].rcMonitor);
                if (num3 < num2)
                    {
                    num1 = index;
                    num2 = num3;
                    }
                }
            return num1;
            }

        public static void AbsolutePositionToRelativePosition(Double left, Double top, out Int32 display, out Point relativePosition)
            {
            display = FindDisplayForAbsolutePosition(new Point(left, top));
            relativePosition = new Point();
            if (-1 == display)
                return;
            relativePosition.X = left - _displays[display].rcMonitor.Left;
            relativePosition.Y = top - _displays[display].rcMonitor.Top;
            }

        public static void AbsoluteRectToRelativeRect(Double left, Double top, Double width, Double height, out Int32 display, out Rect relativePosition)
            {
            AbsoluteRectToRelativeRect(new Rect(new Point(left, top), new Size(width, height)), out display, out relativePosition);
            }

        public static void AbsoluteRectToRelativeRect(Rect absoluteRect, out Int32 display, out Rect relativeRect)
            {
            display = FindDisplayForWindowRect(absoluteRect);
            relativeRect = AbsoluteRectToRelativeRect(display, absoluteRect);
            }

        public static Rect AbsoluteRectToRelativeRect(Int32 display, Rect absoluteRect)
            {
            Validate.IsWithinRange(display, 0, _displays.Count - 1, "display");
            var rcMonitor = _displays[display].rcMonitor;
            return new Rect(absoluteRect.X - rcMonitor.Left, absoluteRect.Y - rcMonitor.Top, absoluteRect.Width, absoluteRect.Height);
            }

        public static Point RelativePositionToAbsolutePosition(Int32 display, Double left, Double top)
            {
            if (display < 0)
                throw new ArgumentOutOfRangeException(nameof(display));
            RECT rect;
            if (display >= _displays.Count)
                {
                var display1 = _displays[_displays.Count - 1];
                rect = new RECT(display1.rcMonitor.Left + display1.rcMonitor.Width, display1.rcMonitor.Top, display1.rcMonitor.Right + display1.rcMonitor.Width, display1.rcMonitor.Bottom);
                }
            else
                rect = _displays[display].rcMonitor;
            return new Point(rect.Left + left, rect.Top + top);
            }

        public static Rect RelativeRectToAbsoluteRect(Int32 display, Rect relativeRect)
            {
            return new Rect(RelativePositionToAbsolutePosition(display, relativeRect.Left, relativeRect.Top), relativeRect.Size);
            }

        internal static Point WorkAreaToScreen(Point pt)
            {
            Rect monitorRect;
            Rect workAreaRect;
            FindMonitorRectsFromPoint(pt, out monitorRect, out workAreaRect);
            return new Point(pt.X - monitorRect.Left + workAreaRect.Left, pt.Y - monitorRect.Top + workAreaRect.Top);
            }

        internal static Point ScreenToWorkArea(Point pt)
            {
            Rect monitorRect;
            Rect workAreaRect;
            FindMonitorRectsFromPoint(pt, out monitorRect, out workAreaRect);
            return new Point(pt.X - workAreaRect.Left + monitorRect.Left, pt.Y - workAreaRect.Top + monitorRect.Top);
            }

        private static Double Distance(RECT rect1, RECT rect2)
            {
            return Distance(GetRectCenter(rect1), GetRectCenter(rect2));
            }

        private static Double Distance(Point point, RECT rect)
            {
            return Distance(point, GetRectCenter(rect));
            }

        private static Double Distance(Point point1, Point point2)
            {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2.0) + Math.Pow(point1.Y - point2.Y, 2.0));
            }

        private static Point GetRectCenter(RECT rect)
            {
            return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            }

        private static void OnDisplayChange(Object sender, EventArgs e)
            {
            UpdateDisplays();
            }

        private static void UpdateDisplays()
            {
            _displays.Clear();
            NativeMethods.EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT rect, IntPtr lpData) =>
            {
                var monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = (UInt32)Marshal.SizeOf(typeof(MONITORINFO));
                if (NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo))
                    _displays.Add(monitorInfo);
                return true;
            }, IntPtr.Zero);
            // ISSUE: reference to a compiler-generated field
            DisplayConfigChanged.RaiseEvent(null);
            }

        internal static void SetDisplays(IEnumerable<MONITORINFO> displays)
            {
            Validate.IsNotNull(displays, "displays");
            _displays.Clear();
            _displays.AddRange(displays);
            }
        }
    }