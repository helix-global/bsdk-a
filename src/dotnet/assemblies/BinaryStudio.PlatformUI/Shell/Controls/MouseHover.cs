using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public static class MouseHover
        {
        public static readonly RoutedEvent MouseHoverEvent = EventManager.RegisterRoutedEvent("MouseHover", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(MouseHover));
        public static readonly DependencyProperty MouseHoverDelayProperty = DependencyProperty.RegisterAttached("MouseHoverDelay", typeof(TimeSpan), typeof(MouseHover), new FrameworkPropertyMetadata(TimeSpan.Zero));
        public static readonly DependencyProperty IsMouseHoverTrackingEnabledProperty = DependencyProperty.RegisterAttached("IsMouseHoverTrackingEnabled", typeof(Boolean), typeof(MouseHover), new FrameworkPropertyMetadata(Boxes.BooleanFalse, OnIsMouseHoverTrackingEnabledChanged));
        private static readonly DependencyProperty MouseHoverMonitorProperty = DependencyProperty.RegisterAttached("MouseHoverMonitor", typeof(Monitor), typeof(MouseHover), new FrameworkPropertyMetadata(null));

        public static Boolean GetIsMouseHoverTrackingEnabled(UIElement element)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (Boolean)element.GetValue(IsMouseHoverTrackingEnabledProperty);
            }

        public static void SetIsMouseHoverTrackingEnabled(UIElement element, Boolean value)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(IsMouseHoverTrackingEnabledProperty, Boxes.Box(value));
            }

        public static TimeSpan GetMouseHoverDelay(UIElement element)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (TimeSpan)element.GetValue(MouseHoverDelayProperty);
            }

        public static void SetMouseHoverDelay(UIElement element, TimeSpan value)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(MouseHoverDelayProperty, value);
            }

        private static Monitor GetMouseHoverMonitor(UIElement element)
            {
            return (Monitor)element.GetValue(MouseHoverMonitorProperty);
            }

        private static void SetMouseHoverMonitor(UIElement element, Monitor value)
            {
            element.SetValue(MouseHoverMonitorProperty, value);
            }

        private static void OnIsMouseHoverTrackingEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
            {
            var element = obj as UIElement;
            if (element == null)
                throw new ArgumentException("MouseHover element must be a UIElement.");
            if ((Boolean)args.NewValue)
                AttachHoverMonitor(element);
            else
                DetachHoverMonitor(element);
            }

        private static void DetachHoverMonitor(UIElement element)
            {
            var mouseHoverMonitor = GetMouseHoverMonitor(element);
            if (mouseHoverMonitor == null)
                return;
            mouseHoverMonitor.Dispose();
            SetMouseHoverMonitor(element, null);
            }

        private static void AttachHoverMonitor(UIElement element)
            {
            DetachHoverMonitor(element);
            SetMouseHoverMonitor(element, new Monitor(element));
            }

        private class Monitor : DisposableObject
            {
            private Boolean isActive;
            private PresentationSource source;
            private Boolean isMouseOver;

            private UIElement Element { get; }

            private DispatcherTimer Timer { get; set; }

            private PresentationSource Source
                {
                get
                    {
                    return source;
                    }
                set
                    {
                    if (source == value)
                        return;
                    if (source == null && value != null)
                        {
                        BroadcastMessageMonitor.Instance.Activated += OnActivated;
                        BroadcastMessageMonitor.Instance.Deactivated += OnDeactivated;
                        IsMouseOver = Element.IsMouseOver;
                        IsActive = BroadcastMessageMonitor.Instance.IsActive;
                        }
                    else if (source != null && value == null)
                        {
                        BroadcastMessageMonitor.Instance.Activated -= OnActivated;
                        BroadcastMessageMonitor.Instance.Deactivated -= OnDeactivated;
                        }
                    source = value;
                    }
                }

            private Boolean IsActive
                {
                get
                    {
                    return isActive;
                    }
                set
                    {
                    if (isActive == value)
                        return;
                    isActive = value;
                    UpdateTimerState();
                    }
                }

            private Boolean IsMouseOver
                {
                get
                    {
                    return isMouseOver;
                    }
                set
                    {
                    if (isMouseOver == value)
                        return;
                    isMouseOver = value;
                    UpdateTimerState();
                    }
                }

            public Monitor(UIElement element)
                {
                Element = element;
                Element.MouseEnter += OnMouseEnter;
                Element.MouseLeave += OnMouseLeave;
                PresentationSource.AddSourceChangedHandler(Element, OnSourceChanged);
                Source = PresentationSource.FromVisual(Element);
                }

            private void UpdateTimerState()
                {
                if (IsMouseOver && IsActive)
                    StartTimer();
                else
                    StopTimer();
                }

            private void StartTimer()
                {
                StopTimer();
                Timer = new DispatcherTimer(GetMouseHoverDelay(Element), DispatcherPriority.Input, OnTimerTick, Element.Dispatcher);
                }

            private void StopTimer()
                {
                if (Timer == null)
                    return;
                Timer.Stop();
                Timer = null;
                }

            private void OnTimerTick(Object sender, EventArgs args)
                {
                Element.RaiseEvent(new RoutedEventArgs(MouseHoverEvent));
                StopTimer();
                }

            private void OnMouseEnter(Object sender, MouseEventArgs e)
                {
                IsMouseOver = true;
                }

            private void OnMouseLeave(Object sender, MouseEventArgs e)
                {
                IsMouseOver = false;
                }

            private void OnActivated(Object sender, EventArgs e)
                {
                IsActive = true;
                }

            private void OnDeactivated(Object sender, EventArgs e)
                {
                IsActive = false;
                }

            private void OnSourceChanged(Object sender, SourceChangedEventArgs e)
                {
                Source = e.NewSource;
                }

            protected override void DisposeManagedResources()
                {
                StopTimer();
                Element.MouseEnter -= OnMouseEnter;
                Element.MouseLeave -= OnMouseLeave;
                BroadcastMessageMonitor.Instance.Activated -= OnActivated;
                BroadcastMessageMonitor.Instance.Deactivated -= OnDeactivated;
                base.DisposeManagedResources();
                }
            }
        }
    }