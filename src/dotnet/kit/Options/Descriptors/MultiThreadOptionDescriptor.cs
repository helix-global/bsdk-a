using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class MultiThreadOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "mt"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("mt:")) {
                    option = new MultiThreadOption(Split(source.Substring(3).Trim()));
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("mt:{number}");
            }
        }
    }