using FormulaEngine.Core.Exceptions;

namespace FormulaEngine.Core.Lexing;

/// <summary>Converts a formula string into a flat list of <see cref="Token"/>s
/// (numbers, identifiers, operators, punctuation), terminated by <see cref="TokenType.EndOfInput"/>.</summary>
public sealed class Lexer
{
    private readonly string _text;
    private int _pos;

    public Lexer(string text)
    {
        _text = text;
        _pos = 0;
    }

    /// <exception cref="Exceptions.FormulaLexException">An unrecognized character or malformed
    /// number literal was found; <see cref="Exceptions.FormulaException.Position"/> points at it.</exception>
    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (true)
        {
            SkipWhitespace();

            if (_pos >= _text.Length)
            {
                tokens.Add(new Token(TokenType.EndOfInput, string.Empty, _pos));
                break;
            }

            char c = _text[_pos];

            if (char.IsDigit(c) || (c == '.' && _pos + 1 < _text.Length && char.IsDigit(_text[_pos + 1])))
            {
                tokens.Add(ReadNumber());
                continue;
            }

            if (char.IsLetter(c) || c == '_')
            {
                tokens.Add(ReadIdentifier());
                continue;
            }

            switch (c)
            {
                case '+': tokens.Add(new Token(TokenType.Plus, "+", _pos)); _pos++; break;
                case '-': tokens.Add(new Token(TokenType.Minus, "-", _pos)); _pos++; break;
                case '*': tokens.Add(new Token(TokenType.Star, "*", _pos)); _pos++; break;
                case '/': tokens.Add(new Token(TokenType.Slash, "/", _pos)); _pos++; break;
                case '^': tokens.Add(new Token(TokenType.Caret, "^", _pos)); _pos++; break;
                case '(': tokens.Add(new Token(TokenType.LParen, "(", _pos)); _pos++; break;
                case ')': tokens.Add(new Token(TokenType.RParen, ")", _pos)); _pos++; break;
                case ',': tokens.Add(new Token(TokenType.Comma, ",", _pos)); _pos++; break;
                case '=': tokens.Add(new Token(TokenType.Equal, "=", _pos)); _pos++; break;
                case '<':
                    if (_pos + 1 < _text.Length && _text[_pos + 1] == '=')
                    {
                        tokens.Add(new Token(TokenType.LessEqual, "<=", _pos));
                        _pos += 2;
                    }
                    else if (_pos + 1 < _text.Length && _text[_pos + 1] == '>')
                    {
                        tokens.Add(new Token(TokenType.NotEqual, "<>", _pos));
                        _pos += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Less, "<", _pos));
                        _pos++;
                    }
                    break;
                case '>':
                    if (_pos + 1 < _text.Length && _text[_pos + 1] == '=')
                    {
                        tokens.Add(new Token(TokenType.GreaterEqual, ">=", _pos));
                        _pos += 2;
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.Greater, ">", _pos));
                        _pos++;
                    }
                    break;
                default:
                    throw new FormulaLexException($"Unexpected character '{c}'", _pos);
            }
        }

        return tokens;
    }

    private void SkipWhitespace()
    {
        while (_pos < _text.Length && char.IsWhiteSpace(_text[_pos]))
            _pos++;
    }

    private Token ReadNumber()
    {
        int start = _pos;
        bool sawDot = false;

        while (_pos < _text.Length && (char.IsDigit(_text[_pos]) || (_text[_pos] == '.' && !sawDot)))
        {
            if (_text[_pos] == '.') sawDot = true;
            _pos++;
        }

        string text = _text.Substring(start, _pos - start);
        if (!double.TryParse(text, System.Globalization.CultureInfo.InvariantCulture, out double value))
            throw new FormulaLexException($"Invalid number literal '{text}'", start);

        return new Token(TokenType.Number, text, start, value);
    }

    private Token ReadIdentifier()
    {
        int start = _pos;

        while (_pos < _text.Length && (char.IsLetterOrDigit(_text[_pos]) || _text[_pos] == '_'))
            _pos++;

        string text = _text.Substring(start, _pos - start);
        return new Token(TokenType.Identifier, text, start);
    }
}
