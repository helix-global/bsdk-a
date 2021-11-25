using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace BinaryStudio.PlatformUI
    {
    public class SplitterGrip : Thumb
        {
        static SplitterGrip()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterGrip), new FrameworkPropertyMetadata(typeof(SplitterGrip)));
            }

        #region P:Orientation:Orientation
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SplitterGrip), new PropertyMetadata(default(Orientation)));
        public Orientation Orientation {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
            }
        #endregion
        #region P:ResizeBehavior:GridResizeBehavior
        public static readonly DependencyProperty ResizeBehaviorProperty = DependencyProperty.Register("ResizeBehavior", typeof(GridResizeBehavior), typeof(SplitterGrip), new PropertyMetadata(GridResizeBehavior.CurrentAndNext));
        public GridResizeBehavior ResizeBehavior {
            get { return (GridResizeBehavior)GetValue(ResizeBehaviorProperty); }
            set { SetValue(ResizeBehaviorProperty, value); }
            }
        #endregion
        }
    }
