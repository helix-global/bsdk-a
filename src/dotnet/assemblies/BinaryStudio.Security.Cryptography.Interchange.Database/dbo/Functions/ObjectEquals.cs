using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
    {
    [SqlFunction]
    public static SqlBoolean ObjectEquals(Object x, Object y)
        {
        return new SqlBoolean(Object.Equals(x,y));
        }
    }
