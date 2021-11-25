using System;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Controls
    {
    using UIHeaderedContentControl = System.Windows.Controls.HeaderedContentControl;
    public class HeaderedContentControl : UIHeaderedContentControl
        {
        static HeaderedContentControl()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderedContentControl), new FrameworkPropertyMetadata(typeof(HeaderedContentControl)));
            }

        #region P:HeaderedContentControl.Header:Object
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.RegisterAttached("Header", typeof(Object), typeof(HeaderedContentControl), new PropertyMetadata(default(Object), OnHeaderChanged));
        public static void SetHeader(DependencyObject source, Object value) {
            if (source == null) { throw new ArgumentNullException("source"); }
            source.SetValue(HeaderProperty, value);
            }

        public static Object GetHeader(DependencyObject source) {
            if (source == null) { throw new ArgumentNullException("source"); }
            return (Object)source.GetValue(HeaderProperty);
            }
        private static void OnHeaderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as UIHeaderedContentControl);
            if (source != null) {
                source.Header = e.NewValue;
                }
            }
        #endregion
        #region P:StripPlacement:Dock
        public static readonly DependencyProperty StripPlacementProperty = DependencyProperty.Register("StripPlacement", typeof(Dock), typeof(HeaderedContentControl), new PropertyMetadata(Dock.Top));
        public Dock StripPlacement
            {
            get { return (Dock)GetValue(StripPlacementProperty); }
            set { SetValue(StripPlacementProperty, value); }
            }
        #endregion
        }
    }
