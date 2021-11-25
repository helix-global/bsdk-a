namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum CV_COOKIETYPE : byte
        {
        CV_COOKIETYPE_COPY = 0, 
        CV_COOKIETYPE_XOR_SP, 
        CV_COOKIETYPE_XOR_BP,
        CV_COOKIETYPE_XOR_R13,
        }
    }