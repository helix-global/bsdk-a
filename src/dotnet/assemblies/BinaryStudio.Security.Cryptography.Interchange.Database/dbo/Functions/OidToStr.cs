using System;
using System.ComponentModel.Design;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
    {
    private const String szOID_ECDSA_SHA224 = "1.2.840.10045.4.3.1";
    private const String szOID_NIST_sha224  = "2.16.840.1.101.3.4.2.4";

    [SqlFunction]
    public static SqlString OidToStr(SqlString value)
        {
        if (value.IsNull) { return SqlString.Null; }
        var r = value.Value;
             if (r == szOID_NIST_sha224)  { r = "sha224";      }
        else if (r == szOID_ECDSA_SHA224) { r = "sha224ECDSA"; }
        else
            {
            r = new Oid(r).FriendlyName;
            }
        return new SqlString(r);
        }
    }
