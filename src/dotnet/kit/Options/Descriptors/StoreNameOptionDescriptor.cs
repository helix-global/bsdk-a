using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class StoreNameOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "storename"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("storename:")) {
                    option = new StoreNameOption(source.Substring(10));
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("storename:{My|Root|CA|CertificateAuthority|AddressBook|AuthRoot|Disallowed|TrustedPeople|TrustedPublisher|TrustedDevices|Device|{file://{folder}}}");
            }
        }
    }