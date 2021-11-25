using System;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class SplitterItemsControl : LayoutSynchronizedItemsControl
        {
        static SplitterItemsControl()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterItemsControl), new FrameworkPropertyMetadata(typeof(SplitterItemsControl)));
            }

        #region P:Orientation:Orientation
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SplitterItemsControl), new PropertyMetadata(default(Orientation), OnOrientationChanged));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            ((SplitterItemsControl)d).OnOrientationChanged();
            }

        protected virtual void OnOrientationChanged() { }

        public Orientation Orientation {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
            }
        #endregion
        #region P:SplitterItemsControl.SplitterGripSize:Double
        public static readonly DependencyProperty SplitterGripSizeProperty = DependencyProperty.RegisterAttached("SplitterGripSize", typeof(Double), typeof(SplitterItemsControl), new PropertyMetadata(5.0));
        public static void SetSplitterGripSize(DependencyObject source, Double value) {
            source.SetValue(SplitterGripSizeProperty, value);
            }
        public static Double GetSplitterGripSize(DependencyObject source) {
            return (Double)source.GetValue(SplitterGripSizeProperty);
            }
        #endregion
        #region P:SplitterItemsControl.IsFirst:Boolean
        private static readonly DependencyPropertyKey IsFirstPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsFirst", typeof(Boolean), typeof(SplitterItemsControl), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsFirstProperty = IsFirstPropertyKey.DependencyProperty;
        public static Boolean GetIsFirst(UIElement source) {
            return (Boolean)source.GetValue(IsFirstProperty);
            }
        internal static void SetIsFirst(UIElement source, Boolean value) {
            source.SetValue(IsFirstPropertyKey, value);
            }
        #endregion
        #region P:SplitterItemsControl.IsLast:Boolean
        private static readonly DependencyPropertyKey IsLastPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsLast", typeof(Boolean), typeof(SplitterItemsControl), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty IsLastProperty = IsLastPropertyKey.DependencyProperty;
        public static Boolean GetIsLast(DependencyObject source) {
            return (Boolean)source.GetValue(IsLastProperty);
            }
        internal static void SetIsLast(DependencyObject source, Boolean value) {
            source.SetValue(IsLastPropertyKey, value);
            }
        #endregion

        #region M:PrepareContainerForItemOverride(DependencyObject,Object)
        protected override void PrepareContainerForItemOverride(DependencyObject element, Object item)
            {
            base.PrepareContainerForItemOverride(element, item);
            }
        #endregion
        #region M:IsItemItsOwnContainerOverride(Object):Boolean
        protected override Boolean IsItemItsOwnContainerOverride(Object item) {
            return item is SplitterItem;
            }
        #endregion
        #region M:GetContainerForItemOverride:DependencyObject
        protected override DependencyObject GetContainerForItemOverride() {
            return new SplitterItem();
            }
        #endregion
        }
    }
