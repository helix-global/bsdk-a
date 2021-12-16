using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
using Microsoft.SqlServer.Server;

[Serializable]
[SqlUserDefinedAggregate(
    Format.UserDefined,
    IsInvariantToNulls      = true,
    IsInvariantToDuplicates = true,
    IsInvariantToOrder      = true,
    MaxByteSize = -1)]
public struct string_agg : IBinarySerialize
    {
    private StringBuilder r;
    private String separator;
    public void Init()
        {
        r = new StringBuilder();
        separator = null;
        }

    public void Accumulate(SqlString value, SqlString separator, SqlString left, SqlString right) {
        this.separator = separator.IsNull
            ? null
            : separator.Value;
        if ((r.Length > 0) && (this.separator != null)) { r.Append(separator.Value); }
        if (!left.IsNull) { r.Append(left.Value); }
        r.Append(value.Value);
        if (!right.IsNull) { r.Append(right.Value); }
        }

    public void Merge(string_agg other)
        {
        if ((r.Length > 0) && (this.separator != null)) { r.Append(other.r); }
        }

    public SqlString Terminate()
        {
        return new SqlString(r.ToString());
        }

    void IBinarySerialize.Read(BinaryReader r)
        {
        this.r = new StringBuilder(r.ReadString());
        }

    void IBinarySerialize.Write(BinaryWriter w)
        {
        w.Write(r.ToString());
        }
    }
