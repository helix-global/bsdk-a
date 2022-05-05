using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class ProviderTypeOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "provider"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("provider:")) {
                    option = new ProviderTypeOption(
                        Int32.Parse(source.Substring(9))
                        );
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("provider:{number}");
            }
        }
    }