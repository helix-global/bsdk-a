using System.Data.SqlTypes;
using System.Security.Cryptography;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
    {
    [SqlFunction]
    public static SqlString OidToStr(SqlString value)
        {
        return (!value.IsNull)
            ? new SqlString(new Oid(value.Value).FriendlyName)
            : SqlString.Null;
        }
    }
