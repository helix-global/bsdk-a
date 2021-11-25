using System;
using System.Windows;
using System.Windows.Interop;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    [TemplatePart(Name = "PART_HwndHost", Type = typeof(HwndHost))]
    public class AutoHideWindow : LayoutSynchronizedContentControl, IResizable, IDisposable
        {
        public static readonly RoutedEvent SlideOpenedEvent = EventManager.RegisterRoutedEvent("SlideOpened", RoutingStrategy.Direct, typeof(EventHandler<ViewEventArgs>), typeof(AutoHideWindow));
        public static readonly RoutedEvent SlideClosedEvent = EventManager.RegisterRoutedEvent("SlideClosed", RoutingStrategy.Direct, typeof(EventHandler<ViewEventArgs>), typeof(AutoHideWindow));
        private Boolean disposed;

        #region P:DockRootElement:FrameworkElement
        public static readonly DependencyProperty DockRootElementProperty = DependencyProperty.Register("DockRootElement", typeof(FrameworkElement), typeof(AutoHideWindow));
        public FrameworkElement DockRootElement
            {
            get { return (FrameworkElement)GetValue(DockRootElementProperty); }
            set { SetValue(DockRootElementProperty, value); }
            }
        #endregion

        public Size MinSize
            {
            get
                {
                return new Size(MinWidth.IsNonreal() ? 0.0 : MinWidth, MinHeight.IsNonreal() ? 0.0 : MinHeight);
                }
            }

        public Size MaxSize
            {
            get
                {
                return new Size(MaxWidth.IsNonreal() ? Double.MaxValue : MaxWidth, MaxHeight.IsNonreal() ? Double.MaxValue : MaxHeight);
                }
            }

        public Rect CurrentScreenBounds
            {
            get
                {
                var point = new Point(0.0, 0.0);
                if (this.IsConnectedToPresentationSource())
                    point = PointToScreen(point);
                return new Rect(point, RenderSize.LogicalToDeviceUnits());
                }
            }

        public Rect CurrentBounds
            {
            get
                {
                return new Rect(new Point(0.0, 0.0), RenderSize);
                }
            }

        static AutoHideWindow()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideWindow), new FrameworkPropertyMetadata(typeof(AutoHideWindow)));
            }

        public AutoHideWindow()
            {
            SetIsAutoHidden(this, true);
            }

        #region P:AutoHideWindow.IsAutoHidden:Boolean
        public static readonly DependencyProperty IsAutoHiddenProperty = DependencyProperty.RegisterAttached("IsAutoHidden", typeof(Boolean), typeof(AutoHideWindow), new FrameworkPropertyMetadata(Boxes.BooleanFalse, FrameworkPropertyMetadataOptions.Inherits));
        public static Boolean GetIsAutoHidden(UIElement element)
            {
            if (element == null) { throw new ArgumentNullException(nameof(element)); }
            return (Boolean)element.GetValue(IsAutoHiddenProperty);
            }

        public static void SetIsAutoHidden(UIElement element, Boolean value)
            {
            if (element == null) { throw new ArgumentNullException(nameof(element)); }
            element.SetValue(IsAutoHiddenProperty, Boxes.Box(value));
            }
        #endregion

        public void UpdateBounds(Double leftDelta, Double topDelta, Double widthDelta, Double heightDelta)
            {
            var rect1 = new Rect(0.0, 0.0, ActualWidth, ActualHeight);
            var minSize = MinSize;
            var maxSize = MaxSize;
            var rect2 = rect1.Resize(new Vector(leftDelta, topDelta), new Vector(widthDelta, heightDelta), minSize, maxSize);
            if (widthDelta != 0.0)
                Width = rect2.Width;
            if (heightDelta == 0.0)
                return;
            Height = rect2.Height;
            }

        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }

        protected virtual void Dispose(Boolean disposing)
            {
            if (disposed) { return; }
            if (disposing)
                {
                var templateChild = GetTemplateChild("PART_HwndHost") as IDisposable;
                if (templateChild != null)
                    {
                    templateChild.Dispose();
                    }
                }
            disposed = true;
            }
        }
    }