using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BinaryStudio.Diagnostics;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.DirectoryServices;
using BinaryStudio.Security.Cryptography.CryptographicMessageSyntax;
using BinaryStudio.Security.Cryptography.DataInterchangeFormat;
using Options;

namespace Operations
    {
    internal class FileOperation : Operation
        {
        public String TargetFolder { get;set; }
        public DirectoryServiceSearchOptions Options { get;set; }
        public String Pattern { get;set; }
        public Func<IFileService,DirectoryServiceSearchOptions,FileOperationArgs,FileOperationStatus> ExecuteAction { get;set; }
        public event EventHandler<DirectoryServiceRequestEventArgs> DirectoryServiceRequest;
        public event EventHandler DirectoryCompleted;
        public event EventHandler FileCompleted;
        public event EventHandler<NumberOfFilesNotifyEventArgs> NumberOfFilesNotify;
        private readonly Object console = new Object();
        private Int32 NumberOfFiles = 1;
        private readonly MultiThreadOption MultiThreadOption;
        private readonly TraceOption TraceOption;
        private readonly Boolean StopOnError = false;

        public FileOperation(TextWriter output, TextWriter error, IList<OperationOption> args = null)
            :base(output, error, new OperationOption[0])
            {
            MultiThreadOption = args?.OfType<MultiThreadOption>().FirstOrDefault() ??
                new MultiThreadOption{
                    NumberOfThreads = 32
                    };
            TraceOption = args?.OfType<TraceOption>()?.FirstOrDefault();
            if (TraceOption != null) {
                StopOnError = TraceOption.HasValue("StopOnError");
                }
            }

        public override void Execute(TextWriter output) {
            throw new NotImplementedException();
            }

        #region M:Execute(IEnumerable<String>):FileOperationStatus
        public FileOperationStatus Execute(IEnumerable<String> inputsource)
            {
            var status = FileOperationStatus.Skip;
            foreach (var inputitem in inputsource) {
                var file = inputitem;
                if (Path.GetFileNameWithoutExtension(file).Contains("*")) {
                    var folder = Path.GetDirectoryName(file);
                    var pattern = Path.GetFileName(file);
                    folder = String.IsNullOrEmpty(folder) ? ".\\" : $"file://{folder}";
                    status = Max(status, Execute(
                        GetService<IDirectoryService>(folder),
                        pattern));
                    }
                else
                    {
                    status = Max(status, Execute(
                        GetService<IFileService>(new Uri($"file://{file}")),
                        Pattern = Pattern ?? "*.*"));
                    }
                }
            return status;
            }
        #endregion
        #region M:Execute(IDirectoryService,String):FileOperationStatus
        protected virtual FileOperationStatus Execute(IDirectoryService service, String pattern) {
            if (service == null) { throw new ArgumentNullException(nameof(service)); }
            var status = new InterlockedInternal<FileOperationStatus>(FileOperationStatus.Skip);
            var files = service.GetFiles(pattern, Options).ToArray();
            NumberOfFiles = files.Length;
            NumberOfFilesNotify?.Invoke(this, new NumberOfFilesNotifyEventArgs{
                NumberOfFiles = NumberOfFiles
                });

            #if DEBUG4
            foreach (var i in files.OrderBy(i => i.FullName)) {
                status.Value = Max(status.Value, Execute(i, pattern));
                FileCompleted?.Invoke(this, EventArgs.Empty);
                }
            #else
            Parallel.ForEach(files,
                new ParallelOptions{
                    MaxDegreeOfParallelism = MultiThreadOption.NumberOfThreads
                    },
                i=>
                    {
                    status.Value = Max(status.Value, Execute(i, pattern));
                    FileCompleted?.Invoke(this, EventArgs.Empty);
                    });
            #endif
            DirectoryCompleted?.Invoke(this, EventArgs.Empty);
            return status.Value;
            }
        #endregion
        #region M:Execute(IFileService,String):FileOperationStatus
        private FileOperationStatus Execute(IFileService service, String pattern) {
            if (service == null) { throw new ArgumentNullException(nameof(service)); }
            var status = FileOperationStatus.Skip;
            var targetfolder = (TargetFolder ?? Path.GetDirectoryName(service.FileName)) ?? String.Empty;
            var timer = new Stopwatch();
            timer.Start();
            try
                {
                switch (Path.GetExtension(service.FileName).ToLower()) {
                    #region {.ldif}
                    case ".ldif":
                        if (Options.HasFlag(DirectoryServiceSearchOptions.Containers)) {
                            status = Execute(new LDIFFile(service), Pattern);
                            }
                        break;
                    #endregion
                    #region {.ml  }
                    case ".ml":
                        if (Options.HasFlag(DirectoryServiceSearchOptions.Containers)) {
                            using (var msg = new CmsMessage(service.ReadAllBytes())) {
                                var masterList = (CSCAMasterList)msg.ContentInfo.GetService(typeof(CSCAMasterList));
                                if (masterList != null) {
                                    status = Execute(masterList, Pattern);
                                    }
                                }
                            }
                        break;
                    #endregion
                    default:
                        if (Options.HasFlag(DirectoryServiceSearchOptions.Containers)) {
                            var dirreq = new DirectoryServiceRequestEventArgs(service);
                            DirectoryServiceRequest?.Invoke(this, dirreq);
                            if (dirreq.Handled && (dirreq.Service != null)) {
                                status = Execute(dirreq.Service, Pattern);
                                if (status != FileOperationStatus.Skip) {
                                    break;
                                    }
                                }
                            }
                        status = ExecuteAction(service, Options, new FileOperationArgs
                            {
                            TargetFolder = targetfolder,
                            Pattern = Pattern??"*.*"
                            });
                        if (status == FileOperationStatus.Skip) {
                            if (Options.HasFlag(DirectoryServiceSearchOptions.Containers)) {
                                if (DirectoryService.GetService<IDirectoryService>(service, out var folder)) {
                                    status = Execute(folder, Pattern);
                                    }
                                }
                            }
                        break;
                    }
                }
            catch (Exception e)
                {
                e.Add("Service", new
                    {
                    Type = service.GetType().FullName,
                    Self = service
                    });
                status = FileOperationStatus.Error;
                Logger.Log(LogLevel.Warning, e);
                if (StopOnError)
                    {
                    throw;
                    }
                }
            finally
                {
                timer.Stop();
                }
            switch (status) {
                case FileOperationStatus.Success:
                    lock(console) {
                        Write(Out,ConsoleColor.Green, "{ok}");
                        Write(Out,ConsoleColor.Gray, ":");
                        Write(Out,ConsoleColor.Cyan, $"{{{timer.Elapsed}}}");
                        WriteLine(Out,ConsoleColor.Gray, $":{service.FileName}");
                        }
                    break;
                case FileOperationStatus.Warning:
                    lock(console) {
                        Write(Out,ConsoleColor.Yellow, "{skip}");
                        Write(Out,ConsoleColor.Gray, ":");
                        Write(Out,ConsoleColor.Cyan, $"{{{timer.Elapsed}}}");
                        WriteLine(Out,ConsoleColor.Gray, $":{service.FileName}");
                        }
                    break;;
                case FileOperationStatus.Error:
                    lock(console) {
                        Write(Out,ConsoleColor.Red, "{error}");
                        Write(Out,ConsoleColor.Gray, ":");
                        Write(Out,ConsoleColor.Cyan, $"{{{timer.Elapsed}}}");
                        WriteLine(Out,ConsoleColor.Gray, $":{service.FileName}");
                        }
                    break;
                    }
            return status;
            }
        #endregion
        #region M:Max(FileOperationStatus,FileOperationStatus)
        public static FileOperationStatus Max(FileOperationStatus x, FileOperationStatus y) {
            if (x == y) { return x; }
            return (FileOperationStatus)Math.Max(
                (Int32)x,
                (Int32)y);
            }
        #endregion

        public override object GetService(object source, Type service) {
            if (service == typeof(IDirectoryService)) {
                var file = GetService<IFileService>(source);
                if (file != null) {
                    switch (Path.GetExtension(file.FullName).ToLower()) {
                        case ".hexcsv": { return new HexCSVGroupService(file); }
                        }
                    }
                }
            return base.GetService(source, service);
            }
        }
    }