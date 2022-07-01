namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface NamedElement : Element
        {
        Dependency[] ClientDependency { get; }
        //Namespace Namespace { get; }
        }
    }