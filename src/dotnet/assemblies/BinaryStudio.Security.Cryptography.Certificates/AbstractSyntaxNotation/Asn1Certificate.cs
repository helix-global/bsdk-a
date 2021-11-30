using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Diagnostics;
using BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    [TypeConverter(typeof(ObjectTypeConverter))]
    public sealed class Asn1Certificate : Asn1SpecificObject, IIcaoCertificate
        {
        private String _thumbprint;

        public Int32 Version { get; }
        public Asn1ByteArray SerialNumber { get; }
        public Asn1RelativeDistinguishedNameSequence Issuer  { get; }
        public Asn1RelativeDistinguishedNameSequence Subject { get; }
        public Asn1SignatureAlgorithm SignatureAlgorithm { get; }
        public DateTime NotBefore { get; }
        public DateTime NotAfter  { get; }
        #region P:[10]:Extensions:X509CertificateExtensionCollection
        public Asn1CertificateExtensionCollection Extensions { get; }
        #endregion
        public Byte[] PublicKeyParameters { get; }
        public Byte[] PublicKey { get; }
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

        public Asn1Certificate(Asn1Object o)
            : base(o)
            {
            PublicKey = new Byte[0];
            State |= ObjectState.Failed;
            if (o is Asn1Sequence u)
                {
                if ((u[0] is Asn1Sequence) &&
                    (u[1] is Asn1Sequence) &&
                    (u[2] is Asn1BitString))
                    {
                    using (new TraceScope()) {
                        try
                            {
                            var j = 0;
                            if (u[0][0] is Asn1ContextSpecificObject) {
                                Version = (Int32)(Asn1Integer)u[0][0][0];
                                j++;
                                }
                            else
                                {
                                Version = 1;
                                }
                            SerialNumber = new Asn1ByteArray(((Asn1Integer)u[0][j]));
                            SignatureAlgorithm = Asn1SignatureAlgorithm.From(new Asn1SignatureAlgorithm(u[0][j + 1]));
                            Issuer  = Make(u[0][j + 2]);
                            Subject = Make(u[0][j + 4]);
                            #region Validity
                            if (u[0][j + 3] is Asn1Sequence)
                                {
                                NotBefore = (Asn1Time)u[0][j + 3][0];
                                NotAfter  = (Asn1Time)u[0][j + 3][1];
                                }
                            else
                                {
                                State |= ObjectState.Failed;
                                return;
                                }
                            #endregion
                            if (u[0][j + 5] is Asn1Sequence) {
                                var key = new Asn1CertificateSubjectPublicKeyInfo(u[0][j + 5]);
                                if (!key.IsFailed) {
                                    var algid = key.AlgorithmIdentifier?.Identifier;
                                    if ((algid != null) && !String.Equals(SignatureAlgorithm.SignatureAlgorithm.ToString(), algid.ToString())) {
                                        //SignatureAlgorithm = new Asn1SignatureAlgorithm(algid);
                                        }
                                    PublicKey = u[0][j + 5].Body;
                                    }
                                PublicKeyParameters = new Byte[0];
                                if (u[0][j + 5][0].Count > 1) {
                                    PublicKeyParameters = u[0][j + 5][0][1].Body;
                                    }
                                }
                            var contextspecifics = u[0].Find(i => (i.Class == Asn1ObjectClass.ContextSpecific)).ToArray();
                            #region Extensions
                            var specific = contextspecifics.FirstOrDefault(i => ((Asn1ContextSpecificObject)i).Type == 3);
                            if (!ReferenceEquals(specific, null)) {
                                Extensions = new Asn1CertificateExtensionCollection(specific[0].Select(i => Asn1CertificateExtension.From(new Asn1CertificateExtension(i))).ToList());
                                }
                            #endregion
                            Country = GetCountry(Subject) ?? GetCountry(Issuer);
                            EnsureExtendedProperties();
                            State &= ~ObjectState.Failed;
                            }
                        catch (Exception)
                            {
                            #if DEBUG
                            var filename = Path.Combine(@"C:\Failed", Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + ".cer");
                            using (var writer = File.OpenWrite(filename)) {
                                u.Write(writer);
                                }
                            #endif
                            throw;
                            }
                        }
                    }
                }
            }

        [Browsable(false)]
        public String FriendlyName { get {
            var SK = ((Asn1CertificateSubjectKeyIdentifierExtension)Extensions?.FirstOrDefault(i => i is Asn1CertificateSubjectKeyIdentifierExtension))?.Value?.ToString("FL");
            var AK = ((Asn1CertificateAuthorityKeyIdentifierExtension)Extensions?.FirstOrDefault(i => i is Asn1CertificateAuthorityKeyIdentifierExtension))?.KeyIdentifier?.ToString("FL");
            if ((!String.IsNullOrWhiteSpace(SK)))
                {
                return ((String.IsNullOrWhiteSpace(AK)))
                    ? $"{String.Join(",", Subject.Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value.ToString())}"))},SKI=[{SK}],SN=[{SerialNumber}]"
                    : $"{String.Join(",", Subject.Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value.ToString())}"))},SKI=[{SK}],AKI=[{AK}],SN=[{SerialNumber}]";
                }
            else
                {
                return $"{String.Join(",", Subject.Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value.ToString())}"))},AKI=[{AK}],SN=[{SerialNumber}]";
                }
            }}

        [Browsable(false)]
        public String AlternativeFriendlyName { get {
            var SK = ((Asn1CertificateSubjectKeyIdentifierExtension)Extensions?.FirstOrDefault(i => i is Asn1CertificateSubjectKeyIdentifierExtension))?.Value?.ToString("FL");
            var AK = ((Asn1CertificateAuthorityKeyIdentifierExtension)Extensions?.FirstOrDefault(i => i is Asn1CertificateAuthorityKeyIdentifierExtension))?.KeyIdentifier?.ToString("FL");
            if ((!String.IsNullOrWhiteSpace(SK)))
                {
                return ((String.IsNullOrWhiteSpace(AK)))
                    ? $"{String.Join(",", Subject.Where(FilterGeneral).Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value.ToString())}"))},SKI=[{SK}],SN=[{SerialNumber}]"
                    : $"{String.Join(",", Subject.Where(FilterGeneral).Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value.ToString())}"))},SKI=[{SK}],AKI=[{AK}],SN=[{SerialNumber}]";
                }
            else
                {
                return $"{String.Join(",", Subject.Where(FilterGeneral).Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value.ToString())}"))},AKI=[{AK}],SN=[{SerialNumber}]";
                }
            }}

        private static Boolean FilterGeneral(KeyValuePair<Asn1ObjectIdentifier, Object> source) {
            switch (source.Key.ToString()) {
                case "2.5.4.20":
                case "2.5.4.9":
                case "1.2.840.113549.1.9.1":
                    return false;
                }
            return true;
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

        /**
         * <summary>Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.</summary>
         * <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.</returns>
         */
        public override String ToString()
            {
            return FriendlyName;
            }

        #region M:Make(Asn1Object):Asn1RelativeDistinguishedNameSequence
        internal static Asn1RelativeDistinguishedNameSequence Make(Asn1Object source) {
            return new Asn1RelativeDistinguishedNameSequence(source.
                Select(i => new KeyValuePair<Asn1ObjectIdentifier, Object>(
                    (Asn1ObjectIdentifier)i[0][0], i[0][1].ToString())));
            }
        #endregion

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                writer.WriteValue(serializer, "(Self)", FriendlyName);
                writer.WriteValue(serializer, nameof(Version), Version);
                writer.WriteValue(serializer, nameof(SerialNumber), SerialNumber.ToString());
                writer.WriteValue(serializer, nameof(SignatureAlgorithm), SignatureAlgorithm);
                writer.WriteValue(serializer, nameof(Issuer), Issuer);
                writer.WriteValue(serializer, nameof(Subject), Subject);
                writer.WriteValue(serializer, nameof(NotBefore), NotBefore);
                writer.WriteValue(serializer, nameof(NotAfter), NotAfter);
                var extensions = Extensions;
                if (!IsNullOrEmpty(extensions)) {
                    WriteValue(writer, serializer, nameof(Extensions), extensions);
                    }
                var array = PublicKey;
                writer.WritePropertyName(nameof(PublicKey));
                if (array.Length > 50) {
                    using (writer.ArrayScope(serializer)) {
                        foreach (var value in Convert.ToBase64String(array, Base64FormattingOptions.InsertLineBreaks).Split('\n')) {
                            writer.WriteValue(value);
                            }
                        }
                    }
                else
                    {
                    writer.WriteValue(array);
                    }
                //array = PublicKeyParameters;
                //writer.WritePropertyName(nameof(PublicKeyParameters));
                //if (array.Length > 50)
                //    {
                //    writer.WriteRaw(" \"");
                //    foreach (var value in Convert.ToBase64String(Content.ToArray(), Base64FormattingOptions.InsertLineBreaks).Split('\n')) {
                //        writer.WriteIndent();
                //        writer.WriteRaw($"         {value}");
                //        }
                //    writer.WriteRawValue("\"");
                //    }
                //else
                //    {
                //    writer.WriteValue(array);
                //    }
                var icao = (IIcaoCertificate)this;
                if (icao.Type != IcaoCertificateType.None) {
                    writer.WritePropertyName("ICAO");
                    using (writer.ObjectScope(serializer)) {
                        writer.WriteValue(serializer, nameof(IIcaoCertificate.Type), icao.Type);
                        }
                    }
                }
            }

        private void EnsureExtendedProperties() {
            var keyusage = Extensions.OfType<CertificateKeyUsage>().FirstOrDefault();
            if (keyusage != null) {
                var keyusageflags = keyusage.KeyUsage;
                if (keyusageflags == (X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.KeyCertSign)) {
                    #region [CSCA Self-Signed Root][CSCA Link]
                    #endregion
                    }
                }
            }

        private static String GetCountry(Asn1RelativeDistinguishedNameSequence source) {
            return source.TryGetValue("2.5.4.6", out var r)
                ? r.ToString().ToLower()
                : null;
            }

        IcaoCertificateType IIcaoCertificate.Type { get; }
        }
    }
