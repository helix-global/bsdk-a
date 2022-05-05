using System;
using System.IO;
using System.Security.Cryptography;

namespace BinaryStudio.IO.Compression
    {
    public sealed class CRC32 : HashAlgorithm
        {
        public const UInt32 DefaultPolynomial = 0xedb88320;
        public const UInt32 DefaultSeed = 0xffffffff;

        private UInt32 hash;
        private UInt32 seed;
        private UInt32[] table;
        private static UInt32[] defaultTable;

        public CRC32()
            {
            table = InitializeTable(DefaultPolynomial);
            seed = DefaultSeed;
            Initialize();
            }

        public CRC32(UInt32 polynomial, UInt32 seed)
            {
            table = InitializeTable(polynomial);
            this.seed = seed;
            Initialize();
            }

        /// <summary>Initializes an implementation of the <see cref="T:System.Security.Cryptography.HashAlgorithm" /> class.</summary>
        public override void Initialize()
            {
            hash = seed;
            }

        /// <summary>When overridden in a derived class, routes data written to the object into the hash algorithm for computing the hash.</summary>
        /// <param name="buffer">The input to compute the hash code for.</param>
        /// <param name="start">The offset into the byte array from which to begin using data.</param>
        /// <param name="length">The number of bytes in the byte array to use as data.</param>
        protected override void HashCore(Byte[] buffer, Int32 start, Int32 length)
            {
            hash = CalculateHash(table, hash, buffer, start, length);
            }

        /// <summary>When overridden in a derived class, finalizes the hash computation after the last data is processed by the cryptographic stream object.</summary>
        /// <returns>The computed hash code.</returns>
        protected override Byte[] HashFinal()
            {
            var r = UInt32ToBigEndianBytes(~hash);
            HashValue = r;
            return r;
            }

        /// <summary>Gets the size, in bits, of the computed hash code.</summary>
        /// <returns>The size, in bits, of the computed hash code.</returns>
        public override Int32 HashSize
            {
            get { return 32; }
            }

        public static UInt32 Compute(Byte[] buffer)
            {
            return ~CalculateHash(
                InitializeTable(DefaultPolynomial),
                DefaultSeed, buffer, 0, buffer.Length);
            }

        public static UInt32 Compute(UInt32 seed, Byte[] buffer)
            {
            return ~CalculateHash(
                InitializeTable(DefaultPolynomial),
                seed, buffer, 0, buffer.Length);
            }

        public static unsafe UInt32 Compute(UInt32 seed, Byte* buffer, Int32 length)
            {
            return ~CalculateHash(
                InitializeTable(DefaultPolynomial),
                seed, buffer, 0, length);
            }

        public static UInt32 Compute(UInt32 polynomial, UInt32 seed, Byte[] buffer)
            {
            return ~CalculateHash(
                InitializeTable(polynomial),
                seed, buffer, 0, buffer.Length);
            }

        public static UInt32 Compute(UInt32 seed, Stream stream, Int32 length)
            {
            var buffer = new Byte[length];
            stream.Read(buffer, 0, length);
            return ~CalculateHash(
                InitializeTable(DefaultPolynomial),
                seed, buffer, 0, buffer.Length);
            }

        public static unsafe UInt32 Compute(UInt32 polynomial, UInt32 seed, Byte* buffer, Int32 length)
            {
            return ~CalculateHash(
                InitializeTable(polynomial),
                seed, buffer, 0, length);
            }

        private static UInt32[] InitializeTable(UInt32 polynomial) {
            if (polynomial == DefaultPolynomial && defaultTable != null) return defaultTable;
            var createTable = new UInt32[256];
            for (var i = 0; i < 256; i++) {
                var entry = (UInt32)i;
                for (var j = 0; j < 8; j++)
                    if ((entry & 1) == 1)
                        entry = (entry >> 1) ^ polynomial;
                    else
                        entry = entry >> 1;
                createTable[i] = entry;
                }

            if (polynomial == DefaultPolynomial) defaultTable = createTable;
            return createTable;
            }

        private static unsafe UInt32 CalculateHash(UInt32[] table, UInt32 seed, Byte[] buffer, Int32 start, Int32 size) {
            fixed (Byte *i = buffer)
                {
                return CalculateHash(table, seed, i, start, size);
                }
            }

        private static unsafe UInt32 CalculateHash(UInt32[] table, UInt32 seed, Byte* buffer, Int32 start, Int32 size) {
            var crc = seed;
            for (var i = start; i < size; i++)
                unchecked
                    {
                    crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
                    }
            return crc;
            }

        private static Byte[] UInt32ToBigEndianBytes(UInt32 x) {
            return new Byte[] {
                (Byte)((x >> 24) & 0xff),
                (Byte)((x >> 16) & 0xff),
                (Byte)((x >> 8) & 0xff),
                (Byte)(x & 0xff)
                };
            }
        }
    }