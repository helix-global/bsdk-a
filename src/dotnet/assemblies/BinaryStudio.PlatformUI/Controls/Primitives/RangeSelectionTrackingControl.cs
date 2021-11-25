using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    internal class RangeSelectionTrackingControl : Border
        {
        static RangeSelectionTrackingControl()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeSelectionTrackingControl), new FrameworkPropertyMetadata(typeof(RangeSelectionTrackingControl)));
            }

        #region P:TotalLength:Int64
        public static readonly DependencyProperty TotalLengthProperty = DependencyProperty.Register("TotalLength", typeof(Int64), typeof(RangeSelectionTrackingControl), new PropertyMetadata(default(Int64), OnTotalLengthChanged));
        private static void OnTotalLengthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as RangeSelectionTrackingControl);
            if (source != null)
                {
                source.OnTotalLengthChanged();
                }
            }

        private void OnTotalLengthChanged()
            {
            InvalidateVisual();
            }

        public Int64 TotalLength
            {
            get { return (Int64)GetValue(TotalLengthProperty); }
            set { SetValue(TotalLengthProperty, value); }
            }
        #endregion
        #region P:Start:Int64
        public static readonly DependencyProperty StartProperty = DependencyProperty.Register("Start", typeof(Int64), typeof(RangeSelectionTrackingControl), new PropertyMetadata(default(Int64), OnStartChanged));
        private static void OnStartChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as RangeSelectionTrackingControl);
            if (source != null)
                {
                source.OnStartChanged();
                }
            }

        private void OnStartChanged()
            {
            InvalidateVisual();
            }

        public Int64 Start
            {
            get { return (Int64)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
            }
        #endregion
        #region P:Length:Int64
        public static readonly DependencyProperty LengthProperty = DependencyProperty.Register("Length", typeof(Int64), typeof(RangeSelectionTrackingControl), new PropertyMetadata(default(Int64), OnLengthChanged));
        private static void OnLengthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as RangeSelectionTrackingControl);
            if (source != null)
                {
                source.OnLengthChanged();
                }
            }

        private void OnLengthChanged()
            {
            InvalidateVisual();
            }

        public Int64 Length
            {
            get { return (Int64)GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
            }
        #endregion

        protected override void OnRender(DrawingContext context)
            {
            var totallength = TotalLength;
            Int64 α,β;
            if (Length >= 0) {
                α = Start;
                β = Length;
                }
            else
                {
                var δ = Start + Length;
                if (δ < 0)
                    {
                    α = Start + Length - δ;
                    β = -Length + δ;
                    }
                else
                    {
                    α = Start + Length;
                    β = -Length;
                    }
                }
            if ((totallength > 0) && (β > 0)) {
                var γ = ActualHeight;
                if (γ > 0) {
                    var ε = γ/TotalLength;
                    var r = new Rect(0, α, ActualWidth, β);
                    r.Scale(1, ε);
                    context.DrawRectangle(Background, null, r);
                    }
                }
            }
        }
    }
