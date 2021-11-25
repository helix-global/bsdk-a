using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using BinaryStudio.PlatformUI.Controls.Markups;
using BinaryStudio.PlatformUI.Extensions;
using BinaryStudio.DataProcessing;
using BinaryStudio.DataProcessing.Annotations;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    internal class HexBoxPanelH : FrameworkElement, INotifyPropertyChanged
        {
        protected const Double PaddingLeft = 5.0;
        protected const Double PaddingTop = 2.0;
        protected const Int16 LEFT  = 0;
        protected const Int16 RIGHT = 1;
        public HexBoxPanelH()
            {
            Focusable = true;
            Cursor = Cursors.IBeam;
            Places = new Slot<Double, Double, Double, Double, Double>[16];
            ActualPlaces = new Slot<Double, Double, Double, Double,Double>[16];
            for (var i = 0; i < 16; ++i) {
                Places[i] = Slot.Create(0.0, 0.0, 0.0, 0.0,0.0);
                ActualPlaces[i] = Slot.Create(0.0, 0.0, 0.0, 0.0,0.0);
                }
            }

        #region P:Background:Brush
        public static readonly DependencyProperty BackgroundProperty = Control.BackgroundProperty.AddOwner(typeof(HexBoxPanelH), new FrameworkPropertyMetadata(Brushes.Transparent, OnBackgroundChanged));
        private static void OnBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as HexBoxPanelH);
            if (source != null)
                {
                source.OnBackgroundChanged();
                }
            }

        private void OnBackgroundChanged()
            {
            InvalidateVisual();
            }

        public Brush Background
            {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
            }
        #endregion
        #region P:Source:Stream
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Stream), typeof(HexBoxPanelH), new PropertyMetadata(default(Stream), OnSourceChanged));
        private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as HexBoxPanelH);
            if (source != null)
                {
                source.OnSourceChanged();
                }
            }

        private void OnSourceChanged()
            {
            InvalidateMeasure();
            }

        public Stream Source
            {
            get { return (Stream)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
            }
        #endregion
        #region P:ItemsCount:Int64
        public static readonly DependencyProperty ItemsCountProperty = DependencyProperty.Register("ItemsCount", typeof(Int64), typeof(HexBoxPanelH), new PropertyMetadata(default(Int64), OnItemsCountChanged));
        private static void OnItemsCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as HexBoxPanelH);
            if (source != null)
                {
                source.OnItemsCountChanged();
                }
            }

        private void OnItemsCountChanged()
            {
            InvalidateVisual();
            }

        public Int64 ItemsCount
            {
            get { return (Int64)GetValue(ItemsCountProperty); }
            set { SetValue(ItemsCountProperty, value); }
            }
        #endregion
        #region P:ItemSize:Size
        public static readonly DependencyProperty ItemSizeProperty = DependencyProperty.Register("ItemSize", typeof(Size), typeof(HexBoxPanelH), new PropertyMetadata(default(Size), OnItemSizeChanged));
        private static void OnItemSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as HexBoxPanelH);
            if (source != null) {
                source.OnItemSizeChanged();
                }
            }

        protected virtual void OnItemSizeChanged()
            {
            var x = PaddingLeft;
            for (var i = 0; i < 16; ++i) {
                Places[i].Item1 = Math.Round(x);
                Places[i].Item2 = ItemSize.Width * ((i == 7) ? 4 : 3);
                Places[i].Item3 = x - ItemSize.Width * ((i == 8) ? 1.0 : 0.5);
                Places[i].Item4 = x + ItemSize.Width * ((i == 7) ? 1.0 : 0.5) + ItemSize.Width*2;
                Places[i].Item5 = Math.Round(Places[i].Item1 + ItemSize.Width*2);
                x += Places[i].Item2;
                }
            OnHorizontalOffsetChanged();
            InvalidateVisual();
            }

        public Size ItemSize
            {
            get { return (Size)GetValue(ItemSizeProperty); }
            set { SetValue(ItemSizeProperty, value); }
            }
        #endregion
        #region P:VerticalOffset:Double
        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset", typeof(Double), typeof(HexBoxPanelH), new PropertyMetadata(default(Double), OnVerticalOffsetChanged));
        private static void OnVerticalOffsetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as HexBoxPanelH);
            if (source != null)
                {
                source.OnVerticalOffsetChanged();
                }
            }

        private void OnVerticalOffsetChanged()
            {
            InvalidateVisual();
            ShowOrHideTextCaret(false);
            InvalidateMarkups();
            }

        public Double VerticalOffset
            {
            get { return (Double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
            }
        #endregion
        #region P:HorizontalOffset:Double
        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register("HorizontalOffset", typeof(Double), typeof(HexBoxPanelH), new PropertyMetadata(default(Double), OnHorizontalOffsetChanged));
        private static void OnHorizontalOffsetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as HexBoxPanelH);
            if (source != null) {
                source.OnHorizontalOffsetChanged();
                }
            }

        protected void OnHorizontalOffsetChanged()
            {
            var x = HorizontalOffset;
            for (var i = 0; i < 16; ++i) {
                ActualPlaces[i].Item2 = Places[i].Item2;
                ActualPlaces[i].Item1 = Math.Round(Places[i].Item1 + x);
                ActualPlaces[i].Item3 = Places[i].Item3 + x;
                ActualPlaces[i].Item4 = Places[i].Item4 + x;
                ActualPlaces[i].Item5 = Math.Round(Places[i].Item5 + x);
                }
            }

        public Double HorizontalOffset
            {
            get { return (Double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
            }
        #endregion
        #region P:ViewportHeight:Double
        public static readonly DependencyProperty ViewportHeightProperty = DependencyProperty.Register("ViewportHeight", typeof(Double), typeof(HexBoxPanelH), new PropertyMetadata(default(Double), OnViewportHeightChanged));
        private static void OnViewportHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as HexBoxPanelH);
            if (source != null)
                {
                source.OnViewportHeightChanged();
                }
            }

        private void OnViewportHeightChanged()
            {
            InvalidateVisual();
            InvalidateMarkups();
            }

        public Double ViewportHeight
            {
            get { return (Double)GetValue(ViewportHeightProperty); }
            set { SetValue(ViewportHeightProperty, value); }
            }
        #endregion
        #region P:Typeface:Typeface
        public static readonly DependencyProperty TypefaceProperty = DependencyProperty.Register("Typeface", typeof(Typeface), typeof(HexBoxPanelH), new PropertyMetadata(default(Typeface), OnTypefaceChanged));
        private static void OnTypefaceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as HexBoxPanelH);
            if (source != null)
                {
                source.OnTypefaceChanged();
                }
            }

        private void OnTypefaceChanged()
            {
            InvalidateVisual();
            }

        public Typeface Typeface
            {
            get { return (Typeface)GetValue(TypefaceProperty); }
            set { SetValue(TypefaceProperty, value); }
            }
        #endregion
        #region P:ScrollableObject:IScrollInfo
        public static readonly DependencyProperty ScrollableObjectProperty = DependencyProperty.Register("ScrollableObject", typeof(IScrollInfo), typeof(HexBoxPanelH), new PropertyMetadata(default(IScrollInfo), OnScrollableObjectChanged));
        private static void OnScrollableObjectChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as HexBoxPanelH);
            if (source != null)
                {
                source.OnScrollableObjectChanged();
                }
            }

        private void OnScrollableObjectChanged()
            {
            }

        public IScrollInfo ScrollableObject
            {
            get { return (IScrollInfo)GetValue(ScrollableObjectProperty); }
            set { SetValue(ScrollableObjectProperty, value); }
            }
        #endregion
        #region P:PhysicalViewport:Vector
        public static readonly DependencyProperty PhysicalViewportProperty = DependencyProperty.Register("PhysicalViewport", typeof(Vector), typeof(HexBoxPanelH), new PropertyMetadata(default(Vector)));
        public Vector PhysicalViewport
            {
            get { return (Vector)GetValue(PhysicalViewportProperty); }
            set { SetValue(PhysicalViewportProperty, value); }
            }
        #endregion
        #region P:ExtentHeight:Double
        public static readonly DependencyProperty ExtentHeightProperty = DependencyProperty.Register("ExtentHeight", typeof(Double), typeof(HexBoxPanelH), new PropertyMetadata(default(Double), OnExtentHeightChanged));
        private static void OnExtentHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var source = (sender as HexBoxPanelH);
            if (source != null)
            {
                source.OnExtentHeightChanged();
            }
        }

        private void OnExtentHeightChanged()
        {
        }

        public Double ExtentHeight
        {
            get { return (Double)GetValue(ExtentHeightProperty); }
            set { SetValue(ExtentHeightProperty, value); }
        }
        #endregion
        #region P:Selection:RangeSelection
        public static readonly DependencyProperty SelectionProperty = DependencyProperty.Register("Selection", typeof(RangeSelection), typeof(HexBoxPanelH), new PropertyMetadata(default(RangeSelection), OnSelectionChanged));
        private static void OnSelectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as HexBoxPanelH);
            if (source != null)
                {
                source.OnSelectionChanged();
                }
            }

        private void OnSelectionChanged()
            {
            ShowOrHideSelection();
            }

        public RangeSelection Selection
            {
            get { return (RangeSelection)GetValue(SelectionProperty); }
            set { SetValue(SelectionProperty, value); }
            }
        #endregion
        #region P:Markups:MarkupCollection
        public static readonly DependencyProperty MarkupsProperty = DependencyProperty.Register(nameof(Markups), typeof(MarkupCollection), typeof(HexBoxPanelH), new PropertyMetadata(default(MarkupCollection), OnMarkupsChanged));
        private static void OnMarkupsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            var source = (sender as HexBoxPanelH);
            if (source != null) {
                source.OnMarkupsChanged(
                    (MarkupCollection)e.OldValue,
                    (MarkupCollection)e.NewValue);
                }
            }

        private void OnMarkupsChanged(MarkupCollection o, MarkupCollection n)
            {
            if (o != null) { o.CollectionChanged -= OnMarkupsChanged; }
            if (n != null) { n.CollectionChanged += OnMarkupsChanged; }
            }

        private void OnMarkupsChanged(Object sender, NotifyCollectionChangedEventArgs e) {
            OnPropertyChanged(nameof(Markups));
            var layer = AdornerLayer.GetAdornerLayer(this);
            if (layer != null) {
                foreach (var markup in markups) { layer.Remove(markup); }
                markups.Clear();
                if (Markups != null) {
                    foreach (var markup in Markups) {
                        var adorner = new GeometrySelectionAdorner(this) {
                            SelectionStrokeBrush = new SolidColorBrush(markup.Color),
                            SelectionFillBrush = new SolidColorBrush(markup.Color),
                            SelectionFillBrushOpacity = 0.05,
                            Visibility = Visibility.Visible,
                            };
                        markups.Add(adorner);
                        layer.Add(adorner);
                        }
                    }
                }
            InvalidateMarkups();
            }

        public MarkupCollection Markups
            {
            get { return (MarkupCollection)GetValue(MarkupsProperty); }
            set { SetValue(MarkupsProperty, value); }
            }
        #endregion
        #region P:VisibleMarkups:MarkupCollection
        private static readonly DependencyPropertyKey VisibleMarkupsPropertyKey = DependencyProperty.RegisterReadOnly("VisibleMarkups", typeof(MarkupCollection), typeof(HexBoxPanelH), new PropertyMetadata(default(MarkupCollection)));
        public static readonly DependencyProperty VisibleMarkupsProperty = VisibleMarkupsPropertyKey.DependencyProperty;
        public MarkupCollection VisibleMarkups
            {
            get { return (MarkupCollection)GetValue(VisibleMarkupsProperty); }
            private set { SetValue(VisibleMarkupsPropertyKey, value); }
            }
        #endregion
        #region P:InternalMarkers:IEnumerable
        private static readonly DependencyPropertyKey InternalMarkersPropertyKey = DependencyProperty.RegisterReadOnly("InternalMarkers", typeof(IEnumerable), typeof(HexBoxPanelH), new PropertyMetadata(default(IEnumerable)));
        public static readonly DependencyProperty InternalMarkersProperty = InternalMarkersPropertyKey.DependencyProperty;
        public IEnumerable InternalMarkers
            {
            get { return (IEnumerable)GetValue(InternalMarkersProperty); }
            private set { SetValue(InternalMarkersPropertyKey, value); }
            }
        #endregion
        #region P:CaretPosition:ValueTuple<Int64,Int16>
        internal static readonly DependencyProperty CaretPositionProperty = DependencyProperty.Register("CaretPosition", typeof(ValueTuple<Int64, Int16>), typeof(HexBoxPanelH), new PropertyMetadata(default(ValueTuple<Int64, Int16>), OnCaretPositionChanged));
        private static void OnCaretPositionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
            var source = (sender as HexBoxPanelH);
            if (source != null)
                {
                source.OnCaretPositionChanged();
                }
            }

        private void OnCaretPositionChanged()
            {
            CaretIndex = CaretPosition.Item1;
            CaretRelativePosition = CaretPosition.Item2;
            UpdateCaretPosition(CaretIndex, CaretRelativePosition);
            }

        internal ValueTuple<Int64, Int16> CaretPosition
            {
            get { return (ValueTuple<Int64, Int16>)GetValue(CaretPositionProperty); }
            set { SetValue(CaretPositionProperty, value); }
            }
        #endregion

        #region M:MeasureOverride(Size)
        /// <summary>When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class. </summary>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        /// <param name="availablesize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availablesize)
            {
            var itemsize = ItemSize;
            if (itemsize.Width > 0) {
                return new Size(PaddingLeft + (16 * 3 + 1)*itemsize.Width, 0);
                }
            return base.MeasureOverride(availablesize);
            }
        #endregion
        #region M:OnRender(DrawingContext)
        /// <summary>When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. </summary>
        /// <param name="context">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext context) {
            base.OnRender(context);
            var background = Background;
            var foreground = TextBlock.GetForeground(this);
            if (background != null)
                {
                context.DrawRectangle(background.Clone(), null, new Rect(0, 0, ActualWidth, ActualHeight));
                }
            var source = Source;
            var count = ItemsCount;
            var itemsize = ItemSize;
            var typeface = Typeface;
            if ((source != null) && (count > 0) && (itemsize.Height > 0) && (typeface != null)) {
                lock (source)
                    {
                    var fontsize = TextBlock.GetFontSize(this);
                    var H = (Int64)VerticalOffset;
                    var y = PaddingTop;
                    var offset = H * 16;
                    var buffer = new Byte[16];
                    var I = 0;
                    for (var i = H; (i < count) && (I <= (ViewportHeight + 1)); i++) {
                        I++;
                        source.Seek(offset, SeekOrigin.Begin);
                        var sz = source.Read(buffer, 0, 16);
                        var r = new StringBuilder();
                        for (var j = 0; j < sz; j++) {
                            r.AppendFormat("{0:X2}", buffer[j]);
                            if (j != 15) {
                                r.Append((j == 7)
                                    ? "  "
                                    : " ");
                                }
                            }
                        offset += sz;
                        var text = new FormattedText(r.ToString(), CultureInfo.CurrentCulture, FlowDirection, typeface, fontsize, foreground);
                        context.DrawText(text, new Point(PaddingLeft, y));
                        y += itemsize.Height;
                        }
                    }
                }
            }
        #endregion
        #region M:ShowOrHideTextCaret(Boolean)
        private void ShowOrHideTextCaret(Boolean force = false) {
            if (IsKeyboardFocused && (Selection.Length == 0)) {
                ShowTextCaret(force);
                }
            else
                {
                HideTextCaret();
                }
            ShowOrHideSelection();
            }
        #endregion
        #region M:ShowTextCaret(Boolean)
        protected virtual void ShowTextCaret(Boolean force = false) {
            if (Source != null) {
                if (TextCaret == null) {
                    TextCaret = new TextCaret(this);
                    TextCaret.SetResourceReference(TextCaret.CaretBrushProperty, "ControlTextBrushKey");
                    //TextCaret.CaretBrush = new SolidColorBrush(Colors.Red);
                    TextCaret.IsBlinking = true;
                    TextCaret.CaretHeight = ItemSize.Height;
                    var layer = AdornerLayer.GetAdornerLayer(this);
                    if (layer != null) {
                        layer.Add(TextCaret);
                        }
                    }
                TextCaret.Visibility = Visibility.Visible;
                TextCaret.OffsetX = Math.Round(TextCaretPosition.X);
                TextCaret.OffsetY = TextCaretPosition.Y;
                if (force) {
                    TextCaret.Visibility = Visibility.Hidden;
                    TextCaret.Visibility = Visibility.Visible;
                    }
                }
            }
        #endregion
        #region M:ShowOrHideSelection
        private void ShowOrHideSelection() {
            if (Selection.Length != 0) {
                if (SelectionAdorner == null) {
                    SelectionAdorner = new GeometrySelectionAdorner(this);
                    var layer = AdornerLayer.GetAdornerLayer(this);
                    if (layer != null) {
                        layer.Add(SelectionAdorner);
                        }
                    }
                SelectionAdorner.Geometry = BuildSelectionGeometry();
                SelectionAdorner.Visibility = Visibility.Visible;
                if (IsKeyboardFocused) {
                    SelectionAdorner.SelectionFillBrush = SystemColors.HighlightBrush;
                    SelectionAdorner.SelectionStrokeBrush = SystemColors.HotTrackBrush;
                    }
                else
                    {
                    SelectionAdorner.SelectionFillBrush = TextBlock.GetForeground(this);
                    SelectionAdorner.SelectionStrokeBrush = TextBlock.GetForeground(this);
                    }
                }
            else
                {
                if (SelectionAdorner != null) {
                    SelectionAdorner.Visibility = Visibility.Hidden;
                    }
                }
            }
        #endregion
        #region M:MakeVisible(Int64)
        private void MakeVisible(Int64 i) {
            var c = new Cell(i);
            var offset = (Int64)VerticalOffset;
            var y = c.Y - offset;
            if (y > ViewportHeight) {
                SetVerticalOffset(y - ViewportHeight + offset);
                }
            else if (y < 0)
                {
                SetVerticalOffset(y + offset);
                }
            }
        #endregion
        #region M:BuildSelectionGeometry:Geometry
        private Geometry BuildSelectionGeometry() {
            var r = Selection;
            if (r.Length > 0)
                {
                return BuildSelectionGeometry(r.Start, r.Length);
                }
            else
                {
                var δ = r.Start + r.Length;
                if (δ < 0)
                    {
                    return BuildSelectionGeometry(r.Start + r.Length - δ, -r.Length + δ);
                    }
                return BuildSelectionGeometry(r.Start + r.Length, -r.Length);
                }
            }
        #endregion
        #region M:BuildSelectionGeometry(Int64,Int64):Geometry
        private Geometry BuildSelectionGeometry(Int64 si, Int64 sz)
            {
            // TODO : make it cached
            var r = BuildSelection(si, sz);
            var g = Geometry.Empty;
            var η = ItemSize.Height;
            var viewport = new Rect(new Point(0, 0), PhysicalViewport);
            for (var i = 0; i < r.Length; ++i) {
                if (!r[i].IsEmpty) {
                    r[i].Offset(0, -η*VerticalOffset + PaddingTop);
                    var rc = DoubleUtil.Round(Rect.Intersect(viewport,r[i]));
                    if (!rc.IsEmpty) {
                        g = Geometry.Combine(g, new RectangleGeometry(rc),GeometryCombineMode.Union, null);
                        }
                    }
                }
            return g;
            }
        #endregion
        #region M:BuildSelection(Int64,Int64):Rect[]
        protected virtual Rect[] BuildSelection(Int64 si, Int64 sz)
            {
            var r = new List<Rect>();
            if (sz > 0) {
                var ε = si + sz;
                var α = si / 16;
                var β = si % 16;
                var γ = ε / 16;
                var δ = ε % 16;
                var η = ItemSize.Height;
                var μ = 0.0;
                if (α == γ) {
                    r.Add(new Rect(
                        new Point(ActualPlaces[β].Item1, Math.Round(η * (α - μ))),
                        new Point(ActualPlaces[δ - 1].Item5, Math.Round(η * (α - μ + 1)))
                        ));
                    }
                else
                    {
                    var ξ = 0.0;
                    if ((γ - α) > 1)
                        {
                        ξ = +1.0;
                        }
                    /* partial line from [β] to the end */
                    r.Add(new Rect(
                        new Point(ActualPlaces[ β].Item1, Math.Round(η * (α - μ))),
                        new Point(ActualPlaces[15].Item5, Math.Round(η * (α - μ + 1) + ξ))));
                    /* full line from [α] to [γ] */
                    if ((γ - α) > 1)
                        {
                        r.Add(new Rect(
                            new Point(ActualPlaces[ 0].Item1, Math.Round(η * (α - μ + 1))),
                            new Point(ActualPlaces[15].Item5, Math.Round(η * (γ - μ)))));
                        ξ = -1.0;
                        }
                    /* partial line from the begining to the [δ] */
                    if (δ > 0)
                        {
                        r.Add(new Rect(
                            new Point(ActualPlaces[    0].Item1, Math.Round(η * (γ - μ) + ξ)),
                            new Point(ActualPlaces[δ - 1].Item5, Math.Round(η * (γ - μ + 1)))
                            ));
                        }
                    }
                }
            return r.ToArray();
            }
        #endregion
        #region M:BuildMarkupGeometry:Geometry
        private Geometry BuildMarkupGeometry(Int64 firstindex, Int64 lastindex) {
            if (firstindex > lastindex) { return BuildMarkupGeometry(lastindex, firstindex); }
            firstindex = Math.Max(0, firstindex);
            lastindex = Math.Max(0, lastindex);
            lastindex = Math.Min(lastindex, ItemsCount);
            return BuildSelectionGeometry(firstindex, lastindex - firstindex);
            }
        #endregion
        #region M:HideTextCaret
        private void HideTextCaret() {
            if (TextCaret != null) {
                TextCaret.Visibility = Visibility.Hidden;
                }
            }
        #endregion

        #region M:OnLostKeyboardFocus(KeyboardFocusChangedEventArgs)
        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.LostKeyboardFocus" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. </summary>
        /// <param name="e">The <see cref="KeyboardFocusChangedEventArgs"/> that contains event data.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
            base.OnLostKeyboardFocus(e);
            ShowOrHideTextCaret();
            }
        #endregion
        #region M:OnPreviewMouseLeftButtonDown(MouseButtonEventArgs)
        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonDown" /> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> that contains the event data. The event data reports that the left mouse button was pressed.</param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e) {
            base.OnPreviewMouseLeftButtonDown(e);
            if (Focusable) { Focus(); }
            CaptureMouse();
            var caretindex = GetItemIndexFromPoint(e.GetPosition(this));
            var ci = CaretIndex;
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) {
                if (Selection.Length == 0) {
                    var δ = caretindex - ci;
                    Selection = new RangeSelection(ci, δ);
                    }
                else
                    {
                    var δ = caretindex - Selection.Start;
                    Selection = new RangeSelection(Selection.Start, δ);
                    }
                CaretIndex = caretindex;
                CaretRelativePosition = LEFT;
                }
            else
                {
                CaretIndex = caretindex;
                CaretRelativePosition = LEFT;
                Selection = new RangeSelection(CaretIndex, 0);
                IsMouseLeftButtonDown = true;
                }
            ItemIndex = CaretIndex;
            CaretPosition = new ValueTuple<Int64, Int16>(CaretIndex, CaretRelativePosition);
            ShowOrHideTextCaret();
            }
        #endregion
        #region M:OnPreviewKeyDown(KeyEventArgs)
        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.PreviewKeyDown" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> that contains the event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e) {
            if (Source == null) { return; }
            base.OnPreviewKeyDown(e);
            var selection = Selection;
            var selectionlength = selection.Length;
            var selectionstart = selection.Start;
            switch (e.Key)
                {
                #region [Right]
                case Key.Right:
                    {
                    e.Handled = true;
                    #region [Shift]+[Right]
                    if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift)) {
                        var i = CaretIndex;
                        if (selectionlength == 0) {
                            if (((i + 1) % 16) == 0) {
                                if (CaretRelativePosition == RIGHT) {
                                    if (i == ItemsCount - 1) { break; }
                                    }
                                }
                            }
                        selectionlength = Math.Min(selectionstart + selectionlength + 1, ItemsCount) - selectionstart;
                        CaretIndex = selectionstart + selectionlength;
                        CaretRelativePosition = LEFT;
                        }
                    #endregion
                    #region [Right]
                    else
                        {
                        selectionlength = 0;
                        CaretIndex = Math.Min(CaretIndex + 1, ItemsCount);
                        CaretRelativePosition = LEFT;
                        selectionstart = CaretIndex;
                        }
                    #endregion
                    }
                    break;
                #endregion
                #region [Left]
                case Key.Left:
                    {
                    e.Handled = true;
                    #region [Shift]+[Left]
                    if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift)) {
                        var i = CaretIndex;
                        if ((((i + 1) % 16) == 0) && (selectionlength == 0)) {
                            if (CaretRelativePosition == RIGHT) {
                                CaretRelativePosition = LEFT;
                                selectionstart = i + 1;
                                selectionlength = -1;
                                break;
                                }
                            }
                        if (selectionlength > 0) {
                            selectionlength--;
                            }
                        else
                            {
                            selectionlength--;
                            if ((selectionstart + selectionlength) < 0) {
                                selectionlength++;
                                }
                            }
                        CaretIndex = selectionstart + selectionlength;
                        CaretRelativePosition = LEFT;
                        }
                    #endregion
                    #region [Left]
                    else
                        {
                        if (CaretRelativePosition != RIGHT)
                            {
                            CaretIndex = Math.Max(CaretIndex - 1, 0);
                            }
                        CaretRelativePosition = LEFT;
                        selectionlength = 0;
                        selectionstart = CaretIndex;
                        }
                    #endregion
                    }
                    break;
                #endregion
                #region [Up]
                case Key.Up:
                    {
                    e.Handled = true;
                    #region [Shift]+[Up]
                    if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift)) {
                        selectionlength -= 16;
                        if ((selectionstart + selectionlength) < 0) {
                            selectionlength = -selectionstart;
                            }
                        CaretIndex = selectionstart + selectionlength;
                        }
                    #endregion
                    #region [Ctrl]+[Up]
                    else if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)) {
                        var ζ = (Int64)VerticalOffset;
                        SetVerticalOffset(ζ - 1);
                        selectionlength = 0;
                        selectionstart = CaretIndex;
                        }
                    #endregion
                    #region [Up]
                    else
                        {
                        var β = CaretIndex % 16;
                        var η = Math.Max(CaretIndex - 16, 0);
                        CaretIndex = η;
                        if ((η % 16) != β) {
                            CaretRelativePosition = LEFT;
                            }
                        selectionlength = 0;
                        selectionstart = CaretIndex;
                        }
                    #endregion
                    }
                    break;
                #endregion
                #region [Down]
                case Key.Down:
                    {
                    e.Handled = true;
                    #region [Shift]+[Down]
                    if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift)) {
                        selectionlength = Math.Min(selectionstart + selectionlength + 16, ItemsCount) - selectionstart;
                        CaretIndex = selectionstart + selectionlength;
                        }
                    #endregion
                    #region [Ctrl]+[Down]
                    else if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control)) {
                        var ζ = (Int64)VerticalOffset;
                        SetVerticalOffset(ζ + 1);
                        selectionlength = 0;
                        selectionstart = CaretIndex;
                        }
                    #endregion
                    #region [Down]
                    else
                        {
                        var β = CaretIndex % 16;
                        var η = Math.Min(CaretIndex + 16, ItemsCount);
                        CaretIndex = η;
                        if ((η % 16) != β) {
                            CaretRelativePosition = LEFT;
                            }
                        selectionlength = 0;
                        selectionstart = CaretIndex;
                        }
                    #endregion
                    }
                    break;
                #endregion
                #region [PageUp]
                case Key.PageUp:
                    {
                    e.Handled = true;
                    #region [Shift]+[PageUp]
                    if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift))
                        {
                        selectionlength -= (Int64)Math.Round(ViewportHeight*16);
                        if ((selectionstart + selectionlength) < 0) {
                            selectionlength = -selectionstart;
                            }
                        CaretIndex = selectionstart + selectionlength;
                        }
                    #endregion
                    #region [PageUp]
                    else
                        {
                        var β = CaretIndex % 16;
                        var η = Math.Max(CaretIndex - (Int64)Math.Round(ViewportHeight * 16), 0);
                        CaretIndex = η;
                        if ((η % 16) != β) {
                            CaretRelativePosition = LEFT;
                            }
                        selectionlength = 0;
                        selectionstart = CaretIndex;
                        }
                    #endregion
                    }
                    break;
                #endregion
                #region [PageDown]
                case Key.PageDown:
                    {
                    e.Handled = true;
                    #region [Shift]+[PageDown]
                    if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift)) {
                        selectionlength = Math.Min(selectionstart + selectionlength + (Int64)Math.Round(ViewportHeight*16), ItemsCount) - selectionstart;
                        CaretIndex = selectionstart + selectionlength;
                        }
                    #endregion
                    #region [PageDown]
                    else
                        {
                        var β = CaretIndex % 16;
                        var η = Math.Min(CaretIndex + (Int64)Math.Round(ViewportHeight*16), ItemsCount);
                        CaretIndex = η;
                        if ((η % 16) != β) {
                            CaretRelativePosition = LEFT;
                            }
                        selectionlength = 0;
                        selectionstart = CaretIndex;
                        }
                    #endregion
                    }
                    break;
                #endregion
                #region [End]
                case Key.End:
                    {
                    e.Handled = true;
                    #region [Shift]+[End]
                    if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift))
                        {
                        var α = (selectionstart + selectionlength) % 16;
                        var β = 15 - α;
                        selectionlength = Math.Min(selectionstart + selectionlength + β + 1, ItemsCount) - selectionstart;
                        CaretIndex = selectionstart + selectionlength;
                        }
                    #endregion
                    #region [Control]+[End]
                    else if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control))
                        {
                        CaretIndex = ItemsCount;
                        CaretRelativePosition = LEFT;
                        selectionlength = 0;
                        selectionstart = CaretIndex;
                        }
                    #endregion
                    #region [End]
                    else
                        {
                        var i = CaretIndex % 16;
                        var δ = 15 - i;
                        CaretIndex = Math.Min(CaretIndex + δ, ItemsCount);
                        i = (CaretIndex + 1) % 16;
                        CaretRelativePosition = (i == 0)
                                ? RIGHT
                                : LEFT;
                        selectionlength = 0;
                        selectionstart = CaretIndex;
                        }
                    #endregion
                    }
                    break;
                #endregion
                #region [Home]
                case Key.Home:
                    {
                    e.Handled = true;
                    #region [Shift]+[Home]
                    if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift))
                        {
                        var α = (selectionstart + selectionlength) % 16;
                        if ((selectionlength == 0) && (CaretRelativePosition == RIGHT)) {
                            selectionstart++;
                            α++;
                            }
                        selectionlength -= α;
                        if ((selectionstart + selectionlength) < 0) {
                            selectionlength = -selectionstart;
                            }
                        CaretIndex = selectionstart + selectionlength;
                        CaretRelativePosition = LEFT;
                        }
                    #endregion
                    #region [Control]+[Home]
                    else if (e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control))
                        {
                        CaretIndex = 0;
                        CaretRelativePosition = LEFT;
                        selectionlength = 0;
                        selectionstart = CaretIndex;
                        }
                    #endregion
                    #region [Home]
                    else
                        {
                        var i = CaretIndex % 16;
                        CaretIndex = Math.Max(CaretIndex - i, 0);
                        CaretRelativePosition = LEFT;
                        selectionlength = 0;
                        selectionstart = CaretIndex;
                        }
                    #endregion
                    }
                    break;
                #endregion
                }
            if (e.Handled)
                {
                ItemIndex = CaretIndex;
                CaretPosition = new ValueTuple<Int64, Int16>(CaretIndex, CaretRelativePosition);
                Selection = new RangeSelection(selectionstart, selectionlength);
                ShowOrHideTextCaret();
                }
            }
        #endregion
        #region M:OnPreviewMouseLeftButtonUp(MouseButtonEventArgs)
        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.UIElement.PreviewMouseLeftButtonUp" /> routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.</summary>
        /// <param name="e">The <see cref="MouseButtonEventArgs" /> that contains the event data. The event data reports that the left mouse button was released.</param>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnPreviewMouseLeftButtonUp(e);
            IsMouseLeftButtonDown = false;
            ReleaseMouseCapture();
            }
        #endregion
        #region M:OnPreviewMouseMove(MouseEventArgs)
        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseMove" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. </summary>
        /// <param name="e">The <see cref="MouseEventArgs" /> that contains the event data.</param>
        protected override void OnPreviewMouseMove(MouseEventArgs e) {
            base.OnPreviewMouseMove(e);
            if (IsMouseLeftButtonDown) {
                var caretindex = GetItemIndexFromPoint(e.GetPosition(this));
                var ci = CaretIndex;
                if (Selection.Length == 0) {
                    var δ = caretindex - ci;
                    Selection = new RangeSelection(ci, δ);
                    }
                else
                    {
                    var δ = caretindex - Selection.Start;
                    Selection = new RangeSelection(Selection.Start, δ);
                    }
                MakeVisible(caretindex);
                ShowOrHideTextCaret();
                }
            }
        #endregion

        /// <summary>Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.GotKeyboardFocus" /> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. </summary>
        /// <param name="e">The <see cref="KeyboardFocusChangedEventArgs" /> that contains the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
            {
            base.OnGotKeyboardFocus(e);
            ShowOrHideTextCaret();
            }

        protected void SetVerticalOffset(Double offset) {
            var source = ScrollableObject;
            if (source != null) {
                source.SetVerticalOffset(offset);
                }
            }

        #region M:UpdateCaretPosition()
        private void UpdateCaretPosition(Int64 carentindex, Int64 relation) {
            TextCaretPosition = PointFromByteIndex(carentindex, relation);
            }
        #endregion
        protected virtual Point PointFromByteIndex(Int64 carentindex, Int64 relation)
            {
            var Y_offset = (Int64)VerticalOffset;
            var X_offset = HorizontalOffset;
            if (carentindex > (ItemsCount + 1)) { throw new ArgumentOutOfRangeException(nameof(carentindex)); }
            var Y = (carentindex / 16) - Y_offset;
            if (Y > ViewportHeight)
                {
                SetVerticalOffset(Y - ViewportHeight + Y_offset);
                return PointFromByteIndex(carentindex, relation);
                }
            if (Y < 0)
                {
                SetVerticalOffset(Y + Y_offset);
                return PointFromByteIndex(carentindex, relation);
                }
            var I = carentindex % 16;
            var X = (((I > 7) ? 1 : 0) + I * 3) * ItemSize.Width + X_offset;
            if (relation == RIGHT) {
                X += ItemSize.Width*2;
                }
            return new Point(PaddingLeft + X, Y * ItemSize.Height + PaddingTop);
            }
        #region M:GetItemIndexFromPoint(Point):Int64
        private Int64 GetItemIndexFromPoint(Point pt)
            {
            var H = ItemSize.Height;
            var li = Math.Min((Int64)(DoubleUtil.ToInt64((pt.Y - H*0.5) / H) + VerticalOffset), (Int64)ExtentHeight);
            Int32 i;
                 if (pt.X <= ActualPlaces[ 0].Item1) { i =  0; }
            else if (pt.X >= ActualPlaces[15].Item1) { i = 16; }
            else
                {
                for (i = 0; i < ActualPlaces.Length - 1; ++i) {
                    var α = ActualPlaces[i].Item3;
                    var β = ActualPlaces[i].Item4;
                    if (DoubleUtil.GreaterThanOrClose(pt.X, α)) {
                        if (DoubleUtil.LessThan(pt.X, β))
                            {
                            break;
                            }
                        }
                    }
                }
            return Math.Min(li * 16 + i, ItemsCount);
            }
        #endregion

        private class Cell
            {
            public Int64 X;
            public Int64 Y;
            public Cell(Int64 i)
                {
                Y = i / 16;
                X = i % 16;
                }

            public override String ToString()
                {
                return $"{{{Y};{X}}}";
                }
            }

        protected void Select(Int64 start, Int64 length)
            {
            //if (length > 0) {
            //    selectionstart = start;
            //    selectionlength = length;
            //    }
            //else
            //    {
            //    var δ = selectionstart + selectionlength;
            //    if (δ < 0)
            //        {
            //        selectionstart = selectionstart + selectionlength - δ;
            //        selectionlength = -selectionlength + δ;
            //        }
            //    else
            //        {
            //        selectionstart = selectionstart + selectionlength;
            //        selectionlength = -selectionlength;
            //        }
            //    }
            ShowOrHideSelection();
            }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = null)
            {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
            {
            base.OnPropertyChanged(e);
            OnPropertyChanged(e.Property.Name);
            }

        private void InvalidateMarkups() {
            var n = new MarkupCollection();
            var j = 0;
            for (var i = 0; i < markups.Count; i++) {
                var markup = Markups[i];
                markups[i].Geometry = BuildMarkupGeometry(markup.FirstIndex, markup.LastIndex);
                if (!markups[i].Geometry.IsEmpty()) {
                    n.Add(markup);
                    var α = markups[i].Geometry.Bounds.TopRight.Y + 10;
                    markup.Update(α, 20 - j * 3, null);
                    j++;
                    }
                }
            var o = VisibleMarkups;
            var r = false;
            if (!ReferenceEquals(n, o)) {
                     if (o == null) { r = true; }
                else if (o.Count != n.Count) { r = true; }
                else
                     {
                     for (var i = 0; i < o.Count; i++) {
                        if (!Equals(o[i], n[i])) {
                            r = true;
                            break;
                            }
                        }
                     }
                }
            if (r)
                {
                VisibleMarkups = n;
                }
            }

        private TextCaret TextCaret;
        private Point TextCaretPosition = new Point(PaddingLeft, PaddingTop);
        private GeometrySelectionAdorner SelectionAdorner;
        protected readonly Slot<Double,Double,Double,Double,Double>[] Places;
        protected readonly Slot<Double,Double,Double,Double,Double>[] ActualPlaces;
        private Int64 CaretIndex;
        private Int16 CaretRelativePosition;
        private Int64 ItemIndex;
        private Boolean IsMouseLeftButtonDown;
        private UInt16 state;
        private readonly List<GeometrySelectionAdorner> markups = new List<GeometrySelectionAdorner>();
        }
    }