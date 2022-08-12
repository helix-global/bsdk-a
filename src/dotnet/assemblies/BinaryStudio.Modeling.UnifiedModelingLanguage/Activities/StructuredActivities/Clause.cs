namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public partial interface Clause : Element
        {
        ExecutableNode[] Body { get; }
        OutputPin Decider { get; }
        Clause[] PredecessorClause { get; }
        Clause[] SuccessorClause { get; }
        ExecutableNode[] Test { get; }
        }
    }