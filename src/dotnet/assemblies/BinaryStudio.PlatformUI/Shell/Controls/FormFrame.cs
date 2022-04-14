using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class FormFrame : ContentControl
        {
        static FormFrame()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FormFrame), new FrameworkPropertyMetadata(typeof(FormFrame)));
            }

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
            {
            base.OnApplyTemplate();
            InstallThumbHandlers(ThumbRight       = GetTemplateChild("ThumbRight")       as Thumb);
            InstallThumbHandlers(ThumbBottom      = GetTemplateChild("ThumbBottom")      as Thumb);
            InstallThumbHandlers(ThumbRightBottom = GetTemplateChild("ThumbRightBottom") as Thumb);
            InstallThumbHandlers(ThumbTop         = GetTemplateChild("ThumbTop")         as Thumb);
            InstallThumbHandlers(ThumbLeft        = GetTemplateChild("ThumbLeft")        as Thumb);
            InstallThumbHandlers(ThumbRightTop    = GetTemplateChild("ThumbRightTop")    as Thumb);
            InstallThumbHandlers(ThumbLeftBottom  = GetTemplateChild("ThumbLeftBottom")  as Thumb);
            InstallThumbHandlers(ThumbLeftTop     = GetTemplateChild("ThumbLeftTop")     as Thumb);
            }

        private void InstallThumbHandlers(Thumb source) {
            if (source != null) {
                source.DragStarted   += OnDragStarted;
                source.DragCompleted += OnDragCompleted;
                source.DragDelta     += OnDragDelta;
                }
            }

        #region P:Target:Window
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(Window), typeof(FormFrame), new PropertyMetadata(default(Window)));
        public Window Target
            {
            get { return (Window)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
            }
        #endregion

        #region M:OnDragStarted(Object,DragStartedEventArgs)
        private void OnDragStarted(Object sender, DragStartedEventArgs e)
            {
            IsDragging = true;
            }
        #endregion
        #region M:OnDragCompleted(Object,DragCompletedEventArgs)
        private void OnDragCompleted(Object sender, DragCompletedEventArgs e) {
            IsDragging = false;
            }
        #endregion
        #region M:OnDragDelta(Object,DragDeltaEventArgs)
        private void OnDragDelta(Object sender, DragDeltaEventArgs e)
            {
            e.Handled = true;
            if (IsDragging) {
                if (Target is Window target)
                    {
                    var handle = new WindowInteropHelper(target).Handle;
                    #region ThumbRight
                    if (ReferenceEquals(sender, ThumbRight))
                        {
                        target.Width = Math.Max(target.Width + e.HorizontalChange, target.MinWidth);
                        }
                    #endregion
                    #region ThumbLeft
                    else if (ReferenceEquals(sender, ThumbLeft))
                        {
                        var width = Math.Max(target.Width - e.HorizontalChange, target.MinWidth);
                        //var left  = target.Left + e.HorizontalChange;
                        //target.Arrange(new Rect(new Point(left, target.Top), new Size(width, target.Height)));
                        //target.Left += e.HorizontalChange;
                        //target.Width = Math.Max(width - e.HorizontalChange, target.MinWidth);
                        NativeMethods.GetWindowRect(handle, out var rc);
                        rc.Left = (Int32)(rc.Left + e.HorizontalChange);
                        NativeMethods.SetWindowPos(handle, IntPtr.Zero, rc.Left, rc.Top, rc.Width, rc.Height, NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
                        //NativeMethods.MoveWindow(handle, rc.Left, rc.Top, rc.Width, rc.Height, false);
                        }
                    #endregion
                    #region ThumbBottom
                    else if (ReferenceEquals(sender, ThumbBottom))
                        {
                        target.Height = Math.Max(target.Height + e.VerticalChange, target.MinHeight);
                        }
                    #endregion
                    #region ThumbTop
                    else if (ReferenceEquals(sender, ThumbTop))
                        {
                        target.Height = Math.Max(target.Height - e.VerticalChange, target.MinHeight);
                        target.Top    = target.Top + e.VerticalChange;
                        }
                    #endregion
                    #region ThumbRightTop
                    else if (ReferenceEquals(sender, ThumbRightTop))
                        {
                        target.Height = Math.Max(target.Height - e.VerticalChange, target.MinHeight);
                        target.Width  = Math.Max(target.Width + e.HorizontalChange, target.MinWidth);
                        target.Top    = target.Top + e.VerticalChange;
                        }
                    #endregion
                    #region ThumbRightBottom
                    else if (ReferenceEquals(sender, ThumbRightBottom))
                        {
                        NativeMethods.GetWindowRect(handle, out var rc);
                        NativeMethods.SetWindowPos(handle, IntPtr.Zero, rc.Left, rc.Top,
                            (Int32)Math.Max(target.Width + e.HorizontalChange, target.MinWidth),
                            (Int32)Math.Max(target.Height + e.VerticalChange, target.MinHeight),
                            NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
                        }
                    #endregion
                    #region ThumbLeftBottom
                    else if (ReferenceEquals(sender, ThumbLeftBottom))
                        {
                        NativeMethods.GetWindowRect(handle, out var rc);
                        rc.Left   = (Int32)(rc.Left   + e.HorizontalChange);
                        rc.Bottom = (Int32)(rc.Bottom + e.VerticalChange);
                        NativeMethods.SetWindowPos(handle, IntPtr.Zero, rc.Left, rc.Top,
                            rc.Width,
                            rc.Height,
                            NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
                        }
                    #endregion
                    #region ThumbLeftTop
                    else if (ReferenceEquals(sender, ThumbLeftTop))
                        {
                        NativeMethods.GetWindowRect(handle, out var rc);
                        rc.Left   = (Int32)(rc.Left + e.HorizontalChange);
                        rc.Top    = (Int32)(rc.Top  + e.VerticalChange);
                        NativeMethods.SetWindowPos(handle, IntPtr.Zero, rc.Left, rc.Top,
                            rc.Width,
                            rc.Height,
                            NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
                        }
                    #endregion
                    }
                }
            }
        #endregion

        private Thumb ThumbBottom;
        private Thumb ThumbLeft;
        private Thumb ThumbLeftBottom;
        private Thumb ThumbLeftTop;
        private Thumb ThumbRight;
        private Thumb ThumbRightBottom;
        private Thumb ThumbRightTop;
        private Thumb ThumbTop;
        private Boolean IsDragging = false;
        }
    }