using System;
using System.Collections.Generic;

namespace Options
    {
    internal class ReportOption : OperationOptionWithParameters
        {
        public String Prefix { get; }
        public ReportOption(IList<String> values)
            : base(values)
            {
            Prefix = String.Empty;
            foreach(var value in values) {
                if (value.StartsWith("Prefix=", StringComparison.OrdinalIgnoreCase)) {
                    Prefix = value.Substring(7).Trim();
                    }
                }
            }

        public ReportOption()
            {
            Prefix = String.Empty;
            }

        public override String OptionName { get { return "report"; }}
        }
    }