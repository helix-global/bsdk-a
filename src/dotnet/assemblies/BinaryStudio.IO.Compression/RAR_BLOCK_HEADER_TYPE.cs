namespace BinaryStudio.IO.Compression
    {
    public enum RAR_BLOCK_HEADER_TYPE : byte
        {
        HEAD_MARK           = 0x00,
        HEAD_MAIN           = 0x01,
        HEAD_FILE           = 0x02,
        HEAD_SERVICE        = 0x03,
        HEAD_CRYPT          = 0x04,
        HEAD_ENDARC         = 0x05,
        HEAD_UNKNOWN        = 0xff,
        HEAD3_MARK          = 0x72,
        HEAD3_MAIN          = 0x73,
        HEAD3_FILE          = 0x74,
        HEAD3_CMT           = 0x75,
        HEAD3_AV            = 0x76,
        HEAD3_OLDSERVICE    = 0x77,
        HEAD3_PROTECT       = 0x78,
        HEAD3_SIGN          = 0x79,
        HEAD3_SERVICE       = 0x7a,
        HEAD3_ENDARC        = 0x7b
        }
    }