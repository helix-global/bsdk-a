using System;
using System.Collections.Generic;

namespace Options
    {
    internal class HashOption : OperationOptionWithParameters
        {
        public HashOption(IList<String> values)
            :base(values)
            {
            }

        public HashOption()
            {
            }

        public override String OptionName { get { return "hash"; }}
        }
    }