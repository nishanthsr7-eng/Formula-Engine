using FormulaEngine.Core.Functions;

namespace FormulaEngine.Core.Evaluation;

/// <summary>The inputs an evaluation runs against: variable values and the set of
/// callable functions. Pass a custom <see cref="FunctionRegistry"/> to extend or
/// restrict which functions a formula can call.</summary>
public sealed class EvaluationContext
{
    public Dictionary<string, double> Variables { get; }
    public FunctionRegistry Functions { get; }

    public EvaluationContext(Dictionary<string, double>? variables = null, FunctionRegistry? functions = null)
    {
        Variables = variables ?? new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        Functions = functions ?? FunctionRegistry.CreateDefault();
    }
}
