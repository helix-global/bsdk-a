using System.ComponentModel;
using System.Drawing.Design;

namespace BinaryStudio.Security.Cryptography.Certificates.Converters
    {
    #if USE_WINFORMS
    internal class NoEditor : UITypeEditor
        {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
            return UITypeEditorEditStyle.None;
            }
        }
    #endif
    }