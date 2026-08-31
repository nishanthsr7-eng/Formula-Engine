using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FormulaEngine.Core;
using FormulaEngine.Core.Ast;
using FormulaEngine.Core.Evaluation;
using FormulaEngine.Core.Exceptions;
using FormulaEngine.Core.Functions;

namespace FormulaEngine.Studio;

public partial class MainWindow : Window
{
    private readonly Brush _defaultInputBorder;
    private Dictionary<AstNode, TreeViewItem> _nodeToTreeViewItem = new();
    private TreeViewItem? _highlightedItem;
    private IReadOnlyList<EvaluationStep> _steps = Array.Empty<EvaluationStep>();
    private int _stepIndex = -1;

    public MainWindow()
    {
        InitializeComponent();
        _defaultInputBorder = FormulaInput.BorderBrush;
        FunctionsList.ItemsSource = FunctionRegistry.CreateDefault().All
            .OrderBy(f => f.Name)
            .Select(f => $"{f.Name}({FormatArity(f)})")
            .ToList();
    }

    private static string FormatArity(IFunction f) => f.MinArgs == f.MaxArgs
        ? f.MinArgs.ToString()
        : f.MaxArgs == int.MaxValue
            ? $"{f.MinArgs}+"
            : $"{f.MinArgs}-{f.MaxArgs}";

    private void EvaluateButton_Click(object sender, RoutedEventArgs e)
    {
        ResetStepping();
        ClearErrorHighlight();

        try
        {
            var context = new EvaluationContext(ParseVariables(VariablesInput.Text));
            var ast = Formula.Parse(FormulaInput.Text);
            var result = new Evaluator(context).Evaluate(ast);

            ResultText.Foreground = Brushes.Black;
            ResultText.Text = $"= {result}";
            BuildTree(ast);
        }
        catch (FormulaException ex)
        {
            ShowError(ex);
        }
        catch (Exception ex)
        {
            ResultText.Foreground = Brushes.Firebrick;
            ResultText.Text = $"Error: {ex.Message}";
        }
    }

    private void StepThroughButton_Click(object sender, RoutedEventArgs e)
    {
        ResetStepping();
        ClearErrorHighlight();

        try
        {
            var context = new EvaluationContext(ParseVariables(VariablesInput.Text));
            var (_, ast, steps) = Formula.EvaluateWithTrace(FormulaInput.Text, context);

            _steps = steps;
            BuildTree(ast);

            ResultText.Foreground = Brushes.Black;
            ResultText.Text = "Ready — click Next Step.";
            NextStepButton.IsEnabled = _steps.Count > 0;
        }
        catch (FormulaException ex)
        {
            ShowError(ex);
        }
        catch (Exception ex)
        {
            ResultText.Foreground = Brushes.Firebrick;
            ResultText.Text = $"Error: {ex.Message}";
        }
    }

    private void NextStepButton_Click(object sender, RoutedEventArgs e)
    {
        if (_stepIndex + 1 >= _steps.Count) return;

        _stepIndex++;
        var step = _steps[_stepIndex];

        Highlight(step.Node);
        StepText.Text = $"Step {_stepIndex + 1}/{_steps.Count}: {step.Description}";

        if (_stepIndex == _steps.Count - 1)
        {
            ResultText.Text = $"= {step.Value}";
            NextStepButton.IsEnabled = false;
        }
        else
        {
            ResultText.Text = $"→ {step.Value} so far…";
        }
    }

    private void BuildTree(AstNode ast)
    {
        AstTreeView.Items.Clear();
        _nodeToTreeViewItem = new Dictionary<AstNode, TreeViewItem>();
        AstTreeView.Items.Add(AstTreeBuilder.Build(ast).ToTreeViewItem(_nodeToTreeViewItem));
    }

    private void Highlight(AstNode node)
    {
        if (_highlightedItem != null)
            _highlightedItem.Background = Brushes.Transparent;

        if (!_nodeToTreeViewItem.TryGetValue(node, out var item)) return;

        item.Background = Brushes.Yellow;
        item.BringIntoView();
        _highlightedItem = item;
    }

    private void ResetStepping()
    {
        _steps = Array.Empty<EvaluationStep>();
        _stepIndex = -1;
        _highlightedItem = null;
        StepText.Text = string.Empty;
        NextStepButton.IsEnabled = false;
    }

    private void ShowError(FormulaException ex)
    {
        ResultText.Foreground = Brushes.Firebrick;
        ResultText.Text = $"Error at position {ex.Position}: {ex.Message}";

        FormulaInput.BorderBrush = Brushes.Firebrick;
        FormulaInput.BorderThickness = new Thickness(2);

        if (ex.Position >= 0 && ex.Position < FormulaInput.Text.Length)
        {
            FormulaInput.Focus();
            FormulaInput.Select(ex.Position, FormulaInput.Text.Length - ex.Position);
        }
    }

    private void ClearErrorHighlight()
    {
        FormulaInput.BorderBrush = _defaultInputBorder;
        FormulaInput.BorderThickness = new Thickness(1);
    }

    private static Dictionary<string, double> ParseVariables(string text)
    {
        var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        foreach (var line in text.Split('\n'))
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0) continue;

            var parts = trimmed.Split('=', 2);
            if (parts.Length != 2) continue;

            var name = parts[0].Trim();
            if (double.TryParse(parts[1].Trim(), System.Globalization.CultureInfo.InvariantCulture, out var value))
                result[name] = value;
        }

        return result;
    }
}
