using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BinaryStudio.PlatformUI.Extensions;

namespace BinaryStudio.PlatformUI.Controls.Internal
    {
        public class GridEntryContentTextBox : TextBox
        {
        static GridEntryContentTextBox()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridEntryContentTextBox), new FrameworkPropertyMetadata(typeof(GridEntryContentTextBox)));
            }

        private ContentPresenter DecoratorPresenter;
        private FrameworkElement Host;

        public static readonly DependencyProperty DecoratorProperty = DependencyProperty.Register("Decorator", typeof(Object), typeof(GridEntryContentTextBox), new PropertyMetadata(default(Object)));
        public Object Decorator {
            get { return (Object)GetValue(DecoratorProperty); }
            set { SetValue(DecoratorProperty, value); }
            }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            DecoratorPresenter = GetTemplateChild("DecoratorPresenter") as ContentPresenter;
            Host = GetTemplateChild("Host") as FrameworkElement;
            if ((DecoratorPresenter != null) && (Host != null)) {
                DecoratorPresenter.MouseLeftButtonDown += OnMouseLeftButtonDown;
                if (Decorator != null) {
                    IsDecorating = true;
                    }
                }
            }


        private void OnMouseLeftButtonDown(Object sender, MouseButtonEventArgs e) {
            IsDecorating = false;
            }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            base.OnLostKeyboardFocus(e);
            if (ReferenceEquals(e.Source, this)) {
                if (Host != null) {
                    if (!Host.IsKeyboardFocusWithin) {
                        IsDecorating = true;
                        }
                    }
                }
            }

        private static readonly DependencyPropertyKey IsDecoratingPropertyKey = DependencyProperty.RegisterReadOnly("IsDecorating", typeof(Boolean), typeof(GridEntryContentTextBox), new PropertyMetadata(default(Boolean), IsDecoratingChanged));
        private static void IsDecoratingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var source = d as GridEntryContentTextBox;
            if (source != null) {
                source.IsDecoratingChanged();
                }
            }

        private void IsDecoratingChanged() {
            if (IsDecorating) {
                if (Decorator != null) {
                    Host.Visibility = Visibility.Hidden;
                    DecoratorPresenter.Visibility = Visibility.Visible;
                    }
                }
            else
                {
                Host.Visibility = Visibility.Visible;
                DecoratorPresenter.Visibility = Visibility.Hidden;
                if (Host is TextBox) {
                    ((TextBox)Host).DoAfterLayoutUpdated(i=> {
                        i.Focus();
                        i.SelectAll();
                        });
                    }
                }
            }

        public static readonly DependencyProperty IsDecoratingProperty = IsDecoratingPropertyKey.DependencyProperty;
        public Boolean IsDecorating {
            get { return (Boolean)GetValue(IsDecoratingProperty); }
            private set { SetValue(IsDecoratingPropertyKey, value); }
            }
        }
    }