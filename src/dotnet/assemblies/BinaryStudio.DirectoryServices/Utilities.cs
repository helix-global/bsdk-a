using System;
using System.IO;

#if NET5_0
using System.Net;
using System.Net.Http;
#endif
#if NET45
using System.Net;
using System.Security.Cryptography.X509Certificates;
#endif

namespace BinaryStudio.DirectoryServices
    {
    public class Utilities
        {
        #if NET5_0
        public static void DownloadTo(String uri, Stream target)
            {
            if (uri == null) { throw new ArgumentNullException(nameof(uri)); }
            if (String.IsNullOrWhiteSpace(uri)) { throw new ArgumentOutOfRangeException(nameof(uri)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            using (var client = new HttpClient()) {
                var response = client.GetAsync(uri).GetAwaiter().GetResult();
                var source = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
                source.CopyTo(target);
                }
            }
        #endif
        #if NET45
        internal class EIgnoreCertificatePolicy : ICertificatePolicy
            {
            public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
                {
                return true;
                }
            }

        public static ICertificatePolicy IgnoreCertificatePolicy { get; }
        static Utilities()
            {
            IgnoreCertificatePolicy = new EIgnoreCertificatePolicy();
            }
        #endif
        }
    }
