using System.ComponentModel;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    internal class ECFiniteField : Asn1LinkObject
        {
        protected internal ECFiniteField(Asn1Object source)
            : base(source)
            {
            }
        }
    }