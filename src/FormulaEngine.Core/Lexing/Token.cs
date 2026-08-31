namespace FormulaEngine.Core.Lexing;

public readonly struct Token
{
    public TokenType Type { get; }
    public string Text { get; }
    public double NumberValue { get; }
    public int Position { get; }

    public Token(TokenType type, string text, int position, double numberValue = 0)
    {
        Type = type;
        Text = text;
        Position = position;
        NumberValue = numberValue;
    }

    public override string ToString() => $"{Type}('{Text}')";
}
