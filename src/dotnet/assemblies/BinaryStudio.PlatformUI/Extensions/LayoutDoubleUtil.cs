using System;
using System.Windows;

namespace BinaryStudio.PlatformUI
    {
    internal static class LayoutDoubleUtil
        {
        private const Double DBL_EPSILON = 1.53E-06;

        #region M:AreClose(Double,Double):Boolean
        public static Boolean AreClose(Double x, Double y) {
            if (x.IsNonreal() || y.IsNonreal()) { return x.CompareTo(y) == 0; }
            if (x == y) { return true; }
            var Δ = x - y;
            return (Δ < DBL_EPSILON) && (Δ > -DBL_EPSILON);
        }
        #endregion
        #region M:AreClose(Rect,Rect):Boolean
        public static Boolean AreClose(Rect x, Rect y) {
            return x.Location.AreClose(y.Location) && x.Size.AreClose(y.Size);
            }
        #endregion
        #region M:AreClose(Size,Size):Boolean
        public static Boolean AreClose(this Size x, Size y) {
            return AreClose(x.Width, y.Width) && AreClose(x.Height, y.Height);
            }
        #endregion
        #region M:AreClose(Point,Point):Boolean
        public static Boolean AreClose(this Point x, Point y) {
            return AreClose(x.X, y.X) && AreClose(x.Y, y.Y);
            }
        #endregion
        #region M:LessThan(Double,Double):Boolean
        public static Boolean LessThan(Double x, Double y) {
            return (x < y) && !AreClose(x, y);
            }
        #endregion
        #region M:LessThanOrClose(Double,Double):Boolean
        public static Boolean LessThanOrClose(Double x, Double y) {
            return !(x >= y) || AreClose(x, y);
            }
        #endregion
        #region M:GreaterThan(Double,Doubel):Boolean
        public static Boolean GreaterThan(Double x, Double y) {
            return (x > y) && !AreClose(x, y);
            }
        #endregion
        #region M:GreaterThanOrClose(Double,Double):Boolean
        public static Boolean GreaterThanOrClose(Double x, Double y) {
            return !(x <= y) || AreClose(x, y);
            }
        #endregion

        public static Boolean IsNonreal(this Double value) {
            if (!Double.IsNaN(value))
                return Double.IsInfinity(value);
            return true;
            }
        }
    }