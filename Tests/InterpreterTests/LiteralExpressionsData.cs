using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.InterpreterTests;

public static class LiteralExpressionsData
{
    public static readonly List<string> ValidNames = new()
    {
        "int",
        "double",
        "bool",
        "char",
        "string",
    };

    public static readonly List<string> InvalidNames = new() { };

    public static readonly Dictionary<string, Expression> Inputs = new()
    {
        { "int", new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 } },
        { "double", new LiteralExpression(1.2d) { Type = ExpressionType.Double, Line = 1 } },
        { "bool", new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 } },
        { "char", new LiteralExpression('a') { Type = ExpressionType.Char, Line = 1 } },
        { "string", new LiteralExpression("abc") { Type = ExpressionType.String, Line = 1 } },
    };

    public static readonly Dictionary<string, object?> Outputs = new()
    {
        { "int", 1 },
        { "double", 1.2d },
        { "bool", true },
        { "char", 'a' },
        { "string", "abc" },
    };
}