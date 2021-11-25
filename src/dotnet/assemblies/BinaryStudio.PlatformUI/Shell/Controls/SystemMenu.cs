using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Shell.Controls
    {
    public class SystemMenu : Control
        {
        static SystemMenu()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SystemMenu), new FrameworkPropertyMetadata(typeof(SystemMenu)));
            }

        #region P:VectorIcon:DrawingBrush
        public static readonly DependencyProperty VectorIconProperty = DependencyProperty.Register("VectorIcon", typeof(DrawingBrush), typeof(SystemMenu), new PropertyMetadata(default(DrawingBrush)));
        public DrawingBrush VectorIcon
            {
            get { return (DrawingBrush)GetValue(VectorIconProperty); }
            set { SetValue(VectorIconProperty, value); }
            }
        #endregion
        }
    }