using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BinaryStudio.Diagnostics
    {
    public static class Exceptions
        {
        public static T Add<T>(this T e, String key, Object value)
            where T: Exception
            {
            if (value != null) {
                var type = value.GetType();
                if ((value is ISerializable) ||
                    (type.GetCustomAttributes(typeof(SerializableAttribute),true).Any()))
                    {
                    e.Data[key] = value;
                    }
                else
                    {
                    e.Data[key] = new ExceptionObjectDecorator(value);
                    }
                }
            return e;
            }

        public static void WriteTo(Exception source, TextWriter target) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            var index = 0;
            WriteTo(ref index, String.Empty, source, target);
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

        private static String GetHRForException(Exception e) {
            return Marshal.GetHRForException(e).ToString("x8");
            }

        private class Block<T1>
            {
            public T1 Item1 { get;set; }
            public Block(T1 item1)
                {
                Item1 = item1;
                }
            }

        private class Block<T1,T2> : Block<T1>
            {
            public T2 Item2 { get;set; }
            public Block(T1 item1, T2 item2)
                :base(item1)
                {
                Item2 = item2;
                }
            }

        private class ExceptionBlock
            {
            public String Message { get; }
            public Int32? ExceptionIndex { get; }
            public IDictionary<Object,ISet<Object>> ExceptionData { get; }
            public IList<Exception> Exceptions { get; }
            public ExceptionBlock(String message, Int32? index)
                {
                Message = message;
                ExceptionIndex = index;
                ExceptionData = new Dictionary<Object, ISet<Object>>();
                Exceptions = new List<Exception>();
                }

            public void UpdateExceptionData(Object key, Object value) {
                if (!ExceptionData.TryGetValue(key, out var r)) {
                    ExceptionData.Add(key, r = new HashSet<Object>());
                    }
                r.Add(value);
                }
            }

        private static void WriteTo(ref Int32 index, String left, Exception source, TextWriter target) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            AggregateException a = null;
            var messages = new List<Block<String,Int32>>();
            var blocks = new LinkedList<ExceptionBlock>();
            var e = source;
            while (e != null) {
                messages.Add(new Block<String,Int32>($"{{{e.GetType().FullName}}}:{{{GetHRForException(e)}}}:{e.Message?.Trim()}", index));
                a = e as AggregateException;
                if ((a != null) && (a.InnerExceptions.Count < 2)) { a = null; }
                if (e.StackTrace != null) {
                    var lineindex = 0;
                    var lines = e.StackTrace.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    var c = lines.Length;
                    foreach (var line in lines.Reverse()) {
                        var regex = new Regex(@"(\p{Zs}{3}(\p{L}+)\p{Zs}(?<S>.+)\p{Zs}(\p{L}+)\p{Zs}(?<N>.+):(\p{L}+)\p{Zs}(?<L>\p{Nd}+))+");
                        var matches = regex.Matches(line);
                        if (matches.Count > 0) {
                            for (var i = matches.Count - 1; i >= 0; i--) {
                                ExceptionBlock block;
                                var m = matches[i];
                                var s = $"at {m.Groups["S"]} in {Path.GetFileName(m.Groups["N"].Value)}:line {m.Groups["L"]}";
                                blocks.AddFirst(block = (lineindex == c - 1)
                                    ? new ExceptionBlock(s, index)
                                    : new ExceptionBlock(s, null));
                                if (lineindex == 0) {
                                    if (e.Data.Count > 0) {
                                        foreach (DictionaryEntry data in e.Data) {
                                            block.UpdateExceptionData(
                                                data.Key,
                                                data.Value);
                                            }
                                        }
                                    if ((a != null) && (a.InnerExceptions.Count > 0)) {
                                        foreach (var exception in a.InnerExceptions) {
                                            block.Exceptions.Add(exception);
                                            }
                                        }
                                    }
                                }
                            }
                        else
                            {
                            regex = new Regex(@"(\p{Zs}{3}(\p{L}+)\p{Zs}(?<S>.+))");
                            matches = regex.Matches(line);
                            if (matches.Count > 0) {
                                ExceptionBlock block;
                                var s = $"at {matches[0].Groups["S"]}";
                                blocks.AddFirst(block = (lineindex == c - 1)
                                    ? new ExceptionBlock(s, index)
                                    : new ExceptionBlock(s, null));
                                if (lineindex == 0) {
                                    if (e.Data.Count > 0) {
                                        foreach (DictionaryEntry data in e.Data) {
                                            block.UpdateExceptionData(
                                                data.Key,
                                                data.Value);
                                            }
                                        }
                                    if ((a != null) && (a.InnerExceptions.Count > 0)) {
                                        foreach (var exception in a.InnerExceptions) {
                                            block.Exceptions.Add(exception);
                                            }
                                        }
                                    }
                                }
                            }
                        lineindex++;
                        }
                    }
                index++;
                if (a != null)
                    {
                    break;
                    }
                e = e.InnerException;
                }
            var d = (index - 1).ToString().Length;
            var offset = new String(' ', d - 1); 
            target.WriteLine(String.Join("\n", messages.Select(i =>{
                return $"{left} {{{i.Item2.ToString($"D{d}")}}} {i.Item1}";
                })));
            target.WriteLine($"{left}{offset}        # Exception stack trace:");
            foreach (var block in blocks) {
                target.WriteLine((block.ExceptionIndex != null)
                    ? $"{left}    {{{block.ExceptionIndex.Value.ToString($"D{d}")}}} {block.Message}"
                    : $"{left}{offset}        {block.Message}");
                if (block.ExceptionData.Count > 0) {
                    target.WriteLine($"{left}{offset}        # Exception data:");
                    foreach (var data in block.ExceptionData) {
                        target.Write($@"{left}{offset}          ""{data.Key}"":");
                        var j = 0;
                        foreach (var line in Serialize(data.Value)) {
                            if (j > 0) { target.Write($"{left}{offset}          "); }
                            target.WriteLine(line);
                            j++;
                            }
                        }
                    }
                if (block.Exceptions.Count > 0) {
                    target.WriteLine($"{left}        # Inner exceptions {{Count={block.Exceptions.Count}}}:");
                    var j = 1;
                    foreach (var exception in block.Exceptions) {
                        target.WriteLine($"{left}        # Inner exception {{Order={j}}}:");
                        WriteTo(ref index, $"{left}        ", exception, target);
                        j++;
                        }
                    }
                }
            /*
            var messages = new LinkedList<Block>();
            var lines = new List<String>();
            var data = new HashSet<Tuple<String,String>>();
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
                                    "   at {0} in {1}:line {2}", m.Groups["S"],
                                    Path.GetFileName(m.Groups["N"].Value), m.Groups["L"]);
                                if (!block.Contains(s)) { block.AddLast(s); }
                                }
                            }
                        else
                            {
                            regex = new Regex(@"(\p{Zs}{3}(at|в)\p{Zs}(?<S>.+))");
                            matches = regex.Matches(line);
                            if (matches.Count > 0) {
                                var s = String.Format("   at {0}", matches[0].Groups["S"]);
                                if (!block.Contains(s)) { block.AddLast(s); }
                                }
                            }
                        }
                    }
                block.AddFirst($@"   {{{source.GetType()}}}:{{{Marshal.GetHRForException(source).ToString("x8")}}}:""{source.Message.Trim()}""");
                lines.AddRange(block);
                var c = source.Data.Count;
                if (c > 0) {
                    data.UnionWith(source.Data.OfType<DictionaryEntry>().Select(i =>{
                        var value = i.Value;
                        if (value is IExceptionSerializable e) {
                            var j = 0;
                            var b = new StringBuilder();
                            foreach (var line in Serialize(e)) {
                                if (j > 0) { b.Append($"   "); }
                                b.AppendLine(line);
                                j++;
                                }
                            value = b;
                            }
                        else
                            {
                            value = $@"""{value}""";
                            }
                        return Tuple.Create(i.Key.ToString(), value.ToString());
                        }));
                    }
                if (source is AggregateException) { break; }
                source = source.InnerException;
                if (source != null)
                    {
                    lines.Add($"   --- Начало трассировки внутреннего стека исключений ---");
                    }
                }
            #endregion
            foreach (var i in messages) { target.WriteLine($@"   {{{i.Type}}}:{{{i.Scode.ToString("x8")}}}:""{i.Message}"""); }
            target.WriteLine($"   --- Начало трассировки стека исключений ---");
            foreach (var i in lines) { target.WriteLine(i); }
            if (source is AggregateException exception) {
                WriteTo(exception, target);
                }
            if (data.Count > 0) {
                target.WriteLine($"   --- Дополнительная информация ---");
                foreach (var i in data) {
                    target.WriteLine($"   {i.Item1}={i.Item2}");
                    }
                }
            */
            }

        private static IEnumerable<String> Serialize(Object source) {
                 if (source == null) { yield return "null"; }
            else if (source is IExceptionSerializable e) {
                foreach (var i in Serialize(e)) {
                    yield return i;
                    }
                }
            else
                {
                var type = source.GetType();
                if ((type == typeof(Byte))  || (type == typeof(SByte))  || (type == typeof(Decimal)) ||
                    (type == typeof(Int16)) || (type == typeof(UInt16)) || (type == typeof(Double))  ||
                    (type == typeof(Int32)) || (type == typeof(UInt32)) || (type == typeof(Single))  ||
                    (type == typeof(Int64)) || (type == typeof(UInt64)))
                    {
                    yield return source.ToString();
                    }
                else if (type == typeof(String)) { yield return $@"""{source}"""; }
                else if (source is IEnumerable values) {
                    foreach (var i in Serialize(values)) {
                        yield return i;
                        }
                    }
                else
                    {
                    yield return $@"""{source}""";
                    }
                }
            }

        private static IEnumerable<String> Serialize(IExceptionSerializable source) {
            var target = new StringBuilder();
            using (var writer = new StringWriter(target)) { source.WriteTo(writer); }
            using (var reader = new StringReader(target.ToString())) {
                while (true)
                    {
                    var r = reader.ReadLine();
                    if (r == null) { break; }
                    yield return r;
                    }
                }
            }

        private static IEnumerable<String> Serialize(IEnumerable source) {
            var values = source.OfType<Object>().ToArray();
            if (values.Length == 1) {
                foreach (var i in Serialize(values[0])) {
                    yield return i;
                    }
                yield break;
                }
            var target = new StringBuilder();
            using (var writer = new StringWriter(target)) { WriteTo(writer, source); }
            using (var reader = new StringReader(target.ToString())) {
                while (true)
                    {
                    var r = reader.ReadLine();
                    if (r == null) { break; }
                    yield return r;
                    }
                }
            }

        private static void WriteTo(TextWriter target, IEnumerable source) {
            using (var writer = new JsonTextWriter(target){
                    Formatting = Formatting.Indented,
                    Indentation = 2,
                    IndentChar = ' '
                    }) {
                var serializer = new JsonSerializer();
                writer.WriteStartArray();
                foreach (var value in source) {
                    if (value is IExceptionSerializable S)
                        {
                        S.WriteTo(writer, serializer);
                        }
                    else
                        {
                        writer.WriteValue(value);
                        }
                    }
                writer.WriteEnd();
                writer.Flush();
                }
            }
        }
    }