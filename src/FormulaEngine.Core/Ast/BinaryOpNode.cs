namespace FormulaEngine.Core.Ast;

public enum BinaryOperator
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Power,
    Equal,
    NotEqual,
    Less,
    LessEqual,
    Greater,
    GreaterEqual
}

public sealed class BinaryOpNode : AstNode
{
    public AstNode Left { get; }
    public BinaryOperator Operator { get; }
    public AstNode Right { get; }

    public BinaryOpNode(AstNode left, BinaryOperator @operator, AstNode right)
    {
        Left = left;
        Operator = @operator;
        Right = right;
    }

    public override T Accept<T>(IAstVisitor<T> visitor) => visitor.VisitBinaryOp(this);
}
