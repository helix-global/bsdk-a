using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    [DebuggerDisplay("Foreground={ForegroundBrush}")]
    public sealed class CustomTextRunProperties : TextRunProperties
        {
        public override Typeface Typeface { get; }
        public override Double FontRenderingEmSize { get; }
        public override Double FontHintingEmSize { get; }
        public override TextDecorationCollection TextDecorations { get; }
        public override Brush ForegroundBrush { get; }
        public override Brush BackgroundBrush { get; }
        public override CultureInfo CultureInfo { get; }
        public override TextEffectCollection TextEffects { get; }

        public CustomTextRunProperties(FrameworkElement source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var c = source as Control;
            if (c != null) {
                Typeface = new Typeface(c.FontFamily, c.FontStyle, c.FontWeight, c.FontStretch);
                ForegroundBrush = c.Foreground;
                BackgroundBrush = c.Background;
                FontHintingEmSize = c.FontSize;
                FontRenderingEmSize = c.FontSize;
                }
            else
                {
                Typeface = new Typeface(
                    TextBlock.GetFontFamily(source),
                    TextBlock.GetFontStyle(source),
                    TextBlock.GetFontWeight(source),
                    TextBlock.GetFontStretch(source));
                ForegroundBrush = TextBlock.GetForeground(source);
                BackgroundBrush = Brushes.Transparent;
                FontRenderingEmSize = TextBlock.GetFontSize(source);
                FontHintingEmSize = FontRenderingEmSize;
                }
            CultureInfo = CultureInfo.CurrentUICulture;
            TextDecorations = new TextDecorationCollection();
            TextEffects = new TextEffectCollection();
            }

        public CustomTextRunProperties(TextRunProperties source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Typeface = source.Typeface;
            FontRenderingEmSize = source.FontRenderingEmSize;
            FontHintingEmSize = source.FontHintingEmSize;
            TextDecorations = (source.TextDecorations != null)
                ? new TextDecorationCollection(source.TextDecorations)
                : new TextDecorationCollection();
            ForegroundBrush = source.ForegroundBrush;
            BackgroundBrush = source.BackgroundBrush;
            CultureInfo = source.CultureInfo;
            TextEffects = (source.TextEffects != null)
                ? new TextEffectCollection(source.TextEffects)
                : new TextEffectCollection();
            }

        public CustomTextRunProperties(TextRunProperties source, Brush foreground)
            :this(source)
            {
            ForegroundBrush = foreground;
            }

        #if DEBUG
        public override String ToString() {
            return ForegroundBrush?.ToString();
            }
        #endif
        }
    }