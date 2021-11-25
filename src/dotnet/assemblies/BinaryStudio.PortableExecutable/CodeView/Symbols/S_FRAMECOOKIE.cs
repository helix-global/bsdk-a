using System;
using BinaryStudio.PortableExecutable.Win32;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    [CodeViewSymbol(DEBUG_SYMBOL_INDEX.S_FRAMECOOKIE)]
    internal class S_FRAMECOOKIE : CodeViewSymbol
        {
        public override DEBUG_SYMBOL_INDEX Type { get { return DEBUG_SYMBOL_INDEX.S_FRAMECOOKIE; }}
        public UInt32 off { get; }
        public UInt16 reg { get; }
        public CV_COOKIETYPE cookietype { get; }
        public Byte flags { get; }
        public unsafe S_FRAMECOOKIE(CodeViewSymbolsSSection section, Int32 offset, IntPtr content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = (FRAMECOOKIE*)content;
            off = r->off;
            reg = r->reg;
            cookietype = r->cookietype;
            flags = r->flags;
            }
        }
    }