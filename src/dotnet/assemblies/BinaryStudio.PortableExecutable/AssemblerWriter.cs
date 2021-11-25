using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace BinaryStudio.PortableExecutable
    {
    public class AssemblerWriter
        {
        private const Byte REX_PREFIX = 0x40;
        private const Byte REX_W      = 0x08;
        private const Byte REX_R      = 0x04;
        private const Byte REX_X      = 0x02;
        private const Byte REX_B      = 0x01;
        private static IList<Opcode> opcodes = new List<Opcode>();

        private static Boolean Is64BitOperandSize(Byte value) { return (value & 0x08) == 0x08; }
        private static Boolean IsModRMByte(Byte value)        { return (value & 0xC0) == 0xC0; }

        [Flags]
        private enum Register
            {
            NONE = -1,
            R32  =  0x0010,
            R16  =  0x0020,
            R8H  =  0x0040,
            R8L  =  0x0080,
            EAX  =  R32 | 0x00,
            ECX  =  R32 | 0x01,
            EDX  =  R32 | 0x02,
            EBX  =  R32 | 0x03,
            ESP  =  R32 | 0x04,
            EBP  =  R32 | 0x05,
            ESI  =  R32 | 0x06,
            EDI  =  R32 | 0x07,
            AX   =  R16 | 0x00,
            CX   =  R16 | 0x01,
            DX   =  R16 | 0x02,
            BX   =  R16 | 0x03,
            SP   =  R16 | 0x04,
            BP   =  R16 | 0x05,
            SI   =  R16 | 0x06,
            DI   =  R16 | 0x07,
            }

        private class AddressingForm {
            public Boolean IsRelative { get;set; }
            public Boolean IsDisp8  { get;set; }
            public Boolean IsDisp32 { get;set; }
            public Boolean IsSIB { get;set; }
            public Register FirstRegister  { get;set; }
            public Register SecondRegister { get;set; }
            public AddressingForm() {
                }
            }

        private static readonly AddressingForm[] Table32BitAddressingFormsWithTheModRMByte = new AddressingForm[]{
            /* 00 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EAX },
            /* 01 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ECX },
            /* 02 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDX },
            /* 03 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EBX },
            /* 04 */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 05 */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 06 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ESI },
            /* 07 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDI },
            /* 08 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EAX },
            /* 09 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ECX },
            /* 0A */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDX },
            /* 0B */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EBX },
            /* 0C */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 0D */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 0E */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ESI },
            /* 0F */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDI },
            /* 10 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EAX },
            /* 11 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ECX },
            /* 12 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDX },
            /* 13 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EBX },
            /* 14 */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 15 */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 16 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ESI },
            /* 17 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDI },
            /* 18 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EAX },
            /* 19 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ECX },
            /* 1A */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDX },
            /* 1B */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EBX },
            /* 1C */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 1D */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 1E */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ESI },
            /* 1F */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDI },
            /* 20 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EAX },
            /* 21 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ECX },
            /* 22 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDX },
            /* 23 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EBX },
            /* 24 */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 25 */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 26 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ESI },
            /* 27 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDI },
            /* 28 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EAX },
            /* 29 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ECX },
            /* 2A */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDX },
            /* 2B */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EBX },
            /* 2C */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 2D */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 2E */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ESI },
            /* 2F */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDI },
            /* 30 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EAX },
            /* 31 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ECX },
            /* 32 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDX },
            /* 33 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EBX },
            /* 34 */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 35 */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 36 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ESI },
            /* 37 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDI },
            /* 38 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EAX },
            /* 39 */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ECX },
            /* 3A */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDX },
            /* 3B */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EBX },
            /* 3C */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 3D */ new AddressingForm{ IsRelative = true, IsSIB = true },
            /* 3E */ new AddressingForm{ IsRelative = true, FirstRegister = Register.ESI },
            /* 3F */ new AddressingForm{ IsRelative = true, FirstRegister = Register.EDI },
            /* 40 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EAX },
            /* 41 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ECX },
            /* 42 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDX },
            /* 43 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBX },
            /* 44 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, IsSIB = true },
            /* 45 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBP },
            /* 46 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ESI },
            /* 47 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDI },
            /* 48 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EAX },
            /* 49 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ECX },
            /* 4A */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDX },
            /* 4B */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBX },
            /* 4C */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, IsSIB = true },
            /* 4D */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBP },
            /* 4E */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ESI },
            /* 4F */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDI },
            /* 50 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EAX },
            /* 51 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ECX },
            /* 52 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDX },
            /* 53 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBX },
            /* 54 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, IsSIB = true },
            /* 55 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBP },
            /* 56 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ESI },
            /* 57 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDI },
            /* 58 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EAX },
            /* 59 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ECX },
            /* 5A */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDX },
            /* 5B */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBX },
            /* 5C */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, IsSIB = true },
            /* 5D */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBP },
            /* 5E */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ESI },
            /* 5F */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDI },
            /* 60 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EAX },
            /* 61 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ECX },
            /* 62 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDX },
            /* 63 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBX },
            /* 64 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, IsSIB = true },
            /* 65 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBP },
            /* 66 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ESI },
            /* 67 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDI },
            /* 68 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EAX },
            /* 69 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ECX },
            /* 6A */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDX },
            /* 6B */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBX },
            /* 6C */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, IsSIB = true },
            /* 6D */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBP },
            /* 6E */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ESI },
            /* 6F */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDI },
            /* 70 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EAX },
            /* 71 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ECX },
            /* 72 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDX },
            /* 73 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBX },
            /* 74 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, IsSIB = true },
            /* 75 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBP },
            /* 76 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ESI },
            /* 77 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDI },
            /* 78 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EAX },
            /* 79 */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ECX },
            /* 7A */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDX },
            /* 7B */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBX },
            /* 7C */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, IsSIB = true },
            /* 7D */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EBP },
            /* 7E */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.ESI },
            /* 7F */ new AddressingForm{ IsRelative = true,  IsDisp8 = true, FirstRegister = Register.EDI },
            /* 80 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EAX },
            /* 81 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ECX },
            /* 82 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDX },
            /* 83 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBX },
            /* 84 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, IsSIB = true },
            /* 85 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBP },
            /* 86 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ESI },
            /* 87 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDI },
            /* 88 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EAX },
            /* 89 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ECX },
            /* 8A */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDX },
            /* 8B */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBX },
            /* 8C */ new AddressingForm{ IsRelative = true, IsDisp32 = true, IsSIB = true },
            /* 8D */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBP },
            /* 8E */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ESI },
            /* 8F */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDI },
            /* 90 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EAX },
            /* 91 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ECX },
            /* 92 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDX },
            /* 93 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBX },
            /* 94 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, IsSIB = true },
            /* 95 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBP },
            /* 96 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ESI },
            /* 97 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDI },
            /* 98 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EAX },
            /* 99 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ECX },
            /* 9A */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDX },
            /* 9B */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBX },
            /* 9C */ new AddressingForm{ IsRelative = true, IsDisp32 = true, IsSIB = true },
            /* 9D */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBP },
            /* 9E */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ESI },
            /* 9F */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDI },
            /* A0 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EAX },
            /* A1 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ECX },
            /* A2 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDX },
            /* A3 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBX },
            /* A4 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, IsSIB = true },
            /* A5 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBP },
            /* A6 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ESI },
            /* A7 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDI },
            /* A8 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EAX },
            /* A9 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ECX },
            /* AA */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDX },
            /* AB */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBX },
            /* AC */ new AddressingForm{ IsRelative = true, IsDisp32 = true, IsSIB = true },
            /* AD */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBP },
            /* AE */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ESI },
            /* AF */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDI },
            /* B0 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EAX },
            /* B1 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ECX },
            /* B2 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDX },
            /* B3 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBX },
            /* B4 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, IsSIB = true },
            /* B5 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBP },
            /* B6 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ESI },
            /* B7 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDI },
            /* B8 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EAX },
            /* B9 */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ECX },
            /* BA */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDX },
            /* BB */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBX },
            /* BC */ new AddressingForm{ IsRelative = true, IsDisp32 = true, IsSIB = true },
            /* BD */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EBP },
            /* BE */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.ESI },
            /* BF */ new AddressingForm{ IsRelative = true, IsDisp32 = true, FirstRegister = Register.EDI },
            /* C0 */ new AddressingForm{ FirstRegister = Register.EAX | Register.AX  },
            /* C1 */ new AddressingForm{ FirstRegister = Register.ECX },
            /* C2 */ new AddressingForm{ FirstRegister = Register.EDX },
            /* C3 */ new AddressingForm{ FirstRegister = Register.EBX },
            /* C4 */ new AddressingForm{ FirstRegister = Register.ESP },
            /* C5 */ new AddressingForm{ FirstRegister = Register.EBP },
            /* C6 */ new AddressingForm{ FirstRegister = Register.ESI },
            /* C7 */ new AddressingForm{ FirstRegister = Register.EDI },
            /* C8 */ new AddressingForm{ FirstRegister = Register.EAX },
            /* C9 */ new AddressingForm{ FirstRegister = Register.ECX },
            /* CA */ new AddressingForm{ FirstRegister = Register.EDX },
            /* CB */ new AddressingForm{ FirstRegister = Register.EBX },
            /* CC */ new AddressingForm{ FirstRegister = Register.ESP },
            /* CD */ new AddressingForm{ FirstRegister = Register.EBP },
            /* CE */ new AddressingForm{ FirstRegister = Register.ESI },
            /* CF */ new AddressingForm{ FirstRegister = Register.EDI },
            /* D0 */ new AddressingForm{ FirstRegister = Register.EAX },
            /* D1 */ new AddressingForm{ FirstRegister = Register.ECX },
            /* D2 */ new AddressingForm{ FirstRegister = Register.EDX },
            /* D3 */ new AddressingForm{ FirstRegister = Register.EBX },
            /* D4 */ new AddressingForm{ FirstRegister = Register.ESP },
            /* D5 */ new AddressingForm{ FirstRegister = Register.EBP },
            /* D6 */ new AddressingForm{ FirstRegister = Register.ESI },
            /* D7 */ new AddressingForm{ FirstRegister = Register.EDI },
            /* D8 */ new AddressingForm{ FirstRegister = Register.EAX },
            /* D9 */ new AddressingForm{ FirstRegister = Register.ECX },
            /* DA */ new AddressingForm{ FirstRegister = Register.EDX },
            /* DB */ new AddressingForm{ FirstRegister = Register.EBX },
            /* DC */ new AddressingForm{ FirstRegister = Register.ESP },
            /* DD */ new AddressingForm{ FirstRegister = Register.EBP },
            /* DE */ new AddressingForm{ FirstRegister = Register.ESI },
            /* DF */ new AddressingForm{ FirstRegister = Register.EDI },
            /* E0 */ new AddressingForm{ FirstRegister = Register.EAX },
            /* E1 */ new AddressingForm{ FirstRegister = Register.ECX },
            /* E2 */ new AddressingForm{ FirstRegister = Register.EDX },
            /* E3 */ new AddressingForm{ FirstRegister = Register.EBX },
            /* E4 */ new AddressingForm{ FirstRegister = Register.ESP },
            /* E5 */ new AddressingForm{ FirstRegister = Register.EBP },
            /* E6 */ new AddressingForm{ FirstRegister = Register.ESI },
            /* E7 */ new AddressingForm{ FirstRegister = Register.EDI },
            /* E8 */ new AddressingForm{ FirstRegister = Register.EAX },
            /* E9 */ new AddressingForm{ FirstRegister = Register.ECX },
            /* EA */ new AddressingForm{ FirstRegister = Register.EDX },
            /* EB */ new AddressingForm{ FirstRegister = Register.EBX },
            /* EC */ new AddressingForm{ FirstRegister = Register.ESP },
            /* ED */ new AddressingForm{ FirstRegister = Register.EBP },
            /* EE */ new AddressingForm{ FirstRegister = Register.ESI },
            /* EF */ new AddressingForm{ FirstRegister = Register.EDI },
            /* F0 */ new AddressingForm{ FirstRegister = Register.EAX },
            /* F1 */ new AddressingForm{ FirstRegister = Register.ECX },
            /* F2 */ new AddressingForm{ FirstRegister = Register.EDX },
            /* F3 */ new AddressingForm{ FirstRegister = Register.EBX },
            /* F4 */ new AddressingForm{ FirstRegister = Register.ESP },
            /* F5 */ new AddressingForm{ FirstRegister = Register.EBP },
            /* F6 */ new AddressingForm{ FirstRegister = Register.ESI },
            /* F7 */ new AddressingForm{ FirstRegister = Register.EDI },
            /* F8 */ new AddressingForm{ FirstRegister = Register.EAX },
            /* F9 */ new AddressingForm{ FirstRegister = Register.ECX },
            /* FA */ new AddressingForm{ FirstRegister = Register.EDX },
            /* FB */ new AddressingForm{ FirstRegister = Register.EBX },
            /* FC */ new AddressingForm{ FirstRegister = Register.ESP },
            /* FD */ new AddressingForm{ FirstRegister = Register.EBP },
            /* FE */ new AddressingForm{ FirstRegister = Register.ESI },
            /* FF */ new AddressingForm{ FirstRegister = Register.EDI },
            };

        private class Opcode {
            public String[] Values { get; }
            public Opcode(String[] source) {
                Values = source;
                }

            /// <summary>Returns a string that represents the current object.</summary>
            /// <returns>A string that represents the current object.</returns>
            public override String ToString()
                {
                return String.Join(" ", Values);
                }
            }

        static AssemblerWriter()
            {
            var r = new XmlDocument();
            r.Load(@"Properties\\x86-x64.xml");
            foreach (XmlNode node in r.DocumentElement.ChildNodes) {
                opcodes.Add(new Opcode(node.InnerText.Split(new []{' '}, StringSplitOptions.RemoveEmptyEntries)));
                }
            }

        public unsafe void Write(TextWriter writer, Byte* source, Int64 size) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            while (size > 0) {
                if ((*source & 0xF0) == REX_PREFIX) {
                    var IsW = Is64BitOperandSize(*source);
                    var IsR = (*source & REX_R) == REX_R;
                    var IsX = (*source & REX_X) == REX_X;
                    var IsB = (*source & REX_B) == REX_B;
                    var index = 0;
                    Opcode[] values = null;
                    if (IsW) {
                        values = opcodes.Where(i => i.Values[index] == "REX.W").ToArray();
                        index++;
                        }
                    else
                        {
                        values = opcodes.Where(i => i.Values[index] != "REX.W").ToArray();
                        }
                    var opcode = *++source;
                    values = values.Where(i => i.Values[index] == opcode.ToString("X2").ToUpper()).ToArray();
                    Byte? reg = null;
                    Byte? rm = null;
                    var opdesc = values[0].Values[index + 1];
                    if (IsR) {
                        var ModRM = *++source;
                        reg = (Byte)((ModRM >> 3) & 0x07);
                        rm =  (Byte)(ModRM & 0x07);
                        if (IsX) {
                            var SIB = *++source;
                            }
                        }
                    else if (IsB)
                        {
                        reg = (Byte)(opcode & 0x07);
                        }
                    if (opdesc == "/r") {
                        var ModRM = *++source;
                        var mod = (Byte)((ModRM >> 6) & 0x03);
                        reg = (Byte)((ModRM >> 3) & 0x07);
                        rm =  (Byte)(ModRM & 0x07);
                        }

                    //size--;
                    //source++;
                    //switch (*source) {
                    //    case 0x89:
                    //    case 0x8B:
                    //    case 0x8C:
                    //    case 0x8E:
                    //        {
                    //        writer.Write("mov");
                    //        var ModRM = source[1];
                    //        var REG = (ModRM & 0x38) >> 3;
                    //        var RM  = (ModRM & 0x07);
                    //        if (IsB) {
                    //            var SIB = source[2];
                    //            }
                    //        }
                    //        break;
                    //    #region MOV AL,moffs8
                    //    case 0xA0:
                    //        {
                    //        writer.Write("mov");
                    //        }
                    //        break;
                    //    #endregion
                    //    #region MOV RAX,moffs64
                    //    case 0xA1:
                    //        {
                    //        writer.Write("mov");
                    //        }
                    //        break;
                    //    #endregion
                    //    #region MOV moffs8,AL
                    //    case 0xA2:
                    //        {
                    //        writer.Write("mov");
                    //        }
                    //        break;
                    //    #endregion
                    //    #region MOV moffs64,RAX
                    //    case 0xA3:
                    //        {
                    //        writer.Write("mov");
                    //        }
                    //        break;
                    //    #endregion
                    //    #region MOV r64, imm64
                    //    case 0xB8:
                    //        {
                    //        writer.Write("mov");
                    //        }
                    //        break;
                    //    #endregion
                    //    #region MOV r/m8, imm8
                    //    case 0xC6:
                    //        {
                    //        writer.Write("mov");
                    //        }
                    //        break;
                    //    #endregion
                    //    #region MOV r/m64, imm32
                    //    case 0xC7:
                    //        {
                    //        writer.Write("mov");
                    //        }
                    //        break;
                    //    #endregion
                    //    }
                    }
                }
            }

        public unsafe void Write(TextWriter writer, Byte[] source) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            fixed (Byte* target = source) {
                Write(writer, target, source.LongLength);
                }
            }
        }
    }