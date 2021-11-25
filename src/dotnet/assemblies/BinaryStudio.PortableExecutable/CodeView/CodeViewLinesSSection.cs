using BinaryStudio.PortableExecutable.Win32;
using BinaryStudio.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewLinesSSection : CodeViewPrimarySSection
        {
        private const UInt16 CV_LINES_HAVE_COLUMNS = 0x0001;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct HEADER
            {
            public readonly UInt32 offCon;
            public readonly UInt16 segCon;
            public readonly UInt16 flags;
            public readonly UInt32 cbCon;
            }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct FILEBLOCK
            {
            public readonly UInt32 fileid;
            public readonly Int32 nLines;
            public readonly Int32 cbFileBlock;
            }

        public override DEBUG_S Type { get { return DEBUG_S.DEBUG_S_LINES; }}
        public Boolean HasColumns { get; }
        public UInt32 offCon { get; }
        public UInt16 segCon { get; }
        public UInt32 cbCon { get; }
        public UInt16 flags { get; }
        public IList<CodeViewLinesSSectionFileBlock> Blocks { get; }

        internal unsafe CodeViewLinesSSection(CodeViewSection section, Int32 offset, Byte* content, Int32 length)
            : base(section, offset, content, length)
            {
            var r = content;
            #if DEBUG
            Debug.Assert(length >= sizeof(HEADER));
            #endif
            if (length < sizeof(HEADER)) { throw new ArgumentOutOfRangeException(nameof(length)); }
            var blocks = new List<CodeViewLinesSSectionFileBlock>();
            var H = (HEADER*)r;
            var LA = r + length;
            var hascolumns = HasColumns = ((H->flags & CV_LINES_HAVE_COLUMNS) == CV_LINES_HAVE_COLUMNS);
            offCon = H->offCon;
            segCon = H->segCon;
            cbCon = H->cbCon;
            flags = H->flags;
            r += sizeof(HEADER);
            #if DEBUG
            length -= sizeof(HEADER);
            #endif
            while (r < LA) {
                var block = (FILEBLOCK*)r;
                var B = new CodeViewLinesSSectionFileBlock(block->fileid); 
                r += sizeof(FILEBLOCK);
                blocks.Add(B);
                if (block->nLines > 0) {
                    var LI = (CV_Line_t*)r;
                    var CO = hascolumns
                        ? (CV_Column_t*)(r + sizeof(CV_Line_t)*block->nLines)
                        : null;
                    for (var i = 0; i < block->nLines; i++) {
                        B.Add(LI, H->offCon);
                        LI++;
                        if (hascolumns)
                            {
                            B.Add(CO);
                            CO++;
                            }
                        }
                    }
                r += block->cbFileBlock - sizeof(FILEBLOCK);
                #if DEBUG
                length -= sizeof(FILEBLOCK);
                length -= block->cbFileBlock - sizeof(FILEBLOCK);
                #endif
                }
            #if DEBUG
            Debug.Assert(length == 0);
            #endif
            Blocks = blocks.ToArray();
            }

        protected internal override unsafe void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            return;
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            using (writer.ObjectScope(serializer)) {
                writer.WriteValue(serializer, nameof(Type), Type.ToString());
                writer.WriteValue(serializer, nameof(Offset), Offset.ToString("X6"));
                writer.WriteValue(serializer, nameof(Length), Length.ToString("X6"));
                writer.WriteValue(serializer, nameof(HasColumns), HasColumns);
                writer.WriteValue(serializer, "{Self}", $"{segCon:X4}:{offCon:X8}-{offCon + cbCon:X8}");
                writer.WriteValue(serializer, nameof(flags), flags.ToString("X4"));
                if (Blocks.Count > 0) {
                    var T = stackalloc[] { 0, 0, 0, 0, 0, 0};
                    writer.WritePropertyName(nameof(Blocks));
                    using (writer.ObjectScope(serializer)) {
                        writer.WriteValue(serializer, nameof(Blocks.Count), Blocks.Count);
                        writer.WritePropertyName("[Self]");
                        using (writer.ArrayScope(serializer)) {
                            foreach (var block in Blocks) {
                                using (writer.ObjectScope(serializer)) {
                                    writer.WriteValue(serializer, nameof(block.Identifier), block.Identifier.ToString("X8"));
                                    if (block.Lines.Count > 0) {
                                        writer.WritePropertyName(nameof(block.Lines));
                                        using (writer.ArrayScope(serializer)) {
                                            T[1] = 0;
                                            T[2] = 0;
                                            var firstitem = true;
                                            foreach (var I in block.Lines) {
                                                T[1] = Math.Max(T[1], I.IsSpecialLine ? 0 : 1);
                                                if (I.LineStartNumber.HasValue) {
                                                    T[2] = Math.Max(T[2], ((I.LineEndDelta > 0)
                                                        ? $"{I.LineStartNumber:D6}-{I.LineStartNumber + I.LineEndDelta:D6}"
                                                        : $"{I.LineStartNumber:D6}").Length);
                                                    }
                                                }
                                            using (writer.SuspendFormatingScope()) {
                                                foreach (var I in block.Lines) {
                                                    ValidateFirstItem(writer, ref firstitem);
                                                    using (writer.ObjectScope(serializer)) {
                                                        writer.Formatting = Formatting.None;
                                                        var lineno = (I.LineStartNumber.HasValue)
                                                            ? (I.LineEndDelta > 0)
                                                                ? $"{I.LineStartNumber:D6}-{I.LineStartNumber + I.LineEndDelta:D6}"
                                                                : $"{I.LineStartNumber:D6}"
                                                            : null;
                                                        writer.WriteValue(serializer, nameof(I.Offset), I.Offset.ToString("X8"));
                                                        writer.WriteValue(serializer, nameof(I.IsSpecialLine), I.IsSpecialLine, T[1] - (I.IsSpecialLine ? 0 : 1));
                                                        if (lineno != null)
                                                            {
                                                            writer.WriteValue(serializer, "Line", lineno, T[2] - lineno.Length);
                                                            }
                                                        else
                                                            {
                                                            //writer.WriteIndentSpace(T[2] + 10);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }