using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class ListOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "list"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source == "list") {
                    option = new ListOption(true);
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("list");
            }
        }
    }