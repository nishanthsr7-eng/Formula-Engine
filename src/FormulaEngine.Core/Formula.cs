using FormulaEngine.Core.Ast;
using FormulaEngine.Core.Evaluation;
using FormulaEngine.Core.Parsing;

namespace FormulaEngine.Core;

/// <summary>Facade tying the lexer, parser and evaluator together.</summary>
public static class Formula
{
    public static AstNode Parse(string expression) => Parser.Parse(expression);

    public static double Evaluate(string expression, EvaluationContext? context = null)
    {
        context ??= new EvaluationContext();
        var ast = Parse(expression);
        return new Evaluator(context).Evaluate(ast);
    }

    /// <summary>Evaluates and also returns a step-by-step trace, in the order nodes are actually
    /// evaluated, for driving a step-through debugger UI.</summary>
    public static (double Result, AstNode Ast, IReadOnlyList<EvaluationStep> Steps) EvaluateWithTrace(
        string expression, EvaluationContext? context = null)
    {
        context ??= new EvaluationContext();
        var ast = Parse(expression);
        var steps = new List<EvaluationStep>();
        var result = new Evaluator(context, steps).Evaluate(ast);
        return (result, ast, steps);
    }
}
