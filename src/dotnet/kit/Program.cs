using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using BinaryStudio.IO;
using BinaryStudio.PortableExecutable;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using BinaryStudio.Serialization;
using BinaryStudio.Diagnostics;
using BinaryStudio.PlatformComponents.Win32;
using kit;
using Microsoft.Win32;
using Newtonsoft.Json;
using Operations;
using Options;
#if USE_WINDOWS_API_CODE_PACK
using Microsoft.WindowsAPICodePack.Taskbar;
#endif
using Formatting = Newtonsoft.Json.Formatting;
using X509StoreLocation = BinaryStudio.Security.Cryptography.Certificates.X509StoreLocation;

namespace Kit
    {
    public class Program
        {
        #if USE_WINDOWS_API_CODE_PACK
        internal static TaskbarManager taskbar = TaskbarManager.Instance;
        #endif

        #region Usage
        private static void Usage()
            {
            Console.WriteLine("kit.exe [options]");
            Console.WriteLine("Options:");
            Console.WriteLine(@"
  -i:{input-file-name}              -- The input file name for operation.
  -o:{output-file-name}             -- The output file name for operation.
  -size:{size}[K|M[G]               -- The size parameter for current operation.
  -random                           -- Use random generator.
  -operation:{operation}            -- Use specific operation.
  -storename:{name}                 -- Specifies store name for certificate search.
                                       Available values:
                                        AddressBook,
                                        AuthRoot,
                                        CertificateAuthority,
                                        Disallowed,
                                        My (Default),
                                        Root,
                                        TrustedPeople,
                                        TrustedPublisher,
                                        TrustedDevices,
                                        NTAuth
                                        {folder}
  -storelocation:{name}             -- Specifies store location for certificate search.
                                       Available values:
                                        CurrentUser (Default),
                                        LocalMachine,
                                        Enterprise
  -pincode:{pincode}                -- PIN-code for crypto operations.
  -l
  -certificates:{name;name;...}     -- Specifies signer or recipient list.
  -stream                           -- Specifies stream-based methods.
  -serialize:{'xml'|'json'}         -- Output options.
  -providertype:{number}            -- Specifies CSP provider type. The default value is 75.
  -asn1:{'ber'|'der'}               -- Specified ASN1
  -signature:{operation}            -- Signature operation ('verify','create','update') options
                                        # Update signature for specified file
                                        -signature:update -i:{file} -o:{file} -signer:{thumbprint} [-storename:{storename} -storelocation:{storeloc}]

                                        # Verify signature for specified file by specified certificate thumbprint
                                        -signature:verify -i:{file} -storename:{storename} -storelocation:{storeloc} -certificates:{thumbprint}

                                        # Verify signature for specified file, by issuer certificate from specified location
                                        -signature:verify -i:{file} -storename:{storename} -storelocation:{storeloc}

                                        # Verify signature for specified file, by issuer certificate from My+CurrentUser location
                                        -signature:verify -i:{file}

                                        # Create detached simple signature for specified file
                                        -signature:create -i:{file} -certificates:{thumbprint}
    samples:
    # batch rename certificate files
    input:{folder}\*.cer batch:rename [quarantine:{folder}]
    # extracts all certificates from specified (or default) storage to specified folder.
    -o:{folder} -extract [-storename:{storename} -storelocation:{storeloc}]
    # creates an encrypted message for the specified recipients
    -i:{input-file} -o:{output-file} -message:encrypt -certificates:{name;name;...} -algid:{oid}
    -m                              -- Using message operations
    input:{input-file} output:{output-file} start:{start-offset} batch:none
");
            }
        #endregion
        #region M:ToStoreLocation(String):X509StoreLocation
        private static X509StoreLocation ToStoreLocation(String source) {
            if (source == null) { return X509StoreLocation.CurrentUser; }
            switch (source.ToUpper())
                {
                case "CURRENTUSER":  { return X509StoreLocation.CurrentUser;  }
                case "LOCALMACHINE": { return X509StoreLocation.LocalMachine; }
                case "ENTERPRISE":   { return X509StoreLocation.LocalMachineEnterprise;  }
                }
            return X509StoreLocation.CurrentUser;
            }
        #endregion
        #region M:ToStoreName(String):X509StoreName?
        private static X509StoreName ToStoreName(String source) {
            if (source == null) { return X509StoreName.My; }
            switch (source.ToUpper())
                {
                case "ADDRESSBOOK"          : { return X509StoreName.AddressBook;          }
                case "AUTHROOT"             : { return X509StoreName.AuthRoot;             }
                case "CERTIFICATEAUTHORITY" : { return X509StoreName.CertificateAuthority; }
                case "DISALLOWED"           : { return X509StoreName.Disallowed;           }
                case "MY"                   : { return X509StoreName.My;                   }
                case "ROOT"                 : { return X509StoreName.Root;                 }
                case "TRUSTEDPEOPLE"        : { return X509StoreName.TrustedPeople;        }
                case "TRUSTEDPUBLISHER"     : { return X509StoreName.TrustedPublisher;     }
                case "TRUSTEDDEVICES"       : { return X509StoreName.TrustedDevices;       }
                case "NTAUTH"               : { return X509StoreName.NTAuth;               }
                }
            return X509StoreName.My;
            }
        #endregion

        [Flags]
        internal enum Flags
            {
            None              = 0x00000000,
            List              = 0x00000001,
            VerifyCertificate = 0x00000002,
            Xml               = 0x10000000,
            Json              = 0x20000000,
            Html              = 0x20000000,
            }

        #region M:Validate(Boolean)
        protected static void Validate(Int32 status) {
            Exception e;
            var i = status;
            if ((i >= 0xFFFF) || (i < 0))
                {
                e = new COMException($"[HRESULT:{(HRESULT)i}]", i);
                #if DEBUG
                Debug.Print($"COMException:{e.Message}");
                #endif
                }
            else
                {
                e = new COMException($"[{(Win32ErrorCode)i}]", i);
                #if DEBUG
                Debug.Print($"COMException:{e.Message}");
                #endif
                }
            return;
            //throw e;
            }
        #endregion
        internal static unsafe void InternalLoad(IList<OperationOption> options, String[] args)
            {
            var xxxx = new SHA1Managed();
#if USE_WINDOWS_API_CODE_PACK
            taskbar.SetProgressValue(0, 1, WindowHandle);
#endif
#if DEBUG
            //Console.WriteLine("Press [ENTER] to continue...");
            //Console.ReadLine();
#endif
            var crrdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
                {
                using (new TraceScope()) {
                    Boolean json;
                    Boolean randomgeneration;
                    String[] inputfilename;
                    String outputfilename;
                    Boolean dumpcodes;
                    var operations = new String[0];
                    Int64 sizevalue;

                    Operation.Logger = new ConsoleLogger();
                    Operation operation = new UsageOperation(Console.Out, Console.Error, options);
                    if (!HasOption(options, typeof(ProviderTypeOption)))  { options.Add(new ProviderTypeOption(80));                             }
                    if (!HasOption(options, typeof(StoreLocationOption))) { options.Add(new StoreLocationOption(X509StoreLocation.CurrentUser)); }
                    if (!HasOption(options, typeof(StoreNameOption)))     { options.Add(new StoreNameOption(nameof(X509StoreName.My)));          }
                    if (!HasOption(options, typeof(PinCodeRequestType)))  { options.Add(new PinCodeRequestType(PinCodeRequestTypeKind.Default)); }
                    if (!HasOption(options, typeof(OutputTypeOption)))    { options.Add(new OutputTypeOption("none"));                           }
                    if (HasOption(options, typeof(MessageGroupOption))) {
                             if (HasOption(options, typeof(CreateOption)))  { operation = new CreateMessageOperation(Console.Out, Console.Error, options);  }
                        else if (HasOption(options, typeof(VerifyOption)))  { operation = new VerifyMessageOperation(Console.Out, Console.Error, options);  }
                        else if (HasOption(options, typeof(EncryptOption))) { operation = new EncryptMessageOperation(Console.Out, Console.Error, options); }
                        }
                    else if (HasOption(options, typeof(VerifyOption)))            { operation = new VerifyOperation(Console.Out, Console.Error, options);         }
                    else if (HasOption(options, typeof(InfrastructureOption)))    { operation = new InfrastructureOperation(Console.Out, Console.Error, options); }
                    else if (HasOption(options, typeof(HashOption)))              { operation = new HashOperation(Console.Out, Console.Error, options);           }
                    else if (HasOption(options, typeof(InputFileOrFolderOption))) { operation = new BatchOperation(Console.Out, Console.Error, options);          }
                    operation.Execute(Console.Out);
                    operation = null;
                    GC.Collect();
                    return;
                    for (var i = 0; i < args.Length; ++i) {
                        if (args[i][0] == '-') {
                                  if (args[i] == "-dumpcodes")     { dumpcodes = true; }
                            else if (args[i].StartsWith("-size:")) {
                                var r = Regex.Match(args[i].Substring(6).ToUpper(), @"^(\p{N}+)([KMGКМГ])?");
                                if (r.Success) {
                                    sizevalue = Int64.Parse(r.Groups[1].Value);
                                    switch (r.Groups[2].Value) {
                                        case "K":
                                        case "К":
                                            {
                                            sizevalue *= 1024;
                                            }
                                            break;
                                        case "M":
                                        case "М":
                                            {
                                            sizevalue *= 1024*1024;
                                            }
                                            break;
                                        case "G":
                                        case "Г":
                                            {
                                            sizevalue *= 1024*1024*1024;
                                            }
                                            break;
                                        }
                                    }
                                }
                            else if (args[i] == "-random")  { randomgeneration = true; }
                            }
                        }
                    if (dumpcodes) {
                        using (var scope = new MetadataScope()) {
                            if (scope.LoadObject(inputfilename[0]) is PortableExecutableSource o) {
                                using (var writer = !String.IsNullOrWhiteSpace(outputfilename)
                                    ? new StreamWriter(File.Create(outputfilename))
                                    : null)
                                    {
                                    var messagetable = o.Resources.FirstOrDefault(i => i.ToString() == "RT_MESSAGETABLE");
                                    if (messagetable != null) {
                                        if (messagetable.Resources[0].Resources[0] is IDictionary<UInt32,String> values) {
                                            foreach (var value in values) {
                                                if (writer != null)
                                                    {
                                                    writer.WriteLine($"{value.Key}:{value.Value}");
                                                    }
                                                else
                                                    {
                                                    Console.WriteLine($"{value.Key}:{value.Value}");
                                                    }
                                                }
                                            }
                                        }
                                    if (writer != null)
                                        {
                                        writer.Write("------------------- RT_STRING ------------------------");
                                        }
                                    var strings = o.Resources.FirstOrDefault(i => i.ToString() == "RT_STRING");
                                    if (strings.Resources[0] is IDictionary<UInt32,String> src) {
                                        foreach (var value in src) {
                                            if (writer != null)
                                                {
                                                writer.WriteLine($"{value.Key}:{value.Value}");
                                                }
                                            else
                                                {
                                                Console.WriteLine($"{value.Key}:{value.Value}");
                                                }
                                            }
                                        }
                                    if (writer != null)
                                        {
                                        writer.Flush();
                                        }
                                    }
                                }
                            }
                        }
                    else if (randomgeneration)
                        {
                        if (String.IsNullOrWhiteSpace(outputfilename)) { throw new ArgumentOutOfRangeException("outputfilename"); }
                        if (sizevalue == -1) { throw new ArgumentOutOfRangeException("sizevalue"); }
                        var random = new Random();
                        var sz = sizevalue;
                        var buffersize = 8*1024*1024;
                        var buffer = new Byte[buffersize];
                        var i = 0L;
                        using (var target = File.Create(outputfilename)) {
                            for (;;) {
                                Thread.Yield();
                                random.NextBytes(buffer);
                                var count = (Int32)Math.Max(0, Math.Min(buffersize, sz - i));
                                if (count == 0) { break; }
                                target.Write(buffer, 0, count);
                                i = i + buffersize;
                                }
                            }
                        }
                    else if (json)
                        {
                        JsonOperations.GenerateJson(inputfilename[0], outputfilename, "*.*");
                        }
                    #region Operations
                    else if (operations.Length > 0)
                    {
                        var providertype = 81;
                        using (var context = new SCryptographicContext((CRYPT_PROVIDER_TYPE)providertype, CryptographicContextFlags.CRYPT_SILENT | CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                            var storename = "MY";
                            var storeloc  = X509StoreLocation.CurrentUser;
                            String certificates   = null;
                            var store = Utilities.BuildCertificateList(storeloc, storename, certificates, (CRYPT_PROVIDER_TYPE)providertype);
                            #region Certificate Operations
                                {
                                foreach (var certificate in store.Certificates)
                                    {
                                    Console.WriteLine("Certificate:{0}", certificate.FriendlyName);
                                    if (HasFlag(operations, "verify")) {
                                        if (certificate.Verify(out var e, context, store)) {
                                            using (new ConsoleColorScope(ConsoleColor.Green))
                                                {
                                                Console.WriteLine("  Status:[no errors]");
                                                }
                                            }
                                        else
                                            {
                                            using (new ConsoleColorScope(ConsoleColor.Red))
                                                {
                                                if (e is AggregateException a) {
                                                    Console.WriteLine("  Status:");
                                                    Console.WriteLine(String.Join("\n", a.InnerExceptions.Select(i => $"    {i.Message}")));
                                                    }
                                                else
                                                    {
                                                    Console.WriteLine(@"  Status:""{0}""", e.Message);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            #endregion
                            }
                    }
                    #endregion
                    else
                        {
                        Usage();
                        }
                    }
                //TraceManager.Instance.Write(Console.Out);
                }
            catch(OptionRequiredException exception)
                {
                Console.WriteLine("error: required option missing");
                exception.Descriptor.Usage(Console.Error);
                Console.WriteLine();
                }
            catch(Exception x)
                {
                (new ExceptionFormatter()).Write(Console.Error, x);
                //Console.WriteLine(e.Message);
                //Console.WriteLine(e.StackTrace);
                }
            }

        //private static void OnPercentageChanged(Object sender, PercentageChangedEventArgs e)
        //    {
        //    switch (e.State)
        //        {
        //        case ProgressState.Indeterminate:
        //            {
        //            #if USE_WINDOWS_API_CODE_PACK
        //            taskbar.SetProgressState(TaskbarProgressBarState.Indeterminate, WindowHandle);
        //            taskbar.SetProgressValue(0, 0, WindowHandle);
        //            #endif
        //            }
        //            break;
        //        case ProgressState.None:
        //            {
        //            #if USE_WINDOWS_API_CODE_PACK
        //            taskbar.SetProgressState(TaskbarProgressBarState.NoProgress, WindowHandle);
        //            taskbar.SetProgressValue(0, 0, WindowHandle);
        //            #endif
        //            }
        //            break;
        //        case ProgressState.Normal:
        //            {
        //            #if USE_WINDOWS_API_CODE_PACK
        //            taskbar.SetProgressState(TaskbarProgressBarState.Normal, WindowHandle);
        //            taskbar.SetProgressValue((Int32)(e.Value*100), 100, WindowHandle);
        //            #endif
        //            }
        //            break;
        //        }
        //    }

        [MTAThread]
        internal static void Main(String[] args)
            {
            var options = Operation.Parse(args);
            using (new TraceScope())
                {
                InternalLoad(options, args);
                }
            using (new TraceScope("GC.Collect")) {
                GC.Collect();
                }
            if (HasOption(options, typeof(TraceOption))) {
                using (new ConsoleColorScope(ConsoleColor.DarkGray))
                    {
                    TraceScope.WriteTo(Console.Out);
                    //TraceManager.Instance.Write(Console.Out);
                    }
                }
            }

        #region M:GetFolderAbsolutePath(String):String
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Unicode, SetLastError = true, EntryPoint = "GetFullPathNameW")] private static extern Int32 GetFullPathName(String filename, Int32 bufferlength, [MarshalAs(UnmanagedType.LPArray)] Byte[] buffer, IntPtr filepart);
        private static String GetFolderAbsolutePath(String folder)
            {
            var buffer = new Byte[32767*2];
            var r = GetFullPathName(folder, 32767, buffer, IntPtr.Zero);
            return Encoding.Unicode.GetString(buffer, 0, r*2);
            }
        #endregion
        #region M:ListCertificates(String,X509StoreLocation,Flags,IList<String>)
        private static void ListCertificates(CRYPT_PROVIDER_TYPE providertype, TextWriter output, String storename, X509StoreLocation storeloc, Flags flags, IList<String> filters) {
            using (new TraceScope()) {
                using (var writer = !flags.HasFlag(Flags.Xml)
                    ? null
                    : XmlWriter.Create(output ?? Console.Out, new XmlWriterSettings {
                        Indent = true,
                        OmitXmlDeclaration = true,
                        })) {
                    using (IX509CertificateStorage store = (storename == "device")
                        ? (IX509CertificateStorage)(new SCryptographicContext(providertype, CryptographicContextFlags.CRYPT_SILENT|CryptographicContextFlags.CRYPT_VERIFYCONTEXT)).GetService(typeof(IX509CertificateStorage))
                        : Directory.Exists(storename)
                            ? (IX509CertificateStorage)new X509CertificateStorage(new Uri(GetFolderAbsolutePath(storename)))
                            : new X509CertificateStorage(ToStoreName(storename), storeloc)) {
                        if (writer != null) { writer.WriteStartElement("Certificates"); }
                        foreach (var certificate in store.Certificates) {
                            PrintCertificate(writer, certificate, flags);
                            }
                        if (writer != null) { writer.WriteEndElement(); }
                        }
                    }
                }
            }
        #endregion
        #region M:PrintCertificate(X509Certificate,Flags)
        private static void PrintCertificate(XmlWriter writer, IX509Certificate certificate, Flags flags)
            {
            using (new TraceScope()) {
                if (flags.HasFlag(Flags.Xml)) {
                    if (flags.HasFlag(Flags.VerifyCertificate))
                        {
                        try
                            {
                            //certificate.Verify();
                            //certificate.WriteXml(writer);
                            }
                        catch (Exception e)
                            {
                            (new ExceptionXmlSerializer()).Write(writer, e);
                            }
                        }
                    else
                        {
                        //certificate.WriteXml(writer);
                        }
                    }
                else
                    {
                    Console.Write($"{certificate.Thumbprint}");
                    }
                Console.WriteLine();
                }
            }
        #endregion
        #region M:ToStringArray(String):String[]
        private static String[] ToStringArray(String source)
            {
            if (source == null) { return new String[0]; }
            return source.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(Replace).ToArray();
            }
        #endregion
        #region M:Replace(String):String
        private static String Replace(String source) {
            if (source == null) { return null; }
            return source.ToUpper().Replace(" ", "");
            }
        #endregion
        #region M:OidFromSource(String):Oid
        private static Oid OidFromSource(String oid)
            {
            if (oid == null) { throw new ArgumentNullException(nameof(oid)); }
            if (String.IsNullOrWhiteSpace(oid)) { throw new ArgumentOutOfRangeException(nameof(oid)); }
            oid = oid.Replace(" ", String.Empty).ToUpper();
            return new Oid(oid);
            }
        #endregion

        [DllImport("kernel32.dll")] private static extern IntPtr GetConsoleWindow();

        public static IntPtr WindowHandle { get {
            var r = GetConsoleWindow();
            //Debug.Print($"{(Int64)r:X}");
            return r;
            }}

        private static Boolean HasFlag(String[] values, String value)
            {
            if ((values == null) || (values.Length == 0)) { return false; }
            foreach (var i in values) {
                if (String.Equals(i, value, StringComparison.OrdinalIgnoreCase)) {
                    return true;
                    }
                }
            return false;
            }

        #region M:JsonSerialize(Object,TextWriter)
        private static void JsonSerialize(Object value, TextWriter output) {
            //Console.WriteLine(value.ToString());
            using (var writer = new JsonTextWriter(output){
                    Formatting = Formatting.Indented,
                    Indentation = 2,
                    IndentChar = ' '
                    })
                {
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
        #region M:XmlSerialize(Object,TextWriter)
        private static void XmlSerialize(Object value, TextWriter output)
            {
            if (value is IXmlSerializable serializable) {
                using (var writer = XmlWriter.Create(output, new XmlWriterSettings {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineOnAttributes = true
                    }))
                    {
                    serializable.WriteXml(writer);
                    }
                }
            }
        #endregion

        private static Boolean HasOption(IList<OperationOption> source, Type type) {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            return source.Any(i => i.GetType() == type);
            }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)] private static extern IntPtr LoadLibraryExW([In] string lpwLibFileName, [In] IntPtr hFile, [In] uint dwFlags);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern Boolean FreeLibrary(IntPtr module);
        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Ansi)] private static extern IntPtr GetProcAddress(IntPtr module, String procedure);
        [DllImport("kernel32.dll")] private static extern Int32 GetProcessId(IntPtr handle);
        [DllImport("kernel32.dll")] private static extern IntPtr GetCurrentProcess();
        }
    }
