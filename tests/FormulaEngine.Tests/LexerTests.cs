using FormulaEngine.Core.Exceptions;
using FormulaEngine.Core.Lexing;
using Xunit;

namespace FormulaEngine.Tests;

public class LexerTests
{
    [Fact]
    public void Tokenize_SimpleExpression_ProducesExpectedTokens()
    {
        var tokens = new Lexer("1 + 2").Tokenize();

        Assert.Equal(TokenType.Number, tokens[0].Type);
        Assert.Equal(TokenType.Plus, tokens[1].Type);
        Assert.Equal(TokenType.Number, tokens[2].Type);
        Assert.Equal(TokenType.EndOfInput, tokens[3].Type);
    }

    [Fact]
    public void Tokenize_DecimalNumber_ParsesValue()
    {
        var tokens = new Lexer("3.14").Tokenize();
        Assert.Equal(3.14, tokens[0].NumberValue);
    }

    [Fact]
    public void Tokenize_Identifier_IsRecognized()
    {
        var tokens = new Lexer("x1_2").Tokenize();
        Assert.Equal(TokenType.Identifier, tokens[0].Type);
        Assert.Equal("x1_2", tokens[0].Text);
    }

    [Fact]
    public void Tokenize_InvalidCharacter_ThrowsLexException()
    {
        Assert.Throws<FormulaLexException>(() => new Lexer("1 $ 2").Tokenize());
    }

    [Theory]
    [InlineData("=", TokenType.Equal)]
    [InlineData("<>", TokenType.NotEqual)]
    [InlineData("<", TokenType.Less)]
    [InlineData("<=", TokenType.LessEqual)]
    [InlineData(">", TokenType.Greater)]
    [InlineData(">=", TokenType.GreaterEqual)]
    public void Tokenize_ComparisonOperators_AreRecognized(string text, TokenType expected)
    {
        var tokens = new Lexer(text).Tokenize();
        Assert.Equal(expected, tokens[0].Type);
        Assert.Equal(text, tokens[0].Text);
    }

    [Fact]
    public void Tokenize_InvalidCharacter_ReportsExactPosition()
    {
        var ex = Assert.Throws<FormulaLexException>(() => new Lexer("1 + 2 + $").Tokenize());
        Assert.Equal(8, ex.Position);
    }
}
