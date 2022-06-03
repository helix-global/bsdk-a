using System;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public static class Extensions
        {
        #if NET35
        internal static Boolean HasFlag(this Asn1Object.ObjectState source, Asn1Object.ObjectState flags)
            {
            return (source & flags) == flags;
            }

        internal static Boolean HasFlag(this Asn1ReadFlags source, Asn1ReadFlags flags)
            {
            return (source & flags) == flags;
            }
        #endif
        }
    }