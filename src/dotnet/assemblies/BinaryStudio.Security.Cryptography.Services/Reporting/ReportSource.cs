using System;

namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    public abstract class ReportSource
        {
        private class ReportSheetFactory : IReportElementFactory<ReportSheet> {
            public ReportSheet CreateElement()
                {
                return new ReportSheet();
                }
            }

        protected ReportSource()
            {
            Sheets = new ReportSheetCollection(new ReportSheetFactory());
            }

        public ReportSheetCollection Sheets { get; }
        public ReportSheet this[Int32 key] { get {
            return Sheets[key];
            }}

        public abstract void BuildTarget();
        }
    }