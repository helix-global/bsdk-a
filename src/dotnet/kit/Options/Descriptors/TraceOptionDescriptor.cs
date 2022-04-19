using System;
using System.Collections.Generic;
using System.IO;

namespace Options.Descriptors
    {
    internal class TraceOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "trace"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("trace:")) {
                    option = new TraceOption(
                        source.Substring(6).
                        Split(new []{','}, StringSplitOptions.RemoveEmptyEntries));
                    return true;
                    }
                if (source == "trace")
                    {
                    option = new TraceOption(new []{ "enable" });
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("trace:{enable,suspend}");
            }
        }
    }