using System;
using System.Collections.Generic;

namespace Options
    {
    internal class EncryptOption : OperationOptionWithParameters
        {
        public override String OptionName { get { return "encrypt"; }}
        public EncryptOption(IList<String> values)
            :base(values)
            {
            }

        public EncryptOption()
            {
            }
        }
    }