using System;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DockAdorner : ContentControl
        {
        public IntPtr OwnerHwnd { get; set; }

        #region P:AdornedElement:FrameworkElement
        public static readonly DependencyProperty AdornedElementProperty = DependencyProperty.Register("AdornedElement", typeof(FrameworkElement), typeof(DockAdorner), new PropertyMetadata(null));
        public FrameworkElement AdornedElement
            {
            get { return (FrameworkElement)GetValue(AdornedElementProperty); }
            set { SetValue(AdornedElementProperty, value); }
            }
        #endregion
        #region P:DockDirection:DockDirection
        public static readonly DependencyProperty DockDirectionProperty = DependencyProperty.Register("DockDirection", typeof(DockDirection), typeof(DockAdorner), new PropertyMetadata(DockDirection.Fill));
        public DockDirection DockDirection
            {
            get { return (DockDirection)GetValue(DockDirectionProperty); }
            set { SetValue(DockDirectionProperty, value); }
            }
        #endregion
        #region P:Orientation:Orientation?
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation?), typeof(DockAdorner), new PropertyMetadata(null));
        public Orientation? Orientation
            {
            get { return (Orientation?)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
            }
        #endregion
        #region P:AreOuterTargetsEnabled:Boolean
        public static readonly DependencyProperty AreOuterTargetsEnabledProperty = DependencyProperty.Register("AreOuterTargetsEnabled", typeof(Boolean), typeof(DockAdorner), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        public Boolean AreOuterTargetsEnabled
            {
            get { return (Boolean)GetValue(AreOuterTargetsEnabledProperty); }
            set { SetValue(AreOuterTargetsEnabledProperty, Boxes.Box(value)); }
            }
        #endregion
        #region P:AreInnerTargetsEnabled:Boolean
        public static readonly DependencyProperty AreInnerTargetsEnabledProperty = DependencyProperty.Register("AreInnerTargetsEnabled", typeof(Boolean), typeof(DockAdorner), new FrameworkPropertyMetadata(Boxes.BooleanTrue));
        public Boolean AreInnerTargetsEnabled
            {
            get { return (Boolean)GetValue(AreInnerTargetsEnabledProperty); }
            set { SetValue(AreInnerTargetsEnabledProperty, Boxes.Box(value)); }
            }
        #endregion
        #region P:IsInnerCenterTargetEnabled:Boolean
        public static readonly DependencyProperty IsInnerCenterTargetEnabledProperty = DependencyProperty.Register("IsInnerCenterTargetEnabled", typeof(Boolean), typeof(DockAdorner), new FrameworkPropertyMetadata(Boxes.BooleanTrue));
        public Boolean IsInnerCenterTargetEnabled
            {
            get { return (Boolean)GetValue(IsInnerCenterTargetEnabledProperty); }
            set { SetValue(IsInnerCenterTargetEnabledProperty, Boxes.Box(value)); }
            }
        #endregion
        #region P:AreInnerSideTargetsEnabled:Boolean
        public static readonly DependencyProperty AreInnerSideTargetsEnabledProperty = DependencyProperty.Register("AreInnerSideTargetsEnabled", typeof(Boolean), typeof(DockAdorner), new FrameworkPropertyMetadata(Boxes.BooleanTrue));
        public Boolean AreInnerSideTargetsEnabled
            {
            get { return (Boolean)GetValue(AreInnerSideTargetsEnabledProperty); }
            set { SetValue(AreInnerSideTargetsEnabledProperty, Boxes.Box(value)); }
            }
        #endregion

        public void UpdateContent()
            {
            UpdateContentCore();
            InvalidateArrange();
            UpdateLayout();
            }

        protected virtual void UpdateContentCore()
            {
            }
        }
    }