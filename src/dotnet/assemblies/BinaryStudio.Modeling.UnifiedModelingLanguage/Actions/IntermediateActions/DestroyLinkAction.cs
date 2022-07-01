namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface DestroyLinkAction : WriteLinkAction
        {
        LinkEndDestructionData[] EndData { get; }
        }
    }