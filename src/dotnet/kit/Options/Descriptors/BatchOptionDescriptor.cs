using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class BatchOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "batch"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("batch:")) {
                    option = new BatchOption(source.Substring(6).
                        Split(new []{','}, StringSplitOptions.RemoveEmptyEntries));
                    return true;
                    }
                if (source == "batch")
                    {
                    option = new BatchOption();
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("batch:{rename,serialize,extract,group,install,uninstall,asn1,report}+");
            }
        }
    }