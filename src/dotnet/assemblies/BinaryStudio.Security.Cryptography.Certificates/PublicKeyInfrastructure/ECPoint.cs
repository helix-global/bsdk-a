using System.ComponentModel;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    public class ECPoint : Asn1LinkObject
        {
        public ECPoint(Asn1Object source)
            : base(source)
            {
            }
        }
    }