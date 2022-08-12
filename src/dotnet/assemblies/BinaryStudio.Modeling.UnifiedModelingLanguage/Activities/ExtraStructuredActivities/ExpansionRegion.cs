namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ExpansionRegion : StructuredActivityNode
        {
        ExpansionNode[] InputElement { get; }
        ExpansionKind Mode { get; }
        ExpansionNode[] OutputElement { get; }
        }
    }