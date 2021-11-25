using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BinaryStudio.PortableExecutable
    {
    internal class OpcodeStreamReader64 : OpcodeStreamReader
        {
        private class Opcode : IOpcode
            {
            public Int32 Size { get; }
            public String Instruction { get; }
            public IInstructionOperandCollection Operands { get; }
            public String Postfix { get; }

            public String ToStringInternal()
                {
                return opcode;
                }

            public Opcode(Int32 size, String opcode) {
                Size = size;
                this.opcode = opcode;
                }

            public Opcode(Int64 size, String opcode) {
                Size = (Int32)size;
                this.opcode = opcode;
                }

            private readonly String opcode;
            }

        private abstract class R
            {
            public abstract Int32 Size { get; }
            public String Name { get; }
            protected R(String name)
                {
                Name = name;
                }

            /// <summary>Returns a string that represents the current object.</summary>
            /// <returns>A string that represents the current object.</returns>
            public override String ToString()
                {
                return Name;
                }
            }

        private class r8 : R
            {
            public override Int32 Size { get { return 8; }}
            public r8(String name)
                : base(name)
                {
                }
            }

        private class r16 : R
            {
            public override Int32 Size { get { return 16; }}
            public r16(String name)
                : base(name)
                {
                }
            }

        private class r32 : R
            {
            public override Int32 Size { get { return 32; }}
            public r32(String name)
                : base(name)
                {
                }
            }

        private class r64 : R
            {
            public override Int32 Size { get { return 64; }}
            public r64(String name)
                : base(name)
                {
                }
            }

        private static R[] AL_BH    = { new  r8("al"),   new r8("cl"),    new r8("dl"),   new r8("bl"), new r8("ah"), new r8("ch"), new r8("dh"), new r8("bh") };
        private static R[] AX_DI    = { new r16("ax"),  new r16("cx"),   new r16("dx"),  new r16("bx"), new r16("sp"), new r16("bp"), new r16("si"), new r16("di") };
        private static R[] EAX_EDI  = { new r32("eax"), new r32("ecx"), new r32("edx"),  new r32("ebx"), new r32("esp"), new r32("ebp"), new r32("esi"), new r32("edi") };
        private static R[] RAX_RDI  = { new r64("rax"), new r64("rcx"), new r64("rdx"),  new r64("rbx"), new r64("rsp"), new r64("rbp"), new r64("rsi"), new r64("rdi") };
        private static R[] R8_R15   = { new r64("r8"),   new r64("r9"), new r64("r10"),  new r64("r11"), new r64("r12"), new r64("r13"), new r64("r14"), new r64("r15") };
        private static R[] R8b_R15b = { new  r8("r8b"),  new r8("r9b"),  new r8("r10b"), new r8("r11b"), new r8("r12b"), new r8("r13b"), new r8("r14b"), new r8("r15b") };
        private static R[] R8d_R15d = { new r32("r8d"), new r32("r9d"), new r32("r10d"), new r32("r11d"), new r32("r12d"), new r32("r13d"), new r32("r14d"), new r32("r15d") };
        private static R[] R8w_R15w = { new r16("r8w"), new r16("r9w"), new r16("r10w"), new r16("r11w"), new r16("r12w"), new r16("r13w"), new r16("r14w"), new r16("r15w") };

        [Flags]
        private enum REXPrefixFlags : byte
            {
            ExtensionOfTheRMFieldSIBBaseFieldOrOpcodeRegField = 0x01,
            ExtensionOfTheSIBIndexField                       = 0x02,
            ExtensionOfTheRegField                            = 0x04,
            Is64BitOperandSize                                = 0x08
            }

        private interface OpcodeDecoder
            {
            }

        private class OpcodeDecoderMR : OpcodeDecoder
            {
            public OpcodeDecoderMR(String opcode) {

                }
            }

        private static readonly IDictionary<Byte,OpcodeDecoder> DecodingTable = new Dictionary<Byte,OpcodeDecoder>{
            { 0x00, new OpcodeDecoderMR("ADD") },
            { 0x01, new OpcodeDecoderMR("ADD") },
            { 0x08, new OpcodeDecoderMR("OR") },
            { 0x09, new OpcodeDecoderMR("OR") },
            { 0x10, new OpcodeDecoderMR("ADC") },
            { 0x11, new OpcodeDecoderMR("ADC") },
            { 0x18, new OpcodeDecoderMR("SBB") },
            { 0x19, new OpcodeDecoderMR("SBB") },
            { 0x20, new OpcodeDecoderMR("AND") },
            { 0x21, new OpcodeDecoderMR("AND") },
            { 0x28, new OpcodeDecoderMR("SUB") },
            { 0x29, new OpcodeDecoderMR("SUB") },
            { 0x30, new OpcodeDecoderMR("XOR") },
            { 0x31, new OpcodeDecoderMR("XOR") },
            { 0x38, new OpcodeDecoderMR("CMP") },
            { 0x39, new OpcodeDecoderMR("CMP") },
            { 0x84, new OpcodeDecoderMR("TEST") },
            { 0x85, new OpcodeDecoderMR("TEST") },
            { 0x86, new OpcodeDecoderMR("XCHG") },
            { 0x87, new OpcodeDecoderMR("XCHG") },
            { 0x88, new OpcodeDecoderMR("MOV") },
            { 0x89, new OpcodeDecoderMR("MOV") },
            { 0x8C, new OpcodeDecoderMR("MOV") },
            };

        private readonly unsafe Byte* First;
        private unsafe Byte* Last,Current;
        private readonly Int64 Size = -1;
        public unsafe OpcodeStreamReader64(IntPtr source)
            : base(source)
            {
            First = (Byte*)source;
            Current = First;
            Last = null;
            }

        public OpcodeStreamReader64(Byte[] source)
            : base(source)
            {
            }

        private static unsafe UInt32 ReadUInt32(ref Byte* i)
            {
            var r = *(UInt32*)i;
            i += sizeof(UInt32);
            return r;
            }

        private static unsafe Byte ReadByte(ref Byte* i)
            {
            return *i++;
            }

        private static String ToString(UInt32 value) {
            var r = value.ToString("x");
            return (r[0] > '9')
                ? $"0{r}h"
                : $"{r}h";
            }

        private static String ToString(Byte value) {
            var r = value.ToString("x");
            return (r[0] > '9')
                ? $"0{r}h"
                : $"{r}h";
            }

    static unsafe string DecodeSIB(ref Byte* i, Byte mo, R[] OSprefix, R[] ASprefix, R sreg) {
        var si = (UInt16)((*i & 0x38) >> 3);
        var rg = ((*i & 0x07));
        switch (((*i & 0xC0) >> 6))
            {
            case 0:
                {
                i++;
                if ((si != 4) && (rg != 5)) {
                    return ASprefix[si].ToString() + "+" + OSprefix[rg].ToString();
                    }
                if (rg != 5) { return ASprefix[rg].ToString(); }
                if (sreg != null) { return sreg.ToString() + ":"; }
                Debug.Assert(false);
                }
                break;
            case 1:
                {
                i++;
                if ((si != 4) && (rg != 5)) {
                    return ASprefix[si].ToString() + "*2+" + OSprefix[rg].ToString();
                    }
                Debug.Assert(false);
                }
                break;
            case 2:
                {
                i++;
                if ((si != 4) && (rg != 5)) {
                    return ASprefix[si].ToString() + "*4+" + OSprefix[rg].ToString();
                    }
                Debug.Assert(false);
                }
                break;
            case 3:
                {
                i++;
                if ((si != 4) && (rg != 5)) {
                    return ASprefix[si].ToString() + "*8+" + OSprefix[rg].ToString();
                    }
                Debug.Assert(false);
                }
                break;
            default: { Debug.Assert(false); } break;
            }
            return null;
            }

        private static unsafe String DecodeRM(ref Byte* i, R[] OSprefix, R[] ASprefix, R sreg) {
            var I = (UInt32)((*i & 0x07));
            var r = new StringBuilder();
            switch (((*i & 0xC0) >> 6)) {
                case 0:
                    {
                    i++;
                    if ((I != 4) && (I != 5)) {
                        return $"[{ASprefix[I]}]";
                        }
                    if (I == 5) {
                        I = ReadUInt32(ref i);
                        return $"{ToString(I)}";
                        }
                    if (I == 4) {
                        r.Append("[");
                        r.Append(DecodeSIB(ref i,1, OSprefix, ASprefix, sreg));
                        I = ReadUInt32(ref i);
                        if (I != 0) {
                            r.Append("+");
                            r.Append(ToString(I));
                            }
                        r.Append("]");
                        break;
                        }
                    Debug.Assert(false);
                    }
                    break;
                case 1:
                    {
                    i++;
                    if (I != 4) {
                        return $"[{ASprefix[I]}+{ToString(ReadByte(ref i))}]";
                        }
                    r.Append("[");
                    r.Append(DecodeSIB(ref i,1, OSprefix, ASprefix, sreg));
                    I = ReadByte(ref i);
                    if (I != 0) {
                        r.Append("+");
                        r.Append(ToString(I));
                        }
                    r.Append("]");
                    }
                    break;
                case 2:
                    {
                    i++;
                    if (I != 4) {
                        return $"[{ASprefix[I]}+{ToString(ReadUInt32(ref i))}]";
                        }
                    Debug.Assert(false);
                    }
                    break;
                case 3:
                    {
                    i++;
                    return ASprefix[I].ToString();
                    }
                default: { Debug.Assert(false); } break;
                }
            return r.ToString();
            }

        private static unsafe IOpcode OP_r_m_r(Byte* i, R[] OSprefix, R[] ASprefix, R sreg, String c, Int32 n)
            {
            var FI = i;
            var rg = ((*i & 0x38) >> 3);
            var op = DecodeRM(ref i, OSprefix, ASprefix, sreg);
            return new Opcode(i - FI + n, c + " " + op + "," + (OSprefix[rg]));
            }

        private static unsafe IOpcode OP_ri(Byte i, R[] OSprefix, R[] ASprefix, R sreg, String c, Int32 n)
            {
            return new Opcode(n, c + " " + OSprefix[i]);
            }

        private static unsafe IOpcode OP_r_r_m(Byte* i, R[] OSprefix, R[] ASprefix, R sreg, String c, Int32 n)
            {
            var FI = i;
            var rg = ((*i & 0x38) >> 3);
            var op = DecodeRM(ref i, OSprefix, ASprefix, sreg);
            return new Opcode(i - FI + n, c + " " + OSprefix[rg] + "," + op);
            }

        private static unsafe IOpcode Read(Byte* I, REXPrefixFlags? rflags, R[] OSprefix, R[] ASprefix, R sreg, Int32 n) {
            if ((I[0] & 0xF0) == 0x40) {
                if (rflags != null) { throw new InvalidDataException("Only one REX prefix is allowed per instruction."); }
                var rex = (REXPrefixFlags)(I[0] & 0x0F);
                return Read(I + 1, rex, OSprefix ?? RAX_RDI, ASprefix ?? RAX_RDI, sreg, n + 1);
                }
            switch (I[0] & 0xF0) {
                case 0x00 :
                    {
                    switch (I[0] & 0x0F) {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { throw new NotImplementedException(); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0x10 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { throw new NotImplementedException(); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0x20 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { throw new NotImplementedException(); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0x30 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { throw new NotImplementedException(); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0x50 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { return OP_ri(0, OSprefix ?? RAX_RDI, null, null, "push", n + 1); }
                        case 0x01: { return OP_ri(1, OSprefix ?? RAX_RDI, null, null, "push", n + 1); }
                        case 0x02: { return OP_ri(2, OSprefix ?? RAX_RDI, null, null, "push", n + 1); }
                        case 0x03: { return OP_ri(3, OSprefix ?? RAX_RDI, null, null, "push", n + 1); }
                        case 0x04: { return OP_ri(4, OSprefix ?? RAX_RDI, null, null, "push", n + 1); }
                        case 0x05: { return OP_ri(5, OSprefix ?? RAX_RDI, null, null, "push", n + 1); }
                        case 0x06: { return OP_ri(6, OSprefix ?? RAX_RDI, null, null, "push", n + 1); }
                        case 0x07: { return OP_ri(7, OSprefix ?? RAX_RDI, null, null, "push", n + 1); }
                        case 0x08: { return OP_ri(0, OSprefix ?? RAX_RDI, null, null, "pop", n + 1); }
                        case 0x09: { return OP_ri(1, OSprefix ?? RAX_RDI, null, null, "pop", n + 1); }
                        case 0x0A: { return OP_ri(2, OSprefix ?? RAX_RDI, null, null, "pop", n + 1); }
                        case 0x0B: { return OP_ri(3, OSprefix ?? RAX_RDI, null, null, "pop", n + 1); }
                        case 0x0C: { return OP_ri(4, OSprefix ?? RAX_RDI, null, null, "pop", n + 1); }
                        case 0x0D: { return OP_ri(5, OSprefix ?? RAX_RDI, null, null, "pop", n + 1); }
                        case 0x0E: { return OP_ri(6, OSprefix ?? RAX_RDI, null, null, "pop", n + 1); }
                        case 0x0F: { return OP_ri(7, OSprefix ?? RAX_RDI, null, null, "pop", n + 1); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0x60 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { throw new NotImplementedException(); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0x70 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { throw new NotImplementedException(); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0x80 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { return OP_r_m_r(I + 1, OSprefix ?? EAX_EDI, ASprefix ?? EAX_EDI, sreg, "mov", n + 1); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0x90 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { throw new NotImplementedException(); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0xA0 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { throw new NotImplementedException(); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0xB0 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { throw new NotImplementedException(); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0xC0 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { throw new NotImplementedException(); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0xD0 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { throw new NotImplementedException(); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0xE0 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { throw new NotImplementedException(); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                case 0xF0 :
                    {
                    switch (I[0] & 0x0F)
                        {
                        case 0x00: { throw new NotImplementedException(); }
                        case 0x01: { throw new NotImplementedException(); }
                        case 0x02: { throw new NotImplementedException(); }
                        case 0x03: { throw new NotImplementedException(); }
                        case 0x04: { throw new NotImplementedException(); }
                        case 0x05: { throw new NotImplementedException(); }
                        case 0x06: { throw new NotImplementedException(); }
                        case 0x07: { throw new NotImplementedException(); }
                        case 0x08: { throw new NotImplementedException(); }
                        case 0x09: { throw new NotImplementedException(); }
                        case 0x0A: { throw new NotImplementedException(); }
                        case 0x0B: { throw new NotImplementedException(); }
                        case 0x0C: { throw new NotImplementedException(); }
                        case 0x0D: { throw new NotImplementedException(); }
                        case 0x0E: { throw new NotImplementedException(); }
                        case 0x0F: { throw new NotImplementedException(); }
                        default: { throw new NotImplementedException(); }
                        }
                    }
                default: { throw new NotImplementedException(); }
                }
            return null;
            }

        public override unsafe IOpcode Read() {
            var I = Current;
            var r = Read(I, null, null, null, null, 0);
            Current += r.Size;
            return r;
            }
        }
    }