using System;
using System.Collections.Generic;
using System.IO;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public interface ICustomCryptographicMessageProvider
        {
        Boolean VerifyMessage(SCryptographicContext context, Stream input, Stream output, CryptographicMessageFlags type, out IList<IX509Certificate> certificates, out Exception e);
        }
    }