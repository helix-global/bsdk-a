using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DockTarget : Border
        {
        #region P:DockTargetType:DockTargetType
        public static readonly DependencyProperty DockTargetTypeProperty = DependencyProperty.Register("DockTargetType", typeof(DockTargetType), typeof(DockTarget), new FrameworkPropertyMetadata(DockTargetType.Inside));
        public DockTargetType DockTargetType
            {
            get { return (DockTargetType)GetValue(DockTargetTypeProperty); }
            set { SetValue(DockTargetTypeProperty, value); }
            }
        #endregion
        #region P:DockSiteType:DockSiteType
        public static readonly DependencyProperty DockSiteTypeProperty = DependencyProperty.Register("DockSiteType", typeof(DockSiteType), typeof(DockTarget), new FrameworkPropertyMetadata(DockSiteType.Default));
        public DockSiteType DockSiteType
            {
            get { return (DockSiteType)GetValue(DockSiteTypeProperty); }
            set { SetValue(DockSiteTypeProperty, value); }
            }
        #endregion
        #region P:AdornmentTarget:FrameworkElement
        public static readonly DependencyProperty AdornmentTargetProperty = DependencyProperty.Register("AdornmentTarget", typeof(FrameworkElement), typeof(DockTarget));
        public FrameworkElement AdornmentTarget
            {
            get { return (FrameworkElement)GetValue(AdornmentTargetProperty); }
            set { SetValue(AdornmentTargetProperty, value); }
            }
        #endregion
        #region P:TargetElement:ViewElement
        public ViewElement TargetElement { get {
            return (AdornmentTarget == null)
                ? DataContext as ViewElement
                : AdornmentTarget.DataContext as ViewElement;
            }}
        #endregion
        }
    }
