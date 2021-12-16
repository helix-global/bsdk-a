using System;
using System.ComponentModel;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation
    {
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <pre style="font-family: Consolas">
    /// SubjectPublicKeyInfo  ::=  SEQUENCE  {
    ///   algorithm            AlgorithmIdentifier,
    ///   subjectPublicKey     BIT STRING
    ///   }
    /// </pre>
    /// </remarks>
    [TypeConverter(typeof(ObjectTypeConverter))]
    public sealed class Asn1CertificateSubjectPublicKeyInfo : Asn1LinkObject, IAsn1CertificateSubjectPublicKeyInfo
        {
        public X509AlgorithmIdentifier AlgorithmIdentifier { get; }
        [Browsable(false)] public override Byte[] Body { get { return base.Body; }}
        [TypeConverter(typeof(X509ByteArrayTypeConverter))] public Byte[] PublicKey { get; }

        internal Asn1CertificateSubjectPublicKeyInfo(Asn1Object o)
            : base(o)
            {
            State |= ObjectState.Failed;
            if (o is Asn1Sequence u) {
                if (u.Count == 2) {
                    if (u[0] is Asn1Sequence i) {
                        AlgorithmIdentifier = new X509AlgorithmIdentifier(i);
                        }
                    PublicKey = u[1].Body;
                    State &= ~ObjectState.Failed;
                    }
                }
            }
        }
    }