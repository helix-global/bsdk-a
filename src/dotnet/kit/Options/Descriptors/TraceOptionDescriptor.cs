using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class TraceOptionDescriptor : OptionDescriptor
        {
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source == "trace")
                    {
                    option = new TraceOption(true);
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("trace");
            }
        }
    }