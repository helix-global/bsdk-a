using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    [DebuggerDisplay("\\{{OffsetX},{OffsetY}\\}")]
    public class TextCaret : Adorner
        {
        public TextCaret(UIElement adornedElement)
            : base(adornedElement)
            {
            IsHitTestVisible = false;
            Focusable = false;
            Translation = new TranslateTransform();
            IsBlinking = true;
            }

        /// <summary>When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. </summary>
        /// <param name="context">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext context) {
            base.OnRender(context);
            if (IsVisible) {
                context.PushTransform(Translation);
                context.PushOpacity(Opacity);
                context.PushGuidelineSet(
                    new GuidelineSet(new []
                        {
                        (OffsetX - CaretWidth/2.0),
                        (OffsetX),
                        (OffsetX + CaretWidth/2.0)
                        },
                        null));
                context.DrawRectangle(CaretBrush, null, new Rect(-CaretWidth/2.0, 0.0, CaretWidth, CaretHeight));
                context.Pop();
                context.Pop();
                context.Pop();
                }
            }

        /// <summary>Retrieves the time required to invert the caret's pixels.</summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern Int32 GetCaretBlinkTime();

        #region P:CaretWidth:Double
        /// <summary>Identifies the <see cref="CaretWidth" /> dependency property. </summary>
        /// <returns>The identifier for the <see cref="CaretWidth" /> dependency property.</returns>
        public static readonly DependencyProperty CaretWidthProperty = DependencyProperty.Register("CaretWidth", typeof(Double), typeof(TextCaret), new PropertyMetadata(SystemParameters.CaretWidth));
        public Double CaretWidth {
            get { return (Double)GetValue(CaretWidthProperty); }
            set { SetValue(CaretWidthProperty, value); }
            }
        #endregion
        #region P:CaretBrush:Brush
        /// <summary>Identifies the <see cref="CaretBrush" /> dependency property. </summary>
        /// <returns>The identifier for the <see cref="CaretBrush" /> dependency property.</returns>
        public static readonly DependencyProperty CaretBrushProperty = DependencyProperty.Register("CaretBrush", typeof(Brush), typeof(TextCaret), new PropertyMetadata(Brushes.Black));
        public Brush CaretBrush {
            get { return (Brush)GetValue(CaretBrushProperty); }
            set { SetValue(CaretBrushProperty, value); }
            }
        #endregion
        #region P:CaretHeight:Double
        /// <summary>Identifies the <see cref="CaretHeight" /> dependency property. </summary>
        /// <returns>The identifier for the <see cref="CaretHeight" /> dependency property.</returns>
        public static readonly DependencyProperty CaretHeightProperty = DependencyProperty.Register("CaretHeight", typeof(Double), typeof(TextCaret), new PropertyMetadata(20.0, OnCaretHeightChanged));
        private static void OnCaretHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var source = d as TextCaret;
            if (source != null) {
                source.InvalidateVisual();
                }
            }

        public Double CaretHeight {
            get { return (Double)GetValue(CaretHeightProperty); }
            set { SetValue(CaretHeightProperty, value); }
            }
        #endregion
        #region P:IsBlinking:Boolean
        /// <summary>Identifies the <see cref="IsBlinking" /> dependency property. </summary>
        /// <returns>The identifier for the <see cref="IsBlinking" /> dependency property.</returns>
        public static readonly DependencyProperty IsBlinkingProperty = DependencyProperty.Register("IsBlinking", typeof(Boolean), typeof(TextCaret), new PropertyMetadata(default(Boolean), IsBlinkingChanged));
        private static void IsBlinkingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            var source = d as TextCaret;
            if (source != null) {
                source.IsBlinkingChanged();
                }
            }

        private void IsBlinkingChanged() {
            if (IsBlinking) {
                var n = GetCaretBlinkTime();
                n = (n > 0) ? n : 500;
                if (n > 0) {
                    if (BlinkAnimationClock == null) {
                        var duration = new Duration(TimeSpan.FromMilliseconds(n*2));
                        var animation = new DoubleAnimationUsingKeyFrames {
                            BeginTime = null,
                            RepeatBehavior = RepeatBehavior.Forever
                            };
                        animation.KeyFrames.Add(new DiscreteDoubleKeyFrame(1.0, KeyTime.FromPercent(0.0)));
                        animation.KeyFrames.Add(new DiscreteDoubleKeyFrame(0.0, KeyTime.FromPercent(0.5)));
                        animation.Duration = duration;
                        Timeline.SetDesiredFrameRate(animation, 10);
                        BlinkAnimationClock = animation.CreateClock();
                        if (BlinkAnimationClock != null) {
                            if (BlinkAnimationClock.Controller != null) {
                                BlinkAnimationClock.Controller.Begin();
                                }
                            ApplyAnimationClock(OpacityProperty, BlinkAnimationClock);
                            }
                        }
                    }
                }
            else
                {
                if (BlinkAnimationClock != null) {
                    BlinkAnimationClock.Controller?.Stop();
                    ApplyAnimationClock(OpacityProperty, null);
                    BlinkAnimationClock = null;
                    }
                }
            }

        public Boolean IsBlinking {
            get { return (Boolean)GetValue(IsBlinkingProperty); }
            set { SetValue(IsBlinkingProperty, value); }
            }
        #endregion
        #region P:OffsetX:Double
        public Double OffsetX {
            get { return Translation.X; }
            set {
                Translation.X = value;
                Update();
                }
            }
        #endregion
        #region P:OffsetY:Double
        public Double OffsetY {
            get { return Translation.Y; }
            set {
                Translation.Y = value;
                Update();
                }
            }
        #endregion

        public void Update() {
            if (IsBlinking) {
                IsBlinking = false;
                InvalidateVisual();
                IsBlinking = true;
                }
            else
                {
                InvalidateVisual();
                }
            }

        private AnimationClock BlinkAnimationClock;
        private readonly TranslateTransform Translation;
        }
    }