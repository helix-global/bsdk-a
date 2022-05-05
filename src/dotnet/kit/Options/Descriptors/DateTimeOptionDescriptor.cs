using System;
using System.Globalization;
using System.IO;

namespace Options.Descriptors
    {
    internal class DateTimeOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "datetime"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("datetime:")) {
                    DateTime v;
                    var s = source.Substring(9).Trim().ToUpper();
                    try
                        {
                        switch (s)
                            {
                            case "NOW": v = DateTime.Now; break;
                            default:
                                {
                                v = DateTime.ParseExact(s,new []
                                    {
                                    "yyyy-MM-ddTHH:mm:ss.fff",
                                    "yyyy-MM-ddTHH:mm:ss",
                                    "o",
                                    },
                                    CultureInfo.InstalledUICulture,
                                    DateTimeStyles.RoundtripKind);
                                }
                                break;
                            }
                        option = new DateTimeOption(v);
                        return true;
                        }
                    catch (Exception e)
                        {
                        e.Data["Value"]=s;
                        throw;
                        }
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("datetime:{now|yyyy-MM-ddThh:mm:ss}");
            }
        }
    }