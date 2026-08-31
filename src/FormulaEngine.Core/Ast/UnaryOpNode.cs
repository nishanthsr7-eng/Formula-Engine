namespace FormulaEngine.Core.Ast;

public enum UnaryOperator
{
    Negate
}

public sealed class UnaryOpNode : AstNode
{
    public UnaryOperator Operator { get; }
    public AstNode Operand { get; }

    public UnaryOpNode(UnaryOperator @operator, AstNode operand)
    {
        Operator = @operator;
        Operand = operand;
    }

    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitUnaryOp(this);
}
