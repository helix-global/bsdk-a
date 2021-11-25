using System.Diagnostics.CodeAnalysis;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    #if CODE_ANALYSIS
    [SuppressMessage("Microsoft.Design","CA1028")]
    #endif
    public enum CryptKeyBLOBType : byte
        {
        SIMPLEBLOB              = 0x1,
        PublicKey               = 0x6,
        PrivateKey              = 0x7,
        PLAINTEXTKEYBLOB        = 0x8,
        OPAQUEKEYBLOB           = 0x9,
        FullPublicKey           = 0xA,
        SYMMETRICWRAPKEYBLOB    = 0xB,
        KEYSTATEBLOB            = 0xC
        }
    }