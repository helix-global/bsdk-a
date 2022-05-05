using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Xml;
using BinaryStudio.PlatformComponents;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public sealed class Asn1CertificateRevocationList : Asn1LinkObject
        {
        private String ThumbprintProperty;
        private static Int32 gref;
        private Asn1CertificateRevocationListEntry[] entries = EmptyArray<Asn1CertificateRevocationListEntry>.Value;
        private Asn1CertificateExtension[] extensions = EmptyArray<Asn1CertificateExtension>.Value;

        public DateTime  EffectiveDate { get; }
        public DateTime? NextUpdate { get; }
        public Asn1RelativeDistinguishedNameSequence Issuer { get;private set; }
        public X509AlgorithmIdentifier SignatureAlgorithm { get;private set; }
        public Int32 Version { get; }
        public IList<Asn1CertificateRevocationListEntry> Entries { get { return entries; }}
        public Asn1CertificateExtensionCollection Extensions { get { return new Asn1CertificateExtensionCollection(extensions); }}
        public Asn1BitString Signature { get;private set; }
        public String Country { get; }

        public String Thumbprint { get {
            if (ThumbprintProperty == null) {
                using (var engine = SHA1.Create())
                using(var output = new MemoryStream()) {
                    UnderlyingObject.WriteTo(output);
                    output.Seek(0, SeekOrigin.Begin);
                    ThumbprintProperty = engine.ComputeHash(output).ToString("x");
                    }
                }
            return ThumbprintProperty;
            }}

        public Asn1CertificateRevocationList(Asn1Object o)
            : base(o)
            {
            Interlocked.Increment(ref gref);
            State |= ObjectState.Failed;
            State &= ~ObjectState.DisposeUnderlyingObject;
            if (o is Asn1Sequence u)
                {
                if (u.Count == 3)
                    {
                    if ((u[0] is Asn1Sequence) &&
                        (u[1] is Asn1Sequence) &&
                        (u[2] is Asn1BitString))
                        {
                        Version = (Int32)(Asn1Integer)u[0][0];
                        SignatureAlgorithm = new X509AlgorithmIdentifier((Asn1Sequence)o[0][1]);
                        Issuer = new Asn1RelativeDistinguishedNameSequence(o[0][2].
                            Select(j => new KeyValuePair<Asn1ObjectIdentifier, String>(
                                (Asn1ObjectIdentifier)j[0][0], j[0][1].ToString())));
                        EffectiveDate = (Asn1Time)o[0][3];
                        var i = 4;
                        if (o[0][i] is Asn1Time) {
                            NextUpdate = (Asn1Time)o[0][4];
                            i++;
                            }
                        if (o[0][i] is Asn1Sequence) {
                            var r = new List<Asn1CertificateRevocationListEntry>();
                            foreach (var e in o[0][i]) {
                                r.Add(new Asn1CertificateRevocationListEntry(e));
                                }
                            entries = r.ToArray();
                            i++;
                            }
                        if (o[0][i] is Asn1ContextSpecificObject) {
                            var specific = (Asn1ContextSpecificObject)o[0][i];
                            if (specific.Type == 0) {
                                extensions = o[0][i][0].Select(x => Asn1CertificateExtension.From(new Asn1CertificateExtension(x))).ToArray();
                                }
                            }
                        Signature = (Asn1BitString)o[2];
                        Country = GetCountry(Issuer);
                        State &= ~ObjectState.Failed;
                        State |= ObjectState.DisposeUnderlyingObject;
                        }
                    }
                }
            }

        #region M:ToString(Object):String
        public static String ToString(String source) {
            if (source == null) { return String.Empty; }
            var value = (String)source;
            value = value.Replace("\\=", "=");
            value = value.Replace("\\,", ",");
            value = value.Replace("\"" , "");
            value = value.Replace("/", "%2f");
            value = value.Replace(":",  "%3a");
            return value.Trim();
            }
        #endregion

        public String FriendlyName { get {
            var K = ((CertificateAuthorityKeyIdentifier)Extensions.FirstOrDefault(i => i is CertificateAuthorityKeyIdentifier))?.KeyIdentifier?.ToString("FL");
            if (String.IsNullOrWhiteSpace(K))
                {
                return (NextUpdate != null)
                    ? $"{String.Join(",", Issuer.Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value.ToString())}"))},ED=[{EffectiveDate:yyMMddhhmmss}],NU=[{NextUpdate:yyMMddhhmmss}]"
                    : $"{String.Join(",", Issuer.Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value.ToString())}"))},ED=[{EffectiveDate:yyMMddhhmmss}]";
                }
            else
                {
                return (NextUpdate != null)
                    ? $"{String.Join(",", Issuer.Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value.ToString())}"))},AKI=[{K}],ED=[{EffectiveDate:yyMMdd}],NU=[{NextUpdate:yyMMdd}]"
                    : $"{String.Join(",", Issuer.Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value.ToString())}"))},AKI=[{K}],ED=[{EffectiveDate:yyMMdd}]";
                }
            }}

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                WriteValue(writer, serializer, nameof(Version), Version);
                WriteValue(writer, serializer, nameof(EffectiveDate), EffectiveDate.ToString("O"));
                if (NextUpdate != null) {
                    writer.WritePropertyName(nameof(NextUpdate));
                    writer.WriteRaw("   ");
                    writer.WriteValue(NextUpdate.Value.ToString("O"));
                    }
                var issuer = Issuer;
                if (!IsNullOrEmpty(issuer))
                    {
                    WriteValue(writer, serializer, nameof(Issuer), issuer);
                    }
                var extensions = Extensions;
                if (!IsNullOrEmpty(extensions))
                    {
                    WriteValue(writer, serializer, nameof(Extensions), extensions);
                    }
                var entries = Entries;
                if (!IsNullOrEmpty(entries))
                    {
                    writer.WriteValue(serializer, nameof(Entries), entries);
                    }
                }
            }

        private static String GetCountry(Asn1RelativeDistinguishedNameSequence source) {
            return source.TryGetValue("2.5.4.6", out var r)
                ? r.ToString().ToLower()
                : null;
            }

        /// <summary>
        /// Releases the unmanaged resources used by the instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(Boolean disposing) {
            if (!State.HasFlag(ObjectState.Disposed)) {
                Interlocked.Decrement(ref gref);
                lock (this)
                    {
                    Dispose(ref entries);
                    Dispose(ref extensions);
                    Issuer = null;
                    Signature = null;
                    SignatureAlgorithm = null;
                    }
                base.Dispose(disposing);
                State |= ObjectState.Disposed;
                }
            }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer) {
            writer.WriteStartElement("CertificateRevocationList");
            if (!String.IsNullOrWhiteSpace(Country)) { writer.WriteAttributeString(nameof(Country), Country); }
            writer.WriteAttributeString(nameof(EffectiveDate), EffectiveDate.ToString("O"));
            if (NextUpdate != null) { writer.WriteAttributeString(nameof(NextUpdate), NextUpdate.Value.ToString("O")); }
            writer.WriteAttributeString(nameof(Thumbprint), Thumbprint);
            if ((Issuer != null) && (Issuer.Count > 0)) {
                writer.WriteStartElement("CertificateRevocationList.Issuer");
                Issuer.WriteXml(writer);
                writer.WriteEndElement();
                }
            if (!IsNullOrEmpty(Extensions)) {
                writer.WriteStartElement(nameof(Extensions));
                foreach (var extension in Extensions.OfType<Asn1CertificateExtension>()){
                    extension.WriteXml(writer);
                    }
                writer.WriteEndElement();
                }
            writer.WriteEndElement();
            }
        }
    }