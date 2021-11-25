namespace BinaryStudio.Security.Cryptography.AbstractSyntaxNotation.Converters
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