namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ExpansionNode : ObjectNode
        {
        ExpansionRegion RegionAsInput { get; }
        ExpansionRegion RegionAsOutput { get; }
        }
    }