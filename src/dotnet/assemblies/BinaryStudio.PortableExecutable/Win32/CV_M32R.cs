namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum CV_M32R
        {
        CV_M32R_NOREG    =   0,

        CV_M32R_R0    =   10,
        CV_M32R_R1    =   11,
        CV_M32R_R2    =   12,
        CV_M32R_R3    =   13,
        CV_M32R_R4    =   14,
        CV_M32R_R5    =   15,
        CV_M32R_R6    =   16,
        CV_M32R_R7    =   17,
        CV_M32R_R8    =   18,
        CV_M32R_R9    =   19,
        CV_M32R_R10   =   20,
        CV_M32R_R11   =   21,
        CV_M32R_R12   =   22,   // Gloabal Pointer, if used
        CV_M32R_R13   =   23,   // Frame Pointer, if allocated
        CV_M32R_R14   =   24,   // Link Register
        CV_M32R_R15   =   25,   // Stack Pointer
        CV_M32R_PSW   =   26,   // Preocessor Status Register
        CV_M32R_CBR   =   27,   // Condition Bit Register
        CV_M32R_SPI   =   28,   // Interrupt Stack Pointer
        CV_M32R_SPU   =   29,   // User Stack Pointer
        CV_M32R_SPO   =   30,   // OS Stack Pointer
        CV_M32R_BPC   =   31,   // Backup Program Counter
        CV_M32R_ACHI  =   32,   // Accumulator High
        CV_M32R_ACLO  =   33,   // Accumulator Low
        CV_M32R_PC    =   34,   // Program Counter
        }
    }