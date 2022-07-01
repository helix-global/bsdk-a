namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface Abstraction : Dependency
        {
        OpaqueExpression Mapping { get; }
        }
    }