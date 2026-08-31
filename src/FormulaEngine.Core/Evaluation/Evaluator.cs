using FormulaEngine.Core.Ast;
using FormulaEngine.Core.Exceptions;

namespace FormulaEngine.Core.Evaluation;

/// <summary>Tree-walking evaluator: an <see cref="IAstVisitor{T}"/> that computes a
/// <see cref="double"/> for each node, recursing into children first (post-order).</summary>
public sealed class Evaluator : IAstVisitor<double>
{
    private readonly EvaluationContext _context;
    private readonly List<EvaluationStep>? _trace;

    /// <param name="trace">When supplied, one <see cref="EvaluationStep"/> is appended per node,
    /// in the order nodes actually finish evaluating (post-order) — enough to drive a step-through UI.</param>
    public Evaluator(EvaluationContext context, List<EvaluationStep>? trace = null)
    {
        _context = context;
        _trace = trace;
    }

    public double Evaluate(AstNode node) => node.Accept(this);

    public double VisitNumber(NumberNode node)
    {
        Record(node, node.Value, $"{node.Value}");
        return node.Value;
    }

    public double VisitVariable(VariableNode node)
    {
        if (!_context.Variables.TryGetValue(node.Name, out var value))
            throw new FormulaEvaluationException($"Undefined variable '{node.Name}'");

        Record(node, value, $"{node.Name} = {value}");
        return value;
    }

    public double VisitUnaryOp(UnaryOpNode node)
    {
        var operand = node.Operand.Accept(this);
        var value = node.Operator switch
        {
            UnaryOperator.Negate => -operand,
            _ => throw new FormulaEvaluationException($"Unsupported unary operator '{node.Operator}'")
        };

        Record(node, value, $"-({operand}) = {value}");
        return value;
    }

    public double VisitBinaryOp(BinaryOpNode node)
    {
        var left = node.Left.Accept(this);
        var right = node.Right.Accept(this);

        var value = node.Operator switch
        {
            BinaryOperator.Add => left + right,
            BinaryOperator.Subtract => left - right,
            BinaryOperator.Multiply => left * right,
            BinaryOperator.Divide => right == 0
                ? throw new FormulaEvaluationException("Division by zero")
                : left / right,
            BinaryOperator.Power => Math.Pow(left, right),
            BinaryOperator.Equal => left == right ? 1 : 0,
            BinaryOperator.NotEqual => left != right ? 1 : 0,
            BinaryOperator.Less => left < right ? 1 : 0,
            BinaryOperator.LessEqual => left <= right ? 1 : 0,
            BinaryOperator.Greater => left > right ? 1 : 0,
            BinaryOperator.GreaterEqual => left >= right ? 1 : 0,
            _ => throw new FormulaEvaluationException($"Unsupported binary operator '{node.Operator}'")
        };

        Record(node, value, $"{left} {Symbol(node.Operator)} {right} = {value}");
        return value;
    }

    public double VisitFunctionCall(FunctionCallNode node)
    {
        if (!_context.Functions.TryGet(node.Name, out var function))
            throw new FormulaEvaluationException($"Undefined function '{node.Name}'");

        var args = node.Arguments.Select(a => a.Accept(this)).ToArray();

        if (args.Length < function.MinArgs || args.Length > function.MaxArgs)
            throw new FormulaEvaluationException(
                $"Function '{node.Name}' expects between {function.MinArgs} and {function.MaxArgs} argument(s), got {args.Length}");

        var value = function.Invoke(args);
        Record(node, value, $"{node.Name}({string.Join(", ", args)}) = {value}");
        return value;
    }

    private void Record(AstNode node, double value, string description) =>
        _trace?.Add(new EvaluationStep(node, value, description));

    private static string Symbol(BinaryOperator op) => op switch
    {
        BinaryOperator.Add => "+",
        BinaryOperator.Subtract => "-",
        BinaryOperator.Multiply => "*",
        BinaryOperator.Divide => "/",
        BinaryOperator.Power => "^",
        BinaryOperator.Equal => "=",
        BinaryOperator.NotEqual => "<>",
        BinaryOperator.Less => "<",
        BinaryOperator.LessEqual => "<=",
        BinaryOperator.Greater => ">",
        BinaryOperator.GreaterEqual => ">=",
        _ => "?"
    };
}
