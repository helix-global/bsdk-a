using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class QuarantineFolderOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "quarantine"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("quarantine:")) {
                    option = new QuarantineFolderOption(
                        source.Substring(11));
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("quarantine:[folder]");
            }
        }
    }