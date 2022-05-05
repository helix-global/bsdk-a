using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using BinaryStudio.DataProcessing;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Diagnostics;
using BinaryStudio.DirectoryServices;
using BinaryStudio.PlatformComponents;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters;
using BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation;
using BinaryStudio.Serialization;
using Newtonsoft.Json;
using BinaryStudio.Security.Cryptography.Certificates;

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
    public sealed class Asn1Certificate : Asn1SpecificObject, IIcaoCertificate, IFileService
        {
        private String thumbprint;
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
            if (thumbprint == null) {
                using (var engine = SHA1.Create())
                using(var output = new MemoryStream()) {
                    UnderlyingObject.WriteTo(output);
                    output.Seek(0, SeekOrigin.Begin);
                    thumbprint = engine.ComputeHash(output).ToString("x");
                    }
                }
            return thumbprint;
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
                        catch (Exception e)
                            {
                            #if DEBUG
                            var filename = Path.Combine(@"C:\Failed", Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + ".cer");
                            using (var writer = File.OpenWrite(filename)) {
                                u.WriteTo(writer);
                                }
                            e.Add("FailedFileName", filename);
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
            var r = new List<KeyValuePair<Asn1ObjectIdentifier, String>>();
            foreach (var i in source) {
                if (i.Count > 0) {
                    var j = i[0];
                    if (j.Count > 1) {
                        r.Add(new KeyValuePair<Asn1ObjectIdentifier, String>((Asn1ObjectIdentifier)
                            j[0],
                            j[1].ToString()));
                        continue;
                        }
                    else if (j.Count == 1)
                        {
                        r.Add(new KeyValuePair<Asn1ObjectIdentifier, String>((Asn1ObjectIdentifier)
                            j[0],
                            String.Empty));
                        continue;
                        }
                    if (j is Asn1ObjectIdentifier) {
                        r.Add(new KeyValuePair<Asn1ObjectIdentifier, String>((Asn1ObjectIdentifier)
                            j,
                            i[1].ToString()));
                        }
                    }
                else
                    {
                    break;
                    }
                }
            return new Asn1RelativeDistinguishedNameSequence(r);
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
                    writer.WritePropertyName(nameof(Extensions));
                    using (writer.ObjectScope(serializer)) {
                        writer.WriteValue(serializer, "Count", Extensions.Count);
                        writer.WritePropertyName("(Self)");
                        using (writer.ArrayScope(serializer)) {
                            foreach (var i in Extensions.OfType<Asn1CertificateExtension>()) {
                                i.WriteJson(writer, serializer);
                                }
                            }
                        }
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
            var o = source.TryGetValue("2.5.4.6", out var r)
                ? r.ToString().ToLower()
                : null;
            if (o != null) {
                if (o.Length == 3) {
                    o = IcaoCountry.ThreeLetterCountries[o];
                    }
                }
            return o;
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

        String IFileService.FileName { get { return $"{FriendlyName}.cer"; }}
        String IFileService.FullName { get { return ((IFileService)this).FileName; }}

        Byte[] IFileService.ReadAllBytes() {
            using (var r = new MemoryStream()) {
                WriteTo(r);
                return r.ToArray();
                }
            }

        Stream IFileService.OpenRead()
            {
            return new MemoryStream(((IFileService)this).ReadAllBytes());
            }

        void IFileService.MoveTo(String target)
            {
            ((IFileService)this).MoveTo(target, false);
            }

        /// <summary>Move an existing file to a new file. Overwriting a file of the same name is allowed.</summary>
        /// <param name="target">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite"><see langword="true"/> if the destination file can be overwritten; otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. -or-  <paramref name="target"/> is read-only.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="target"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars"/>.  -or-  <paramref name="target"/> specifies a directory.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path specified in <paramref name="target"/> is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.IO.IOException"><paramref name="target"/> exists and <paramref name="overwrite"/> is <see langword="false"/>. -or- An I/O error has occurred.</exception>
        /// <exception cref="T:System.NotSupportedException"><paramref name="target"/> is in an invalid format.</exception>
        void IFileService.MoveTo(String target, Boolean overwrite)
            {
            ((IFileService)this).CopyTo(target, overwrite);
            }

        /// <summary>Copies an existing file to a new file. Overwriting a file of the same name is allowed.</summary>
        /// <param name="target">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite"><see langword="true"/> if the destination file can be overwritten; otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. -or-  <paramref name="target"/> is read-only.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="target"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars"/>.  -or-  <paramref name="target"/> specifies a directory.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path specified in <paramref name="target"/> is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.IO.IOException"><paramref name="target"/> exists and <paramref name="overwrite"/> is <see langword="false"/>. -or- An I/O error has occurred.</exception>
        /// <exception cref="T:System.NotSupportedException"><paramref name="target"/> is in an invalid format.</exception>
        void IFileService.CopyTo(String target, Boolean overwrite)
            {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            using (var sourcestream = ((IFileService)this).OpenRead()) {
                if (File.Exists(target)) {
                    if (!overwrite) { throw new IOException(); }
                    File.Delete(target);
                    }
                var folder = Path.GetDirectoryName(target);
                if (!String.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                using (var targetstream = File.OpenWrite(target)) {
                    sourcestream.CopyTo(targetstream);
                    }
                }
            }

        /// <summary>Converts an object into its XML representation.</summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer) {
            writer.WriteStartElement("Certificate");
            writer.WriteAttributeString(nameof(NotBefore), NotBefore.ToString("o"));
            writer.WriteAttributeString(nameof(NotAfter),  NotAfter.ToString("o"));
            writer.WriteAttributeString(nameof(Thumbprint), Thumbprint);
            if (!String.IsNullOrWhiteSpace(Country)) { writer.WriteAttributeString(nameof(Country), Country); }
            if (SerialNumber != null) { writer.WriteAttributeString(nameof(SerialNumber), SerialNumber.Value.ToString("x")); }
            if ((Issuer != null) && (Issuer.Count > 0)) {
                writer.WriteStartElement("Certificate.Issuer");
                Issuer.WriteXml(writer);
                writer.WriteEndElement();
                }
            if ((Subject != null) && (Subject.Count > 0)) {
                writer.WriteStartElement("Certificate.Subject");
                Subject.WriteXml(writer);
                writer.WriteEndElement();
                }
            if (!IsNullOrEmpty(Extensions)) {
                writer.WriteStartElement(nameof(Extensions));
                foreach (var extension in Extensions.OfType<Asn1CertificateExtension>()){
                    extension.WriteXml(writer);
                    }
                writer.WriteEndElement();
                }
            if (SignatureAlgorithm != null) {
                writer.WriteStartElement("Certificate.SignatureAlgorithm");
                SignatureAlgorithm.WriteXml(writer);
                writer.WriteEndElement();
                }
            writer.WriteEndElement();
            }
        }
    }
