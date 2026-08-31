namespace FormulaEngine.Core.Functions;

public static class BuiltInFunctions
{
    public static IReadOnlyList<IFunction> All { get; } = new IFunction[]
    {
        new DelegateFunction("SUM", 1, int.MaxValue, args => args.Sum()),
        new DelegateFunction("AVG", 1, int.MaxValue, args => args.Average()),
        new DelegateFunction("MIN", 1, int.MaxValue, args => args.Min()),
        new DelegateFunction("MAX", 1, int.MaxValue, args => args.Max()),
        new DelegateFunction("ABS", 1, 1, args => Math.Abs(args[0])),
        new DelegateFunction("SQRT", 1, 1, args => Math.Sqrt(args[0])),
        new DelegateFunction("POW", 2, 2, args => Math.Pow(args[0], args[1])),
        new DelegateFunction("ROUND", 1, 2, args => args.Count == 2
            ? Math.Round(args[0], (int)args[1])
            : Math.Round(args[0])),
        new DelegateFunction("IF", 3, 3, args => args[0] != 0 ? args[1] : args[2]),
        new DelegateFunction("AND", 1, int.MaxValue, args => args.All(a => a != 0) ? 1 : 0),
        new DelegateFunction("OR", 1, int.MaxValue, args => args.Any(a => a != 0) ? 1 : 0),
        new DelegateFunction("NOT", 1, 1, args => args[0] == 0 ? 1 : 0),
        new DelegateFunction("LN", 1, 1, args => Math.Log(args[0])),
        new DelegateFunction("LOG", 1, 2, args => args.Count == 2
            ? Math.Log(args[0], args[1])
            : Math.Log10(args[0])),
        new DelegateFunction("EXP", 1, 1, args => Math.Exp(args[0])),
        new DelegateFunction("MOD", 2, 2, args => args[0] - args[1] * Math.Floor(args[0] / args[1])),
        new DelegateFunction("CEILING", 1, 1, args => Math.Ceiling(args[0])),
        new DelegateFunction("FLOOR", 1, 1, args => Math.Floor(args[0])),
        new DelegateFunction("PI", 0, 0, _ => Math.PI),
        new DelegateFunction("E", 0, 0, _ => Math.E),
    };
}
