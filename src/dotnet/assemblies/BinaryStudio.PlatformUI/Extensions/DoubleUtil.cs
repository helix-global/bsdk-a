using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace BinaryStudio.PlatformUI.Extensions
    {
    internal static class DoubleUtil
        {
        internal const Double DBL_EPSILON = 2.22044604925031E-16; /* smallest such that 1.0+DBL_EPSILON != 1.0 */
        internal const Single FLT_MIN = 1.175494E-38f;            /* Number close to zero, where Single.MinValue is -Single.MaxValue */

        #region M:AreClose(Double,Double):Boolean
        /// <summary>
        /// AreClose - Returns whether or not two doubles are "close".  That is, whether or 
        /// not they are within epsilon of each other.  Note that this epsilon is proportional
        /// to the numbers themselves to that AreClose survives scalar multiplication.
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="x"> The first double to compare. </param>
        /// <param name="y"> The second double to compare. </param>
        public static Boolean AreClose(Double x, Double y) {
            if (x == y) { return true; }
            var ε = (Math.Abs(x) + Math.Abs(y) + 10.0) * DBL_EPSILON;
            var Δ = x - y;
            return (-ε < Δ) && (ε > Δ);
            }
        #endregion
        #region M:LessThan(Double,Double):Boolean
        /// <summary>
        /// LessThan - Returns whether or not the first double is less than the second double.
        /// That is, whether or not the first is strictly less than *and* not within epsilon of
        /// the other number.  Note that this epsilon is proportional to the numbers themselves
        /// to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the LessThan comparision.
        /// </returns>
        /// <param name="x"> The first double to compare. </param>
        /// <param name="y"> The second double to compare. </param>
        public static Boolean LessThan(Double x, Double y) {
            return x < y && !AreClose(x, y);
            }
        #endregion
        #region M:GreaterThan(Double,Double):Boolean
        /// <summary>
        /// GreaterThan - Returns whether or not the first double is greater than the second double.
        /// That is, whether or not the first is strictly greater than *and* not within epsilon of
        /// the other number.  Note that this epsilon is proportional to the numbers themselves
        /// to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the GreaterThan comparision.
        /// </returns>
        /// <param name="x"> The first double to compare. </param>
        /// <param name="y"> The second double to compare. </param>
        public static Boolean GreaterThan(Double x, Double y) {
            return x > y && !AreClose(x, y);
            }
        #endregion
        #region M:LessThanOrClose(Double,Double):Boolean
        /// <summary>
        /// LessThanOrClose - Returns whether or not the first double is less than or close to
        /// the second double.  That is, whether or not the first is strictly less than or within
        /// epsilon of the other number.  Note that this epsilon is proportional to the numbers 
        /// themselves to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the LessThanOrClose comparision.
        /// </returns>
        /// <param name="x"> The first double to compare. </param>
        /// <param name="y"> The second double to compare. </param>
        public static Boolean LessThanOrClose(Double x, Double y) {
            return !(x >= y) || AreClose(x, y);
            }
        #endregion
        #region M:GreaterThanOrClose(Double,Double):Boolean
        /// <summary>
        /// GreaterThanOrClose - Returns whether or not the first double is greater than or close to
        /// the second double.  That is, whether or not the first is strictly greater than or within
        /// epsilon of the other number.  Note that this epsilon is proportional to the numbers 
        /// themselves to that AreClose survives scalar multiplication.  Note,
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.  This is important enough to repeat:
        /// NB: NO CODE CALLING THIS FUNCTION SHOULD DEPEND ON ACCURATE RESULTS - this should be
        /// used for optimizations *only*.
        /// </summary>
        /// <returns>
        /// bool - the result of the GreaterThanOrClose comparision.
        /// </returns>
        /// <param name="x"> The first double to compare. </param>
        /// <param name="y"> The second double to compare. </param>
        public static Boolean GreaterThanOrClose(Double x, Double y) {
            return !(x <= y) || AreClose(x, y);
            }
        #endregion
        #region M:IsOne(Double):Boolean
        /// <summary>
        /// IsOne - Returns whether or not the double is "close" to 1.  Same as AreClose(double, 1),
        /// but this is faster.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value"> The double to compare to 1. </param>
        public static Boolean IsOne(Double value) {
            return Math.Abs(value - 1.0) < 10.0 * DBL_EPSILON;
            }
        #endregion
        #region M:IsZero(Double):boolean
        /// <summary>
        /// IsZero - Returns whether or not the double is "close" to 0.  Same as AreClose(double, 0),
        /// but this is faster.
        /// </summary>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        /// <param name="value"> The double to compare to 0. </param>
        public static Boolean IsZero(Double value) {
            return Math.Abs(value) < 10.0 * DBL_EPSILON;
            }
        #endregion
        #region M:AreClose(Point,Point):Boolean
        /// <summary>
        /// Compares two points for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='x'>The first point to compare</param>
        /// <param name='y'>The second point to compare</param>
        /// <returns>Whether or not the two points are equal</returns>
        public static Boolean AreClose(Point x, Point y) {
            return AreClose(x.X, y.X) &&
                   AreClose(x.Y, y.Y);
            }
        #endregion
        #region M:AreClose(Size,Size):Boolean
        /// <summary>
        /// Compares two Size instances for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='x'>The first size to compare</param>
        /// <param name='y'>The second size to compare</param>
        /// <returns>Whether or not the two Size instances are equal</returns>
        public static Boolean AreClose(Size x, Size y) {
            return AreClose(x.Width, y.Width) &&
                   AreClose(x.Height, y.Height);
            }
        #endregion
        #region M:AreClose(Vector,Vector):Boolean
        /// <summary>
        /// Compares two Vector instances for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='x'>The first Vector to compare</param>
        /// <param name='y'>The second Vector to compare</param>
        /// <returns>Whether or not the two Vector instances are equal</returns>
        public static Boolean AreClose(Vector x, Vector y) {
            return AreClose(x.X, y.X) &&
                   AreClose(x.Y, y.Y);
            }
        #endregion
        #region M:AreClose(Rect,Rect):Boolean
        /// <summary>
        /// Compares two rectangles for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='x'>The first rectangle to compare</param>
        /// <param name='y'>The second rectangle to compare</param>
        /// <returns>Whether or not the two rectangles are equal</returns>
        public static Boolean AreClose(Rect x, Rect y) {
            if (x.IsEmpty) { return y.IsEmpty; }
            return !y.IsEmpty &&
                   (AreClose(x.X, y.X)) &&
                   (AreClose(x.Y, y.Y) && AreClose(x.Height, y.Height)) &&
                   (AreClose(x.Width, y.Width));
            }
        #endregion
        #region M:IsBetweenZeroAndOne(Double):Boolean
        public static Boolean IsBetweenZeroAndOne(Double source) {
            return GreaterThanOrClose(source, 0.0) &&
                   LessThanOrClose(source, 1.0);
            }
        #endregion
        #region M:ToInt32(Double):Int32
        public static Int32 ToInt32(Double source) {
            return (0.0 >= source)
                ? (Int32)(source - 0.5)
                : (Int32)(source + 0.5);
            }
        #endregion
        #region M:ToInt64(Double):Int64
        public static Int64 ToInt64(Double source) {
            return (0.0 >= source)
                ? (Int64)(source - 0.5)
                : (Int64)(source + 0.5);
            }
        #endregion
        #region M:RectHasNaN(Rect):Boolean
        /// <summary>
        /// rectHasNaN - this returns true if this rect has X, Y , Height or Width as NaN.
        /// </summary>
        /// <param name='r'>The rectangle to test</param>
        /// <returns>returns whether the Rect has NaN</returns>        
        public static Boolean RectHasNaN(Rect r) {
            return IsNaN(r.X) ||
                   IsNaN(r.Y) ||
                   (IsNaN(r.Height) || IsNaN(r.Width));
            }
        #endregion
        #region M:IsNaN(Double):Boolean
        // The standard CLR double.IsNaN() function is approximately 100 times slower than this wrapper,
        // so please make sure to use DoubleUtil.IsNaN() in performance sensitive code.
        // PS item that tracks the CLR improvement is DevDiv Schedule : 26916.
        // IEEE 754 : If the argument is any value in the range 0x7ff0000000000001L through 0x7fffffffffffffffL 
        // or in the range 0xfff0000000000001L through 0xffffffffffffffffL, the result will be NaN.         
        public static Boolean IsNaN(Double value) {
            var ρ = new NanUnion {DoubleValue = value};
            var ε = ρ.UIntValue & 0xFFF0000000000000;
            var μ = ρ.UIntValue & 0x000FFFFFFFFFFFFF;
            return ((ε == 0x7FF0000000000000) || (ε == 0xFFF0000000000000)) &&
                   (μ != 0);
            }
        #endregion

        public static Rect Round(Rect source) {
            if (source.IsEmpty) { return source; }
            return new Rect(
                Math.Round(source.Left),
                Math.Round(source.Top),
                Math.Round(source.Width),
                Math.Round(source.Height));
            }

        public static Rect Inflate(Rect source, Double x, Double y)
            {
            var r = source;
            if (r.IsEmpty)
            {
                return new Rect(new Size(x, y));
            }
            r.Inflate(x, y);
            return r;
            }

        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion {
            [FieldOffset(0)] internal Double DoubleValue;
            [FieldOffset(0)] internal readonly UInt64 UIntValue;
            }
        }
    }