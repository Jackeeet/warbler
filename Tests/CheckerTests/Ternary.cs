using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.CheckerTests;

public static class Ternary
{
    public static readonly List<string> ValidNames = new()
    {
        "comparisonCondition",
        "equalityCondition",
        "numericBranches"
    };

    public static readonly List<string> InvalidNames = new()
    {
        "intCondition",
        "doubleCondition",
        "charCondition",
        "stringCondition",
        "mismatchingBranches"
    };

    public static readonly Dictionary<string, Expression> Inputs = new()
    {
        {
            "comparisonCondition",
            new TernaryExpression(
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.LessThan, "<", null, 1),
                    new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 },
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "equalityCondition",
            new TernaryExpression(
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.DoubleEqual, "==", null, 1),
                    new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 },
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "numericBranches",
            new TernaryExpression(
                new LiteralExpression(false) { Type = ExpressionType.Boolean, Line = 1 },
                new LiteralExpression(2.0d) { Type = ExpressionType.Double, Line = 1 },
                new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "intCondition",
            new TernaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 },
                new LiteralExpression(4) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "doubleCondition",
            new TernaryExpression(
                new LiteralExpression(2.2d) { Type = ExpressionType.Double, Line = 1 },
                new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 },
                new LiteralExpression(4) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "charCondition",
            new TernaryExpression(
                new LiteralExpression('2') { Type = ExpressionType.Char, Line = 1 },
                new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 },
                new LiteralExpression(4) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "stringCondition",
            new TernaryExpression(
                new LiteralExpression("2") { Type = ExpressionType.String, Line = 1 },
                new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 },
                new LiteralExpression(4) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "mismatchingBranches",
            new TernaryExpression(
                new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 },
                new LiteralExpression(false) { Type = ExpressionType.Boolean, Line = 1 },
                new LiteralExpression(4) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
    };

    public static readonly Dictionary<string, Expression> Outputs = new()
    {
        {
            "comparisonCondition",
            new TernaryExpression(
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.LessThan, "<", null, 1),
                    new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                ) { Type = ExpressionType.Boolean, Line = 1 },
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "equalityCondition",
            new TernaryExpression(
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.DoubleEqual, "==", null, 1),
                    new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                ) { Type = ExpressionType.Boolean, Line = 1 },
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "numericBranches",
            new TernaryExpression(
                new LiteralExpression(false) { Type = ExpressionType.Boolean, Line = 1 },
                new LiteralExpression(2.0d) { Type = ExpressionType.Double, Line = 1 },
                new LiteralExpression(3) { Type = ExpressionType.Double, Line = 1 }
            ) { Type = ExpressionType.Double, Line = 1 }
        },
    };
}