using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class IsMessageGroupOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "message"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source == "message") {
                    option = new MessageGroupOption(true);
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("message");
            }
        }
    }