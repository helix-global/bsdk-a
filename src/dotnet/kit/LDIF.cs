using System;
using System.IO;
using System.Linq;
using BinaryStudio.IO;
using BinaryStudio.Security.Cryptography.DataInterchangeFormat;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographicMessageSyntax;
using Operations;

namespace kit
    {
    public static class LDIF
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
        #region M:ProcessCertificateRevocationList(Object,String,Asn1Certificate)
        private static void ProcessCertificateRevocationList(Object so, String folder, Asn1CertificateRevocationList o, BatchOperationFlags flags, X509CertificateStorage store)
            {
            var subject = o.Issuer;
            if (flags.HasFlag(BatchOperationFlags.Extract)) {
                if (flags.HasFlag(BatchOperationFlags.Group)&&
                    subject.TryGetValue("2.5.4.6", out var contry)) {
                    lock(so)
                        {
                        var targetfolder = Path.Combine(folder, contry.ToString().ToLower());
                        if (!Directory.Exists(targetfolder))
                            {
                            Directory.CreateDirectory(targetfolder);
                            }
                        var filename = Path.Combine(targetfolder, $"{o.FriendlyName}.crl");
                        WriteObject(filename, o);
                        }
                    }
                else
                    {
                    var filename = Path.Combine(folder, $"{o.FriendlyName}.crl");
                    WriteObject(filename, o);
                    }
                }
                 if (flags.HasFlag(BatchOperationFlags.Install))   { store.Add(new X509CertificateRevocationList(o));    }
            else if (flags.HasFlag(BatchOperationFlags.Uninstall)) { store.Remove(new X509CertificateRevocationList(o)); }
            Console.WriteLine(String.IsNullOrWhiteSpace(o.Country)
                ? $"{{{o.FriendlyName}}}"
                : $"{{{o.Country}}}:{{{o.FriendlyName}}}");
            }
        #endregion

        public static void ExtractFromLDIF(Object so, CSCAMasterList masterList, String folder, BatchOperationFlags flags, X509CertificateStorage store, String filter) {
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

        private static Int32 g = 0;
        public static void ExtractFromLDIF(String inputfilename, String folder, BatchOperationFlags flags, X509CertificateStorage store, String filter)
            {
            var file = new LDIFFile(inputfilename);
            var filesystem = new Object();
            file.Entries.AsParallel().ForAll(entry => 
                {
                Object value;
                #region userCertificate
                if (entry.TryGetValue("userCertificate;binary", 0, out value) &&
                    (String.IsNullOrWhiteSpace(filter) || String.Equals(filter, "*.cer", StringComparison.OrdinalIgnoreCase)))
                    {
                    var r = (Byte[])value;
                    try
                        {
                        var certificate = new Asn1Certificate(Asn1Object.Load(new ReadOnlyMemoryMappingStream(r)).FirstOrDefault());
                        if (certificate.IsFailed)
                            {
                            Console.WriteLine("Error:");
                            }
                        else
                            {
                            ProcessCertificate(filesystem, folder,
                                certificate, certificate.FriendlyName,
                                flags, store);
                            }
                        }
                    catch (Exception e)
                        {
                        Console.Error.WriteLine(e.ToString());
                        File.WriteAllBytes(Path.Combine(@"C:\Failed", Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + ".cer"), r);
                        }
                    }
                #endregion
                #region certificateRevocationList
                else if (entry.TryGetValue("certificateRevocationList;binary", 0, out value) &&
                    (String.IsNullOrWhiteSpace(filter) || String.Equals(filter, "*.crl", StringComparison.OrdinalIgnoreCase)))
                    {
                    var r = (Byte[])value;
                    var certificate = new Asn1CertificateRevocationList(Asn1Object.Load(new ReadOnlyMemoryMappingStream(r)).FirstOrDefault());
                    if (certificate.IsFailed)
                        {
                        Console.WriteLine("Error:");
                        }
                    else
                        {
                        ProcessCertificateRevocationList(filesystem, folder,
                            certificate, flags, store);
                        }
                    }
                #endregion
                #region CscaMasterListData
                else if (entry.TryGetValue("CscaMasterListData", 0, out value))
                    {
                    var r = new CmsMessage((Byte[])value);
                    var masterList = (CSCAMasterList)r.ContentInfo.GetService(typeof(CSCAMasterList));
                    ExtractFromLDIF(filesystem, masterList, folder, flags, store, filter);
                    }
                #endregion
                });
            }
        }
    }