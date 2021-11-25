using System;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class ViewPresenter : LayoutSynchronizedContentControl
        {
        public static readonly RoutedEvent ContentShowingEvent = EventManager.RegisterRoutedEvent("ContentShowing", RoutingStrategy.Direct, typeof(EventHandler<ViewEventArgs>), typeof(ViewPresenter));
        public static readonly RoutedEvent ContentHidingEvent = EventManager.RegisterRoutedEvent("ContentHiding", RoutingStrategy.Direct, typeof(EventHandler<ViewEventArgs>), typeof(ViewPresenter));
        private DependencyObject currentFocusScope;

        #region P:View:View
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register("View", typeof(View), typeof(ViewPresenter));
        public View View {
            get { return (View)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
            }
        #endregion
        #region P:ContainingElement:ViewElement
        public static readonly DependencyProperty ContainingElementProperty = DependencyProperty.Register("ContainingElement", typeof(ViewElement), typeof(ViewPresenter));
        public ViewElement ContainingElement {
            get { return (ViewElement)GetValue(ContainingElementProperty); }
            set { SetValue(ContainingElementProperty, value); }
            }
        #endregion
        #region P:ViewPresenter.CanActivateFromLeftClick:Boolean
        public static readonly DependencyProperty CanActivateFromLeftClickProperty = DependencyProperty.RegisterAttached("CanActivateFromLeftClick", typeof(Boolean), typeof(ViewPresenter), new FrameworkPropertyMetadata(Boxes.BooleanTrue));
        public static Boolean GetCanActivateFromLeftClick(DependencyObject element) {
            Validate.IsNotNull(element, "element");
            return (Boolean)element.GetValue(CanActivateFromLeftClickProperty);
            }

        public static void SetCanActivateFromLeftClick(DependencyObject element, Boolean value) {
            Validate.IsNotNull(element, "element");
            element.SetValue(CanActivateFromLeftClickProperty, Boxes.Box(value));
            }
        #endregion
        #region P:ViewPresenter.CanActivateFromMiddleClick:Boolean
        public static readonly DependencyProperty CanActivateFromMiddleClickProperty = DependencyProperty.RegisterAttached("CanActivateFromMiddleClick", typeof(Boolean), typeof(ViewPresenter), new FrameworkPropertyMetadata(Boxes.BooleanTrue));
        public static Boolean GetCanActivateFromMiddleClick(DependencyObject element) {
            Validate.IsNotNull(element, "element");
            return (Boolean)element.GetValue(CanActivateFromMiddleClickProperty);
            }

        public static void SetCanActivateFromMiddleClick(DependencyObject element, Boolean value) {
            Validate.IsNotNull(element, "element");
            element.SetValue(CanActivateFromMiddleClickProperty, Boxes.Box(value));
            }
        #endregion

        static ViewPresenter() {
            IsTabStopProperty.OverrideMetadata(typeof(ViewPresenter), new FrameworkPropertyMetadata(Boxes.BooleanFalse));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(ViewPresenter), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
            }

        public ViewPresenter() {
            IsVisibleChanged += OnIsVisibleChanged;
            UtilityMethods.AddPresentationSourceCleanupAction(this, () => {
                Content = null;
                if (currentFocusScope == null)
                    return;
                FocusManager.SetFocusedElement(currentFocusScope, null);
            });
            AccessKeyManager.AddAccessKeyPressedHandler(this, OnAccessKeyPressed);
            }

        private void OnAccessKeyPressed(Object sender, AccessKeyPressedEventArgs e) {
            e.Scope = View;
            e.Handled = true;
            }

        #region M:OnPropertyChanged(DependencyPropertyChangedEventArgs)
        /// <summary>Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement" /> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)" />.</summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);
            if (e.Property != DataContextProperty || !IsVisible) { return; }
            var oldValue = e.OldValue as View;
            var newValue = e.NewValue as View;
            if (oldValue != null) { AsyncRaiseEvent(new ViewEventArgs(ContentHidingEvent, oldValue)); }
            if (newValue == null) { return; }
            AsyncRaiseEvent(new ViewEventArgs(ContentShowingEvent, newValue));
            }
        #endregion
        #region M:OnCreateAutomationPeer:AutomationPeer
        /// <summary>Returns class-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementations for the Windows Presentation Foundation (WPF) infrastructure.</summary>
        /// <returns>The type-specific <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> implementation.</returns>
        protected override AutomationPeer OnCreateAutomationPeer() {
            return new ViewPresenterAutomationPeer(this);
            }
        #endregion
        #region M:OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs)
        /// <summary>Invoked just before the <see cref="E:System.Windows.UIElement.IsKeyboardFocusWithinChanged" /> event is raised by this element. Implement this method to add class handling for this event. </summary>
        /// <param name="e">A <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" /> that contains the event data.</param>
        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
            if (!IsKeyboardFocusWithin) { return; }
            currentFocusScope = FocusManager.GetFocusScope(this);
            }
        #endregion

        private void AsyncRaiseEvent(RoutedEventArgs args) {
            Dispatcher.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback(delegate (Object arg) {
                if (this.IsConnectedToPresentationSource()) {
                    RaiseEvent((RoutedEventArgs)arg);
                    }
                return null;
                }), args);
            }

        private void OnIsVisibleChanged(Object sender, DependencyPropertyChangedEventArgs args) {
            var newValue = (Boolean)args.NewValue;
            var dataContext = DataContext as View;
            if (dataContext == null) { return; }
            AsyncRaiseEvent(new ViewEventArgs(newValue ? ContentShowingEvent : ContentHidingEvent, dataContext));
            }
        }
    }
