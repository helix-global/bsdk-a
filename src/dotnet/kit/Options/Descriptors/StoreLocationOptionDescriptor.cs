using System;
using System.IO;
using BinaryStudio.Security.Cryptography.Certificates;

namespace Options.Descriptors
    {
    internal class StoreLocationOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "storelocation"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("storelocation:")) {
                    option = new StoreLocationOption(ToStoreLocation(source.Substring(14)));
                    return true;
                    }
                }
            return false;
            }

        #region M:ToStoreLocation(String):X509StoreLocation
        private static X509StoreLocation ToStoreLocation(String source) {
            if (source == null) { return X509StoreLocation.CurrentUser; }
            switch (source.ToUpper())
                {
                case "CURRENTUSER":  { return X509StoreLocation.CurrentUser;  }
                case "LOCALMACHINE": { return X509StoreLocation.LocalMachine; }
                case "ENTERPRISE":   { return X509StoreLocation.LocalMachineEnterprise;  }
                }
            return X509StoreLocation.CurrentUser;
            }
        #endregion

        public override void Usage(TextWriter output)
            {
            output.Write("storelocation:{CurrentUser|LocalMachine}");
            }
        }
    }