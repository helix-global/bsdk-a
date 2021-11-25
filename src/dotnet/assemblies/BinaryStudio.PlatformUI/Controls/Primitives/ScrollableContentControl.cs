using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    public class ScrollableContentControl : ContentControl, IScrollInfo, INotifyPropertyChanged
        {
        static ScrollableContentControl()
            {
            FontFamilyProperty.OverrideMetadata(typeof(ScrollableContentControl),new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, DoTypefaceChanged));
            FontStyleProperty.OverrideMetadata(typeof(ScrollableContentControl),new FrameworkPropertyMetadata(SystemFonts.MessageFontStyle, DoTypefaceChanged));
            FontStretchProperty.OverrideMetadata(typeof(ScrollableContentControl),new FrameworkPropertyMetadata(FontStretches.Normal, DoTypefaceChanged));
            FontWeightProperty.OverrideMetadata(typeof(ScrollableContentControl),new FrameworkPropertyMetadata(FontWeights.Normal, DoTypefaceChanged));
            }

        #region M:IScrollInfo.LineUp
        /// <summary>
        /// Scrolls up within content by one logical unit.
        /// </summary>
        void IScrollInfo.LineUp() {
            SetVerticalOffset(Offset.Y - 1);
            }
        #endregion
        #region M:IScrollInfo.LineDown
        /// <summary>
        /// Scrolls down within content by one logical unit.
        /// </summary>
        void IScrollInfo.LineDown() {
            SetVerticalOffset(Math.Round(Offset.Y + 1));
            }
        #endregion
        #region M:IScrollInfo.LineLeft
        /// <summary>
        /// Scrolls left within content by one logical unit.
        /// </summary>
        void IScrollInfo.LineLeft() {
            LineLeft();
            }
        /// <summary>
        /// Scrolls left within content by one logical unit.
        /// </summary>
        protected virtual void LineLeft() {
            SetHorizontalOffset(Offset.X - 1);
            }
        #endregion
        #region M:IScrollInfo.LineRight
        /// <summary>
        /// Scrolls right within content by one logical unit.
        /// </summary>
        void IScrollInfo.LineRight() {
            LineRight();
            }

        /// <summary>
        /// Scrolls right within content by one logical unit.
        /// </summary>
        protected virtual void LineRight() {
            SetHorizontalOffset(Offset.X + 1);
            }
        #endregion
        #region M:IScrollInfo.PageUp
        /// <summary>
        /// Scrolls up within content by one page.
        /// </summary>
        void IScrollInfo.PageUp() {
            SetVerticalOffset(Offset.Y - Viewport.Y);
            }
        #endregion
        #region M:IScrollInfo.PageDown
        /// <summary>
        /// Scrolls down within content by one page.
        /// </summary>
        void IScrollInfo.PageDown() {
            SetVerticalOffset(Offset.Y + Viewport.Y);
            }
        #endregion
        #region M:IScrollInfo.PageLeft
        /// <summary>
        /// Scrolls left within content by one page.
        /// </summary>
        void IScrollInfo.PageLeft() {
            SetHorizontalOffset(Offset.X - Viewport.X);
            }
        #endregion
        #region M:IScrollInfo.PageRight
        /// <summary>
        /// Scrolls right within content by one page.
        /// </summary>
        void IScrollInfo.PageRight() {
            SetHorizontalOffset(Offset.X + Viewport.X);
            }
        #endregion
        #region M:IScrollInfo.MouseWheelUp
        /// <summary>
        /// Scrolls up within content after a user clicks the wheel button on a mouse.
        /// </summary>
        void IScrollInfo.MouseWheelUp() {
            ((IScrollInfo)this).LineUp();
            }
        #endregion
        #region M:IScrollInfo.MouseWheelDown
        /// <summary>
        /// Scrolls down within content after a user clicks the wheel button on a mouse.
        /// </summary>
        void IScrollInfo.MouseWheelDown() {
            ((IScrollInfo)this).LineDown();
            }
        #endregion
        #region M:IScrollInfo.MouseWheelLeft
        /// <summary>
        /// Scrolls left within content after a user clicks the wheel button on a mouse.
        /// </summary>
        void IScrollInfo.MouseWheelLeft() {
            ((IScrollInfo)this).LineLeft();
            }
        #endregion
        #region M:IScrollInfo.MouseWheelRight
        /// <summary>
        /// Scrolls right within content after a user clicks the wheel button on a mouse.
        /// </summary>
        void IScrollInfo.MouseWheelRight() {
            ((IScrollInfo)this).LineRight();
            }
        #endregion
        #region M:IScrollInfo.SetHorizontalOffset(Double)
        /// <summary>
        /// Sets the amount of horizontal offset.
        /// </summary>
        /// <param name="offset">The degree to which content is horizontally offset from the containing viewport.</param>
        void IScrollInfo.SetHorizontalOffset(Double offset) {
            SetHorizontalOffset(offset);
            }

        /// <summary>
        /// Sets the amount of horizontal offset.
        /// </summary>
        /// <param name="offset">The degree to which content is horizontally offset from the containing viewport.</param>
        protected virtual void SetHorizontalOffset(Double offset) {
            Offset = new Vector(
                Math.Min(Math.Max(0, offset), Extent.X),
                Offset.Y);
            }
        #endregion
        #region M:IScrollInfo.SetVerticalOffset(Double)
        /// <summary>
        /// Sets the amount of vertical offset.
        /// </summary>
        /// <param name="offset"></param>
        void IScrollInfo.SetVerticalOffset(Double offset) {
            SetVerticalOffset(Math.Round(offset));
            }

        /// <summary>
        /// Invoked when the vertical offset changed.
        /// </summary>
        /// <param name="offset">The amount of vertical offset.</param>
        /// <param name="δ">The difference of vertical offset.</param>
        protected virtual void OnSetVerticalOffset(Double offset, Double δ) {}

        /// <summary>
        /// Sets the amount of vertical offset.
        /// </summary>
        /// <param name="offset"></param>
        protected void SetVerticalOffset(Double offset) {
            var γ = Math.Min(Math.Max(0, offset), Math.Max(0, Extent.Y - 1));
            var δ = Offset.Y - γ;
            Offset = new Vector(Offset.X, γ);
            OnSetVerticalOffset(offset, δ);
            }
        #endregion
        #region M:IScrollInfo.MakeVisible(Visual,Rect):Rect
        /// <summary>
        /// Forces content to scroll until the coordinate space of a <see cref="Visual"/> object is visible.
        /// </summary>
        /// <param name="visual">A <see cref="Visual"/> that becomes visible.</param>
        /// <param name="rectangle">A bounding rectangle that identifies the coordinate space to make visible.</param>
        /// <returns></returns>
        Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle) {
            return rectangle;
            }
        #endregion
        #region P:IScrollInfo.CanVerticallyScroll:Boolean
        /// <summary>Identifies the <see cref="CanVerticallyScroll" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="CanVerticallyScroll" /> dependency property.</returns>
        public static readonly DependencyProperty CanVerticallyScrollProperty = DependencyProperty.Register("CanVerticallyScroll", typeof(Boolean), typeof(ScrollableContentControl), new PropertyMetadata(true, OnCanVerticallyScrollChanged));
        private static void OnCanVerticallyScrollChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as ScrollableContentControl);
            if (source != null)
                {
                source.OnCanVerticallyScrollChanged();
                }
            }

        protected virtual void OnCanVerticallyScrollChanged() { }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the vertical axis is possible.
        /// </summary>
        public Boolean CanVerticallyScroll {
            get { return (Boolean)GetValue(CanVerticallyScrollProperty); }
            set { SetValue(CanVerticallyScrollProperty, value); }
            }
        #endregion
        #region P:IScrollInfo.CanHorizontallyScroll:Boolean
        /// <summary>Identifies the <see cref="CanHorizontallyScroll" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="CanHorizontallyScroll" /> dependency property.</returns>
        public static readonly DependencyProperty CanHorizontallyScrollProperty = DependencyProperty.Register("CanHorizontallyScroll", typeof(Boolean), typeof(ScrollableContentControl), new PropertyMetadata(true, OnCanHorizontallyScrollChanged));
        private static void OnCanHorizontallyScrollChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as ScrollableContentControl);
            if (source != null) {
                source.OnCanHorizontallyScrollChanged();
                }
            }

        protected virtual void OnCanHorizontallyScrollChanged() { }

        /// <summary>
        /// Gets or sets a value that indicates whether scrolling on the horizontal axis is possible.
        /// </summary>
        public Boolean CanHorizontallyScroll {
            get { return (Boolean)GetValue(CanHorizontallyScrollProperty); }
            set { SetValue(CanHorizontallyScrollProperty, value); }
            }
        #endregion
        #region P:IScrollInfo.ExtentWidth(ExtentHeight):Vector
        private static readonly DependencyPropertyKey ExtentPropertyKey = DependencyProperty.RegisterReadOnly("Extent", typeof(Vector), typeof(ScrollableContentControl), new PropertyMetadata(default(Vector), OnExtentChanged));
        /// <summary>Identifies the <see cref="Extent" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="Extent" /> dependency property.</returns>
        public static readonly DependencyProperty ExtentProperty = ExtentPropertyKey.DependencyProperty;
        private static void OnExtentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as ScrollableContentControl);
            if (source != null) {
                source.OnExtentChanged();
                }
            }

        protected virtual void OnExtentChanged() {
            InvalidateScrollInfo();
            }

        /// <summary>
        /// Gets the vertical and horizontal size of the extent.
        /// </summary>
        public Vector Extent {
            get { return (Vector)GetValue(ExtentProperty); }
            protected set { SetValue(ExtentPropertyKey, value); }
            }
        #region P:IScrollInfo.ExtentWidth:Double
        /// <summary>Gets the horizontal size of the extent.</summary>
        /// <returns>A <see cref="T:System.Double" /> that represents, in device independent pixels, the horizontal size of the extent. This property has no default value.</returns>
        Double IScrollInfo.ExtentWidth { get {
            return Extent.X;
            }}
        #endregion
        #region P:IScrollInfo.ExtentHeight:Double
        /// <summary>Gets the vertical size of the extent.</summary>
        /// <returns>A <see cref="T:System.Double" /> that represents, in device independent pixels, the vertical size of the extent.This property has no default value.</returns>
        Double IScrollInfo.ExtentHeight { get {
            return Extent.Y;
            }}
        #endregion
        #endregion
        #region P:IScrollInfo.ViewportWidth(ViewportHeight):Vector
        private static readonly DependencyPropertyKey ViewportPropertyKey = DependencyProperty.RegisterReadOnly("Viewport", typeof(Vector), typeof(ScrollableContentControl), new PropertyMetadata(default(Vector), OnViewportChanged));
        /// <summary>Identifies the <see cref="Viewport" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="Viewport" /> dependency property.</returns>
        public static readonly DependencyProperty ViewportProperty = ViewportPropertyKey.DependencyProperty;
        private static void OnViewportChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as ScrollableContentControl);
            if (source != null) {
                source.OnViewportChanged((Vector)e.OldValue, (Vector)e.NewValue);
                }
            }

        protected virtual void OnViewportChanged(Vector o, Vector n) {
            if (!DoubleUtil.AreClose(o.Y, n.Y)) { OnPropertyChanged(nameof(ViewportHeight)); }
            if (ScrollOwner != null) {
                ScrollOwner.InvalidateScrollInfo();
                }
            }

        /// <summary>
        /// Gets the vertical and horizontal size of the viewport for this content.
        /// </summary>
        public Vector Viewport {
            get { return (Vector)GetValue(ViewportProperty); }
            protected set { SetValue(ViewportPropertyKey, value); }
            }
        #region P:IScrollInfo.ViewportWidth:Double
        /// <summary>Gets the horizontal size of the viewport for this content.</summary>
        /// <returns>A <see cref="T:System.Double" /> that represents, in device independent pixels, the horizontal size of the viewport for this content. This property has no default value.</returns>
        Double IScrollInfo.ViewportWidth { get {
            return Viewport.X;
            }}
        #endregion
        #region P:IScrollInfo.ViewportHeight:Double
        /// <summary>Gets the vertical size of the viewport for this content.</summary>
        /// <returns>A <see cref="T:System.Double" /> that represents, in device independent pixels, the vertical size of the viewport for this content. This property has no default value.</returns>
        public Double ViewportHeight { get {
            return Viewport.Y;
            }}
        #endregion
        #endregion
        #region P:IScrollInfo.HorizontalOffset(VerticalOffset):Vector
        private static readonly DependencyPropertyKey OffsetPropertyKey = DependencyProperty.RegisterReadOnly("Offset", typeof(Vector), typeof(ScrollableContentControl), new PropertyMetadata(default(Vector), OnOffsetChanged));
        /// <summary>Identifies the <see cref="Offset" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="Offset" /> dependency property.</returns>
        public static readonly DependencyProperty OffsetProperty = OffsetPropertyKey.DependencyProperty;
        private static void OnOffsetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as ScrollableContentControl);
            if (source != null) {
                source.OnOffsetChanged(new ObjectPropertyChangedEventArgs<Vector>(e));
                }
            }

        /// <summary>Invoked where offset has been changed.</summary>
        /// <param name="e">The <see cref="ObjectPropertyChangedEventArgs{Vector}"/> that contains the event data.</param>
        protected virtual void OnOffsetChanged(ObjectPropertyChangedEventArgs<Vector> e) {
            InvalidateScrollInfo();
            }

        /// <summary>
        /// Gets the vertical and horizontal offset of the scrolled content.
        /// </summary>
        public Vector Offset {
            get { return (Vector)GetValue(OffsetProperty); }
            protected set { SetValue(OffsetPropertyKey, value); }
            }
        #region P:IScrollInfo.HorizontalOffset:Double
        /// <summary>Gets the horizontal offset of the scrolled content.</summary>
        /// <returns>A <see cref="T:System.Double" /> that represents, in device independent pixels, the horizontal offset. This property has no default value.</returns>
        Double IScrollInfo.HorizontalOffset { get {
            return Offset.X;
            }}
        #endregion
        #region P:IScrollInfo.VerticalOffset:Double
        /// <summary>Gets the vertical offset of the scrolled content.</summary>
        /// <returns>A <see cref="T:System.Double" /> that represents, in device independent pixels, the vertical offset of the scrolled content. Valid values are between zero and the <see cref="P:System.Windows.Controls.Primitives.IScrollInfo.ExtentHeight" /> minus the <see cref="P:System.Windows.Controls.Primitives.IScrollInfo.ViewportHeight" />. This property has no default value.</returns>
        Double IScrollInfo.VerticalOffset { get {
            return Offset.Y;
            }}
        #endregion
        #endregion
        //#region P:IScrollInfo.ScrollOwner:ScrollViewer
        //private static readonly DependencyPropertyKey ScrollOwnerPropertyKey = DependencyProperty.RegisterReadOnly("ScrollOwner", typeof(ScrollViewer), typeof(ScrollableContentControl), new PropertyMetadata(default(ScrollViewer), OnScrollOwnerChanged));
        ///// <summary>Identifies the <see cref="ScrollOwner" /> dependency property.</summary>
        ///// <returns>The identifier for the <see cref="ScrollOwner" /> dependency property.</returns>
        //public static readonly DependencyProperty ScrollOwnerProperty = ScrollOwnerPropertyKey.DependencyProperty;
        //private static void OnScrollOwnerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
        //    var source = (sender as ScrollableContentControl);
        //    if (source != null) {
        //        source.OnScrollOwnerChanged(e.OldValue as ScrollViewer, e.NewValue as ScrollViewer);
        //        }
        //    }

        //protected virtual void OnScrollOwnerChanged(ScrollViewer o, ScrollViewer n) {
        //    if (o != null) { o.LayoutUpdated -= OnScrollOwnerLayoutUpdated; }
        //    if (n != null) { n.LayoutUpdated += OnScrollOwnerLayoutUpdated; }
        //    OnScrollOwnerChanged();
        //    }

        //protected virtual void OnScrollOwnerChanged() {
        //   InvalidateScrollInfo();
        //    }

        //public ScrollViewer ScrollOwner {
        //    get { return (ScrollViewer)GetValue(ScrollOwnerProperty); }
        //    protected set { SetValue(ScrollOwnerPropertyKey, value); }
        //    }

        ///// <summary>
        ///// Gets or sets a <see cref="ScrollViewer"/> element that controls scrolling behavior.
        ///// </summary>
        //ScrollViewer IScrollInfo.ScrollOwner {
        //    get { return ScrollOwner; }
        //    set { ScrollOwner = value; }
        //    }
        //#endregion
        #region P:ScrollOwner:ScrollViewer
        public static readonly DependencyProperty ScrollOwnerProperty = DependencyProperty.Register("ScrollOwner", typeof(ScrollViewer), typeof(ScrollableContentControl), new PropertyMetadata(default(ScrollViewer), OnScrollOwnerChanged));
        private static void OnScrollOwnerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as ScrollableContentControl);
            if (source != null)
                {
                source.OnScrollOwnerChanged(e.OldValue as ScrollViewer, e.NewValue as ScrollViewer);
                }
            }

        protected virtual void OnScrollOwnerChanged(ScrollViewer o, ScrollViewer n) {
            if (o != null) { o.LayoutUpdated -= OnScrollOwnerLayoutUpdated; }
            if (n != null) { n.LayoutUpdated += OnScrollOwnerLayoutUpdated; }
            OnScrollOwnerChanged();
            }

        private void OnScrollOwnerChanged()
            {
            InvalidateScrollInfo();
            }

        public ScrollViewer ScrollOwner {
            get { return (ScrollViewer)GetValue(ScrollOwnerProperty); }
            set { SetValue(ScrollOwnerProperty, value); }
            }
        #endregion
        #region P:PhysicalViewport:Vector
        private static readonly DependencyPropertyKey PhysicalViewportPropertyKey = DependencyProperty.RegisterReadOnly("PhysicalViewport", typeof(Vector), typeof(ScrollableContentControl), new PropertyMetadata(default(Vector), OnPhysicalViewportChanged));
        /// <summary>Identifies the <see cref="PhysicalViewport" /> dependency property.</summary>
        /// <returns>The identifier for the <see cref="PhysicalViewport" /> dependency property.</returns>
        public static readonly DependencyProperty PhysicalViewportProperty = PhysicalViewportPropertyKey.DependencyProperty;
        private static void OnPhysicalViewportChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as ScrollableContentControl);
            if (source != null) {
                source.OnPhysicalViewportChanged();
                }
            }

        protected virtual void OnPhysicalViewportChanged() {
            InvalidateVisual();
            }

        public Vector PhysicalViewport {
            get { return (Vector)GetValue(PhysicalViewportProperty); }
            private set { SetValue(PhysicalViewportPropertyKey, value); }
            }
        #endregion
        #region M:OnScrollOwnerLayoutUpdated(Object,EventArgs)
        private void OnScrollOwnerLayoutUpdated(Object sender, EventArgs e) {
            OnScrollOwnerLayoutUpdated(ScrollOwner);
            }
        #endregion
        #region M:OnScrollOwnerLayoutUpdated(ScrollViewer)
        protected virtual void OnScrollOwnerLayoutUpdated(ScrollViewer sender) {
            if (sender != null) {
                var W = ((sender.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible) || ((sender.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto) && (sender.ComputedHorizontalScrollBarVisibility == Visibility.Visible)))
                    ? SystemParameters.VerticalScrollBarWidth
                    : 0.0;
                var H = ((sender.VerticalScrollBarVisibility == ScrollBarVisibility.Visible) || ((sender.VerticalScrollBarVisibility == ScrollBarVisibility.Auto) && (sender.ComputedVerticalScrollBarVisibility == Visibility.Visible)))
                    ? SystemParameters.HorizontalScrollBarHeight
                    : 0.0;
                OnScrollOwnerLayoutUpdated(new Size(
                    Math.Max(0, sender.ActualWidth  - W),
                    Math.Max(0, sender.ActualHeight - H)));
                }
            }
        #endregion
        #region M:OnScrollOwnerLayoutUpdated(Size)
        protected virtual void OnScrollOwnerLayoutUpdated(Size sz) {
            PhysicalViewport = new Vector(sz.Width, sz.Height);
            }
        #endregion
        #region M:InvalidateScrollInfo
        protected virtual void InvalidateScrollInfo() {
            if (ScrollOwner != null) {
                ScrollOwner.InvalidateScrollInfo();
                InvalidateVisual();
                }
            }
        #endregion
        #region P:Typeface:Typeface
        private static readonly DependencyPropertyKey TypefacePropertyKey = DependencyProperty.RegisterReadOnly("Typeface", typeof(Typeface), typeof(ScrollableContentControl), new PropertyMetadata(default(Typeface), OnTypefaceChanged));
        public static readonly DependencyProperty TypefaceProperty = TypefacePropertyKey.DependencyProperty;
        private static void OnTypefaceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as ScrollableContentControl);
            if (source != null)
                {
                source.OnTypefaceChanged();
                }
            }

        protected virtual void OnTypefaceChanged()
            {
            }

        private void DoTypefaceChanged()
            {
            Typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            }

        private static void DoTypefaceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as ScrollableContentControl);
            if (source != null)
                {
                source.DoTypefaceChanged();
                }
            }

        public Typeface Typeface {
            get { return (Typeface)GetValue(TypefaceProperty); }
            private set { SetValue(TypefacePropertyKey, value); }
            }
        #endregion

        //#region P:ScrollableContentControl.ViewportHeight:Double
        //public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.RegisterAttached("ViewportHeight", typeof(Double), typeof(ScrollableContentControl), new PropertyMetadata(default(Double), OnViewportHeightChanged));
        //public static void SetViewportHeight(DependencyObject source, Double value)
        //    {
        //    if (source == null) { throw new ArgumentNullException("source"); }
        //    source.SetValue(ViewportHeightProperty, value);
        //    }

        //public static Double GetViewportHeight(DependencyObject source)
        //    {
        //    if (source == null) { throw new ArgumentNullException("source"); }
        //    return (Double)source.GetValue(ViewportHeightProperty);
        //    }
        //public Double ViewportHeight {
        //    get { return GetViewportHeight(this); }
        //    set { SetViewportHeight(this, value); }
        //    }
        //private static void OnViewportHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
        //    var source = (sender as ScrollableContentControl);
        //    if (source != null) {
        //        source.Viewport = new Vector(source.Viewport.X, (Double)e.NewValue);
        //        }
        //    }
        //#endregion
        #region P:ScrollableContentControl.ExtentHeight:Double
        public static readonly DependencyProperty ExtentHeightProperty = DependencyProperty.RegisterAttached("ExtentHeight", typeof(Double), typeof(ScrollableContentControl), new PropertyMetadata(default(Double), OnExtentHeightChanged));
        public static void SetExtentHeight(DependencyObject source, Double value)
            {
            if (source == null) { throw new ArgumentNullException("source"); }
            source.SetValue(ExtentHeightProperty, value);
            }

        public static Double GetExtentHeight(DependencyObject source)
            {
            if (source == null) { throw new ArgumentNullException("source"); }
            return (Double)source.GetValue(ExtentHeightProperty);
            }
        public Double ExtentHeight {
            get { return GetExtentHeight(this); }
            set { SetExtentHeight(this, value); }
            }
        private static void OnExtentHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as ScrollableContentControl);
            if (source != null) {
                source.Extent = new Vector(source.Extent.X, (Double)e.NewValue);
                }
            }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        //TODO:[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
            {
            base.OnPropertyChanged(e);
            OnPropertyChanged(e.Property.Name);
            }
        }    }