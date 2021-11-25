namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation
    {
    //[DebuggerDisplay(@"\{{ToString(),nq}\}")]
    //public class Asn1NameSequence :
    //    Asn1ReadOnlyCollection<KeyValuePair<Asn1ObjectIdentifier, String>>,
    //    IAsn1GeneralName,
    //    IJsonSerializable
    //    {
    //    public Asn1NameSequence(IEnumerable<KeyValuePair<Asn1ObjectIdentifier, String>> source)
    //        : base(source)
    //        {
    //        }

    //    public Asn1NameSequence(Asn1Object source)
    //        :base(source.Select(i =>
    //            new KeyValuePair<Asn1ObjectIdentifier,String>(
    //                (Asn1ObjectIdentifier)i[0][0],
    //                i[0][1].ToString())))
    //        {
    //        }

    //    #region M:ToString(Object):String
    //    internal static String ToString(String source) {
    //        if (source == null) { return String.Empty; }
    //        if (source.IndexOf("\"") != -1) {
    //            return $"\"{source.Replace("\"", "\"\"")}\"";
    //            }
    //        return source;
    //        }
    //    #endregion
    //    #region M:ToString:String
    //    /**
    //     * <summary>Returns a string that represents the current object.</summary>
    //     * <returns>A string that represents the current object.</returns>
    //     * <filterpriority>2</filterpriority>
    //     * */
    //    public override String ToString()
    //        {
    //        #if NET35
    //        return String.Join(", ", Items.Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value)}").ToArray());
    //        #else
    //        return String.Join(", ", Items.Select(i => $"{new Oid(i.Key.ToString()).FriendlyName}={ToString(i.Value)}"));
    //        #endif
    //        }
    //    #endregion

    //    public IList<String> this[String key] { get {
    //        if (key == null) { throw new ArgumentNullException(nameof(key)); }
    //        #if NET35
    //        if (String.IsNullOrEmpty(key)) { throw new ArgumentOutOfRangeException(nameof(key)); }
    //        #else
    //        if (String.IsNullOrWhiteSpace(key)) { throw new ArgumentOutOfRangeException(nameof(key)); }
    //        #endif
    //        var r = new List<String>();
    //        foreach (var item in Items) {
    //            if (String.Equals(item.Key.ToString(), key, StringComparison.OrdinalIgnoreCase)) {
    //                r.Add(item.Value);
    //                }
    //            }
    //        return r;
    //        }}

    //    public void WriteJson(JsonWriter writer, JsonSerializer serializer)
    //        {
    //        if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
    //        writer.WriteStartObject();
    //        WriteValue(writer, serializer, "(default)", ToString());
    //        foreach (var item in Items)
    //            {
    //            WriteValue(writer, serializer, (new Oid(item.Key.ToString())).FriendlyName, item.Value);
    //            }
    //        writer.WriteEndObject();
    //        }
    //    }
    }
