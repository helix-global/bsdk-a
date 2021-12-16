using System;
using System.Collections.Generic;
using BinaryStudio.Security.Cryptography.Certificates;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Services
    {
    public interface ICryptographicOperations
        {
        Boolean IsAlive { get; }
        IList<String> Keys(CRYPT_PROVIDER_TYPE providertype, X509StoreLocation store);
        }
    }
