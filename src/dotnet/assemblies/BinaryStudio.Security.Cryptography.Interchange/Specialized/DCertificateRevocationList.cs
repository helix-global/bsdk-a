namespace BinaryStudio.Security.Cryptography.Interchange
    {
    public partial class DCertificateRevocationList : IDObject
        {
        DObjectType IDObject.Type
            {
            get { return ((IDObject)Object).Type; }
            set { ((IDObject)Object).Type = value; }
            }
        }
    }