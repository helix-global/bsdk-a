namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Manifestation : Abstraction
        {
        PackageableElement UtilizedElement { get; }
        }
    }