using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Options.Descriptors
    {
    internal class OutputFileOrFolderOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "output"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("output:")) {
                    var values = new List<String>();
                    var src = source.Substring(7);
                    var value = new StringBuilder();
                    var flags = 0;
                    for (var i = 0; i < src.Length; i++) {
                        var c = src[i];
                        switch (c) {
                            case '{':
                                {
                                if (flags != 0) { throw new ArgumentOutOfRangeException(nameof(source)); }
                                flags = 1;
                                }
                                break;
                            case '}':
                                {
                                if (flags == 0) { throw new ArgumentOutOfRangeException(nameof(source)); }
                                flags = 0;
                                values.Add(value.ToString());
                                value.Clear();
                                }
                                break;
                            case ';':
                                {
                                if (flags == 1) {
                                    value.Append(c);
                                    break;
                                    }
                                if (value.Length > 0) {
                                    values.Add(value.ToString());
                                    value.Clear();
                                    }
                                }
                                break;
                            default:
                                {
                                value.Append(c);
                                }
                                break;
                            }
                        }
                    if (value.Length > 0) {
                        values.Add(value.ToString());
                        value.Clear();
                        }
                    option = new OutputFileOrFolderOption(values);
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("output:[[file_or_folder][;[file_or_folder]]+]");
            }
        }
    }