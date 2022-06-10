using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryStudio.Security.Cryptography.Services.Reporting;
using Options;

namespace Operations
    {
    internal class ReportOperation : Operation
        {
        public IList<String> InputFileName { get; }
        private String TargetFolder { get; }
        private String ReportName { get; }
        private ReportOption ReportOption { get; }

        public ReportOperation(TextWriter output, TextWriter error, IList<OperationOption> args) 
            : base(output, error, args)
            {
            InputFileName = args.OfType<InputFileOrFolderOption>().FirstOrDefault()?.Values;
            TargetFolder  = args.OfType<OutputFileOrFolderOption>().FirstOrDefault()?.Values?.FirstOrDefault();
            ReportOption  = args.OfType<ReportOption>().FirstOrDefault();
            ReportName = ReportOption?.ReportName??String.Empty;
            }

        public override void Execute(TextWriter output) {
            switch (ReportName.ToLowerInvariant()) {
                case "chain-report":
                    {
                    var report = new ChainReport(InputFileName.FirstOrDefault(), TargetFolder);
                    report.BuildReport();
                    }
                break;
                }
            }
        }
    }