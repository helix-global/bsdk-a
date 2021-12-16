using System.Security.Cryptography;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    public class Gost3411_12_256 : CryptHashAlgorithm
        {
        public Gost3411_12_256()
            : base(new SCryptographicContext(
                new Oid(ObjectIdentifiers.szOID_CP_GOST_R3411_12_256),
                CryptographicContextFlags.CRYPT_VERIFYCONTEXT|CryptographicContextFlags.CRYPT_SILENT),
                ALG_ID.CALG_GR3411_2012_256)
            {
            }
        }
    }