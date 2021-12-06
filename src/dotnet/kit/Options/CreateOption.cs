using System;
using System.Collections.Generic;

namespace Options
    {
    internal class CreateOption : OperationOptionWithParameters
        {
        public CreateOption(IList<String> values)
            :base(values)
            {
            }

        public CreateOption()
            {
            }

        public override String OptionName { get { return "create"; }}
        }
    }