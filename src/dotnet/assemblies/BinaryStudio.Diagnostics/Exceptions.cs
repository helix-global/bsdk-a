using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace BinaryStudio.Diagnostics
    {
    public static class Exceptions
        {
        public static void WriteTo(Exception source, TextWriter target) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            using (var writer = new InternalTextWriter(target)) {
                WriteTo(source, (IColorTextWriter)writer);
                }
            }

        public static void WriteTo(Exception source, IColorTextWriter target) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            WriteTo(0, source, target);
            }

        public static void WriteTo(Exception source, StringBuilder target) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            using (var writer = new StringWriter(target)) {
                WriteTo(source, writer);
                }
            }

        public static String ToString(Exception source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var r = new StringBuilder();
            WriteTo(source, r);
            return r.ToString();
            }

        private class Block
            {
            public readonly String Message;
            public readonly String Type;
            public readonly Int32 Scode;
            public Block(String message, String type, Int32 scode)
                {
                Message = message;
                Type = type;
                Scode = scode;
                }
            public Block(String message, Type type, Int32 scode)
                {
                Message = message;
                Type = type.FullName;
                Scode = scode;
                }
            }


        private static void WriteTo(Int32 indent, AggregateException e, IColorTextWriter writer) {
            if (writer != null) {
                if (e.InnerExceptions.Count > 0) {
                    writer.WriteLine($"{new String(' ', indent)}   --- Начало трассировки агрегированных исключений (Count = {e.InnerExceptions.Count}) --- ");
                    var indentstr = new String(' ', indent + 2);
                    foreach (var i in e.InnerExceptions) {
                        writer.WriteLine($"{indentstr}   {{");
                        WriteTo(indent + 2, i, writer);
                        writer.WriteLine($"{indentstr}   }}");
                        }
                    }
                }
            }

        private static void WriteTo(Int32 indent, Exception source, IColorTextWriter target)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            var identstr = new String(' ', indent);
            var messages = new LinkedList<Block>();
            var lines = new List<String>();
            var data = new HashSet<Tuple<String,Object>>();
            #region Formatting exception output
            while (source != null)
                {
                var block = new LinkedList<String>();
                messages.AddFirst(new Block(source.Message.Trim(), source.GetType(), Marshal.GetHRForException(source)));
                if (source.StackTrace != null) {
                    foreach (var line in source.StackTrace.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)) {
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
                block.AddFirst($@"{identstr}   {{{source.GetType()}}}:{{{Marshal.GetHRForException(source).ToString("x8")}}}:""{source.Message.Trim()}""");
                lines.AddRange(block);
                var c = source.Data.Count;
                if (c > 0) {
                    data.UnionWith(source.Data.OfType<DictionaryEntry>().Select(i => Tuple.Create(i.Key.ToString(), i.Value)));
                    }
                if (source is AggregateException) { break; }
                source = source.InnerException;
                if (source != null)
                    {
                    lines.Add($"{identstr}   --- Начало трассировки внутреннего стека исключений ---");
                    }
                }
            #endregion
            foreach (var i in messages) { target.WriteLine($@"{identstr}   {{{i.Type}}}:{{{i.Scode.ToString("x8")}}}:""{i.Message}"""); }
            target.WriteLine($"{identstr}   --- Начало трассировки стека исключений ---");
            foreach (var i in lines) { target.WriteLine(i); }
            if (source is AggregateException exception) {
                WriteTo(indent, exception, target);
                }
            if (data.Count > 0) {
                target.WriteLine($"{identstr}   --- Дополнительная информация ---");
                foreach (var i in data) {
                    target.WriteLine($"{identstr}   {i.Item1}=\"{i.Item2}\"");
                    }
                }
            }

        private class InternalTextWriter : TextWriter, IColorTextWriter
            {
            public override Encoding Encoding { get; }
            public InternalTextWriter(TextWriter source)
                {
                if (source == null) { throw new ArgumentNullException(nameof(source)); }
                Encoding = source.Encoding;
                this.source = source;
                }

            /// <summary>Writes a line terminator to the text string or stream.</summary>
            /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed.</exception>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
            public override void WriteLine()
                {
                source.WriteLine();
                }

            /// <summary>Writes a string followed by a line terminator to the text string or stream.</summary>
            /// <param name="value">The string to write. If <paramref name="value"/> is <see langword="null"/>, only the line terminator is written.</param>
            /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed.</exception>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
            public override void WriteLine(String value)
                {
                source.WriteLine(value);
                }

            /// <summary>Writes out a formatted string and a new line, using the same semantics as <see cref="M:System.String.Format(System.String,System.Object)" />.</summary>
            /// <param name="format">A composite format string.</param>
            /// <param name="args">An object array that contains zero or more objects to format and write.</param>
            /// <exception cref="T:System.ArgumentNullException">A string or object is passed in as <see langword="null"/>.</exception>
            /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed.</exception>
            /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
            /// <exception cref="T:System.FormatException"><paramref name="format"/> is not a valid composite format string. -or-
            /// The index of a format item is less than 0 (zero), or greater than or equal to the length of the <paramref name="args"/> array.</exception>
            public override void WriteLine(String format, params Object[] args)
                {
                source.WriteLine(format, args);
                }

            private readonly TextWriter source;
            }
        }
    }