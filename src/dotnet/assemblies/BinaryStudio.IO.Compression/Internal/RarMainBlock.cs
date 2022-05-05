using System;

namespace BinaryStudio.IO.Compression
    {
    internal class RarMainBlock : RarBaseBlock
        {
        public RAR_MHFL Flags { get; }
        public Boolean IsSolidArchive { get { return Flags.HasFlag(RAR_MHFL.MHFL_SOLID); }}
        public Boolean IsRecoveryRecordPresent { get { return Flags.HasFlag(RAR_MHFL.MHFL_PROTECT); }}
        public Boolean IsLockedArchive { get { return Flags.HasFlag(RAR_MHFL.MHFL_LOCK); }}
        public Int32? VolumeNumber { get;internal set; }
        public Int64? LocatorQuickOpenOffset { get;internal set; }
        public Int64? LocatorRecoveryRecordOffset { get;internal set; }

        public RarMainBlock(RAR_MHFL flags)
            {
            Flags = flags;
            }
        }
    }