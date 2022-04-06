using System;
using System.Windows;
using System.Windows.Threading;
using BinaryStudio.PlatformUI.Shell;

namespace BinaryStudio.PlatformUI
    {
    public static class PendingFocusHelper
        {
        public static readonly DispatcherPriority FocusPriority = DispatcherPriority.Loaded;
        private static FrameworkElement pendingFocusElement = null;

        private static Action<FrameworkElement> PendingFocusAction { get; set; }

        private static FrameworkElement PendingFocusElement
            {
            get
                {
                return pendingFocusElement;
                }
            set
                {
                if (pendingFocusElement == value)
                    return;
                if (pendingFocusElement != null)
                    pendingFocusElement.Loaded -= OnPendingFocusElementLoaded;
                pendingFocusElement = value;
                if (pendingFocusElement == null)
                    return;
                pendingFocusElement.Loaded += OnPendingFocusElementLoaded;
                }
            }

        private static void OnPendingFocusElementLoaded(Object sender, RoutedEventArgs args)
            {
            var pendingFocusElement = PendingFocusElement;
            if (pendingFocusElement != null)
                MoveFocusInto(pendingFocusElement, PendingFocusAction);
            PendingFocusElement = null;
            PendingFocusAction = null;
            }

        private static void MoveFocusInto(FrameworkElement element, Action<FrameworkElement> focusAction)
            {
            if (focusAction != null)
                focusAction(element);
            else
                FocusOperations.MoveFocusInto(element);
            }

        public static void SetFocusOnLoad(FrameworkElement element, Action<FrameworkElement> focusAction = null)
            {
            Validate.IsNotNull(element, "element");
            if (element.IsLoaded && Extensions.Extensions.IsConnectedToPresentationSource(element))
                {
                PendingFocusElement = null;
                PendingFocusAction = null;
                MoveFocusInto(element, focusAction);
                }
            else
                {
                PendingFocusElement = element;
                PendingFocusAction = focusAction;
                }
            }
        }
    }