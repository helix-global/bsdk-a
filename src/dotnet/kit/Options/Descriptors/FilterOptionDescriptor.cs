using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class FilterOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "filter"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("filter:")) {
                    option = new FilterOption(source.Substring(7).Trim());
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("filter:{value}");
            }
        }
    }