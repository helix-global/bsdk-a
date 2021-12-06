using System;
using System.Collections.Generic;

namespace Options
    {
    internal class BatchOption : OperationOptionWithParameters
        {
        public BatchOption(IList<String> values)
            : base(values)
            {
            }

        public BatchOption()
            {
            }

        public override String OptionName { get { return "batch"; }}
        }
    }