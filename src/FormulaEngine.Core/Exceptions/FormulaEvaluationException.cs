namespace FormulaEngine.Core.Exceptions;

public sealed class FormulaEvaluationException : FormulaException
{
    public FormulaEvaluationException(string message, int position = -1) : base(message, position)
    {
    }
}
