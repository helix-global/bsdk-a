namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public enum InteractionOperatorKind
        {
        Seq,
        Alt,
        Opt,
        Break,
        Par,
        Strict,
        Loop,
        Critical,
        Neg,
        Assert,
        Ignore,
        Consider
        }
    }