using FormulaEngine.Core.Ast;
using FormulaEngine.Core.Exceptions;
using FormulaEngine.Core.Lexing;

namespace FormulaEngine.Core.Parsing;

/// <summary>
/// Recursive-descent parser. Precedence, loosest to tightest:
/// comparison (= &lt;&gt; &lt; &lt;= &gt; &gt;=, left-assoc)
/// then + - (left-assoc), then * / (left-assoc), then unary minus, then ^ (right-assoc).
/// Unary minus binds looser than ^, matching standard math convention: -2^2 == -4.
/// </summary>
public sealed class Parser
{
    private readonly IReadOnlyList<Token> _tokens;
    private int _pos;

    public Parser(IReadOnlyList<Token> tokens)
    {
        _tokens = tokens;
        _pos = 0;
    }

    public static AstNode Parse(string expression)
    {
        var tokens = new Lexer(expression).Tokenize();
        var parser = new Parser(tokens);
        var node = parser.ParseComparison();
        parser.Expect(TokenType.EndOfInput);
        return node;
    }

    private Token Current => _tokens[_pos];

    private Token Advance()
    {
        var token = Current;
        if (_pos < _tokens.Count - 1) _pos++;
        return token;
    }

    private Token Expect(TokenType type)
    {
        if (Current.Type != type)
            throw new FormulaParseException($"Expected {type} but found {Current.Type} ('{Current.Text}')", Current.Position);
        return Advance();
    }

    private AstNode ParseComparison()
    {
        var left = ParseAdditive();

        while (Current.Type is TokenType.Equal or TokenType.NotEqual or TokenType.Less
               or TokenType.LessEqual or TokenType.Greater or TokenType.GreaterEqual)
        {
            var op = Advance().Type switch
            {
                TokenType.Equal => BinaryOperator.Equal,
                TokenType.NotEqual => BinaryOperator.NotEqual,
                TokenType.Less => BinaryOperator.Less,
                TokenType.LessEqual => BinaryOperator.LessEqual,
                TokenType.Greater => BinaryOperator.Greater,
                _ => BinaryOperator.GreaterEqual
            };
            left = new BinaryOpNode(left, op, ParseAdditive());
        }

        return left;
    }

    private AstNode ParseAdditive()
    {
        var left = ParseTerm();

        while (Current.Type is TokenType.Plus or TokenType.Minus)
        {
            var op = Advance().Type == TokenType.Plus ? BinaryOperator.Add : BinaryOperator.Subtract;
            left = new BinaryOpNode(left, op, ParseTerm());
        }

        return left;
    }

    private AstNode ParseTerm()
    {
        var left = ParseUnary();

        while (Current.Type is TokenType.Star or TokenType.Slash)
        {
            var op = Advance().Type == TokenType.Star ? BinaryOperator.Multiply : BinaryOperator.Divide;
            left = new BinaryOpNode(left, op, ParseUnary());
        }

        return left;
    }

    private AstNode ParseUnary()
    {
        if (Current.Type == TokenType.Minus)
        {
            Advance();
            return new UnaryOpNode(UnaryOperator.Negate, ParseUnary());
        }

        return ParsePower();
    }

    private AstNode ParsePower()
    {
        var left = ParsePrimary();

        if (Current.Type == TokenType.Caret)
        {
            Advance();
            var right = ParseUnary(); // right-associative; also allows "2^-1"
            return new BinaryOpNode(left, BinaryOperator.Power, right);
        }

        return left;
    }

    private AstNode ParsePrimary()
    {
        switch (Current.Type)
        {
            case TokenType.Number:
                return new NumberNode(Advance().NumberValue);

            case TokenType.LParen:
            {
                Advance();
                var expr = ParseComparison();
                Expect(TokenType.RParen);
                return expr;
            }

            case TokenType.Identifier:
            {
                var name = Advance().Text;

                if (Current.Type != TokenType.LParen)
                    return new VariableNode(name);

                Advance();
                var args = new List<AstNode>();

                if (Current.Type != TokenType.RParen)
                {
                    args.Add(ParseComparison());
                    while (Current.Type == TokenType.Comma)
                    {
                        Advance();
                        args.Add(ParseComparison());
                    }
                }

                Expect(TokenType.RParen);
                return new FunctionCallNode(name, args);
            }

            default:
                throw new FormulaParseException($"Unexpected token {Current.Type} ('{Current.Text}')", Current.Position);
        }
    }
}
