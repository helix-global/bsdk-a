using System;
using System.Collections.Generic;

namespace Options
    {
    internal class MultiThreadOption : OperationOptionWithParameters
        {
        public Int32 NumberOfThreads { get;set; }
        public MultiThreadOption(IList<String> values)
            :base(values)
            {
            NumberOfThreads = -1;
            foreach(var value in values) {
                if (value.StartsWith("NumberOfThreads=", StringComparison.OrdinalIgnoreCase)) {
                    NumberOfThreads = Int32.Parse(value.Substring(6).Trim());
                    }
                }
            if (NumberOfThreads < 0) {
                foreach(var value in values) {
                    if (Int32.TryParse(value, out var r)) {
                        NumberOfThreads = r;
                        break;
                        }
                    }
                }
            if (NumberOfThreads < 0) {
                NumberOfThreads = 32;
                }
            }

        public MultiThreadOption()
            :base(new String[0])
            {
            NumberOfThreads = 1;
            }

        public override String OptionName { get { return "mt"; }}
        }
    }