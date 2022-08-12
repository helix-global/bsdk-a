namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface AcceptCallAction : AcceptEventAction
        {
        OutputPin ReturnInformation { get; }
        }
    }