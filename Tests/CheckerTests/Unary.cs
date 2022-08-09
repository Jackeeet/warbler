using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.CheckerTests;

public static class Unary
{
    public static readonly List<string> ValidNames = new()
    {
        "minusInteger",
        "minusDouble",
        "minusAddition",
        "notBoolean",
        "notRelational",
        "notEquality"
    };

    public static readonly List<string> InvalidNames = new()
    {
        "notInteger",
        "notDouble",
        "notString",
        "notChar",
        "minusString",
        "minusChar",
        "minusBoolean",
    };

    public static readonly Dictionary<string, Expression> Inputs = new()
    {
        {
            "minusInteger",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer }
            )
        },
        {
            "minusDouble",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(2.0) { Type = ExpressionType.Double }
            )
        },
        {
            "minusAddition",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(2.0d) { Type = ExpressionType.Double }
                )
            )
        },
        {
            "notBoolean",
            new UnaryExpression(
                new Token(TokenKind.Not, "!", null, 1),
                new LiteralExpression(false) { Type = ExpressionType.Boolean }
            )
        },
        {
            "notRelational",
            new UnaryExpression(
                new Token(TokenKind.Not, "!", null, 1),
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer },
                    new Token(TokenKind.LessEqual, "<=", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer }
                )
            )
        },
        {
            "notEquality",
            new UnaryExpression(
                new Token(TokenKind.Not, "!", null, 1),
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer },
                    new Token(TokenKind.DoubleEqual, "==", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer }
                )
            )
        },

        /* ---- invalid inputs ---- */

        {
            "notInteger",
            new UnaryExpression(
                new Token(TokenKind.Not, "!", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "notDouble",
            new UnaryExpression(
                new Token(TokenKind.Not, "!", null, 1),
                new LiteralExpression(2.2d) { Type = ExpressionType.Double, Line = 1 }
            ) { Line = 1 }
        },
        {
            "notString",
            new UnaryExpression(
                new Token(TokenKind.Not, "!", null, 1),
                new LiteralExpression("2") { Type = ExpressionType.String, Line = 1 }
            ) { Line = 1 }
        },
        {
            "notChar",
            new UnaryExpression(
                new Token(TokenKind.Not, "!", null, 1),
                new LiteralExpression('2') { Type = ExpressionType.Char, Line = 1 }
            ) { Line = 1 }
        },
        {
            "minusString",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression("2") { Type = ExpressionType.String, Line = 1 }
            ) { Line = 1 }
        },
        {
            "minusChar",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression('2') { Type = ExpressionType.Char, Line = 1 }
            ) { Line = 1 }
        },
        {
            "minusBoolean",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(false) { Type = ExpressionType.Boolean, Line = 1 }
            ) { Line = 1 }
        },
    };

    public static readonly Dictionary<string, Expression> Outputs = new()
    {
        {
            "minusInteger",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer }
            ) { Type = ExpressionType.Integer }
        },
        {
            "minusDouble",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(2.0d) { Type = ExpressionType.Double }
            ) { Type = ExpressionType.Double }
        },
        {
            "minusAddition",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Double },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(2.0d) { Type = ExpressionType.Double }
                ) { Type = ExpressionType.Double }
            ) { Type = ExpressionType.Double }
        },
        {
            "notBoolean",
            new UnaryExpression(
                new Token(TokenKind.Not, "!", null, 1),
                new LiteralExpression(false) { Type = ExpressionType.Boolean }
            ) { Type = ExpressionType.Boolean }
        },
        {
            "notRelational",
            new UnaryExpression(
                new Token(TokenKind.Not, "!", null, 1),
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer },
                    new Token(TokenKind.LessEqual, "<=", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer }
                ) { Type = ExpressionType.Boolean }
            ) { Type = ExpressionType.Boolean }
        },
        {
            "notEquality",
            new UnaryExpression(
                new Token(TokenKind.Not, "!", null, 1),
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer },
                    new Token(TokenKind.DoubleEqual, "==", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer }
                ) { Type = ExpressionType.Boolean }
            ) { Type = ExpressionType.Boolean }
        },
    };
}