using System;
using System.Windows;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Controls.Markups
    {
    public class MarkupElement : DependencyObject
        {
        #region P:FirstIndex:Int64
        public static readonly DependencyProperty FirstIndexProperty = DependencyProperty.Register("FirstIndex", typeof(Int64), typeof(MarkupElement), new PropertyMetadata(default(Int64)));
        public Int64 FirstIndex {
            get { return (Int64)GetValue(FirstIndexProperty); }
            set { SetValue(FirstIndexProperty, value); }
            }
        #endregion
        #region P:LastIndex:Int64
        public static readonly DependencyProperty LastIndexProperty = DependencyProperty.Register("LastIndex", typeof(Int64), typeof(MarkupElement), new PropertyMetadata(default(Int64)));
        public Int64 LastIndex {
            get { return (Int64)GetValue(LastIndexProperty); }
            set { SetValue(LastIndexProperty, value); }
            }
        #endregion
        #region P:Color:Color
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Color), typeof(MarkupElement), new PropertyMetadata(default(Color)));
        public Color Color {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
            }
        #endregion
        #region P:Content:Object
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(Object), typeof(MarkupElement), new PropertyMetadata(default(Object)));
        public Object Content {
            get { return (Object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
            }
        #endregion
        #region P:IsSelected:Boolean
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(Boolean), typeof(MarkupElement), new PropertyMetadata(default(Boolean)));
        public Boolean IsSelected
            {
            get { return (Boolean)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
            }
        #endregion
        //#region P:EndPoint:Double
        //public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register("EndPoint", typeof(Double), typeof(MarkupElement), new PropertyMetadata(default(Double)));
        //public Double EndPoint
        //    {
        //    get { return (Double)GetValue(EndPointProperty); }
        //    set { SetValue(EndPointProperty, value); }
        //    }
        //#endregion
        #region P:BeginPoint:Double
        public static readonly DependencyProperty BeginPointProperty = DependencyProperty.Register("BeginPoint", typeof(Double), typeof(MarkupElement), new PropertyMetadata(default(Double)));
        public Double BeginPoint
            {
            get { return (Double)GetValue(BeginPointProperty); }
            set { SetValue(BeginPointProperty, value); }
            }
        #endregion
        #region P:Points:PointCollection
        public static readonly DependencyProperty PointsProperty = DependencyProperty.Register("Points", typeof(PointCollection), typeof(MarkupElement), new PropertyMetadata(default(PointCollection)));
        public PointCollection Points
            {
            get { return (PointCollection)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
            }
        #endregion

        public MarkupElement()
            {
            }

        public MarkupElement(Int64 firstindex, Int64 lastindex, Color color)
            :this()
            {
            FirstIndex = firstindex;
            LastIndex = lastindex;
            Color = color;
            }

        public MarkupElement(Int64 firstindex, Int64 lastindex, Color color, Object content)
            :this(firstindex, lastindex, color)
            {
            Content = content;
            }

        public void Update(Double? x, Double? y, Double? z)
            {
            this.x = x ?? this.x;
            this.y = y ?? this.y;
            this.z = z ?? this.z;
            //var r = new PointCollection {
            //    new Point(0, this.x),
            //    new Point(10, this.x),
            //    new Point(20, this.z),
            //    new Point(30, this.z)
            //    };
            var r = new PointCollection {
                new Point(0, this.x),
                new Point(this.y, this.x),
                new Point(this.y, this.z),
                new Point(30, this.z)
                };
            Points = r;
            }

        private Double x;
        private Double y;
        private Double z;
        }
    }