using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    [TemplatePart(Name = "PART_AutoHideSlideout", Type = typeof(Canvas))]
    public class AutoHideChannelControl : LayoutSynchronizedItemsControl
        {
        public static readonly RoutedEvent AutoHideChannelContextMenuEvent = EventManager.RegisterRoutedEvent("AutoHideChannelContextMenu", RoutingStrategy.Bubble, typeof(EventHandler<AutoHideChannelContextMenuEventArgs>), typeof(AutoHideChannelControl));

        #region P:AutoHideSlideout:Object
        public static readonly DependencyProperty AutoHideSlideoutProperty = DependencyProperty.Register("AutoHideSlideout", typeof(Object), typeof(AutoHideChannelControl));
        public Object AutoHideSlideout
            {
            get { return GetValue(AutoHideSlideoutProperty); }
            set { SetValue(AutoHideSlideoutProperty, value); }
            }
        #endregion

        static AutoHideChannelControl()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoHideChannelControl), new FrameworkPropertyMetadata(typeof(AutoHideChannelControl)));
            }

        #region P:AutoHideChannelControl.Orientation:Orientation
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.RegisterAttached("Orientation", typeof(Orientation), typeof(AutoHideChannelControl), new FrameworkPropertyMetadata(Orientation.Vertical, FrameworkPropertyMetadataOptions.Inherits));
        public static Orientation GetOrientation(DependencyObject element)
            {
            if (element == null) { throw new ArgumentNullException(nameof(element)); }
            return (Orientation)element.GetValue(OrientationProperty);
            }

        public static void SetOrientation(DependencyObject element, Orientation value)
            {
            if (element == null) { throw new ArgumentNullException(nameof(element)); }
            element.SetValue(OrientationProperty, value);
            }
        #endregion
        #region P:AutoHideChannelControl.ChannelDock:Dock
        public static readonly DependencyProperty ChannelDockProperty = DependencyProperty.RegisterAttached("ChannelDock", typeof(Dock), typeof(AutoHideChannelControl), new FrameworkPropertyMetadata(Dock.Left, FrameworkPropertyMetadataOptions.Inherits));
        public static Dock GetChannelDock(UIElement element)
            {
            if (element == null) { throw new ArgumentNullException(nameof(element)); }
            return (Dock)element.GetValue(OrientationProperty);
            }

        public static void SetChannelDock(UIElement element, Dock value)
            {
            if (element == null) { throw new ArgumentNullException(nameof(element)); }
            element.SetValue(OrientationProperty, value);
            }
        #endregion

        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
            {
            var templateChild = GetTemplateChild("PART_AutoHideSlideout") as UIElement;
            var source = (Visual)e.Source;
            if (templateChild == null || templateChild.IsAncestorOf(source))
                return;
            e.Handled = true;
            var channelPoint = ((Visual)e.OriginalSource).TransformToVisual(this).Transform(new Point(e.CursorLeft, e.CursorTop));
            RaiseEvent(new AutoHideChannelContextMenuEventArgs(AutoHideChannelContextMenuEvent, channelPoint));
            }

        #region M:PrepareContainerForItemOverride(DependencyObject,Object)
        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, Object item) {
            base.PrepareContainerForItemOverride(element, item);
            var i = item as DependencyObject;
            if (i != null)
                {
                SetOrientation(i, GetOrientation(this));
                }
            }
        #endregion
        }
    }
