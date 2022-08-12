using System;
using System.IO;

namespace Options.Descriptors
    {
    internal class ReportOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "report"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("report:")) {
                    option = new ReportOption(Split(source.Substring(7)));
                    return true;
                    }
                if (source == "report")
                    {
                    option = new ReportOption();
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("report:{prefix={prefix-name};{name={report-name}}");
            }
        }
    }