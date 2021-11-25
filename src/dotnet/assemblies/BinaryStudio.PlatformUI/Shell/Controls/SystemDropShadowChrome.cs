using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class SystemDropShadowChrome : Decorator
        {
        private const Double ShadowDepth = 5.0;
        private const Int32 TopLeft     = 0;
        private const Int32 Top         = 1;
        private const Int32 TopRight    = 2;
        private const Int32 Left        = 3;
        private const Int32 Center      = 4;
        private const Int32 Right       = 5;
        private const Int32 BottomLeft  = 6;
        private const Int32 Bottom      = 7;
        private const Int32 BottomRight = 8;

        private static Brush[] _commonBrushes;
        private static CornerRadius _commonCornerRadius;
        private static readonly Object _resourceAccess = new Object();

        private Brush[] _brushes;

        #region P:Color:Color
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(SystemDropShadowChrome), new FrameworkPropertyMetadata(Color.FromArgb(113, 0, 0, 0), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(ClearBrushes)));
        public Color Color
            {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
            }
        private static void ClearBrushes(DependencyObject o, DependencyPropertyChangedEventArgs e)
            {
            ((SystemDropShadowChrome)o)._brushes = null;
            }
        #endregion
        #region P:CornerRadius:CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(SystemDropShadowChrome), new FrameworkPropertyMetadata(default(CornerRadius), FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(ClearBrushes)), new ValidateValueCallback(IsCornerRadiusValid));
        public CornerRadius CornerRadius
            {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
            }
        private static Boolean IsCornerRadiusValid(Object value)
            {
            var cornerRadius = (CornerRadius)value;
            return cornerRadius.TopLeft >= 0.0 && cornerRadius.TopRight >= 0.0 && cornerRadius.BottomLeft >= 0.0 && cornerRadius.BottomRight >= 0.0 && !Double.IsNaN(cornerRadius.TopLeft) && !Double.IsNaN(cornerRadius.TopRight) && !Double.IsNaN(cornerRadius.BottomLeft) && !Double.IsNaN(cornerRadius.BottomRight) && !Double.IsInfinity(cornerRadius.TopLeft) && !Double.IsInfinity(cornerRadius.TopRight) && !Double.IsInfinity(cornerRadius.BottomLeft) && !Double.IsInfinity(cornerRadius.BottomRight);
            }
        #endregion

        protected override void OnRender(DrawingContext drawingContext)
            {
            var cornerRadius = CornerRadius;
            var rect = new Rect(new Point(ShadowDepth, ShadowDepth), new Size(RenderSize.Width, RenderSize.Height));
            var color = Color;
            if (rect.Width > 0.0 && rect.Height > 0.0 && color.A > 0)
                {
                var num = rect.Right - rect.Left - 10.0;
                var num2 = rect.Bottom - rect.Top - 10.0;
                var val = Math.Min(num * 0.5, num2 * 0.5);
                cornerRadius.TopLeft = Math.Min(cornerRadius.TopLeft, val);
                cornerRadius.TopRight = Math.Min(cornerRadius.TopRight, val);
                cornerRadius.BottomLeft = Math.Min(cornerRadius.BottomLeft, val);
                cornerRadius.BottomRight = Math.Min(cornerRadius.BottomRight, val);
                var brushes = GetBrushes(color, cornerRadius);
                var num3 = rect.Top  + ShadowDepth;
                var num4 = rect.Left + ShadowDepth;
                var num5 = rect.Right  - ShadowDepth;
                var num6 = rect.Bottom - ShadowDepth;
                var array = new[]
                    {
                    num4,
                    num4 + cornerRadius.TopLeft,
                    num5 - cornerRadius.TopRight,
                    num4 + cornerRadius.BottomLeft,
                    num5 - cornerRadius.BottomRight,
                    num5
                    };
                var array2 = new[]
                    {
                    num3,
                    num3 + cornerRadius.TopLeft,
                    num3 + cornerRadius.TopRight,
                    num6 - cornerRadius.BottomLeft,
                    num6 - cornerRadius.BottomRight,
                    num6
                    };
                drawingContext.PushGuidelineSet(new GuidelineSet(array, array2));
                cornerRadius.TopLeft  += ShadowDepth;
                cornerRadius.TopRight += ShadowDepth;
                cornerRadius.BottomLeft  += ShadowDepth;
                cornerRadius.BottomRight += ShadowDepth;
                var rectangle = new Rect(rect.Left, rect.Top, cornerRadius.TopLeft, cornerRadius.TopLeft);
                drawingContext.DrawRectangle(brushes[0], null, rectangle);
                var num7 = array[2] - array[1];
                if (num7 > 0.0)
                    {
                    var rectangle2 = new Rect(array[1], rect.Top, num7, ShadowDepth);
                    drawingContext.DrawRectangle(brushes[1], null, rectangle2);
                    }
                var rectangle3 = new Rect(array[2], rect.Top, cornerRadius.TopRight, cornerRadius.TopRight);
                drawingContext.DrawRectangle(brushes[2], null, rectangle3);
                var num8 = array2[3] - array2[1];
                if (num8 > 0.0)
                    {
                    var rectangle4 = new Rect(rect.Left, array2[1], ShadowDepth, num8);
                    drawingContext.DrawRectangle(brushes[3], null, rectangle4);
                    }
                var num9 = array2[4] - array2[2];
                if (num9 > 0.0)
                    {
                    var rectangle5 = new Rect(array[5], array2[2], ShadowDepth, num9);
                    drawingContext.DrawRectangle(brushes[5], null, rectangle5);
                    }
                var rectangle6 = new Rect(rect.Left, array2[3], cornerRadius.BottomLeft, cornerRadius.BottomLeft);
                drawingContext.DrawRectangle(brushes[6], null, rectangle6);
                var num10 = array[4] - array[3];
                if (num10 > 0.0)
                    {
                    var rectangle7 = new Rect(array[3], array2[5], num10, ShadowDepth);
                    drawingContext.DrawRectangle(brushes[7], null, rectangle7);
                    }
                var rectangle8 = new Rect(array[4], array2[4], cornerRadius.BottomRight, cornerRadius.BottomRight);
                drawingContext.DrawRectangle(brushes[8], null, rectangle8);
                if (cornerRadius.TopLeft == ShadowDepth && cornerRadius.TopLeft == cornerRadius.TopRight && cornerRadius.TopLeft == cornerRadius.BottomLeft && cornerRadius.TopLeft == cornerRadius.BottomRight)
                    {
                    var rectangle9 = new Rect(array[0], array2[0], num, num2);
                    drawingContext.DrawRectangle(brushes[4], null, rectangle9);
                    }
                else
                    {
                    var pathFigure = new PathFigure();
                    if (cornerRadius.TopLeft > ShadowDepth)
                        {
                        pathFigure.StartPoint = new Point(array[1], array2[0]);
                        pathFigure.Segments.Add(new LineSegment(new Point(array[1], array2[1]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[0], array2[1]), true));
                        }
                    else
                        {
                        pathFigure.StartPoint = new Point(array[0], array2[0]);
                        }
                    if (cornerRadius.BottomLeft > ShadowDepth)
                        {
                        pathFigure.Segments.Add(new LineSegment(new Point(array[0], array2[3]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[3], array2[3]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[3], array2[5]), true));
                        }
                    else
                        {
                        pathFigure.Segments.Add(new LineSegment(new Point(array[0], array2[5]), true));
                        }
                    if (cornerRadius.BottomRight > ShadowDepth)
                        {
                        pathFigure.Segments.Add(new LineSegment(new Point(array[4], array2[5]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[4], array2[4]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[5], array2[4]), true));
                        }
                    else
                        {
                        pathFigure.Segments.Add(new LineSegment(new Point(array[5], array2[5]), true));
                        }
                    if (cornerRadius.TopRight > ShadowDepth)
                        {
                        pathFigure.Segments.Add(new LineSegment(new Point(array[5], array2[2]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[2], array2[2]), true));
                        pathFigure.Segments.Add(new LineSegment(new Point(array[2], array2[0]), true));
                        }
                    else
                        {
                        pathFigure.Segments.Add(new LineSegment(new Point(array[5], array2[0]), true));
                        }
                    pathFigure.IsClosed = true;
                    pathFigure.Freeze();
                    var pathGeometry = new PathGeometry();
                    pathGeometry.Figures.Add(pathFigure);
                    pathGeometry.Freeze();
                    drawingContext.DrawGeometry(brushes[4], null, pathGeometry);
                    }
                drawingContext.Pop();
                }
            }

        private static GradientStopCollection CreateStops(Color c, Double cornerRadius)
            {
            var n = 1.0 / (cornerRadius + ShadowDepth);
            var r = new GradientStopCollection {new GradientStop(c, (0.5 + cornerRadius) * n)};
            var color = c;
            color.A = (Byte)(0.74336 * c.A);
            r.Add(new GradientStop(color, (1.5 + cornerRadius) * n));
            color.A = (Byte)(0.38053 * c.A);
            r.Add(new GradientStop(color, (2.5 + cornerRadius) * n));
            color.A = (Byte)(0.12389 * c.A);
            r.Add(new GradientStop(color, (3.5 + cornerRadius) * n));
            color.A = (Byte)(0.02654 * c.A);
            r.Add(new GradientStop(color, (4.5 + cornerRadius) * n));
            color.A = 0;
            r.Add(new GradientStop(color, (5.0 + cornerRadius) * n));
            r.Freeze();
            return r;
            }

        private static Brush[] CreateBrushes(Color c, CornerRadius cornerRadius)
            {
            var r = new Brush[9];
            r[Center] = new SolidColorBrush(c);
            r[Center].Freeze();
            var gradientStopCollection = CreateStops(c, 0.0);
            var linearGradientBrush = new LinearGradientBrush(gradientStopCollection, new Point(0.0, 1.0), new Point(0.0, 0.0));
            linearGradientBrush.Freeze();
            r[Top] = linearGradientBrush;
            var linearGradientBrush2 = new LinearGradientBrush(gradientStopCollection, new Point(1.0, 0.0), new Point(0.0, 0.0));
            linearGradientBrush2.Freeze();
            r[Left] = linearGradientBrush2;
            var linearGradientBrush3 = new LinearGradientBrush(gradientStopCollection, new Point(0.0, 0.0), new Point(1.0, 0.0));
            linearGradientBrush3.Freeze();
            r[Right] = linearGradientBrush3;
            var linearGradientBrush4 = new LinearGradientBrush(gradientStopCollection, new Point(0.0, 0.0), new Point(0.0, 1.0));
            linearGradientBrush4.Freeze();
            r[Bottom] = linearGradientBrush4;
            GradientStopCollection gradientStopCollection2;
            if (cornerRadius.TopLeft == 0.0)
                {
                gradientStopCollection2 = gradientStopCollection;
                }
            else
                {
                gradientStopCollection2 = CreateStops(c, cornerRadius.TopLeft);
                }
            var radialGradientBrush = new RadialGradientBrush(gradientStopCollection2);
            radialGradientBrush.RadiusX = 1.0;
            radialGradientBrush.RadiusY = 1.0;
            radialGradientBrush.Center = new Point(1.0, 1.0);
            radialGradientBrush.GradientOrigin = new Point(1.0, 1.0);
            radialGradientBrush.Freeze();
            r[TopLeft] = radialGradientBrush;
            GradientStopCollection gradientStopCollection3;
            if (cornerRadius.TopRight == 0.0)
                {
                gradientStopCollection3 = gradientStopCollection;
                }
            else if (cornerRadius.TopRight == cornerRadius.TopLeft)
                {
                gradientStopCollection3 = gradientStopCollection2;
                }
            else
                {
                gradientStopCollection3 = CreateStops(c, cornerRadius.TopRight);
                }
            var radialGradientBrush2 = new RadialGradientBrush(gradientStopCollection3);
            radialGradientBrush2.RadiusX = 1.0;
            radialGradientBrush2.RadiusY = 1.0;
            radialGradientBrush2.Center = new Point(0.0, 1.0);
            radialGradientBrush2.GradientOrigin = new Point(0.0, 1.0);
            radialGradientBrush2.Freeze();
            r[TopRight] = radialGradientBrush2;
            GradientStopCollection gradientStopCollection4;
            if (cornerRadius.BottomLeft == 0.0)
                {
                gradientStopCollection4 = gradientStopCollection;
                }
            else if (cornerRadius.BottomLeft == cornerRadius.TopLeft)
                {
                gradientStopCollection4 = gradientStopCollection2;
                }
            else if (cornerRadius.BottomLeft == cornerRadius.TopRight)
                {
                gradientStopCollection4 = gradientStopCollection3;
                }
            else
                {
                gradientStopCollection4 = CreateStops(c, cornerRadius.BottomLeft);
                }
            var radialGradientBrush3 = new RadialGradientBrush(gradientStopCollection4);
            radialGradientBrush3.RadiusX = 1.0;
            radialGradientBrush3.RadiusY = 1.0;
            radialGradientBrush3.Center = new Point(1.0, 0.0);
            radialGradientBrush3.GradientOrigin = new Point(1.0, 0.0);
            radialGradientBrush3.Freeze();
            r[BottomLeft] = radialGradientBrush3;
            GradientStopCollection gradientStopCollection5;
            if (cornerRadius.BottomRight == 0.0)
                {
                gradientStopCollection5 = gradientStopCollection;
                }
            else if (cornerRadius.BottomRight == cornerRadius.TopLeft)
                {
                gradientStopCollection5 = gradientStopCollection2;
                }
            else if (cornerRadius.BottomRight == cornerRadius.TopRight)
                {
                gradientStopCollection5 = gradientStopCollection3;
                }
            else if (cornerRadius.BottomRight == cornerRadius.BottomLeft)
                {
                gradientStopCollection5 = gradientStopCollection4;
                }
            else
                {
                gradientStopCollection5 = CreateStops(c, cornerRadius.BottomRight);
                }
            var radialGradientBrush4 = new RadialGradientBrush(gradientStopCollection5);
            radialGradientBrush4.RadiusX = 1.0;
            radialGradientBrush4.RadiusY = 1.0;
            radialGradientBrush4.Center = new Point(0.0, 0.0);
            radialGradientBrush4.GradientOrigin = new Point(0.0, 0.0);
            radialGradientBrush4.Freeze();
            r[BottomRight] = radialGradientBrush4;
            return r;
            }

        private Brush[] GetBrushes(Color c, CornerRadius cornerRadius)
            {
            if (_commonBrushes == null)
                {
                var resourceAccess = _resourceAccess;
                lock (resourceAccess)
                    {
                    if (_commonBrushes == null)
                        {
                        _commonBrushes = CreateBrushes(c, cornerRadius);
                        _commonCornerRadius = cornerRadius;
                        }
                    }
                }
            if (c == ((SolidColorBrush)_commonBrushes[4]).Color && cornerRadius == _commonCornerRadius)
                {
                _brushes = null;
                return _commonBrushes;
                }
            if (_brushes == null)
                {
                _brushes = CreateBrushes(c, cornerRadius);
                }
            return _brushes;
            }
        }
    }
