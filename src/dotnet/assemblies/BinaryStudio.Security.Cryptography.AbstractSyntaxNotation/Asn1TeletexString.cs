using System;
using System.Text;
using BinaryStudio.IO;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    internal class Asn1TeletexString : Asn1String
        {
        public override Asn1ObjectType Type { get { return Asn1ObjectType.TeletexString; }}
        public override Encoding Encoding { get { return new T61Encoding(); }}

        public Asn1TeletexString(ReadOnlyMappingStream source, long forceoffset)
            : base(source, forceoffset)
            {
            }

        internal class T61Encoding : ASCIIEncoding
            {
            /// <summary>Decodes a range of bytes from a byte array into a string.</summary>
            /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
            /// <param name="index">The index of the first byte to decode.</param>
            /// <param name="count">The number of bytes to decode.</param>
            /// <returns>A <see cref="T:System.String" /> containing the results of decoding the specified sequence of bytes.</returns>
            /// <exception cref="T:System.ArgumentNullException">
            /// <paramref name="bytes" /> is <see langword="null" />.</exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index" /> or <paramref name="count" /> is less than zero. -or-
            /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range in <paramref name="bytes" />.</exception>
            /// <exception cref="T:System.Text.DecoderFallbackException">A fallback occurred (see Character Encoding in the .NET Framework for complete explanation) -and-
            /// <see cref="P:System.Text.Encoding.DecoderFallback" /> is set to <see cref="T:System.Text.DecoderExceptionFallback" />.</exception>
            public override String GetString(Byte[] bytes, Int32 index, Int32 count)
                {
                var r = new StringBuilder();
                for (var i = 0; i < count; i++) {
                    var c = (char)bytes[index + i];
                    r.Append(c);
                    }
                return r.ToString();
                }

            private static readonly char[] Table =  {
            /* 00 */ '\u0000', /* 01 */ '\u0001', /* 02 */ '\u0002', /* 03 */ '\u0003',
            /* 04 */ '\u0004', /* 05 */ '\u0005', /* 06 */ '\u0006', /* 07 */ '\u0007',
            /* 08 */ '\u0008', /* 09 */ '\u0009', /* 0A */ '\u000A', /* 0B */ '\u000B',
            /* 0C */ '\u000C', /* 0D */ '\u000D', /* 0E */ '\u000E', /* 0F */ '\u000F',
            /* 10 */ '\u0010', /* 11 */ '\u0011', /* 12 */ '\u0012', /* 13 */ '\u0013',
            /* 14 */ '\u0014', /* 15 */ '\u0015', /* 16 */ '\u0016', /* 17 */ '\u0017',
            /* 18 */ '\u0018', /* 19 */ '\u0019', /* 1A */ '\u001A', /* 1B */ '\u001B',
            /* 1C */ '\u001C', /* 1D */ '\u001D', /* 1E */ '\u001E', /* 1F */ '\u001F',
            /* 20 */ '\u0020', /* 21 */ '\u0021', /* 22 */ '\u0022', /* 23 */ Char.MaxValue,
            /* 24 */ Char.MaxValue, /* 25 */ '\u0025', /* 26 */ '\u0026', /* 27 */ '\u0027',
            /* 28 */ '\u0028', /* 29 */ '\u0029', /* 2A */ '\u002A', /* 2B */ '\u002B',
            /* 2C */ '\u002C', /* 2D */ '\u002D', /* 2E */ '\u002E', /* 2F */ '\u002F',
            /* 30 */ '\u0030', /* 31 */ '\u0031', /* 32 */ '\u0032', /* 33 */ '\u0033',
            /* 34 */ '\u0034', /* 35 */ '\u0035', /* 36 */ '\u0036', /* 37 */ '\u0037',
            /* 38 */ '\u0038', /* 39 */ '\u0039', /* 3A */ '\u003A', /* 3B */ '\u003B',
            /* 3C */ '\u003C', /* 3D */ '\u003D', /* 3E */ '\u003E', /* 3F */ '\u003F',
            /* 40 */ '\u0040', /* 41 */ '\u0041', /* 42 */ '\u0042', /* 43 */ '\u0043',
            /* 44 */ '\u0044', /* 45 */ '\u0045', /* 46 */ '\u0046', /* 47 */ '\u0047',
            /* 48 */ '\u0048', /* 49 */ '\u0049', /* 4A */ '\u004A', /* 4B */ '\u004B',
            /* 4C */ '\u004C', /* 4D */ '\u004D', /* 4E */ '\u004E', /* 4F */ '\u004F',
            /* 50 */ '\u0050', /* 51 */ '\u0051', /* 52 */ '\u0052', /* 53 */ '\u0053',
            /* 54 */ '\u0054', /* 55 */ '\u0055', /* 56 */ '\u0056', /* 57 */ '\u0057',
            /* 58 */ '\u0058', /* 59 */ '\u0059', /* 5A */ '\u005A', /* 5B */ '\u005B',
            /* 5C */ Char.MaxValue, /* 5D */ '\u005D', Char.MaxValue, /* 5F */ '\u005F',
            /* 60 */ Char.MaxValue, /* 61 */ '\u0061', /* 62 */ '\u0062', /* 63 */ '\u0063',
            /* 64 */ '\u0064', /* 65 */ '\u0065', /* 66 */ '\u0066', /* 67 */ '\u0067',
            /* 68 */ '\u0068', /* 69 */ '\u0069', /* 6A */ '\u006A', /* 6B */ '\u006B',
            /* 6C */ '\u006C', /* 6D */ '\u006D', /* 6E */ '\u006E', /* 6F */ '\u006F',
            /* 70 */ '\u0070', /* 71 */ '\u0071', /* 72 */ '\u0072', /* 73 */ '\u0073',
            /* 74 */ '\u0074', /* 75 */ '\u0075', /* 76 */ '\u0076', /* 77 */ '\u0077',
            /* 78 */ '\u0078', /* 79 */ '\u0079', /* 7A */ '\u007A', Char.MaxValue,
            /* 7C */ '\u007C', Char.MaxValue, Char.MaxValue, /* 7F */ '\u007F',
            /* 80 */ '\u0080', /* 81 */ '\u0081', /* 82 */ '\u0082', /* 83 */ '\u0083',
            /* 84 */ '\u0084', /* 85 */ '\u0085', /* 86 */ '\u0086', /* 87 */ '\u0087',
            /* 88 */ '\u0088', /* 89 */ '\u0089', /* 8A */ '\u008A', /* 8B */ '\u008B',
            /* 8C */ '\u008C', /* 8D */ '\u008D', /* 8E */ '\u008E', /* 8F */ '\u008F',
            /* 90 */ '\u0090', /* 91 */ '\u0091', /* 92 */ '\u0092', /* 93 */ '\u0093',
            /* 94 */ '\u0094', /* 95 */ '\u0095', /* 96 */ '\u0096', /* 97 */ '\u0097',
            /* 98 */ '\u0098', /* 99 */ '\u0099', /* 9A */ '\u009A', /* 9B */ '\u009B',
            /* 9C */ '\u009C', /* 9D */ '\u009D', /* 9E */ '\u009E', /* 9F */ '\u009F',
            /* A0 */ Char.MaxValue, /* A1 */ '\u00A1', /* A2 */ '\u00A2', /* A3 */ '\u00A3',
            /* A4 */ '\u0024', /* A5 */ '\u00A5', /* A6 */ '\u0023', /* A7 */ '\u00A7',
            /* A8 */ '\u00A4', Char.MaxValue, Char.MaxValue, /* AB */ '\u00AB',
            /* AC */ Char.MaxValue, Char.MaxValue, Char.MaxValue, Char.MaxValue,
            /* B0 */ '\u00B0', /* B1 */ '\u00B1', /* B2 */ '\u00B2', /* B3 */ '\u00B3',
            /* B4 */ '\u00D7', /* B5 */ '\u00B5', /* B6 */ '\u00B6', /* B7 */ '\u00B7',
            /* B8 */ '\u00F7', Char.MaxValue, Char.MaxValue, /* BB */ '\u00BB',
            /* BC */ '\u00BC', /* BD */ '\u00BD', /* BE */ '\u00BE', /* BF */ '\u00BF',
            /* C0 */ Char.MaxValue, /* C1 */ '\u0300', /* C2 */ '\u0301', /* C3 */ '\u0302',
            /* C4 */ '\u0303', /* C5 */ '\u0304', /* C6 */ '\u0306', /* C7 */ '\u0307',
            /* C8 */ '\u0308', /* C9 */ '\u00c9', /* CA */ '\u030A', /* CB */ '\u0327',
            /* CC */ '\u0332', /* CD */ '\u030B', /* CE */ '\u0328', /* CF */ '\u030C',
            /* D0 */ '\u00d0', /* D1 */ '\u00d1', /* D2 */ '\u00d2', /* D3 */ '\u00d3',
            /* D4 */ Char.MaxValue, Char.MaxValue, Char.MaxValue, Char.MaxValue,
            /* D8 */ Char.MaxValue, Char.MaxValue, Char.MaxValue, Char.MaxValue,
            /* DC */ Char.MaxValue, Char.MaxValue, Char.MaxValue, Char.MaxValue,
            /* E0 */ '\u2126', /* E1 */ '\u00C6', /* E2 */ '\u00D0', /* E3 */ '\u00AA',
            /* E4 */ '\u0126', Char.MaxValue, /* E6 */ '\u0132', /* E7 */ '\u013F',
            /* E8 */ '\u0141', /* E9 */ '\u00D8', /* EA */ '\u0152', /* EB */ '\u00BA',
            /* EC */ '\u00DE', /* ED */ '\u0166', /* EE */ '\u014A', /* EF */ '\u0149',
            /* F0 */ '\u0138', /* F1 */ '\u00E6', /* F2 */ '\u0111', /* F3 */ '\u00F0',
            /* F4 */ '\u0127', /* F5 */ '\u0131', /* F6 */ '\u0133', /* F7 */ '\u0140',
            /* F8 */ '\u0142', /* F9 */ '\u00F8', /* FA */ '\u0153', /* FB */ '\u00DF',
            /* FC */ '\u00FE', /* FD */ '\u0167', /* FE */ '\u014B', Char.MaxValue
        };
            }
        }
    }
