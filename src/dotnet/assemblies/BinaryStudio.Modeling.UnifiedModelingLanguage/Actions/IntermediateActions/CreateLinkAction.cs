namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface CreateLinkAction : WriteLinkAction
        {
        new LinkEndCreationData[] EndData { get; }
        }
    }