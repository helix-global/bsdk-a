using System;
using System.Collections.Generic;
using System.Linq;
using BinaryStudio.DirectoryServices;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.CryptographicMessageSyntax
    {
    [CmsContentSpecificObject("2.23.136.1.1.2")]
    public class CSCAMasterList : CmsContentSpecificObject, IDirectoryService
        {
        public Int32 Version { get; }
        public IList<X509Certificate> Certificates { get { return certificates; }}

        public CSCAMasterList(Asn1Object source)
            : base(source)
            {
            var content = source[1][0][0];
            Version = (Int32)(Asn1Integer)content[0];
            certificates = content[1].Select(i => new X509Certificate(new Asn1Certificate(i))).ToArray();
            }

        IEnumerable<IFileService> IDirectoryService.GetFiles(String searchpattern, DirectoryServiceSearchOptions searchoption) {
            foreach (var i in Certificates.OfType<IFileService>()) {
                if (PathUtils.IsMatch(searchpattern, i.FileName)) {
                    yield return i;
                    }
                }
            }

        /// <summary>
        /// Releases the unmanaged resources used by the instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        protected override void Dispose(Boolean disposing) {
            if (disposing) {
                if (certificates != null) {
                    foreach (var certificate in certificates) { certificate?.Dispose(); }
                    certificates = null;
                    }
                }
            base.Dispose(disposing);
            }

        private IList<X509Certificate> certificates;
        }
    }