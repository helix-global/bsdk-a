namespace BinaryStudio.IO.Compression
    {
    internal class NullableValue<T>
        where T: struct
        {
        public static T? Value = default;
        }
    }