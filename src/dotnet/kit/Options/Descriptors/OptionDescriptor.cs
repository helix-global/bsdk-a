using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Options.Descriptors
    {
    internal abstract class OptionDescriptor
        {
        public abstract String OptionName { get; }
        public abstract Boolean TryParse(String source, out OperationOption option);
        public abstract void Usage(TextWriter output);

        protected static IList<String> Split(String value) {
            var r = new List<String>();
            var o = new StringBuilder();
            var flags = 0;
            for (var i = 0; i < value.Length; i++) {
                var c = value[i];
                switch (c) {
                    case '{':
                        {
                        if (flags != 0) { throw new ArgumentOutOfRangeException(nameof(value)); }
                        flags = 1;
                        }
                        break;
                    case '}':
                        {
                        if (flags == 0) { throw new ArgumentOutOfRangeException(nameof(value)); }
                        flags = 0;
                        r.Add(o.ToString());
                        o.Clear();
                        }
                        break;
                    case ';':
                        {
                        if (flags == 1) {
                            o.Append(c);
                            break;
                            }
                        if (o.Length > 0) {
                            r.Add(o.ToString());
                            o.Clear();
                            }
                        }
                        break;
                    default:
                        {
                        o.Append(c);
                        }
                        break;
                    }
                }
            if (o.Length > 0) {
                r.Add(o.ToString());
                o.Clear();
                }
            return r;
            }
        }
    }