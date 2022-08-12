namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Interaction : Behavior, InteractionFragment
        {
        Action[] Action { get; }
        InteractionFragment[] Fragment { get; }
        Lifeline[] Lifeline { get; }
        Message[] Message { get; }
        }
    }