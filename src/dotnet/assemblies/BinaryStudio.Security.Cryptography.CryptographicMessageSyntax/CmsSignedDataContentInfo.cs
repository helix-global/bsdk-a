﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.PublicKeyInfrastructure;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

#pragma warning disable 1591

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    /// <summary>
    /// The <see cref="CmsContentInfo" /> class represents the <a href="https://datatracker.ietf.org/doc/html/rfc5652#section-5.1">signed-data</a> CMS content.
    /// <br/>The signed-data content type shall have ASN.1 type SignedData:
    /// <pre style="font-family: Consolas">
    ///   SignedData ::= SEQUENCE {
    ///     version CMSVersion,
    ///     digestAlgorithms DigestAlgorithmIdentifiers,
    ///     encapContentInfo EncapsulatedContentInfo,
    ///     certificates [0] IMPLICIT CertificateSet OPTIONAL,
    ///     crls [1] IMPLICIT RevocationInfoChoices OPTIONAL,
    ///     signerInfos SignerInfos
    ///   }
    ///   DigestAlgorithmIdentifiers ::= SET OF DigestAlgorithmIdentifier
    ///   SignerInfos ::= SET OF SignerInfo
    /// </pre>
    /// </summary>
    public class CmsSignedDataContentInfo : CmsContentInfo
        {
        private const Int32 INDEX_VERSION                   = 0;
        private const Int32 INDEX_DIGEST_ALGORITHMS         = 1;
        private const Int32 INDEX_ENCAPSULATED_CONTENT_INFO = 2;

        /// <summary>
        /// Gives a syntax version number, for compatibility
        /// with future revisions of this specification.
        /// </summary>
        public Int32 Version { get; }
        public new CmsContentSpecificObject Content { get; }

        /// <summary>
        /// A collection of message digest algorithm identifiers.
        /// </summary>
        public ISet<X509AlgorithmIdentifier> DigestAlgorithms { get; }

        /// <summary>
        /// A collection of certificates.
        /// </summary>
        public ISet<Asn1Certificate> Certificates { get; }

        /// <summary>
        /// A collection of certificate revocation lists.
        /// </summary>
        public ISet<Asn1CertificateRevocationList> CertificateRevocationList { get; }

        /// <summary>
        /// Retrieves the <see cref="ISet{CmsSignerInfo}"/> collection associated with the CMS message.
        /// </summary>
        public ISet<CmsSignerInfo> Signers { get; }
        public CmsSignedDataContentInfo(Asn1Object source)
            : base(source)
            {
            var sequence = source[0];
            var c = sequence.Count;
            Certificates = new HashSet<Asn1Certificate>();
            CertificateRevocationList = new HashSet<Asn1CertificateRevocationList>();
            Signers = new HashSet<CmsSignerInfo>();
            Version = (Int32)(Asn1Integer)sequence[INDEX_VERSION];
            DigestAlgorithms = new HashSet<X509AlgorithmIdentifier>(sequence[INDEX_DIGEST_ALGORITHMS].Select(i => new X509AlgorithmIdentifier((Asn1Sequence)i)));
            Content = CmsContentSpecificObject.From(new CmsContentSpecificObject(sequence[INDEX_ENCAPSULATED_CONTENT_INFO]));
            for (var i = INDEX_ENCAPSULATED_CONTENT_INFO + 1; i < c; i++) {
                if (sequence[i] is Asn1ContextSpecificObject contextspecific) {
                    switch (contextspecific.Type) {
                        #region Certificates
                        case 0:
                            {
                            foreach (var o in contextspecific) {
                                     if (o is Asn1Sequence) { Certificates.Add(new Asn1Certificate(o)); }
                                else if (o is Asn1ContextSpecificObject specific) {
                                    switch (specific.Type) {
                                        #region v1AttrCert
                                        case 1:
                                            {
                                            
                                            }
                                            break;
                                        #endregion
                                        }
                                    }
                                }
                            }
                            break;
                        #endregion
                        #region Certificate Revocation Lists
                        case 1:
                            {
                            foreach (var o in contextspecific) {
                                CertificateRevocationList.Add(new Asn1CertificateRevocationList(o));
                                }
                            }
                            break;
                        #endregion
                        default: throw new InvalidDataException($"Unexpected [{contextspecific.Type}] IMPLICIT tag.", Marshal.GetExceptionForHR((Int32)HRESULT.CRYPT_E_ASN1_BADTAG));
                        }
                    }
                else
                    {
                    if (sequence[i] is Asn1EndOfContent) { break; }
                    if (!(sequence[i] is Asn1Set)) { throw new ArgumentOutOfRangeException(nameof(source)); }
                    Signers.UnionWith(sequence[i].Select(o => new CmsSignerInfo(o)));
                    break;
                    }
                }
            }

        /**
         * <summary>Gets the service object of the specified type.</summary>
         * <param name="service">An object that specifies the type of service object to get.</param>
         * <returns>A service object of type <paramref name="service"/>.-or- null if there is no service object of type <paramref name="service"/>.</returns>
         * <filterpriority>2</filterpriority>
         * */
        public override Object GetService(Type service)
            {
            if (service == null) { throw new ArgumentNullException(nameof(service)); }
            return (service == Content.GetType())
                ? Content
                : base.GetService(service);
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            writer.WriteStartObject();
            writer.WriteValue(serializer, nameof(Version), Version);
            if (!IsNullOrEmpty(DigestAlgorithms)) {
                writer.WritePropertyName(nameof(DigestAlgorithms));
                writer.WriteStartArray();
                foreach (var i in DigestAlgorithms) {
                    i.WriteJson(writer, serializer);
                    }
                writer.WriteEndArray();
                }
            if (!IsNullOrEmpty(Certificates)) {
                writer.WritePropertyName(nameof(Certificates));
                writer.WriteStartObject();
                    writer.WriteValue(serializer, "Count", Certificates.Count);
                    writer.WritePropertyName("(Self)");
                    writer.WriteStartArray();
                    foreach (var i in Certificates) {
                        i.WriteJson(writer, serializer);
                        }
                    writer.WriteEndArray();
                writer.WriteEndObject();
                }
            if (!IsNullOrEmpty(Signers)) {
                writer.WritePropertyName(nameof(Signers));
                writer.WriteStartArray();
                foreach (var i in Signers) {
                    i.WriteJson(writer, serializer);
                    }
                writer.WriteEndArray();
                }
            writer.WriteEndObject();
            }
        }
    }