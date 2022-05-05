using System;
using System.Collections.Generic;

namespace Options
    {
    internal class TraceOption : OperationOptionWithParameters
        {
        public TraceOption(IList<String> values)
            :base(values)
            {
            }

        public override String OptionName { get { return "trace"; }}
        }
    }