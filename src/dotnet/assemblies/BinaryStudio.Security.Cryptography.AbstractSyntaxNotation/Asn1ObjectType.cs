namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// ASN.1 object type.
    /// </summary>
    public enum Asn1ObjectType : sbyte
        {
        /// <summary>
        /// <see langword="EOC"/>
        /// </summary>
        EndOfContent,
        /// <summary>
        /// <see langword="BOOLEAN"/>
        /// </summary>
        Boolean                     =  1,
        /// <summary>
        /// <see langword="INTEGER"/>
        /// </summary>
        Integer                     =  2,
        /// <summary>
        /// <see langword="BIT STRING"/>
        /// </summary>
        BitString                   =  3,
        /// <summary>
        /// <see langword="OCTET STRING"/>
        /// </summary>
        OctetString                 =  4,
        /// <summary>
        /// <see langword="NULL"/>
        /// </summary>
        Null                        =  5,
        /// <summary>
        /// <see langword="OBJECT IDENTIFIER"/>
        /// </summary>
        ObjectIdentifier            =  6,
        /// <summary>
        /// <see langword="OBJECT DESCRIPTOR"/>
        /// </summary>
        ObjectDescriptor            =  7,
        /// <summary>
        /// <see langword="EXTERNAL"/>
        /// </summary>
        External                    =  8,
        /// <summary>
        /// <see langword="REAL"/>
        /// </summary>
        Real                        =  9,
        /// <summary>
        /// <see langword="ENUMERATED"/>
        /// </summary>
        Enum                        = 10,
        /// <summary>
        /// <see langword="EMBEDDED PDV"/>
        /// </summary>
        EmbeddedPDV                 = 11,
        /// <summary>
        /// <see langword="RELATIVE-OID"/>
        /// </summary>
        RelativeObjectIdentifier    = 13,
        /// <summary>
        /// <see langword="SEQUENCE"/>
        /// </summary>
        Sequence                    = 16,
        /// <summary>
        /// <see langword="SET"/>
        /// </summary>
        Set                         = 17,
        /// <summary>
        /// <see langword="UTF8STRING"/>
        /// </summary>
        Utf8String                  = 12,
        /// <summary>
        /// <see langword="NUMERICSTRING"/>
        /// </summary>
        NumericString               = 18,
        /// <summary>
        /// <see langword="PRINTABLESTRING"/>
        /// </summary>
        PrintableString             = 19,
        /// <summary>
        /// <see langword="T61STRING"/>
        /// </summary>
        TeletexString               = 20,
        /// <summary>
        /// <see langword="VIDEOTEXSTRING"/>
        /// </summary>
        VideotexString              = 21,
        /// <summary>
        /// <see langword="IA5STRING"/>
        /// </summary>
        IA5String                   = 22,
        /// <summary>
        /// <see langword="UTCTIME"/>
        /// </summary>
        UtcTime                     = 23,
        /// <summary>
        /// <see langword="GENERALIZEDTIME"/>
        /// </summary>
        GeneralTime                 = 24,
        /// <summary>
        /// <see langword="GRAPHICSTRING"/>
        /// </summary>
        GraphicString               = 25,
        /// <summary>
        /// <see langword="VISIBLESTRING"/>
        /// </summary>
        VisibleString               = 26,
        /// <summary>
        /// <see langword="GENERALSTRING"/>
        /// </summary>
        GeneralString               = 27,
        /// <summary>
        /// <see langword="UNIVERSALSTRING"/>
        /// </summary>
        UniversalString             = 28,
        /// <summary>
        /// <see langword="BMPSTRING"/>
        /// </summary>
        UnicodeString               = 30,
        /// <summary>
        /// non-ASN.1 or unknown type.
        /// </summary>
        None                        = -1
        }
    }
