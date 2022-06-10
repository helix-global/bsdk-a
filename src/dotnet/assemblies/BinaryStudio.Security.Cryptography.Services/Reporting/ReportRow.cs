using System;

namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    public class ReportRow
        {
        private class ReportCellFactory : IReportElementFactory<ReportCell> {
            public ReportCell CreateElement()
                {
                return new ReportCell {
                    Font = new ReportFont {
                        FontName = "Arial",
                        FontSize = 10
                        }
                    };
                }
            }

        internal ReportRow()
            {
            Cells = new ReportCellCollection(new ReportCellFactory());
            }

        public ReportCellCollection Cells { get; }
        public ReportCell this[Int32 index] { get {
            return Cells[index];
            }}
        }
    }