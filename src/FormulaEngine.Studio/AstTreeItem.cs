using System.Windows.Controls;
using FormulaEngine.Core.Ast;

namespace FormulaEngine.Studio;

public sealed class AstTreeItem
{
    public AstNode Node { get; }
    public string Text { get; }
    public List<AstTreeItem> Children { get; } = new();

    public AstTreeItem(AstNode node, string text)
    {
        Node = node;
        Text = text;
    }

    /// <summary>Builds the WPF tree and records each node's TreeViewItem in <paramref name="map"/>
    /// so the step-through UI can look one up by AST node to highlight it.</summary>
    public TreeViewItem ToTreeViewItem(Dictionary<AstNode, TreeViewItem> map)
    {
        var item = new TreeViewItem { Header = Text, IsExpanded = true };
        map[Node] = item;
        foreach (var child in Children)
            item.Items.Add(child.ToTreeViewItem(map));
        return item;
    }
}
