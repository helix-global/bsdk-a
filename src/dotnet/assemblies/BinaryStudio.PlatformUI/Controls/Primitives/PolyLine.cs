using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    public class PolyLine : Shape
        {
        #region P:Points:PointCollection
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(PointCollection), typeof(PolyLine), new PropertyMetadata(default(PointCollection), OnPointsChanged));
        private static void OnPointsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as PolyLine);
            if (source != null)
                {
                source.OnPointsChanged();
                }
            }

        private void OnPointsChanged()
            {
            UpdateGeometry();
            InvalidateVisual();
            }

        public PointCollection Points
            {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
            }
        #endregion
        #region P:FillRule:FillRule
        public static readonly DependencyProperty FillRuleProperty = DependencyProperty.Register("FillRule", typeof(FillRule), typeof(PolyLine), new PropertyMetadata(default(FillRule), OnFillRuleChanged));
        private static void OnFillRuleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as PolyLine);
            if (source != null)
                {
                source.OnFillRuleChanged();
                }
            }

        private void OnFillRuleChanged()
            {
            UpdateGeometry();
            }

        public FillRule FillRule
            {
            get { return (FillRule)GetValue(FillRuleProperty); }
            set { SetValue(FillRuleProperty, value); }
            }
        #endregion

        private void UpdateGeometry() {
            var points = Points;
            var pathfigure = new PathFigure();
            if (points == null)
                {
                geometry = Geometry.Empty;
                return;
                }
            if (points.Count > 0)
                {
                pathfigure.StartPoint = points[0];
                if (points.Count > 1)
                    {
                    var array = new Point[points.Count - 1];
                    for (var i = 1; i < points.Count; i++)
                        {
                        array[i - 1] = points[i];
                        }
                    pathfigure.Segments.Add(new PolyLineSegment(array, true));
                    }
                }
            var pathgeometry = new PathGeometry();
            pathgeometry.Figures.Add(pathfigure);
            pathgeometry.FillRule = FillRule;
            if (pathgeometry.Bounds == Rect.Empty)
                {
                geometry = Geometry.Empty;
                return;
                }
            geometry = pathgeometry;
            }

        #region P:DefiningGeometry:Geometry
        private Geometry geometry;
        /// <summary>Gets a value that represents the <see cref="T:System.Windows.Media.Geometry" /> of the <see cref="T:System.Windows.Shapes.Shape" />.</summary>
        /// <returns>The <see cref="T:System.Windows.Media.Geometry" /> of the <see cref="T:System.Windows.Shapes.Shape" />.</returns>
        protected override Geometry DefiningGeometry
            {
            get { return geometry; }
            }
        #endregion

        internal Boolean IsPenNoOp { get {
            var strokeThickness = StrokeThickness;
            return Stroke == null || DoubleUtil.IsNaN(strokeThickness) || DoubleUtil.IsZero(strokeThickness);
            }}

        internal Pen EnsurePen()
            {
            if (IsPenNoOp) { return null; }
            if (pen == null)
                {
                var strokethickness = StrokeThickness;
                var thickness = Math.Abs(strokethickness);
                pen = new Pen
                    {
                    Thickness = thickness,
                    Brush = Stroke,
                    StartLineCap = StrokeStartLineCap,
                    EndLineCap = StrokeEndLineCap,
                    DashCap = StrokeDashCap,
                    LineJoin = StrokeLineJoin,
                    MiterLimit = StrokeMiterLimit
                    };
                var doublecollection = StrokeDashArray;
                var strokedashoffset = StrokeDashOffset;
                if (doublecollection != null || strokedashoffset != 0.0)
                    {
                    pen.DashStyle = new DashStyle(doublecollection, strokedashoffset);
                    }
                }
            return pen;
            }

        /// <summary>Provides a means to change the default appearance of a <see cref="T:System.Windows.Shapes.Shape" /> element.</summary>
        /// <param name="context">A <see cref="T:System.Windows.Media.DrawingContext" /> object that is drawn during the rendering pass of this <see cref="T:System.Windows.Shapes.Shape" />.</param>
        protected override void OnRender(DrawingContext context)
            {
            if (Stretch == Stretch.None) {
                var g = DefiningGeometry;
                if ((g != null) && (!g.IsEmpty())) {
                    context.PushGuidelineSet(new GuidelineSet(
                        new []{0.1, 0.1, 0.5},
                        new []{0.1, 0.1, 0.5}));
                    context.DrawGeometry(Fill, EnsurePen(), g);
                    context.Pop();
                    }
                return;
                }
            base.OnRender(context);
            }

        private Pen pen;
        }
    }