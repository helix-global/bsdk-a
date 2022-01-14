using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using Microsoft.Win32;
using Options;

namespace Operations
    {
    internal class VerifyOperation : Operation
        {
        public CRYPT_PROVIDER_TYPE ProviderType { get; }
        public String Policy { get; }
        public String InputFileName { get; }
        public DateTime DateTime { get; }
        private IX509CertificateChainPolicy CertificateChainPolicy { get; }
        private readonly Object so = new Object();
        private const Int32 PURGE = 1000000;
        private TimeSpan? MinElapsedTicks;
        private TimeSpan? MaxElapsedTicks;
        private TimeSpan? AvgElapsedTicks;

        public VerifyOperation(TextWriter output, TextWriter error, IList<OperationOption> args)
            : base(output, error, args)
            {
            CertificateChainPolicy = X509CertificateChainPolicy.POLICY_BASE;
            ProviderType  = (CRYPT_PROVIDER_TYPE)args.OfType<ProviderTypeOption>().First().Type;
            Policy        = args.OfType<PolicyOption>().FirstOrDefault()?.Value;
            InputFileName = args.OfType<InputFileOrFolderOption>().FirstOrDefault()?.Values.FirstOrDefault();
            DateTime      = args.OfType<DateTimeOption>().First().Value;
            if (Policy != null) {
                switch (Policy) {
                    case "base"    : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_BASE;                break;
                    case "auth"    : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_AUTHENTICODE;        break;
                    case "authts"  : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_AUTHENTICODE_TS;     break;
                    case "ssl"     : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_SSL;                 break;
                    case "basic"   : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_BASIC_CONSTRAINTS;   break;
                    case "ntauth"  : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_NT_AUTH;             break;
                    case "msroot"  : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_MICROSOFT_ROOT;      break;
                    case "ev"      : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_EV;                  break;
                    case "ssl_f12" : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_SSL_F12;             break;
                    case "ssl_hpkp": CertificateChainPolicy = X509CertificateChainPolicy.POLICY_SSL_HPKP_HEADER;     break;
                    case "third"   : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_THIRD_PARTY_ROOT;    break;
                    case "ssl_key" : CertificateChainPolicy = X509CertificateChainPolicy.POLICY_SSL_KEY_PIN;         break;
                    case "icao"    : CertificateChainPolicy = X509CertificateChainPolicy.IcaoCertificateChainPolicy; break;
                    default: throw new NotSupportedException();
                    }
                }
            }

        #region M:Execute(TextWriter)
        public override void Execute(TextWriter output) {
            using (var context = new CryptographicContext(Logger, ProviderType, CryptographicContextFlags.CRYPT_VERIFYCONTEXT|CryptographicContextFlags.CRYPT_SILENT)) {
                using (var store = new X509CombinedCertificateStorage(true,
                    new X509CertificateStorage(X509StoreName.Root, X509StoreLocation.LocalMachine),
                    new X509CertificateStorage(X509StoreName.CertificateAuthority, X509StoreLocation.LocalMachine))) {
                    if (Path.GetFileNameWithoutExtension(InputFileName).Contains("*")) {
                        var folder = Path.GetDirectoryName(InputFileName);
                        var pattern = Path.GetFileName(InputFileName);
                        if (String.IsNullOrEmpty(folder)) { folder = ".\\"; }
                        var j = 0;
                        #if DEBUG2
                        foreach (var i in Directory.GetFiles(folder, pattern, SearchOption.AllDirectories)) {
                            Execute(context, store, i);
                            if (j%PURGE == 0)
                                {
                                GC.Collect();
                                }
                            j++;
                            }
                        #else
                        Directory.GetFiles(folder, pattern, SearchOption.AllDirectories).AsParallel().ForAll(i =>{
                            Execute(context, store, i);
                            if (j%PURGE == 0)
                                {
                                GC.Collect();
                                }
                            j++;
                            });
                        #endif
                        }
                    else
                        {
                        Execute(context, store, InputFileName);
                        }
                    }
                }
            Write(Out,ConsoleColor.Gray,  "Min:");Write(Out,ConsoleColor.Cyan, $"{{{MinElapsedTicks}}}");
            Write(Out,ConsoleColor.Gray, ":Max:");Write(Out,ConsoleColor.Cyan, $"{{{MaxElapsedTicks}}}");
            Write(Out,ConsoleColor.Gray, ":Avg:");Write(Out,ConsoleColor.Cyan, $"{{{AvgElapsedTicks}}}");
            Out.WriteLine();
            }
        #endregion
        #region M:Execute(CryptographicContext,String)
        private void Execute(CryptographicContext context, IX509CertificateStorage store, String filename) {
            if (filename == null) { throw new ArgumentNullException(nameof(filename)); }
            var E = Path.GetExtension(filename).ToLower();
            var timer = new Stopwatch();
            timer.Start();
            switch (E) {
                case ".cer":
                    {
                    using (var certificate = new X509Certificate(File.ReadAllBytes(filename))) {
                        try
                            {
                            certificate.Verify(context, CertificateChainPolicy, store, DateTime);
                            timer.Stop();
                            lock(so)
                                {
                                Write(Out,ConsoleColor.Green, "{ok}");
                                Write(Out,ConsoleColor.Gray, ":");
                                Write(Out,ConsoleColor.Cyan, $"{{{timer.Elapsed}}}");
                                WriteLine(Out,ConsoleColor.Gray, $":{filename}");
                                MaxElapsedTicks = Max(MaxElapsedTicks, timer.ElapsedTicks);
                                MinElapsedTicks = Min(MinElapsedTicks, timer.ElapsedTicks);
                                AvgElapsedTicks = Avg(AvgElapsedTicks, timer.ElapsedTicks);
                                }
                            }
                        catch (Exception e)
                            {
                            timer.Stop();
                            lock(so)
                                {
                                Write(Out,ConsoleColor.Red, "{error}");
                                Write(Out,ConsoleColor.Gray, ":");
                                Write(Out,ConsoleColor.Cyan, $"{{{timer.Elapsed}}}");
                                WriteLine(Out,ConsoleColor.Gray, $":{filename}");
                                MaxElapsedTicks = Max(MaxElapsedTicks, timer.ElapsedTicks);
                                MinElapsedTicks = Min(MinElapsedTicks, timer.ElapsedTicks);
                                AvgElapsedTicks = Avg(AvgElapsedTicks, timer.ElapsedTicks);
                                }
                            Logger.Log(LogLevel.Warning, e);
                            }
                        }
                    }
                    break;
                }
            }
        #endregion
        #region M:Min(TimeSpan?,Int64):TimeSpan
        private static TimeSpan Min(TimeSpan? x, Int64 y) {
            return new TimeSpan((x == null)
                ? y
                : Math.Min(x.Value.Ticks, y));
            }
        #endregion
        #region M:Max(TimeSpan?,Int64):TimeSpan
        private static TimeSpan Max(TimeSpan? x, Int64 y) {
            return new TimeSpan((x == null)
                ? y
                : Math.Max(x.Value.Ticks, y));
            }
        #endregion
        #region M:Avg(TimeSpan?,Int64):TimeSpan
        private static TimeSpan Avg(TimeSpan? x, Int64 y) {
            return new TimeSpan((x == null)
                ? y
                : (x.Value.Ticks + y)/2);
            }
        #endregion
        }
    }