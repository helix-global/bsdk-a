using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BinaryStudio.DirectoryServices;
using BinaryStudio.IO;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public class X509CertificateRevocationList : X509Object, IJsonSerializable, IX509CertificateRevocationList, IFileService
        {
        internal Asn1CertificateRevocationList UnderlyingObject;
        private IntPtr InternalHandle;
        public DateTime  EffectiveDate { get { return UnderlyingObject.EffectiveDate; }}
        public DateTime? NextUpdate    { get { return UnderlyingObject.NextUpdate;    }}
        public Int32 Version           { get { return UnderlyingObject.Version;       }}
        public String FriendlyName     { get { return UnderlyingObject.FriendlyName;  }}
        public String Thumbprint       { get { return UnderlyingObject.Thumbprint;    }}
        public String Country          { get { return UnderlyingObject.Country;       }}
        public X509RelativeDistinguishedNameSequence Issuer { get { return new X509RelativeDistinguishedNameSequence(UnderlyingObject.Issuer); }}
        public IList<IX509CertificateExtension> Extensions { get { return UnderlyingObject.Extensions; }}

        public Byte[] SignatureValue { get {
            return UnderlyingObject[2].Content.ToArray();
            }}

        IX509RelativeDistinguishedNameSequence IX509CertificateRevocationList.Issuer { get { return Issuer; }}

        public X509CertificateRevocationList(Byte[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            InternalHandle = CertCreateCRLContext(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, source, source.Length);
            UnderlyingObject = ConstructFromBinary(source);
            }

        public X509CertificateRevocationList(Asn1CertificateRevocationList source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            UnderlyingObject = source;
            InternalHandle = CertCreateCRLContext(X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, source.Body, source.Body.Length);
            }

        public unsafe X509CertificateRevocationList(IntPtr handle)
            {
            if (handle == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(handle)); }
            InternalHandle = CertDuplicateCRLContext(handle);
            UnderlyingObject = ConstructFromBinary((CRL_CONTEXT*)handle);
            }

        public unsafe X509CertificateRevocationList(CRL_CONTEXT* context)
            {
            if (context == null) { throw new ArgumentOutOfRangeException(nameof(context)); }
            InternalHandle = CertDuplicateCRLContext((IntPtr)context);
            UnderlyingObject = ConstructFromBinary(context);
            }

        #region M:ConstructFromBinary(ReadOnlyMemoryMappingStream):Asn1CertificateRevocationList
        private static Asn1CertificateRevocationList ConstructFromBinary(ReadOnlyMemoryMappingStream source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var o = Asn1Object.Load(source).FirstOrDefault();
            if (o == null) { throw new ArgumentOutOfRangeException(nameof(source)); }
            return new Asn1CertificateRevocationList(o);
            }
        #endregion
        #region M:ConstructFromBinary(Byte[]):Asn1CertificateRevocationList
        private static Asn1CertificateRevocationList ConstructFromBinary(Byte[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return ConstructFromBinary(new ReadOnlyMemoryMappingStream(source));
            }
        #endregion
        #region M:ConstructFromBinary(CRL_CONTEXT*):Asn1CertificateRevocationList
        private static unsafe Asn1CertificateRevocationList ConstructFromBinary(CRL_CONTEXT* source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var size  = source->CrlEncodedSize;
            var bytes = source->CrlEncodedData;
            var buffer = new Byte[size];
            for (var i = 0U; i < size; ++i) {
                buffer[i] = bytes[i];
                }
            return ConstructFromBinary(buffer);
            }
        #endregion

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
            {
            UnderlyingObject.WriteJson(writer, serializer);
            }

        /**
         * <summary>Returns a string that represents the current object.</summary>
         * <returns>A string that represents the current object.</returns>
         */
        public override String ToString()
            {
            return FriendlyName;
            }

        public override X509ObjectType ObjectType { get { return X509ObjectType.Crl; }}
        public override IntPtr Handle { get { return InternalHandle; }}

        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr CertCreateCRLContext(UInt32 dwCertEncodingType, [MarshalAs(UnmanagedType.LPArray)] Byte[] blob, Int32 size);
        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern IntPtr CertDuplicateCRLContext(IntPtr context);
        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)] private static extern Boolean CertFreeCRLContext(IntPtr context);

        #region M:GetSigningStreamInternal:IEnumerable<Byte>
        private IEnumerable<Byte> GetSigningStreamInternal() {
            return UnderlyingObject[0].Body;
            }
        #endregion
        #region M:GetSigningStream:Stream
        public Stream GetSigningStream()
            {
            return new ForwardOnlyByteStream(GetSigningStreamInternal);
            }
        #endregion

        String IFileService.FileName { get { return $"{FriendlyName}.crl"; }}

        unsafe Byte[] IFileService.ReadAllBytes()
            {
            var src   = (CRL_CONTEXT*)Handle;
            var size  = src->CrlEncodedSize;
            var bytes = src->CrlEncodedData;
            var r = new Byte[size];
            for (var i = 0U; i < size; ++i) {
                r[i] = bytes[i];
                }
            return r;
            }

        Stream IFileService.OpenRead()
            {
            return new MemoryStream(((IFileService)this).ReadAllBytes());
            }

        void IFileService.MoveTo(String target)
            {
            MoveTo(target, false);
            }

        public void MoveTo(String target, Boolean overwrite)
            {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            using (var sourcestream = ((IFileService)this).OpenRead()) {
                if (File.Exists(target)) {
                    if (!overwrite) { throw new IOException(); }
                    File.Delete(target);
                    }
                var folder = Path.GetDirectoryName(target);
                if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                using (var targetstream = File.OpenWrite(target)) {
                    sourcestream.CopyTo(targetstream);
                    }
                }
            }

        /// <summary>
        /// Releases the unmanaged resources used by the instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(Boolean disposing) {
            lock (this) {
                Dispose(ref UnderlyingObject);
                if (InternalHandle != IntPtr.Zero)
                    {
                    CertFreeCRLContext(InternalHandle);
                    InternalHandle = IntPtr.Zero;
                    }
                }
            base.Dispose(disposing);
            }
        }
    }