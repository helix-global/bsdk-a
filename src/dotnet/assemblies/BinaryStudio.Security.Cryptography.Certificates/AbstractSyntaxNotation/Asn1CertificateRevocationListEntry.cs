using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BinaryStudio.PlatformComponents;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    public class Asn1CertificateRevocationListEntry : Asn1LinkObject
        {
        private Asn1CertificateExtension[] extensions = EmptyArray<Asn1CertificateExtension>.Value;
        private String serialnumber = "{building}";

        public String SerialNumber { get { return serialnumber; }}
        public DateTime RevocationDate { get; }
        public IList<Asn1CertificateExtension> Extensions { get { return extensions; }}
        internal Asn1CertificateRevocationListEntry(Asn1Object source)
            : base(source)
            {
            serialnumber = String.Join(String.Empty, ((Asn1Integer)source[0]).Value.ToByteArray().Select(i => i.ToString("X2")));
            RevocationDate = (Asn1Time)source[1];
            if (source.Count > 2) {
                extensions = source[2].Select(
                    i => Asn1CertificateExtension.From(
                        new Asn1CertificateExtension(i))).ToArray();
                }
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return SerialNumber;
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            using (writer.ObjectScope(serializer)) {
                WriteValue(writer, serializer, nameof(SerialNumber), SerialNumber);
                WriteValue(writer, serializer, nameof(RevocationDate), RevocationDate.ToString("O"));
                var extensions = Extensions;
                if (!IsNullOrEmpty(extensions))
                    {
                    WriteValue(writer, serializer, nameof(Extensions), extensions);
                    }
                }
            }

        /// <summary>
        /// Releases the unmanaged resources used by the instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(Boolean disposing) {
            if (!State.HasFlag(ObjectState.Disposed)) {
                lock(this)
                    {
                    serialnumber = "{destroyed}";
                    Dispose(ref extensions);
                    }
                base.Dispose(disposing);
                State |= ObjectState.Disposed;
                }
            }
        }
    }