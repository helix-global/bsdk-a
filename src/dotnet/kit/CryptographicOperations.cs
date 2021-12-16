using System;
using System.Collections.Generic;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using BinaryStudio.Security.Cryptography.Services;
using log4net;
using Microsoft.Win32;

namespace srv
    {
    public class CryptographicOperations : MarshalByRefObject, ICryptographicOperations
        {
        private static readonly ILogger logger = new ServiceLogger(LogManager.GetLogger(nameof(CryptographicOperations)));
        public Boolean IsAlive { get { return true; }}
        public IList<String> Keys(CRYPT_PROVIDER_TYPE providertype, X509StoreLocation store)
            {
            var r = new List<String>();
            var Is64Bit = Environment.Is64BitProcess;
            logger.Log(LogLevel.Information, $"Keys {{{providertype}}}:{{{store}}}:");
            logger.Log(LogLevel.Information, "  User Keys:");
            var j = 0;
            var flags = CryptographicContextFlags.CRYPT_SILENT|CryptographicContextFlags.CRYPT_VERIFYCONTEXT;
            if (store == X509StoreLocation.LocalMachine)
                {
                flags |= CryptographicContextFlags.CRYPT_MACHINE_KEYSET;
                }
            using (var context = new CryptographicContext(logger, providertype, flags)) {
                foreach (var i in context.Keys) {
                    var handle = Is64Bit
                        ? ((Int64)i.Handle).ToString("X16")
                        : ((Int32)i.Handle).ToString("X8");
                    var o = $"{{{handle}}}:{i.Container}";
                    logger.Log(LogLevel.Information, $"    {{{j}}}:{o}");
                    r.Add(o);
                    j++;
                    }
                }
            return r;
            }
        }
    }