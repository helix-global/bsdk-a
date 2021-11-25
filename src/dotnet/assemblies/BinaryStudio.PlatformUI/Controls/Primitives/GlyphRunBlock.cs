using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using BinaryStudio.DataProcessing;

namespace BinaryStudio.PlatformUI.Controls.Primitives
    {
    internal class GlyphRunBlock : FrameworkElement
        {
        public String Source { get; }
        public GlyphRun GlyphRun { get; }
        public Double FontRenderingEmSize { get; }
        public Double LineSpacing { get; }
        public Size Size { get; }
        public GlyphRunDrawing GlyphRunDrawing { get; }
        public TextRunProperties TextRunProperties { get; }
        public Point[] GlyphOffsets { get; }
        public Double[] GlyphWidths { get; }

        public GlyphRunBlock(String source, TextRunProperties properties) {
            Source = source;
            GlyphRun = new GlyphRun();
            FontRenderingEmSize = properties.FontRenderingEmSize;
            LineSpacing = properties.Typeface.FontFamily.LineSpacing;
            TextRunProperties = properties;
            UseLayoutRounding = true;
            using (new SupportInitialize(GlyphRun)) {
                var Length = Source.Length;
                var Typeface = GetGlyphTypeface(properties.Typeface);
                var TypefaceAdvanceWidths  = Typeface.AdvanceWidths;
                var TypefaceAdvanceHeights = Typeface.AdvanceHeights;
                var Offsets  = new Point[Length];
                var AdvanceWidths = new Double[Length];
                var GlyphIndices  = new UInt16[Length];
                GlyphOffsets = new Point[Length];
                GlyphWidths  = new Double[Length];
                GlyphRun.GlyphTypeface = Typeface;
                GlyphRun.Characters = Source.ToCharArray();
                GlyphRun.FontRenderingEmSize = FontRenderingEmSize;
                var x = 0.0;
                var height = 0.0;
                for (var i = 0; i < Length; i++) {
                    GlyphIndices[i] = Typeface.CharacterToGlyphMap[Source[i]];
                    AdvanceWidths[i] = GlyphWidths[i] = TypefaceAdvanceWidths[GlyphIndices[i]]*FontRenderingEmSize;
                    GlyphOffsets[i] = new Point(x, 0);
                    x += AdvanceWidths[i];
                    height = Math.Max(height, TypefaceAdvanceHeights[GlyphIndices[i]]*FontRenderingEmSize);
                    }
                Size = new Size(x, height*LineSpacing);
                GlyphRun.GlyphIndices  = GlyphIndices;
                GlyphRun.AdvanceWidths = AdvanceWidths;
                GlyphRun.GlyphOffsets = Offsets;
                TextOptions.SetTextRenderingMode(this,TextRenderingMode.ClearType);
                }
            GlyphRunDrawing = new GlyphRunDrawing(properties.ForegroundBrush, GlyphRun);
            }

        #region M:MeasureOverride(Size):Size
        protected override Size MeasureOverride(Size availableSize) {
            return Size;
            }
        #endregion
        #region M:GetGlyphTypeface(Typeface):GlyphTypeface
        private static GlyphTypeface GetGlyphTypeface(Typeface source) {
            GlyphTypeface r;
            return source.TryGetGlyphTypeface(out r)
                ? r
                : null;
            }
        #endregion
        #region M:OnRender(DrawingContext)
        protected override void OnRender(DrawingContext context) {
            base.OnRender(context);
            if (GlyphRunDrawing != null) {
                using (new DrawingContextOperation(context,
                    new TranslateTransform(0, Math.Round(Size.Height - TextRunProperties.Typeface.CapsHeight)))) {
                    context.DrawDrawing(GlyphRunDrawing);
                    }
                }
            }
        #endregion
        }
    }