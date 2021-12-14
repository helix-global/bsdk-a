using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using BinaryStudio.Security.Cryptography;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using BinaryStudio.Security.Cryptography.Xml;
using Kit;
using Options;

namespace Operations
    {
    internal class CreateMessageOperation : CreateOrEncryptMessageOperation
        {
        public CryptographicMessageFlags MessageFlags    { get; }
        public PinCodeRequestTypeKind PinCodeRequestType { get; }
        public Boolean IsXml {get; }

        public CreateMessageOperation(TextWriter output, TextWriter error, IList<OperationOption> args)
            : base(output, error, args)
            {
            PinCodeRequestType = args.OfType<PinCodeRequestType>().First().Value;
            var flags = args.OfType<CreateOption>().FirstOrDefault();
            if (flags != null) {
                if (flags.HasValue("detached"))   { MessageFlags |= CryptographicMessageFlags.Detached;         }
                if (flags.HasValue("indefinite")) { MessageFlags |= CryptographicMessageFlags.IndefiniteLength; }
                if (flags.HasValue("xml"))        { IsXml = true;                                               }
                if (!flags.HasValue("excludecertificate")) { MessageFlags |= CryptographicMessageFlags.IncludeSigningCertificate; }
                }
            else
                {
                MessageFlags = CryptographicMessageFlags.Attached|CryptographicMessageFlags.IncludeSigningCertificate;
                }
            }

        protected override void Execute(TextWriter output, CryptographicContext context, IX509CertificateStorage store) {
            CreateMessage(
                context, store,
                InputFileName[0], OutputFileName?.FirstOrDefault(),
                MessageFlags);
            }

        #region M:CreateMessage(CryptographicContext,IX509CertificateStorage,String,String,CryptographicMessageFlags)
        public void CreateMessage(CryptographicContext context, IX509CertificateStorage store, String inputfilename, String outputfilename, CryptographicMessageFlags flags) {
            if (store == null) { throw new ArgumentNullException(nameof(store)); }
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (String.IsNullOrEmpty(inputfilename)) { throw new ArgumentOutOfRangeException(nameof(inputfilename)); }
            var console = String.IsNullOrEmpty(outputfilename);
            using (var inputfile = File.OpenRead(inputfilename)) {
                using (var outputfile = console
                    ? (Stream)new MemoryStream()
                    : File.OpenWrite(outputfilename))
                    {
                    CreateMessage(context, store.Certificates, inputfile, outputfile, flags);
                    if (console)
                        {
                        Utilities.Hex(((MemoryStream)outputfile).ToArray(), Console.Out);
                        }
                    }
                }
            }
        #endregion
        #region M:CreateMessage(CryptographicContext,IEnumerable<IX509Certificate>,Stream,Stream,CryptographicMessageFlags)
        private void CreateMessage(CryptographicContext context, IEnumerable<IX509Certificate> certificates, Stream inputfile, Stream outputfile, CryptographicMessageFlags flags) {
            if (certificates == null) { throw new ArgumentNullException(nameof(certificates)); }
            if (inputfile  == null) { throw new ArgumentNullException(nameof(inputfile));  }
            if (outputfile == null) { throw new ArgumentNullException(nameof(outputfile)); }
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            RequestSecureStringEventHandler handler = null;
            switch (PinCodeRequestType) {
                case PinCodeRequestTypeKind.Console:
                    {
                    handler = RequestConsoleSecureStringEventHandler;
                    }
                    break;
                case PinCodeRequestTypeKind.Window:
                    {
                    handler = RequestWindowSecureStringEventHandler;
                    }
                    break;
                }
            if (IsXml) {
                var document = new XmlDocument();
                document.Load(InputFileName[0]);
                SignedXmlTools.CreateAttachedSignature(certificates.ToArray(), document);
                document.Save(new StreamWriter(outputfile, Encoding.UTF8));
                return;
                }
            context.CreateMessageSignature(inputfile, outputfile,
                certificates.ToArray(), flags,
                handler);
            }
        #endregion
        }
    }