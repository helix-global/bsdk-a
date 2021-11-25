using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BinaryStudio.Diagnostics
    {
    public class ExceptionFormatter
        {
        public void Write(TextWriter writer, Exception e)
            {
            WriteInternal(0, writer, e);
            }

        internal void WriteInternal(Int32 indent, TextWriter writer, AggregateException e) {
            if (writer != null) {
                if (e.InnerExceptions.Count > 0) {
                    writer.WriteLine($"{new String(' ', indent)}   --- Начало трассировки агрегированных исключений (Count = {e.InnerExceptions.Count}) --- ");
                    var indentstr = new String(' ', indent + 2);
                    foreach (var i in e.InnerExceptions) {
                        writer.WriteLine($"{indentstr}   {{");
                        WriteInternal(indent + 2, writer, i);
                        writer.WriteLine($"{indentstr}   }}");
                        }
                    }
                }
            }

        internal void WriteInternal(Int32 indent, TextWriter writer, Exception e) {
            if (writer != null) {
                var identstr = new String(' ', indent);
                var messages = new LinkedList<Tuple<String,String>>();
                var lines = new List<String>();
                var data = new HashSet<Tuple<String,Object>>();
                #region Formatting exception output
                while (e != null)
                    {
                    var block = new LinkedList<String>();
                    messages.AddFirst(Tuple.Create(e.Message.Trim(), e.GetType().FullName));
                    if (e.StackTrace != null) {
                        foreach (var line in e.StackTrace.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)) {
                            var regex = new Regex(@"(\p{Zs}{3}(at|в)\p{Zs}(?<S>.+)\p{Zs}(in|в)\p{Zs}(?<N>.+):(line|строка)\p{Zs}(?<L>\p{Nd}+))+");
                            var matches = regex.Matches(line);
                            if (matches.Count > 0) {
                                for (var i = matches.Count - 1; i >= 0; i--) {
                                    var m = matches[i];
                                    var s = String.Format(
                                        "{3}   at {0} in {1}:line {2}", m.Groups["S"],
                                        Path.GetFileName(m.Groups["N"].Value), m.Groups["L"], identstr);
                                    if (!block.Contains(s)) { block.AddLast(s); }
                                    }
                                }
                            else
                                {
                                regex = new Regex(@"(\p{Zs}{3}(at|в)\p{Zs}(?<S>.+))");
                                matches = regex.Matches(line);
                                if (matches.Count > 0) {
                                    var s = String.Format("{1}   at {0}", matches[0].Groups["S"], identstr);
                                    if (!block.Contains(s)) { block.AddLast(s); }
                                    }
                                }
                            }
                        }
                    block.AddFirst(String.Format(@"{2}   [{1}]:""{0}""", e.Message.Trim(), e.GetType(), identstr));
                    lines.AddRange(block);
                    var c = e.Data.Count;
                    if (c > 0) {
                        data.UnionWith(e.Data.OfType<DictionaryEntry>().Select(i => Tuple.Create(i.Key.ToString(), i.Value)));
                        }
                    if (e is AggregateException) { break; }
                    e = e.InnerException;
                    if (e != null)
                        {
                        lines.Add($"{identstr}   --- Начало трассировки внутреннего стека исключений ---");
                        }
                    }
                #endregion
                foreach (var i in messages) { writer.WriteLine(@"{2}   [{0}]: ""{1}""", i.Item1, i.Item2, identstr); }
                writer.WriteLine($"{identstr}   --- Начало трассировки стека исключений ---");
                foreach (var i in lines) { writer.WriteLine(i); }
                if (e is AggregateException exception) {
                    WriteInternal(indent, writer, exception);
                    }
                if (data.Count > 0) {
                    writer.WriteLine($"{identstr}   --- Дополнительная информация ---");
                    foreach (var i in data) {
                        writer.WriteLine($"{identstr}   {i.Item1}=\"{i.Item2}\"");
                        }
                    }
                }
            }
        }
    }