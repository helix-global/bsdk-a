#define FEATURE_TYPELIB
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.PortableExecutable.CodeView;
using BinaryStudio.PortableExecutable.CommonObjectFile.Sections;
using BinaryStudio.PortableExecutable.Win32;
using BinaryStudio.Serialization;
using Microsoft.Win32;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace BinaryStudio.PortableExecutable
    {
    public class CommonObjectFileSource : MetadataObject
        {
        private const Int32 IMAGE_NT_OPTIONAL_HDR32_MAGIC = 0x10b;
        private const Int32 IMAGE_NT_OPTIONAL_HDR64_MAGIC = 0x20b;
        private const Int32 IMAGE_ROM_OPTIONAL_HDR_MAGIC  = 0x107;
        private const Int32 IMAGE_NUMBEROF_DIRECTORY_ENTRIES = 16;
        private const Int16 IMAGE_SYM_UNDEFINED =  0;
        private const Int16 IMAGE_SYM_ABSOLUTE  = -1;
        private const Int16 IMAGE_SYM_DEBUG     = -2;

        protected virtual Boolean IgnoreOptionalHeaderSize { get { return true; }}
        public IList<ImportLibraryReference> ImportLibraryReferences { get;private set; }
        public IList<ResourceDescriptor> Resources { get;private set; }
        public IList<CommonObjectFileSection> Sections { get;private set; }
        public IList<ExportSymbolDescriptor> ExportDescriptors { get;private set; }
        public IList<ISymbol> SymbolTable { get;private set; }
        public ImageFlags Flags { get;private set; }
        public IMAGE_FILE_MACHINE Machine  { get;private set; }
        public Int32 NumberOfSymbols       { get;private set; }
        public Int32 SizeOfOptionalHeader  { get;private set; }
        public IMAGE_FILE_CHARACTERISTIC Characteristics  { get;private set; }
        public DateTime TimeDateStamp { get; }
        public Boolean Is64Bit { get { return Flags.HasFlag(ImageFlags.Is64Bit); }}
        private TypeLibraryDescriptor TypeLibrary;
        private unsafe IMAGE_FILE_HEADER* Header;
        public CV_CPU_TYPE? CPU;
       
        /// <summary>
        /// [Resource Data Entry]
        /// Each Resource Data entry describes an actual unit of raw data in the Resource Data area.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct IMAGE_RESOURCE_DATA_ENTRY
            {
            public  readonly UInt32 OffsetToData;           /* The address of a unit of resource data in the Resource Data area.                                                                            */
            public  readonly UInt32 Size;                   /* The size, in bytes, of the resource data that is pointed to by the Data RVA field.                                                           */
            public  readonly UInt32 CodePage;               /* The code page that is used to decode code point values within the resource data. Typically, the code page would be the Unicode code page.    */
            private readonly UInt32 Reserved;               /* Reserved, must be 0.                                                                                                                         */
            }
        
        /// <summary>
        /// [Resource Directory Table]
        /// This data structure should be considered the heading of a table because the table actually consists of
        /// directory entries.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct IMAGE_RESOURCE_DIRECTORY
            {
            private readonly UInt32 Characteristics;        /* Resource flags. This field is reserved for future use. It is currently set to zero.                                                                                          */
            private readonly UInt32 TimeDateStamp;          /* The time that the resource data was created by the resource compiler.                                                                                                        */
            private readonly UInt16 MajorVersion;           /* The major version number, set by the user.                                                                                                                                   */
            private readonly UInt16 MinorVersion;           /* The minor version number, set by the user.                                                                                                                                   */
            public  readonly UInt16 NumberOfNamedEntries;   /* The number of directory entries immediately following the table that use strings to identify [Type], [Name], or [Language] entries (depending on the level of the table).    */
            public  readonly UInt16 NumberOfIdEntries;      /* The number of directory entries immediately following the Name entries that use numeric IDs for [Type], [Name], or [Language] entries.                                       */
            }

        /// <summary>
        /// [The Delay-Load Directory Table]
        /// The delay-load directory table is the counterpart to the import directory table.
        /// It can be retrieved through the Delay Import Descriptor entry in the optional header data directories list (offset 200).
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct IMAGE_DELAYLOAD_DESCRIPTOR
            {
            public readonly UInt32 Attributes;              /* Must be zero.                                                                                                */
            public readonly UInt32 Name;                    /* The RVA of the name of the DLL to be loaded. The name resides in the read-only data section of the image.    */
            public readonly UInt32 ModuleHandle;            /* The RVA of the module handle (in the data section of the image) of the DLL to be delay-loaded.
                                               It is used for storage by the routine that is supplied to manage delay-loading. */
            public readonly UInt32 DelayImportAddressTable; /* The RVA of the delay-load import address table.                                                              */
            public readonly UInt32 DelayImportNameTable;    /* The RVA of the delay-load name table, which contains the names of the imports that
                                               might need to be loaded. This matches the layout of the import name table. */
            public readonly UInt32 BoundDelayImportTable;   /* The RVA of the bound delay-load address table, if it exists.                                                 */
            public readonly UInt32 UnloadDelayImportTable;  /* The RVA of the unload delay-load address table, if it exists.
                                               This is an exact copy of the delay import address table. If the caller unloads the DLL,
                                               this table should be copied back over the delay import address table so that subsequent
                                               calls to the DLL continue to use the thunking mechanism correctly. */
            public readonly UInt32 TimeStamp;               /* The timestamp of the DLL to which this image has been bound.                                                 */
            }

        /// <summary>
        /// [Import Directory Table]
        /// The import information begins with the import directory table, which describes the remainder of the import
        /// information. The import directory table contains address information that is used to resolve fixup
        /// references to the entry points within a DLL image. The import directory table consists of an array of
        /// import directory entries, one entry for each DLL to which the image refers. The last directory entry is
        /// empty (filled with null values), which indicates the end of the directory table.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct IMAGE_IMPORT_DIRECTORY
            {
            public  readonly UInt32 ImportLookupTable;      /* The RVA of the import lookup table. This table contains a name or ordinal for each import.                                                         */
            private readonly UInt32 TimeDateStamp;          /* The stamp that is set to zero until the image is bound. After the image is bound, this field is set to the time/data stamp of the DLL.             */
            private readonly UInt32 ForwarderChain;         /* The index of the first forwarder reference.                                                                                                        */
            public  readonly UInt32 Name;                   /* The address of an ASCII string that contains the name of the DLL. This address is relative to the image base.                                      */
            public  readonly UInt32 ImportAddressTable;     /* The RVA of the import address table. The contents of this table are identical to the contents of the import lookup table until the image is bound. */
            }

        /// <summary>
        /// [Export Directory Table]
        /// The export symbol information begins with the export directory table, which describes the remainder of the
        /// export symbol information. The export directory table contains address information that is used to resolve
        /// imports to the entry points within this image.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct IMAGE_EXPORT_DIRECTORY
            {
            private readonly UInt32 ExportFlags;            /* Reserved, must be 0.                                                                                                                                         */
            private readonly UInt32 TimeDateStamp;          /* The time and date that the export data was created.                                                                                                          */
            private readonly UInt16 MajorVersion;           /* The major version number. The major and minor version numbers can be set by the user.                                                                        */
            private readonly UInt16 MinorVersion;           /* The minor version number.                                                                                                                                    */
            private readonly UInt32 NameRVA;                /* The address of the ASCII string that contains the name of the DLL. This address is relative to the image base.                                               */
            public  readonly UInt32 OrdinalBase;            /* The starting ordinal number for exports in this image. This field specifies the starting ordinal number for the export address table. It is usually set to 1.*/
            public  readonly UInt32 AddressTableEntries;    /* The number of entries in the export address table.                                                                                                           */
            public  readonly UInt32 NumberOfNamePointers;   /* The number of entries in the name pointer table. This is also the number of entries in the ordinal table.                                                    */
            public  readonly UInt32 ExportAddressTableRVA;  /* The address of the export address table, relative to the image base.                                                                                         */
            public  readonly UInt32 NamePointerRVA;         /* The address of the export name pointer table, relative to the image base. The table size is given by the [NumberOfNamePointers] field.                       */
            public  readonly UInt32 OrdinalTableRVA;        /* The address of the ordinal table, relative to the image base.                                                                                                */
            }

        protected enum IMAGE_DIRECTORY_ENTRY
            {
            IMAGE_DIRECTORY_ENTRY_EXPORT            =  0,   /* Export Directory                     */
            IMAGE_DIRECTORY_ENTRY_IMPORT            =  1,   /* Import Directory                     */
            IMAGE_DIRECTORY_ENTRY_RESOURCE          =  2,   /* Resource Directory                   */
            IMAGE_DIRECTORY_ENTRY_EXCEPTION         =  3,   /* Exception Directory                  */
            IMAGE_DIRECTORY_ENTRY_SECURITY          =  4,   /* Security Directory                   */
            IMAGE_DIRECTORY_ENTRY_BASERELOC         =  5,   /* Base Relocation Table                */
            IMAGE_DIRECTORY_ENTRY_DEBUG             =  6,   /* Debug Directory                      */
            IMAGE_DIRECTORY_ENTRY_COPYRIGHT         =  7,   /* (X86 usage)                          */
            IMAGE_DIRECTORY_ENTRY_ARCHITECTURE      =  7,   /* Architecture Specific Data           */
            IMAGE_DIRECTORY_ENTRY_GLOBALPTR         =  8,   /* RVA of GP                            */
            IMAGE_DIRECTORY_ENTRY_TLS               =  9,   /* TLS Directory                        */
            IMAGE_DIRECTORY_ENTRY_LOAD_CONFIG       = 10,   /* Load Configuration Directory         */
            IMAGE_DIRECTORY_ENTRY_BOUND_IMPORT      = 11,   /* Bound Import Directory in headers    */
            IMAGE_DIRECTORY_ENTRY_IAT               = 12,   /* Import Address Table                 */
            IMAGE_DIRECTORY_ENTRY_DELAY_IMPORT      = 13,   /* Delay Load Import Descriptors        */
            IMAGE_DIRECTORY_ENTRY_COM_DESCRIPTOR    = 14    /* COM Runtime descriptor               */
            }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct IMAGE_OPTIONAL_HEADER32
            {
            private readonly UInt16 Magic;                                  /* The unsigned integer that identifies the state of the image file.                                                                                                                                                                                                                                                                */
            private readonly Byte   MajorLinkerVersion;                     /* The linker major version number.                                                                                                                                                                                                                                                                                                 */
            private readonly Byte   MinorLinkerVersion;                     /* The linker minor version number.                                                                                                                                                                                                                                                                                                 */
            private readonly UInt32 SizeOfCode;                             /* The size of the code (text) section, or the sum of all code sections if there are multiple sections.                                                                                                                                                                                                                             */
            private readonly UInt32 SizeOfInitializedData;                  /* The size of the initialized data section, or the sum of all such sections if there are multiple data sections.                                                                                                                                                                                                                   */
            private readonly UInt32 SizeOfUninitializedData;                /* The size of the uninitialized data section (BSS), or the sum of all such sections if there are multiple BSS sections.                                                                                                                                                                                                            */
            private readonly UInt32 AddressOfEntryPoint;                    /* The address of the entry point relative to the image base when the executable file is loaded into memory. For program images, this is the starting address. For device drivers, this is the address of the initialization function. An entry point is optional for DLLs. When no entry point is present, this field must be zero.*/
            private readonly UInt32 BaseOfCode;                             /* The address that is relative to the image base of the beginning-of-code section when it is loaded into memory.                                                                                                                                                                                                                   */
            private readonly UInt32 BaseOfData;                             /* The address that is relative to the image base of the beginning-of-data section when it is loaded into memory.                                                                                                                                                                                                                   */
            private readonly UInt32 ImageBase;                              /* The preferred address of the first byte of image when loaded into memory; must be a multiple of 64 K. The default for DLLs is 0x10000000. The default for Windows CE EXEs is 0x00010000. The default for Windows NT, Windows 2000, Windows XP, Windows 95, Windows 98, and Windows Me is 0x00400000.                             */
            private readonly UInt32 SectionAlignment;                       /* The alignment (in bytes) of sections when they are loaded into memory. It must be greater than or equal to FileAlignment. The default is the page size for the architecture.                                                                                                                                                     */
            private readonly UInt32 FileAlignment;                          /* The alignment factor (in bytes) that is used to align the raw data of sections in the image file. The value should be a power of 2 between 512 and 64 K, inclusive. The default is 512. If the [SectionAlignment] is less than the architecture’s page size, then FileAlignment must match [SectionAlignment].                   */
            private readonly UInt16 MajorOperatingSystemVersion;            /* The major version number of the required operating system.                                                                                                                                                                                                                                                                       */
            private readonly UInt16 MinorOperatingSystemVersion;            /* The minor version number of the required operating system.                                                                                                                                                                                                                                                                       */
            private readonly UInt16 MajorImageVersion;                      /* The major version number of the image.                                                                                                                                                                                                                                                                                           */
            private readonly UInt16 MinorImageVersion;                      /* The minor version number of the image.                                                                                                                                                                                                                                                                                           */
            private readonly UInt16 MajorSubsystemVersion;                  /* The major version number of the subsystem.                                                                                                                                                                                                                                                                                       */
            private readonly UInt16 MinorSubsystemVersion;                  /* The minor version number of the subsystem.                                                                                                                                                                                                                                                                                       */
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly UInt32 Win32VersionValue;                      /* Reserved, must be zero.                                                                                                                                                                                                                                                                                                          */
            private readonly UInt32 SizeOfImage;                            /* The size (in bytes) of the image, including all headers, as the image is loaded in memory. It must be a multiple of [SectionAlignment].                                                                                                                                                                                          */
            private readonly UInt32 SizeOfHeaders;                          /* The combined size of an MS DOS stub, PE header, and section headers rounded up to a multiple of [FileAlignment].                                                                                                                                                                                                                 */
            private readonly UInt32 CheckSum;                               /* The image file checksum. The algorithm for computing the checksum is incorporated into IMAGHELP.DLL. The following are checked for validation at load time: all drivers, any DLL loaded at boot time, and any DLL that is loaded into a critical Windows process.                                                                */
            private readonly IMAGE_SUBSYSTEM Subsystem;                     /* The subsystem that is required to run this image.                                                                                                                                                                                                                                                                                */
            private readonly IMAGE_DLLCHARACTERISTICS DllCharacteristics;   /* DLL characteristics.                                                                                                                                                                                                                                                                                                             */
            private readonly UInt32 SizeOfStackReserve;                     /* The size of the stack to reserve. Only [SizeOfStackCommit] is committed; the rest is made available one page at a time until the reserve size is reached.                                                                                                                                                                        */
            private readonly UInt32 SizeOfStackCommit;                      /* The size of the stack to commit.                                                                                                                                                                                                                                                                                                 */
            private readonly UInt32 SizeOfHeapReserve;                      /* The size of the local heap space to reserve. Only [SizeOfHeapCommit] is committed; the rest is made available one page at a time until the reserve size is reached.                                                                                                                                                              */
            private readonly UInt32 SizeOfHeapCommit;                       /* The size of the local heap space to commit.                                                                                                                                                                                                                                                                                      */
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly UInt32 LoaderFlags;                            /* Reserved, must be zero.                                                                                                                                                                                                                                                                                                          */
            public  readonly UInt32 NumberOfRvaAndSizes;                    /* The number of data-directory entries in the remainder of the optional header. Each describes a location and size.                                                                                                                                                                                                                */
            }

        #region T:IMAGE_ROM_OPTIONAL_HEADER
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct IMAGE_ROM_OPTIONAL_HEADER
            {
            private readonly UInt16 Magic;
            private readonly Byte MajorLinkerVersion;
            private readonly Byte MinorLinkerVersion;
            private readonly UInt32 SizeOfCode;
            private readonly UInt32 SizeOfInitializedData;
            private readonly UInt32 SizeOfUninitializedData;
            private readonly UInt32 AddressOfEntryPoint;
            private readonly UInt32 BaseOfCode;
            private readonly UInt32 BaseOfData;
            private readonly UInt32 BaseOfBss;
            private readonly UInt32 GprMask;
            private unsafe fixed UInt32 CprMask[4];
            private readonly UInt32 GpValue;
            }
        #endregion

        private unsafe delegate void* RVA(UInt32 virtualaddress);

        #region M:Load([Out]Exception,Byte*,Int64):Boolean
        protected internal override unsafe Boolean Load(out Exception e, Byte* source, Int64 size)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            e = null;
            var OH = (ANON_OBJECT_HEADER*)source;
            if ((OH->Sig1 == 0) && (OH->Sig2 == 0xFFFF)) {
                if (OH->Version >= 1) {
                    return false;
                    if (Enum.IsDefined(typeof(IMAGE_FILE_MACHINE), OH->Machine)) {
                        return Load(source, OH, size);
                        }
                    }
                }
            var header = (IMAGE_FILE_HEADER*)source;
            if (Enum.IsDefined(typeof(IMAGE_FILE_MACHINE), header->Machine)) {
                return Load(source, header, size);
                }
            return false;
            }
        #endregion
        protected unsafe Boolean Load(Byte* mapping, ANON_OBJECT_HEADER* source, Int64 size) {
            if (size > Marshal.SizeOf(typeof(ANON_OBJECT_HEADER))) {
                if (Enum.IsDefined(typeof(IMAGE_FILE_MACHINE), source->Machine)) {
                    var machine = source->Machine;
                    switch (source->ClassID.ToString("N").ToLower()) {
                        case "0cb3fe38d9a54dabac9bd6b6222653c2":
                            {
                            var H = (ANON_OBJECT_HEADER_BIGOBJ_V1*)source;
                            var sections = (IMAGE_SECTION_HEADER*)(H + 1);
                            #if DEBUG
                            for (var i = 0; i < H->NumberOfSections; ++i) {
                                Debug.Print("section:\"{0}\":{1:X8}:{2:X8}",
                                    sections[i],
                                    sections[i].VirtualAddress,
                                    sections[i].VirtualAddress + sections[i].VirtualSize);
                                }
                            #endif
                            #region COFF Symbol Table
                            if (H->PointerToSymbolTable != 0) {
                                SymbolTable = new ReadOnlyCollection<ISymbol>(LoadSymbolTable(mapping,
                                    //H->NumberOfSymbols,
                                    10,
                                    H->PointerToSymbolTable));
                                }
                            #endregion
                            }
                            break;
                        }
                    var riid = Guid.Parse("00000000-0000-0000-C000-000000000046");
                    var hr = CoCreateInstance(ref source->ClassID, IntPtr.Zero, CLSCTX.CLSCTX_INPROC_SERVER, ref riid, out var ppv);
                    
                    //var rvami = new RVA((virtualaddress)=>{
                    //    for (var i = 0; i < source->NumberOfSections; ++i) {
                    //        if ((sections[i].VirtualAddress <= virtualaddress) && (virtualaddress <= (sections[i].VirtualAddress + sections[i].VirtualSize))) {
                    //            return mapping
                    //                + (Int64)(sections[i].PointerToRawData
                    //                - sections[i].VirtualAddress + virtualaddress);
                    //            }
                    //        }
                    //    return null;
                    //    });
                    }
                }
            return false;
            }
        #region M:Load(Byte*,IMAGE_FILE_HEADER*,Int64):Boolean
        protected unsafe Boolean Load(Byte* mapping, IMAGE_FILE_HEADER* source, Int64 size) {
            if (size > Marshal.SizeOf(typeof(IMAGE_FILE_HEADER))) {
                if (Enum.IsDefined(typeof(IMAGE_FILE_MACHINE), source->Machine)) {
                    Header = source;
                    Machine = source->Machine;
                    Characteristics = source->Characteristics;
                    var machine = source->Machine;
                    var r = (Byte*)(source + 1);
                    IMAGE_DATA_DIRECTORY*[] directories = null;
                    if ((source->SizeOfOptionalHeader > 0) && !IgnoreOptionalHeaderSize) {
                        var magic = *(UInt16*)r;
                        if ((magic == IMAGE_NT_OPTIONAL_HDR32_MAGIC) && (source->SizeOfOptionalHeader == (sizeof(IMAGE_OPTIONAL_HEADER32) + sizeof(IMAGE_DATA_DIRECTORY)*IMAGE_NUMBEROF_DIRECTORY_ENTRIES)))   { directories = Load((IMAGE_OPTIONAL_HEADER32*)r, size); }
                        if ((magic == IMAGE_NT_OPTIONAL_HDR64_MAGIC) && (source->SizeOfOptionalHeader == (sizeof(IMAGE_OPTIONAL_HEADER64) + sizeof(IMAGE_DATA_DIRECTORY)*IMAGE_NUMBEROF_DIRECTORY_ENTRIES)))   { directories = Load((IMAGE_OPTIONAL_HEADER64*)r, size); Flags |= ImageFlags.Is64Bit; }
                        if ((magic == IMAGE_ROM_OPTIONAL_HDR_MAGIC)  && (source->SizeOfOptionalHeader == sizeof(IMAGE_ROM_OPTIONAL_HEADER))) { directories = Load((IMAGE_ROM_OPTIONAL_HEADER*)r, size); }
                        r += source->SizeOfOptionalHeader;
                        }
                    var sections = (IMAGE_SECTION_HEADER*)r;
                    var rvami = new RVA((virtualaddress)=>{
                        for (var i = 0; i < source->NumberOfSections; ++i) {
                            if ((sections[i].VirtualAddress <= virtualaddress) && (virtualaddress <= (sections[i].VirtualAddress + sections[i].VirtualSize))) {
                                return mapping
                                    + (Int64)(sections[i].PointerToRawData
                                    - sections[i].VirtualAddress + virtualaddress);
                                }
                            }
                        return null;
                        });
                    #region COFF Symbol Table
                    if (source->PointerToSymbolTable != 0) {
                        SymbolTable = new ReadOnlyCollection<ISymbol>(LoadSymbolTable(mapping, source));
                        }
                    #endregion
                    #if DEBUG
                    for (var i = 0; i < source->NumberOfSections; ++i) {
                        Debug.Print("section:\"{0}\":{1:X8}:{2:X8}",
                            sections[i],
                            sections[i].VirtualAddress,
                            sections[i].VirtualAddress + sections[i].VirtualSize);
                        }
                    #endif
                    if ((directories != null) && (source->NumberOfSections > 0)) {
                        var sz = source->NumberOfSections;
                        var entries = new List<Tuple<IntPtr, IntPtr, IMAGE_DIRECTORY_ENTRY>>();
                        for (var i = 0; i < directories.Length; i++) {
                            if (directories[i]->Size > 0) {
                                for (var j = 0; j < sz; j++) {
                                    if ((sections[j].VirtualAddress == directories[i]->VirtualAddress) ||
                                        ((directories[i]->VirtualAddress >= sections[j].VirtualAddress) &&
                                         (directories[i]->VirtualAddress < sections[j].VirtualAddress + sections[j].SizeOfRawData))) {
                                        entries.Add(Tuple.Create(
                                            (IntPtr)(void*)directories[i],
                                            (IntPtr)(void*)&sections[j],
                                            (IMAGE_DIRECTORY_ENTRY)i));
                                        break;
                                        }
                                    }
                                }
                            }

                        foreach(var i in entries) {
                            Load(mapping, (IMAGE_DATA_DIRECTORY*)i.Item1.ToPointer(), (IMAGE_SECTION_HEADER*)i.Item2.ToPointer(), i.Item3, machine, rvami);
                            }
                        }

                    //Sections = LoadSections(mapping, source, sections);
                    }
                return true;
                }
            return false;
            }
        #endregion
        #region M:Load(IMAGE_ROM_OPTIONAL_HEADER*,Int64):IMAGE_DATA_DIRECTORY*[]
        private unsafe IMAGE_DATA_DIRECTORY*[] Load(IMAGE_ROM_OPTIONAL_HEADER* source, Int64 size)
            {
            throw new NotImplementedException();
            }
        #endregion
        #region M:Load(IMAGE_OPTIONAL_HEADER32*,Int64):IMAGE_DATA_DIRECTORY*[]
        private unsafe IMAGE_DATA_DIRECTORY*[] Load(IMAGE_OPTIONAL_HEADER32* source, Int64 size) {
            var r = new IMAGE_DATA_DIRECTORY*[source->NumberOfRvaAndSizes];
            source++;
            var entries = (IMAGE_DATA_DIRECTORY*)source;
            for (var i = 0; i < r.Length; i++) {
                r[i] = &entries[i];
                }
            return r;
            }
        #endregion
        #region M:Load(IMAGE_OPTIONAL_HEADER64*,Int64):IMAGE_DATA_DIRECTORY*[]
        protected virtual unsafe IMAGE_DATA_DIRECTORY*[] Load(IMAGE_OPTIONAL_HEADER64* source, Int64 size) {
            var r = new IMAGE_DATA_DIRECTORY*[source->NumberOfRvaAndSizes];
            source++;
            var entries = (IMAGE_DATA_DIRECTORY*)source;
            for (var i = 0; i < r.Length; i++) {
                r[i] = &entries[i];
                }
            return r;
            }
        #endregion
        #region M:Load(Byte*,IMAGE_DATA_DIRECTORY,IMAGE_SECTION_HEADER,IMAGE_DIRECTORY_ENTRY,IMAGE_FILE_MACHINE,RVA)
        private unsafe void Load(Byte* source, IMAGE_DATA_DIRECTORY* directory, IMAGE_SECTION_HEADER* section, IMAGE_DIRECTORY_ENTRY index, IMAGE_FILE_MACHINE machine, RVA rvami) {
            var address = source + (Int64)section->PointerToRawData - (Int64)section->VirtualAddress;
            switch (index) {
                case IMAGE_DIRECTORY_ENTRY.IMAGE_DIRECTORY_ENTRY_IMPORT:    { Load(address, (IMAGE_IMPORT_DIRECTORY*)(address + directory->VirtualAddress));    } break;
                case IMAGE_DIRECTORY_ENTRY.IMAGE_DIRECTORY_ENTRY_EXCEPTION: { Load(address, (IMAGE_RUNTIME_FUNCTION_ENTRY*)(address + directory->VirtualAddress), directory->Size, machine, rvami); } break;
                case IMAGE_DIRECTORY_ENTRY.IMAGE_DIRECTORY_ENTRY_EXPORT:    { Load(address, (IMAGE_EXPORT_DIRECTORY*)(address + directory->VirtualAddress), section); } break;
                case IMAGE_DIRECTORY_ENTRY.IMAGE_DIRECTORY_ENTRY_RESOURCE:
                    {
                    var r = Load(source, address + (Int64)directory->VirtualAddress, (IMAGE_RESOURCE_DIRECTORY*)(address + directory->VirtualAddress), section, null);
                    if (r != null) {
                        var descriptor = r.FirstOrDefault(i => i.Identifier.Identifier == (Int32)IMAGE_RESOURCE_TYPE.RT_STRING);
                        if (descriptor != null) {
                            /* Reorganize RT_STRING to RT_STRING->LangId */
                            var L = new Dictionary<Int32, ResourceStringTableDescriptor>();
                            foreach (var i in descriptor.Resources) {
                                foreach (var j in i.Resources.OfType<ResourceStringTableDescriptor>()) {
                                    #region DEBUG
                                    Debug.Assert(j.Identifier.Identifier != null);
                                    #endregion
                                    ResourceStringTableDescriptor table;
                                    if (!L.TryGetValue((Int32)j.Identifier.Identifier, out table)) {
                                        L.Add((Int32)j.Identifier.Identifier, table = new ResourceStringTableDescriptor(descriptor, j.Identifier){ Level = IMAGE_RESOURCE_LEVEL.LEVEL_LANGUAGE });
                                        }
                                    table.Merge(j);
                                    }
                                }
                            descriptor.Resources.Clear();
                            descriptor.AddRange(L.Values);
                            }
                        }
                    Resources = r;
                    }
                    break;
                }
            }
        #endregion
        #region M:Load(Byte*,IMAGE_IMPORT_DIRECTORY*)
        private unsafe void Load(Byte* address, IMAGE_IMPORT_DIRECTORY* source) {
            if (ImportLibraryReferences != null) { throw new InvalidOperationException(); }
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var r = new List<ImportLibraryReference>();
            var header = source;
            while ((header->ImportAddressTable != 0) && (header->ImportLookupTable != 0)) {
                var library = (header->Name != 0)
                    ? GetString(Encoding.ASCII, address + (Int64)header->Name)
                    : null;
                var symbols = new HashSet<ImportSymbolDescriptor>();
                symbols.UnionWith(LoadImportTable(address, header->ImportLookupTable));
                symbols.UnionWith(LoadImportTable(address, header->ImportAddressTable));
                r.Add(new ImportLibraryReference(library, symbols));
                header++;
                }
            ImportLibraryReferences = new ReadOnlyCollection<ImportLibraryReference>(r.OrderBy(i => i).ToArray());
            }
        #endregion
        #region M:LoadImportTable(Byte*,Int64):IEnumerable<ImportSymbolDescriptor>
        private unsafe IEnumerable<ImportSymbolDescriptor> LoadImportTable(Byte* address, Int64 offset) {
            var r = address + offset;
            var target = new List<ImportSymbolDescriptor>();
            for (;;) {
                var ordinalnumber = -1;
                var name = 0UL;
                if (Is64Bit) {
                    var i = *(UInt64*)r;
                    if (i == 0) { break; }
                    if ((i & 0x8000000000000000) == 0x8000000000000000) {
                        ordinalnumber = (UInt16)(i & 0xFFFF);
                        }
                    else
                        {
                        name = i & 0x7FFFFFFFFFFFFFFF;
                        }
                    r += 8;
                    }
                else
                    {
                    var i = *(UInt32*)r;
                    if (i == 0) { break; }
                    if ((i & 0x80000000) == 0x80000000) {
                        ordinalnumber = (UInt16)(i & 0xFFFF);
                        }
                    else
                        {
                        name = i & 0x7FFFFFFF;
                        }
                    r += 4;
                    }
                if (ordinalnumber > 0)
                    {
                    target.Add(new ImportSymbolDescriptor(ordinalnumber));
                    }
                else if (name != 0)
                    {
                    var i = address + (Int64)name;
                    target.Add(new ImportSymbolDescriptor(GetString(Encoding.ASCII, i + 2), (Int32)(*(UInt16*)i)));
                    }
                }
            return target;
            }
        #endregion
        #region M:Load(Byte*,IMAGE_EXPORT_DIRECTORY*)
        private unsafe void Load(Byte* address, IMAGE_EXPORT_DIRECTORY* source, IMAGE_SECTION_HEADER* section) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            var ordinaltable     = (UInt16*)(address + (Int64)source->OrdinalTableRVA);
            var namepointertable = (UInt32*)(address + (Int64)source->NamePointerRVA);
            var exportaddresses  = (UInt32*)(address + (Int64)source->ExportAddressTableRVA);
            var left = section->VirtualAddress;
            var right = section->VirtualAddress + section->VirtualSize;
            var r = new ExportSymbolDescriptor[source->AddressTableEntries];
            for (var i = 0; i < source->NumberOfNamePointers; ++i) {
                var ordinal = ordinaltable[i];
                var name    = GetString(Encoding.ASCII, (Int64)namepointertable[i] + address);
                r[ordinal] = new ExportSymbolDescriptor(name, (Int32)(ordinal + source->OrdinalBase));
                }
            for (var i = 0; i < source->AddressTableEntries; ++i) {
                var n = exportaddresses[i];
                if (n == 0) { continue; }
                if ((n >= left) && (n <= right)) {
                    /* Forwarder RVA */
                    if (r[i] != null) {
                        r[i].EntryPointName = GetString(Encoding.ASCII, (Int64)n + address);
                        }
                    else
                        {
                        r[i] = new ExportSymbolDescriptor((Int32)(i + source->OrdinalBase), GetString(Encoding.ASCII, (Int64)n + address));
                        }
                    }
                else
                    {
                    /* Export RVA */
                    if (r[i] != null) {
                        r[i].EntryPoint = n;
                        }
                    else
                        {
                        r[i] = new ExportSymbolDescriptor((Int32)(i + source->OrdinalBase), n);
                        }
                    }
                }
            var target = r.Where(i => i != null).ToArray();
            ExportDescriptors = target;
            return;
            }
        #endregion
        #region M:Load(Byte*,Byte*,IMAGE_RESOURCE_DIRECTORY*,IMAGE_SECTION_HEADER*,ResourceDescriptor):IList<ResourceDescriptor>
        private unsafe IList<ResourceDescriptor> Load(Byte* baseaddress, Byte* address, IMAGE_RESOURCE_DIRECTORY* directory, IMAGE_SECTION_HEADER* section, ResourceDescriptor owner) {
            var r = new List<ResourceDescriptor>();
            var source = (IMAGE_DIRECTORY_ENTRY_RESOURCE*)(directory + 1);
            for (var i = 0; i < directory->NumberOfNamedEntries + directory->NumberOfIdEntries; i++) {
                var resource = new ResourceDescriptor(owner, new ResourceIdentifier(address, source));
                if ((source->DataEntryOffset & 0x80000000) == 0x80000000) {
                    resource.AddRange(Load(
                        baseaddress,
                        address,
                        (IMAGE_RESOURCE_DIRECTORY*)(address + (source->DataEntryOffset & 0x7FFFFFFF)),
                        section, resource));
                    }
                else
                    {
                    resource = Load(baseaddress, (IMAGE_RESOURCE_DATA_ENTRY*)(address + source->DataEntryOffset), section, resource);
                    }
                r.Add(resource);
                source++;
                }
            return r;
            }
        #endregion
        #region M:Load(Byte*,IMAGE_RESOURCE_DATA_ENTRY*,IMAGE_SECTION_HEADER*,ResourceDescriptor):ResourceDescriptor
        private unsafe ResourceDescriptor Load(Byte* baseaddress, IMAGE_RESOURCE_DATA_ENTRY* source, IMAGE_SECTION_HEADER* section, ResourceDescriptor descriptor) {
            var bytes = new Byte[source->Size];
            ResourceDescriptor r = null;
            Marshal.Copy((IntPtr)(void*)(baseaddress + (Int64)source->OffsetToData - (Int64)section->VirtualAddress + (Int64)section->PointerToRawData),bytes, 0, bytes.Length);
            if (descriptor.Level == IMAGE_RESOURCE_LEVEL.LEVEL_LANGUAGE) {
                if (descriptor.Owner.Owner.Identifier.Identifier != null) {
                    switch ((IMAGE_RESOURCE_TYPE)descriptor.Owner.Owner.Identifier.Identifier) {
                        case IMAGE_RESOURCE_TYPE.RT_MESSAGETABLE: { r = new ResourceMessageTableDescriptor(descriptor.Owner, descriptor.Identifier, bytes); } break;
                        case IMAGE_RESOURCE_TYPE.RT_STRING:       { r = new ResourceStringTableDescriptor(descriptor.Owner, descriptor.Identifier, bytes); } break;
                        }
                    }
                else
                    {
                    switch (descriptor.Owner.Owner.Identifier.Name) {
                        #region MUI
                        case "MUI"           :
                            {
                            #if FEATURE_MUI
                            // TODO: Переработать механизм загрузки MUI на ассинхронный вариант с уведомлением
                            var mui = new ResourceMUIDescriptor(descriptor.Owner, descriptor.Identifier, bytes);
                            r = mui;
                            if (mui.IsUltimateFallbackLocationExternal) {
                                /* Try to find appropriate MUI file */
                                MUI = LoadMUI(mui.UltimateFallbackLanguage) ??
                                      LoadMUI(CultureInfo.CurrentUICulture);
                                }
                            else
                                {
                                Language = mui.Language;
                                }
                            #endif
                            }
                            break;
                        #endregion
                        #region TYPELIB
                        case "TYPELIB":
                            {
                            Trace.WriteLine("TYPELIB");
                            #if FEATURE_TYPELIB
                            fixed (Byte* memory = bytes)
                                {
                                TypeLibrary = (TypeLibraryDescriptor)Scope.LoadObject(memory, bytes.Length).GetService(typeof(TypeLibraryDescriptor));
                                #if DEBUG
                                //File.WriteAllBytes($"{TypeLibrary.Name}.tlb", bytes);
                                #endif
                                }
                            #endif
                            }
                            break;
                        #endregion
                        }
                    }
                }
            r = r ?? new ResourceDescriptor(descriptor.Owner, descriptor.Identifier, bytes);
            r.CodePage = source->CodePage;
            return r;
            }
        #endregion

        #region M:GetString(Encoding,Byte*):String
        private static unsafe String GetString(Encoding encoding, Byte* source) {
            if (source == null) { return null; }
            var c = 0;
            for (;;++c) {
                if (source[c] == 0) { break; }
                }
            var r = new Byte[c];
            for (var i = 0;i < c;++i) {
                r[i] = source[i];
                }
            return encoding.GetString(r);
            }
        #endregion
        #region M:GetString(Encoding,Byte*,Int64):String
        private static unsafe String GetString(Encoding encoding, Byte* source, Int64 size) {
            return encoding.GetString(GetBytes(source, size));
            }
        #endregion
        #region M:GetString(Encoding,Byte*,UInt32):String
        internal static unsafe String GetString(Encoding encoding, Byte* source, UInt32 size) {
            return GetString(encoding, source, (Int64)size);
            }
        #endregion
        #region M:GetBytes(Byte*,Int64):Byte[]
        private static unsafe Byte[] GetBytes(Byte* source, Int64 size)
            {
            if (source == null) { return null; }
            var r = new Byte[size];
            for (var i = 0;i < size;++i) {
                r[i] = source[i];
                }
            return r;
            }
        #endregion
        #region M:GetBytes(Byte*,UInt32):Byte[]
        internal static unsafe Byte[] GetBytes(Byte* source, UInt32 size)
            {
            return GetBytes(source, (Int64)size);
            }
        #endregion
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
        #region M:ReadInt32([Ref]Byte*):Int32
        private static unsafe Int32 ReadInt32(ref Byte* source)
            {
            var r = *((Int32*)source);
            source += sizeof(Int32);
            return r;
            }
        #endregion
        #region M:ReadUInt32(Byte*):UInt32
        private static unsafe UInt32 ReadUInt32(Byte* source)
            {
            var r = *((UInt32*)source);
            return r;
            }
        #endregion
        #region M:ReadBytes([Ref]Byte*,Int32):Int32
        private static unsafe Byte[] ReadBytes(ref Byte* source, Int32 count)
            {
            if (source == null) { return null; }
            var r = new Byte[count];
            for (var i = 0;i < count;++i) {
                r[i] = source[i];
                }
            source += count;
            return r;
            }
        #endregion

        internal CommonObjectFileSource(MetadataScope scope)
            : base(scope)
            {
            Sections = EmptyList<CommonObjectFileSection>.Value;
            SymbolTable = EmptyList<ISymbol>.Value;
            }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct IMAGE_RUNTIME_FUNCTION_ENTRY
            {
            [FieldOffset(0)] public  readonly UInt32 BeginAddress;
            [FieldOffset(4)] public  readonly UInt32 EndAddress;
            [FieldOffset(8)] public  readonly UInt32 UnwindInfoAddress;
            [FieldOffset(8)] private readonly UInt32 UnwindData;
            }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct IMAGE_MIPS_RUNTIME_FUNCTION_ENTRY
            {
            [FieldOffset( 0)] public readonly UInt32 BeginAddress;
            [FieldOffset( 4)] public readonly UInt32 EndAddress;
            [FieldOffset( 8)] public readonly UInt32 ExceptionHandler;
            [FieldOffset(12)] public readonly UInt32 HandlerData;
            [FieldOffset(16)] public readonly UInt32 PrologEndAddress;
            }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct IMAGE_CE_RUNTIME_FUNCTION_ENTRY
            {
            [FieldOffset(0)] private readonly UInt32 BeginAddress;
            [FieldOffset(4)] private readonly UInt32 UnwindData;
            }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct IMAGE_ARM_RUNTIME_FUNCTION_ENTRY
            {
            [FieldOffset(0)] public readonly UInt32 BeginAddress;
            [FieldOffset(4)] public readonly UInt32 UnwindData;
            }
        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct IMAGE_ARM64_RUNTIME_FUNCTION_ENTRY
            {
            [FieldOffset(0)] public readonly UInt32 BeginAddress;
            [FieldOffset(4)] public readonly UInt32 UnwindData;
            }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct IMAGE_ALPHA_RUNTIME_FUNCTION_ENTRY
            {
            [FieldOffset( 0)] public readonly UInt32 BeginAddress;
            [FieldOffset( 4)] public readonly UInt32 EndAddress;
            [FieldOffset( 8)] public readonly UInt32 ExceptionHandler;
            [FieldOffset(12)] public readonly UInt32 HandlerData;
            [FieldOffset(16)] public readonly UInt32 PrologEndAddress;
            }

        [StructLayout(LayoutKind.Explicit, Pack = 1)]
        private struct IMAGE_ALPHA64_RUNTIME_FUNCTION_ENTRY
            {
            [FieldOffset( 0)] private readonly UInt64 BeginAddress;
            [FieldOffset( 4)] private readonly UInt64 EndAddress;
            [FieldOffset( 8)] private readonly UInt64 ExceptionHandler;
            [FieldOffset(12)] private readonly UInt64 HandlerData;
            [FieldOffset(16)] private readonly UInt64 PrologEndAddress;
            }

        private class IMAGE_RUNTIME_FUNCTION_RECORD
            {
            public UInt64 BeginAddress { get; }
            public IMAGE_RUNTIME_FUNCTION_RECORD(UInt64 beginaddress)
                {
                BeginAddress = beginaddress;
                }
            }

        private class IMAGE_ARM64_RUNTIME_FUNCTION_RECORD : IMAGE_RUNTIME_FUNCTION_RECORD
            {
            public UInt32 FunctionLength { get; }
            public UInt16 FrameSize      { get; }
            public Byte RegF { get; }
            public Byte RegI { get; }
            public Byte CR   { get; }
            public Boolean H { get; }
            public Boolean IsPacked { get; }
            public IMAGE_ARM64_RUNTIME_FUNCTION_RECORD(UInt64 beginaddress, UInt32 packeddata)
                : base(beginaddress)
                {
                var flag = packeddata & 0x3;
                if ((flag == 1) || (flag == 2)) {
                    IsPacked = true;
                    FunctionLength = (UInt32)((packeddata >>  2) & 0x07FF);
                    RegF           = (Byte)((packeddata >> 13) & 0x0007);
                    RegI           = (Byte)((packeddata >> 16) & 0x000F);
                    H              = ((packeddata >> 20) & 0x0001) == 1;
                    CR             = (Byte)((packeddata >> 21) & 0x0003);
                    FrameSize      = (UInt16)((packeddata >> 23) & 0x01FF);
                    }
                else { throw new ArgumentOutOfRangeException(nameof(packeddata)); }
                }

            public unsafe IMAGE_ARM64_RUNTIME_FUNCTION_RECORD(UInt64 beginaddress, Byte* xdata)
                :base(beginaddress)
                {
                if (xdata == null) { throw new ArgumentNullException(nameof(xdata)); }
                var r = *(UInt32*)xdata;
                FunctionLength = (r & 0x3FFFF) << 2;
                var ver = (r >> 18) & 0x3;
                var X = (r >> 20) & 0x01;
                var E = (r >> 21) & 0x01;
                var EpilogCount = (r >> 22) & 0x1F;
                var CodeWords   = (r >> 27) & 0x1F;
                if ((EpilogCount == 0) && (CodeWords == 0)) {
                    xdata += 4;
                    r = *(UInt32*)xdata;
                    EpilogCount = r & 0xFFFF;
                    CodeWords = (r >> 16) & 0xFF;
                    }
                xdata += 4;
                r = *(UInt32*)xdata;
                var EpilogStartOffset = (r & 0x3FFFF);
                var EpilogStartIndex  = (r >> 22) & 0x03FF;
                }

            public override String ToString()
                {
                return String.Format("{0:X8}:{1}", BeginAddress, IsPacked ? "Y" : "N");
                }
            }

        private enum IMAGE_ARM_RUNTIME_FUNCTION_RETURN : byte
            {
            ReturnViaPop = 0,
            ReturnByUsing16bitBranch,
            ReturnByUsing32bitBranch,
            NoEpilogueAtAll
            }

        private class IMAGE_ARM_RUNTIME_FUNCTION_RECORD : IMAGE_RUNTIME_FUNCTION_RECORD
            {
            public UInt32 FunctionLength { get; }
            public IMAGE_ARM_RUNTIME_FUNCTION_RETURN Return { get; }
            public UInt16 StackAdjust    { get; }
            public Byte Reg { get; }
            public Byte H { get; }
            public Byte R { get; }
            public Byte L { get; }
            public Byte C { get; }
            public Byte X { get; }
            public Byte E { get; }
            public Byte F { get; }
            public Byte PF { get; } /* Prologue folding */
            public Byte EF { get; } /* Epilogue folding */
            public Boolean IsPacked { get; }
            public UInt32 EpilogCount { get; }
            public UInt32 CodeWords   { get; }
            public UInt32 EpilogStartOffset { get; }
            public UInt32 EpilogStartIndex { get; }
            public Byte   Version { get; }
            public Byte   Condition { get; }
            public IList<String> PrologUnwindCodes { get; }
            public IList<String> EpilogUnwindCodes { get; }
            public IMAGE_ARM_RUNTIME_FUNCTION_RECORD(UInt64 beginaddress, UInt32 packeddata)
                : base(beginaddress)
                {
                PrologUnwindCodes = new List<String>();
                EpilogUnwindCodes = new List<String>();
                var flag = packeddata & 0x3;
                if ((flag == 1) || (flag == 2)) {
                    IsPacked = true;
                    FunctionLength = (packeddata >> 2) & 0x07FF;
                    Return = (IMAGE_ARM_RUNTIME_FUNCTION_RETURN)((packeddata >> 13) & 0x03);
                    H = (Byte)((packeddata >> 15) & 0x01);
                    R = (Byte)((packeddata >> 19) & 0x01);
                    L = (Byte)((packeddata >> 20) & 0x01);
                    C = (Byte)((packeddata >> 21) & 0x01);
                    Reg = (Byte)((packeddata >> 16) & 0x07);
                    StackAdjust = (UInt16)((packeddata >> 22) & 0x03FF);
                    if (StackAdjust >= 0x3F4) {
                        PF = (Byte)((StackAdjust & 0x04) >> 2);
                        EF = (Byte)((StackAdjust & 0x08) >> 3);
                        }
                    var S = (~StackAdjust) & 3;
                    var ipushed = "(none)";
                    var vpushed = "(none)";
                    var X = 0;
                    switch ((C << 3) | (L << 2) | (R << 1) | PF)
                        {
                        #region [r4-rN       ][none ]
                        case 0:
                            {
                            ipushed = "r4-r{0}";
                            X = Reg + 1;
                            }
                            break;
                        #endregion
                        #region [rS-rN       ][none ]
                        case 1:
                            {
                            ipushed = "r{1}-r{0}";
                            X = Reg + 5 - S;
                            }
                            break;
                        #endregion
                        #region [none        ][d8-dE]
                        case 2:
                            {
                            vpushed = "d8-dE";
                            }
                            break;
                        #endregion
                        #region [rS-r3       ][d8-dE]
                        case 3:
                            {
                            ipushed = "r{1}-r3";
                            vpushed = "d8-d{0}";
                            X = 4 - ((~StackAdjust) & 3);
                            }
                            break;
                        #endregion
                        #region [r4-rN,lr    ][none ]
                        case 4:
                            {
                            ipushed = "r4-r{0},lr";
                            X = Reg + 1;
                            }
                            break;
                        #endregion
                        #region [rS-rN,lr    ][none ]
                        case 5:
                            {
                            ipushed = "r{1}-r{0},lr";
                            X = Reg + 5 - ((~StackAdjust) & 3);
                            }
                            break;
                        #endregion
                        #region [lr          ][d8-dE]
                        case 6:
                            {
                            ipushed = "lr";
                            vpushed = "d8-d{0}";
                            }
                            break;
                        #endregion
                        #region [rS-r3,lr    ][d8-dE]
                        case 7:
                            {
                            ipushed = "r{1}-r3,lr";
                            vpushed = "d8-d{0}";
                            X = 4 - ((~StackAdjust) & 3);
                            }
                            break;
                        #endregion
                        #region [r4-rN,r11   ][none ]
                        case 8:
                            {
                            ipushed = "r4-r{0},r11";
                            X = Reg + 1;
                            }
                            break;
                        #endregion
                        #region [rS-rN,r11   ][none ]
                        case 9:
                            {
                            ipushed = "r{1}-r{0},r11";
                            X = Reg + 5 - ((~StackAdjust) & 3);
                            }
                            break;
                        #endregion
                        #region [r11         ][d8-dE]
                        case 10:
                            {
                            ipushed = "r11";
                            vpushed = "d8-d{0}";
                            }
                            break;
                        #endregion
                        #region [rS-r3,r11   ][d8-dE]
                        case 11:
                            {
                            ipushed = "r{1}-r3,r11";
                            vpushed = "d8-d{0}";
                            X = 4 - ((~StackAdjust) & 3);
                            }
                            break;
                        #endregion
                        #region [r4-rN,r11,lr][none ]
                        case 12:
                            {
                            ipushed = "r4-r{0},r11,lr";
                            X = Reg + 1;
                            }
                            break;
                        #endregion
                        #region [rS-rN,r11,lr][none ]
                        case 13:
                            {
                            ipushed = "r{1}-r{0},r11,lr";
                            X = Reg + 5 - ((~StackAdjust) & 3);
                            }
                            break;
                        #endregion
                        #region [r11,lr      ][d8-dE]
                        case 14:
                            {
                            ipushed = "r11,lr";
                            vpushed = "d8-d{0}";
                            }
                            break;
                        #endregion
                        #region [rS-r3,r11,lr][d8-dE]
                        case 15:
                            {
                            ipushed = "r{1}-r3,r11,lr";
                            vpushed = "d8-d{0}";
                            X = 4 - ((~StackAdjust) & 3);
                            }
                            break;
                        #endregion
                        }
                    ipushed = String.Format(ipushed, Reg + 4, (~StackAdjust) & 3);
                    vpushed = String.Format(vpushed, Reg + 8);
                    if (H == 1) { PrologUnwindCodes.Add("push {r0-r3}"); }
                    if ((C == 1) || (L == 1) || (R == 0) || (PF == 1))   { PrologUnwindCodes.Add(String.Format("push {{{0}}}", ipushed)); }
                    if ((C == 1) && ((L == 0) || (R == 1) || (PF == 0))) { PrologUnwindCodes.Add("mov r11,sp"); }
                    if ((C == 1) && ((L == 1) || (R == 0) || (PF == 1))) { PrologUnwindCodes.Add(String.Format("add r11,sp,#{0}", X << 2)); }
                    if ((R == 1) && (Reg != 7)) { PrologUnwindCodes.Add(String.Format("vpush {{{0}}}", vpushed)); }
                    if ((StackAdjust != 0) && (PF == 0)) { PrologUnwindCodes.Add(String.Format("sub sp,sp,#{0}", X << 2)); }

                    if ((StackAdjust != 0) && (EF == 0)) { EpilogUnwindCodes.Add($"add sp,sp,#{X << 2}"); }
                    if ((R == 1) && (Reg != 7)) { EpilogUnwindCodes.Add(String.Format("vpop {{{0}}}", vpushed)); }
                    if ((C == 1) || ((L == 1) && (H == 0)) || (R == 0) || (EF == 1)) { EpilogUnwindCodes.Add(String.Format("pop {{{0}}}", ipushed)); }
                    if ((H == 1) && (L == 0)) { EpilogUnwindCodes.Add("add sp,sp,#0x10"); }
                    if ((H == 1) && (L == 1)) { EpilogUnwindCodes.Add("ldr pc,[sp],#0x14"); }
                    if (Return == IMAGE_ARM_RUNTIME_FUNCTION_RETURN.ReturnByUsing16bitBranch) { EpilogUnwindCodes.Add("bx reg"); }
                    if (Return == IMAGE_ARM_RUNTIME_FUNCTION_RETURN.ReturnByUsing32bitBranch) { EpilogUnwindCodes.Add("b address"); }
                    if (PrologUnwindCodes.Count > 0) {
                        Debug.Print("Prolog:");
                        Debug.Print(String.Join("\r\n", PrologUnwindCodes));
                        }
                    if (EpilogUnwindCodes.Count > 0) {
                        Debug.Print("Epilog:");
                        Debug.Print(String.Join("\r\n", EpilogUnwindCodes));
                        }
                    return;
                    }
                else { throw new ArgumentOutOfRangeException(nameof(packeddata)); }
                }

            public unsafe IMAGE_ARM_RUNTIME_FUNCTION_RECORD(UInt64 beginaddress, UInt32* xdata)
                :base(beginaddress)
                {
                if (xdata == null) { throw new ArgumentNullException(nameof(xdata)); }
                var r = *xdata;
                FunctionLength = (r & 0x3FFFF) << 2;
                Version = (Byte)((r >> 18) & 0x3);
                X = (Byte)((r >> 20) & 0x01);
                E = (Byte)((r >> 21) & 0x01);
                F = (Byte)((r >> 22) & 0x01);
                EpilogCount = ((r >> 23) & 0x1F);
                CodeWords =   ((r >> 28) & 0x0F);
                if ((EpilogCount == 0) && (CodeWords == 0)) {
                    xdata++;
                    r = *(UInt32*)xdata;
                    EpilogCount = r & 0xFFFF;
                    CodeWords   = (r >> 16) & 0xFF;
                    }
                xdata++;
                r = *xdata;
                EpilogStartOffset = (r & 0x3FFFF);
                EpilogStartIndex  = ((r >> 24)) & 0xFF;
                Condition = (Byte)((r >> 20) & 0xF);
                }

            public override String ToString()
                {
                return String.Format("{0:X8}:{1}", BeginAddress, IsPacked ? "Y" : "N");
                }
            }

        private unsafe void Load(Byte* address, IMAGE_RUNTIME_FUNCTION_ENTRY* entries, Int64 size, IMAGE_FILE_MACHINE machine, RVA rvami) {
            return;
            switch (machine) {
                #region MIPS16,MIPSFPU
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_MIPS16:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_MIPSFPU:
                    {
                    Load(address, (IMAGE_MIPS_RUNTIME_FUNCTION_ENTRY*)entries, size);
                    }
                    return;
                #endregion
                #region POWERPC,POWERPCFP,SH3,SH3DSP,SH4,CEE,CEF
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_POWERPC:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_POWERPCFP:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_SH3:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_SH3DSP:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_SH4:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_CEE:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_CEF:
                    {
                    Load(address, (IMAGE_CE_RUNTIME_FUNCTION_ENTRY*)entries, size);
                    }
                    return;
                #endregion
                #region ALPHA
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_ALPHA:
                    {
                    Load(address, (IMAGE_ALPHA_RUNTIME_FUNCTION_ENTRY*)entries, size);
                    }
                    return;
                #endregion
                #region ALPHA64
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_ALPHA64:
                    {
                    Load(address, (IMAGE_ALPHA64_RUNTIME_FUNCTION_ENTRY*)entries, size);
                    }
                    return;
                #endregion
                #region ARM,ARMNT
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_ARM:
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_ARMNT:
                    {
                    Load(address, (IMAGE_ARM_RUNTIME_FUNCTION_ENTRY*)entries, size, rvami);
                    }
                    return;
                #endregion
                #region ARM64
                case IMAGE_FILE_MACHINE.IMAGE_FILE_MACHINE_ARM64:
                    {
                    Load(address, (IMAGE_ARM64_RUNTIME_FUNCTION_ENTRY*)entries, size, rvami);
                    }
                    return;
                #endregion
                }
            var sz = sizeof(IMAGE_RUNTIME_FUNCTION_ENTRY);
            while (size >= sz) {
                Debug.Print("{0:X8}:{1:X8}:{2:X8}", entries->BeginAddress, entries->EndAddress, entries->UnwindInfoAddress);
                entries++;
                size -= sz;
                }
            return;
            }

        private unsafe void Load(Byte* address, IMAGE_MIPS_RUNTIME_FUNCTION_ENTRY* entries, Int64 size) {
            throw new NotImplementedException();
            }

        private unsafe void Load(Byte* address, IMAGE_CE_RUNTIME_FUNCTION_ENTRY* entries, Int64 size) {
            throw new NotImplementedException();
            }

        private unsafe void Load(Byte* address, IMAGE_ALPHA_RUNTIME_FUNCTION_ENTRY* entries, Int64 size) {
            throw new NotImplementedException();
            }

        private unsafe void Load(Byte* address, IMAGE_ALPHA64_RUNTIME_FUNCTION_ENTRY* entries, Int64 size) {
            throw new NotImplementedException();
            }

        private unsafe IEnumerable<IMAGE_RUNTIME_FUNCTION_RECORD> Load(Byte* address, IMAGE_ARM_RUNTIME_FUNCTION_ENTRY* entries, Int64 size, RVA rvami) {
            var r = new List<IMAGE_RUNTIME_FUNCTION_RECORD>();
            var sz = sizeof(IMAGE_ARM64_RUNTIME_FUNCTION_ENTRY);
            while (size >= sz) {
                var flag = entries->UnwindData & 0x3;
                r.Add((flag == 0)
                    ? new IMAGE_ARM_RUNTIME_FUNCTION_RECORD(entries->BeginAddress, (UInt32*)rvami((entries->UnwindData)))
                    : new IMAGE_ARM_RUNTIME_FUNCTION_RECORD(entries->BeginAddress, entries->UnwindData));
                entries++;
                size -= sz;
                }
            return r;
            }

        private unsafe IEnumerable<IMAGE_RUNTIME_FUNCTION_RECORD> Load(Byte* address, IMAGE_ARM64_RUNTIME_FUNCTION_ENTRY* entries, Int64 size, RVA rvami) {
            var r = new List<IMAGE_RUNTIME_FUNCTION_RECORD>();
            var sz = sizeof(IMAGE_ARM64_RUNTIME_FUNCTION_ENTRY);
            while (size >= sz) {
                var flag = entries->UnwindData & 0x3;
                r.Add((flag == 0)
                    ? new IMAGE_ARM64_RUNTIME_FUNCTION_RECORD(entries->BeginAddress, (Byte*)rvami((entries->UnwindData >> 2)))
                    : new IMAGE_ARM64_RUNTIME_FUNCTION_RECORD(entries->BeginAddress, entries->UnwindData));
                entries++;
                size -= sz;
                }
            return r;
            }

        public override Object GetService(Type type)
            {
            var r = base.GetService(type);
            if (r != null) { return r; }
            if (type == typeof(ITypeLibraryDescriptor)) { return TypeLibrary; }
            if (type == typeof(TypeLibraryDescriptor))  { return TypeLibrary; }
            return null;
            }

        [DllImport("kernel32.dll", ExactSpelling = true, CharSet = CharSet.None)] private static extern UInt16 GetACP();

        #region M:LoadSections(Byte*,IMAGE_FILE_HEADER*,IMAGE_SECTION_HEADER*):IList<Section>
        private unsafe IList<CommonObjectFileSection> LoadSections(Byte* mapping, IMAGE_FILE_HEADER* header, IMAGE_SECTION_HEADER* sections) {
            var r = new List<CommonObjectFileSection>();
            for (var i = 0; i < header->NumberOfSections; ++i) {
                r.Add(LoadSection(i, mapping, &sections[i]));
                }
            return new ReadOnlyCollection<CommonObjectFileSection>(r);
            }
        #endregion
        #region M:LoadSection(IMAGE_SECTION_HEADER*)
        private unsafe CommonObjectFileSection LoadSection(Int32 index, Byte* mapping, IMAGE_SECTION_HEADER* section) {
            switch (section->ToString()) {
                #region .drectve
                case ".drectve":
                    {
                    if (section->Characteristics.HasFlag(IMAGE_SCN.IMAGE_SCN_LNK_INFO)) {
                        return new DirectiveSection(this, index, mapping, section);
                        }
                    }
                    break;
                #endregion
                #region .debug$S
                case ".debug$S":
                    {
                    /* Check for CodeView version */
                    switch (*(CV_SIGNATURE*)(Int32*)(section->PointerToRawData + mapping)) {
                        case CV_SIGNATURE.CV_SIGNATURE_C6:  { return new MSC06CodeViewSection(this, index, mapping, section); }
                        case CV_SIGNATURE.CV_SIGNATURE_C7:  { return new MSC07CodeViewSection(this, index, mapping, section); }
                        case CV_SIGNATURE.CV_SIGNATURE_C11: { return new MSC11CodeViewSection(this, index, mapping, section); }
                        case CV_SIGNATURE.CV_SIGNATURE_C13: { return new MSC13CodeViewSection(this, index, mapping, section); }
                        }
                    }
                    break;
                #endregion
                case ".debug$T":
                case ".text"   :
                case ".text$yc":
                case "/1086"   :
                case ".cormeta":
                default:
                    {
                    }
                    break;
                }
            return new CommonObjectFileSection(this, index, mapping, section);
            }
        #endregion
        #region M:LoadSymbolTable(Byte*,UInt32,UInt32)
        private unsafe IList<ISymbol> LoadSymbolTable(Byte* mapping, UInt32 numberOfSymbols, UInt32 pointerToSymbolTable) {
            var R = new ISymbol[numberOfSymbols];
            var I = new Int32[numberOfSymbols];
            var T = (IMAGE_SYMBOL*)(mapping + pointerToSymbolTable);
            var S = LoadStringTable((Byte*)(T + numberOfSymbols));
            for (var i = 0; i < numberOfSymbols;) {
                Symbol parent;
                if (T->Short == 0) {
                    var index = T->Long;
                    R[i] = parent = new Symbol(T, S[index]);
                    }
                else
                    {
                    R[i] = parent = new Symbol(T, GetString(Encoding.UTF8, T->ShortName, 8).Trim());
                    }
                var P = (IMAGE_AUX_SYMBOL*)(Byte*)(T + 1);
                if (T->NumberOfAuxSymbols > 0) {
                    for (var j = 0; j < T->NumberOfAuxSymbols;j++) {
                        i++;
                        switch (parent.StorageClass) {
                            case IMAGE_SYM_CLASS.IMAGE_SYM_CLASS_CLR_TOKEN:
                                {
                                R[i] = new CommonLanguageInfrastructureTokenDefinition((IMAGE_AUX_SYMBOL_TOKEN_DEF*)P);
                                }
                                break;
                            case IMAGE_SYM_CLASS.IMAGE_SYM_CLASS_STATIC:
                                {
                                R[i] = new SectionDefinitionSymbol((IMAGE_AUX_SYMBOL_SECTION_DEF*)P);
                                }
                                break;
                            default:
                                {
                                R[i] = new AuxiliarySymbol(P, parent);
                                }
                                break;
                            }
                        P++;
                        }
                    }
                T = (IMAGE_SYMBOL*)((Byte*)(T + 1) + T->NumberOfAuxSymbols*sizeof(IMAGE_AUX_SYMBOL));
                i++;
                }
            return R;
            }
        #endregion
        #region M:LoadSymbolTable(Byte*,IMAGE_FILE_HEADER*)
        private unsafe IList<ISymbol> LoadSymbolTable(Byte* mapping, IMAGE_FILE_HEADER* header) {
            return LoadSymbolTable(mapping,
                header->NumberOfSymbols,
                header->PointerToSymbolTable);
            }
        #endregion

        private static unsafe IDictionary<Int32,String> LoadStringTable(Byte* source)
            {
            var r = new Dictionary<Int32, String>();
            var LT = source;
            var SZ = ReadInt32(ref source);
            var LS = LT + SZ;
            while (source < LS) {
                r.Add(
                    (Int32)(source - LT),
                    ReadString(Encoding.UTF8, ref source));
                }
            return r;
            }

        public override void WriteJson(JsonWriter writer, JsonSerializer serializer) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            using (writer.ObjectScope(serializer)) {
                writer.WriteValue(serializer, nameof(Machine), Machine.ToString());
                writer.WriteValue(serializer, nameof(Characteristics), Characteristics.ToString());
                if (Sections.Count > 0) {
                    writer.WritePropertyName(nameof(Sections));
                    using (writer.ObjectScope(serializer)) {
                        writer.WriteValue(serializer, nameof(Sections.Count), Sections.Count);
                        writer.WriteValue(serializer, "[Self]", Sections);
                        }
                    }
                if (SymbolTable.Any()) {
                    writer.WritePropertyName(nameof(SymbolTable));
                    using (writer.ObjectScope(serializer)) {
                        writer.WriteValue(serializer, nameof(SymbolTable.Count), SymbolTable.Count);
                        writer.WritePropertyName("[Self]");
                        using (writer.ArrayScope(serializer)) {
                            using (writer.SuspendFormatingScope()) {
                                const Int32 SECT = 2;
                                writer.Formatting = Formatting.None;
                                var widths = new Int32[3];
                                foreach (var symbol in SymbolTable) {
                                    if (symbol is Symbol sI) {
                                        widths[1] = Math.Max(widths[1], sI.StorageClass.ToString().Length);
                                        }
                                    }
                                var i = 0;
                                widths[0] = SymbolTable.Count.ToString("X").Length;
                                widths[SECT] = Sections.Count.ToString("X").Length;
                                foreach (var symbol in SymbolTable) {
                                    break;
                                    if (i == 0) {
                                        //writer.WriteIndent();
                                        }
                                    else
                                        {
                                        writer.Formatting = Formatting.Indented;
                                        }
                                    using (writer.ObjectScope(serializer)) {
                                        writer.Formatting = Formatting.None;
                                        writer.WriteValue(serializer, "Index", i.ToString($"X{widths[0]}"));
                                        if (symbol is Symbol sI) {
                                            String SectionNumber;
                                            String StorageClass = sI.StorageClass.ToString();
                                            writer.WriteValue(serializer, "Value", sI.Value.ToString("X8"));
                                            writer.WriteValue(serializer, "StorageClass", StorageClass);
                                            //writer.WriteIndentSpace(widths[1] - StorageClass.Length);
                                            switch (sI.SectionNumber) {
                                                case IMAGE_SYM_UNDEFINED: { SectionNumber = "IMAGE_SYM_UNDEFINED"; } break;
                                                case IMAGE_SYM_ABSOLUTE:  { SectionNumber = "IMAGE_SYM_ABSOLUTE";  } break;
                                                case IMAGE_SYM_DEBUG:     { SectionNumber = "IMAGE_SYM_DEBUG";     } break;
                                                default: { SectionNumber = sI.SectionNumber.ToString($"X{widths[SECT]}"); } break;
                                                }
                                            writer.WriteValue(serializer, "SectionNumber", SectionNumber);
                                            //writer.WriteIndentSpace(19 - SectionNumber.Length);
                                            }
                                        else if (symbol is CommonLanguageInfrastructureTokenDefinition Sc)
                                            {
                                            writer.WriteValue(serializer, "Type", "{CLR Token Definition}");
                                            //writer.WriteLine(":");
                                            //writer.WriteLine("      Symbol table index:{0:X}", clr.SymbolTableIndex);
                                            }
                                        else if (symbol is SectionDefinitionSymbol Sd)
                                            {
                                            writer.Formatting = Formatting.Indented;
                                            writer.WriteValue(serializer, "(Type)", "{Section Definition}");
                                            writer.WriteValue(serializer, "Length", Sd.Length.ToString("X8"));
                                            writer.WriteValue(serializer, "NumberOfRelocations", Sd.NumberOfRelocations.ToString("X"));
                                            writer.WriteValue(serializer, "NumberOfLineNumbers", Sd.NumberOfLineNumbers.ToString("X"));
                                            writer.WriteValue(serializer, "CheckSum", Sd.CheckSum.ToString("X8"));
                                            writer.WriteValue(serializer, "Number", Sd.Number.ToString("X"));
                                            writer.WriteValue(serializer, "Selection", Sd.Selection.ToString("X2"));
                                            }
                                        }
                                    i++;
                                    }
                                }
                            }
                        }
                    }
                }
            }


        protected internal override void WriteText(Int32 offset, TextWriter writer) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            var IndentString = new String(' ', offset);
            if (Sections.Any()) {
                var i = 1;
                var SI = Sections.Count.ToString("X").Length;
                foreach (var section in Sections) {
                    writer.WriteLine("{0}SECTION HEADER {{{1}}}", IndentString, i.ToString($"X{SI}"));
                    section.WriteText(offset + 2, writer);
                    i++;
                    }
                }
            }

        protected internal override unsafe void WriteXml(XmlWriter writer) {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteStartElement("CommonObjectFile");
            if (Sections.Any()) {
                var T = stackalloc[] { 0 };
                T[0] = Sections.Count.ToString("X").Length;
                writer.WriteStartElement($"CommonObjectFile.{nameof(Sections)}");
                var i = 1;
                foreach (var section in Sections) {
                    writer.WriteStartElement("Section");
                    writer.WriteAttributeString("SectionIndex", String.Format($"{{0:X{T[0]}}}", i));
                    section.WriteXml(writer);
                    writer.WriteEndElement();
                    i++;
                    }
                writer.WriteEndElement();
                }
            writer.WriteEndElement();
            }

        //protected internal override void WriteXml(IXamlWriter writer) {
        //    if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
        //    using (writer.WriteElementScope(nameof(CommonObjectFileSource))) {

        //        }
        //    }

        [DllImport("ole32.dll", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)] private static extern HRESULT CoCreateInstance(ref Guid rclsid, IntPtr outer, CLSCTX flags, ref Guid riid, out IntPtr r);
        }
    }