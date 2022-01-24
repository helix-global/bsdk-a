using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BinaryStudio.DirectoryServices;
using BinaryStudio.Security.Cryptography.CryptographicMessageSyntax;
using BinaryStudio.Security.Cryptography.DataInterchangeFormat;
using Options;

namespace Operations
    {
    internal class FileOperation<T> : Operation
        where T: FileOperationArgs, new()
        {
        public String TargetFolder { get;set; }
        public DirectoryServiceSearchOptions Options { get;set; }
        public String Pattern { get;set; }
        public Func<IFileService,DirectoryServiceSearchOptions,T,FileOperationStatus> ExecuteAction { get;set; }
        private readonly Object console = new Object();

        public FileOperation(TextWriter output, TextWriter error)
            :base(output, error, new OperationOption[0])
            {
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
                    if (String.IsNullOrEmpty(folder)) { folder = ".\\"; }
                    status = Max(status, Execute(
                        DirectoryService.GetService<IDirectoryService>(new Uri($"file://{folder}")),
                        pattern));
                    }
                status = Max(status, ExecuteAction(
                    DirectoryService.GetService<IFileService>(new Uri($"file://{file}")),
                    Options, new T
                        {
                        TargetFolder = TargetFolder,
                        Pattern = Pattern??"*.*"
                        }));
                }
            return status;
            }
        #endregion
        #region M:Execute(IDirectoryService,String):FileOperationStatus
        private FileOperationStatus Execute(IDirectoryService service, String pattern) {
            if (service == null) { throw new ArgumentNullException(nameof(service)); }
            var status = FileOperationStatus.Skip;
            foreach (var i in service.GetFiles(pattern, Options)) {
                status = Max(status, Execute(i, pattern));
                }
            return status;
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
                    case ".ldif":
                        {
                        if (Options.HasFlag(DirectoryServiceSearchOptions.Containers)) {
                            status = Execute(new LDIFFile(service), Pattern);
                            }
                        }
                        break;
                    case ".ml":
                        {
                        if (Options.HasFlag(DirectoryServiceSearchOptions.Containers)) {
                            using (var msg = new CmsMessage(service.ReadAllBytes())) {
                                var masterList = (CSCAMasterList)msg.ContentInfo.GetService(typeof(CSCAMasterList));
                                if (masterList != null) {
                                    status = Execute(masterList, Pattern);
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        status = ExecuteAction(service, Options, new T
                            {
                            TargetFolder = TargetFolder,
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
            catch
                {
                status = FileOperationStatus.Error;
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
        private static FileOperationStatus Max(FileOperationStatus x, FileOperationStatus y) {
            if (x == y) { return x; }
            return (FileOperationStatus)Math.Max(
                (Int32)x,
                (Int32)y);
            }
        #endregion
        }
    }