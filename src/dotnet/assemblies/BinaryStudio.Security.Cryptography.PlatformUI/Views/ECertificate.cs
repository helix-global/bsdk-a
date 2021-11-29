using BinaryStudio.PlatformUI.Shell;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography.PlatformUI.Views
    {
    public class ECertificate : View<X509Certificate>
        {
        public ECertificate(X509Certificate content)
            : base(content)
            {
            }
        }
    }