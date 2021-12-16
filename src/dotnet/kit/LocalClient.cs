using System;
using System.Collections.Generic;
using System.Linq;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates;
using kit;
using Kit;
using Operations;
using Options;

public class LocalClient : ILocalClient
    {
    private static readonly ILogger Logger = new ConsoleLogger();
    public Int32 Main(String[] args)
        {
        try
            {
            using (var sc = new ServiceManager()) {
                try
                    {
                    var options = Operation.Parse(args);
                    Operation.Logger = Logger;
                    Operation.ServiceManager = sc;
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
                finally
                    {
                    Operation.ServiceManager = null;
                    Operation.Logger = null;
                    }
                }
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

    void IDisposable.Dispose()
        {
        }
    }
