using BinaryStudio.PortableExecutable.Win32;
using System;
using System.Collections.Generic;
using BinaryStudio.Serialization;
using Newtonsoft.Json;

namespace BinaryStudio.PortableExecutable.CodeView
    {
    public class CodeViewFrameDataSSection : CodeViewPrimarySSection
        {
        public override DEBUG_S Type { get { return DEBUG_S.DEBUG_S_FRAMEDATA; }}
        public IList<CodeViewFrameData> Frames { get; }
        internal unsafe CodeViewFrameDataSSection(CodeViewSection section, Int32 offset, Byte* content, Int32 length)
            : base(section, offset, content, length)
            {
            Frames = new List<CodeViewFrameData>();
            var F = content;
            var L = content + length;
            ReadUInt32(ref F);
            #if DEBUG
            length -= sizeof(UInt32);
            #endif
            while (F < L) {
                var H = (FRAMEDATA*)F;
                Frames.Add(new CodeViewFrameData(H));
                F += sizeof(FRAMEDATA);
                #if DEBUG
                length -= sizeof(FRAMEDATA);
                #endif
                }
            }

        protected internal override unsafe void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            using (writer.ObjectScope(serializer)) {
                writer.WriteValue(serializer, nameof(Offset), Offset.ToString("X6"));
                writer.WriteValue(serializer, nameof(Length), Length.ToString("X6"));
                writer.WriteValue(serializer, nameof(Type), Type.ToString());
                if (Frames.Count > 0) {
                    var T = stackalloc[] { 0, 0, 0, 0, 0, 0};
                    writer.WritePropertyName(nameof(Frames));
                    using (writer.ObjectScope(serializer)) {
                        writer.WriteValue(serializer, nameof(Frames.Count), Frames.Count);
                        writer.WritePropertyName("[Self]");
                        using (writer.ArrayScope(serializer)) {
                            using (writer.SuspendFormatingScope()) {
                                var firstitem = true;
                                T[0] = 0;
                                foreach (var I in Frames) {
                                    T[0] = Math.Max(T[0], I.flags.ToString().Length);
                                    }
                                foreach (var I in Frames) {
                                    ValidateFirstItem(writer, ref firstitem);
                                    using (writer.ObjectScope(serializer)) {
                                        var flags = I.flags.ToString();
                                        writer.Formatting = Formatting.None;
                                        writer.WriteValue(serializer, nameof(I.ulRvaStart), I.ulRvaStart.ToString("X8"));
                                        writer.WriteValue(serializer, nameof(I.cbBlock), I.cbBlock.ToString("X8"));
                                        writer.WriteValue(serializer, nameof(I.cbLocals), I.cbLocals.ToString("X8"));
                                        writer.WriteValue(serializer, nameof(I.cbParams), I.cbParams.ToString("X8"));
                                        writer.WriteValue(serializer, nameof(I.cbStkMax), I.cbStkMax.ToString("X8"));
                                        writer.WriteValue(serializer, nameof(I.cbProlog), I.cbProlog.ToString("X8"));
                                        writer.WriteValue(serializer, nameof(I.cbSavedRegs), I.cbSavedRegs.ToString("X8"));
                                        writer.WriteValue(serializer, nameof(I.flags), I.flags.ToString(), T[0] - flags.Length);
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