using System;
using System.Windows;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Shell.Controls {
    public class GlyphButton : RoutedCommandButton, INonClientArea
        {
        #region P:PressedBackground:Brush
        public static readonly DependencyProperty PressedBackgroundProperty = DependencyProperty.Register("PressedBackground", typeof(Brush), typeof(GlyphButton));
        public Brush PressedBackground {
            get { return (Brush)GetValue(PressedBackgroundProperty); }
            set { SetValue(PressedBackgroundProperty, value); }
            }
        #endregion
        #region P:PressedBorderBrush:Brush
        public static readonly DependencyProperty PressedBorderBrushProperty = DependencyProperty.Register("PressedBorderBrush", typeof(Brush), typeof(GlyphButton));
        public Brush PressedBorderBrush {
            get { return (Brush)GetValue(PressedBorderBrushProperty); }
            set { SetValue(PressedBorderBrushProperty, value); }
            }
        #endregion
        #region P:PressedBorderThickness:Thickness
        public static readonly DependencyProperty PressedBorderThicknessProperty = DependencyProperty.Register("PressedBorderThickness", typeof(Thickness), typeof(GlyphButton));
        public Thickness PressedBorderThickness {
            get { return (Thickness)GetValue(PressedBorderThicknessProperty); }
            set { SetValue(PressedBorderThicknessProperty, value); }
            }
        #endregion
        #region P:HoverBackground:Brush
        public static readonly DependencyProperty HoverBackgroundProperty = DependencyProperty.Register("HoverBackground", typeof(Brush), typeof(GlyphButton));
        public Brush HoverBackground {
            get { return (Brush)GetValue(HoverBackgroundProperty); }
            set { SetValue(HoverBackgroundProperty, value); }
            }
        #endregion
        #region P:HoverBorderBrush:Brush
        public static readonly DependencyProperty HoverBorderBrushProperty = DependencyProperty.Register("HoverBorderBrush", typeof(Brush), typeof(GlyphButton));
        public Brush HoverBorderBrush {
            get { return (Brush)GetValue(HoverBorderBrushProperty); }
            set { SetValue(HoverBorderBrushProperty, value); }
            }
        #endregion
        #region P:HoverBorderThickness:Thickness
        public static readonly DependencyProperty HoverBorderThicknessProperty = DependencyProperty.Register("HoverBorderThickness", typeof(Thickness), typeof(GlyphButton));
        public Thickness HoverBorderThickness {
            get { return (Thickness)GetValue(HoverBorderThicknessProperty); }
            set { SetValue(HoverBorderThicknessProperty, value); }
            }
        #endregion
        #region P:GlyphForeground:Brush
        public static readonly DependencyProperty GlyphForegroundProperty = DependencyProperty.Register("GlyphForeground", typeof(Brush), typeof(GlyphButton));
        public Brush GlyphForeground {
            get { return (Brush)GetValue(GlyphForegroundProperty); }
            set { SetValue(GlyphForegroundProperty, value); }
            }
        #endregion
        #region P:HoverForeground:Brush
        public static readonly DependencyProperty HoverForegroundProperty = DependencyProperty.Register("HoverForeground", typeof(Brush), typeof(GlyphButton));
        public Brush HoverForeground {
            get { return (Brush)GetValue(HoverForegroundProperty); }
            set { SetValue(HoverForegroundProperty, value); }
            }
        #endregion
        #region P:PressedForeground:Brush
        public static readonly DependencyProperty PressedForegroundProperty = DependencyProperty.Register("PressedForeground", typeof(Brush), typeof(GlyphButton));
        public Brush PressedForeground {
            get { return (Brush)GetValue(PressedForegroundProperty); }
            set { SetValue(PressedForegroundProperty, value); }
            }
        #endregion
        #region P:IsChecked:Boolean
        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(Boolean), typeof(GlyphButton));
        public Boolean IsChecked {
            get { return (Boolean)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, Boxes.Box(value)); }
            }
        #endregion

        static GlyphButton() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GlyphButton), new FrameworkPropertyMetadata(typeof(GlyphButton)));
            }

        Int32 INonClientArea.HitTest(Point point) {
            return 1;
            }
        }
    }
