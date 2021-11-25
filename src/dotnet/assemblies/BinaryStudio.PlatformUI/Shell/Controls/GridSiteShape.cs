using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class GridSiteShape : Shape
        {
        #region P:Site:Grid
        public static readonly DependencyProperty SiteProperty = DependencyProperty.Register("Site", typeof(Grid), typeof(GridSiteShape), new PropertyMetadata(default(Grid), OnSiteChanged));
        private static void OnSiteChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as GridSiteShape);
            if (source != null) {
                source.OnSiteChanged((Grid)e.NewValue, (Grid)e.OldValue);
                }
            }

        private void OnSiteChanged(Grid n, Grid o) {
            if (o != null) { o.LayoutUpdated -= OnLayoutUpdated; }
            if (n != null) { n.LayoutUpdated += OnLayoutUpdated; }
            BuildGeometry();
            }

        public Grid Site
            {
            get { return (Grid)GetValue(SiteProperty); }
            set { SetValue(SiteProperty, value); }
            }
        #endregion

        protected override Geometry DefiningGeometry { get { return geometry; }}

        private void OnLayoutUpdated(Object sender, EventArgs e)
            {
            BuildGeometry();
            }

        protected override Size MeasureOverride(Size constraint)
            {
            if ((geometry != null) && (!geometry.IsEmpty())) {
                var r = new Size(geometry.Bounds.Width, geometry.Bounds.Height);
                return r;
                }
            return base.MeasureOverride(constraint);
            }

        protected override Size ArrangeOverride(Size finalSize)
            {
            return base.ArrangeOverride(finalSize);
            }

        protected override void OnRender(DrawingContext context)
            {
            context.DrawGeometry(Fill, null, geometry);
            }

        private void BuildGeometry() {
            var site = Site;
            if (site != null) {
                var client = new Rect(0, 0, site.ActualWidth, site.ActualHeight);
                if ((client.IsEmpty) || (client.Width == 0) || (client.Height == 0)) {
                    geometry = Geometry.Empty;
                    return;
                    }

                var rchs = new HashSet<Rect>();
                var colc = Math.Max(site.ColumnDefinitions.Count, 1);
                var rowc = Math.Max(site.RowDefinitions.Count, 1);

                #region rows
                var rows = new Double[rowc];
                if (site.RowDefinitions.Count == 0) { rows[0] = client.Height; }
                else
                    {
                    for (var i = 0; i < rowc; i++) {
                        rows[i] = site.RowDefinitions[i].ActualHeight;
                        }
                    }
                #endregion
                #region cols
                var cols = new Double[colc];
                if (site.ColumnDefinitions.Count == 0) { cols[0] = client.Width; }
                else
                    {
                    for (var i = 0; i < colc; i++) {
                        cols[i] = site.ColumnDefinitions[i].ActualWidth;
                        }
                    }
                #endregion
                #region cells
                var cells = new Rect[rowc,colc];
                var y = 0.0;
                for (var row = 0; row < rowc; row++) {
                    var x = 0.0;
                    for (var col = 0; col < colc; col++) {
                        cells[row,col] = new Rect(x, y, cols[col], rows[row]);
                        x += cols[col];
                        }
                    y += rows[row];
                    }
                #endregion

                var M = new Byte[rowc, colc];

                foreach (var e in site.Children.OfType<UIElement>()) {
                    if (e.Visibility == Visibility.Visible) {
                        var cole = Grid.GetColumn(e);
                        var rowe = Grid.GetRow(e);
                        var colspan = Grid.GetColumnSpan(e);
                        var rowspan = Grid.GetRowSpan(e);
                        if ((cole < colc) && (colspan > 0)) {
                            colspan = Math.Min(colspan, colc - cole);
                            if ((rowe < rowc) && (rowspan > 0)) {
                                rowspan = Math.Min(rowspan, rowc - rowe);
                                for (var i = 0; i < rowspan; i++) {
                                    for (var j = 0; j < colspan; j++) {
                                        M[rowe + i, cole + j] = 1;
                                        }
                                    }
                                var lefttop = cells[rowe, cole].TopLeft;
                                var rightbt = cells[rowe + rowspan - 1, cole + colspan - 1].BottomRight;
                                rchs.Add(new Rect(lefttop, rightbt));
                                }
                            }
                        }
                    }
                if (rchs.Count > 0) {
                    var i = 0;
                    var j = 0;
                    var flag = false;
                    var points = new LinkedList<Point>();
                    Print(M, rowc, colc);
                    for (; (i < rowc); i++) {
                        for (j = 0;j < colc; j++) {
                            if (M[i,j] == 1) {
                                flag = true;
                                break;
                                }
                            }
                        if (flag) { break; }
                        }
                    if (flag) {
                        M[i, j] = 0xFF;
                        points.AddLast(cells[i,j].TopLeft);
                        }
                    for (;;)
                        {
                        Print(M, rowc, colc);
                        var r = NextItem(M, rowc, colc, ref i, ref j);
                        if (r.Value == 0) {
                            Print(M, rowc, colc);
                            break;
                            }
                        if (r.Value != 0xFF) { M[i, j]++; continue; }
                        Print(M, rowc, colc);
                        break;
                        }

                    //var groups = new List<RectangularPolygon>();
                    //while (rchs.Count > 0) {
                    //    var rc = rchs.First();
                    //    rchs.Remove(rc);
                    //    var flag = false;
                    //    for (var i = 0; i < groups.Count; i++) {
                    //        if (groups[i].IntersectsWith(rc)) {
                    //            groups[i].Merge(rc);
                    //            flag = true;
                    //            break;
                    //            }
                    //        }
                    //    if (!flag) {
                    //        groups.Add(new RectangularPolygon(rc));
                    //        }
                    //    }
                    //if (groups.Count > 0) {
                    //    var g = Geometry.Empty;
                    //    for (var i = 0; i < groups.Count; i++) {
                    //        g = Geometry.Combine(g, BuildGeometry(groups[i]), GeometryCombineMode.Union, Transform.Identity);
                    //        }
                    //    geometry = g;
                    //    }
                    }
                }
            else
                {
                geometry = Geometry.Empty;
                }
            InvalidateMeasure();
            InvalidateVisual();
            }

        private enum Direction
            {
            None,
            N,E,S,W,
            NE,SE,SW,NW
            }

        private static void Print(Byte[,] M, Int32 rowc, Int32 colc) {
            var r = new StringBuilder();
            r.AppendLine("-------------------");
            for (var i = 0; i < rowc; i++) {
                for (var j = 0; j < colc; j++) {
                    r.Append(M[i,j]);
                    r.Append(' ');
                    }
                r.AppendLine();
                }
            Debug.Print(r.ToString());
            }

        private class DirectionPoint
            {
            public Direction Direction { get; }
            public Byte Value { get; }
            public DirectionPoint(Byte value, Direction direction) {
                Value = value;
                Direction = direction;
                }

            public override String ToString()
                {
                return $"{Direction}:{Value}";
                }
            }

        private static DirectionPoint NextItem(Byte[,] M, Int32 rowc, Int32 colc, ref Int32 row, ref Int32 col) {
            var r =  WItem(M, rowc, colc, ref row, ref col); if (r != 0) { return new DirectionPoint(r, Direction.W);  }
                r = SWItem(M, rowc, colc, ref row, ref col); if (r != 0) { return new DirectionPoint(r, Direction.SW); }
                r =  SItem(M, rowc, colc, ref row, ref col); if (r != 0) { return new DirectionPoint(r, Direction.S);  }
                r = SEItem(M, rowc, colc, ref row, ref col); if (r != 0) { return new DirectionPoint(r, Direction.SE); }
                r =  EItem(M, rowc, colc, ref row, ref col); if (r != 0) { return new DirectionPoint(r, Direction.E);  }
                r = NEItem(M, rowc, colc, ref row, ref col); if (r != 0) { return new DirectionPoint(r, Direction.NE); }
                r =  NItem(M, rowc, colc, ref row, ref col); if (r != 0) { return new DirectionPoint(r, Direction.N);  }
                r = NWItem(M, rowc, colc, ref row, ref col); if (r != 0) { return new DirectionPoint(r, Direction.NW); }
            return new DirectionPoint(0, Direction.None);
            }

        private static Byte WItem(Byte[,] M, Int32 rowc, Int32 colc, ref Int32 row, ref Int32 col) {
            if (col > 0) {
                col--;
                var r = M[row, col];
                if (r == 0) {
                    col++;
                    return 0;
                    }
                return r;
                }
            return 0;
            }

        private static Byte SWItem(Byte[,] M, Int32 rowc, Int32 colc, ref Int32 row, ref Int32 col) {
            if ((col > 0) && (row < rowc - 1)) {
                col--;
                row++;
                var r = M[row, col];
                if (r == 0) {
                    col++;
                    row--;
                    return 0;
                    }
                return r;
                }
            return 0;
            }

        private static Byte NItem(Byte[,] M, Int32 rowc, Int32 colc, ref Int32 row, ref Int32 col) {
            if (row > 0) {
                row--;
                var r = M[row, col];
                if (r == 0) {
                    row++;
                    return 0;
                    }
                return r;
                }
            return 0;
            }

        private static Byte NEItem(Byte[,] M, Int32 rowc, Int32 colc, ref Int32 row, ref Int32 col) {
            if ((row > 0) && (col < colc - 1)) {
                row--;
                col++;
                var r = M[row, col];
                if (r == 0) {
                    row++;
                    col--;
                    return 0;
                    }
                return r;
                }
            return 0;
            }

        private static Byte NWItem(Byte[,] M, Int32 rowc, Int32 colc, ref Int32 row, ref Int32 col) {
            if ((row > 0) && (col > 0)) {
                row--;
                col--;
                var r = M[row, col];
                if (r == 0) {
                    row++;
                    col++;
                    return 0;
                    }
                return r;
                }
            return 0;
            }

        private static Byte SItem(Byte[,] M, Int32 rowc, Int32 colc, ref Int32 row, ref Int32 col) {
            if (row < rowc - 1) {
                row++;
                var r = M[row, col];
                if (r == 0) {
                    row--;
                    return 0;
                    }
                return r;
                }
            return 0;
            }

        private static Byte SEItem(Byte[,] M, Int32 rowc, Int32 colc, ref Int32 row, ref Int32 col) {
            if ((row < rowc - 1) && (col < colc - 1)) {
                row++;
                col++;
                var r = M[row, col];
                if (r == 0) {
                    row--;
                    col--;
                    return 0;
                    }
                return r;
                }
            return 0;
            }

        private static Byte EItem(Byte[,] M, Int32 rowc, Int32 colc, ref Int32 row, ref Int32 col) {
            if (col < colc - 1) {
                col++;
                var r = M[row, col];
                if (r == 0) {
                    col--;
                    return 0;
                    }
                return r;
                }
            return 0;
            }

        private enum PointType
            {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
            }

        #region M:AddPoint(ICollection<Tuple<LinkedListNode<Point>,PointType>>,LinkedListNode<Point>,PointType)
        private static void AddPoint(ICollection<Tuple<LinkedListNode<Point>, PointType>> target, LinkedListNode<Point> point, PointType type) {
            if (point != null) {
                target.Add(Tuple.Create(point, type));
                }
            }
        #endregion
        #region M:Contains(LinkedList<Point>,Rect):IList<Tuple<LinkedListNode<Point>,PointType>>
        private static IList<Tuple<LinkedListNode<Point>, PointType>> Contains(LinkedList<Point> points, Rect rc) {
            var r = new List<Tuple<LinkedListNode<Point>,PointType>>();
            AddPoint(r, Contains(points, rc.TopLeft),     PointType.TopLeft);
            AddPoint(r, Contains(points, rc.TopRight),    PointType.TopRight);
            AddPoint(r, Contains(points, rc.BottomLeft),  PointType.BottomLeft);
            AddPoint(r, Contains(points, rc.BottomRight), PointType.BottomRight);
            return r;
            }
        #endregion
        #region M:Contains(LinkedList<Point>,Point):LinkedListNode<Point>
        private static LinkedListNode<Point> Contains(LinkedList<Point> points, Point pt) {
            for (var i = points.First; i != null; i = i.Next) {
                if (pt == i.Value) {
                    return i;
                    }
                }
            return null;
            }
        #endregion

        private static Geometry BuildGeometry(IList<Point> values) {
            if (values.Count > 1) {
                var segments = new List<PathSegment>();
                for (var i = 1; i < values.Count; i++) {
                    segments.Add(new LineSegment(values[i], false));
                    }
                return new PathGeometry(new []{ new PathFigure(values[0], segments, true) });
                }
            return Geometry.Empty;
            }

        private class Comparer : IComparer<Rect>
            {
            public Int32 Compare(Rect x, Rect y) {
                if (x == y) { return 0; }
                var r = x.Left.CompareTo(y.Left);
                if (r == 0) {
                    r = x.Top.CompareTo(y.Top);
                    if (r == 0) {
                        r = x.Right.CompareTo(y.Right);
                        if (r == 0) {
                            r = x.Bottom.CompareTo(y.Bottom);
                            }
                        }
                    }
                return r;
                }
            }

        private Geometry geometry = Geometry.Empty;
        }
    }