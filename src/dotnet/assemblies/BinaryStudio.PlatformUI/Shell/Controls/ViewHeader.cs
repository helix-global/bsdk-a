using System.Windows;
using System.Windows.Controls;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class ViewHeader : Control
        {
        static ViewHeader()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ViewHeader), new FrameworkPropertyMetadata(typeof(ViewHeader)));
            }

        #region P:ContainingElement:ViewElement
        public static readonly DependencyProperty ContainingElementProperty = DependencyProperty.Register("ContainingElement", typeof(ViewElement), typeof(ViewHeader), new PropertyMetadata(default(ViewElement)));
        public ViewElement ContainingElement {
            get { return (ViewElement)GetValue(ContainingElementProperty); }
            set { SetValue(ContainingElementProperty, value); }
            }
        #endregion
        #region P:ContainingFrameworkElement:FrameworkElement
        public static readonly DependencyProperty ContainingFrameworkElementProperty = DependencyProperty.Register("ContainingFrameworkElement", typeof(FrameworkElement), typeof(ViewHeader), new PropertyMetadata(default(FrameworkElement)));
        public FrameworkElement ContainingFrameworkElement {
            get { return (FrameworkElement)GetValue(ContainingFrameworkElementProperty); }
            set { SetValue(ContainingFrameworkElementProperty, value); }
            }
        #endregion
        #region P:View:View
        public static readonly DependencyProperty ViewProperty = DependencyProperty.Register("View", typeof(View), typeof(ViewHeader), new PropertyMetadata(default(View)));
        public View View {
            get { return (View)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
            }
        #endregion
        }
    }
