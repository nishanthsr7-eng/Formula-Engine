using FormulaEngine.Core;
using FormulaEngine.Core.Evaluation;
using FormulaEngine.Core.Exceptions;
using Xunit;

namespace FormulaEngine.Tests;

public class EvaluatorTests
{
    [Theory]
    [InlineData("1 + 2 * 3", 7)]
    [InlineData("(1 + 2) * 3", 9)]
    [InlineData("2 ^ 3 ^ 2", 512)]   // right-assoc: 2 ^ (3^2) = 2^9
    [InlineData("-2 ^ 2", -4)]       // unary binds looser than ^: -(2^2)
    [InlineData("10 / 4", 2.5)]
    [InlineData("--5", 5)]           // chained unary minus
    [InlineData("  1   +   2  ", 3)] // whitespace robustness
    [InlineData("SUM(1, MAX(2, 3), 4)", 8)] // nested function calls
    public void Evaluate_Arithmetic_ProducesExpectedResult(string expression, double expected)
    {
        Assert.Equal(expected, Formula.Evaluate(expression));
    }

    [Theory]
    [InlineData("1 = 1", 1)]
    [InlineData("1 = 2", 0)]
    [InlineData("1 <> 2", 1)]
    [InlineData("2 < 1", 0)]
    [InlineData("2 <= 2", 1)]
    [InlineData("3 > 2", 1)]
    [InlineData("3 >= 4", 0)]
    [InlineData("1 + 2 > 2", 1)] // comparison binds looser than +
    public void Evaluate_ComparisonOperators_ReturnOneOrZero(string expression, double expected)
    {
        Assert.Equal(expected, Formula.Evaluate(expression));
    }

    [Theory]
    [InlineData("AND(1, 1)", 1)]
    [InlineData("AND(1, 0)", 0)]
    [InlineData("OR(0, 0)", 0)]
    [InlineData("OR(0, 1)", 1)]
    [InlineData("NOT(0)", 1)]
    [InlineData("NOT(5)", 0)]
    [InlineData("IF(AND(1 > 0, 2 > 0), 10, 20)", 10)]
    public void Evaluate_LogicalFunctions_ComputeCorrectly(string expression, double expected)
    {
        Assert.Equal(expected, Formula.Evaluate(expression));
    }

    [Fact]
    public void Evaluate_SqrtOfNegative_ReturnsNaN()
    {
        Assert.True(double.IsNaN(Formula.Evaluate("SQRT(-1)")));
    }

    [Theory]
    [InlineData("LN(1)", 0)]
    [InlineData("LOG(100)", 2)]
    [InlineData("LOG(8, 2)", 3)]
    [InlineData("EXP(0)", 1)]
    [InlineData("MOD(7, 3)", 1)]
    [InlineData("MOD(-7, 3)", 2)]     // Excel-style: result takes the sign of the divisor
    [InlineData("CEILING(2.1)", 3)]
    [InlineData("FLOOR(2.9)", 2)]
    [InlineData("PI()", System.Math.PI)]
    [InlineData("E()", System.Math.E)]
    public void Evaluate_AdditionalMathFunctions_ComputeCorrectly(string expression, double expected)
    {
        Assert.Equal(expected, Formula.Evaluate(expression), precision: 10);
    }

    [Fact]
    public void Evaluate_Variables_AreSubstituted()
    {
        var context = new EvaluationContext(new Dictionary<string, double> { ["x"] = 5, ["y"] = 2 });
        Assert.Equal(10, Formula.Evaluate("x * y", context));
    }

    [Fact]
    public void Evaluate_UndefinedVariable_ThrowsEvaluationException()
    {
        Assert.Throws<FormulaEvaluationException>(() => Formula.Evaluate("x + 1"));
    }

    [Fact]
    public void Evaluate_DivisionByZero_ThrowsEvaluationException()
    {
        Assert.Throws<FormulaEvaluationException>(() => Formula.Evaluate("1 / 0"));
    }

    [Fact]
    public void Evaluate_UnknownFunction_ThrowsEvaluationException()
    {
        Assert.Throws<FormulaEvaluationException>(() => Formula.Evaluate("NOPE(1)"));
    }

    [Fact]
    public void Evaluate_BuiltInFunctions_ComputeCorrectly()
    {
        Assert.Equal(6, Formula.Evaluate("SUM(1, 2, 3)"));
        Assert.Equal(2, Formula.Evaluate("AVG(1, 2, 3)"));
        Assert.Equal(3, Formula.Evaluate("MAX(1, 3, 2)"));
        Assert.Equal(4, Formula.Evaluate("ABS(-4)"));
        Assert.Equal(1, Formula.Evaluate("IF(0, 5, 1)"));
    }

    [Fact]
    public void Evaluate_FunctionArgumentCountMismatch_ThrowsEvaluationException()
    {
        Assert.Throws<FormulaEvaluationException>(() => Formula.Evaluate("ABS(1, 2)"));
    }
}
