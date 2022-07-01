namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface CreateLinkAction : WriteLinkAction
        {
        LinkEndCreationData[] EndData { get; }
        }
    }