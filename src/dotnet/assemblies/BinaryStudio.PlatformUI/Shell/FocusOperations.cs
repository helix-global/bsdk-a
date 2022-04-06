using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.PlatformUI.Shell;

namespace BinaryStudio.PlatformUI
    {
    /// <summary>
    /// Manages focus tasks.
    /// </summary>
    public static class FocusOperations
        {
        #region P:FocusHelper.FocusTarget:UIElement
        /// <summary>
        /// Gets or sets a focus target for an element. The focus target is automatically focused whenever the DependencyObject that the target is attached to receives direct keyboard focus.
        /// </summary>
        public static readonly DependencyProperty FocusTargetProperty = DependencyProperty.RegisterAttached("FocusTarget", typeof(UIElement), typeof(FocusOperations), new FrameworkPropertyMetadata(null, OnFocusTargetChanged));
        private static void OnFocusTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var source = (UIElement)d;
            if (e.NewValue == null) {
                if (e.OldValue == null) { return; }
                //WeakEventManager<UIElement, KeyboardFocusChangedEventArgs>.RemoveHandler(source, "GotKeyboardFocus", OnGotKeyboardFocus);
                }
            else
                {
                if (e.OldValue != null) { return; }
                //WeakEventManager<UIElement, KeyboardFocusChangedEventArgs>.AddHandler(source, "GotKeyboardFocus", OnGotKeyboardFocus);
                }
            }
        private static void OnGotKeyboardFocus(Object sender, KeyboardFocusChangedEventArgs e) {
            var element = (UIElement)sender;
            if (e.OriginalSource != element) { return; }
            MoveFocusInto(GetFocusTarget(element));
            }

        /// <summary>
        /// Gets the focus target for an element. The focus target is automatically focused whenever the DependencyObject that the target is attached to receives direct keyboard focus.
        /// </summary>
        /// <param name="element">The element which should have a focus target attached to it.</param>
        /// <returns>The current focus target for the element.</returns>
        public static UIElement GetFocusTarget(UIElement element) {
            Validate.IsNotNull(element, "element");
            return (UIElement)element.GetValue(FocusTargetProperty);
            }

        /// <summary>
        /// Sets the focus target for an element. The focus target is automatically focused whenever the DependencyObject that the target is attached to receives direct keyboard focus.
        /// </summary>
        /// <param name="element">The element that should have a focus target attached to it.</param>
        /// <param name="value">The new focus target for the element.</param>
        public static void SetFocusTarget(UIElement element, UIElement value) {
            Validate.IsNotNull(element, "element");
            element.SetValue(FocusTargetProperty, value);
            }
        #endregion

        public static readonly DependencyProperty FocusedProperty = DependencyProperty.RegisterAttached("Focused", typeof(Boolean), typeof(FocusOperations), new PropertyMetadata(default(Boolean)));
        public static void SetFocused(DependencyObject element, Boolean value)
            {
            element.SetValue(FocusedProperty, value);
            }

        public static Boolean GetFocused(DependencyObject element)
            {
            return (Boolean) element.GetValue(FocusedProperty);
            }

        /// <summary>
        /// Either sends focus to the <see cref="T:System.Windows.FrameworkElement" /> immediately or delays focusing until the <see cref="T:System.Windows.FrameworkElement" /> is loaded. The last element pending focus on Loaded will be focused and all previous <see cref="T:System.Windows.FrameworkElement" />s will not be focused.
        /// </summary>
        /// <param name="element">The element to focus.</param>
        public static void FocusPossiblyUnloadedElement(FrameworkElement element) {
            Validate.IsNotNull(element, "element");
            PendingFocusHelper.SetFocusOnLoad(element, null);
            }

        /// <summary>
        /// Uses the <see cref="M:System.Windows.UIElement.MoveFocus(System.Windows.Input.TraversalRequest)" /> method to try to move WPF focus to the first valid focusable element inside the given <see cref="T:System.Windows.UIElement" />, after first enduring that WPF will not attempt to change focus because of a cross- <see cref="T:System.Windows.Interop.HwndSource" /> focus change.
        /// </summary>
        /// <param name="element">The element to move focus into.</param>
        public static void MoveFocusInto(UIElement element) {
            if (IsKeyboardFocusWithin(element))
                return;
            element.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }

        #region M:IsKeyboardFocusWithin(UIElement):Boolean
        /// <summary>
        /// Determines whether WPF or Win32 keyboard focus is within the specified element.
        /// </summary>
        /// <param name="element">The element to check.</param>
        /// <returns>Returns true if the focus is within the element; otherwise returns false.</returns>
        public static Boolean IsKeyboardFocusWithin(UIElement element) {
            if (element.IsKeyboardFocusWithin) { return true; }
            var handle = NativeMethods.GetFocus();
            return element.FindDescendants<HwndHost>().Any(i => IsChildOrSame(i.Handle, handle));
            }
        #endregion
        #region M:IsKeyboardFocusWithin(IntPtr):Boolean
        /// <summary>
        /// Determines whether WPF or Win32 keyboard focus is within the specified HWND.
        /// </summary>
        /// <param name="handle">The HWND which may have or contain the focus.</param>
        /// <returns>Returns true if the focus is within or contained by the HWND; otherwise returns false.</returns>
        public static Boolean IsKeyboardFocusWithin(IntPtr handle) {
            return IsChildOrSame(handle, NativeMethods.GetFocus());
            }
        #endregion

        private static Boolean IsChildOrSame(IntPtr hwndParent, IntPtr hwndChild) {
            return hwndParent == hwndChild || NativeMethods.IsChild(hwndParent, hwndChild);
            }
        }
    }