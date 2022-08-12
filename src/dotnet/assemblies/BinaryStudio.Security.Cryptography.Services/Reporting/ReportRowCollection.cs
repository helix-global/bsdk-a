using System;

namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    public class ReportRowCollection : ReportCollection<Int32, ReportRow>
        {
        internal ReportRowCollection(IReportElementFactory<ReportRow> factory)
            : base(factory)
            {
            }
        }
    }