using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class EncryptOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "encrypt"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("encrypt:")) {
                    option = new EncryptOption(
                        source.Substring(8).
                        Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries));
                    return true;
                    }
                if (source == "encrypt")
                    {
                    option = new EncryptOption();
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("encrypt{:{indefinite,block}+}?");
            }
        }
    }