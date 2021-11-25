using System;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public static class WindowResizeGripDirectionExtensions
        {
        public static Boolean IsResizingLeft(this WindowResizeGripDirection direction)
            {
            if (direction != WindowResizeGripDirection.Left && direction != WindowResizeGripDirection.TopLeft)
                return direction == WindowResizeGripDirection.BottomLeft;
            return true;
            }

        public static Boolean IsResizingRight(this WindowResizeGripDirection direction)
            {
            if (direction != WindowResizeGripDirection.Right && direction != WindowResizeGripDirection.TopRight)
                return direction == WindowResizeGripDirection.BottomRight;
            return true;
            }

        public static Boolean IsResizingTop(this WindowResizeGripDirection direction)
            {
            if (direction != WindowResizeGripDirection.Top && direction != WindowResizeGripDirection.TopLeft)
                return direction == WindowResizeGripDirection.TopRight;
            return true;
            }

        public static Boolean IsResizingBottom(this WindowResizeGripDirection direction)
            {
            if (direction != WindowResizeGripDirection.Bottom && direction != WindowResizeGripDirection.BottomLeft)
                return direction == WindowResizeGripDirection.BottomRight;
            return true;
            }

        public static Boolean IsResizingHorizontally(this WindowResizeGripDirection direction)
            {
            if (direction != WindowResizeGripDirection.Top)
                return direction != WindowResizeGripDirection.Bottom;
            return false;
            }

        public static Boolean IsResizingVertically(this WindowResizeGripDirection direction)
            {
            if (direction != WindowResizeGripDirection.Left)
                return direction != WindowResizeGripDirection.Right;
            return false;
            }
        }
    }