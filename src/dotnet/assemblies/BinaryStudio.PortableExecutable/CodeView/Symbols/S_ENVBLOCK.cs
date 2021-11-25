using System;
using System.Collections.Generic;
using System.Text;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_ENVBLOCK)]
    internal class S_ENVBLOCK : CodeViewSymbol
        {
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_ENVBLOCK; }}
        public IDictionary<String, String> Values { get; }
        public unsafe S_ENVBLOCK(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            Values = new Dictionary<String, String>();
            var r = (Byte*)content + 1;
            while (r < (Byte*)content + length - 1) {
                Values[ReadString(Encoding.UTF8, ref r)] = ReadString(Encoding.UTF8, ref r);
                }
            }

        #region M:ReadString(Encoding,[Ref]Byte*):String
        private static unsafe String ReadString(Encoding encoding, ref Byte* source) {
            if (source == null) { return null; }
            var c = 0;
            for (;;++c) {
                if (source[c] == 0) {
                    break;
                    }
                }
            var r = new Byte[c];
            for (var i = 0;i < c;++i) {
                r[i] = source[i];
                }
            source += c + 1;
            return encoding.GetString(r);
            }
        #endregion
        }
    }