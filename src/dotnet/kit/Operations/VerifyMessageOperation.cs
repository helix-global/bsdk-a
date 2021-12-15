using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using BinaryStudio.Security.Cryptography.Xml;
using Kit;
using Options;

namespace Operations
    {
    internal class VerifyMessageOperation : MessageOperation
        {
        public VerifyMessageOperation(TextWriter output, TextWriter error,IList<OperationOption> args)
            : base(output, error, args)
            {
            }

        public override void Execute(TextWriter output) {
            using (var context = new CryptographicContext(
                Logger, ProviderType,
                CryptographicContextFlags.CRYPT_SILENT | CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                X509Certificate[] certificates;
                if (InputFileName.Count > 1)
                    {
                    VerifyDetachedMessage(output, context, InputFileName[0], InputFileName[1], out certificates);
                    }
                else
                    {
                    if (Path.GetExtension(InputFileName[0]).Equals(".xml"))
                        {
                        var document = new XmlDocument();
                        document.Load(InputFileName[0]);
                        SignedXmlTools.VerifySignature(document, out var certificated, new CustomCertificateResolver(StoreName, StoreLocation, output));
                        }
                    else
                        {
                        VerifyAttachedMessage(output, context, InputFileName[0], OutputFileName?.FirstOrDefault(), out certificates);
                        }
                    }
                }
            }

        #region M:VerifyAttachedMessage(TextWriter,CryptographicContext,String,String,[Out]X509Certificate[])
        private void VerifyAttachedMessage(TextWriter output, CryptographicContext context, String inputfilename, String outputfilename, out X509Certificate[] certificates)
            {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (String.IsNullOrEmpty(inputfilename)) { throw new ArgumentOutOfRangeException(nameof(inputfilename)); }
            var console = String.IsNullOrEmpty(outputfilename);
            using (var inputfile = File.OpenRead(inputfilename)) {
                using (var outputfile = console
                    ? (Stream)new MemoryStream()
                    : File.OpenWrite(outputfilename))
                    {
                    VerifyAttachedMessage(output, context, inputfile, outputfile, out certificates);
                    if (console)
                        {
                        Utilities.Hex(((MemoryStream)outputfile).ToArray(), Console.Out);
                        }
                    }
                }
            }
        #endregion
        #region M:VerifyAttachedMessage(TextWriter,CryptographicContext,Stream,Stream,[Out]X509Certificate[])
        private void VerifyAttachedMessage(TextWriter output, CryptographicContext context,Stream inputfile, Stream outputfile, out X509Certificate[] certificates)
            {
            certificates = new X509Certificate[0];
            if (inputfile  == null) { throw new ArgumentNullException(nameof(inputfile));  }
            if (outputfile == null) { throw new ArgumentNullException(nameof(outputfile)); }
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            context.VerifyAttachedMessageSignature(inputfile, outputfile, out var r,
                new CustomCertificateResolver(StoreName, StoreLocation, output));
            foreach (var i in r)
                {
                output.WriteLine($"Certificate:{i.Thumbprint}");
                }
            }
        #endregion
        #region M:VerifyDetachedMessage(TextWriter,CryptographicContext,String,String,[Out]X509Certificate[])
        private void VerifyDetachedMessage(TextWriter output, CryptographicContext context, String inputfilename, String outputfilename, out X509Certificate[] certificates)
            {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (String.IsNullOrEmpty(inputfilename)) { throw new ArgumentOutOfRangeException(nameof(inputfilename)); }
            if (String.IsNullOrEmpty(outputfilename)) { throw new ArgumentOutOfRangeException(nameof(outputfilename)); }
            using (Stream
                inputfile = File.OpenRead(inputfilename),
                outputfile = File.OpenRead(outputfilename)) {
                VerifyDetachedMessage(output, context, inputfile, outputfile, out certificates);
                }
            }
        #endregion
        #region M:VerifyDetachedMessage(TextWriter,CryptographicContext,Stream,Stream,[Out]X509Certificate[])
        private void VerifyDetachedMessage(TextWriter output, CryptographicContext context,Stream inputfile, Stream outputfile, out X509Certificate[] certificates)
            {
            certificates = new X509Certificate[0];
            if (inputfile  == null) { throw new ArgumentNullException(nameof(inputfile));  }
            if (outputfile == null) { throw new ArgumentNullException(nameof(outputfile)); }
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            context.VerifyDetachedMessageSignature(inputfile, outputfile, out var r,
                new CustomCertificateResolver(StoreName, StoreLocation, output));
            foreach (var i in r)
                {
                output.WriteLine($"Certificate:{i.Thumbprint}");
                }
            }
        #endregion
        }
    }