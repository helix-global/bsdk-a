using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    public class ScrollablePanel : Panel, IScrollInfo
        {
        private Boolean _canVerticallyScroll;
        private Boolean _canHorizontallyScroll;
        private Double _extentWidth;
        private Double _extentHeight;
        private Double _viewportWidth;
        private Double _viewportHeight;
        private Double _horizontalOffset;
        private Double _verticalOffset;
        private ScrollViewer _scrollOwner;

        #region P:LinkedScrollInfo:IScrollInfo
        public static readonly DependencyProperty LinkedScrollInfoProperty = DependencyProperty.Register("LinkedScrollInfo", typeof(IScrollInfo), typeof(ScrollablePanel), new PropertyMetadata(default(IScrollInfo)));
        public IScrollInfo LinkedScrollInfo
            {
            get { return (IScrollInfo)GetValue(LinkedScrollInfoProperty); }
            set { SetValue(LinkedScrollInfoProperty, value); }
            }
        #endregion
        #region M:IScrollInfo.LineUp
        void IScrollInfo.LineUp()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IScrollInfo.LineDown
        void IScrollInfo.LineDown()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IScrollInfo.LineLeft
        void IScrollInfo.LineLeft()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IScrollInfo.LineRight
        void IScrollInfo.LineRight()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IScrollInfo.PageUp
        void IScrollInfo.PageUp()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IScrollInfo.PageDown
        void IScrollInfo.PageDown()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IScrollInfo.PageLeft
        void IScrollInfo.PageLeft()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IScrollInfo.PageRight
        void IScrollInfo.PageRight()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IScrollInfo.MouseWheelUp
        void IScrollInfo.MouseWheelUp()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IScrollInfo.MouseWheelDown
        void IScrollInfo.MouseWheelDown()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IScrollInfo.MouseWheelLeft
        void IScrollInfo.MouseWheelLeft()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IScrollInfo.MouseWheelRight
        void IScrollInfo.MouseWheelRight()
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:IScrollInfo.SetHorizontalOffset(Double)
        /// <summary>Sets the amount of horizontal offset.</summary>
        /// <param name="offset">The degree to which content is horizontally offset from the containing viewport.</param>
        void IScrollInfo.SetHorizontalOffset(Double offset)
            {
            HorizontalOffset = offset;
            }
        #endregion
        #region M:IScrollInfo.SetVerticalOffset(Double)
        /// <summary>Sets the amount of vertical offset.</summary>
        /// <param name="offset">The degree to which content is vertically offset from the containing viewport.</param>
        void IScrollInfo.SetVerticalOffset(Double offset)
            {
            VerticalOffset = offset;
            }
        #endregion
        #region M:IScrollInfo.MakeVisible(Visual,Rect)
        Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle)
            {
            throw new NotImplementedException();
            }
        #endregion
        #region P:IScrollInfo.CanHorizontallyScroll:Boolean
        public static readonly DependencyProperty CanHorizontallyScrollProperty = DependencyProperty.Register("CanHorizontallyScroll", typeof(Boolean), typeof(ScrollablePanel), new PropertyMetadata(default(Boolean), OnCanHorizontallyScrollChanged));
        private static void OnCanHorizontallyScrollChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ScrollablePanel source) {
                if (source.LinkedScrollInfo != null) { source.LinkedScrollInfo.CanHorizontallyScroll = (Boolean)e.NewValue; }
                source.InvalidateScrollInfo();
                }
            }

        /// <summary>Gets or sets a value that indicates whether scrolling on the horizontal axis is possible.</summary>
        /// <returns>true if scrolling is possible; otherwise, false. This property has no default value.</returns>
        public Boolean CanHorizontallyScroll
            {
            get { return LinkedScrollInfo?.CanHorizontallyScroll ?? (Boolean)GetValue(CanHorizontallyScrollProperty); }
            set { SetValue(CanHorizontallyScrollProperty, value); }
            }
        #endregion
        #region P:CanVerticallyScroll:Boolean
        public static readonly DependencyProperty CanVerticallyScrollProperty = DependencyProperty.Register("CanVerticallyScroll", typeof(Boolean), typeof(ScrollablePanel), new PropertyMetadata(default(Boolean), OnCanVerticallyScrollChanged));
        private static void OnCanVerticallyScrollChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ScrollablePanel source) {
                if (source.LinkedScrollInfo != null) { source.LinkedScrollInfo.CanVerticallyScroll = (Boolean)e.NewValue; }
                source.InvalidateScrollInfo();
                }
            }

        /// <summary>Gets or sets a value that indicates whether scrolling on the vertical axis is possible. </summary>
        /// <returns>true if scrolling is possible; otherwise, false. This property has no default value.</returns>
        public Boolean CanVerticallyScroll
            {
            get { return LinkedScrollInfo?.CanVerticallyScroll ?? (Boolean)GetValue(CanVerticallyScrollProperty); }
            set { SetValue(CanVerticallyScrollProperty, value); }
            }
        #endregion
        #region P:IScrollInfo.ExtentWidth:Double
        Double IScrollInfo.ExtentWidth
            {
            get { return _extentWidth; }
            }
        #endregion
        #region P:IScrollInfo.ExtentHeight:Double
        Double IScrollInfo.ExtentHeight
            {
            get { return _extentHeight; }
            }
        #endregion
        #region P:IScrollInfo.ViewportWidth:Double
        Double IScrollInfo.ViewportWidth
            {
            get { return _viewportWidth; }
            }
        #endregion
        #region P:IScrollInfo.ViewportHeight:Double
        Double IScrollInfo.ViewportHeight
            {
            get { return _viewportHeight; }
            }
        #endregion
        #region P:IScrollInfo.HorizontalOffset:Double
        private static readonly DependencyPropertyKey HorizontalOffsetPropertyKey = DependencyProperty.RegisterReadOnly("HorizontalOffset", typeof(Double), typeof(ScrollablePanel), new PropertyMetadata(default(Double), OnHorizontalOffsetChanged));
        public static readonly DependencyProperty HorizontalOffsetProperty = HorizontalOffsetPropertyKey.DependencyProperty;
        private static void OnHorizontalOffsetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ScrollablePanel source) {
                source.LinkedScrollInfo?.SetHorizontalOffset((Double)e.NewValue);
                source.InvalidateScrollInfo();
                }
            }

        public Double HorizontalOffset {
            get { return LinkedScrollInfo?.HorizontalOffset ?? (Double)GetValue(HorizontalOffsetProperty); }
            private set { SetValue(HorizontalOffsetPropertyKey, value); }
            }

        /// <summary>Gets the horizontal offset of the scrolled content.</summary>
        /// <returns>A <see cref="T:System.Double" /> that represents, in device independent pixels, the horizontal offset. This property has no default value.</returns>
        Double IScrollInfo.HorizontalOffset
            {
            get { return HorizontalOffset; }
            }
        #endregion
        #region P:IScrollInfo.VerticalOffset:Double
        private static readonly DependencyPropertyKey VerticalOffsetPropertyKey = DependencyProperty.RegisterReadOnly("VerticalOffset", typeof(Double), typeof(ScrollablePanel), new PropertyMetadata(default(Double), OnVerticalOffsetChanged));
        public static readonly DependencyProperty VerticalOffsetProperty = VerticalOffsetPropertyKey.DependencyProperty;
        private static void OnVerticalOffsetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ScrollablePanel source) {
                source.LinkedScrollInfo?.SetVerticalOffset((Double)e.NewValue);
                source.InvalidateScrollInfo();
                }
            }

        public Double VerticalOffset {
            get { return LinkedScrollInfo?.VerticalOffset ?? (Double)GetValue(VerticalOffsetProperty); }
            private set { SetValue(VerticalOffsetPropertyKey, value); }
            }
        Double IScrollInfo.VerticalOffset
            {
            get { return VerticalOffset; }
            }
        #endregion
        #region P:IScrollInfo.ScrollOwner:ScrollViewer
        public static readonly DependencyProperty ScrollOwnerProperty = DependencyProperty.Register("ScrollOwner", typeof(ScrollViewer), typeof(ScrollableContentControl), new PropertyMetadata(default(ScrollViewer), OnScrollOwnerChanged));
        private static void OnScrollOwnerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is ScrollablePanel source) {
                source.OnScrollOwnerChanged(
                    e.OldValue as ScrollViewer,
                    e.NewValue as ScrollViewer);
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

        private void OnScrollOwnerLayoutUpdated(Object sender, EventArgs e) {
            OnScrollOwnerLayoutUpdated(ScrollOwner);
            }

        protected virtual void OnScrollOwnerLayoutUpdated(ScrollViewer sender) {
            if (sender != null) {
                var W = ((sender.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible) || ((sender.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto) && (sender.ComputedHorizontalScrollBarVisibility == Visibility.Visible)))
                    ? SystemParameters.VerticalScrollBarWidth
                    : 0.0;
                var H = ((sender.VerticalScrollBarVisibility == ScrollBarVisibility.Visible) || (
                            (sender.VerticalScrollBarVisibility == ScrollBarVisibility.Auto) &&
                            (sender.ComputedVerticalScrollBarVisibility == Visibility.Visible)))
                    ? SystemParameters.HorizontalScrollBarHeight
                    : 0.0;
                OnScrollOwnerLayoutUpdated(new Size(
                    Math.Max(0, sender.ActualWidth  - W),
                    Math.Max(0, sender.ActualHeight - H)));
                }
            }

        /// <summary>Gets or sets a <see cref="T:System.Windows.Controls.ScrollViewer"/> element that controls scrolling behavior.</summary>
        /// <returns>A <see cref="T:System.Windows.Controls.ScrollViewer"/> element that controls scrolling behavior. This property has no default value.</returns>
        public ScrollViewer ScrollOwner {
            get
                {
                return (LinkedScrollInfo != null)
                    ? LinkedScrollInfo.ScrollOwner
                    : (ScrollViewer)GetValue(ScrollOwnerProperty);
                }
            set
                {
                if (LinkedScrollInfo != null) {
                    LinkedScrollInfo.ScrollOwner = value;
                    return;
                    }
                SetValue(ScrollOwnerProperty, value);
                }
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
        }
    }