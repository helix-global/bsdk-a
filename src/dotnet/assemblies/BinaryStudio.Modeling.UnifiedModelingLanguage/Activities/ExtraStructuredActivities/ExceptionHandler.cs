namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface ExceptionHandler : Element
        {
        ObjectNode ExceptionInput { get; }
        Classifier[] ExceptionType { get; }
        ExecutableNode HandlerBody { get; }
        ExecutableNode ProtectedNode { get; }
        }
    }