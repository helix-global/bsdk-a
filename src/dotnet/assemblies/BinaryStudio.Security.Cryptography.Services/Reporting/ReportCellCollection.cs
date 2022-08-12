using System;

namespace BinaryStudio.Security.Cryptography.Services.Reporting
    {
    public class ReportCellCollection : ReportCollection<Int32, ReportCell>
        {
        internal ReportCellCollection(IReportElementFactory<ReportCell> factory)
            : base(factory)
            {
            }
        }
    }