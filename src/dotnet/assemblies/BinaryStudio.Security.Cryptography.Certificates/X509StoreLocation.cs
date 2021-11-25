using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public enum X509StoreLocation
        {
        CurrentUser               = 0x00010000,
        CurrentService            = 0x00040000,
        LocalMachine              = 0x00020000,
        LocalMachineGroupPolicy   = 0x00080000,
        CurrentUserGroupPolicy    = 0x00070000,
        Services                  = 0x00050000,
        Users                     = 0x00060000,
        LocalMachineEnterprise    = 0x00090000
        }
    }