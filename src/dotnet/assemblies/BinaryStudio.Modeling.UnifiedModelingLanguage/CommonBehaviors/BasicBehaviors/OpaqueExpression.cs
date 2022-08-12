namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface OpaqueExpression
        {
        Behavior Behavior { get; }
        Parameter Result { get; }
        }
    }