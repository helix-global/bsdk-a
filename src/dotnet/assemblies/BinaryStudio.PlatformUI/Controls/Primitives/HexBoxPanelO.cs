using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    internal class HexBoxPanelO : HexBoxPanelH
        {
        public HexBoxPanelO()
            {
            Focusable = false;
            Cursor = Cursors.Arrow;
            }

        /// <summary>When overridden in a derived class, measures the size in layout required for child elements and determines a size for the <see cref="T:System.Windows.FrameworkElement" />-derived class. </summary>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        /// <param name="availablesize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        protected override Size MeasureOverride(Size availablesize)
            {
            var itemsize = ItemSize;
            if (itemsize.Width > 0) {
                var c = ItemsCount;
                return new Size(PaddingLeft + ((c > 0xFFFFFFFF) ? 16 : 8)*itemsize.Width, 0);
                }
            return base.MeasureOverride(availablesize);
            }

        /// <summary>When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing. </summary>
        /// <param name="context">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext context)
            {
            var typeface = Typeface;
            if (typeface != null)
                {
                var foreground = TextBlock.GetForeground(this);
                var fontsize = TextBlock.GetFontSize(this);
                var sz = ViewportHeight + 2;
                var verticaloffset = (Int64)VerticalOffset;
                var γ = PaddingTop;
                var c = ItemsCount;
                var i = verticaloffset*16;
                var ζ = (c > 0xFFFFFFFF) ? "{0:X16}" : "{0:X8}";
                var j = 0;
                while ((j < sz) && (i <= c))
                    {
                    var formatter = new FormattedText(String.Format(ζ, i), CultureInfo.CurrentUICulture, FlowDirection, typeface, fontsize, foreground);
                    context.DrawText(formatter, new Point(0.0, γ));
                    γ += formatter.Height;
                    i += 16;
                    j++;
                    }
                }
            }

        protected override Point PointFromByteIndex(Int64 carentindex, Int64 relation)
            {
            return default(Point);
            }

        protected override void ShowTextCaret(Boolean force = false)
            {
            }
        }
    }