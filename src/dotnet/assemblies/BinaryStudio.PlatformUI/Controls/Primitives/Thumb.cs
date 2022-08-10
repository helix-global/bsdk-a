using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    using UIThumb = System.Windows.Controls.Primitives.Thumb;
    public class Thumb : Control
        {
        static Thumb()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Thumb), new FrameworkPropertyMetadata(typeof(Thumb)));
            }
        private static readonly DependencyPropertyKey IsDraggingPropertyKey = DependencyProperty.RegisterReadOnly("IsDragging", typeof(Boolean), typeof(Thumb), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsDraggingProperty = IsDraggingPropertyKey.DependencyProperty;
        public Boolean IsDragging
            {
            get { return (Boolean) GetValue(IsDraggingProperty); }
            protected set { SetValue(IsDraggingPropertyKey, value); }
            }

        #region E:DragCompleted:DragCompletedEventHandler
        public event DragCompletedEventHandler DragCompleted
            {
            add    { AddHandler(UIThumb.DragCompletedEvent, value);    }
            remove { RemoveHandler(UIThumb.DragCompletedEvent, value); }
            }
        #endregion
        #region E:DragDelta:DragDeltaEventHandler
        public event DragDeltaEventHandler DragDelta
            {
            add    { AddHandler(UIThumb.DragDeltaEvent, value);    }
            remove { RemoveHandler(UIThumb.DragDeltaEvent, value); }
            }
        #endregion
        #region E:DragStarted:DragStartedEventHandler
        public event DragStartedEventHandler DragStarted
            {
            add    { AddHandler(UIThumb.DragStartedEvent, value);    }
            remove { RemoveHandler(UIThumb.DragStartedEvent, value); }
            }
        #endregion

        private void DoPreviewDragOver()
            {
            Thread.Sleep(500);
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,new Action(()=> {
                if (IsDragging) {
                    var position = Mouse.GetPosition(this);
                    var point = PointToScreen(position);
                    if (point == _previousScreenCoordPosition)
                        {
                        RaiseEvent(new DragDeltaEventArgs(position.X - _originThumbPoint.X, position.Y - _originThumbPoint.Y));
                        }
                    (new Action(DoPreviewDragOver)).BeginInvoke(null, null);
                    }
                }));
            }

        #region M:OnMouseLeftButtonDown(MouseButtonEventArgs)
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
            if (!IsDragging) {
                e.Handled = true;
                Focus();
                CaptureMouse();
                SetValue(IsDraggingPropertyKey, value: true);
                _originThumbPoint = e.GetPosition(this);
                _previousScreenCoordPosition = (_originScreenCoordPosition = PointToScreen(_originThumbPoint));
                var flag = true;
                try
                    {
                    RaiseEvent(new DragStartedEventArgs(_originThumbPoint.X, _originThumbPoint.Y));
                    (new Action(DoPreviewDragOver)).BeginInvoke(null, null);
                    flag = false;
                    }
                finally
                    {
                    if (flag)
                        {
                        CancelDrag();
                        }
                    }
                }
            base.OnMouseLeftButtonDown(e);
            }
        #endregion
        #region M:OnMouseLeftButtonUp(MouseButtonEventArgs)
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            if (IsMouseCaptured && IsDragging) {
                e.Handled = true;
                ClearValue(IsDraggingPropertyKey);
                ReleaseMouseCapture();
                Point point = PointToScreen(e.MouseDevice.GetPosition(this));
                RaiseEvent(new DragCompletedEventArgs(point.X - _originScreenCoordPosition.X, point.Y - _originScreenCoordPosition.Y, canceled: false));
                }
            base.OnMouseLeftButtonUp(e);
            }
        #endregion
        #region M:OnMouseMove(MouseEventArgs)
        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            if (!IsDragging) { return; }
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed) {
                var position = e.GetPosition(this);
                var point = PointToScreen(position);
                if (point != _previousScreenCoordPosition)
                    {
                    _previousScreenCoordPosition = point;
                    e.Handled = true;
                    RaiseEvent(new DragDeltaEventArgs(position.X - _originThumbPoint.X, position.Y - _originThumbPoint.Y));
                    }
                }
            else
                {
                if (ReferenceEquals(e.MouseDevice.Captured, this))
                    {
                    ReleaseMouseCapture();
                    }
                ClearValue(IsDraggingPropertyKey);
                _originThumbPoint.X = 0.0;
                _originThumbPoint.Y = 0.0;
                }
            }
        #endregion
        #region M:CancelDrag
        public void CancelDrag() {
            if (IsDragging) {
                if (IsMouseCaptured) { ReleaseMouseCapture(); }
                ClearValue(IsDraggingPropertyKey);
                RaiseEvent(new DragCompletedEventArgs(_previousScreenCoordPosition.X - _originScreenCoordPosition.X, _previousScreenCoordPosition.Y - _originScreenCoordPosition.Y, canceled: true));
                }
            }
        #endregion

        private Point _previousScreenCoordPosition;
        private Point _originThumbPoint;
        private Point _originScreenCoordPosition;        }
    }
