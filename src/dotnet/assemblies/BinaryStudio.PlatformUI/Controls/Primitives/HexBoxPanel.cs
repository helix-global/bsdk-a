using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    internal class HexBoxPanel : ScrollableContentControl
        {
        #region P:OriginalSource:Object
        public static readonly DependencyProperty OriginalSourceProperty = DependencyProperty.Register("OriginalSource", typeof(Object), typeof(HexBoxPanel), new PropertyMetadata(default(Object), OnOriginalSourceChanged));
        private static void OnOriginalSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is HexBoxPanel source) {
                source.OnOriginalSourceChanged();
                }
            }

        private void OnOriginalSourceChanged() {
            if (OriginalSource != null) {
                     if (OriginalSource is Stream) { Source = (Stream)OriginalSource; }
                else if (OriginalSource is Byte[]) { Source = new MemoryStream((Byte[])OriginalSource); }
                else
                    {
                    throw new NotSupportedException();
                    }
                }
            else
                {
                Source = null;
                }
            }

        public Object OriginalSource {
            get { return GetValue(OriginalSourceProperty); }
            set { SetValue(OriginalSourceProperty, value); }
            }
        #endregion
        #region P:Source:Stream
        private static readonly DependencyPropertyKey SourcePropertyKey = DependencyProperty.RegisterReadOnly("Source", typeof(Stream), typeof(HexBoxPanel), new PropertyMetadata(default(Stream), OnSourceChanged));
        public static readonly DependencyProperty SourceProperty = SourcePropertyKey.DependencyProperty;
        private static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is HexBoxPanel source) {
                source.OnSourceChanged();
                }
            }

        private void OnSourceChanged() {
            var source = Source;
            if (source != null) {
                Int64 c;
                lock (source)
                    {
                    var i = source.Position;
                    source.Seek(0, SeekOrigin.End);
                    c = source.Position;
                    source.Seek(i, SeekOrigin.Begin);
                    }
                ItemsCount = c;
                }
            else
                {
                ItemsCount = 0;
                }
            }

        public Stream Source
            {
            get { return (Stream)GetValue(SourceProperty); }
            private set { SetValue(SourcePropertyKey, value); }
            }
        #endregion
        #region P:ItemsCount:Int64
        private static readonly DependencyPropertyKey SizePropertyKey = DependencyProperty.RegisterReadOnly("ItemsCount", typeof(Int64), typeof(HexBoxPanel), new PropertyMetadata(default(Int64), OnItemsCountChanged));
        public static readonly DependencyProperty ItemsCountProperty = SizePropertyKey.DependencyProperty;
        private static void OnItemsCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is HexBoxPanel source) {
                source.OnItemsCountChanged();
                }
            }

        private void OnItemsCountChanged()
            {
            Extent = new Vector(Extent.X, (Int64)Math.Ceiling(ItemsCount / 16.0));
            }

        public Int64 ItemsCount
            {
            get { return (Int64)GetValue(ItemsCountProperty); }
            private set { SetValue(SizePropertyKey, value); }
            }
        #endregion
        #region P:ItemSize:Size
        private static readonly DependencyPropertyKey ItemSizePropertyKey = DependencyProperty.RegisterReadOnly("ItemSize", typeof(Size), typeof(HexBoxPanel), new PropertyMetadata(default(Size), OnItemSizeChanged));
        public static readonly DependencyProperty ItemSizeProperty = ItemSizePropertyKey.DependencyProperty;
        private static void OnItemSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) {
            if (sender is HexBoxPanel source) {
                source.OnItemSizeChanged();
                }
            }

        private void OnItemSizeChanged()
            {
            }

        public Size ItemSize
            {
            get { return (Size)GetValue(ItemSizeProperty); }
            private set { SetValue(ItemSizePropertyKey, value); }
            }
        #endregion
        #region P:Selection:RangeSelection
        public static readonly DependencyProperty SelectionProperty = DependencyProperty.Register("Selection", typeof(RangeSelection), typeof(HexBoxPanel), new PropertyMetadata(default(RangeSelection)));
        public RangeSelection Selection
            {
            get { return (RangeSelection)GetValue(SelectionProperty); }
            set { SetValue(SelectionProperty, value); }
            }
        #endregion

        #region M:OnPhysicalViewportChanged
        protected override void OnPhysicalViewportChanged() {
            if (ItemSize.Height > 0) {
                Viewport = new Vector(
                    PhysicalViewport.X,
                    Math.Max(0, (Int64)(PhysicalViewport.Y / ItemSize.Height) - 1));
                }
            base.OnPhysicalViewportChanged();
            }
        #endregion
        #region M:OnTypefaceChanged
        protected override void OnTypefaceChanged() {
            base.OnTypefaceChanged();
            var typeface = Typeface;
            if (typeface != null) {
                var formatter = new FormattedText("0", CultureInfo.CurrentUICulture, FlowDirection, typeface, TextBlock.GetFontSize(this), null);
                ItemSize = new Size(formatter.Width, formatter.Height);
                }
            else
                {
                ItemSize = new Size(0,0);
                }
            }
        #endregion
        }
    }