using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class DropDownButton : MenuItem
        {
        static DropDownButton()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropDownButton), new FrameworkPropertyMetadata(typeof(DropDownButton)));
            EventManager.RegisterClassHandler(typeof(DropDownButton), Mouse.PreviewMouseDownEvent, new MouseButtonEventHandler(OnPreviewMouseButtonDown));
            EventManager.RegisterClassHandler(typeof(DropDownButton), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
            }

        #region P:Content:Object
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(Object), typeof(DropDownButton), new PropertyMetadata(default(Object)));
        public Object Content
            {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
            }
        #endregion
        //#region P:IsPressed:Boolean
        //public static readonly DependencyProperty IsPressedProperty = DependencyProperty.Register("IsPressed", typeof(Boolean), typeof(DropDownButton), new PropertyMetadata(default(Boolean)));
        //public Boolean IsPressed
        //    {
        //    get { return (Boolean)GetValue(IsPressedProperty); }
        //    set { SetValue(IsPressedProperty, value); }
        //    }
        //#endregion
        #region P:IsCancel:Boolean
        public static readonly DependencyProperty IsCancelProperty = DependencyProperty.Register("IsCancel", typeof(Boolean), typeof(DropDownButton), new PropertyMetadata(default(Boolean)));
        public Boolean IsCancel
            {
            get { return (Boolean)GetValue(IsCancelProperty); }
            set { SetValue(IsCancelProperty, value); }
            }
        #endregion
        #region P:IsDefault:Boolean
        public static readonly DependencyProperty IsDefaultProperty = DependencyProperty.Register("IsDefault", typeof(Boolean), typeof(DropDownButton), new PropertyMetadata(default(Boolean)));
        public Boolean IsDefault
            {
            get { return (Boolean)GetValue(IsDefaultProperty); }
            set { SetValue(IsDefaultProperty, value); }
            }
        #endregion
        #region P:IsDropDownOpen:Boolean
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(Boolean), typeof(DropDownButton), new PropertyMetadata(default(Boolean), OnIsDropDownOpenChanged));
        private static void OnIsDropDownOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is DropDownButton source)
                {
                source.OnIsDropDownOpenChanged(e);
                }
            }

        private void OnIsDropDownOpenChanged(DependencyPropertyChangedEventArgs e) {
            var flag = (Boolean)e.NewValue;
            if (flag)
                {
                Mouse.Capture(this, CaptureMode.SubTree);
                }
            else
                {
                if (HasCapture)
                    {
                    Mouse.Capture(null);
                    }
                }
            }

        public Boolean IsDropDownOpen
            {
            get { return (Boolean)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
            }
        #endregion

        private Boolean HasCapture
            {
            get
                {
                return ReferenceEquals(Mouse.Captured, this);
                }
            }

        public void Close()
            {
            IsDropDownOpen = false;
            }

        public override void OnApplyTemplate()
            {
            base.OnApplyTemplate();
            this.SetBinding(IsDropDownOpenProperty, ContextMenu, ContextMenu.IsOpenProperty, BindingMode.TwoWay);
            if (Popup == null) {
                Popup = GetTemplateChild("Popup") as Popup;
                if (Popup != null) {
                    Popup.Closed += OnPopupClosed;
                    //ToggleButton.Checked   += OnToggleButtonChecked;
                    //ToggleButton.Unchecked += OnToggleButtonUnchecked;
                    }
                }
            }

        private void OnPopupClosed(Object sender, EventArgs e)
            {
            Close();
            }

        private void OnToggleButtonChecked(Object sender, RoutedEventArgs e)
            {
            IsDropDownOpen = true;
            }

        private void OnToggleButtonUnchecked(Object sender, RoutedEventArgs e)
            {
            Close();
            }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {
            //Debug.Print($"OnPreviewMouseDown:{e.OriginalSource}");
            }

        private static void OnPreviewMouseButtonDown(Object sender, MouseButtonEventArgs e) {
            if (sender is DropDownButton source) {
                if (ReferenceEquals(sender, e.OriginalSource)) {
                    if (source.IsDropDownOpen)
                        {
                        e.Handled = true;
                        source.Close();
                        }
                    }
                }
            }

        protected override void OnMouseDown(MouseButtonEventArgs e)
            {
            base.OnMouseDown(e);
            }

        private static void OnLostMouseCapture(Object sender, MouseEventArgs e)
            {
            Debug.Print($"OnLostMouseCapture:{sender}:{e.OriginalSource}");
            }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
            {
            base.OnPreviewMouseRightButtonDown(e);
            e.Handled = true;
            }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
            base.OnIsKeyboardFocusWithinChanged(e);
            Debug.Print($"OnIsKeyboardFocusWithinChanged:{IsKeyboardFocusWithin}");
            if (IsDropDownOpen && !IsKeyboardFocusWithin) {
                DependencyObject dependencyObject = Keyboard.FocusedElement as DependencyObject;
                if (dependencyObject == null)
                    {
                    Close();
                    }
                }
            }
        private Popup Popup;
        }
    }
