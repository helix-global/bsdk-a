using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Options.Descriptors
    {
    internal class InputCertificateOptionDescriptor : OptionDescriptor
        {
        public override String OptionName { get { return "certificate"; }}
        public override Boolean TryParse(String source, out OperationOption option)
            {
            option = null;
            if (!String.IsNullOrWhiteSpace(source)) {
                source = source.Trim();
                if (source.StartsWith("certificate:")) {
                    var value = source.Substring(12);
                    if (String.IsNullOrWhiteSpace(value)) { throw new ArgumentOutOfRangeException(nameof(source)); }
                    option = new InputCertificateOption();
                    foreach (var e in value.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries)) {
                        var i = e.Trim();
                        if (Regex.IsMatch(i, @"[0-9a-fA-F]+")) {
                            ((InputCertificateOption)option).Certificates.Add(new InputCertificate {
                                Thumbprint = i.ToUpper()
                                });
                            }
                        }
                    if (((InputCertificateOption)option).Certificates.Count == 0) { throw new ArgumentOutOfRangeException(nameof(source)); }
                    return true;
                    }
                }
            return false;
            }

        public override void Usage(TextWriter output)
            {
            output.Write("certificate:[certificate]+");
            }
        }
    }