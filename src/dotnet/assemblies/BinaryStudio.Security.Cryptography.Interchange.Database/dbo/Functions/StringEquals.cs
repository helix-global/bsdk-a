using System;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
    {
    [SqlFunction]
    public static SqlBoolean Int32GT(SqlInt32 x, SqlInt32 y)
        {
        if (x.IsNull && y.IsNull) { return new SqlBoolean(false);  }
        if (x.IsNull) { return new SqlBoolean(false);  }
        if (y.IsNull) { return new SqlBoolean(true);   }
        return x > y;
        }

    [SqlFunction]
    public static SqlBoolean StringEquals(SqlString x, SqlString y, SqlString flags)
        {
        if (x.IsNull && y.IsNull) { return new SqlBoolean(true);  }
        if (x.IsNull || y.IsNull) { return new SqlBoolean(false); }
        if (ReferenceEquals(x.Value, y.Value)) { return new SqlBoolean(true); }
        if (flags.IsNull) { return x == y; }
        switch (flags.Value.ToLower()) {
            case "ordinal"                    : return new SqlBoolean(String.Equals(x.Value, y.Value, StringComparison.Ordinal));
            case "ordinalignorecase"          : return new SqlBoolean(String.Equals(x.Value, y.Value, StringComparison.OrdinalIgnoreCase));
            case "invariantculture"           : return new SqlBoolean(String.Equals(x.Value, y.Value, StringComparison.InvariantCulture));
            case "invariantcultureignorecase" : return new SqlBoolean(String.Equals(x.Value, y.Value, StringComparison.InvariantCultureIgnoreCase));
            case "currentculture"             : return new SqlBoolean(String.Equals(x.Value, y.Value, StringComparison.CurrentCulture));
            case "currentcultureignorecase"   : return new SqlBoolean(String.Equals(x.Value, y.Value, StringComparison.CurrentCultureIgnoreCase));
            }
        return x == y;
        }

    [SqlFunction]
    public static SqlString TraceIndentString(SqlInt32 level, SqlInt32 value)
        {
        if (level.IsNull) { return String.Empty; }
        var count = value.IsNull
            ? 1
            : value.Value;
        return new String(' ', level.Value*count);
        }

    [SqlFunction]
    public static SqlString StringFormat1(SqlString format, Object arg0)
        {
        return String.Format(format.Value, arg0);
        }

    [SqlFunction]
    public static SqlString StringFormat2(SqlString format, Object arg0, Object arg1)
        {
        return String.Format(format.Value, arg0, arg1);
        }

    [SqlFunction]
    public static SqlString StringFormat3(SqlString format, Object arg0, Object arg1, Object arg2)
        {
        return String.Format(format.Value, arg0, arg1, arg2);
        }
    }
