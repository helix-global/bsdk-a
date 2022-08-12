using System;
using DocumentFormat.OpenXml.Spreadsheet;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;

namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    public class ReportCell
        {
        internal ReportCell()
            {
            HorizontalAlignment = ReportHorizontalAlignment.Left;
            Indent = 0;
            BorderThickness = new ReportThickness{
                Left = BorderStyleValues.Thin,
                Top  = BorderStyleValues.Thin,
                Right  = BorderStyleValues.Thin,
                Bottom = BorderStyleValues.Thin
                };
            Background = Colors.Transparent;
            }

        public Object Value { get;set; }
        public ReportFont Font { get;set; }
        public ReportHorizontalAlignment HorizontalAlignment { get;set; }
        public UInt32 Indent { get;set; }
        public Int32 ColumnSpan { get;set; }
        public ReportThickness BorderThickness { get;set; }
        public Color Background { get;set; }
        }
    }