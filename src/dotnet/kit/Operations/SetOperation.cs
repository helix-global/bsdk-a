using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using BinaryStudio.Security.Cryptography.Win32;
using Microsoft.Win32;
using Options;

namespace Operations
    {
    internal class SetOperation : CreateOrEncryptMessageOperation
        {
        public IDictionary<String,String> Options { get; }
        public SetOperation(TextWriter output, TextWriter error, IList<OperationOption> args)
            : base(output, error, args)
            {
            Options = args.OfType<SetOption>().FirstOrDefault()?.Properties;
            }

        protected override void Execute(TextWriter output, CryptographicContext context, IX509CertificateStorage store) {
            var i = 0;
            foreach (var certificate in store.Certificates) {
                Execute(context,certificate);
                i++;
                }
            Out.WriteLine($"Certificates:{i}");
            }

        private unsafe void Execute(CryptographicContext context, IX509Certificate certificate) {
            foreach (var option in Options) {
                switch (option.Key) {
                    case "save-pin":
                        {
                        var pinS = Encoding.ASCII.GetBytes(option.Value);
                        fixed (Byte* pinB = pinS)
                        using (var manager = new LocalMemoryManager()) {
                            var parameter = new CRYPT_KEY_PROV_PARAM
                                {
                                ParameterIdentifier = CRYPT_PARAM.PP_KEYEXCHANGE_PIN,
                                ParameterData = pinB,
                                ParameterDataSize = pinS.Length
                                };
                            var pi = new CRYPT_KEY_PROV_INFO{
                                ProviderName  = (IntPtr)manager.StringToMem(context.ProviderName, Encoding.ASCII),
                                ContainerName = (IntPtr)manager.StringToMem(certificate.Container, Encoding.ASCII),
                                ProviderType = context.ProviderType,
                                KeySpec = (CRYPT_KEY_SPEC)(Int32)certificate.KeySpec,
                                ProviderParameterCount = 1,
                                ProviderParameters = &parameter,
                                Flags = (Int32)(context.UseMachineKeySet
                                    ? CryptographicContextFlags.CRYPT_MACHINE_KEYSET
                                    : 0)
                                };
                            if (!CertSetCertificateContextProperty(certificate.Handle,CERT_PROP.CERT_KEY_PROV_INFO_PROP_ID,0,ref pi))
                                {
                                throw new HResultException(GetLastError());
                                }
                            }
                        }
                        break;
                    }
                }
            }

        [DllImport("crypt32.dll", BestFitMapping = false, CharSet = CharSet.None, SetLastError = true)] private static extern Boolean CertSetCertificateContextProperty([In] IntPtr context, [In] CERT_PROP property, Int32 flags, ref CRYPT_KEY_PROV_INFO data);
        [DllImport("kernel32.dll", BestFitMapping = false)] private static extern Int32 GetLastError();
        }
    }