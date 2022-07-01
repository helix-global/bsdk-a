namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Constraint : PackageableElement
        {
        Element[] ConstrainedElement { get; }
        Namespace Context { get; }
        ValueSpecification Specification { get; }
        }
    }