using FormulaEngine.Core.Ast;

namespace FormulaEngine.Studio;

/// <summary>
/// Reuses the Core visitor interface to render an AST for the UI tree view —
/// the same dispatch that drives evaluation drives display.
/// </summary>
public sealed class AstTreeBuilder : IAstVisitor<AstTreeItem>
{
    public static AstTreeItem Build(AstNode node) => node.Accept(new AstTreeBuilder());

    public AstTreeItem VisitNumber(NumberNode node) => new(node, $"Number: {node.Value}");

    public AstTreeItem VisitVariable(VariableNode node) => new(node, $"Variable: {node.Name}");

    public AstTreeItem VisitUnaryOp(UnaryOpNode node)
    {
        var item = new AstTreeItem(node, $"Unary: {node.Operator}");
        item.Children.Add(node.Operand.Accept(this));
        return item;
    }

    public AstTreeItem VisitBinaryOp(BinaryOpNode node)
    {
        var item = new AstTreeItem(node, $"BinaryOp: {node.Operator}");
        item.Children.Add(node.Left.Accept(this));
        item.Children.Add(node.Right.Accept(this));
        return item;
    }

    public AstTreeItem VisitFunctionCall(FunctionCallNode node)
    {
        var item = new AstTreeItem(node, $"Call: {node.Name}()");
        foreach (var arg in node.Arguments)
            item.Children.Add(arg.Accept(this));
        return item;
    }
}
