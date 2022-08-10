namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface DestroyLinkAction : WriteLinkAction
        {
        new LinkEndDestructionData[] EndData { get; }
        }
    }