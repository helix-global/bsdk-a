using System;
using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;


namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    using MediaColors = System.Windows.Media.Colors;
    using MediaColor = System.Windows.Media.Color;
    public class ExcelReportSource : ReportSource
        {
        private readonly IList<Border> TargetBorderCollection = new List<Border>();
        private readonly IList<Fill> TargetFillCollection = new List<Fill>();
        private readonly IList<Font> TargetFontCollection = new List<Font>();
        private readonly IDictionary<FontMapping,UInt32> TargetFontMapping = new Dictionary<FontMapping,UInt32>();
        private readonly IDictionary<ReportThickness,UInt32> TargetBorderMapping = new Dictionary<ReportThickness,UInt32>();
        private readonly IList<CellFormat> TargetCellFormatCollection = new List<CellFormat>();
        private readonly IDictionary<CellFormatMapping,UInt32> TargetCellFormatMapping = new Dictionary<CellFormatMapping,UInt32>();
        private readonly IDictionary<MediaColor,UInt32> TargetFillMapping = new Dictionary<MediaColor, UInt32>();

        private class FontMapping
            {
            public String FontName { get; }
            public Double FontSize { get; }
            public Boolean Bold { get; }
            public FontMapping(ReportFont source) {
                if (source != null) {
                    FontName = source.FontName;
                    FontSize = source.FontSize;
                    Bold = source.Bold;
                    }
                }

            /// <summary>Serves as a hash function for a particular type. </summary>
            /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
            public override Int32 GetHashCode()
                {
                return HashCodeCombiner.GetHashCode(FontName, FontSize, Bold);
                }

            /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
            /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
            /// <param name="other">The object to compare with the current object. </param>
            public override Boolean Equals(Object other) {
                return (other is FontMapping r)
                    ? String.Equals(r.FontName, FontName, StringComparison.OrdinalIgnoreCase) && (r.FontSize == FontSize) && (r.Bold == Bold)
                    : false;
                }
            }

        private class CellFormatMapping
            {
            public FontMapping Font { get; }
            public ReportHorizontalAlignment HorizontalAlignment { get; }
            public UInt32 Indent { get; }
            public ReportThickness BorderThickness { get; }
            public MediaColor Background { get; }
            public CellFormatMapping(ReportCell source) {
                if (source != null) {
                    Font = new FontMapping(source.Font);
                    HorizontalAlignment = source.HorizontalAlignment;
                    Indent = source.Indent;
                    BorderThickness = source.BorderThickness;
                    Background = source.Background;
                    }
                }
            /// <summary>Serves as a hash function for a particular type. </summary>
            /// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
            public override Int32 GetHashCode()
                {
                return HashCodeCombiner.GetHashCode(Font, HorizontalAlignment, Indent, BorderThickness, Background);
                }

            /// <summary>Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.</summary>
            /// <returns>true if the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />; otherwise, false.</returns>
            /// <param name="other">The object to compare with the current object. </param>
            public override Boolean Equals(Object other) {
                return (other is CellFormatMapping r)
                    && Object.Equals(r.Font, Font) && (r.HorizontalAlignment == HorizontalAlignment) && (r.Indent == Indent)
                        && (r.BorderThickness.Equals(BorderThickness))
                        && (r.Background == Background);
                }
            }

        private UInt32 GetFontId(ReportFont source) {
            if (source == null) { return 0U; }
            var key = new FontMapping(source);
            if (!TargetFontMapping.TryGetValue(key, out var r)) {
                Font o;
                TargetFontCollection.Add(o = new Font{
                    FontSize = new FontSize{ Val = source.FontSize},
                    Color = new Color{ Theme = 1},
                    FontName = new FontName{ Val = source.FontName},
                    FontFamilyNumbering = new FontFamilyNumbering{ Val = 2},
                    FontCharSet = new FontCharSet{ Val = 204}
                    });
                if (source.Bold) {
                    o.Bold = new Bold {
                        Val = true
                        };
                    }
                r = (UInt32)TargetFontCollection.Count - 1;
                TargetFontMapping.Add(key, r);
                }
            return r;
            }

        private UInt32 GetFillId(MediaColor source) {
            if (source == MediaColors.Transparent) { return 0; }
            if (!TargetFillMapping.TryGetValue(source, out var r)) {
                TargetFillCollection.Add(new Fill{
                    PatternFill = new PatternFill{
                        PatternType = PatternValues.Solid,
                        BackgroundColor = new BackgroundColor{ Indexed = 64 },
                        ForegroundColor = new ForegroundColor{
                            Rgb = $"{source.A:X2}{source.R:X2}{source.G:X2}{source.B:X2}"
                            }
                        }
                    });
                r = (UInt32)TargetFillCollection.Count - 1;
                TargetFillMapping.Add(source, r);
                }
            return r;
            }

        private UInt32 GetBorderId(ReportThickness source) {
            if (source.Equals(new ReportThickness())) { return 0U; }
            if (!TargetBorderMapping.TryGetValue(source, out var r)) {
                Border o;
                TargetBorderCollection.Add(o = new Border());
                if (source.Left != BorderStyleValues.None) {
                    o.LeftBorder = new LeftBorder{
                        Style = source.Left,
                        Color = new Color{
                            Indexed = 64
                            }
                        };
                    }
                if (source.Right != BorderStyleValues.None) {
                    o.RightBorder = new RightBorder{
                        Style = source.Right,
                        Color = new Color{
                            Indexed = 64
                            }
                        };
                    }
                if (source.Top != BorderStyleValues.None) {
                    o.TopBorder = new TopBorder{
                        Style = source.Top,
                        Color = new Color{
                            Indexed = 64
                            }
                        };
                    }
                if (source.Bottom != BorderStyleValues.None) {
                    o.BottomBorder = new BottomBorder{
                        Style = source.Bottom,
                        Color = new Color{
                            Indexed = 64
                            }
                        };
                    }
                r = (UInt32)TargetBorderCollection.Count - 1;
                TargetBorderMapping.Add(source, r);
                }
            return r;
            }

        private UInt32 GetStyleId(ReportCell source) {
            if (source == null) { return 0U; }
            var key = new CellFormatMapping(source);
            if (!TargetCellFormatMapping.TryGetValue(key, out var r)) {
                CellFormat o;
                TargetCellFormatCollection.Add(o = new CellFormat{
                    NumberFormatId = 0,
                    FillId = GetFillId(source.Background),
                    BorderId = GetBorderId(source.BorderThickness),
                    FormatId = 0,
                    FontId = GetFontId(source.Font),
                    });
                if (o.FontId != 0) { o.ApplyFont = true; }
                if (o.FillId != 0) { o.ApplyFill = true; }
                if (o.BorderId != 0) { o.ApplyBorder = true; }
                if (source.HorizontalAlignment == ReportHorizontalAlignment.Left) {
                    o.Alignment = new Alignment{
                        Horizontal = HorizontalAlignmentValues.Left,
                        Indent = source.Indent
                        };
                    o.ApplyAlignment = true;
                    }
                else if (source.HorizontalAlignment == ReportHorizontalAlignment.Center) {
                    o.Alignment = new Alignment{
                        Horizontal = HorizontalAlignmentValues.Center,
                        Indent = source.Indent
                        };
                    o.ApplyAlignment = true;
                    }
                r = (UInt32)TargetCellFormatCollection.Count - 1;
                TargetCellFormatMapping.Add(key, r);
                }
            return r;
            }

        public ExcelReportSource()
            {
            TargetCellFormatCollection.Add(new CellFormat { NumberFormatId = 0, FontId = 0, FillId = 0, BorderId = 0, FormatId = 0 });
            TargetFontCollection.Add(new Font{ FontSize = new FontSize{ Val = 11}, Color = new Color{ Theme = 1}, FontName = new FontName{ Val = "Calibri"}, FontFamilyNumbering = new FontFamilyNumbering{ Val = 2},FontCharSet = new FontCharSet{ Val = 204}, FontScheme = new FontScheme{ Val = FontSchemeValues.Minor}});
            TargetBorderCollection.Add(new Border{ LeftBorder = new LeftBorder(), RightBorder = new RightBorder(), TopBorder = new TopBorder(), BottomBorder = new BottomBorder(), DiagonalBorder = new DiagonalBorder()});
            TargetFillCollection.Add(new Fill{ PatternFill = new PatternFill{ PatternType = PatternValues.None }});
            TargetFillCollection.Add(new Fill{ PatternFill = new PatternFill{ PatternType = PatternValues.Gray125 }});
            TargetFontMapping[new FontMapping(null)] = 0;
            TargetBorderMapping[new ReportThickness()] = 0;
            TargetFillMapping[MediaColors.Transparent] = 0;
            }

        public static String ColumnName(Int32 index) {
            var r = String.Empty;
            while (index > 0) {
                var i = (index - 1) % 26;
                r = Convert.ToChar('A' + i) + r;
                index = (index - i) / 26;
                }
            return r;
            }

        public override void BuildTarget() {
            using (var document = SpreadsheetDocument.Create("zzzz.xlsx", SpreadsheetDocumentType.Workbook)) {
                var WorkbookPart = document.AddWorkbookPart();
                WorkbookPart.Workbook = new Workbook();
                var Sheets = WorkbookPart.Workbook.AppendChild(new Sheets());
                var SheetId = 1U;
                foreach (var SourceSheet in this.Sheets) {
                    var TargetMergeCells = new HashSet<String>();
                    SheetData SheetData;
                    var WorksheetPart = WorkbookPart.AddNewPart<WorksheetPart>();
                    WorksheetPart.Worksheet = new Worksheet(SheetData = new SheetData()){
                        MCAttributes = new MarkupCompatibilityAttributes{
                            Ignorable = "x14ac xr xr2 xr3"
                            }
                        };
                    WorksheetPart.Worksheet.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                    WorksheetPart.Worksheet.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
                    WorksheetPart.Worksheet.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
                    WorksheetPart.Worksheet.AddNamespaceDeclaration("xr", "http://schemas.microsoft.com/office/spreadsheetml/2014/revision");
                    WorksheetPart.Worksheet.AddNamespaceDeclaration("xr2", "http://schemas.microsoft.com/office/spreadsheetml/2015/revision2");
                    WorksheetPart.Worksheet.AddNamespaceDeclaration("xr3", "http://schemas.microsoft.com/office/spreadsheetml/2016/revision3");
                    //WorksheetPart.Worksheet.AppendChildColumns(new Column{ BestFit = true, Min = 1, Max = 1, Width = 40, CustomWidth = true});
                    WorksheetPart.Worksheet.SheetFormatProperties = new SheetFormatProperties{
                        DefaultRowHeight = 15
                        };
                    Sheets.AppendChild(new Sheet
                        {
                        Name = SourceSheet.Value.Name,
                        SheetId = SheetId++,
                        Id = WorkbookPart.GetIdOfPart(WorksheetPart)
                        });
                    foreach (var SourceRow in SourceSheet.Value.Rows) {
                        var RowIndex = SourceRow.Key + 1;
                        var TargetRow = SheetData.AppendChild(new Row{ RowIndex = new UInt32Value((UInt32)RowIndex)});
                        foreach (var SourceCell in SourceRow.Value.Cells) {
                            var ColumnIndex = ColumnName(SourceCell.Key + 1);
                            var TargetCell = TargetRow.AppendChild(new Cell{
                                CellReference = $"{ColumnIndex}{RowIndex}",
                                StyleIndex = GetStyleId(SourceCell.Value)
                                }).UpdateCellValue(SourceCell.Value.Value);
                            if (SourceCell.Value.ColumnSpan > 1) {
                                TargetMergeCells.Add($"{ColumnIndex}{RowIndex}:{ColumnName(SourceCell.Key + SourceCell.Value.ColumnSpan)}{RowIndex}");
                                }
                            }
                        }
                    WorksheetPart.Worksheet.AppendChildMergeCells(
                        TargetMergeCells.Select(i => new MergeCell{
                            Reference = i
                            }));
                    }
                var WorkbookStylesPart = WorkbookPart.AddNewPart<WorkbookStylesPart>();
                WorkbookStylesPart.Stylesheet = new Stylesheet {
                    MCAttributes = new MarkupCompatibilityAttributes { Ignorable = "x14ac x16r2 xr" },
                    TableStyles = new TableStyles{ Count = 0, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16"},
                    DifferentialFormats = new DifferentialFormats { Count = 0 },
                    Fills            = CreateFills(TargetFillCollection),
                    CellStyleFormats = CreateCellStyleFormats(new CellFormat { NumberFormatId = 0, FontId = 0, FillId = 0, BorderId = 0, FormatId = 0 }),
                    Borders          = CreateBorders(TargetBorderCollection),
                    CellStyles       = CreateCellStyles(new CellStyle { Name = "Normal", FormatId =  0, BuiltinId =  0}),
                    CellFormats      = CreateCellFormats(TargetCellFormatCollection.ToArray()),
                    Fonts            = CreateFonts(TargetFontCollection.ToArray())
                    };
                WorkbookStylesPart.Stylesheet.AddNamespaceDeclaration("mc","http://schemas.openxmlformats.org/markup-compatibility/2006");
                WorkbookStylesPart.Stylesheet.AddNamespaceDeclaration("xr","http://schemas.microsoft.com/office/spreadsheetml/2014/revision");
                WorkbookStylesPart.Stylesheet.AddNamespaceDeclaration("x14ac","http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
                WorkbookStylesPart.Stylesheet.AddNamespaceDeclaration("x16r2","http://schemas.microsoft.com/office/spreadsheetml/2015/02/main");
                document.WorkbookPart.Workbook.Save();
                document.Save();
                }
            }

        #region M:CreateCellStyleFormats(OpenXmlElement[]):CellStyleFormats
        private static CellStyleFormats CreateCellStyleFormats(params OpenXmlElement[] args) {
            return new CellStyleFormats(args) {
                Count = (UInt32)args.Length
                };
            }
        #endregion
        #region M:CreateCellFormats(CellFormat[]):CellFormats
        private static CellFormats CreateCellFormats(params CellFormat[] values) {
            return new CellFormats(values.OfType<OpenXmlElement>()) {
                Count = (UInt32)values.Length
                };
            }
        #endregion
        #region M:CreateFills(IEnumerable<Fill>):Fills
        private static Fills CreateFills(IEnumerable<Fill> values) {
            var r = values.OfType<OpenXmlElement>().ToArray();
            return new Fills(r) {
                Count = (UInt32)r.Length
                };
            }
        #endregion
        #region M:CreateCellStyles(OpenXmlElement[]):CellStyles
        private static CellStyles CreateCellStyles(params OpenXmlElement[] values) {
            return new CellStyles(values) {
                Count = (UInt32)values.Length
                };
            }
        #endregion
        #region M:CreateBorders(IEnumerable<Border>):Borders
        private static Borders CreateBorders(IEnumerable<Border> values) {
            var r = values.OfType<OpenXmlElement>().ToArray();
            return new Borders(r)
                {
                Count = (UInt32)r.Length
                };
            }
        #endregion
        #region M:CreateFonts(Font[]):Fonts
        private static Fonts CreateFonts(params Font[] values) {
            return new Fonts(values.OfType<OpenXmlElement>())
                {
                Count = (UInt32)values.Length
                };
            }
        #endregion
        }
    }