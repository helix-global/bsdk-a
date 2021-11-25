namespace BinaryStudio.PortableExecutable.Win32
    {
    public enum CV_AM33
        {
        CV_AM33_NOREG   =   0,

        // "Extended" (general purpose integer) registers
        CV_AM33_E0      =   10,
        CV_AM33_E1      =   11,
        CV_AM33_E2      =   12,
        CV_AM33_E3      =   13,
        CV_AM33_E4      =   14,
        CV_AM33_E5      =   15,
        CV_AM33_E6      =   16,
        CV_AM33_E7      =   17,

        // Address registers
        CV_AM33_A0      =   20,
        CV_AM33_A1      =   21,
        CV_AM33_A2      =   22,
        CV_AM33_A3      =   23,

        // Integer data registers
        CV_AM33_D0      =   30,
        CV_AM33_D1      =   31,
        CV_AM33_D2      =   32,
        CV_AM33_D3      =   33,

        // (Single-precision) floating-point registers
        CV_AM33_FS0     =   40,
        CV_AM33_FS1     =   41,
        CV_AM33_FS2     =   42,
        CV_AM33_FS3     =   43,
        CV_AM33_FS4     =   44,
        CV_AM33_FS5     =   45,
        CV_AM33_FS6     =   46,
        CV_AM33_FS7     =   47,
        CV_AM33_FS8     =   48,
        CV_AM33_FS9     =   49,
        CV_AM33_FS10    =   50,
        CV_AM33_FS11    =   51,
        CV_AM33_FS12    =   52,
        CV_AM33_FS13    =   53,
        CV_AM33_FS14    =   54,
        CV_AM33_FS15    =   55,
        CV_AM33_FS16    =   56,
        CV_AM33_FS17    =   57,
        CV_AM33_FS18    =   58,
        CV_AM33_FS19    =   59,
        CV_AM33_FS20    =   60,
        CV_AM33_FS21    =   61,
        CV_AM33_FS22    =   62,
        CV_AM33_FS23    =   63,
        CV_AM33_FS24    =   64,
        CV_AM33_FS25    =   65,
        CV_AM33_FS26    =   66,
        CV_AM33_FS27    =   67,
        CV_AM33_FS28    =   68,
        CV_AM33_FS29    =   69,
        CV_AM33_FS30    =   70,
        CV_AM33_FS31    =   71,

        // Special purpose registers

        // Stack pointer
        CV_AM33_SP      =   80,

        // Program counter
        CV_AM33_PC      =   81,

        // Multiply-divide/accumulate registers
        CV_AM33_MDR     =   82,
        CV_AM33_MDRQ    =   83,
        CV_AM33_MCRH    =   84,
        CV_AM33_MCRL    =   85,
        CV_AM33_MCVF    =   86,

        // CPU status words
        CV_AM33_EPSW    =   87,
        CV_AM33_FPCR    =   88,

        // Loop buffer registers
        CV_AM33_LIR     =   89,
        CV_AM33_LAR     =   90,
        }
    }