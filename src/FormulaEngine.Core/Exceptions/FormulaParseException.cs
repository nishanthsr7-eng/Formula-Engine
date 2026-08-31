namespace FormulaEngine.Core.Exceptions;

public sealed class FormulaParseException : FormulaException
{
    public FormulaParseException(string message, int position) : base(message, position)
    {
    }
}
