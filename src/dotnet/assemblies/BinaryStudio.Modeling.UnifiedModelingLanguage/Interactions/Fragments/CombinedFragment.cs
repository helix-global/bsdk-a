namespace BinaryStudio.Modeling.UnifiedModelingLanguage
    {
    public interface CombinedFragment : InteractionFragment
        {
        Gate[] CfragmentGate { get; }
        InteractionOperatorKind InteractionOperator { get; }
        InteractionOperand[] Operand { get; }
        }
    }