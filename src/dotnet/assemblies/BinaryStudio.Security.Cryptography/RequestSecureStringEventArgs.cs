using System;
using System.Security;

namespace BinaryStudio.Security.Cryptography
    {
    public class RequestSecureStringEventArgs : EventArgs
        {
        public SecureString SecureString { get;set; }
        public Boolean Canceled { get;set; }
        public Boolean StoreSecureString { get;set; }
        public String Container { get;set; }
        }

    public delegate void RequestSecureStringEventHandler(Object sender, RequestSecureStringEventArgs e);
    }