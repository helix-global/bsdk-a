using System;

namespace Options
    {
    [Flags]
    internal enum InfrastructureFlags
        {
        CSP      =  1,
        CSPtypes =  2,
        CSPkeys  =  4,
        CSPalgs  =  8
        }
    }