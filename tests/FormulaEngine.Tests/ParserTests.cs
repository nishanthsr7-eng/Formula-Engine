using FormulaEngine.Core.Ast;
using FormulaEngine.Core.Exceptions;
using FormulaEngine.Core.Parsing;
using Xunit;

namespace FormulaEngine.Tests;

public class ParserTests
{
    [Fact]
    public void Parse_RespectsMultiplicationPrecedenceOverAddition()
    {
        var root = Assert.IsType<BinaryOpNode>(Parser.Parse("1 + 2 * 3"));

        Assert.Equal(BinaryOperator.Add, root.Operator);
        Assert.IsType<NumberNode>(root.Left);
        var right = Assert.IsType<BinaryOpNode>(root.Right);
        Assert.Equal(BinaryOperator.Multiply, right.Operator);
    }

    [Fact]
    public void Parse_Parentheses_OverridePrecedence()
    {
        var root = Assert.IsType<BinaryOpNode>(Parser.Parse("(1 + 2) * 3"));

        Assert.Equal(BinaryOperator.Multiply, root.Operator);
        Assert.IsType<BinaryOpNode>(root.Left);
    }

    [Fact]
    public void Parse_PowerIsRightAssociative()
    {
        // 2 ^ 3 ^ 2 should parse as 2 ^ (3 ^ 2)
        var root = Assert.IsType<BinaryOpNode>(Parser.Parse("2 ^ 3 ^ 2"));

        Assert.Equal(BinaryOperator.Power, root.Operator);
        Assert.IsType<NumberNode>(root.Left);
        Assert.IsType<BinaryOpNode>(root.Right);
    }

    [Fact]
    public void Parse_FunctionCall_CollectsArguments()
    {
        var call = Assert.IsType<FunctionCallNode>(Parser.Parse("SUM(1, 2, x)"));

        Assert.Equal("SUM", call.Name);
        Assert.Equal(3, call.Arguments.Count);
    }

    [Fact]
    public void Parse_UnbalancedParentheses_ThrowsParseException()
    {
        Assert.Throws<FormulaParseException>(() => Parser.Parse("(1 + 2"));
    }

    [Fact]
    public void Parse_UnexpectedToken_ThrowsParseException()
    {
        Assert.Throws<FormulaParseException>(() => Parser.Parse("1 + * 2"));
    }

    [Fact]
    public void Parse_ComparisonBindsLooserThanAddition()
    {
        // "1 + 2 > 2" should parse as (1 + 2) > 2, not 1 + (2 > 2)
        var root = Assert.IsType<BinaryOpNode>(Parser.Parse("1 + 2 > 2"));

        Assert.Equal(BinaryOperator.Greater, root.Operator);
        var left = Assert.IsType<BinaryOpNode>(root.Left);
        Assert.Equal(BinaryOperator.Add, left.Operator);
    }

    [Theory]
    [InlineData("1 = 1", BinaryOperator.Equal)]
    [InlineData("1 <> 2", BinaryOperator.NotEqual)]
    [InlineData("1 < 2", BinaryOperator.Less)]
    [InlineData("1 <= 2", BinaryOperator.LessEqual)]
    [InlineData("2 > 1", BinaryOperator.Greater)]
    [InlineData("2 >= 1", BinaryOperator.GreaterEqual)]
    public void Parse_ComparisonOperators_ProduceExpectedNode(string expression, BinaryOperator expected)
    {
        var root = Assert.IsType<BinaryOpNode>(Parser.Parse(expression));
        Assert.Equal(expected, root.Operator);
    }

    [Fact]
    public void Parse_ChainedUnaryMinus_NestsUnaryNodes()
    {
        var root = Assert.IsType<UnaryOpNode>(Parser.Parse("--5"));
        Assert.Equal(UnaryOperator.Negate, root.Operator);
        Assert.IsType<UnaryOpNode>(root.Operand);
    }

    [Fact]
    public void Parse_NestedFunctionCalls_AreSupported()
    {
        var call = Assert.IsType<FunctionCallNode>(Parser.Parse("SUM(1, MAX(2, 3), 4)"));
        Assert.Equal(3, call.Arguments.Count);
        Assert.IsType<FunctionCallNode>(call.Arguments[1]);
    }

    [Fact]
    public void Parse_ComparisonInsideFunctionArgument_IsSupported()
    {
        var call = Assert.IsType<FunctionCallNode>(Parser.Parse("IF(x > 5, 1, 0)"));
        Assert.IsType<BinaryOpNode>(call.Arguments[0]);
    }
}
