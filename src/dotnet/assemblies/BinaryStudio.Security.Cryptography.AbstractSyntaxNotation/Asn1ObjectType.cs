namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public enum Asn1ObjectType : sbyte
        {
        EndOfContent,
        Boolean                     =  1,
        Integer                     =  2,
        BitString                   =  3,
        OctetString                 =  4,
        Null                        =  5,
        ObjectIdentifier            =  6,
        ObjectDescriptor            =  7,
        External                    =  8,
        Real                        =  9,
        Enum                        = 10,
        EmbeddedPDV                 = 11,
        RelativeObjectIdentifier    = 13,
        Sequence                    = 16,
        Set                         = 17,
        Utf8String                  = 12,
        NumericString               = 18,
        PrintableString             = 19,
        TeletexString               = 20,
        VideotexString              = 21,
        IA5String                   = 22,
        UtcTime                     = 23,
        GeneralTime                 = 24,
        GraphicString               = 25,
        VisibleString               = 26,
        GeneralString               = 27,
        UniversalString             = 28,
        UnicodeString               = 30,
        None                        = -1
        }
    }
