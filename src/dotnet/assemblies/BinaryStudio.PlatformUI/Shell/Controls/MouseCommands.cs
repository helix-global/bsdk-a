using System;
using System.Windows;
using System.Windows.Input;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public static class MouseCommands
        {
        public static readonly DependencyProperty MouseCommandActionProperty = DependencyProperty.RegisterAttached("MouseCommandAction", typeof(MouseAction), typeof(MouseCommands), new FrameworkPropertyMetadata(MouseAction.LeftClick, OnMouseCommandChanged));
        public static readonly DependencyProperty MouseCommandProperty = DependencyProperty.RegisterAttached("MouseCommand", typeof(ICommand), typeof(MouseCommands), new FrameworkPropertyMetadata(OnMouseCommandChanged));
        public static readonly DependencyProperty MouseCommandParameterProperty = DependencyProperty.RegisterAttached("MouseCommandParameter", typeof(Object), typeof(MouseCommands), new FrameworkPropertyMetadata(OnMouseCommandChanged));
        public static readonly DependencyProperty ControlMouseCommandProperty = DependencyProperty.RegisterAttached("ControlMouseCommand", typeof(ICommand), typeof(MouseCommands), new FrameworkPropertyMetadata(OnMouseCommandChanged));
        public static readonly DependencyProperty ControlMouseCommandParameterProperty = DependencyProperty.RegisterAttached("ControlMouseCommandParameter", typeof(Object), typeof(MouseCommands), new FrameworkPropertyMetadata(OnMouseCommandChanged));
        public static readonly DependencyProperty ShiftMouseCommandProperty = DependencyProperty.RegisterAttached("ShiftMouseCommand", typeof(ICommand), typeof(MouseCommands), new FrameworkPropertyMetadata(OnMouseCommandChanged));
        public static readonly DependencyProperty ShiftMouseCommandParameterProperty = DependencyProperty.RegisterAttached("ShiftMouseCommandParameter", typeof(Object), typeof(MouseCommands), new FrameworkPropertyMetadata(OnMouseCommandChanged));

        public static MouseAction GetMouseCommandAction(UIElement element)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (MouseAction)element.GetValue(MouseCommandActionProperty);
            }

        public static void SetMouseCommandAction(UIElement element, MouseAction value)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(MouseCommandActionProperty, value);
            }

        public static ICommand GetMouseCommand(UIElement element)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (ICommand)element.GetValue(MouseCommandProperty);
            }

        public static void SetMouseCommand(UIElement element, ICommand value)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(MouseCommandProperty, value);
            }

        public static Object GetMouseCommandParameter(UIElement element)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return element.GetValue(MouseCommandParameterProperty);
            }

        public static void SetMouseCommandParameter(UIElement element, Object value)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(MouseCommandParameterProperty, value);
            }

        public static ICommand GetControlMouseCommand(UIElement element)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (ICommand)element.GetValue(ControlMouseCommandProperty);
            }

        public static void SetControlMouseCommand(UIElement element, ICommand value)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(ControlMouseCommandProperty, value);
            }

        public static Object GetControlMouseCommandParameter(UIElement element)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return element.GetValue(ControlMouseCommandParameterProperty);
            }

        public static void SetControlMouseCommandParameter(UIElement element, Object value)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(ControlMouseCommandParameterProperty, value);
            }

        public static ICommand GetShiftMouseCommand(UIElement element)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (ICommand)element.GetValue(ShiftMouseCommandProperty);
            }

        public static void SetShiftMouseCommand(UIElement element, Object value)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(ShiftMouseCommandProperty, value);
            }

        public static Object GetShiftMouseCommandParameter(UIElement element)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return element.GetValue(ShiftMouseCommandParameterProperty);
            }

        public static void SetShiftMouseCommandParameter(UIElement element, Object value)
            {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(ShiftMouseCommandParameterProperty, value);
            }

        private static void OnMouseCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
            {
            RefreshMouseBinding((UIElement)obj);
            }

        private static void RefreshMouseBinding(UIElement element)
            {
            if (!HasMouseCommand(element))
                return;
            var flag = false;
            foreach (InputBinding inputBinding in element.InputBindings)
                {
                var mouseBinding = inputBinding as MouseBinding;
                if (mouseBinding != null)
                    {
                    UpdateMouseBinding(element, mouseBinding);
                    flag = true;
                    }
                }
            if (flag)
                return;
            AddMouseBindings(element);
            }

        private static Boolean HasMouseCommand(UIElement element)
            {
            return GetMouseCommand(element) != null || GetControlMouseCommand(element) != null || GetShiftMouseCommand(element) != null;
            }

        private static void UpdateMouseBinding(UIElement element, MouseBinding mouseBinding)
            {
            ((MouseGesture)mouseBinding.Gesture).MouseAction = GetMouseCommandAction(element);
            mouseBinding.Command = KeyboardModifierChain.Instance;
            mouseBinding.CommandParameter = element;
            }

        private static void AddMouseBindings(UIElement element)
            {
            var mouseCommandAction = GetMouseCommandAction(element);
            var mouseBinding1 = new MouseBinding();
            var mouseGesture1 = new MouseGesture(mouseCommandAction, ModifierKeys.None);
            mouseBinding1.Gesture = mouseGesture1;
            var instance1 = KeyboardModifierChain.Instance;
            mouseBinding1.Command = instance1;
            var uiElement1 = element;
            mouseBinding1.CommandParameter = uiElement1;
            var mouseBinding2 = mouseBinding1;
            element.InputBindings.Add(mouseBinding2);
            var mouseBinding3 = new MouseBinding();
            var mouseGesture2 = new MouseGesture(mouseCommandAction, ModifierKeys.Control);
            mouseBinding3.Gesture = mouseGesture2;
            var instance2 = KeyboardModifierChain.Instance;
            mouseBinding3.Command = instance2;
            var uiElement2 = element;
            mouseBinding3.CommandParameter = uiElement2;
            var mouseBinding4 = mouseBinding3;
            element.InputBindings.Add(mouseBinding4);
            var mouseBinding5 = new MouseBinding();
            var mouseGesture3 = new MouseGesture(mouseCommandAction, ModifierKeys.Shift);
            mouseBinding5.Gesture = mouseGesture3;
            var instance3 = KeyboardModifierChain.Instance;
            mouseBinding5.Command = instance3;
            var uiElement3 = element;
            mouseBinding5.CommandParameter = uiElement3;
            var mouseBinding6 = mouseBinding5;
            element.InputBindings.Add(mouseBinding6);
            }

        private class KeyboardModifierChain : ICommand
            {
            private static KeyboardModifierChain instance;

            public static KeyboardModifierChain Instance
                {
                get
                    {
                    return instance ?? (instance = new KeyboardModifierChain());
                    }
                }

            event EventHandler ICommand.CanExecuteChanged
                {
                add
                    {
                    }
                remove
                    {
                    }
                }

            public Boolean CanExecute(Object parameter)
                {
                return true;
                }

            public void Execute(Object parameter)
                {
                var element = parameter as UIElement;
                if (element == null)
                    return;
                var modifierKeys = NativeMethods.ModifierKeys;
                if (modifierKeys == ModifierKeys.Control && ExecuteCommand(element, GetControlMouseCommand, GetControlMouseCommandParameter) || modifierKeys == ModifierKeys.Shift && ExecuteCommand(element, GetShiftMouseCommand, GetShiftMouseCommandParameter))
                    return;
                ExecuteCommand(element, GetMouseCommand, GetMouseCommandParameter);
                }

            private Boolean ExecuteCommand(UIElement element, Func<UIElement, ICommand> commandAccessor, Func<UIElement, Object> commandParameterAccessor)
                {
                var command = commandAccessor(element);
                if (command == null)
                    return false;
                var parameter = commandParameterAccessor(element);
                var routedCommand = command as RoutedCommand;
                if (routedCommand != null)
                    routedCommand.Execute(parameter, element);
                else
                    command.Execute(parameter);
                return true;
                }
            }
        }
    }