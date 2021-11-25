using System;
using System.Windows;
using BinaryStudio.PlatformUI.Controls;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class DockGroupAdorner : DockAdorner
        {
        #region P:DockSiteType:DockSiteType
        public static readonly DependencyProperty DockSiteTypeProperty = DependencyProperty.Register("DockSiteType", typeof(DockSiteType), typeof(DockGroupAdorner), new PropertyMetadata(DockSiteType.Default));
        public DockSiteType DockSiteType
            {
            get { return (DockSiteType)GetValue(DockSiteTypeProperty); }
            set { SetValue(DockSiteTypeProperty, value); }
            }
        #endregion
        #region P:IsFirst:Boolean
        public static readonly DependencyProperty IsFirstProperty = DependencyProperty.Register("IsFirst", typeof(Boolean), typeof(DockGroupAdorner), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        public Boolean IsFirst
            {
            get { return (Boolean)GetValue(IsFirstProperty); }
            set { SetValue(IsFirstProperty, Boxes.Box(value)); }
            }
        #endregion
        #region P:IsLast:Boolean
        public static readonly DependencyProperty IsLastProperty = DependencyProperty.Register("IsLast", typeof(Boolean), typeof(DockGroupAdorner), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
        public Boolean IsLast
            {
            get { return (Boolean)GetValue(IsLastProperty); }
            set { SetValue(IsLastProperty, Boxes.Box(value)); }
            }
        #endregion

        static DockGroupAdorner()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DockGroupAdorner), new FrameworkPropertyMetadata(typeof(DockGroupAdorner)));
            }

        protected override void UpdateContentCore()
            {
            base.UpdateContentCore();
            var adornedElement = AdornedElement as DockTarget;
            if (adornedElement == null) { return; }
            DockSiteType = adornedElement.DockSiteType;
            var ancestor = adornedElement.FindAncestor<SplitterItem>();
            if (ancestor == null) { return; }
            IsFirst = SplitterPanel.GetIsFirst(ancestor);
            IsLast = SplitterPanel.GetIsLast(ancestor);
            }
        }
    }