using System;

namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    public class ReportSheetCollection : ReportCollection<Int32,ReportSheet>
        {
        internal ReportSheetCollection(IReportElementFactory<ReportSheet> factory)
            : base(factory)
            {
            }
        }
    }