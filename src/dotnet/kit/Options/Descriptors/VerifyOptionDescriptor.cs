using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class VerifyOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "verify"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("verify:")) {
                    option = new VerifyOption(
                        source.Substring(7).
                        Split(new []{','}, StringSplitOptions.RemoveEmptyEntries));
                    return true;
                    }
                if (source == "verify")
                    {
                    option = new VerifyOption();
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("verify");
            }
        }
    }