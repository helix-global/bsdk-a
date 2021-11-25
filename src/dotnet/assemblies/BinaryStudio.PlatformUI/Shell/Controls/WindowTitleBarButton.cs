using System.Windows;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class WindowTitleBarButton : GlyphButton
        {
        #region P:CornerRadius:CornerRadius
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(WindowTitleBarButton), new FrameworkPropertyMetadata(new CornerRadius(0.0)));
        public CornerRadius CornerRadius
            {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
            }
        #endregion

        static WindowTitleBarButton()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowTitleBarButton), new FrameworkPropertyMetadata(typeof(WindowTitleBarButton)));
            }

        protected override void OnCommandTargetChanged()
            {
            base.OnCommandTargetChanged();
            }
        }
    }
