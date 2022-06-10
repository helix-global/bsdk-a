using System;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    public static class ExcelReportFile
        {
        #region M:CreateCellStyleFormats(OpenXmlElement[]):CellStyleFormats
        private static CellStyleFormats CreateCellStyleFormats(params OpenXmlElement[] args) {
            return new CellStyleFormats(args) {
                Count = (UInt32)args.Length
                };
            }
        #endregion
        #region M:CreateCellFormats(OpenXmlElement[]):CellFormats
        private static CellFormats CreateCellFormats(params OpenXmlElement[] values) {
            return new CellFormats(values) {
                Count = (UInt32)values.Length
                };
            }
        #endregion
        #region M:CreateFills(OpenXmlElement[]):Fills
        private static Fills CreateFills(params OpenXmlElement[] values) {
            return new Fills(values)
                {
                Count = (UInt32)values.Length
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

        public static WorkbookStylesPart CreateDefaultWorkbookStylesPart(WorkbookPart WorkbookPart)
            {
            var WorkbookStylesPart = WorkbookPart.AddNewPart<WorkbookStylesPart>();
            WorkbookStylesPart.Stylesheet = new Stylesheet {
                MCAttributes = new MarkupCompatibilityAttributes { Ignorable = "x14ac x16r2 xr" },
                CellFormats      = CreateCellFormats(
                    /*  0 */ new CellFormat { NumberFormatId = 0, FontId = 0, FillId = 0, BorderId = 0, FormatId = 0 },
                    /*  1 */ new CellFormat { NumberFormatId = 0, FontId = 1, FillId = 0, BorderId = 0, FormatId = 0, ApplyFont = true},
                    /*  2 */ new CellFormat { NumberFormatId = 0, FontId = 1, FillId = 0, BorderId = 0, FormatId = 0, ApplyFont = true, ApplyAlignment = true, Alignment = new Alignment{ Horizontal = HorizontalAlignmentValues.Left, Indent = 1}},
                    /*  3 */ new CellFormat { NumberFormatId = 0, FontId = 1, FillId = 0, BorderId = 0, FormatId = 0, ApplyFont = true, ApplyAlignment = true, Alignment = new Alignment{ Horizontal = HorizontalAlignmentValues.Left, Indent = 2}},
                    /*  4 */ new CellFormat { NumberFormatId = 0, FontId = 1, FillId = 0, BorderId = 0, FormatId = 0, ApplyFont = true, ApplyAlignment = true, Alignment = new Alignment{ Horizontal = HorizontalAlignmentValues.Left, Indent = 3}},
                    /*  5 */ new CellFormat { NumberFormatId = 0, FontId = 1, FillId = 0, BorderId = 0, FormatId = 0, ApplyFont = true, ApplyAlignment = true, Alignment = new Alignment{ Horizontal = HorizontalAlignmentValues.Left, Indent = 4}},
                    /*  6 */ new CellFormat { NumberFormatId = 0, FontId = 2, FillId = 0, BorderId = 0, FormatId = 0, ApplyFont = true},
                    /*  7 */ new CellFormat { NumberFormatId = 0, FontId = 2, FillId = 0, BorderId = 0, FormatId = 0, ApplyFont = true, ApplyAlignment = true, Alignment = new Alignment{ Horizontal = HorizontalAlignmentValues.Left, Indent = 1}},
                    /*  8 */ new CellFormat { NumberFormatId = 0, FontId = 2, FillId = 0, BorderId = 0, FormatId = 0, ApplyFont = true, ApplyAlignment = true, Alignment = new Alignment{ Horizontal = HorizontalAlignmentValues.Left, Indent = 2}},
                    /*  9 */ new CellFormat { NumberFormatId = 0, FontId = 2, FillId = 0, BorderId = 0, FormatId = 0, ApplyFont = true, ApplyAlignment = true, Alignment = new Alignment{ Horizontal = HorizontalAlignmentValues.Left, Indent = 3}},
                    /* 10 */ new CellFormat { NumberFormatId = 0, FontId = 2, FillId = 0, BorderId = 0, FormatId = 0, ApplyFont = true, ApplyAlignment = true, Alignment = new Alignment{ Horizontal = HorizontalAlignmentValues.Left, Indent = 4}}),
                Fills            = CreateFills(new Fill{ PatternFill = new PatternFill{ PatternType = PatternValues.None }}),
                CellStyleFormats = CreateCellStyleFormats(new CellFormat { NumberFormatId = 0, FontId = 0, FillId = 0, BorderId = 0, FormatId = 0 }),
                Borders          = CreateBorders(new Border{ LeftBorder  = new LeftBorder(), RightBorder = new RightBorder(), TopBorder = new TopBorder(), BottomBorder = new BottomBorder(), DiagonalBorder = new DiagonalBorder()}),
                CellStyles       = CreateCellStyles(
                    //new CellStyle { Name = "20% - Accent1",    FormatId = 19, BuiltinId = 30, CustomBuiltin = true},
                    //new CellStyle { Name = "20% - Accent2",    FormatId = 23, BuiltinId = 34, CustomBuiltin = true},
                    //new CellStyle { Name = "20% - Accent3",    FormatId = 27, BuiltinId = 38, CustomBuiltin = true},
                    //new CellStyle { Name = "20% - Accent4",    FormatId = 31, BuiltinId = 42, CustomBuiltin = true},
                    //new CellStyle { Name = "20% - Accent5",    FormatId = 35, BuiltinId = 46, CustomBuiltin = true},
                    //new CellStyle { Name = "20% - Accent6",    FormatId = 39, BuiltinId = 50, CustomBuiltin = true},
                    //new CellStyle { Name = "40% - Accent1",    FormatId = 20, BuiltinId = 31, CustomBuiltin = true},
                    //new CellStyle { Name = "40% - Accent2",    FormatId = 24, BuiltinId = 35, CustomBuiltin = true},
                    //new CellStyle { Name = "40% - Accent3",    FormatId = 28, BuiltinId = 39, CustomBuiltin = true},
                    //new CellStyle { Name = "40% - Accent4",    FormatId = 32, BuiltinId = 43, CustomBuiltin = true},
                    //new CellStyle { Name = "40% - Accent5",    FormatId = 36, BuiltinId = 47, CustomBuiltin = true},
                    //new CellStyle { Name = "40% - Accent6",    FormatId = 40, BuiltinId = 51, CustomBuiltin = true},
                    //new CellStyle { Name = "60% - Accent1",    FormatId = 21, BuiltinId = 32, CustomBuiltin = true},
                    //new CellStyle { Name = "60% - Accent2",    FormatId = 25, BuiltinId = 36, CustomBuiltin = true},
                    //new CellStyle { Name = "60% - Accent3",    FormatId = 29, BuiltinId = 40, CustomBuiltin = true},
                    //new CellStyle { Name = "60% - Accent4",    FormatId = 33, BuiltinId = 44, CustomBuiltin = true},
                    //new CellStyle { Name = "60% - Accent5",    FormatId = 37, BuiltinId = 48, CustomBuiltin = true},
                    //new CellStyle { Name = "60% - Accent6",    FormatId = 41, BuiltinId = 52, CustomBuiltin = true},
                    //new CellStyle { Name = "Accent1",          FormatId = 18, BuiltinId = 29, CustomBuiltin = true},
                    //new CellStyle { Name = "Accent2",          FormatId = 22, BuiltinId = 33, CustomBuiltin = true},
                    //new CellStyle { Name = "Accent3",          FormatId = 26, BuiltinId = 37, CustomBuiltin = true},
                    //new CellStyle { Name = "Accent4",          FormatId = 30, BuiltinId = 41, CustomBuiltin = true},
                    //new CellStyle { Name = "Accent5",          FormatId = 34, BuiltinId = 45, CustomBuiltin = true},
                    //new CellStyle { Name = "Accent6",          FormatId = 38, BuiltinId = 49, CustomBuiltin = true},
                    //new CellStyle { Name = "Bad",              FormatId =  7, BuiltinId = 27, CustomBuiltin = true},
                    //new CellStyle { Name = "Calculation",      FormatId = 11, BuiltinId = 22, CustomBuiltin = true},
                    //new CellStyle { Name = "Check Cell",       FormatId = 13, BuiltinId = 23, CustomBuiltin = true},
                    //new CellStyle { Name = "Explanatory Text", FormatId = 16, BuiltinId = 53, CustomBuiltin = true},
                    //new CellStyle { Name = "Good",             FormatId =  6, BuiltinId = 26, CustomBuiltin = true},
                    //new CellStyle { Name = "Heading 1",        FormatId =  2, BuiltinId = 16, CustomBuiltin = true},
                    //new CellStyle { Name = "Heading 2",        FormatId =  3, BuiltinId = 17, CustomBuiltin = true},
                    //new CellStyle { Name = "Heading 3",        FormatId =  4, BuiltinId = 18, CustomBuiltin = true},
                    //new CellStyle { Name = "Heading 4",        FormatId =  5, BuiltinId = 19, CustomBuiltin = true},
                    //new CellStyle { Name = "Input",            FormatId =  9, BuiltinId = 20, CustomBuiltin = true},
                    //new CellStyle { Name = "Linked Cell",      FormatId = 12, BuiltinId = 24, CustomBuiltin = true},
                    //new CellStyle { Name = "Neutral",          FormatId =  8, BuiltinId = 28, CustomBuiltin = true},
                    //new CellStyle { Name = "Note",             FormatId = 15, BuiltinId = 10, CustomBuiltin = true},
                    //new CellStyle { Name = "Output",           FormatId = 10, BuiltinId = 21, CustomBuiltin = true},
                    //new CellStyle { Name = "Title",            FormatId =  1, BuiltinId = 15, CustomBuiltin = true},
                    //new CellStyle { Name = "Total",            FormatId = 17, BuiltinId = 25, CustomBuiltin = true},
                    //new CellStyle { Name = "Warning Text",     FormatId = 14, BuiltinId = 11, CustomBuiltin = true},
                    new CellStyle { Name = "Normal",           FormatId =  0, BuiltinId =  0}),
                Fonts = CreateFonts(
                    new Font{ FontSize = new FontSize{ Val = 11}, Color = new Color{ Theme = 1}, FontName = new FontName{ Val = "Calibri"}, FontFamilyNumbering = new FontFamilyNumbering{ Val = 2},FontCharSet = new FontCharSet{ Val = 204}, FontScheme = new FontScheme{ Val = FontSchemeValues.Minor}},
                    new Font{ FontSize = new FontSize{ Val = 10}, Color = new Color{ Theme = 1}, FontName = new FontName{ Val = "Arial"  }, FontFamilyNumbering = new FontFamilyNumbering{ Val = 2},FontCharSet = new FontCharSet{ Val = 204}},
                    new Font{ FontSize = new FontSize{ Val = 10}, Color = new Color{ Theme = 1}, FontName = new FontName{ Val = "Arial"  }, FontFamilyNumbering = new FontFamilyNumbering{ Val = 2},FontCharSet = new FontCharSet{ Val = 204},Bold = new Bold{ Val = true}}),
                DifferentialFormats = new DifferentialFormats { Count = 0 },
                TableStyles = new TableStyles{ Count = 0, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16"},
                };
            WorkbookStylesPart.Stylesheet.AddNamespaceDeclaration("mc","http://schemas.openxmlformats.org/markup-compatibility/2006");
            WorkbookStylesPart.Stylesheet.AddNamespaceDeclaration("xr","http://schemas.microsoft.com/office/spreadsheetml/2014/revision");
            WorkbookStylesPart.Stylesheet.AddNamespaceDeclaration("x14ac","http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            WorkbookStylesPart.Stylesheet.AddNamespaceDeclaration("x16r2","http://schemas.microsoft.com/office/spreadsheetml/2015/02/main");
            return WorkbookStylesPart;
            }

        #region M:CreateBorders(OpenXmlElement[]):Borders
        private static Borders CreateBorders(params OpenXmlElement[] values) {
            return new Borders(values)
                {
                Count = (UInt32)values.Length
                };
            }
        #endregion
        #region M:CreateFonts(OpenXmlElement[]):Borders
        private static Fonts CreateFonts(params OpenXmlElement[] values) {
            return new Fonts(values)
                {
                Count = (UInt32)values.Length
                };
            }
        #endregion

        public static Cell AppendChildCell(this Row row, String column, Int32 styleindex, Object value) {
            if (row != null) {
                return UpdateCellValue(row.AppendChild(
                    new Cell{
                        CellReference = $"{column}{row.RowIndex}",
                        StyleIndex = (UInt32)styleindex
                        }), value);
                }
            return null;
            }

        private static Cell UpdateCellValue(Cell target, Object value) {
            if (value != null) {
                var type = value.GetType();
                switch (value.GetType().Name) {
                    case nameof(String)         : target.DataType = CellValues.String;  target.CellValue = new CellValue((String)value);         break;
                    case nameof(Int32)          : target.DataType = CellValues.Number;  target.CellValue = new CellValue((Int32)value);          break;
                    case nameof(DateTime)       : target.DataType = CellValues.Date;    target.CellValue = new CellValue((DateTime)value);       break;
                    case nameof(DateTimeOffset) : target.DataType = CellValues.Date;    target.CellValue = new CellValue((DateTimeOffset)value); break;
                    case nameof(Boolean)        : target.DataType = CellValues.Boolean; target.CellValue = new CellValue((Boolean)value);        break;
                    case nameof(Double)         : target.DataType = CellValues.Number;  target.CellValue = new CellValue((Double)value);         break;
                    case nameof(Single)         : target.DataType = CellValues.Number;  target.CellValue = new CellValue((Single)value);         break;
                    case nameof(Decimal)        : target.DataType = CellValues.Number;  target.CellValue = new CellValue((Decimal)value);        break;
                    case nameof(Guid)           : target.DataType = CellValues.String;  target.CellValue = new CellValue(value.ToString());      break;
                    case nameof(CellFormula)    : target.CellFormula = (CellFormula)value;                                                       break;
                    default:
                        {
                        return UpdateCellValue(target, value.ToString());
                        }
                    }
                }
            else
                {
                target.CellValue = new CellValue();
                }
            return target;
            }

        public static String ExcelColumnName(Int32 index) {
            var r = String.Empty;
            while (index > 0) {
                var i = (index - 1) % 26;
                r = Convert.ToChar('A' + i) + r;
                index = (index - i) / 26;
                }
            return r;
            }
        }
    }