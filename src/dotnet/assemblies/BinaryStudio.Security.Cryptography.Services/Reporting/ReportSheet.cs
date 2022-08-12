using System;

namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    public class ReportSheet
        {
        private class ReportRowFactory : IReportElementFactory<ReportRow> {
            public ReportRow CreateElement()
                {
                return new ReportRow();
                }
            }

        internal ReportSheet()
            {
            Rows = new ReportRowCollection(new ReportRowFactory());
            Name = "{no-name}";
            }

        public ReportRowCollection Rows { get; }
        public String Name { get;set; }
        public ReportRow this[Int32 index] { get {
            return Rows[index];
            }}
        }
    }