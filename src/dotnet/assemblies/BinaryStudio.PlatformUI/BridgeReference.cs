using System;
using System.Windows;

namespace BinaryStudio.PlatformUI
    {
    public class BridgeReference : FrameworkElement {
        #region P:Source:Object
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Object), typeof(BridgeReference), new PropertyMetadata(default(Object), OnSourceChanged));
        private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            (sender as BridgeReference)?.OnSourceChanged();
            }

        private void OnSourceChanged() {
            Target = Source;
            return;
            }

        public Object Source {
            get { return (Object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
            }
        #endregion
        #region P:Target:Object
        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(Object), typeof(BridgeReference), new PropertyMetadata(default(Object), OnTargetChanged));
        private static void OnTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            (sender as BridgeReference)?.OnTargetChanged();
            }

        private void OnTargetChanged() {
            return;
            }

        public Object Target {
            get { return (Object)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
            }
        #endregion
        }
    }