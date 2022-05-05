using System;
using System.IO;
using Options;
using Options.Descriptors;

namespace kit.Options.Descriptors
    {
    internal class OutputTypeOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "output-type"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("output-type:")) {
                    option = new OutputTypeOption(source.Substring(12).Trim());
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("output-type:{json}");
            }
        }
    }