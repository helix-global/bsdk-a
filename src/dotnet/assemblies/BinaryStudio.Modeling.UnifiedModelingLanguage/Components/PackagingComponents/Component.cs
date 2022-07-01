namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Component : Class
        {
        PackageableElement[] PackagedElement { get; }
        }
    }