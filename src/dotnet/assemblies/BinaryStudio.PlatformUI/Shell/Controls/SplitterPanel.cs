using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using BinaryStudio.PlatformUI.Shell.Controls;

namespace BinaryStudio.PlatformUI.Controls
    {
    public class SplitterPanel : Panel
        {
        static SplitterPanel()
            {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterPanel), new FrameworkPropertyMetadata(typeof(SplitterPanel)));
            }

        public SplitterPanel() {
            AddHandler(Thumb.DragStartedEvent, new DragStartedEventHandler(OnSplitterDragStarted));
            }

        #region P:Orientation:Orientation
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SplitterPanel), new PropertyMetadata(default(Orientation)));
        public Orientation Orientation {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
            }
        #endregion
        #region P:ShowResizePreview:Boolean
        public static readonly DependencyProperty ShowResizePreviewProperty = DependencyProperty.Register("ShowResizePreview", typeof(Boolean), typeof(SplitterPanel), new PropertyMetadata(default(Boolean)));
        public Boolean ShowResizePreview {
            get { return (Boolean)GetValue(ShowResizePreviewProperty); }
            set { SetValue(ShowResizePreviewProperty, value); }
            }
        #endregion
        #region P:SplitterPanel.ActualSplitterLength:Double
        private static readonly DependencyPropertyKey ActualSplitterLengthPropertyKey = DependencyProperty.RegisterAttachedReadOnly("ActualSplitterLength", typeof(Double), typeof(SplitterPanel), new PropertyMetadata(default(Double)));
        public static readonly DependencyProperty ActualSplitterLengthProperty = ActualSplitterLengthPropertyKey.DependencyProperty;
        private static void SetActualSplitterLength(DependencyObject source, Double value) {
            source.SetValue(ActualSplitterLengthPropertyKey, value);
            }
        public static Double GetActualSplitterLength(DependencyObject source) {
            return (Double)source.GetValue(ActualSplitterLengthProperty);
            }
        #endregion
        #region P:SplitterPanel.IsFirst:Boolean
        private static readonly DependencyPropertyKey IsFirstPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsFirst", typeof(Boolean), typeof(SplitterPanel), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsFirstProperty = IsFirstPropertyKey.DependencyProperty;
        private static void SetIsFirst(DependencyObject source, Boolean value) {
            source.SetValue(IsFirstPropertyKey, value);
            }
        public static Boolean GetIsFirst(DependencyObject source) {
            return (Boolean)source.GetValue(IsFirstProperty);
            }
        #endregion
        #region P:SplitterPanel.IsLast:Boolean
        private static readonly DependencyPropertyKey IsLastPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsLast", typeof(Boolean), typeof(SplitterPanel), new PropertyMetadata(default(Boolean)));
        public static readonly DependencyProperty IsLastProperty = IsLastPropertyKey.DependencyProperty;
        private static void SetIsLast(DependencyObject source, Boolean value) {
            source.SetValue(IsLastPropertyKey, value);
            }
        public static Boolean GetIsLast(DependencyObject source) {
            return (Boolean)source.GetValue(IsLastProperty);
            }
        #endregion
        #region P:SplitterPanel.Index:Int32
        private static readonly DependencyPropertyKey IndexPropertyKey = DependencyProperty.RegisterAttachedReadOnly("Index", typeof(Int32), typeof(SplitterPanel), new PropertyMetadata(default(Int32)));
        public static readonly DependencyProperty IndexProperty = IndexPropertyKey.DependencyProperty;
        private static void SetIndex(DependencyObject source, Int32 value) {
            source.SetValue(IndexPropertyKey, value);
            }
        public static Int32 GetIndex(DependencyObject source) {
            return (Int32)source.GetValue(IndexProperty);
            }
        #endregion
        #region P:SplitterPanel.MinimumLength:Double
        public static readonly DependencyProperty MinimumLengthProperty = DependencyProperty.RegisterAttached("MinimumLength", typeof(Double), typeof(SplitterPanel), new PropertyMetadata(default(Double)));
        public static void SetMinimumLength(DependencyObject source, Double value) {
            source.SetValue(MinimumLengthProperty, value);
            }
        public static Double GetMinimumLength(DependencyObject source) {
            return (Double)source.GetValue(MinimumLengthProperty);
            }
        #endregion
        #region P:SplitterPanel.MaximumLength:Double
        public static readonly DependencyProperty MaximumLengthProperty = DependencyProperty.RegisterAttached("MaximumLength", typeof(Double), typeof(SplitterPanel), new PropertyMetadata(Double.MaxValue));
        public static void SetMaximumLength(DependencyObject source, Double value) {
            source.SetValue(MaximumLengthProperty, value);
            }
        public static Double GetMaximumLength(DependencyObject source) {
            return (Double)source.GetValue(MaximumLengthProperty);
            }
        #endregion
        #region P:SplitterPanel.SplitterLength:SplitterLength
        public static readonly DependencyProperty SplitterLengthProperty = DependencyProperty.RegisterAttached("SplitterLength", typeof(SplitterLength), typeof(SplitterPanel), new PropertyMetadata(new SplitterLength(100)));
        public static void SetSplitterLength(DependencyObject source, SplitterLength value) {
            source.SetValue(SplitterLengthProperty, value);
            }
        public static SplitterLength GetSplitterLength(DependencyObject source) {
            return (SplitterLength)source.GetValue(SplitterLengthProperty);
            }
        #endregion
        #region P:IsShowingResizePreview:Boolean
        private Boolean IsShowingResizePreview { get {
            return currentPreviewWindow != null;
            }}
        #endregion

        #region M:ArrangeOverride(Size):Size
        protected override Size ArrangeOverride(Size finalSize) {
            var r = new Rect(0.0, 0.0, finalSize.Width, finalSize.Height);
            foreach (UIElement e in InternalChildren) {
                if (e != null) {
                    var ASL = GetActualSplitterLength(e);
                    if (Orientation == Orientation.Horizontal) {
                        r.Width = ASL;
                        e.Arrange(r);
                        r.X += ASL;
                        }
                    else
                    {
                        r.Height = ASL;
                        e.Arrange(r);
                        r.Y += ASL;
                        }
                    }
                }
            return finalSize;
            }
        #endregion
        #region M:Measure(Size,Orientation,SplitterMeasureData[],Boolean)
        internal static Size Measure(Size availableSize, Orientation orientation, SplitterMeasureData[] E, Boolean flag) {
            if (((orientation == Orientation.Horizontal) && (availableSize.Width.IsNonreal())) ||
                ((orientation == Orientation.Vertical) && (availableSize.Height.IsNonreal()))) {
                var W = 0.0;
                var H = 0.0;
                for (var  i = 0; i < E.Length; i++) {
                    if (flag) { E[i].Source.Measure(availableSize); }
                    if (orientation == Orientation.Horizontal) {
                        W += E[i].Source.DesiredSize.Width;
                        H = Math.Max(H, E[i].Source.DesiredSize.Height);
                        }
                    else
                        {
                        H += E[i].Source.DesiredSize.Height;
                        W = Math.Max(W, E[i].Source.DesiredSize.Width);
                        }
                    }
                var r = new Rect(0.0, 0.0, W,H);
                for (var  i = 0; i < E.Length; i++) {
                    if (orientation == Orientation.Horizontal) {
                        r.Width = E[i].Source.DesiredSize.Width;
                        E[i].MeasuredBounds = r;
                        r.X += r.Width;
                        }
                    else
                        {
                        r.Height = E[i].Source.DesiredSize.Height;
                        E[i].MeasuredBounds = r;
                        r.Y += r.Height;
                        }
                    }
                return new Size(W,H);
                }
            else
                {
                var TSAL = 0.0;
                var TAL = 0.0;
                var TML = 0.0;
                var TSML = 0.0;
                for (var i = 0; i < E.Length; i++) {
                    var AL = E[i].AttachedLength;
                    var ML = GetMinimumLength(E[i].Source);
                    if (AL.IsStretch) {
                        TSAL += AL.Value;
                        TSML += ML;
                        }
                    else {
                        TAL += AL.Value;
                        TML += ML;
                        }
                    E[i].IsMinimumReached = false;
                    E[i].IsMaximumReached = false;
                    }
                var W = availableSize.Width;
                var H = availableSize.Height;
                var L = (orientation == Orientation.Horizontal) ? W : H;
                var A = (TAL == 0.0) ? 0.0 : Math.Max(0.0, L - TSAL);
                var B = (A == 0.0) ? L : TSAL;
                if ((TSML + TML) <= L) {
                    for (var i = 0; i < E.Length; i++) {
                        var AL = E[i].AttachedLength;
                        var ML = GetMaximumLength(E[i].Source);
                        if (AL.IsStretch && ((TSAL == 0.0) ? 0.0 : (AL.Value / TSAL * B)) > ML) {
                            E[i].IsMaximumReached = true;
                            if (TSAL == AL.Value) {
                                TSAL = ML;
                                E[i].AttachedLength = new SplitterLength(ML);
                                }
                            else {
                                TSAL -= AL.Value;
                                E[i].AttachedLength = new SplitterLength(TSAL);
                                TSAL = TSAL + TSAL;
                                }
                            A = ((TAL == 0.0) ? 0.0 : Math.Max(0.0, L - TSAL));
                            B = ((A == 0.0) ? L : TSAL);
                            }
                        }
                    if (A < TML) {
                        A = TML;
                        B = L - A;
                        }
                    for (var i = 0; i < E.Length; i++) {
                        var AL = E[i].AttachedLength;
                        var ML = GetMinimumLength(E[i].Source);
                        if (AL.IsFill) {
                            if (((TAL == 0.0) ? 0.0 : (AL.Value / TAL * A)) < ML) {
                                E[i].IsMinimumReached = true;
                                A -= ML;
                                TAL -= AL.Value;
                                }
                            }
                        else if (((TSAL == 0.0) ? 0.0 : (AL.Value / TSAL * B)) < ML) {
                            E[i].IsMinimumReached = true;
                            B -= ML;
                            TSAL -= AL.Value;
                            }
                        }
                    }
                var AS = new Size(W, H);
                var r = new Rect(0.0, 0.0, W, H);
                for (var i = 0; i < E.Length; i++) {
                    var AL = E[i].AttachedLength;
                    Double N;
                    if (!E[i].IsMinimumReached) {
                        if (AL.IsFill) {
                            N = ((TAL == 0.0) ? 0.0 : (AL.Value / TAL * A));
                            }
                        else
                            {
                            N = ((TSAL == 0.0) ? 0.0 : (AL.Value / TSAL * B));
                            }
                        }
                    else { N = GetMinimumLength(E[i].Source); }
                    if (flag) { SetActualSplitterLength(E[i].Source, N); }
                    if (orientation == Orientation.Horizontal) {
                        AS.Width = N;
                        E[i].MeasuredBounds = new Rect(r.Left, r.Top, N, r.Height);
                        r.X += N;
                        if (flag) {
                            E[i].Source.Measure(AS);
                            }
                        }
                    else
                        {
                        AS.Height = N;
                        E[i].MeasuredBounds = new Rect(r.Left, r.Top, r.Width, N);
                        r.Y += N;
                        if (flag) {
                            E[i].Source.Measure(AS);
                            }
                        }
                    }
                return new Size(W, H);
                }
            }
        #endregion
        #region M:MeasureOverride(Size):Size
        protected override Size MeasureOverride(Size availableSize) {
            var count = InternalChildren.Count;
            for (var i = 0; i < count; i++) {
                var e = InternalChildren[i];
                if (e != null) {
                    SetIndex(e, i);
                    SetIsFirst(e, i == 0);
                    SetIsLast(e, i == (count - 1));
                    }
                }
            return Measure(availableSize, Orientation, InternalChildren.OfType<UIElement>().Select(i => new SplitterMeasureData(i)).ToArray(), true);
            }
        #endregion

        #region M:OnSplitterDragStarted(Object,DragStartedEventArgs)
        private void OnSplitterDragStarted(Object sender, DragStartedEventArgs e) {
            var source = e.OriginalSource as SplitterGrip;
            if (source != null) {
                e.Handled = true;
                source.DragDelta += OnSplitterResized;
                source.DragCompleted += OnSplitterDragCompleted;
                if (ShowResizePreview) {
                    currentPreviewWindow = new SplitterResizePreviewWindow();
                    currentPreviewWindow.Show(source);
                    }
                }
            }
        #endregion
        #region M:OnSplitterDragCompleted(Object,DragCompletedEventArgs)
        private void OnSplitterDragCompleted(Object sender, DragCompletedEventArgs e) {
            var source = sender as SplitterGrip;
            if (source != null) {
                e.Handled = true;
                if (IsShowingResizePreview) {
                    currentPreviewWindow.Hide();
                    currentPreviewWindow = null;
                    if (!e.Canceled) {
                        var point = new Point(e.HorizontalChange, e.VerticalChange).DeviceToLogicalUnits();
                        CommitResize(source, point.X, point.Y);
                        }
                    }
                source.DragDelta -= OnSplitterResized;
                source.DragCompleted -= OnSplitterDragCompleted;
                }
            }
        #endregion
        #region M:OnSplitterResized(Object,DragDeltaEventArgs)
        private void OnSplitterResized(Object sender, DragDeltaEventArgs e) {
            var splitterGrip = sender as SplitterGrip;
            if (splitterGrip != null) {
                e.Handled = true;
                if (IsShowingResizePreview) {
                    TrackResizePreview(splitterGrip, e.HorizontalChange, e.VerticalChange);
                    return;
                    }
                CommitResize(splitterGrip, e.HorizontalChange, e.VerticalChange);
                }
            }
        #endregion

        #region M:TrackResizePreview(SplitterGrip,Double,Double)
        private void TrackResizePreview(SplitterGrip grip, Double horizontalChange, Double verticalChange) {
            Int32 i, li, ri;
            if (GetResizeIndices(grip, out i, out li, out ri)) {
                var list = SplitterMeasureData.FromElements(InternalChildren);
                ResizeChildrenCore(list[li], list[ri], (Orientation == Orientation.Horizontal)
                    ? horizontalChange
                    : verticalChange);
                Measure(RenderSize, Orientation, list.ToArray(), false);
                var point = grip.TransformToAncestor(this).Transform(new Point(0.0, 0.0));
                if (Orientation == Orientation.Horizontal) {
                    point.X += list[i].MeasuredBounds.Width - InternalChildren[i].RenderSize.Width;
                    }
                else
                {
                    point.Y += list[i].MeasuredBounds.Height - InternalChildren[i].RenderSize.Height;
                    }
                point = PointToScreen(point);
                currentPreviewWindow.Move((Int32)point.X, (Int32)point.Y);
                }
            }
        #endregion
        #region M:CommitResize(SplitterGrip,Double,Double)
        private void CommitResize(SplitterGrip grip, Double horizontalChange, Double verticalChange) {
            Int32 i, li, ri;
            if (GetResizeIndices(grip, out i, out li, out ri)) {
                ResizeChildren(li, ri, (Orientation == Orientation.Horizontal)
                    ? horizontalChange
                    : verticalChange);
                }
            }
        #endregion
        #region M:ResizeChildrenCore(SplitterMeasureData,SplitterMeasureData,Double)
        private Boolean ResizeChildrenCore(SplitterMeasureData lt, SplitterMeasureData rt, Double pixelAmount) {
            var le = lt.Source;
            var re = rt.Source;
            var ll = lt.AttachedLength;
            var rl = rt.AttachedLength;
            var lal = GetActualSplitterLength(le);
            var ral = GetActualSplitterLength(re);
            var ln = Math.Max(0.0, Math.Min(lal + ral, lal + pixelAmount));
            var rn = Math.Max(0.0, Math.Min(lal + ral, ral - pixelAmount));
            var lml = GetMinimumLength(le);
            var rml = GetMinimumLength(re);
            if (lml + rml <= ln + rn) {
                if (ln < lml) {
                    rn -= lml - ln;
                    ln = lml;
                    }
                if (rn < rml) {
                    ln -= rml - rn;
                    rn = rml;
                    }
                if ((ll.IsFill && rl.IsFill) || (ll.IsStretch && rl.IsStretch)) {
                    lt.AttachedLength = new SplitterLength(ln / (ln + rn) * (ll.Value + rl.Value), ll.SplitterUnitType);
                    rt.AttachedLength = new SplitterLength(rn / (ln + rn) * (ll.Value + rl.Value), ll.SplitterUnitType);
                    }
                else if (ll.IsFill) {
                    rt.AttachedLength = new SplitterLength(rn, SplitterUnitType.Stretch);
                    }
                else
                    {
                    lt.AttachedLength = new SplitterLength(ln, SplitterUnitType.Stretch);
                    }
                return true;
                }
            return false;
            }
        #endregion
        #region M:GetResizeIndices(SplitterGrip,Int32,Int32,Int32):Boolean
        private Boolean GetResizeIndices(SplitterGrip grip, out Int32 index, out Int32 li, out Int32 ri) {
            for (var i = 0; i < InternalChildren.Count; i++) {
                if (InternalChildren[i].IsAncestorOf(grip)) {
                    index = i;
                    switch (grip.ResizeBehavior) {
                        case GridResizeBehavior.CurrentAndNext:
                            li = i;
                            ri = i + 1;
                            break;
                        case GridResizeBehavior.PreviousAndCurrent:
                            li = i - 1;
                            ri = i;
                            break;
                        case GridResizeBehavior.PreviousAndNext:
                            li = i - 1;
                            ri = i + 1;
                            break;
                        default:
                            throw new InvalidOperationException("BasedOnAlignment is not a valid resize behavior");
                        }
                    return (li >= 0) && (ri >= 0) && (li < InternalChildren.Count) && (ri < InternalChildren.Count);
                    }
                }
            index = -1;
            li = -1;
            ri = -1;
            return false;
            }
        #endregion
        #region M:ResizeChildren(Int32,Int32,Double)
        internal void ResizeChildren(Int32 li, Int32 ri, Double pixelAmount) {
            var lt = new SplitterMeasureData(InternalChildren[li]);
            var rt = new SplitterMeasureData(InternalChildren[ri]);
            if (ResizeChildrenCore(lt, rt, pixelAmount)) {
                SetSplitterLength(lt.Source, lt.AttachedLength);
                SetSplitterLength(rt.Source, rt.AttachedLength);
                InvalidateMeasure();
                }
            }
        #endregion

        private SplitterResizePreviewWindow currentPreviewWindow;
        }
    }
