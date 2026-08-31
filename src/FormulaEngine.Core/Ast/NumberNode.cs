namespace FormulaEngine.Core.Ast;

public sealed class NumberNode : AstNode
{
    public double Value { get; }

    public NumberNode(double value)
    {
        Value = value;
    }

    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitNumber(this);
}
