using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    [CmsContentSpecificObject(IcaoObjectIdentifiers.IcaoMrtdSecurityLdsSecurityObject)]
    public sealed class IcaoLdsSecurityObject : CmsContentSpecificObject
        {
        private const Int32 INDEX_VERSION                   = 0;
        private const Int32 INDEX_DIGEST_ALGORITHM          = 1;
        private const Int32 INDEX_DATA_GROUP_HASH_VALUES    = 2;

        [Order(1)] public Int32 Version { get; }
        [Order(2)] public X509AlgorithmIdentifier HashAlgorithm { get; }
        [Order(3)] [TypeConverter(typeof(ObjectCollectionTypeConverter))] public IList<IcaoLdsSecurityObjectDataGroupHash> DataGroupHashValues { get; }
        [Browsable(false)] public override Byte[] Body { get { return base.Body; }}

        internal IcaoLdsSecurityObject(Asn1Object source)
            : base(source)
            {
            DataGroupHashValues = new List<IcaoLdsSecurityObjectDataGroupHash>();
            State |= ObjectState.Failed;
            if (source[1] is Asn1ContextSpecificObject contextspecific) {
                if (contextspecific.Type == 0) {
                    if (contextspecific.Count > 0) {
                        if (contextspecific[0] is Asn1OctetString octet) {
                            if (octet[0] is Asn1Sequence sequence) {
                                Version = (Asn1Integer)sequence[INDEX_VERSION];
                                HashAlgorithm = new X509AlgorithmIdentifier((Asn1Sequence)sequence[INDEX_DIGEST_ALGORITHM]);
                                foreach (var i in sequence[INDEX_DATA_GROUP_HASH_VALUES]) {
                                    DataGroupHashValues.Add(new IcaoLdsSecurityObjectDataGroupHash(i));
                                    }
                                State &= ~ObjectState.Failed;
                                }
                            }
                        }
                    }
                }
            DataGroupHashValues = new ReadOnlyCollection<IcaoLdsSecurityObjectDataGroupHash>(DataGroupHashValues);
            }
        }
    }