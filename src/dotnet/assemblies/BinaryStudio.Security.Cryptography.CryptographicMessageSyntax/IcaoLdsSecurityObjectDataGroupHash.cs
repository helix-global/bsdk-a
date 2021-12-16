using System;
using System.ComponentModel;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    [DefaultProperty(nameof(DataGroupNumber))]
    public sealed class IcaoLdsSecurityObjectDataGroupHash : CmsObject
        {
        private const Int32 INDEX_DATA_GROUP_NUMBER     = 0;
        private const Int32 INDEX_DATA_GROUP_HASH_VALUE = 1;

        [Order(1)] public Int32 DataGroupNumber { get; }
        [Order(2)] [TypeConverter(typeof(CmsMessageDigestTypeConverter))] public Byte[] DataGroupHashValue { get; }

        internal IcaoLdsSecurityObjectDataGroupHash(Asn1Object source)
            : base(source)
            {
            State |= ObjectState.Failed;
            if (source is Asn1Sequence sequence) {
                if (sequence.Count > 1) {
                    DataGroupNumber = (Asn1Integer)sequence[INDEX_DATA_GROUP_NUMBER];
                    DataGroupHashValue = sequence[INDEX_DATA_GROUP_HASH_VALUE].Content.ToArray();
                    }
                }
            }
        }
    }