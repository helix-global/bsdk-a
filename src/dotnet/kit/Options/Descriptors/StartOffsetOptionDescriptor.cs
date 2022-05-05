using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class StartOffsetOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "start"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("start:")) {
                    option = new StartOffset(source.Substring(6).Trim());
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("start:{value}");
            }
        }
    }