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
using BinaryStudio.PlatformComponents;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    /// <summary>
    /// Represents ASN.1 certificate structure.
    /// </summary>
    /// <remarks>
    /// <pre style="font-family: Consolas">
    /// Certificate  ::=  SEQUENCE  {
    ///   tbsCertificate       TBSCertificate,
    ///   signatureAlgorithm   AlgorithmIdentifier,
    ///   signatureValue       BIT STRING
    ///   }
    ///
    /// TBSCertificate  ::=  SEQUENCE  {
    ///   version         [0]  EXPLICIT Version DEFAULT v1,
    ///   serialNumber         CertificateSerialNumber,
    ///   signature            AlgorithmIdentifier,
    ///   issuer               Name,
    ///   validity             Validity,
    ///   subject              Name,
    ///   subjectPublicKeyInfo SubjectPublicKeyInfo,
    ///   issuerUniqueID  [1]  IMPLICIT UniqueIdentifier OPTIONAL, # If present, version MUST be v2 or v3
    ///   subjectUniqueID [2]  IMPLICIT UniqueIdentifier OPTIONAL, # If present, version MUST be v2 or v3
    ///   extensions      [3]  EXPLICIT Extensions OPTIONAL        # If present, version MUST be v3
    ///   }
    /// Version  ::=  INTEGER  {  v1(0), v2(1), v3(2)  }
    /// CertificateSerialNumber  ::=  INTEGER
    /// Validity ::= SEQUENCE {
    ///   notBefore      Time,
    ///   notAfter       Time
    ///   }
    /// 
    /// Time ::= CHOICE {
    ///   utcTime        UTCTime,
    ///   generalTime    GeneralizedTime
    ///   }
    /// 
    /// UniqueIdentifier  ::=  BIT STRING
    /// 
    /// SubjectPublicKeyInfo  ::=  SEQUENCE  {
    ///   algorithm            AlgorithmIdentifier,
    ///   subjectPublicKey     BIT STRING
    ///   }
    /// 
    /// Extensions  ::=  SEQUENCE SIZE (1..MAX) OF Extension
    /// 
    /// Extension  ::=  SEQUENCE  {
    ///   extnID      OBJECT IDENTIFIER,
    ///   critical    BOOLEAN DEFAULT FALSE,
    ///   extnValue   OCTET STRING
    ///                  -- contains the DER encoding of an ASN.1 value
    ///                  -- corresponding to the extension type identified
    ///                  -- by extnID
    ///   }
    /// 
    /// AlgorithmIdentifier  ::=  SEQUENCE  {
    ///   algorithm               OBJECT IDENTIFIER,
    ///   parameters              ANY DEFINED BY algorithm OPTIONAL
    ///   }
    /// </pre>
    /// </remarks>
    [TypeConverter(typeof(ObjectTypeConverter))]
    public sealed class Asn1Certificate : Asn1SpecificObject, IIcaoCertificate
        {
        private String _thumbprint;
        private Asn1CertificateExtension[] extensions = EmptyArray<Asn1CertificateExtension>.Value;

        [Order( 1)] public Int32 Version { get; }
        [Order( 2)] public Asn1ByteArray SerialNumber { get; }
        [Order( 3)] public Asn1SignatureAlgorithm SignatureAlgorithm { get; }
        [Order( 4)] public Asn1RelativeDistinguishedNameSequence Issuer  { get; }
        [Order( 5)] public Asn1RelativeDistinguishedNameSequence Subject { get; }
        [Order( 6)][TypeConverter(typeof(Asn1DateTimeConverter))] public DateTime NotBefore { get; }
        [Order( 7)][TypeConverter(typeof(Asn1DateTimeConverter))] public DateTime NotAfter  { get; }
        [Order( 8)] public IAsn1CertificateSubjectPublicKeyInfo SubjectPublicKeyInfo { get; }
        [Order( 9)] public String Country { get; }
        [Order(10)] public Asn1CertificateExtensionCollection Extensions { get { return new Asn1CertificateExtensionCollection(extensions); }}
        [Order(11)] 
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
            SubjectPublicKeyInfo = new X509NoCertificateSubjectPublicKeyInfo();
            State |= ObjectState.Failed;
            State &= ~ObjectState.DisposeUnderlyingObject;
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
                                    SubjectPublicKeyInfo = key;
                                    }
                                //PublicKeyParameters = new Byte[0];
                                if (u[0][j + 5][0].Count > 1) {
                                    //PublicKeyParameters = u[0][j + 5][0][1].Body;
                                    }
                                }
                            var contextspecifics = u[0].Find(i => (i.Class == Asn1ObjectClass.ContextSpecific)).ToArray();
                            #region Extensions
                            var specific = contextspecifics.FirstOrDefault(i => ((Asn1ContextSpecificObject)i).Type == 3);
                            if (!ReferenceEquals(specific, null)) {
                                extensions = specific[0].Select(i => Asn1CertificateExtension.From(new Asn1CertificateExtension(i))).ToArray();
                                }
                            #endregion
                            Country = GetCountry(Subject) ?? GetCountry(Issuer);
                            EnsureExtendedProperties();
                            State &= ~ObjectState.Failed;
                            State |= ObjectState.DisposeUnderlyingObject;
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
            var SK = ((CertificateSubjectKeyIdentifier)Extensions?.FirstOrDefault(i => i is CertificateSubjectKeyIdentifier))?.KeyIdentifier?.ToString("FL");
            var AK = ((CertificateAuthorityKeyIdentifier)Extensions?.FirstOrDefault(i => i is CertificateAuthorityKeyIdentifier))?.KeyIdentifier?.ToString("FL");
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
            var SK = ((CertificateSubjectKeyIdentifier)Extensions?.FirstOrDefault(i => i is CertificateSubjectKeyIdentifier))?.KeyIdentifier?.ToString("FL");
            var AK = ((CertificateAuthorityKeyIdentifier)Extensions?.FirstOrDefault(i => i is CertificateAuthorityKeyIdentifier))?.KeyIdentifier?.ToString("FL");
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

        private static Boolean FilterGeneral(KeyValuePair<Asn1ObjectIdentifier, String> source) {
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
                Select(i => new KeyValuePair<Asn1ObjectIdentifier, String>(
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
                if (!IsNullOrEmpty(extensions)) {
                    WriteValue(writer, serializer, nameof(Extensions), extensions);
                    }
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
        [Browsable(false)] public override Byte[] Body { get { return base.Body; }}

        /// <summary>
        /// Releases the unmanaged resources used by the instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(Boolean disposing)
            {
            if (!State.HasFlag(ObjectState.Disposed)) {
                Dispose(ref extensions);
                base.Dispose(disposing);
                State |= ObjectState.Disposed;
                }
            }
        }
    }
