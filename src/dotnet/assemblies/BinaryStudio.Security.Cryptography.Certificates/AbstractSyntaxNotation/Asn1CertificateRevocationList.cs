using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public sealed class Asn1CertificateRevocationList : Asn1LinkObject
        {
        private String _thumbprint;

        public DateTime  EffectiveDate { get; }
        public DateTime? NextUpdate { get; }
        public Asn1RelativeDistinguishedNameSequence Issuer { get; }
        public X509AlgorithmIdentifier SignatureAlgorithm { get; }
        public Int32 Version { get; }
        public IList<Asn1CertificateRevocationListEntry> Entries { get; }
        public Asn1CertificateExtensionCollection Extensions { get; }
        public Asn1BitString Signature { get; }
        public String Country { get; }

        public String Thumbprint { get {
            if (_thumbprint == null) {
                using (var engine = SHA1.Create())
                using(var output = new MemoryStream()) {
                    UnderlyingObject.Write(output);
                    output.Seek(0, SeekOrigin.Begin);
                    _thumbprint = engine.ComputeHash(output).ToString("X");
                    }
                }
            return _thumbprint;
            }}

        public Asn1CertificateRevocationList(Asn1Object o)
            : base(o)
            {
            State |= ObjectState.Failed;
            if (o is Asn1Sequence u)
                {
                if (u.Count == 3)
                    {
                    if ((u[0] is Asn1Sequence) &&
                        (u[1] is Asn1Sequence) &&
                        (u[2] is Asn1BitString))
                        {
                        Extensions = new Asn1CertificateExtensionCollection();
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
                        Entries = new Asn1CertificateRevocationListEntry[0];
                        if (o[0][i] is Asn1Sequence) {
                            var r = new List<Asn1CertificateRevocationListEntry>();
                            foreach (var e in o[0][i]) {
                                r.Add(new Asn1CertificateRevocationListEntry(e));
                                }
                            Entries = r;
                            i++;
                            }
                        if (o[0][i] is Asn1ContextSpecificObject) {
                            var specific = (Asn1ContextSpecificObject)o[0][i];
                            if (specific.Type == 0) {
                                Extensions = new Asn1CertificateExtensionCollection(o[0][i][0].Select(x => Asn1CertificateExtension.From(new Asn1CertificateExtension(x))).ToList());
                                }
                            }
                        Signature = (Asn1BitString)o[2];
                        Country = GetCountry(Issuer);
                        State &= ~ObjectState.Failed;
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
        }
    }