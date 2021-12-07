using System;
using System.Collections.Generic;

namespace Options
    {
    internal class InfrastructureOption : OperationOptionWithParameters
        {
        public InfrastructureOption(IList<String> values)
            :base(values)
            {
            }

        public InfrastructureOption()
            {
            }
        public override String OptionName { get { return "infrastructure"; }}
        }
    }