using BinaryStudio.PlatformUI;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Views
    {
    public class EAsn1 : NotifyPropertyChangedDispatcherObject<Asn1Object>
        {
        public EAsn1(Asn1Object source)
            : base(source)
            {
            }
        }
    }