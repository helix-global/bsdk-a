namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum CV_SIGNATURE
        {
        CV_SIGNATURE_C6         = 0,  // Actual signature is >64K
        CV_SIGNATURE_C7         = 1,  // First explicit signature
        CV_SIGNATURE_C11        = 2,  // C11 (vc5.x) 32-bit types
        CV_SIGNATURE_C13        = 4,  // C13 (vc7.x) zero terminated names
        CV_SIGNATURE_RESERVED   = 5   // All signatures from 5 to 64K are reserved
        }
    }