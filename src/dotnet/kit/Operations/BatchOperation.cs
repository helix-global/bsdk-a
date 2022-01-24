using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using BinaryStudio.DirectoryServices;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PortableExecutable;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographicMessageSyntax;
using BinaryStudio.Security.Cryptography.DataInterchangeFormat;
using BinaryStudio.Security.Cryptography.Interchange;
using BinaryStudio.Serialization;
using Newtonsoft.Json;
using Options;
using DRelativeDistinguishedName = BinaryStudio.Security.Cryptography.Interchange.DRelativeDistinguishedName;

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
        private readonly Object FolderObject = new Object();
        private readonly Object console = new Object();
        private Entities connection;
        private MetadataScope scope;
        private X509CertificateStorage store;

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

        private enum Status
            {
            Success,
            Error,
            Skip,
            Warning
            }

        #region M:Execute(String,IFileService,Byte[],Boolean):Status
        private Status Execute(String targetfolder, IFileService fileservice, Byte[] file, Boolean checkbase64) {
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
        #region M:Execute(String,IFileService,Stream):Status
        private Status Execute(String targetfolder, IFileService fileservice, Stream sourcestream) {
            using (var o = Asn1Object.Load(sourcestream).FirstOrDefault()) {
                using (var cer = new Asn1Certificate(o))               if (!cer.IsFailed) { return Execute(targetfolder, fileservice, cer); }
                using (var crl = new Asn1CertificateRevocationList(o)) if (!crl.IsFailed) { return Execute(targetfolder, fileservice, crl); }
                using (var cms = new CmsMessage(o))                    if (!cms.IsFailed) { return Execute(targetfolder, fileservice, cms); }
                return Status.Skip;
                }
            }
        #endregion
        #region M:Execute(String,IFileService,Asn1Certificate):Int32
        private Status Execute(String targetfolder, IFileService fileservice, Asn1Certificate source) {
            var country = Flags.HasFlag(BatchOperationFlags.Group)
                ? GetCountry(source.Subject)??GetCountry(source.Issuer)
                : null;
            if (Flags.HasFlag(BatchOperationFlags.Rename)) {
                var targetfilename = Path.Combine(Combine(targetfolder,country), source.FriendlyName + ".cer");
                if (!String.Equals(targetfilename, fileservice.FileName, StringComparison.OrdinalIgnoreCase)) {
                    if (!String.IsNullOrEmpty(country)) {
                        lock (FolderObject) {
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
                if (connection != null) {
                    Update(connection, source);
                    }
                else
                    {
                    var targetfilename = Path.Combine(targetfolder, Path.GetFileName(fileservice.FileName + ".json"));
                    using (var writer = new StreamWriter(File.OpenWrite(targetfilename))) {
                        JsonSerialize(source, writer);
                        }
                    }
                }
                    if (Flags.HasFlag(BatchOperationFlags.Install))   { store.Add(new X509Certificate(source));    }
            else if (Flags.HasFlag(BatchOperationFlags.Uninstall)) { store.Remove(new X509Certificate(source)); }
            return Status.Success;
            }
        #endregion
        #region M:Execute(String,IFileService,Asn1CertificateRevocationList):Status
        private Status Execute(String targetfolder, IFileService fileservice, Asn1CertificateRevocationList source) {
            var country = Flags.HasFlag(BatchOperationFlags.Group)
                ? GetCountry(source.Issuer)
                : null;
            if (Flags.HasFlag(BatchOperationFlags.Rename)) {
                var targetfilename = Path.Combine(Combine(targetfolder,country), source.FriendlyName + ".crl");
                if (!String.Equals(targetfilename, fileservice.FileName, StringComparison.OrdinalIgnoreCase)) {
                    if (!String.IsNullOrEmpty(country)) {
                        lock (FolderObject) {
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
                if (connection != null) {
                    Update(connection, source);
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
            return Status.Success;
            }
        #endregion
        #region M:Execute(String,IFileService,CmsMessage):Int32
        private Status Execute(String targetfolder, IFileService fileservice, CmsMessage source) {
            if (Flags.HasFlag(BatchOperationFlags.Serialize)) {
                var targetfilename = Path.Combine(targetfolder, Path.GetFileName(fileservice.FileName + ".json"));
                using (var writer = new StreamWriter(File.OpenWrite(targetfilename))) {
                    JsonSerialize(source, writer);
                    }
                }
            return Status.Success;
            }
        #endregion

        private Int32 Execute(IFileService fileservice, TextWriter output) {
            if (fileservice == null) { throw new ArgumentNullException(nameof(fileservice)); }
            var E = Path.GetExtension(fileservice.FileName).ToLower();
            var targetfolder = (TargetFolder ?? Path.GetDirectoryName(fileservice.FileName)) ?? String.Empty;
            var status = Status.Skip;
            var timer = new Stopwatch();
            timer.Start();
            try
                {
                switch (E) {
                    case ".obj":
                        {
                        var o = scope.LoadObject(fileservice.FileName);
                        if (OutputType.IsJson) { JsonSerialize(o, output); }
                        }
                        break;
                    #region asn1
                    case ".crl":
                    case ".cer":
                    case ".der":
                    case ".ber":
                    case ".p7b":
                        {
                        //var r = fileservice.ReadAllBytes();
                        //status = Execute(targetfolder, fileservice, r, true);
                        using (var sourcestream = fileservice.OpenRead())
                            {
                            status = Execute(targetfolder, fileservice, sourcestream);
                            }
                        }
                        break;
                    #endregion
                    case ".ldif":
                        {
                        status = Execute(new LDIFFile(fileservice), output,
                            DirectoryServiceSearchOptions.Recursive, Filter) > 0
                            ? Status.Success
                            : Status.Skip;
                        }
                        break;
                    case ".ml" :
                        {
                        var r = new CmsMessage(fileservice.ReadAllBytes());
                        var masterList = (CSCAMasterList)r.ContentInfo.GetService(typeof(CSCAMasterList));
                        if (masterList != null) {
                            status = Execute(masterList, output,
                                DirectoryServiceSearchOptions.Recursive, Filter) > 0
                                ? Status.Success
                                : Status.Skip;
                            }
                        }
                        break;
                    default:
                        {
                        status = Status.Skip;
                        if (DirectoryService.GetService<IDirectoryService>(fileservice, out var folder)) {
                            status = Execute(folder, output,
                                DirectoryServiceSearchOptions.Recursive, Filter) > 0
                                ? Status.Success
                                : Status.Skip;
                            }
                        }
                        break;
                    }
                timer.Stop();
                switch (status) {
                    case Status.Success:
                        {
                        lock(console) {
                            Write(Out,ConsoleColor.Green, "{ok}");
                            Write(Out,ConsoleColor.Gray, ":");
                            Write(Out,ConsoleColor.Cyan, $"{{{timer.Elapsed}}}");
                            WriteLine(Out,ConsoleColor.Gray, $":{fileservice.FileName}");
                            }
                        }
                        return 1;
                    case Status.Warning:
                        {
                        lock(console) {
                            Write(Out,ConsoleColor.Yellow, "{skip}");
                            Write(Out,ConsoleColor.Gray, ":");
                            Write(Out,ConsoleColor.Cyan, $"{{{timer.Elapsed}}}");
                            WriteLine(Out,ConsoleColor.Gray, $":{fileservice.FileName}");
                            }
                        }
                        return 1;
                    case Status.Error:
                        {
                        lock(console) {
                            Write(Out,ConsoleColor.Red, "{error}");
                            Write(Out,ConsoleColor.Gray, ":");
                            Write(Out,ConsoleColor.Cyan, $"{{{timer.Elapsed}}}");
                            WriteLine(Out,ConsoleColor.Gray, $":{fileservice.FileName}");
                            }
                        if (!String.IsNullOrWhiteSpace(QuarantineFolder)) {
                            try
                                {
                                var targetfilename = Path.Combine(QuarantineFolder, Path.GetFileName(fileservice.FileName));
                                fileservice.MoveTo(targetfilename);
                                }
                            catch
                                {
                                }
                            }
                        }
                        return 1;
                    }
                }
            catch (Exception)
                {
                lock(console) {
                    Write(Out,ConsoleColor.Red, "{error}");
                    Write(Out,ConsoleColor.Gray, ":");
                    Write(Out,ConsoleColor.Cyan, $"{{{timer.Elapsed}}}");
                    WriteLine(Out,ConsoleColor.Gray, $":{fileservice.FileName}");
                    }
                throw;
                }
            return 0;
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
            var r =
                cache.RelativeDistinguishedNames.FirstOrDefault(i => (i.ObjectIdentifier.Value == type) && (i.String.Value == value))??
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

        private static String GetShortKey(Byte[] source) {
            return $"{source[0]:X2}{source[source.Length-1]:X2}";
            }

        private static DbQuery<T> SQL<T>(DbQuery<T> source) {
            //Console.WriteLine($"\nSQL:{{{source.Sql}}}");
            return source;
            }

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
                context.SaveChanges();
                }
            }

        private static void Update(Entities context, Asn1CertificateRevocationList source) {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
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
                context.SaveChanges();
                }
            }

        private Int32 Execute(IDirectoryService service, TextWriter output, DirectoryServiceSearchOptions flags, String pattern) {
            var r = 0;
            #if DEBUG
            foreach (var i in service.GetFiles(pattern, flags)) {
                try
                    {
                    Interlocked.Add(ref r, Execute(i, output));
                    }
                catch (Exception e)
                    {
                    e.Data["FileName"] = i.FileName;
                    throw;
                    }
                }
            #else
            service
                .GetFiles(pattern, flags)
                .AsParallel()
                .ForAll(i =>{
                    try
                        {
                        Interlocked.Add(ref r, Execute(i, output));
                        }
                    catch (Exception e)
                        {
                        e.Data["FileName"] = i.FileName;
                        throw;
                        }
                    });
            #endif
            GC.Collect();
            return r;
            }

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
            store = new X509CertificateStorage(ToStoreName(StoreName).GetValueOrDefault(X509StoreName.My), location);
            try
                {
                foreach (var fileitem in InputFileName) {
                    var fi = fileitem;
                    if (Path.GetFileNameWithoutExtension(fi).Contains("*")) {
                        var flags = fi.StartsWith(":")
                            ? DirectoryServiceSearchOptions.None
                            : DirectoryServiceSearchOptions.Recursive|DirectoryServiceSearchOptions.Containers;
                        fi = fi.StartsWith(":")
                            ? fi.Substring(1)
                            : fi;
                        var folder = Path.GetDirectoryName(fi);
                        var pattern = Path.GetFileName(fi);
                        if (String.IsNullOrEmpty(folder)) { folder = ".\\"; }
                        Execute(DirectoryService.GetService<IDirectoryService>(new Uri($"file://{folder}")),
                            output, flags, pattern);
                        }
                    else
                        {
                        Execute(DirectoryService.GetService<IFileService>(new Uri($"file://{fileitem}")), output);
                        }
                    }
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

        private static Entities CreateConnection(String connection) {
            if (String.IsNullOrWhiteSpace(connection)) { return null; }
            return new Entities(new EntityConnectionStringBuilder
                {
                Provider = "System.Data.SqlClient",
                ProviderConnectionString = connection,
                Metadata = @"res://*/CertificateEntityDataModel.csdl|res://*/CertificateEntityDataModel.ssdl|res://*/CertificateEntityDataModel.msl"
                }.ConnectionString);
            }

        private const Int32 SUCCESS = 0;
        private const Int32 SKIP    = 1;
        private const Int32 ERROR   = 2;
        private const Int32 WARNING = 3;
        }
    }