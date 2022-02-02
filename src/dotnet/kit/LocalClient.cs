using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.PlatformComponents;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.Services;
using kit;
using Kit;
using log4net;
using Operations;
using Options;

public class LocalClient : ILocalClient
    {
    private static readonly ILogger Logger = new ClientLogger(LogManager.GetLogger(nameof(LocalClient)));
    private Service service;
    private ServiceManager sc;
    private ServiceEndPoint<ICryptographicOperations> co;
    private InterlockedInternal<Operation> operation = new InterlockedInternal<Operation>();
    private readonly ManualResetEvent B = new ManualResetEvent(false);

    public ICryptographicOperations CryptographicOperations { get {
        if (co == null) {
            EnsureService();
            co = new ServiceEndPoint<ICryptographicOperations>("CryptographicOperations");
            }
        return co.Channel;
        }}

    Int32 ILocalClient.Main(String[] args)
        {
        try
            {
            var options = Operation.Parse(args);
            Operation.Logger = Logger;
            Operation.LocalClient = this;
            operation.Value = new UsageOperation(Console.Out, Console.Error, options);
            if (!HasOption(options, typeof(ProviderTypeOption)))  { options.Add(new ProviderTypeOption(80));                             }
            if (!HasOption(options, typeof(StoreLocationOption))) { options.Add(new StoreLocationOption(X509StoreLocation.CurrentUser)); }
            if (!HasOption(options, typeof(StoreNameOption)))     { options.Add(new StoreNameOption(nameof(X509StoreName.My)));          }
            if (!HasOption(options, typeof(PinCodeRequestType)))  { options.Add(new PinCodeRequestType(PinCodeRequestTypeKind.Default)); }
            if (!HasOption(options, typeof(OutputTypeOption)))    { options.Add(new OutputTypeOption("none"));                           }
            if (!HasOption(options, typeof(DateTimeOption)))      { options.Add(new DateTimeOption(DateTime.Now));                       }
            if (HasOption(options, typeof(MessageGroupOption))) {
                        if (HasOption(options, typeof(CreateOption)))  { operation.Value = new CreateMessageOperation(Console.Out, Console.Error, options);  }
                else if (HasOption(options, typeof(VerifyOption)))     { operation.Value = new VerifyMessageOperation(Console.Out, Console.Error, options);  }
                else if (HasOption(options, typeof(EncryptOption)))    { operation.Value = new EncryptMessageOperation(Console.Out, Console.Error, options); }
                }
            else if (HasOption(options, typeof(VerifyOption)))            { operation.Value = new VerifyOperation(Console.Out, Console.Error, options);         }
            else if (HasOption(options, typeof(InfrastructureOption)))    { operation.Value = new InfrastructureOperation(Console.Out, Console.Error, options); }
            else if (HasOption(options, typeof(HashOption)))              { operation.Value = new HashOperation(Console.Out, Console.Error, options);           }
            else if (HasOption(options, typeof(InputFileOrFolderOption))) { operation.Value = new BatchOperation(Console.Out, Console.Error, options);          }
            operation.Value.ValidatePermission();
            Task.Factory.StartNew(()=>{
                try
                    {
                    operation.Value.Execute(Console.Out);
                    }
                finally
                    {
                    }
                }).Wait();
            Console.WriteLine("Press [ENTER] to exit...");
            Console.ReadLine();
            return 0;
            }
        catch (PrincipalPermissionException)
            {
            return Elevate(args);
            }
        catch (ThreadInterruptedException) { WriteLine(Console.Out, ConsoleColor.Magenta, "{break}"); return -1; }
        catch (OperationCanceledException) { WriteLine(Console.Out, ConsoleColor.Magenta, "{break}"); return -1; }
        catch (AggregateException e) {
            if (e.InnerExceptions.Count == 1) {
                if (e.InnerExceptions[0] is OperationCanceledException) {
                    WriteLine(Console.Out, ConsoleColor.Magenta, "{break}");
                    return -1;
                    }
                if (e.InnerExceptions[0] is PrincipalPermissionException) {
                    return Elevate(args);
                    }
                }
            Logger.Log(LogLevel.Critical, e);
            return -1;
            }
        catch (Exception e)
            {
            Logger.Log(LogLevel.Critical, e);
            return -1;
            }
        finally
            {
            B.Set();
            operation.Value = null;
            operation = null;
            }
        }

    void ILocalClient.OnCancelKeyPress(Object sender, ConsoleCancelEventArgs e) {
        e.Cancel = true;
        var r = operation.Value;
        if (r != null) {
            r.Break();
            B.WaitOne();
            }
        else
            {
            throw new OperationCanceledException();
            }
        }

    private static Int32 Elevate(IEnumerable<String> args)
        {
        var assembly = Assembly.GetEntryAssembly();
        var pi = new ProcessStartInfo
            {
            UseShellExecute = true,
            WorkingDirectory = Environment.CurrentDirectory,
            FileName = assembly.Location,
            Verb = "runas",
            Arguments = String.Join(" ", args.Select(i => $@"""{i}"""))
            };
        var r = System.Diagnostics.Process.Start(pi);
        r.WaitForExit();
        return r.ExitCode;
        }

    private static Boolean HasOption(IList<OperationOption> source, Type type) {
        if (type == null) { throw new ArgumentNullException(nameof(type)); }
        if (source == null) { throw new ArgumentNullException(nameof(source)); }
        return source.Any(i => i.GetType() == type);
        }

    #region M:EnsureServiceManager
    private void EnsureServiceManager() {
        if (sc == null) {
            sc = new ServiceManager();
            }
        }
    #endregion
    #region M:EnsureService
    private void EnsureService() {
        if (service == null) {
            EnsureServiceManager();
            while (service == null) {
                var installer = new ServiceInstaller();
                var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                installer.InstallService(Path.Combine(dir, "kit.exe"), "{kit}", "{kit}");
                service = sc.OpenService("{kit}");
                }
            if (service.CurrentState != SERVICE_STATE.SERVICE_RUNNING) {
                service.Start(Timeout.Infinite);
                }
            }
        }
    #endregion
    #region M:Dispose<T>([ref]T)
    private static void Dispose<T>(ref T o)
        where T: IDisposable
        {
        if (o != null) {
            o.Dispose();
            o = default(T);
            }
        }
    #endregion
    #region M:WriteLine(ConsoleColor,String,Object[])
    protected void WriteLine(TextWriter writer, ConsoleColor color, String format, params Object[] args) {
        using (new ConsoleColorScope(color)) {
            writer.WriteLine(format, args);
            }
        }
    #endregion
    #region M:WriteLine(ConsoleColor,String)
    protected void WriteLine(TextWriter writer, ConsoleColor color, String message) {
        using (new ConsoleColorScope(color)) {
            writer.WriteLine(message);
            }
        }
    #endregion
    #region M:Write(ConsoleColor,String)
    protected void Write(TextWriter writer, ConsoleColor color, String message) {
        using (new ConsoleColorScope(color)) {
            writer.Write(message);
            }
        }
    #endregion

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    void IDisposable.Dispose() {
        Dispose(ref co);
        if (sc != null) {
            sc.DeleteService(ref service);
            Dispose(ref sc);
            }
        Operation.LocalClient = null;
        Operation.Logger = null;
        }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)] private static extern String GetCommandLine();
    }
