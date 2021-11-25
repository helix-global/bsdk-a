using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI
    {
    public class RoutedCommandButton : Button
        {
        static RoutedCommandButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RoutedCommandButton), new FrameworkPropertyMetadata(typeof(RoutedCommandButton)));
            CommandTargetProperty.OverrideMetadata(typeof(RoutedCommandButton), new FrameworkPropertyMetadata(null, OnCommandTargetChanged));
            }

        private static void OnCommandTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = sender as RoutedCommandButton;
            if (source != null) {
                source.OnCommandTargetChanged();
                }
            }

        protected virtual void OnCommandTargetChanged()
            {
            }
        }
    }
