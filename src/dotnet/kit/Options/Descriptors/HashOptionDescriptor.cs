using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class HashOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "hash"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source == "hash")
                    {
                    option = new HashOption();
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("hash[:[option]+]");
            }
        }
    }