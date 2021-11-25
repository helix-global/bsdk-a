using System;
using System.IO;
using System.Runtime.InteropServices;
using BinaryStudio.IO;
using Microsoft.Win32;

namespace BinaryStudio.PortableExecutable
    {
    public class PortableExecutableSource : CommonObjectFileSource
        {
        protected override Boolean IgnoreOptionalHeaderSize { get { return false; }}
        internal PortableExecutableSource(MetadataScope scope)
            : base(scope)
            {
            }

        private const UInt32 IMAGE_DOS_SIGNATURE    = 0x5A4D;
        private const UInt32 IMAGE_OS2_NE_SIGNATURE = 0x454E;
        private const UInt32 IMAGE_OS2_LE_SIGNATURE = 0x454c;
        private const UInt32 IMAGE_OS2_LX_SIGNATURE = 0x584c;
        private const UInt32 IMAGE_NT_SIGNATURE     = 0x00004550;

        /// <summary>
        /// OS/2 .EXE header.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct IMAGE_OS2_HEADER
            {
            private readonly UInt16 ne_magic;           /* Magic number.                       */
            private readonly SByte  ne_ver;             /* Version number.                     */
            private readonly SByte  ne_rev;             /* Revision number.                    */
            public  readonly UInt16 ne_enttab;          /* Offset of Entry Table.              */
            public  readonly UInt16 ne_cbenttab;        /* Number of bytes in Entry Table.     */
            private readonly UInt32 ne_crc;             /* Checksum of whole file.             */
            private readonly UInt16 ne_flags;           /* Flag word.                          */
            private readonly UInt16 ne_autodata;        /* Automatic data segment number.      */
            private readonly UInt16 ne_heap;            /* Initial heap allocation.            */
            private readonly UInt16 ne_stack;           /* Initial stack allocation.           */
            private readonly UInt32 ne_csip;            /* Initial CS:IP setting.              */
            private readonly UInt32 ne_sssp;            /* Initial SS:SP setting.              */
            public  readonly UInt16 ne_cseg;            /* Count of file segments.             */
            private readonly UInt16 ne_cmod;            /* Entries in Module Reference Table.  */
            public  readonly UInt16 ne_cbnrestab;       /* Size of non-resident name table.    */
            public  readonly UInt16 ne_segtab;          /* Offset of Segment Table.            */
            public  readonly UInt16 ne_rsrctab;         /* Offset of Resource Table.           */
            public  readonly UInt16 ne_restab;          /* Offset of resident name table.      */
            public  readonly UInt16 ne_modtab;          /* Offset of Module Reference Table.   */
            public  readonly UInt16 ne_imptab;          /* Offset of Imported Names Table.     */
            public  readonly UInt32 ne_nrestab;         /* Offset of Non-resident Names Table. */
            private readonly UInt16 ne_cmovent;         /* Count of movable entries.           */
            public  readonly UInt16 ne_align;           /* Segment alignment shift count.      */
            private readonly UInt16 ne_cres;            /* Count of resource segments.         */
            private readonly Byte   ne_exetyp;          /* Target Operating system.            */
            private readonly Byte   ne_flagsothers;     /* Other .EXE flags.                   */
            private readonly UInt16 ne_pretthunks;      /* offset to return thunks.            */
            private readonly UInt16 ne_psegrefbytes;    /* offset to segment ref. bytes.       */
            private readonly UInt16 ne_swaparea;        /* Minimum code swap area size.        */
            private readonly UInt16 ne_expver;          /* Expected Windows version number.    */
            }

        #region T:IMAGE_DOS_HEADER
        /// <summary>
        /// DOS .EXE header.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct IMAGE_DOS_HEADER
            {
            public  readonly UInt16 e_magic;            /* Magic number.                        */
            private readonly UInt16 e_cblp;             /* Bytes on last page of file.          */
            private readonly UInt16 e_cp;               /* Pages in file.                       */
            private readonly UInt16 e_crlc;             /* Relocations.                         */
            private readonly UInt16 e_cparhdr;          /* Size of header in paragraphs.        */
            private readonly UInt16 e_minalloc;         /* Minimum extra paragraphs needed.     */
            private readonly UInt16 e_maxalloc;         /* Maximum extra paragraphs needed.     */
            private readonly UInt16 e_ss;               /* Initial (relative) SS value.         */
            private readonly UInt16 e_sp;               /* Initial SP value.                    */
            private readonly UInt16 e_csum;             /* Checksum.                            */
            private readonly UInt16 e_ip;               /* Initial IP value.                    */
            private readonly UInt16 e_cs;               /* Initial (relative) CS value.         */
            private readonly UInt16 e_lfarlc;           /* File address of relocation table.    */
            private readonly UInt16 e_ovno;             /* Overlay number.                      */
            private unsafe fixed UInt16 e_res[4];       /* Reserved words.                      */
            private readonly UInt16 e_oemid;            /* OEM identifier (for e_oeminfo).      */
            private readonly UInt16 e_oeminfo;          /* OEM information; e_oemid specific.   */
            private unsafe fixed UInt16 e_res2[10];     /* Reserved words.                      */
            public  readonly UInt32 e_lfanew;           /* File address of new exe header.      */
            }
        #endregion

        #region M:Load([Out]Exception,Byte*,Int64):Boolean
        protected internal override unsafe Boolean Load(out Exception e, Byte* source, Int64 size)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            e = null;
            if (size > sizeof(IMAGE_DOS_HEADER)) {
                var header = (IMAGE_DOS_HEADER*)source;
                if (header->e_magic == IMAGE_DOS_SIGNATURE) {
                    Load(source, header, size - sizeof(IMAGE_DOS_HEADER));
                    return true;
                    }
                }
            return false;
            }
        #endregion

        private unsafe void Load(Byte* source, IMAGE_DOS_HEADER* header, Int64 size) {
            var mapping = &source[header->e_lfanew];
            var magic = (UInt32*)mapping;
            if (*magic == IMAGE_NT_SIGNATURE) {
                size    -= sizeof(UInt32);
                mapping += sizeof(UInt32);
                Load(source, (IMAGE_FILE_HEADER*)mapping, size);
                }
            else if ((*magic & 0xFFFF) == IMAGE_OS2_NE_SIGNATURE) { Load(mapping, (IMAGE_OS2_HEADER*)mapping, size); }
            else if ((*magic & 0xFFFF) == IMAGE_OS2_LX_SIGNATURE) {
                throw new NotImplementedException();
                //new LinearExecutableModule(mapping, size);
                }
            else { throw new NotSupportedException(); }
            }

        private unsafe void Load(Byte* source, IMAGE_OS2_HEADER* header, Int64 size) {
            throw new NotImplementedException();
            }
        }
    }