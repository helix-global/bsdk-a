using BinaryStudio.PlatformUI;
using BinaryStudio.Security.Cryptography.CryptographicMessageSyntax;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Views
    {
    public class ECms : NotifyPropertyChangedDispatcherObject<CmsMessage>
        {
        public ECms(CmsMessage source)
            : base(source)
            {
            }
        }
    }