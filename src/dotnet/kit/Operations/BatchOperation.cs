using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using BinaryStudio.DirectoryServices;
using BinaryStudio.IO;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PortableExecutable;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographicMessageSyntax;
using BinaryStudio.Security.Cryptography.Interchange;
using BinaryStudio.Serialization;
using Newtonsoft.Json;
using Options;
using DRelativeDistinguishedName = BinaryStudio.Security.Cryptography.Interchange.DRelativeDistinguishedName;
using Formatting = Newtonsoft.Json.Formatting;

namespace Operations
    {
    internal class BatchOperation : Operation
        {
        public IList<String> InputFileName { get; }
        public IList<String> Options { get; }
        public OutputTypeOption OutputType { get; }
        private BatchOperationFlags Flags { get; }
        private String TargetFolder { get; }
        private String TargetConnectionString { get; }
        private String QuarantineFolder { get; }
        private X509StoreLocation? StoreLocation { get; }
        private String StoreName { get; }
        private String Filter { get; }
        private readonly Object FolderLock = new Object();
        private Entities connection;
        private SqlConnection NativeConnection;
        private MetadataScope scope;
        private X509CertificateStorage store;
        //private HashSet<CertificateReportItem> CertificateReport = new HashSet<CertificateReportItem>();

        //private class CertificateReportItem
        //    {
        //    public String Country { get; }
        //    public String SerialNumber { get; }
        //    public String Issuer { get; }
        //    public String Subject { get; }
        //    public DateTime NotBefore { get; }
        //    public DateTime NotAfter { get; }
        //    public String AuthorityKeyIdentifier { get;set; }

        //    public CertificateReportItem(Asn1Certificate source)
        //        {
        //        Country = source.Country;
        //        SerialNumber = source.SerialNumber.Value.ToString("x");
        //        Issuer = source.Issuer.ToString();
        //        Subject = source.Subject.ToString();
        //        NotAfter = source.NotAfter;
        //        NotBefore = source.NotBefore;
        //        }
        //    }

        public BatchOperation(TextWriter output, TextWriter error, IList<OperationOption> args) 
            : base(output, error, args)
            {
            InputFileName    = args.OfType<InputFileOrFolderOption>().FirstOrDefault()?.Values;
            TargetFolder     = args.OfType<OutputFileOrFolderOption>().FirstOrDefault()?.Values?.FirstOrDefault();
            QuarantineFolder = args.OfType<QuarantineFolderOption>().FirstOrDefault()?.Value;
            OutputType       = args.OfType<OutputTypeOption>().First();
            StoreLocation    = args.OfType<StoreLocationOption>().FirstOrDefault()?.Value;
            StoreName        = args.OfType<StoreNameOption>().FirstOrDefault()?.Value ?? nameof(X509StoreName.My);
            Filter           = args.OfType<FilterOption>().FirstOrDefault()?.Value ?? "*.*";
            Options = (new HashSet<String>(args.OfType<BatchOption>().SelectMany(i => i.Values))).ToArray();
            if (Options.Any(i => String.Equals(i, "rename",    StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Rename;    }
            if (Options.Any(i => String.Equals(i, "serialize", StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Serialize; }
            if (Options.Any(i => String.Equals(i, "extract",   StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Extract;   }
            if (Options.Any(i => String.Equals(i, "group",     StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Group;     }
            if (Options.Any(i => String.Equals(i, "install",   StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Install;   }
            if (Options.Any(i => String.Equals(i, "uninstall", StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Uninstall; }
            if (Options.Any(i => String.Equals(i, "report",    StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Report;    }
            if (Options.Any(i => String.Equals(i, "asn1",      StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.AbstractSyntaxNotation; }
            if (!String.IsNullOrWhiteSpace(TargetFolder)) {
                if (TargetFolder.StartsWith("oledb://")) {
                    TargetConnectionString = TargetFolder.Substring(8).Trim();
                    }
                else if (!TargetFolder.StartsWith(@"\\?\")) {
                    TargetFolder = $@"\\?\{TargetFolder}";
                    }
                }
            }

        #region M:Execute(String,IFileService,Byte[],Boolean):FileOperationStatus
        private FileOperationStatus Execute(String targetfolder, IFileService fileservice, Byte[] file, Boolean checkbase64) {
            if (checkbase64) {
                if (file.Length > 0) {
                    if (file[0] == '-') {
                        var builder = new StringBuilder();
                        using (var reader = new StreamReader(new MemoryStream(file), Encoding.UTF8)) {
                            while (!reader.EndOfStream) {
                                var L = reader.ReadLine();
                                if (!String.IsNullOrWhiteSpace(L)) {
                                    if (!L.StartsWith("-----BEGIN CERTIFICATE-----") &&
                                        !L.StartsWith("-----END CERTIFICATE-----")) {
                                        builder.AppendLine(L);
                                        }
                                    }
                                else
                                    {
                                    builder.AppendLine(L);
                                    }
                                }
                            }
                        return Execute(targetfolder, fileservice, Convert.FromBase64String(builder.ToString()), false);
                        }
                    try
                        {
                        return Execute(targetfolder, fileservice, Convert.FromBase64String(Encoding.ASCII.GetString(file)), false);
                        }
                    catch
                        {
                        return Execute(targetfolder, fileservice, file, false);
                        }
                    }
                }
            using (var sourcestream = new MemoryStream(file)) { return Execute(targetfolder,fileservice,sourcestream); }
            }
        #endregion
        #region M:Execute(String,IFileService,Stream):FileOperationStatus
        private FileOperationStatus Execute(String targetfolder, IFileService fileservice, Stream sourcestream) {
            using (var o = Asn1Object.Load(sourcestream).FirstOrDefault()) {
                using (var cer = new Asn1Certificate(o))               if (!cer.IsFailed) { return Execute(targetfolder, fileservice, cer); }
                using (var crl = new Asn1CertificateRevocationList(o)) if (!crl.IsFailed) { return Execute(targetfolder, fileservice, crl); }
                using (var cms = new CmsMessage(o))                    if (!cms.IsFailed) { return Execute(targetfolder, fileservice, cms); }
                return FileOperationStatus.Skip;
                }
            }
        #endregion
        #region M:Execute(String,IFileService,Asn1Certificate):FileOperationStatus
        private FileOperationStatus Execute(String targetfolder, IFileService fileservice, Asn1Certificate source) {
            var country = Flags.HasFlag(BatchOperationFlags.Group)
                ? GetCountry(source.Subject)??GetCountry(source.Issuer)
                : null;
            if (Flags.HasFlag(BatchOperationFlags.Extract)) {
                var targetfilename = Path.Combine(Combine(targetfolder,country), source.FriendlyName + ".cer");
                if (!String.Equals(targetfilename, fileservice.FileName, StringComparison.OrdinalIgnoreCase)) {
                    if (!String.IsNullOrEmpty(country)) {
                        lock (FolderLock) {
                            if (!Directory.Exists(Combine(targetfolder,country))) {
                                Directory.CreateDirectory(Combine(targetfolder,country));
                                }
                            }
                        }
                    fileservice.MoveTo(targetfilename, true);
                    File.SetCreationTime(targetfilename, source.NotBefore);
                    File.SetLastWriteTime(targetfilename, source.NotBefore);
                    File.SetLastAccessTime(targetfilename, source.NotBefore);
                    }
                }
            if (Flags.HasFlag(BatchOperationFlags.Serialize)) {
                if (NativeConnection != null) {
                    Update(NativeConnection, source);
                    }
                else
                    {
                    var certificate = new X509Certificate(source);
                    var targetfilename = Path.Combine(targetfolder, Path.GetFileName(fileservice.FileName + ".json"));
                    using (var writer = new StreamWriter(File.OpenWrite(targetfilename))) {
                        JsonSerialize(certificate, writer);
                        }
                    }
                }
                    if (Flags.HasFlag(BatchOperationFlags.Install))   { store.Add(new X509Certificate(source));    }
            else if (Flags.HasFlag(BatchOperationFlags.Uninstall)) { store.Remove(new X509Certificate(source)); }
            return FileOperationStatus.Success;
            }
        #endregion
        #region M:Execute(String,IFileService,Asn1CertificateRevocationList):FileOperationStatus
        private FileOperationStatus Execute(String targetfolder, IFileService fileservice, Asn1CertificateRevocationList source) {
            var country = Flags.HasFlag(BatchOperationFlags.Group)
                ? GetCountry(source.Issuer)
                : null;
            if (Flags.HasFlag(BatchOperationFlags.Extract)) {
                var targetfilename = Path.Combine(Combine(targetfolder,country), source.FriendlyName + ".crl");
                if (!String.Equals(targetfilename, fileservice.FileName, StringComparison.OrdinalIgnoreCase)) {
                    if (!String.IsNullOrEmpty(country)) {
                        lock (FolderLock) {
                            if (!Directory.Exists(Combine(targetfolder,country))) {
                                Directory.CreateDirectory(Combine(targetfolder,country));
                                }
                            }
                        }
                    fileservice.MoveTo(targetfilename, true);
                    File.SetCreationTime(targetfilename, source.EffectiveDate);
                    File.SetLastWriteTime(targetfilename, source.EffectiveDate);
                    File.SetLastAccessTime(targetfilename, source.EffectiveDate);
                    }
                }
            if (Flags.HasFlag(BatchOperationFlags.Serialize)) {
                if (NativeConnection != null) {
                    Update(NativeConnection, source);
                    }
                else
                    {
                    var targetfilename = Path.Combine(targetfolder, Path.GetFileName(fileservice.FileName + ".json"));
                    using (var writer = new StreamWriter(File.OpenWrite(targetfilename))) {
                        JsonSerialize(source, writer);
                        }
                    }
                }
                 if (Flags.HasFlag(BatchOperationFlags.Install))   { store.Add(new X509CertificateRevocationList(source));    }
            else if (Flags.HasFlag(BatchOperationFlags.Uninstall)) { store.Remove(new X509CertificateRevocationList(source)); }
            return FileOperationStatus.Success;
            }
        #endregion
        #region M:Execute(String,IFileService,CmsMessage):FileOperationStatus
        private FileOperationStatus Execute(String targetfolder, IFileService fileservice, CmsMessage source) {
            if (Flags.HasFlag(BatchOperationFlags.Serialize)) {
                if (NativeConnection != null) {
                    Update(NativeConnection, source, Path.GetFileNameWithoutExtension(fileservice.FileName));
                    }
                else
                    {
                    var targetfilename = Path.Combine(targetfolder, Path.GetFileName(fileservice.FileName + ".json"));
                    using (var writer = new StreamWriter(File.OpenWrite(targetfilename))) {
                        JsonSerialize(source, writer);
                        }
                    }
                }
            return FileOperationStatus.Success;
            }
        #endregion
        #region M:Execute(IFileService,DirectoryServiceSearchOptions,FileOperationArgs):FileOperationStatus
        private FileOperationStatus Execute(IFileService service,DirectoryServiceSearchOptions flags,FileOperationArgs e) {
            var status = FileOperationStatus.Skip;
            switch (Path.GetExtension(service.FileName).ToLower()) {
                case ".crl":
                case ".cer":
                case ".der":
                case ".ber":
                case ".p7b":
                    using (var sourcestream = service.OpenRead())
                        {
                        status = Execute(
                            e.TargetFolder,
                            service, sourcestream);
                        }
                    break;
                case ".hex":
                    using (var sourcestream = service.OpenRead())
                        {
                        status = ExecuteHex(
                            e.TargetFolder,
                            service, sourcestream);
                        }
                    break;
                }
            return status;
            }
        #endregion
        #region M:Execute(TextWriter)
        public override void Execute(TextWriter output) {
            var location = StoreLocation.GetValueOrDefault(X509StoreLocation.CurrentUser);
            if (Flags.HasFlag(BatchOperationFlags.Install) || Flags.HasFlag(BatchOperationFlags.Uninstall)) {
                if (location == X509StoreLocation.LocalMachine) {
                    PlatformContext.ValidatePermission(WindowsBuiltInRole.Administrator);
                    }
                }
            scope = new MetadataScope();
            connection = CreateConnection(TargetConnectionString);
            NativeConnection = CreateNativeConnection(TargetConnectionString);
            store = new X509CertificateStorage(ToStoreName(StoreName).GetValueOrDefault(X509StoreName.My), location);
            try
                {
                var core = new FileOperation(Out,Error) {
                    TargetFolder = TargetFolder,
                    Pattern = Filter,
                    Options = DirectoryServiceSearchOptions.Recursive|DirectoryServiceSearchOptions.Containers,
                    ExecuteAction = Execute,
                    };
                core.DirectoryServiceRequest += DirectoryServiceRequest;
                core.DirectoryCompleted += DirectoryCompleted;
                core.Execute(InputFileName);
                if (Flags.HasFlag(BatchOperationFlags.Install) || Flags.HasFlag(BatchOperationFlags.Uninstall)) {
                    store.Commit();
                    }
                }
            finally
                {
                Dispose(ref scope);
                Dispose(ref connection);
                Dispose(ref store);
                }
            GC.Collect();
            GC.Collect();
            }

        private void DirectoryCompleted(Object sender, EventArgs e) {
            if (connection != null) {
                connection.SaveChanges();
                }
            }

        private static void DirectoryServiceRequest(Object sender, DirectoryServiceRequestEventArgs e) {
            switch (Path.GetExtension(e.Source.FileName).ToLower()) {
                case ".hexgroup":
                    e.Service = new HexGroupService(e.Source);
                    e.Handled = true;
                    break;
                case ".hexcsv":
                    e.Service = new HexCSVGroupService(e.Source);
                    e.Handled = true;
                    break;
                }
            }
        #endregion
        private FileOperationStatus ExecuteHex(String targetfolder, IFileService service, Stream sourcestream) {
            var index = 0;
            var status = FileOperationStatus.Skip;
            foreach (var o in Asn1Object.Load(new ReadOnlyMemoryMappingStream(service.ReadAllBytes()))) {
                if (o.IsDecoded && !o.IsFailed) {
                    if (o.Count == 1) {
                        if ((o.Class == Asn1ObjectClass.Application) && (((IAsn1Object)o).Type == 23)) {
                            var cms = new CmsMessage(o[0]);
                            if (!cms.IsFailed && (cms.ContentInfo is CmsSignedDataContentInfo signedData)) {
                                var certificates = signedData.Certificates.ToArray();
                                if (certificates.Length > 0) {
                                    var country = certificates[0].Country ?? String.Empty;
                                    if (Flags.HasFlag(BatchOperationFlags.Group)) { targetfolder = Path.Combine(targetfolder, country); }
                                    MakeDir(targetfolder);
                                    service.CopyTo(Path.Combine(targetfolder, service.FileName), true);
                                    ((IFileService)certificates[0]).CopyTo(Path.Combine(targetfolder, Path.ChangeExtension(service.FileName, ".cer")), true);
                                    ((IFileService)cms).CopyTo(Path.Combine(targetfolder, Path.ChangeExtension(service.FileName, ".p7b")), true);
                                    status = FileOperation.Max(status, FileOperationStatus.Success);
                                    }
                                }
                            else
                                {
                                status = FileOperation.Max(status, FileOperationStatus.Warning);
                                }
                            }
                        }
                    }
                else
                    {
                    if (QuarantineFolder != null)
                        {
                        MakeDir(QuarantineFolder);
                        service.CopyTo(Path.Combine(QuarantineFolder, service.FileName), true);
                        }
                    }
                index++;
                }
            return status;
            }

        #region M:Get<E,T>(IDbSet<E>,[ref]Boolean,T):E
        private static E Get<E,T>(IDbSet<E> set, ref Boolean flag, T value)
            where E: class, IDInstance<T>, new()
            {
            var r = set.FirstOrDefault(i => Equals(i.Value, value));
            if (r == null) {
                r = set.Add(new E
                    {
                    Value = value
                    });
                flag = true;
                }
            return r;
            }
        #endregion
        #region M:Get<E>(ISet<E>,IDbSet<E>,String):E
        private static E Get<E>(ISet<E> cache, IDbSet<E> set, String value)
            where E: class, IDInstance<String>, new()
            {
            var r = cache.FirstOrDefault(i => i.Value == value)?? 
                      set.FirstOrDefault(i => i.Value == value);
            if (r == null) {
                r = set.Add(new E
                    {
                    Value = value
                    });
                cache.Add(r);
                }
            return r;
            }
        #endregion
        #region M:Get(ISet<DObjectIdentifier>,IDbSet<DObjectIdentifier>,String):DObjectIdentifier
        private static DObjectIdentifier Get(ISet<DObjectIdentifier> cache, IDbSet<DObjectIdentifier> set, String value)
            {
            var r = cache.FirstOrDefault(i => i.Value == value)?? 
                      set.FirstOrDefault(i => i.Value == value);
            if (r == null) {
                r = set.Add(new DObjectIdentifier
                    {
                    Value = value,
                    ShortName = (new Oid(value)).FriendlyName
                    });
                cache.Add(r);
                }
            return r;
            }
        #endregion
        #region M:Get<E,T>(IDbSet<E>,T):E
        private static E Get<E,T>(IDbSet<E> set, T value)
            where E: class, IDInstance<T>, new()
            where T: class
            {
            var r = set.FirstOrDefault(i => Equals(i.Value, value));
            if (r == null) {
                r = set.Add(new E
                    {
                    Value = value
                    });
                }
            return r;
            }
        #endregion
        #region M:Get:RelativeDistinguishedName
        private static DRelativeDistinguishedName Get(Entities context, LocalCache cache, IDbSet<DRelativeDistinguishedName> set, Oid oid, String value) {
            var type = oid.Value;
            var r = cache.RelativeDistinguishedNames.FirstOrDefault(i => (i.ObjectIdentifier != null) && (i.ObjectIdentifier.Value == type) && (i.String.Value == value));
            r = r??
                set.
                    Include(nameof(DRelativeDistinguishedName.String)).
                    Include(nameof(DRelativeDistinguishedName.ObjectIdentifier)).
                    FirstOrDefault(i => (i.ObjectIdentifier.Value == type) && (i.String.Value == value));
            if (r == null) {
                r = set.Add(new DRelativeDistinguishedName {
                    String = Get(cache.Strings, context.Strings, value),
                    ObjectIdentifier = Get(cache.ObjectIdentifiers, context.ObjectIdentifiers, type)
                    });
                r.ObjectIdentifier.ShortName = oid.FriendlyName;
                cache.RelativeDistinguishedNames.Add(r);
                }
            return r;
            }
        #endregion
        #region M:Get(Asn1IssuerAlternativeName):DExtension
        private static DExtension Get(Entities context, LocalCache cache, Asn1IssuerAlternativeName source) {
            var r = context.IssuerAlternativeNames.Add(new DIssuerAlternativeName {
                Extension = context.Extensions.Add(new DExtension {
                    IsCritical = source.IsCritical,
                    ObjectIdentifier = Get(cache.ObjectIdentifiers, context.ObjectIdentifiers, source.Identifier.ToString())
                    })
                });
            foreach (var name in source.AlternativeName) {
                context.AlternativeNames.Add(new DAlternativeName {
                    GeneralName = Get(context, cache, name),
                    Extension = r.Extension
                    });
                }
            return r.Extension;
            }
        #endregion
        #region M:Get(Asn1SubjectAlternativeName):DExtension
        private static DExtension Get(Entities context, LocalCache cache, Asn1SubjectAlternativeName source) {
            var r = context.SubjectAlternativeNames.Add(new DSubjectAlternativeName {
                Extension = context.Extensions.Add(new DExtension {
                    IsCritical = source.IsCritical,
                    ObjectIdentifier = Get(cache.ObjectIdentifiers, context.ObjectIdentifiers, source.Identifier.ToString())
                    })
                });
            foreach (var name in source.AlternativeName) {
                context.AlternativeNames.Add(new DAlternativeName {
                    GeneralName = Get(context, cache, name),
                    Extension = r.Extension
                    });
                }
            return r.Extension;
            }
        #endregion
        #region M:Get(Asn1CertificateAuthorityKeyIdentifierExtension):Extension
        private static DExtension Get(Entities context, LocalCache cache, CertificateAuthorityKeyIdentifier source) {
            var r = context.AuthorityKeyIdentifiers.Add(new DAuthorityKeyIdentifier {
                LongKey = source.KeyIdentifier.ToString("X"),
                ShortKey = GetShortKey(source.KeyIdentifier),
                SerialNumber = source.SerialNumber,
                GeneralName = Get(context, cache, source.CertificateIssuer),
                Extension = context.Extensions.Add(new DExtension {
                    IsCritical = source.IsCritical,
                    ObjectIdentifier = Get(cache.ObjectIdentifiers, context.ObjectIdentifiers, source.Identifier.ToString()),
                    })
                });
            return r.Extension;
            }
        #endregion
        #region M:Get(Asn1CertificateSubjectKeyIdentifierExtension):DExtension
        private static DExtension Get(Entities context, LocalCache cache, CertificateSubjectKeyIdentifier source) {
            var r = context.SubjectKeyIdentifiers.Add(new DSubjectKeyIdentifier {
                LongKey = source.KeyIdentifier.ToString("X"),
                ShortKey = GetShortKey(source.KeyIdentifier),
                Extension = context.Extensions.Add(new DExtension {
                    IsCritical = source.IsCritical,
                    ObjectIdentifier = Get(cache.ObjectIdentifiers, context.ObjectIdentifiers, source.Identifier.ToString()),
                    })
                });
            return r.Extension;
            }
        #endregion
        #region M:Get(IX509GeneralName):GeneralName
        private static DGeneralName Get(Entities context, LocalCache cache, IX509GeneralName source) {
            if (source == null) { return null; }
            var e = cache.GeneralNames.FirstOrDefault(i => i.Equals(source));
            if (e != null) { return e; }
            foreach (var sequence in context.GeneralNames.
                Include(nameof(DGeneralName.RelativeDistinguishedNameSequence))) {
                if (sequence.Equals(source)) {
                    return sequence;
                    }
                }
            if (source.Type == X509GeneralNameType.Directory) {
                var r = context.RelativeDistinguishedNameSequences.Add(new DRelativeDistinguishedNameSequence {
                    GeneralName = context.GeneralNames.Add(new DGeneralName {
                        Type = (Int32)X509GeneralNameType.Directory,
                        Value = source.ToString()
                        })
                    });
                r.GeneralName.RelativeDistinguishedNameSequence = r;
                foreach (var i in ((Asn1RelativeDistinguishedNameSequence)source).Select(i => Get(context,
                    cache,
                    context.RelativeDistinguishedNames,
                    new Oid(i.Key.ToString()),
                    i.Value.ToString())))
                    {
                    r.RelativeDistinguishedNameSequenceMapping.Add(
                        context.RelativeDistinguishedNameSequenceMappings.Add(new DRelativeDistinguishedNameSequenceMapping {
                            RelativeDistinguishedNameSequence = r,
                            RelativeDistinguishedName = i
                            })
                        );
                    }
                cache.RelativeDistinguishedNameSequences.Add(r);
                cache.GeneralNames.Add(r.GeneralName);
                return r.GeneralName;
                }
            else
                {
                var r = context.GeneralNames.Add(new DGeneralName {
                    Type = (Byte)(Int32)(source.Type),
                    Value = source.ToString()
                    });
                cache.GeneralNames.Add(r);
                return r;
                }
            }
        #endregion
        #region M:Get(Entities,LocalCache,Asn1RelativeDistinguishedNameSequence):DRelativeDistinguishedNameSequence
        private static DRelativeDistinguishedNameSequence Get(Entities context, LocalCache cache, Asn1RelativeDistinguishedNameSequence source) {
            var r = cache.RelativeDistinguishedNameSequences.FirstOrDefault(i => i.Equals(source));
            if (r == null) {
                foreach (var sequence in SQL(context.RelativeDistinguishedNameSequences.
                    Include(nameof(DRelativeDistinguishedNameSequence.RelativeDistinguishedNameSequenceMapping)).
                    Include("RelativeDistinguishedNameSequenceMapping.RelativeDistinguishedName").
                    Include("RelativeDistinguishedNameSequenceMapping.RelativeDistinguishedName.ObjectIdentifier").
                    Include("RelativeDistinguishedNameSequenceMapping.RelativeDistinguishedName.String"))) {
                    if (sequence.Equals(source)) {
                        return sequence;
                        }
                    }
                r = context.RelativeDistinguishedNameSequences.Add(new DRelativeDistinguishedNameSequence {
                    GeneralName = context.GeneralNames.Add(new DGeneralName {
                        Type = (Int32)X509GeneralNameType.Directory,
                        Value = source.ToString()
                        })
                    });
                foreach (var i in source.Select(i => Get(context,
                    cache,
                    context.RelativeDistinguishedNames,
                    new Oid(i.Key.ToString()),
                    i.Value.ToString())))
                    {
                    r.RelativeDistinguishedNameSequenceMapping.Add(
                        context.RelativeDistinguishedNameSequenceMappings.Add(new DRelativeDistinguishedNameSequenceMapping {
                            RelativeDistinguishedNameSequence = r,
                            RelativeDistinguishedName = i
                            })
                        );
                    }
                cache.RelativeDistinguishedNameSequences.Add(r);
                cache.GeneralNames.Add(r.GeneralName);
                }
            return r;
            }
        #endregion

        #region M:Update(Entities,Asn1Certificate)
        private static void Update(Entities context, Asn1Certificate source) {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var thumbprint = source.Thumbprint;
            var target = SQL(context.Certificates.
                Include(nameof(DCertificate.RelativeDistinguishedNameSequence)).
                Include(nameof(DCertificate.RelativeDistinguishedNameSequence1)).
                Include(nameof(DCertificate.Object))).
                FirstOrDefault(i => i.Thumbprint == thumbprint);
            if (target == null) {
                target = new DCertificate {
                    Country = source.Country,
                    Thumbprint = source.Thumbprint,
                    SerialNumber = source.SerialNumber.ToString(),
                    NotAfter = source.NotAfter,
                    NotBefore = source.NotBefore,
                    Object = context.Objects.Add(new DObject())
                    };
                ((IDObject)target).Type = DObjectType.Certificate;
                var cache = new LocalCache();
                target.RelativeDistinguishedNameSequence  = Get(context, cache, source.Issuer);
                target.RelativeDistinguishedNameSequence1 = Get(context, cache, source.Subject);
                foreach (var extension in source.Extensions) {
                    switch (extension.Identifier.ToString()) {
                        case "2.5.29.18":
                            {
                            target.Object.Extension.Add(Get(context, cache, (Asn1IssuerAlternativeName)extension));
                            }
                            break;
                        case "2.5.29.17":
                            {
                            target.Object.Extension.Add(Get(context, cache, (Asn1SubjectAlternativeName)extension));
                            }
                            break;
                        case "2.5.29.35":
                            {
                            target.Object.Extension.Add(Get(context, cache, (CertificateAuthorityKeyIdentifier)extension));
                            }
                            break;
                        case "2.5.29.14":
                            {
                            target.Object.Extension.Add(Get(context, cache, (CertificateSubjectKeyIdentifier)extension));
                            }
                            break;
                        }
                    }
                context.Certificates.Add(target);
                //context.SaveChanges();
                }
            }
        #endregion
        #region M:Update(SqlConnection,Asn1Certificate)
        private static void Update(SqlConnection context, Asn1Certificate source) {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var thumbprint = source.Thumbprint;
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder)) {
                source.WriteXml(writer);
                }
            using (var command = context.CreateCommand()) {
                command.CommandText = "[dbo].[ImportCertificate]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Thumbprint", SqlDbType.NVarChar) {
                    Value = source.Thumbprint
                    });
                command.Parameters.Add(new SqlParameter("@Body", SqlDbType.Xml) {
                    Value = new SqlXml(XElement.Parse(builder.ToString()).CreateReader())
                    });
                command.ExecuteNonQuery();
                }
            return;
            }
        #endregion
        #region M:Update(Entities,Asn1CertificateRevocationList)
        private static void Update(Entities context, Asn1CertificateRevocationList source) {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            //var builder = new StringBuilder();
            //using (var writer = XmlWriter.Create(builder)) {
            //    source.WriteXml(writer);
            //    }
            //using (var command = context.CreateCommand()) {
            //    command.CommandText = "[dbo].[ImportCertificateRevocationList]";
            //    command.CommandType = CommandType.StoredProcedure;
            //    command.Parameters.Add(new SqlParameter("@Thumbprint", SqlDbType.NVarChar) {
            //        Value = source.Thumbprint
            //        });
            //    command.Parameters.Add(new SqlParameter("@Body", SqlDbType.Xml) {
            //        Value = new SqlXml(XElement.Parse(builder.ToString()).CreateReader())
            //        });
            //    command.ExecuteNonQuery();
            //    }
            //return;
            var thumbprint = source.Thumbprint;
            var target = context.CertificateRevocationLists.
                Include(nameof(DCertificateRevocationList.RelativeDistinguishedNameSequence)).
                Include(nameof(DCertificateRevocationList.Object)).
                FirstOrDefault(i => i.Thumbprint == thumbprint);
            if (target == null) {
                target = new DCertificateRevocationList {
                    Country = source.Country,
                    Thumbprint = source.Thumbprint,
                    EffectiveDate = source.EffectiveDate,
                    NextUpdate = source.NextUpdate,
                    Object = context.Objects.Add(new DObject())
                    };
                ((IDObject)target).Type = DObjectType.CertificateRevocationList;
                var cache = new LocalCache();
                target.RelativeDistinguishedNameSequence  = Get(context, cache, source.Issuer);
                foreach (var extension in source.Extensions) {
                    switch (extension.Identifier.ToString()) {
                        case "2.5.29.18":
                            {
                            target.Object.Extension.Add(Get(context, cache, (Asn1IssuerAlternativeName)extension));
                            }
                            break;
                        case "2.5.29.17":
                            {
                            target.Object.Extension.Add(Get(context, cache, (Asn1SubjectAlternativeName)extension));
                            }
                            break;
                        case "2.5.29.35":
                            {
                            target.Object.Extension.Add(Get(context, cache, (CertificateAuthorityKeyIdentifier)extension));
                            }
                            break;
                        case "2.5.29.14":
                            {
                            target.Object.Extension.Add(Get(context, cache, (CertificateSubjectKeyIdentifier)extension));
                            }
                            break;
                        }
                    }
                context.CertificateRevocationLists.Add(target);
                //context.SaveChanges();
                }
            }
        #endregion
        #region M:Update(SqlConnection,Asn1CertificateRevocationList)
        private static void Update(SqlConnection context, Asn1CertificateRevocationList source) {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder)) {
                source.WriteXml(writer);
                }
            using (var command = context.CreateCommand()) {
                command.CommandText = "[dbo].[ImportCertificateRevocationList]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Thumbprint", SqlDbType.NVarChar) {
                    Value = source.Thumbprint
                    });
                command.Parameters.Add(new SqlParameter("@Body", SqlDbType.Xml) {
                    Value = new SqlXml(XElement.Parse(builder.ToString()).CreateReader())
                    });
                command.ExecuteNonQuery();
                }
            return;
            }
        #endregion
        #region M:Update(SqlConnection,Asn1CertificateRevocationList)
        private static void Update(SqlConnection context, CmsMessage source, String key) {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder)) {
                source.WriteXml(writer);
                }
            using (var command = context.CreateCommand()) {
                command.CommandText = "[dbo].[ImportCmsMessage]";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Key", SqlDbType.NVarChar) {
                    Value = key
                    });
                command.Parameters.Add(new SqlParameter("@Body", SqlDbType.Xml) {
                    Value = new SqlXml(XElement.Parse(builder.ToString()).CreateReader())
                    });
                command.ExecuteNonQuery();
                }
            return;
            }
        #endregion

        #region M:JsonSerialize(Object,TextWriter)
        private static void JsonSerialize(Object value, TextWriter output) {
            using (var writer = new JsonTextWriter(output){
                    Formatting = Formatting.Indented,
                    Indentation = 2,
                    IndentChar = ' '
                    }) {
                var serializer = new JsonSerializer();
                if (value is IJsonSerializable o)
                    {
                    o.WriteJson(writer, serializer);
                    }
                else
                    {
                    serializer.Serialize(writer, value);
                    }
                writer.Flush();
                }
            }
        #endregion

        private static String GetShortKey(Byte[] source) {
            return $"{source[0]:X2}{source[source.Length-1]:X2}";
            }

        private static DbQuery<T> SQL<T>(DbQuery<T> source) {
            //Console.WriteLine($"\nSQL:{{{source.Sql}}}");
            return source;
            }

        private static String GetCountry(Asn1RelativeDistinguishedNameSequence source) {
            return source.TryGetValue("2.5.4.6", out var r)
                ? r.ToString().ToLower()
                : null;
            }

        private static String Combine(String x, String y) {
            if (String.IsNullOrWhiteSpace(x)) { return y; }
            if (String.IsNullOrWhiteSpace(y)) { return x; }
            return Path.Combine(x.Trim(), y.Trim());
            }

        #region M:ToStoreName(String):X509StoreName?
        private static X509StoreName? ToStoreName(String source) {
            if (source == null) { return X509StoreName.My; }
            switch (source.ToUpper())
                {
                case "ADDRESSBOOK"          : { return X509StoreName.AddressBook;          }
                case "AUTHROOT"             : { return X509StoreName.AuthRoot;             }
                case "CERTIFICATEAUTHORITY" : { return X509StoreName.CertificateAuthority; }
                case "CA"                   : { return X509StoreName.CertificateAuthority; }
                case "DISALLOWED"           : { return X509StoreName.Disallowed;           }
                case "MY"                   : { return X509StoreName.My;                   }
                case "ROOT"                 : { return X509StoreName.Root;                 }
                case "TRUSTEDPEOPLE"        : { return X509StoreName.TrustedPeople;        }
                case "TRUSTEDPUBLISHER"     : { return X509StoreName.TrustedPublisher;     }
                case "TRUSTEDDEVICES"       : { return X509StoreName.TrustedDevices;       }
                case "NTAUTH"               : { return X509StoreName.NTAuth;               }
                }
            return null;
            }
        #endregion

        private static SqlConnection CreateNativeConnection(String connection) {
            if (String.IsNullOrWhiteSpace(connection)) { return null; }
            var r = new SqlConnection(connection);
            r.Open();
            return r;
            }

        private static Entities CreateConnection(String connection) {
            if (String.IsNullOrWhiteSpace(connection)) { return null; }
            return new Entities(new EntityConnectionStringBuilder
                {
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = connection,
                Metadata = @"res://*/CertificateEntityDataModel.csdl|res://*/CertificateEntityDataModel.ssdl|res://*/CertificateEntityDataModel.msl"
                }.ConnectionString);
            }

        #region M:MakeDir(String)
        private void MakeDir(String folder) {
            if (!String.IsNullOrWhiteSpace(folder)) {
                lock (FolderLock) {
                    if (!Directory.Exists(folder)) {
                        Directory.CreateDirectory(folder);
                        }
                    }
                }
            }
        #endregion
        }
    }