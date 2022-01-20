using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Kit
    {
    internal class Utilities
        {
        #region M:Hex(Byte[],TextWriter):String
        public static void Hex(Byte[] source, TextWriter writer) {
            if (source == null) {
                writer.WriteLine("(none)");
                return;
                }
            var i = 0;
            var sz = source.Length;
            var f = new List<Byte>();
            while (i < sz)
            {
                f.Clear();
                writer.Write("{0:X8}: ", i);
                var j = 0;
                for (; (j < 8) && (i < sz); ++j, ++i)
                {
                    writer.Write("{0:X2} ", source[i]);
                    f.Add(source[i]);
                }
                if (i < sz)
                {
                    writer.Write("| ");
                    j = 0;
                    for (; (j < 8) && (i < sz); ++j, ++i)
                    {
                        writer.Write("{0:X2} ", source[i]);
                        f.Add(source[i]);
                    }
                    if (i >= sz)
                    {
                        writer.Write(new String(' ', (8 - j) * 3));
                    }
                }
                else
                {
                    writer.Write(new String(' ', (16 - j) * 3 + 2));
                }
                foreach (var c in f)
                {
                    if (((c >= 'A') && (c <= 'Z')) ||
                        ((c >= 'a') && (c <= 'z')) ||
                        ((c >= '0') && (c <= '9')) ||
                        ((c >= 0x20) && (c <= 0x7E)))
                    {
                        writer.Write((char)c);
                    }
                    else
                    {
                        writer.Write('.');
                    }
                }
                writer.WriteLine();
            }
        }
        #endregion

        private class ExceptionItem
        {
            public String Message { get; private set; }
            public Type Type { get; private set; }
            public ExceptionItem(String message, Type type)
            {
                Message = message;
                Type = type;
            }
        }

        #region M:FormatException(ExceptionDetail):String
#if FEATURE_EXCEPTION_DETAIL
        public static String FormatException(ExceptionDetail e)
        {
            if (e == null) { return null; }
            var messages = new LinkedList<ExceptionItem>();
            var lines = new LinkedList<String>();
        #region Formatting exception output
            while (e != null)
            {
                messages.AddFirst(new ExceptionItem(e.Message, e.GetType()));
                if (e.StackTrace != null)
                {
                    foreach (var line in e.StackTrace.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var regex = new Regex(@"(\p{Zs}{3}(at|в)\p{Zs}(?<S>.+)\p{Zs}(in|в)\p{Zs}(?<N>.+):(line|строка)\p{Zs}(?<L>\p{Nd}+))+");
                        var matches = regex.Matches(line);
                        if (matches.Count > 0)
                        {
                            for (var i = matches.Count - 1; i >= 0; i--)
                            {
                                var m = matches[i];
                                var s = String.Format(
                                    "   at {0} in {1}:line {2}", m.Groups["S"],
                                    Path.GetFileName(m.Groups["N"].Value), m.Groups["L"]);
                                if (!lines.Contains(s)) { lines.AddFirst(s); }
                            }
                        }
                        else
                        {
                            regex = new Regex(@"(\p{Zs}{3}(at|в)\p{Zs}(?<S>.+))");
                            matches = regex.Matches(line);
                            if (matches.Count > 0)
                            {
                                var s = String.Format("   at {0}", matches[0].Groups["S"]);
                                if (!lines.Contains(s)) { lines.AddFirst(s); }
                            }
                        }
                    }
                }
                e = e.InnerException;
            }
        #endregion
            return String.Format("\r\nHandled Exception: \r\n{0}\r\n{1}\r\n",
                String.Join("\r\n", messages.Select(i =>
                    String.Format("   [{0}]: {1}", i.Second.FullName, i.First)).
                    ToArray()),
                String.Join("\r\n", lines.ToArray()));
        }
#endif
        #endregion
        #region M:FormatException(Exception):String
        public static String FormatException(Exception e)
        {
            if (e == null) { return null; }
            var messages = new LinkedList<ExceptionItem>();
            var lines = new List<String>();
            var alt = String.Empty;
#if FEATURE_EXCEPTION_DETAIL
            if (e is FaultException)
            {
                var pi = e.GetType().GetProperty("Detail");
                if (pi != null)
                {
                    var detail = pi.GetValue(e, null) as ExceptionDetail;
                    if (detail != null)
                    {
                        alt = FormatException(detail);
                    }
                }
            }
#endif
            #region Formatting exception output
            while (e != null)
            {
                var block = new LinkedList<String>();
                messages.AddFirst(new ExceptionItem(e.Message, e.GetType()));
                if (e.StackTrace != null)
                {
                    foreach (var line in e.StackTrace.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var regex = new Regex(@"(\p{Zs}{3}(at|в)\p{Zs}(?<S>.+)\p{Zs}(in|в)\p{Zs}(?<N>.+):(line|строка)\p{Zs}(?<L>\p{Nd}+))+");
                        var matches = regex.Matches(line);
                        if (matches.Count > 0)
                        {
                            for (var i = matches.Count - 1; i >= 0; i--)
                            {
                                var m = matches[i];
                                var s = String.Format(
                                    "   at {0} in {1}:line {2}", m.Groups["S"],
                                    Path.GetFileName(m.Groups["N"].Value), m.Groups["L"]);
                                if (!block.Contains(s)) { block.AddLast(s); }
                            }
                        }
                        else
                        {
                            regex = new Regex(@"(\p{Zs}{3}(at|в)\p{Zs}(?<S>.+))");
                            matches = regex.Matches(line);
                            if (matches.Count > 0)
                            {
                                var s = String.Format("   at {0}", matches[0].Groups["S"]);
                                if (!block.Contains(s)) { block.AddLast(s); }
                            }
                        }
                    }
                }
                block.AddFirst(String.Format(@"   [{1}]:""{0}""", e.Message, e.GetType()));
                lines.AddRange(block);
                var c = e.Data.Count;
                if (c > 0)
                {
                    lines.Add("Дополнительная информация:");
                    lines.AddRange(e.Data.OfType<DictionaryEntry>().Select(i => String.Format("{0}=\"{1}\"", i.Key, i.Value)));
                }
                e = e.InnerException;
                if (e != null)
                {
                    lines.Add("   --- Начало трассировки внутреннего стека исключений ---");
                }
            }
            #endregion
            var r = new StringBuilder(
                String.Format("Обнаружено исключение: \r\n{0}\r\n   --- Начало трассировки стека исключений ---\r\n{1}\r\n",
                    String.Join("\r\n", messages.Select(i =>
                        String.Format(@"   [{0}]: ""{1}""", i.Type.FullName, i.Message)).
                        ToArray()),
                    String.Join("\r\n", lines.ToArray())));
            if (!String.IsNullOrWhiteSpace(alt))
            {
                r.AppendFormat("Дополнительная информация:\r\n{0}", alt);
            }
            return r.ToString();
        }
        #endregion
        }
    }