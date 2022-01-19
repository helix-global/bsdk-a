using System;
using System.IO;
using System.Text;

namespace BinaryStudio.IO.Compression
    {
    internal class RarBlockReader50 : RarBlockReader
        {
        public RarBlockReader50(Encoding encoding, BinaryReader reader)
            : base(encoding, reader)
            {
            }

        public override Int32 MarkHeadSize { get { return 8; }}
        public override unsafe RarBaseBlock NextBlock() {
            var BlockOffset = Reader.BaseStream.Position;
            var crcI  = Reader.ReadUInt32();
            var BSize = Reader.ReadVInt64();
            var HSize = Reader.BaseStream.Position + BSize - BlockOffset;
            var crcO = 0xffffffffU;
            if (HSize > 4) {
                using (Reader.StorePosition()) {
                    Reader.BaseStream.Seek(BlockOffset + sizeof(UInt32), SeekOrigin.Begin);
                    crcO = CRC32.Compute(0xffffffff, Reader.BaseStream, (Int32)(HSize - sizeof(UInt32)));
                    }
                }
            if (crcI != crcO) { throw new InvalidDataException(); }
            var type   = (RAR_BLOCK_HEADER_TYPE)(Byte)Reader.ReadVInt64();
            var BFlags = (RAR_HFL_FLAGS)(Int32)Reader.ReadVInt64();
            var ESize  = (BFlags.HasFlag(RAR_HFL_FLAGS.HFL_EXTRA)) ? Reader.ReadVInt64() : 0;
            var DSize  = (BFlags.HasFlag(RAR_HFL_FLAGS.HFL_DATA))  ? Reader.ReadVInt64() : 0;
            switch (type) {
                case RAR_BLOCK_HEADER_TYPE.HEAD_MAIN:
                    {
                    var Flags  = (RAR_MHFL)(Int32)Reader.ReadVInt64();
                    var VolumeNumber = Flags.HasFlag(RAR_MHFL.MHFL_VOLNUMBER) ? (Int32)Reader.ReadVInt64() : NullableValue<Int32>.Value;
                    var LocatorQuickOpenOffset      = NullableValue<Int64>.Value;
                    var LocatorRecoveryRecordOffset = NullableValue<Int64>.Value;
                    if (ESize > 0) {
                        while (ESize > 0) {
                            var FieldOffset = Reader.BaseStream.Position;
                            var FieldSize   = Reader.ReadVInt64();
                            var FieldType   = Reader.ReadVInt64();
                            if (FieldType == MHEXTRA_LOCATOR) {
                                var LocatorFlags = (RAR_MHEXTRA_LOCATOR_FLAGS)(Int32)Reader.ReadVInt64();
                                LocatorQuickOpenOffset      = (LocatorFlags.HasFlag(RAR_MHEXTRA_LOCATOR_FLAGS.MHEXTRA_LOCATOR_QLIST)) ? Reader.ReadVInt64() : 0;
                                LocatorRecoveryRecordOffset = (LocatorFlags.HasFlag(RAR_MHEXTRA_LOCATOR_FLAGS.MHEXTRA_LOCATOR_RR))    ? Reader.ReadVInt64() : 0;
                                LocatorQuickOpenOffset      = (LocatorQuickOpenOffset      > 0) ? (LocatorQuickOpenOffset      + BlockOffset) : NullableValue<Int64>.Value;
                                LocatorRecoveryRecordOffset = (LocatorRecoveryRecordOffset > 0) ? (LocatorRecoveryRecordOffset + BlockOffset) : NullableValue<Int64>.Value;
                                }
                            ESize -= Reader.BaseStream.Position - FieldOffset;
                            }
                        }
                    var DataOffset = Reader.BaseStream.Position;
                    Reader.BaseStream.Seek(BlockOffset + HSize + DSize, SeekOrigin.Begin);
                    return new RarMainBlock(Flags){
                        VolumeNumber = VolumeNumber,
                        LocatorRecoveryRecordOffset = LocatorRecoveryRecordOffset,
                        LocatorQuickOpenOffset = LocatorQuickOpenOffset,
                        DataOffset = DataOffset,
                        DataSize = (BFlags.HasFlag(RAR_HFL_FLAGS.HFL_DATA)) ? DSize : NullableValue<Int64>.Value
                        };
                    }
                case RAR_BLOCK_HEADER_TYPE.HEAD_FILE:
                case RAR_BLOCK_HEADER_TYPE.HEAD_SERVICE:
                    {
                    var Flags   = (RAR_FHFL)(Int32)Reader.ReadVInt64();
                    var UnpackedSize   = Reader.ReadVUInt64();
                    var FileAttributes = (FileAttributes)(Int32)Reader.ReadVInt64();
                    var Fdate = (Flags.HasFlag(RAR_FHFL.FHFL_UTIME)) ? Reader.ReadUInt32() : NullableValue<UInt32>.Value;
                    var CRC32 = (Flags.HasFlag(RAR_FHFL.FHFL_CRC32)) ? Reader.ReadUInt32() : NullableValue<UInt32>.Value;
                    var Compression = (Int32)Reader.ReadVInt64();
                    var HostOS  = (Int32)Reader.ReadVInt64();
                    var Nsize   = (Int32)Reader.ReadVInt64();
                    var Ndata   = new Byte[Nsize];
                    DateTime? CreationTime   = null;
                    DateTime? LastAccessTime = null;
                    DateTime? LastWriteTime  = null;
                    Reader.BaseStream.Read(Ndata, 0, Nsize);
                    if (ESize > 0) {
                        while (ESize > 0) {
                            var FieldOffset = Reader.BaseStream.Position;
                            var FieldSize   = Reader.ReadVInt64();
                            var FieldType   = Reader.ReadVInt64();
                            switch (FieldType) {
                                case FHEXTRA_HTIME:
                                    {
                                    var FieldFlags = (RAR_FHEXTRA_HTIME_FLAGS)(Int32)Reader.ReadVInt64();
                                    var IsUnixTime = FieldFlags.HasFlag(RAR_FHEXTRA_HTIME_FLAGS.FHEXTRA_HTIME_UNIXTIME);
                                    if (IsUnixTime)
                                        {
                                        throw new NotImplementedException();
                                        }
                                    else
                                        {
                                        LastWriteTime  = (FieldFlags.HasFlag(RAR_FHEXTRA_HTIME_FLAGS.FHEXTRA_HTIME_MTIME)) ? DateTime.FromFileTime(Reader.ReadInt64()) : NullableValue<DateTime>.Value;
                                        LastAccessTime = (FieldFlags.HasFlag(RAR_FHEXTRA_HTIME_FLAGS.FHEXTRA_HTIME_ATIME)) ? DateTime.FromFileTime(Reader.ReadInt64()) : NullableValue<DateTime>.Value;
                                        CreationTime   = (FieldFlags.HasFlag(RAR_FHEXTRA_HTIME_FLAGS.FHEXTRA_HTIME_CTIME)) ? DateTime.FromFileTime(Reader.ReadInt64()) : NullableValue<DateTime>.Value;
                                        }
                                    }
                                    break;
                                case FHEXTRA_SUBDATA:
                                    {
                                    FieldSize = Reader.BaseStream.Position - FieldOffset;
                                    if (FieldSize > 0) {
                                        Reader.BaseStream.Seek(FieldSize, SeekOrigin.Current);
                                        }
                                    }
                                    break;
                                default: throw new ArgumentException(nameof(FieldType));
                                }
                            ESize -= Reader.BaseStream.Position - FieldOffset;
                            }
                        }
                    var DataOffset = Reader.BaseStream.Position;
                    Reader.BaseStream.Seek(BlockOffset + HSize + DSize, SeekOrigin.Begin);
                    var r = (type == RAR_BLOCK_HEADER_TYPE.HEAD_FILE)
                        ? new RarFileBlock(Encoding.GetString(Ndata))
                        : new RarServiceBlock(Encoding.GetString(Ndata));
                    r.CreationTime   = CreationTime;
                    r.LastAccessTime = LastAccessTime;
                    r.LastWriteTime  = LastWriteTime;
                    r.UnpackedSize   = UnpackedSize;
                    r.FileAttributes = FileAttributes;
                    r.CRC32          = CRC32;
                    r.HostOS         = HostOS;
                    r.CompressionMethod  = (Int32)((Compression >> 7) & 0x07);
                    r.CompressionVersion = (Compression & 0x3f) + 50;
                    r.DataOffset = DataOffset;
                    r.DataSize   = (BFlags.HasFlag(RAR_HFL_FLAGS.HFL_DATA)) ? DSize : NullableValue<Int64>.Value;
                    return r;
                    }
                case RAR_BLOCK_HEADER_TYPE.HEAD_ENDARC:
                case RAR_BLOCK_HEADER_TYPE.HEAD3_ENDARC:
                    {
                    var Flags  = (RAR_EHFL)(Int32)Reader.ReadVInt64();
                    var DataOffset = Reader.BaseStream.Position;
                    Reader.BaseStream.Seek(BlockOffset + HSize + DSize, SeekOrigin.Begin);
                    return new RarEndOfArcBlock
                        {
                        NextVolume = Flags.HasFlag(RAR_EHFL.EHFL_NEXTVOLUME),
                        DataOffset = DataOffset,
                        DataSize   = (BFlags.HasFlag(RAR_HFL_FLAGS.HFL_DATA)) ? DSize : NullableValue<Int64>.Value
                        };
                    }
                case RAR_BLOCK_HEADER_TYPE.HEAD_MARK:
                case RAR_BLOCK_HEADER_TYPE.HEAD_CRYPT:
                case RAR_BLOCK_HEADER_TYPE.HEAD_UNKNOWN:
                case RAR_BLOCK_HEADER_TYPE.HEAD3_MARK:
                case RAR_BLOCK_HEADER_TYPE.HEAD3_MAIN:
                case RAR_BLOCK_HEADER_TYPE.HEAD3_FILE:
                case RAR_BLOCK_HEADER_TYPE.HEAD3_CMT:
                case RAR_BLOCK_HEADER_TYPE.HEAD3_AV:
                case RAR_BLOCK_HEADER_TYPE.HEAD3_OLDSERVICE:
                case RAR_BLOCK_HEADER_TYPE.HEAD3_PROTECT:
                case RAR_BLOCK_HEADER_TYPE.HEAD3_SIGN:
                case RAR_BLOCK_HEADER_TYPE.HEAD3_SERVICE:
                default: throw new ArgumentOutOfRangeException();
                }
            return null;
            }

        private const Int32 MHEXTRA_LOCATOR = 0x01;
        private const Int32 FHEXTRA_CRYPT   = 0x01; // Encryption parameters.
        private const Int32 FHEXTRA_HASH    = 0x02; // File hash.
        private const Int32 FHEXTRA_HTIME   = 0x03; // High precision file time.
        private const Int32 FHEXTRA_VERSION = 0x04; // File version information.
        private const Int32 FHEXTRA_REDIR   = 0x05; // File system redirection (links, etc.).
        private const Int32 FHEXTRA_UOWNER  = 0x06; // Unix owner and group information.
        private const Int32 FHEXTRA_SUBDATA = 0x07; // Service header subdata array.
        }
    }