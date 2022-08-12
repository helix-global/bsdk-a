using System;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
    {
    private const String szOID_ECDSA_SHA224 = "1.2.840.10045.4.3.1";
    private const String szOID_NIST_sha224  = "2.16.840.1.101.3.4.2.4";
    private const String szOID_CP_GOST_R3411                     = "1.2.643.2.2.9";
    private const String szOID_CP_GOST_R3411_12_256              = "1.2.643.7.1.1.2.2";
    private const String szOID_CP_GOST_R3411_12_512              = "1.2.643.7.1.1.2.3";
    private const String szOID_CP_GOST_R3411_R3410EL             = "1.2.643.2.2.3";
    private const String szOID_CP_GOST_R3411_R3410               = "1.2.643.2.2.4";
    private const String szOID_CP_GOST_R3410_12_256              = "1.2.643.7.1.1.1.1";
    private const String szOID_CP_GOST_R3410_12_512              = "1.2.643.7.1.1.1.2";
    private const String szOID_tc26_gost_3410_12_256_paramSetA   = "1.2.643.7.1.2.1.1.1";   /* ГОСТ Р 34.10-2012, 256 бит, параметры ТК-26, набор A */
    private const String szOID_tc26_gost_3410_12_512_paramSetA   = "1.2.643.7.1.2.1.2.1";   /* ГОСТ Р 34.10-2012, 512 бит, параметры по умолчанию */
    private const String szOID_tc26_gost_3410_12_512_paramSetB   = "1.2.643.7.1.2.1.2.2";   /* ГОСТ Р 34.10-2012, 512 бит, параметры ТК-26, набор B */
    private const String szOID_tc26_gost_3410_12_512_paramSetC   = "1.2.643.7.1.2.1.2.3";   /* ГОСТ Р 34.10-2012, 512 бит, параметры ТК-26, набор С */
    private const String szOID_CP_GOST_R3411_12_256_R3410        = "1.2.643.7.1.1.3.2";
    private const String szOID_CP_GOST_R3411_12_512_R3410        = "1.2.643.7.1.1.3.3";

    [SqlFunction]
    public static SqlString OidToStr(SqlString value)
        {
        if (value.IsNull) { return SqlString.Null; }
        var r = value.Value;
        switch (r)
            {
            case szOID_NIST_sha224:                       r = "sha224"; break;
            case szOID_ECDSA_SHA224:                      r = "sha224ECDSA"; break;
            case szOID_CP_GOST_R3411:                     r = "GOST R 34.11-94"; break;
            case szOID_CP_GOST_R3411_12_256:              r = "GOST R 34.11-2012 256 bit"; break;
            case szOID_CP_GOST_R3411_12_512:              r = "GOST R 34.11-2012 512 bit"; break;
            case szOID_CP_GOST_R3411_R3410EL:             r = "GOST R 34.11/34.10-2001"; break;
            case szOID_CP_GOST_R3411_R3410:               r = "GOST R 34.10-94"; break;
            case szOID_CP_GOST_R3410_12_256:              r = "GOST R 34.10-2012 256 bit"; break;
            case szOID_CP_GOST_R3410_12_512:              r = "GOST R 34.10-2012 512 bit"; break;
            case szOID_tc26_gost_3410_12_256_paramSetA:   r = "GOST R 34.10-2012 256 bit, parameters TC26 A"; break;
            case szOID_tc26_gost_3410_12_512_paramSetA:   r = "GOST R 34.10-2012 512 bit, default parameters"; break;
            case szOID_tc26_gost_3410_12_512_paramSetB:   r = "GOST R 34.10-2012 512 bit, parameters TC26 B"; break;
            case szOID_tc26_gost_3410_12_512_paramSetC:   r = "GOST R 34.10-2012 512 bit, parameters TC26 C"; break;
            case szOID_CP_GOST_R3411_12_256_R3410:        r = "GOST R 34.11-2012/34.10-2012 256 bit"; break;
            case szOID_CP_GOST_R3411_12_512_R3410:        r = "GOST R 34.11-2012/34.10-2012 512 bit"; break;
            default:
                {
                r = new Oid(r).FriendlyName;
                if (String.IsNullOrEmpty(r))
                    {
                    r = value.Value;
                    }
                }
            break;
            }
        return new SqlString(r);
        }
    }
