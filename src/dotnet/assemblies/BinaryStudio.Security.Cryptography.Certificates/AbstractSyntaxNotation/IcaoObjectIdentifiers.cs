using System;
using System.Linq;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Serialization;

namespace BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation
    {
    public class IcaoObjectIdentifiers
        {
        public const String Icao = "2.23.136";
        public const String IcaoMrtd = Icao + ".1";
        public const String IcaoMrtdSecurity = IcaoMrtd + ".1";
        public const String IcaoMrtdSecurityLdsSecurityObject = IcaoMrtdSecurity + ".1";
        public const String IcaoMrtdSecurityExtensions = IcaoMrtdSecurity + ".6";
        public const String IcaoMrtdSecurityExtensionsDocumentTypeList = IcaoMrtdSecurityExtensions + ".2";
        public const String IcaoMrtdSecurityExtensionsNameChange = IcaoMrtdSecurityExtensions + ".1";

        static IcaoObjectIdentifiers() {
            SerializationManager.CanSerialize += OnCanSerialize;
            SerializationManager.Serialize += OnSerialize;
            }

        private static void OnSerialize(Object sender, SerializeEventArgs e) {
            if (sender is Asn1Certificate certificate) {
                var conformance = false;
                var keyusage = certificate.Extensions.OfType<CertificateKeyUsage>().FirstOrDefault();
                if (keyusage != null) {
                    
                    }
                }
            }

        private static void OnCanSerialize(Object sender, CanSerializeEventArgs e)
            {
            e.CanSerialize = sender is Asn1Certificate;
            }
        }
    }