namespace FormulaEngine.Core.Exceptions;

public sealed class FormulaLexException : FormulaException
{
    public FormulaLexException(string message, int position) : base(message, position)
    {
    }
}
