namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum CHKSUM_TYPE : byte
        {
        CHKSUM_TYPE_NONE    = 0,
        CHKSUM_TYPE_MD5     = 1,
        CHKSUM_TYPE_SHA1    = 2,
        CHKSUM_TYPE_SHA_256 = 3
        }
    }