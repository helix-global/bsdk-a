using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Markup;

[assembly: InternalsVisibleTo("BinaryStudio.Security.Cryptography.CryptographyServiceProvider")]
[assembly: InternalsVisibleTo("BinaryStudio.Security.Cryptography.CryptographicMessageSyntax")]
[assembly: InternalsVisibleTo("BinaryStudio.Security.Cryptography.PlatformUI")]

#if USE_WPF
[assembly: XmlnsPrefix("http://schemas.helix.global", "u")]
[assembly: XmlnsDefinition("http://schemas.helix.global", "BinaryStudio.Security.Cryptography.Certificates")]
#endif

[assembly: Guid("9f1e394c-fda7-42f9-b6f6-99fe4fd85e43")]