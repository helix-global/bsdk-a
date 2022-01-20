using System;
using System.IO;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographicMessageSyntax;
using Operations;

namespace kit
    {
    internal static class LDIF
        {
        private static void WriteObject(String filename, Asn1Object o) {
            using (var output = File.Create(filename)) {
                o.Write(output);
                }
            }

        #region M:ProcessCertificate(Object,String,Asn1Certificate,String)
        private static void ProcessCertificate(Object so, String folder, Asn1Certificate o, String filename, BatchOperationFlags flags, X509CertificateStorage store)
            {
            var subject = o.Subject;
            if (flags.HasFlag(BatchOperationFlags.Extract)) {
                if (flags.HasFlag(BatchOperationFlags.Group) &&
                    subject.TryGetValue("2.5.4.6", out var contry)) {
                    var targetfolder = Path.Combine(folder, contry.ToString().ToLower());
                    lock(so) {
                        if (!Directory.Exists(targetfolder)) {
                            Directory.CreateDirectory(targetfolder);
                            }
                        filename = Path.Combine(targetfolder, $"{filename}.cer");
                        WriteObject(filename, o);
                        }
                    }
                else
                    {
                    filename = Path.Combine(folder, $"{filename}.cer");
                    WriteObject(filename, o);
                    }
                }
                 if (flags.HasFlag(BatchOperationFlags.Install))   { store.Add(new X509Certificate(o));    }
            else if (flags.HasFlag(BatchOperationFlags.Uninstall)) { store.Remove(new X509Certificate(o)); }
            Console.WriteLine(String.IsNullOrWhiteSpace(o.Country)
                ? $"{{{filename}}}"
                : $"{{{o.Country}}}:{{{filename}}}");
            }
        #endregion

        internal static void ExtractFromLDIF(Object so, CSCAMasterList masterList, String folder, BatchOperationFlags flags, X509CertificateStorage store, String filter) {
            if (String.IsNullOrWhiteSpace(filter) || String.Equals(filter, "*.cer", StringComparison.OrdinalIgnoreCase)) {
                foreach (var certificate in masterList.Certificates) {
                    try
                        {
                        ProcessCertificate(so, folder,
                            certificate.Source, certificate.Source.FriendlyName,
                            flags, store);
                        }
                    catch (PathTooLongException)
                        {
                        try
                            {
                            ProcessCertificate(so, folder,
                                certificate.Source, certificate.Source.AlternativeFriendlyName,
                                flags, store);
                            }
                        catch
                            {
                            Console.WriteLine($"ERROR:{certificate.Source.AlternativeFriendlyName}");
                            throw;
                            }
                        }
                    }
                }
            }
        }
    }