using System;
using System.Collections.Generic;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    [CmsContentSpecificObject("2.23.136.1.1.2")]
    public class CSCAMasterList : CmsContentSpecificObject
        {
        public Int32 Version { get; }
        public IList<X509Certificate> Certificates { get; }

        public CSCAMasterList(Asn1Object source)
            : base(source)
            {
            var content = source[1][0][0];
            Version = (Int32)(Asn1Integer)content[0];
            Certificates = content[1].Select(i => new X509Certificate(new Asn1Certificate(i))).ToArray();
            }
        }
    }