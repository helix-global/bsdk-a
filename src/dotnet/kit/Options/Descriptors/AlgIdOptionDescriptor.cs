using System;
using System.IO;
using System.Security.Cryptography;

namespace Options.Descriptors
    {
    internal class AlgIdOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "algid"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("algid:")) {
                    option = new AlgId(new Oid(source.Substring(6).Trim()));
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("algid:{OID}");
            }
        }
    }