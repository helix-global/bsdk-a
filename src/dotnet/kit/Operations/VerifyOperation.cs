﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinaryStudio.Diagnostics;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography;
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
        private CertificateChainPolicy CertificateChainPolicy { get; }
        private readonly Object so = new Object();
        private readonly Object bo = new Object();
        private const Int32 PURGE = 1000000;
        private TimeSpan? MinElapsedTicks;
        private TimeSpan? MaxElapsedTicks;
        private TimeSpan? AvgElapsedTicks;
        private readonly EventWaitHandle E = new AutoResetEvent(false);
        private readonly CancellationTokenSource B = new CancellationTokenSource();
        private volatile Int32 NumberOfErrors = 0;

        public VerifyOperation(TextWriter output, TextWriter error, IList<OperationOption> args)
            : base(output, error, args)
            {
            CertificateChainPolicy = CertificateChainPolicy.CERT_CHAIN_POLICY_BASE;
            ProviderType  = (CRYPT_PROVIDER_TYPE)args.OfType<ProviderTypeOption>().First().Type;
            Policy        = args.OfType<PolicyOption>().FirstOrDefault()?.Value;
            InputFileName = args.OfType<InputFileOrFolderOption>().FirstOrDefault()?.Values.FirstOrDefault();
            DateTime      = args.OfType<DateTimeOption>().First().Value;
            if (Policy != null) {
                switch (Policy) {
                    case "base"    : CertificateChainPolicy = CertificateChainPolicy.CERT_CHAIN_POLICY_BASE;                break;
                    case "auth"    : CertificateChainPolicy = CertificateChainPolicy.CERT_CHAIN_POLICY_AUTHENTICODE;        break;
                    case "authts"  : CertificateChainPolicy = CertificateChainPolicy.CERT_CHAIN_POLICY_AUTHENTICODE_TS;     break;
                    case "ssl"     : CertificateChainPolicy = CertificateChainPolicy.CERT_CHAIN_POLICY_SSL;                 break;
                    case "basic"   : CertificateChainPolicy = CertificateChainPolicy.CERT_CHAIN_POLICY_BASIC_CONSTRAINTS;   break;
                    case "ntauth"  : CertificateChainPolicy = CertificateChainPolicy.CERT_CHAIN_POLICY_NT_AUTH;             break;
                    case "msroot"  : CertificateChainPolicy = CertificateChainPolicy.CERT_CHAIN_POLICY_MICROSOFT_ROOT;      break;
                    case "ev"      : CertificateChainPolicy = CertificateChainPolicy.CERT_CHAIN_POLICY_EV;                  break;
                    case "ssl_f12" : CertificateChainPolicy = CertificateChainPolicy.CERT_CHAIN_POLICY_SSL_F12;             break;
                    case "ssl_hpkp": CertificateChainPolicy = CertificateChainPolicy.CERT_CHAIN_POLICY_SSL_HPKP_HEADER;     break;
                    case "third"   : CertificateChainPolicy = CertificateChainPolicy.CERT_CHAIN_POLICY_THIRD_PARTY_ROOT;    break;
                    case "ssl_key" : CertificateChainPolicy = CertificateChainPolicy.CERT_CHAIN_POLICY_SSL_KEY_PIN;         break;
                    case "icao"    : CertificateChainPolicy = CertificateChainPolicy.Icao;                                  break;
                    default: throw new NotSupportedException();
                    }
                }
            }

        public override void Break() {
            Thread.Yield();
            if (!B.IsCancellationRequested) {
                lock (bo)
                    {
                    WriteLine(Out,ConsoleColor.Magenta, "{breaking...}");
                    B.Cancel();
                    E.WaitOne();
                    }
                }
            }

        #region M:Execute(TextWriter)
        public override void Execute(TextWriter output) {
            try
                {
                using (var context = new CryptographicContext(Logger, ProviderType, CryptographicContextFlags.CRYPT_VERIFYCONTEXT|CryptographicContextFlags.CRYPT_SILENT))
                using (var store = new X509CombinedCertificateStorage(true,
                        new X509CertificateStorage(X509StoreName.Root, X509StoreLocation.LocalMachine),
                        new X509CertificateStorage(X509StoreName.CertificateAuthority, X509StoreLocation.LocalMachine))) {
                        if (Path.GetFileNameWithoutExtension(InputFileName).Contains("*")) {
                            var folder = Path.GetDirectoryName(InputFileName);
                            var pattern = Path.GetFileName(InputFileName);
                            if (String.IsNullOrEmpty(folder)) { folder = ".\\"; }
                            var j = 0;
                            #if DEBUG
                            foreach (var i in Directory.GetFiles(folder, pattern, SearchOption.AllDirectories)) {
                                Execute(context, store, i);
                                if (j%PURGE == 0)
                                    {
                                    GC.Collect();
                                    }
                                j++;
                                if (B.IsCancellationRequested) { break; }
                                }
                            #else
                            Parallel.ForEach(Directory.GetFiles(folder, pattern, SearchOption.AllDirectories),
                                new ParallelOptions
                                    {
                                    CancellationToken = B.Token
                                    },
                                (i,state) =>
                                {
                                try
                                    {
                                    Thread.Yield();
                                    Execute(context, store, i);
                                    }
                                catch (Exception e)
                                    {
                                    state.Stop();
                                    throw;
                                    }
                                });
                            #endif
                            }
                        else
                            {
                            Execute(context, store, InputFileName);
                            }
                        }
                Write(Out,ConsoleColor.Gray,  "Min:");Write(Out,ConsoleColor.Cyan, $"{{{MinElapsedTicks}}}");
                Write(Out,ConsoleColor.Gray, ":Max:");Write(Out,ConsoleColor.Cyan, $"{{{MaxElapsedTicks}}}");
                Write(Out,ConsoleColor.Gray, ":Avg:");Write(Out,ConsoleColor.Cyan, $"{{{AvgElapsedTicks}}}");
                Write(Out,ConsoleColor.Gray, ":Errors:");Write(Out,ConsoleColor.Red, $"{{{NumberOfErrors}}}");
                Out.WriteLine();
                }
            catch(OperationCanceledException)
                {
                throw;
                }
            catch(Exception e)
                {
                Logger.Log(LogLevel.Warning, e);
                throw;
                }
            finally
                {
                E.Set();
                Thread.Yield();
                }
            }
        #endregion
        #region M:Execute(CryptographicContext,String)
        private void Execute(CryptographicContext context, IX509CertificateStorage store, String filename) {
            if (filename == null) { throw new ArgumentNullException(nameof(filename)); }
            Thread.Yield();
            Thread.Sleep(0);
            var E = Path.GetExtension(filename).ToLower();
            var timer = new Stopwatch();
            timer.Start();
            switch (E) {
                case ".cer":
                    {
                    using (var certificate = new X509Certificate(File.ReadAllBytes(filename))) {
                        try
                            {
                            B.Token.ThrowIfCancellationRequested();
                            context.Verify(
                                certificate, store, null, null,
                                TimeSpan.Zero, DateTime,
                                CERT_CHAIN_FLAGS.CERT_CHAIN_REVOCATION_CHECK_CHAIN|CERT_CHAIN_FLAGS.CERT_CHAIN_TIMESTAMP_TIME,
                                CertificateChainPolicy,IntPtr.Zero);
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
                        catch (OperationCanceledException)
                            {
                            timer.Stop();
                            lock(so)
                                {
                                Write(Out,ConsoleColor.Yellow, "{break}");
                                Write(Out,ConsoleColor.Gray, ":");
                                Write(Out,ConsoleColor.Cyan, $"{{{timer.Elapsed}}}");
                                WriteLine(Out,ConsoleColor.Gray, $":{filename}");
                                MaxElapsedTicks = Max(MaxElapsedTicks, timer.ElapsedTicks);
                                MinElapsedTicks = Min(MinElapsedTicks, timer.ElapsedTicks);
                                AvgElapsedTicks = Avg(AvgElapsedTicks, timer.ElapsedTicks);
                                }
                            throw;
                            }
                        catch (Exception e)
                            {
                            Interlocked.Increment(ref NumberOfErrors);
                            e.Add("Service", filename);
                            e.Add("Policy" , CertificateChainPolicy);
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
                                Logger.Log(LogLevel.Warning, e);
                                }
                            }
                        }
                    }
                    break;
                case ".hex":
                    {
                    var policy = (CertificateChainPolicy == CertificateChainPolicy.Icao)
                            ? VerificationPolicy.Icao
                            : VerificationPolicy.Default;
                    try
                        {
                        B.Token.ThrowIfCancellationRequested();
                        using (var sourcestream = File.OpenRead(filename)) {
                            context.VerifyAttachedMessageSignature(
                                sourcestream, null,
                                out var certificates,
                                null, policy);
                            }
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
                    catch (OperationCanceledException)
                        {
                        timer.Stop();
                        lock(so)
                            {
                            Write(Out,ConsoleColor.Yellow, "{break}");
                            Write(Out,ConsoleColor.Gray, ":");
                            Write(Out,ConsoleColor.Cyan, $"{{{timer.Elapsed}}}");
                            WriteLine(Out,ConsoleColor.Gray, $":{filename}");
                            MaxElapsedTicks = Max(MaxElapsedTicks, timer.ElapsedTicks);
                            MinElapsedTicks = Min(MinElapsedTicks, timer.ElapsedTicks);
                            AvgElapsedTicks = Avg(AvgElapsedTicks, timer.ElapsedTicks);
                            }
                        throw;
                        }
                    catch (Exception e)
                        {
                        Interlocked.Increment(ref NumberOfErrors);
                        e.Add("Service", filename);
                        e.Add("Policy" , policy);
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