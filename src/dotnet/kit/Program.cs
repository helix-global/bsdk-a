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
using BinaryStudio.DataProcessing;
using BinaryStudio.IO;
using BinaryStudio.PortableExecutable;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using BinaryStudio.Security.Cryptography.CryptographicMessageSyntax;
using BinaryStudio.Serialization;
using BinaryStudio.Diagnostics;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.PortableExecutable.DebugEngine;
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
            //using (var sc = new TraceScope())
            //    {
            //    var x1 = sc.Enter("1");
            //    var x2 = sc.Enter("2");
            //    sc.Leave(x2);
            //    sc.Leave(x1);
            //    sc.Print();
            //    }

            /*
            #if H1
            var encoder = new ASCII85Encoder();
            unsafe
                {
                for (var i = 0; i < 64; i++) {
                    var buffer = new Byte[64];
                    Array.Copy(Guid.NewGuid().ToByteArray(), 0, buffer, 0, 16);
                    Array.Copy(Guid.NewGuid().ToByteArray(), 0, buffer, 16, 16);
                    Array.Copy(Guid.NewGuid().ToByteArray(), 0, buffer, 32, 16);
                    Array.Copy(Guid.NewGuid().ToByteArray(), 0, buffer, 48, 16);
                    using (var hash = new SHA512Managed())
                        {
                        Console.Out.WriteLine(encoder.Encode(hash.ComputeHash(buffer)));
                        }
                    }
                }
            #else
            var encoder = new ASCII85Encoder();
            unsafe
                {
                for (var i = 0; i < 64; i++) {
                    var buffer = new Byte[64];
                    Array.Copy(Guid.NewGuid().ToByteArray(), 0, buffer, 0, 16);
                    Array.Copy(Guid.NewGuid().ToByteArray(), 0, buffer, 16, 16);
                    Array.Copy(Guid.NewGuid().ToByteArray(), 0, buffer, 32, 16);
                    Array.Copy(Guid.NewGuid().ToByteArray(), 0, buffer, 48, 16);
                    using (var hash = new MD5CryptoServiceProvider())
                        {
                        Console.Out.WriteLine(encoder.Encode(hash.ComputeHash(buffer)));
                        }
                    }
                }
            return;
            #endif
            */

            //{
            //var n = Enum.GetNames(typeof(NTSTATUS)).Max(i => i.Length);
            //var format = "{0,-" + n + "} = unchecked((Int32)0x{1:X8}),";
            //foreach (var value in Enum.GetValues(typeof(NTSTATUS)))
            //    {
            //    var x = String.Format(format,
            //        Enum.GetName(typeof(NTSTATUS), value),
            //        (Int32)(NTSTATUS)value
            //        );
            //    Console.WriteLine(x);
            //    }
            //}

            //Debug.Print($"sizeof(HIDP_BUTTON_CAPS)={sizeof(HIDP_BUTTON_CAPS)}");
            //foreach (var i in HidDeviceInterface.GetDeviceInterfaces(DIGCF_FLAGS.DIGCF_PRESENT | DIGCF_FLAGS.DIGCF_DEVICEINTERFACE)) {
            //    JsonSerialize(i, Console.Out);
            //    }
            //HID.HidD_GetHidGuid(out var guid);
            //var devinfoset = HID.SetupDiGetClassDevs(ref guid, null, IntPtr.Zero, DIGCF_FLAGS.DIGCF_PRESENT | DIGCF_FLAGS.DIGCF_DEVICEINTERFACE);
            //if (devinfoset != IntPtr.Zero) {
            //    Int16 deviceIndex = 0;
            //    var deviceInterfaceData = new SP_DEVICE_INTERFACE_DATA{ Size = sizeof(SP_DEVICE_INTERFACE_DATA)};
            //    while (HID.SetupDiEnumDeviceInterfaces(devinfoset, IntPtr.Zero, ref guid, deviceIndex, ref deviceInterfaceData)) {
            //        if (HID.SetupDiGetDeviceInterfaceDetail(devinfoset, ref deviceInterfaceData, out var devicepath, IntPtr.Zero)) {
            //            Debug.Print($"[{devicepath}]");
            //            var file = HID.CreateFile(devicepath, 0, FileShare.ReadWrite, null, FileMode.Open, 0, IntPtr.Zero);
            //            if (!file.IsInvalid) {
            //                if (HID.HidD_GetPreparsedData(file, out var preparsedData)) {
            //                    var attributes = new HIDD_ATTRIBUTES { Size = sizeof(HIDD_ATTRIBUTES)};
            //                    if (HID.HidD_GetAttributes(file, ref attributes)) {
            //                        if (HID.HidP_GetCaps(preparsedData, out var capabilities) == HID.HIDP_STATUS_SUCCESS) {
            //                            Debug.Print($"  Capabilities:Usage={capabilities.Usage}[0x{(Int32)capabilities.Usage:x}];UsagePage={capabilities.UsagePage}[0x{(Int32)capabilities.UsagePage:x}]");
            //                            }
            //                        if (HID.HidP_GetButtonCaps(HIDP_REPORT_TYPE.HidP_Input, out var buttonCaps, preparsedData) == HID.HIDP_STATUS_SUCCESS) {
            //                            var i = 0;
            //                            foreach (var buttonCap in buttonCaps) {
            //                                Debug.Print($"  ButtonCap#{i}:UsagePage={buttonCap.UsagePage}[0x{(Int32)buttonCap.UsagePage:x}];Usage={buttonCap.NotRange.Usage}[0x{(Int32)buttonCap.NotRange.Usage:x};IsAlias={buttonCap.IsAlias};IsRange={buttonCap.IsRange}]");
            //                                i++;
            //                                }
            //                            }
            //                        if (HID.HidP_GetValueCaps(HIDP_REPORT_TYPE.HidP_Input, out var valueCaps, preparsedData) == HID.HIDP_STATUS_SUCCESS) {
            //                            var i = 0;
            //                            foreach (var valueCap in valueCaps) {
            //                                Debug.Print($"  ValueCap#{i}:UsagePage={valueCap.UsagePage}[0x{(Int32)valueCap.UsagePage:x}];Usage={valueCap.NotRange.Usage}[0x{(Int32)valueCap.NotRange.Usage:x};IsAlias={valueCap.IsAlias};IsRange={valueCap.IsRange}]");
            //                                i++;
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            else
            //                {
            //                var lastWin32Error = Marshal.GetLastWin32Error();
            //                Validate(lastWin32Error);
            //                }
            //            }
            //        deviceIndex++;
            //        }
            //}
            //using (var md5 = new MD5Cng())
            //    {
            //    using (var file = File.OpenRead(@"c:\program files (x86)\windows kits\10\include\10.0.17763.0\um\unknwnbase.h"))
            //        {
            //        md5.Initialize();
            //        var r = md5.ComputeHash(file);
            //        Debug.Print(String.Join("", r.Select(i => i.ToString("X2"))));
            //        }
            //    }

            //using (var device = new HidDeviceStream(@"\\?\hid#vid_060e&pid_16c1#6&321daaac&0&0000#{4d1e55b2-f16f-11cf-88cb-001111000030}"))
            //{
            //    var buffer = new Byte[1024];
            //    while (true)
            //    {
            //        var sz = device.Read(buffer, 0, buffer.Length);
            //        Debug.Print($"Read:{sz}");
            //        Thread.Sleep(1000);
            //    }
            //}
            ////var lastWin32Error = Marshal.GetLastWin32Error();
            ////Validate(lastWin32Error);

            ////var deviceInfo = new SP_DEVINFO_DATA { cbSize = sizeof(SP_DEVINFO_DATA) };
            ////while (HID.SetupDiEnumDeviceInfo(devinfoset, deviceIndex, ref deviceInfo))
            ////    {
            ////    Int32 c = 0;
            ////    HID.SetupDiGetClassPropertyKeys(ref deviceInfo.ClassGuid, IntPtr.Zero, 0, &c, DICLASSPROP_FLAGS.DICLASSPROP_INSTALLER);
            ////    var keys = new DEVPROPKEY[c];
            ////    HID.SetupDiGetClassPropertyKeys(ref deviceInfo.ClassGuid, keys, keys.Length, &c, DICLASSPROP_FLAGS.DICLASSPROP_INSTALLER);
            ////    //if (HID.SetupDiGetDeviceProperty(devinfoset, ref deviceInfo, ))
            ////    deviceIndex++;
            ////    }
            //}
            using (var scope = new MetadataScope())
                {
                //var z1 = scope.LoadObject(@"C:\Temp4\winhttp.dll.mui.dll ");
                //return;
                }

            /*
            var output = new Dictionary<String,Tuple<List<Int32>,HashSet<String>>>();
            var maxcols = 0;
            using (var reader = new CSVDataReader(
                new StreamReader(new FileStream(@"C:\TFS\h2\rfc\IA-32E.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite)),
                ";", String.Empty, CultureInfo.CurrentCulture, Encoding.UTF8, CSVDataReaderFlags.Delimited, 1)) {
                var oregex = new Regex(@"^(?:(NFx|NP)\s+)?(?:(?:(?:(REX[.][WR]|REX)(?:\s+[+])?))\s+)?(?:\s*((?:EVEX|VEX)[A-Z0-9.]+)\s+)?([A-Fa-f0-9]{2})(?:\s+([A-Fa-f0-9]{2}|REX[.][Ww]|REX))*([+]\s*(?:rb|rd|rw|i))?(?:\s*(/r|/r|/0?[0-9]))?(?:\s+(cp|cw|ib|id|io|imm8|iw|/ib|/is4|/vsib))*(\s+(00|01))?$");
                var iregex = new Regex(@"^\p{L}+");
                while (reader.Read()) {
                    var n = reader.GetInt32(0);
                    var o = reader.GetString(4);
                    var ins = reader.GetString(5);
                    if (!oregex.IsMatch(o)) { throw new InvalidDataException($"Invalid opcode at {n}"); }
                    else
                        {
                        var match = oregex.Match(o);
                        var builder = new StringBuilder();
                        var j = 0;
                        foreach (var i in match.Groups.OfType<Group>().Where(i => i.Success).Skip(1).SelectMany(i => i.Captures.OfType<Capture>()).Select(i => i.Value.Replace(" ",""))) {
                            if (j == 0) {
                                if (i.StartsWith("REX")) {
                                    builder.Append($"{i.ToUpper()}");
                                    }
                                else if (i.StartsWith("NP"))
                                    {
                                    j--;
                                    }
                                else
                                    {
                                    builder.Append(i);
                                    }
                                }
                            else
                                {
                                if (i[0] != '+') { builder.Append(' '); }
                                if (i.StartsWith("REX")) {
                                    builder.Append($"{i.ToUpper()}");
                                    }
                                else if (i.StartsWith("NP")) {
                                    
                                    }
                                else
                                    {
                                    builder.Append(i);
                                    }
                                }
                            j++;
                            }
                        var key = builder.ToString();
                        //var values = key.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries);
                        //if (!output.TryGetValue(key, out var r)) {
                        //    output.Add(key, r = Tuple.Create(new List<Int32>(),new HashSet<String>()));
                        //    }
                        //r.Item1.Add(n);
                        //r.Item2.Add(ins);
                        //maxcols = Math.Max(maxcols, r.Item2.Count);
                        Console.WriteLine($"{n};{String.Join(" ", key.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries).Select(x => $"{{{x}}}"))}");
                        }
                    }
                return;
                }

            foreach (var item in output) {
                Console.Write($"{String.Join(",", item.Value.Item1)};{item.Key};");
                if (item.Value.Item2.Count == maxcols)
                    {
                    Console.Write($"{String.Join(";", item.Value.Item2)}");
                    }
                else
                    {
                    Console.Write($"{String.Join(";", item.Value.Item2)}");
                    Console.Write(new String(';', maxcols - item.Value.Item2.Count));
                    }
                Console.WriteLine();
                }
                return;
                */
            //File.WriteAllBytes(@"C:\TFS\Old_ztfs_Collection\FCOD\Packages\ti\FT.Security.Cryptography\FT.Security.Cryptography.1.0.131\csecapi\.intermediate\x64\Release\asn1_r.obj", nfile);
            //int X = Int32.MinValue;
            //var L = LoadLibraryExW("kernel32.dll", IntPtr.Zero, 0);
            //var F = GetProcAddress(L, "AddDllDirectory");
            //var client = new DebugClient(-1);
            //var BeginOffset = F;
            //for (var i = 0; i < 30; i++)
            //{
            //    var r = client.Disassemble(BeginOffset, out var DisassemblySize, out var EndOffset);
            //    Debug.Print(r);
            //    BeginOffset = EndOffset;
            //}
            

            //using (var reader = new OpcodeStreamReader64(GetProcAddress(L, "AddDllDirectory"))) {
            ////using (var reader = new OpcodeStreamReader(new Byte[]{
            ////        0x48,0x81,0x6C,0x24,0x40,0x00,0x00,0x00,0xA0,
            ////        0x48,0x81,0xEC,0x00,0x00,0x00,0xA0,
            ////        })) {
            //    for (var i = 0; i < 100; i++) {
            //        var opcode = reader.Read();
            //        Console.WriteLine($@"Opcode:""{opcode.Size}"":{opcode.ToStringInternal()}");
            //        }
            //    return;
            //    }

            //using (var context = new SCardContext(SCardScope.User)) {
            //    foreach (var reader in context.Readers) {
            //        Debug.Print($@"Reader:""{reader.Name}""");
            //        }
            //    }

            //var n = 0;
            //using (var file = File.OpenRead(@"C:\TFS\Old_ztfs_Collection\FCOD\Packages\ti\FT.Security.Cryptography\FT.Security.Cryptography.1.0.131\packages\csecapi\OPK001_20210120175503.orig")) {
            //    using (var reader = new BinaryReader(file)) {
            //        file.Seek(4, SeekOrigin.Begin);
            //        var sz = reader.ReadInt32();
            //        var buffer = new Byte[sz];
            //        Debug.Assert(file.Read(buffer, 0, sz) == sz);
            //        File.WriteAllBytes($@"c:\temp5\out\{n:D5}.bin", buffer);
            //        n++;
            //        }
            //    }
            //return;

            //var jws = new SignedJWS(new JsonTextReader(new StreamReader(File.OpenRead(@"C:\TFS\h2\samples\ChannelManifest.json"))));
            //JsonSerialize(jws, Console.Out);

            //JsonSerialize(new CmsMessage(File.ReadAllBytes(@"C:\TFS\h2\icao\ru\rfid\rfid07\DGE040.p7")), new StreamWriter(File.Create(@"C:\TFS\h2\icao\ru\rfid\rfid07\DGE040.json")));
            //JsonSerialize(new X509CertificateRevocationList(File.ReadAllBytes(@"C:\Temp\29_09_2020.crl")), new StreamWriter(File.Create(@"c:\temp\29_09_2020.crl.json"))); 
            //return;
            //JsonSerialize(new X509CertificateRevocationList(File.ReadAllBytes(@"C:\Temp\20-02-12.crl")), new StreamWriter(File.Create(@"c:\temp\20-02-12.json"))); 
            //foreach (var file in Directory.EnumerateFiles(@"C:\TFS\h2\icao\", "*.crl", SearchOption.AllDirectories))
            //    {
            //    JsonSerialize(new X509CertificateRevocationList(File.ReadAllBytes(file)), Console.Out); 
            //    }
            //JsonSerialize(new X509Certificate(File.ReadAllBytes(@"C:\TFS\h2\icao\tm\C=TM,O=GOV,OU=SFCRS,CN=Document Signer 3,SN=61038A9D000100000093.cer")), Console.Out);
            //return;
            //foreach (var file in Directory.EnumerateFiles(@"C:\TFS\h2\icao\", "*.cer", SearchOption.AllDirectories))
            //    {
            //    try
            //        {
            //        JsonSerialize(new X509Certificate(File.ReadAllBytes(file)), Console.Out);
            //        }
            //    catch (Exception e)
            //        {
            //        Console.Error.WriteLine($@"FILE:""{file}""");
            //        Console.Error.WriteLine(e);
            //        throw;
            //        }
            //    }
            //JsonSerialize(Asn1Object.Load(@"C:\TFS\h2\icao\csca-0\cer,C=AU,O=GOV,OU=DFAT,OU=PTB,CN=Passport Country Signing Authority,SN=30.cer" ), Console.Out);
            //XmlSerialize(Asn1Object.Load(@"C:\TFS\h2\icao\csca-0\cer,C=AU,O=GOV,OU=DFAT,OU=PTB,CN=Passport Country Signing Authority,SN=30.cer").FirstOrDefault(), Console.Out);
            //XmlSerialize(new X509CertificateRevocationList(File.ReadAllBytes(@"C:\TFS\h2\icao\ru\O=ФГУП НТЦ Атлас,OU=ЦС,C=RU,L=Москва,CN=Центр Сертификации эмиссии и контроля ГС ПВДНП,ED=19-09-09,NU=19-12-09.crl")), Console.Out); 
            //JsonSerialize(new X509CertificateRevocationList(File.ReadAllBytes(@"C:\TFS\h2\icao\ru\O=ФГУП НТЦ Атлас,OU=ЦС,C=RU,L=Москва,CN=Центр Сертификации эмиссии и контроля ГС ПВДНП,ED=19-09-09,NU=19-12-09.crl")), Console.Out); 
            //JsonSerialize(new X509CertificateRevocationList(File.ReadAllBytes(@"C:\Temp\серт1.crl")), Console.Out); 
            //JsonSerialize(new X509CertificateRevocationList(File.ReadAllBytes(@"C:\TFS\h2\icao\ru\O=ФГУП НТЦ Атлас,OU=ЦС,C=RU,L=Москва,CN=Центр Сертификации эмиссии и контроля ГС ПВДНП,ED=19-09-09,NU=19-12-09.crl")), Console.Out); 
            //JsonSerialize(new X509Certificate(File.ReadAllBytes(@"C:\TFS\h2\icao\ru\O=ФГУП НТЦ Атлас,OU=ЦС,C=RU,L=Москва,CN=Центр Сертификации эмиссии и контроля ГС ПВДНП,SN=0114.cer")), Console.Out); 
            
            //return;

            //using (var context = new CryptographicContext(CRYPT_PROVIDER_TYPE.PROV_GOST_2012_256, CryptographicContextFlags.CRYPT_NONE)) {
            //    using (var store = (IX509CertificateStorage)context.GetService(typeof(IX509CertificateStorage))) {
            //        var certificate = store.Certificates.FirstOrDefault(i=>i.Thumbprint == "559D92BD773323E5BD520A385FEB1F352C8F726E");
            //        if (certificate != null) {
            //            using (var ctx = new CryptographicContext(context, certificate, CRYPT_ACQUIRE_FLAGS.CRYPT_ACQUIRE_NONE)) {
            //                ctx.SetParameter(out var e, CRYPT_PARAM.PP_PIN_PROMPT_STRING, 0 ,Encoding.Unicode, "Hello!");
            //                using (var hash = ctx.CreateHashAlgorithm(ALG_ID.CALG_GR3411)) {
            //                    var signature = new MemoryStream();
            //                    hash.Compute(Encoding.UTF8.GetBytes("The data that is to be hashed and signed."));
            //                    hash.CreateSignature(signature, KeySpec.Exchange|KeySpec.Signature);
            //                    signature.Seek(0, SeekOrigin.Begin);
            //                    Console.WriteLine($"Signature:{signature.ToArray().ToString("X")}");
            //                    }
            //                }
            //            }
            //        }
            //    }
            //return;

            var crrdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
                {
                using (new TraceScope()) {
                    var json = false;
                    var extract = false;
                    var oflags = Flags.None;
                    Boolean randomgeneration = false;
                    String tspserver      = null;
                    String[] inputfilename  = null;
                    var message = false;
                    String outputfilename = null;
                    String xmldsig        = null;
                    String certificates   = null;
                    var storename = "MY";
                    var storeloc  = X509StoreLocation.CurrentUser;
                    var providertype = 81;
                    var timestamp = "none";
                    var algid = new String[0];
                    var dumpcodes = false;
                    var keys = false;
                    var providertypes = false;
                    var operations = new String[0];
                    Int64 sizevalue = -1;
                    var SSSS = Path.GetFileNameWithoutExtension(@"C:\TFS\Old_ztfs_Collection\FCOD\Packages\ti\FT.Security.Cryptography\FT.Security.Cryptography.1.0.131\csecapi\.intermediate\*.obj");


                    #if DEBUG
//                    var x = (new ASCII85Decoder()).Decode(@"
//9jqo^BlbD-BleB1DJ+*+F(f,q/0JhKF<GL>Cj@.4Gp$d7F!,L7@<6@)/0JDEF<G%<+EV:2F!,
//O<DJ+*.@<*K0@<6L(Df-\0Ec5e;DffZ(EZee.Bl.9pF""AGXBPCsi+DGm>@3BB/F*&OCAfu2/AKY
//i(DIb:@FD,*)+C]U=@3BN#EcYf8ATD3s@q?d$AftVqCh[NqF<G:8+EV:.+Cf>-FD5W8ARlolDIa
//l(DId<j@<?3r@:F%a+D58'ATD4$Bl@l3De:,-DJs`8ARoFb/0JMK@qB4^F!,R<AKZ&-DfTqBG%G
//>uD.RTpAKYo'+CT/5+Cei#DII?(E,9)oF*2M7/c~>");
/*
                    var encoder = new ASCII85Encoder();
                    unsafe
                        {
                        for (var i = 0; i < 64; i++) {
                            var buffer = new Byte[64];
                            Array.Copy(Guid.NewGuid().ToByteArray(), 0, buffer, 0, 16);
                            Array.Copy(Guid.NewGuid().ToByteArray(), 0, buffer, 16, 16);
                            Array.Copy(Guid.NewGuid().ToByteArray(), 0, buffer, 32, 16);
                            Array.Copy(Guid.NewGuid().ToByteArray(), 0, buffer, 48, 16);
                            using (var hash = new SHA512Managed())
                                {
                                Console.Out.WriteLine(encoder.Encode(hash.ComputeHash(buffer)));
                                }
                            }
                        }
                    var x = (new ASCII85Encoder()).Encode(Encoding.ASCII.GetBytes("Man is distinguished, not only by his reason, but by this singular passion from other animals, which is a lust of the mind, that by a perseverance of delight in the continued and indefatigable generation of knowledge, exceeds the short vehemence of any carnal pleasure."));
                    */
                    //using (var scope = new MetadataScope())
                    //{
                    //    var o = scope.LoadObject(@"D:\Symbols\AboveLockAppHost.pdb\0268BACF32391B1C6525B8E51BBDD2FC1\AboveLockAppHost.pdb") as MultiStreamFile;
                    //}
                    //return;

                    //var ofile = File.ReadAllBytes(@"C:\TFS\Old_ztfs_Collection\FCOD\Packages\ti\FT.Security.Cryptography\FT.Security.Cryptography.1.0.131\csecapi\.intermediate\x64\Release\asn1.obj");
                    //var nfile = new Byte[ofile.Length - 6];
                    //Array.Copy(ofile, 6, nfile, 0, nfile.Length);
                    //File.WriteAllBytes(@"C:\TFS\Old_ztfs_Collection\FCOD\Packages\ti\FT.Security.Cryptography\FT.Security.Cryptography.1.0.131\csecapi\.intermediate\x64\Release\asn1_r.obj", nfile);
#endif
                    //Console.WriteLine($"---------------------");
                    //Console.WriteLine($"{nameof(CryptographicContext)}.{nameof(CryptographicContext.AvailableProviders)}:");
                    //foreach (var pi in CryptographicContext.AvailableProviders) {
                    //    Console.WriteLine($"  ProviderName:{pi.Key}");
                    //    Console.WriteLine($"  ProviderType:{pi.Value}");
                    //    using (var context = new CryptographicContext(null, pi.Key, pi.Value, CryptographicContextFlags.CRYPT_SILENT|CryptographicContextFlags.CRYPT_VERIFYCONTEXT, null)) {
                    //        foreach (var alg in context.SupportedAlgorithms) {
                    //            Console.WriteLine($"    Alg:{alg.Key}:{alg.Value}");
                    //            }
                    //        }
                    //    }

                    //foreach (var country in Country.Countries) {
                    //    Console.WriteLine($"INSERT INTO [dbo].[Country]([Code],[ShortName],[TwoLetterISOCountryName],[ThreeLetterISOCountryName]) VALUES ({country.Value.Code},'{country.Value.ShortName}','{country.Value.TwoLetterISOCountryName}','{country.Value.ThreeLetterISOCountryName}');");
                    //    }
                    //return;

                    Operation operation = new UsageOperation(Console.Error, options);
                    if (!HasOption(options, typeof(ProviderTypeOption)))  { options.Add(new ProviderTypeOption(80));                             }
                    if (!HasOption(options, typeof(StoreLocationOption))) { options.Add(new StoreLocationOption(X509StoreLocation.CurrentUser)); }
                    if (!HasOption(options, typeof(StoreNameOption)))     { options.Add(new StoreNameOption(nameof(X509StoreName.My)));          }
                    if (!HasOption(options, typeof(PinCodeRequestType)))  { options.Add(new PinCodeRequestType(PinCodeRequestTypeKind.Default)); }
                    if (!HasOption(options, typeof(OutputTypeOption)))    { options.Add(new OutputTypeOption("none"));                           }
                    if (HasOption(options, typeof(MessageGroupOption))) {
                             if (HasOption(options, typeof(CreateOption)))  { operation = new CreateMessageOperation(Console.Error, options);  }
                        else if (HasOption(options, typeof(VerifyOption)))  { operation = new VerifyMessageOperation(Console.Error, options);  }
                        else if (HasOption(options, typeof(EncryptOption))) { operation = new EncryptMessageOperation(Console.Error, options); }
                        }
                    else if (HasOption(options, typeof(VerifyOption)))  {
                        operation = new VerifyOperation(Console.Error, options);
                        }
                    else if (HasOption(options, typeof(InputFileOrFolderOption)))
                        {
                        operation = new BatchOperation(Console.Error, options);
                        }
                    else if (HasOption(options, typeof(InfrastructureOption)))
                        {
                        operation = new InfrastructureOperation(Console.Error, options);
                        }
                    operation.Execute(Console.Out);
                    return;
                    for (var i = 0; i < args.Length; ++i) {
                        if (args[i][0] == '-') {
                               if (args[i].StartsWith("-tsp:"))           { tspserver = args[i].Substring( 5);                  }
                            //else if (args[i].StartsWith("-message:"))       { message   = args[i].Substring( 9).Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries); }
                            else if (args[i].StartsWith("-m"))              { message = true; }
                            else if (args[i].StartsWith("-xmldsig:"))       { xmldsig   = args[i].Substring( 9);                  }
                            else if (args[i].StartsWith("-certificates:"))  { certificates = args[i].Substring(14);               }
                            else if (args[i].StartsWith("-timestamp:"))     { timestamp = args[i].Substring(11);                  }
                            else if (args[i].StartsWith("-operation:"))     { operations = args[i].Substring(11).Split(new []{';'}, StringSplitOptions.RemoveEmptyEntries); }
                            else if (args[i].StartsWith("-algid:"))         { algid = ToStringArray(args[i].Substring(7));        }
                            else if (args[i] == "-dumpcodes")               { dumpcodes = true;                                   }
                            else if (args[i] == "-l")                       { oflags |= Flags.List;                               }
                            else if (args[i] == "-keys")                    { keys   = true;                                      }
                            else if (args[i] == "-json")                    { json   = true; }
                            else if (args[i] == "-extract")                 { extract  = true; }
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
                    else if (providertype < 0) {
                        foreach (var type in SCryptographicContext.AvailableTypes)
                            {
                            Console.WriteLine($"{type.Key}[{(UInt32)type.Key}]:\"{type.Value}\"");
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
                        using (var context = new SCryptographicContext((CRYPT_PROVIDER_TYPE)providertype, CryptographicContextFlags.CRYPT_SILENT | CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                            var store = Utilities.BuildCertificateList(storeloc, storename, certificates, (CRYPT_PROVIDER_TYPE)providertype);
                            #region Message Operations
                            if (message) {
                                if (HasFlag(operations, "verify")) {
                                    if (HasFlag(operations, "asn1")) {
                                        var o = Asn1Object.Load(new ReadOnlyFileMappingStream(inputfilename[0])).FirstOrDefault();
                                        if (o != null) {
                                            var p = o.FindAll(i=>{
                                                if (i is Asn1Sequence) {
                                                    if (i.Count >= 2) {
                                                        if (i[0] is Asn1ObjectIdentifier identifier) {
                                                            if (identifier.ToString() == "1.2.840.113549.1.7.2") {
                                                                return true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                return false;
                                                }).FirstOrDefault();
                                            if (p != null) {
                                                var target = new CmsMessage(p);
                                                JsonSerialize(target, Console.Out);
                                                }
                                            }
                                        }
                                    }
                                }
                            #endregion
                            #region Certificate Operations
                            else
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
                    //else if (!String.IsNullOrEmpty(xmldsig)) {
                    //    using (var context = new CryptographicContext((CRYPT_PROVIDER_TYPE)providertype, CryptographicContextFlags.CRYPT_SILENT | CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                    //        context.PercentageChanged += OnPercentageChanged;
                    //        switch (xmldsig.ToUpper()) {
                    //            case "CREATE":
                    //                {
                    //                /*
                    //                 * -i:{input-file} -o:{output-file} -xmldsig:create -signers:{name;name;...} [-timestamp:{value}]
                    //                 */
                    //                XmlDSigOperations.CreateAttachedMessage(context,
                    //                    Utilities.BuildCertificateList(storeloc, storename, signers, (CRYPT_PROVIDER_TYPE)providertype),
                    //                    inputfilename[0], outputfilename, timestamp, XAdESFlags.IncludeSigningCertificate|XAdESFlags.IncludeTimeStamp);
                    //                }
                    //                break;
                    //            case "VERIFY":
                    //                {
                    //                HashSet<IX509Certificate> certificates;
                    //                /*
                    //                 * -i:{input-file} -xmldsig:verify
                    //                 */
                    //                XmlDSigOperations.VerifyMessage(context, inputfilename[0], outputfilename, out certificates);
                    //                foreach (var certificate in certificates)
                    //                    {
                    //                    Console.WriteLine($"{certificate.Thumbprint}");
                    //                    }
                    //                }
                    //                break;
                    //            }
                    //        }
                    //    }
                    //#region Message Operations
                    //else if (message.Length > 0) {
                    //    using (var context = new CryptographicContext((CRYPT_PROVIDER_TYPE)providertype, CryptographicContextFlags.CRYPT_SILENT | CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                    //        context.PercentageChanged += OnPercentageChanged;
                    //        #region encrypt
                    //        else if (HasFlag(message, "encrypt"))
                    //            {
                    //            if (HasFlag(message, "block"))
                    //                {
                    //                MessageOperations.EncryptMessage(context, inputfilename[0], outputfilename,
                    //                    Utilities.BuildCertificateList(storeloc, storename, signers, (CRYPT_PROVIDER_TYPE)providertype).ToList(),
                    //                    OidFromSource(algid[0]), "ber,block");
                    //                }
                    //            else
                    //                {
                    //                MessageOperations.EncryptMessage(context, inputfilename[0], outputfilename,
                    //                    Utilities.BuildCertificateList(storeloc, storename, signers, (CRYPT_PROVIDER_TYPE)providertype).ToList(),
                    //                    OidFromSource(algid[0]), "ber");
                    //                }
                    //            }
                    //        #endregion
                    //        else
                    //            {
                    //            throw new InvalidDataException();
                    //            }
                    //        }
                    //    }
                    //#endregion
                    //using (var output = new StreamWriter(File.OpenWrite(tempfilename), Encoding.UTF8)) {
                    //    if (keys) {
                    //        using (var provider = new CryptographicContext((CRYPT_PROVIDER_TYPE)providertype, CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                    //            foreach (var key in provider.EnumUserKeys(false)) {
                    //                Console.WriteLine("Container:[{0}]",(key.Container != null) ? key.Container.TrimEnd('\0') : "(none)");
                    //                }
                    //            using (var store = new CryptographicContextStorage(provider)) {
                    //                foreach (var certificate in store.Certificates) {
                    //                    Console.WriteLine("Certificate:[{0}]",certificate.Thumbprint);
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    else
                    //        {
                    //        #region [Flags.List]
                    //        if (oflags.HasFlag(Flags.List))
                    //            {
                    //            ListCertificates((CRYPT_PROVIDER_TYPE)providertype, output, storename, storeloc, oflags, new String[0]);
                    //            }
                    //        #endregion
                    //        }
                    //    }
                    //if (!oflags.HasFlag(Flags.Html)) { File.Delete(tempfilename); }
                    //else
                    //    {
                    //    var outputfile = Path.Combine(Directory.GetCurrentDirectory(), "output.xml");
                    //    if (File.Exists(outputfile)) { File.Delete(outputfile); }
                    //    File.Move(tempfilename, outputfile);
                    //    var xslt = new XslCompiledTransform();
                    //    xslt.Load(Path.Combine(Path.Combine(crrdir, "Properties"), "Certificate.xsl"));
                    //    xslt.Transform(outputfile, "output.html");
                    //    }
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
