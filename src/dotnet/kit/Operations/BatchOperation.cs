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
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PortableExecutable;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Extensions;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.Certificates.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.CryptographicMessageSyntax;
using BinaryStudio.Security.Cryptography.Interchange;
using BinaryStudio.Serialization;
using kit;
using Kit;
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

        public BatchOperation(TextWriter output, TextWriter error, IList<OperationOption> args) 
            : base(output, error, args)
            {
            InputFileName    = args.OfType<InputFileOrFolderOption>().FirstOrDefault()?.Values;
            TargetFolder     = args.OfType<OutputFileOrFolderOption>().FirstOrDefault()?.Values?.FirstOrDefault();
            QuarantineFolder = args.OfType<QuarantineFolderOption>().FirstOrDefault()?.Value;
            OutputType       = args.OfType<OutputTypeOption>().First();
            StoreLocation    = args.OfType<StoreLocationOption>().FirstOrDefault()?.Value;
            StoreName        = args.OfType<StoreNameOption>().FirstOrDefault()?.Value ?? nameof(X509StoreName.My);
            Filter           = args.OfType<FilterOption>().FirstOrDefault()?.Value;
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
                }
            }


        private void ProcessFile(Object so, MetadataScope scope, String filename, TextWriter output, X509CertificateStorage store, Entities connection)
            {
            if (filename == null) { throw new ArgumentNullException(nameof(filename)); }
            var E = Path.GetExtension(filename).ToLower();
            var targetfolder = (TargetFolder ?? Path.GetDirectoryName(filename)) ?? String.Empty;
            var F = 0;
            switch (E) {
                case ".obj":
                    {
                    var o = scope.LoadObject(filename);
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
                    var r = File.ReadAllBytes(filename);
                    try
                        {
                        #region BASE64
                        if (r.Length > 0) {
                            if (r[0] == '-') {
                                /* -----BEGIN CERTIFICATE----- */
                                var builder = new StringBuilder();
                                using (var reader = new StreamReader(new MemoryStream(r), Encoding.UTF8)) {
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
                                r = Encoding.UTF8.GetBytes(builder.ToString());
                                }
                            else
                                {
                                
                                }
                            }
                        var n = Convert.FromBase64String(Encoding.ASCII.GetString(r));
                        r = n;
                        #endregion
                        }
                    catch
                        {
                        /* do nothing */
                        }
                    try
                        {
                        var o = Asn1Object.Load(filename).FirstOrDefault();
                        if (o != null) {
                            #region certificate
                            {
                            var σ = (o is Asn1ApplicationObject)
                                ? o.FindAll(i => i is Asn1Sequence).FirstOrDefault()??o
                                : o;
                            var target = new Asn1Certificate(σ);
                            if (!target.IsFailed) {
                                var country = Flags.HasFlag(BatchOperationFlags.Group)
                                    ? GetCountry(target.Subject)??GetCountry(target.Issuer)
                                    : null;
                                if (Flags.HasFlag(BatchOperationFlags.Rename)) {
                                    var targetfilename = Path.Combine(Combine(targetfolder,country), target.FriendlyName + ".cer");
                                    if (!String.Equals(targetfilename, filename, StringComparison.OrdinalIgnoreCase)) {
                                        if (!String.IsNullOrEmpty(country)) {
                                            lock (so) {
                                                if (!Directory.Exists(Combine(targetfolder,country))) {
                                                    Directory.CreateDirectory(Combine(targetfolder,country));
                                                    }
                                                }
                                            }
                                        File.Move(filename, targetfilename);
                                        File.SetCreationTime(targetfilename, target.NotBefore);
                                        File.SetLastWriteTime(targetfilename, target.NotBefore);
                                        File.SetLastAccessTime(targetfilename, target.NotBefore);
                                        }
                                    }
                                if (Flags.HasFlag(BatchOperationFlags.Serialize)) {
                                    if (connection != null) {
                                        Update(connection, target);
                                        }
                                    else
                                        {
                                        var targetfilename = Path.Combine(targetfolder, Path.GetFileName(filename + ".json"));
                                        using (var writer = new StreamWriter(File.OpenWrite(targetfilename))) {
                                            JsonSerialize(target, writer);
                                            }
                                        }
                                    }
                                     if (Flags.HasFlag(BatchOperationFlags.Install))   { store.Add(new X509Certificate(target));    }
                                else if (Flags.HasFlag(BatchOperationFlags.Uninstall)) { store.Remove(new X509Certificate(target)); }
                                break;
                                }
                            }
                            #endregion
                            #region certificate revocation list
                            {
                            var target = new Asn1CertificateRevocationList(o);
                            if (!target.IsFailed) {
                                var country = Flags.HasFlag(BatchOperationFlags.Group)
                                    ? GetCountry(target.Issuer)
                                    : null;
                                if (Flags.HasFlag(BatchOperationFlags.Rename)) {
                                    var targetfilename = Path.Combine(Combine(targetfolder,country), target.FriendlyName + ".crl");
                                    if (!String.Equals(targetfilename, filename, StringComparison.OrdinalIgnoreCase)) {
                                        if (!String.IsNullOrEmpty(country)) {
                                            lock (so) {
                                                if (!Directory.Exists(Combine(targetfolder,country))) {
                                                    Directory.CreateDirectory(Combine(targetfolder,country));
                                                    }
                                                }
                                            }
                                        File.Move(filename, targetfilename);
                                        File.SetCreationTime(targetfilename, target.EffectiveDate);
                                        File.SetLastWriteTime(targetfilename, target.EffectiveDate);
                                        File.SetLastAccessTime(targetfilename, target.EffectiveDate);
                                        }
                                    }
                                if (Flags.HasFlag(BatchOperationFlags.Serialize)) {
                                    if (connection != null) {
                                        Update(connection, target);
                                        }
                                    else
                                        {
                                        var targetfilename = Path.Combine(targetfolder, Path.GetFileName(filename + ".json"));
                                        using (var writer = new StreamWriter(File.OpenWrite(targetfilename))) {
                                            JsonSerialize((Flags.HasFlag(BatchOperationFlags.AbstractSyntaxNotation))
                                                ? o
                                                : target, writer);
                                            }
                                        }
                                    }
                                     if (Flags.HasFlag(BatchOperationFlags.Install))   { store.Add(new X509CertificateRevocationList(target));    }
                                else if (Flags.HasFlag(BatchOperationFlags.Uninstall)) { store.Remove(new X509CertificateRevocationList(target)); }
                                break;
                                }
                            }
                            #endregion
                            #region cms
                            {
                            var target = new CmsMessage(o);
                            if (!target.IsFailed) {
                                if (Flags.HasFlag(BatchOperationFlags.Serialize)) {
                                    var targetfilename = Path.Combine(targetfolder, Path.GetFileName(filename + ".json"));
                                    using (var writer = new StreamWriter(File.OpenWrite(targetfilename))) {
                                        JsonSerialize(target, writer);
                                        }
                                    }
                                break;
                                }
                            }
                            #endregion
                            }
                        F = 1;
                        }
                    catch(Exception e)
                        {
                        Logger.Log(LogLevel.Debug, e);
                        F = 2;
                        }
                    }
                    break;
                #endregion
                case ".ldif":
                    {
                    LDIF.ExtractFromLDIF(filename, targetfolder, Flags, store, Filter);
                    }
                    break;
                case ".ml" :
                    {
                    var r = new CmsMessage(File.ReadAllBytes(filename));
                    var masterList = (CSCAMasterList)r.ContentInfo.GetService(typeof(CSCAMasterList));
                    LDIF.ExtractFromLDIF(so, masterList, targetfolder, Flags, store, Filter);
                    }
                    break;
                }
            switch (F)
                {
                case 0: { Console.Error.WriteLine($"FILE:{filename}:[OK]"); } break;
                case 1: { Console.Error.WriteLine($"FILE:{filename}:[SKIP]"); } break;
                case 2:
                    {
                    using (new ConsoleColorScope(ConsoleColor.Red)) {
                        Console.Error.WriteLine($"FILE:{filename}:[FAILED]");
                        if (!String.IsNullOrWhiteSpace(QuarantineFolder)) {
                            try
                                {
                                var targetfilename = Path.Combine(QuarantineFolder, Path.GetFileName(filename));
                                File.Move(filename, targetfilename);
                                }
                            catch
                                {
                                }
                            }
                        }
                    }
                    break;
                }
            }

        private void Verify(Asn1Certificate source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var certificate = new X509Certificate(source);
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

        private static String GetShortKey(Byte[] source) {
            return $"{source[0]:X2}{source[source.Length-1]:X2}";
            }

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
        #region M:Execute(TextWriter)
        public override void Execute(TextWriter output) {
            var location = StoreLocation.GetValueOrDefault(X509StoreLocation.CurrentUser);
            if (Flags.HasFlag(BatchOperationFlags.Install) || Flags.HasFlag(BatchOperationFlags.Uninstall)) {
                if (location == X509StoreLocation.LocalMachine) {
                    PlatformContext.ValidatePermission(WindowsBuiltInRole.Administrator);
                    }
                }
            var so = new Object();
            using (var connection = CreateConnection(TargetConnectionString))
            using (var store = new X509CertificateStorage(ToStoreName(StoreName).GetValueOrDefault(X509StoreName.My), location)) {
                using (var scope = new MetadataScope()) {
                    foreach (var fileitem in InputFileName) {
                        var fi = fileitem;
                        if (Path.GetFileNameWithoutExtension(fi).Contains("*")) {
                            var flags = fi.StartsWith(":")
                                ? SearchOption.TopDirectoryOnly
                                : SearchOption.AllDirectories;
                            fi = fi.StartsWith(":")
                                ? fi.Substring(1)
                                : fi;
                            var folder = Path.GetDirectoryName(fi);
                            var pattern = Path.GetFileName(fi);
                            if (String.IsNullOrEmpty(folder)) { folder = ".\\"; }
                            foreach (var filename in Directory.GetFiles(folder, pattern, flags)) {
                                ProcessFile(so, scope, filename, output, store, connection);
                                }
                            }
                        else
                            {
                            ProcessFile(so, scope, fileitem, output, store, connection);
                            }
                        }
                    }
                if (Flags.HasFlag(BatchOperationFlags.Install) || Flags.HasFlag(BatchOperationFlags.Uninstall)) {
                    store.Commit();
                    }
                }
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
        }
    }