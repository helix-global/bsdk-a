namespace BinaryStudio.Serialization
    {
    public interface IXamlSerializable
        {
        void WriteXml(IXamlWriter writer);
        }
    }