using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.Services;
using kit;
using Operations;
using Options;

public class LocalClient : ILocalClient
    {
    private static readonly ILogger Logger = new ConsoleLogger();
    private Service service;
    private ServiceManager sc;
    private ServiceEndPoint<ICryptographicOperations> co;

    public ICryptographicOperations CryptographicOperations { get {
        if (co == null) {
            EnsureService();
            co = new ServiceEndPoint<ICryptographicOperations>("CryptographicOperations");
            }
        return co.Channel;
        }}

    public Int32 Main(String[] args)
        {
        try
            {
            var options = Operation.Parse(args);
            Operation.Logger = Logger;
            Operation.LocalClient = this;
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
            return 0;
            }
        catch (Exception e)
            {
            Logger.Log(LogLevel.Critical, $"{e}");
            return -1;
            }
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
                installer.InstallService(Path.Combine(dir, "srv.exe"), "{kit}", "{kit}");
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
    }
