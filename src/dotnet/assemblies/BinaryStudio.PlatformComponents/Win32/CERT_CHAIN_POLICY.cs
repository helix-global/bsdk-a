namespace BinaryStudio.PlatformComponents.Win32
    {
    public enum CERT_CHAIN_POLICY
        {
        CERT_CHAIN_POLICY_BASE              =  1,
        CERT_CHAIN_POLICY_AUTHENTICODE      =  2,
        CERT_CHAIN_POLICY_AUTHENTICODE_TS   =  3,
        CERT_CHAIN_POLICY_SSL               =  4,
        CERT_CHAIN_POLICY_BASIC_CONSTRAINTS =  5,
        CERT_CHAIN_POLICY_NT_AUTH           =  6,
        CERT_CHAIN_POLICY_MICROSOFT_ROOT    =  7,
        CERT_CHAIN_POLICY_EV                =  8,
        CERT_CHAIN_POLICY_SSL_F12           =  9,
        CERT_CHAIN_POLICY_SSL_HPKP_HEADER   = 10,
        CERT_CHAIN_POLICY_THIRD_PARTY_ROOT  = 11,
        CERT_CHAIN_POLICY_SSL_KEY_PIN       = 12
        }
    }