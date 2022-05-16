﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using BinaryStudio.DirectoryServices;
using BinaryStudio.IO;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PortableExecutable;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographicMessageSyntax;
using BinaryStudio.Serialization;
using Newtonsoft.Json;
using Options;
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
        private SqlConnection NativeConnection;
        private MetadataScope scope;
        private X509CertificateStorage store;
        private readonly MultiThreadOption MultiThreadOption;
        private readonly TraceOption TraceOption;
        private Int32 NumberOfFiles = 1;
        private Int64 FileIndex = 0;
        private String GroupNumber = "";

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
            MultiThreadOption = args.OfType<MultiThreadOption>().FirstOrDefault() ??
                new MultiThreadOption{
                    NumberOfThreads = 32
                    };
            TraceOption = args.OfType<TraceOption>()?.FirstOrDefault();
            Options = (new HashSet<String>(args.OfType<BatchOption>().SelectMany(i => i.Values))).ToArray();
            if (Options.Any(i => String.Equals(i, "rename",    StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Rename;    }
            if (Options.Any(i => String.Equals(i, "serialize", StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Serialize; }
            if (Options.Any(i => String.Equals(i, "extract",   StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Extract;   }
            if (Options.Any(i => String.Equals(i, "group",     StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Group;     }
            if (Options.Any(i => String.Equals(i, "install",   StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Install;   }
            if (Options.Any(i => String.Equals(i, "uninstall", StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Uninstall; }
            if (Options.Any(i => String.Equals(i, "report",    StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Report;    }
            if (Options.Any(i => String.Equals(i, "force",     StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.Force;     }
            if (Options.Any(i => String.Equals(i, "asn1",      StringComparison.OrdinalIgnoreCase))) { Flags |= BatchOperationFlags.AbstractSyntaxNotation; }

            foreach (var option in Options) {
                if (option.StartsWith("group=", StringComparison.OrdinalIgnoreCase)) {
                    Flags |= BatchOperationFlags.Group;
                    GroupNumber = option.Substring(6);
                    break;
                    }
                }
            if (!String.IsNullOrWhiteSpace(TargetFolder)) {
                if (TargetFolder.StartsWith("oledb://")) {
                    TargetConnectionString = TargetFolder.Substring(8).Trim();
                    }
                else if (!TargetFolder.StartsWith(@"\\?\")) {
                    TargetFolder = $@"\\?\{TargetFolder}";
                    }
                }
            }

        private void UpdateTitle()
            {
            lock(this)
                {
                var fileindex = Interlocked.Read(ref FileIndex);
                Console.Title = $"Total:{NumberOfFiles}:FileCount:{fileindex}:{((Single)fileindex/NumberOfFiles)*100:F2}%";
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
            sourcestream.Seek(0, SeekOrigin.Begin);
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
                    Update(NativeConnection, source,
                        Path.GetFileNameWithoutExtension(fileservice.FileName),
                        GroupNumber, Flags.HasFlag(BatchOperationFlags.Force));
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
                    try
                        {
                        using (var sourcestream = service.OpenRead()) {
                            status = Execute(e.TargetFolder,service, sourcestream);
                            }
                        }
                    catch
                        {
                        if (QuarantineFolder != null) {
                            MakeDir(QuarantineFolder);
                            service.CopyTo(Path.Combine(QuarantineFolder, service.FileName), true);
                            }
                        throw;
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
            var timer = new Stopwatch();
            timer.Start();
            var location = StoreLocation.GetValueOrDefault(X509StoreLocation.CurrentUser);
            if (Flags.HasFlag(BatchOperationFlags.Install) || Flags.HasFlag(BatchOperationFlags.Uninstall)) {
                if (location == X509StoreLocation.LocalMachine) {
                    PlatformContext.ValidatePermission(WindowsBuiltInRole.Administrator);
                    }
                }
            scope = new MetadataScope();
            NativeConnection = CreateNativeConnection(TargetConnectionString);
            store = new X509CertificateStorage(ToStoreName(StoreName).GetValueOrDefault(X509StoreName.My), location);
            try
                {
                var core = new FileOperation(Out,Error,new OperationOption[]{
                        MultiThreadOption,
                        TraceOption
                        }) {
                    TargetFolder = TargetFolder,
                    Pattern = Filter,
                    Options = DirectoryServiceSearchOptions.Recursive|DirectoryServiceSearchOptions.Containers,
                    ExecuteAction = Execute,
                    };
                core.DirectoryServiceRequest += DirectoryServiceRequest;
                core.DirectoryCompleted += DirectoryCompleted;
                core.NumberOfFilesNotify += NumberOfFilesNotify;
                core.FileCompleted += FileCompleted;
                core.Execute(InputFileName);
                if (Flags.HasFlag(BatchOperationFlags.Install) || Flags.HasFlag(BatchOperationFlags.Uninstall)) {
                    store.Commit();
                    }
                Write(Out,ConsoleColor.Gray, "Total:" );Write(Out,ConsoleColor.Yellow, $"{{{timer.Elapsed}}}");
                }
            finally
                {
                Dispose(ref scope);
                Dispose(ref store);
                }
            GC.Collect();
            GC.Collect();
            }

        private void FileCompleted(Object sender, EventArgs e)
            {
            Interlocked.Increment(ref FileIndex);
            UpdateTitle();
            }

        private void NumberOfFilesNotify(Object sender, NumberOfFilesNotifyEventArgs e)
            {
            NumberOfFiles += e.NumberOfFiles;
            }

        private void DirectoryCompleted(Object sender, EventArgs e) {
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
            var status = FileOperationStatus.Skip;
            foreach (var o in Asn1Object.Load(new ReadOnlyMemoryMappingStream(service.ReadAllBytes()))) {
                if (o.IsDecoded && !o.IsFailed) {
                    if (o.Count == 1) {
                        if ((o.Class == Asn1ObjectClass.Application) && (((IAsn1Object)o).Type == 23)) {
                            var cms = new CmsMessage(o[0]);
                            if (!cms.IsFailed && (cms.ContentInfo is CmsSignedDataContentInfo signedData)) {
                                var certificates = signedData.Certificates.ToArray();
                                if (certificates.Length > 0) {
                                    var country = certificates[0].Country;
                                    if (country == null) {
                                        if (service is HexFile hxfile) {
                                            if (hxfile.CountryICAO != null) {
                                                IcaoCountry.ThreeLetterCountries.TryGetValue(hxfile.CountryICAO, out country);
                                                }
                                            }
                                        }
                                    country = country ?? String.Empty;
                                    if (Flags.HasFlag(BatchOperationFlags.Group)) { targetfolder = Path.Combine(targetfolder, country); }
                                    MakeDir(targetfolder);
                                    if (!PathUtils.IsSame(service.FullName, Path.Combine(targetfolder, service.FileName))) {
                                        service.CopyTo(Path.Combine(targetfolder, service.FileName), true);
                                        }
                                    //((IFileService)certificates[0]).CopyTo(Path.Combine(targetfolder, Path.ChangeExtension(service.FileName, ".cer")), true);
                                    ((IFileService)cms).CopyTo(Path.Combine(targetfolder, Path.ChangeExtension(service.FileName, ".p7b")), true);
                                    status = FileOperation.Max(status, FileOperationStatus.Success);
                                    return status;
                                    }
                                }
                            else
                                {
                                status = FileOperation.Max(status, FileOperationStatus.Warning);
                                }
                            }
                        }
                    }
                }
            if (QuarantineFolder != null)
                {
                MakeDir(QuarantineFolder);
                service.CopyTo(Path.Combine(QuarantineFolder, service.FileName), true);
                }
            return status;
            }

        #region M:Update(SqlConnection,Asn1Certificate)
        private static void Update(SqlConnection context, Asn1Certificate source) {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var thumbprint = source.Thumbprint;
            var r = XmlSerialize(source);
            lock(context) {
                using (var transaction = context.BeginTransaction()) {
                    using (var command = context.CreateCommand()) {
                        command.Transaction = transaction;
                        command.CommandText = "[dbo].[ImportCertificate]";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Thumbprint", SqlDbType.NVarChar) { Value = thumbprint });
                        command.Parameters.Add(new SqlParameter("@Body", SqlDbType.Xml) { Value = new SqlXml(XElement.Parse(r).CreateReader()) });
                        command.ExecuteNonQuery();
                        }
                    transaction.Commit();
                    }
                }
            }

        #endregion
        #region M:Update(SqlConnection,Asn1CertificateRevocationList)
        private static void Update(SqlConnection context, Asn1CertificateRevocationList source) {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var r = XmlSerialize(source);
            lock(context) {
                using (var transaction = context.BeginTransaction()) {
                    using (var command = context.CreateCommand()) {
                        command.Transaction = transaction;
                        command.CommandText = "[dbo].[ImportCertificateRevocationList]";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Thumbprint", SqlDbType.NVarChar) { Value = source.Thumbprint });
                        command.Parameters.Add(new SqlParameter("@Body", SqlDbType.Xml) { Value = new SqlXml(XElement.Parse(r).CreateReader()) });
                        command.ExecuteNonQuery();
                        }
                    transaction.Commit();
                    }
                }
            }
        #endregion
        #region M:Update(SqlConnection,Asn1CertificateRevocationList)
        private static void Update(SqlConnection context, CmsMessage source, String key, String group, Boolean force) {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var r = XmlSerialize(source);
            lock(context) {
                var ShortFileIdentifier = String.Empty;
                if (key != null) {
                    var match = Regex.Match(key, @"(\w+)[{](\w+)[}]");
                    if ((match != null) && (match.Success)) {
                        key = match.Groups[1].Value;
                        ShortFileIdentifier = match.Groups[2].Value;
                        }
                    }
                using (var transaction = context.BeginTransaction()) {
                    using (var command = context.CreateCommand()) {
                        command.Transaction = transaction;
                        command.CommandText = "[dbo].[ImportCmsMessage]";
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@Key", SqlDbType.NVarChar) { Value = key });
                        command.Parameters.Add(new SqlParameter("@ShortFileIdentifier", SqlDbType.NVarChar) { Value = ShortFileIdentifier });
                        command.Parameters.Add(new SqlParameter("@Body", SqlDbType.Xml) { Value = new SqlXml(XElement.Parse(r).CreateReader()) });
                        command.Parameters.Add(new SqlParameter("@Thumbprint", SqlDbType.NVarChar) { Value = source.Thumbprint });
                        command.Parameters.Add(new SqlParameter("@Group", SqlDbType.NVarChar) { Value = group });
                        command.Parameters.Add(new SqlParameter("@Force", SqlDbType.Bit)      { Value = force });
                        command.ExecuteNonQuery();
                        }
                    transaction.Commit();
                    }
                }
            }
        #endregion

        #region M:XmlSerialize(IXmlSerial):String
        private static String XmlSerialize(IXmlSerializable o) {
            var r = new StringBuilder();
            using (var writer = XmlWriter.Create(r)) {
                o.WriteXml(writer);
                }
            return r.ToString();
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

        private static SqlConnection CreateNativeConnection(String connection) {
            if (String.IsNullOrWhiteSpace(connection)) { return null; }
            var r = new SqlConnection(connection);
            r.Open();
            return r;
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