using FormulaEngine.Core.Ast;

namespace FormulaEngine.Core.Evaluation;

/// <summary>One completed node evaluation, in the order evaluation actually happens (post-order).</summary>
public sealed class EvaluationStep
{
    public AstNode Node { get; }
    public double Value { get; }
    public string Description { get; }

    public EvaluationStep(AstNode node, double value, string description)
    {
        Node = node;
        Value = value;
        Description = description;
    }
}
