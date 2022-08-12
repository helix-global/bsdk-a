using System;
using System.Collections.Generic;

namespace Options
    {
    internal class ReportOption : OperationOptionWithParameters
        {
        public String ReportPrefix { get; }
        public String ReportName { get; }
        public ReportOption(IList<String> values)
            : base(values)
            {
            ReportPrefix = String.Empty;
            foreach(var value in values) {
                     if (value.StartsWith("Prefix=", StringComparison.OrdinalIgnoreCase)) { ReportPrefix = value.Substring(7).Trim(); }
                else if (value.StartsWith("Name=",   StringComparison.OrdinalIgnoreCase)) { ReportName   = value.Substring(5).Trim(); }
                }
            }

        public ReportOption()
            {
            ReportPrefix = String.Empty;
            }

        public override String OptionName { get { return "report"; }}
        }
    }