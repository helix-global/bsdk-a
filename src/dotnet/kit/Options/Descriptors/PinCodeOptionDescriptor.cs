using System;
using System.IO;
using Options;
using Options.Descriptors;

namespace kit.Options.Descriptors
    {
    internal class PinCodeOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "pincode"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("pincode:")) {
                    option = new PinCodeRequestType(source.Substring(8).Trim().ToUpper());
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("pincode:{default|console|window}");
            }
        }
    }
