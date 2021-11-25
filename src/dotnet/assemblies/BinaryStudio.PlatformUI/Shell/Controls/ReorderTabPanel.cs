using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class ReorderTabPanel : Panel, IWeakEventListener
        {
        public static readonly RoutedEvent PanelLayoutUpdatedEvent = EventManager.RegisterRoutedEvent("PanelLayoutUpdated", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(ReorderTabPanel));
        private Boolean layoutUpdatedHandlerAdded;

        #region P:IsNotificationNeeded:Boolean
        public Boolean IsNotificationNeeded {
            get { return layoutUpdatedHandlerAdded; }
            set {
                if (value == layoutUpdatedHandlerAdded) { return; }
                if (value)
                    {
                    LayoutUpdated += OnLayoutUpdated;
                    }
                else
                    {
                    LayoutUpdated -= OnLayoutUpdated;
                    }
                layoutUpdatedHandlerAdded = value;
                }
            }
        #endregion
        #region P:IsVerticallyOriented:Boolean
        internal Boolean IsVerticallyOriented { get {
            return (HasLogicalOrientation) && (LogicalOrientation == Orientation.Vertical);
            }}
        #endregion
        #region P:ExpandedTearOffMargin:Thickness
        public static readonly DependencyProperty ExpandedTearOffMarginProperty = DependencyProperty.Register("ExpandedTearOffMargin", typeof(Thickness), typeof(ReorderTabPanel), new FrameworkPropertyMetadata(new Thickness(0.0)));
        public Thickness ExpandedTearOffMargin {
            get { return (Thickness)GetValue(ExpandedTearOffMarginProperty); }
            set { SetValue(ExpandedTearOffMarginProperty, value); }
            }
        #endregion
        #region P:UndockingOffset:Double
        protected Double UndockingOffset { get {
            return FloatingWindow.GetIsUndockingTab(this)
                ? DockManager.Instance.UndockedTabItemOffset
                : 0.0;
                }}
        #endregion
        #region P:UndockingLength:Double
        protected Double UndockingLength { get {
            return FloatingWindow.GetIsUndockingTab(this)
                ? DockManager.Instance.UndockedTabItemLength
                : 0.0;
                }}
        #endregion

        public ReorderTabPanel() {
            layoutUpdatedHandlerAdded = false;
            DataContextChanged += OnDataContextChanged;
            }

        private void OnLayoutUpdated(Object sender, EventArgs e) {
            RaiseEvent(new RoutedEventArgs(PanelLayoutUpdatedEvent));
            IsNotificationNeeded = false;
            }

        protected static View GetView(UIElement child) {
            var frameworkElement = child as FrameworkElement;
            if (frameworkElement != null) { return frameworkElement.DataContext as View; }
            return null;
            }

        protected virtual void OnDataContextChanged(Object sender, DependencyPropertyChangedEventArgs e) {
            var oldValue = e.OldValue as NestedGroup;
            if (oldValue != null) { CollectionChangedEventManager.RemoveListener(oldValue.Children, this); }
            var newValue = e.NewValue as NestedGroup;
            if (newValue == null) { return; }
            CollectionChangedEventManager.AddListener(newValue.Children, this);
            }

        protected virtual void OnDependentCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
            InvalidateMeasure();
            }

        Boolean IWeakEventListener.ReceiveWeakEvent(Type managerType, Object sender, EventArgs e) {
            if (!(managerType == typeof(CollectionChangedEventManager))) { return false; }
            OnDependentCollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);
            return true;
            }
        }
    }
