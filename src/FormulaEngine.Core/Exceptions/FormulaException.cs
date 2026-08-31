namespace FormulaEngine.Core.Exceptions;

/// <summary>Base of every error the engine raises. <see cref="Position"/> is the character
/// offset into the source expression where the problem was found (-1 when not applicable,
/// e.g. a runtime error like division by zero that isn't tied to one character).</summary>
public abstract class FormulaException : Exception
{
    public int Position { get; }

    protected FormulaException(string message, int position) : base(message)
    {
        Position = position;
    }
}
