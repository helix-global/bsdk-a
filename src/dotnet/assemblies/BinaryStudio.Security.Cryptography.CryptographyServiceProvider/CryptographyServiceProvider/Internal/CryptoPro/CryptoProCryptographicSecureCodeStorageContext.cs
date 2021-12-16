using System;
using System.DirectoryServices.AccountManagement;
using BinaryStudio.Diagnostics.Logging;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class CryptoProCryptographicSecureCodeStorageContext : CryptographicSecureCodeStorageContext
        {
        public CryptoProCryptographicSecureCodeStorageContext(IntPtr handle, ILogger logger)
            : base(handle, logger)
            {
            }

        #region M:IsSecureCodeStored(CryptographicContext):Boolean
        public override Boolean IsSecureCodeStored(SCryptographicContext context)
            {
            var uniquecontainer = context.UniqueContainer;
            if (String.IsNullOrWhiteSpace(uniquecontainer)) { return false; }
            var registry = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Crypto Pro\Settings", false);
            if (registry != null) {
                if (!context.UseMachineKeySet)
                    {
                    var upn = UserPrincipal.Current;
                    var reg = registry.OpenSubKey($@"Users\{upn.Sid.Value}\KeyDevices\passwords", false);
                    if (reg != null) {
                        var values = uniquecontainer.Split('\\');
                        if (values.Length > 0) {
                            var key = (values.Length > 3)
                                ? reg.OpenSubKey($@"{values[0]}\{values[1]}\{values[2]}")
                                : reg.OpenSubKey($@"{String.Join("\\", values)}");
                            if (key != null) {
                                return key.GetValue("passwd") != null;
                                }
                            }
                        }
                    }
                else
                    {
                    throw new NotImplementedException();
                    }
                }
            return false;
            }
        #endregion
        }
    }