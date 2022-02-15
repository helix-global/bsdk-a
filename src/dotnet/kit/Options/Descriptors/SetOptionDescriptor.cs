using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class SetOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "set"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("set:")) {
                    option = new SetOption(
                        source.Substring(4).
                        Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries));
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("set:{key}:{value}");
            }
        }
    }