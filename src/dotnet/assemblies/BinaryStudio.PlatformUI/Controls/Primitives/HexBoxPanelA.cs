using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BinaryStudio.IO;
using Microsoft.Win32;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    internal class HexBoxPanelA : HexBoxPanelH
        {
        /// <summary>When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class. </summary>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        /// <param name="availablesize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availablesize)
            {
            var itemsize = ItemSize;
            if (itemsize.Width > 0) {
                return new Size(PaddingLeft + 16*itemsize.Width, 0);
                }
            return base.MeasureOverride(availablesize);
            }

        private Boolean IsPrintableCharacter(Byte source) {
            if ((source >= 32) && (source < 128)) { return true; }
            if (source >= 128) { return true; }
            return false;
            }

        /// <summary>When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. </summary>
        /// <param name="context">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext context) {
            lock(this) {
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
                        for (var i = H;
                            (i < count) &&
                            (I <= (ViewportHeight + 1)) &&
                            (offset <= count); i++)
                            {
                            I++;
                            source.Seek(offset, SeekOrigin.Begin);
                            var sz = source.Read(buffer, 0, 16);
                            var r = new StringBuilder();
                            for (var j = 0; j < sz; j++) {
                                r.Append(IsPrintableCharacter(buffer[j])
                                    ? OEM.GetString(new Byte[]{buffer[j] })
                                    : ".");
                                }
                            offset += sz;
                            var text = new FormattedText(r.ToString(), CultureInfo.CurrentCulture, FlowDirection, typeface, fontsize, foreground);
                            context.DrawText(text, new Point(PaddingLeft, y));
                            y += itemsize.Height;
                            }
                        }
                    }
                }
            }

        protected override Point PointFromByteIndex(Int64 carentindex, Int64 relation)
            {
            Debug.Print($"{carentindex}:{(Int64)VerticalOffset}\n");
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
            var X = I * ItemSize.Width + X_offset;
            if (relation == RIGHT) {
                X += ItemSize.Width*2;
                }
            return new Point(PaddingLeft + X, Y * ItemSize.Height);
            }

        protected override void OnItemSizeChanged()
            {
            var x = PaddingLeft;
            for (var i = 0; i < 16; ++i) {
                Places[i].Item1 = x;
                Places[i].Item2 = ItemSize.Width;
                Places[i].Item3 = x - ItemSize.Width * 0.5;
                Places[i].Item4 = x + ItemSize.Width * 0.5;
                Places[i].Item5 = Math.Round(Places[i].Item1 + ItemSize.Width);
                x += Places[i].Item2;
                }
            OnHorizontalOffsetChanged();
            InvalidateVisual();
            }

        private static readonly Encoding OEM = Encoding.GetEncoding(GetACP());
        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.None)] private static extern UInt16 GetACP();

        protected override void OnSelectionChanged()
            {
            Debug.Print("A:OnSelectionChanged");
            base.OnSelectionChanged();
            }

        protected internal override void OnCanExecuteCommand(Object sender, CanExecuteRoutedEventArgs e) {
            if (!e.Handled) {
                if (ReferenceEquals(e.Command, ApplicationCommands.Copy)) {
                    e.CanExecute = (Selection.Length > 0);
                    e.Handled = true;
                    return;
                    }
                }
            }

        protected internal override void OnExecutedCommand(Object sender, ExecutedRoutedEventArgs e) {
            if (!e.Handled) {
                if (ReferenceEquals(e.Command, ApplicationCommands.Copy)) {
                    var range = Selection;
                    if (range.Length > 0) {
                        var source = Source;
                        using (source.StorePosition()) {
                            var r = new StringBuilder();
                            var block = new Byte[1024];
                            source.Seek(range.Start, SeekOrigin.Begin);
                            var count = range.Length;
                            for (;count > 0;) {
                                var c = source.Read(block, 0, Math.Min((Int32)count, block.Length));
                                if (c == 0) { break; }
                                for (var i = 0; i < c; i++) {
                                    r.Append(IsPrintableCharacter(block[i])
                                        ? OEM.GetString(new Byte[]{block[i] })
                                        : ".");
                                    }
                                count -= c;
                                }
                            Clipboard.SetText(r.ToString(), TextDataFormat.UnicodeText);
                            }
                        }
                    }
                }
            }
        }
    }