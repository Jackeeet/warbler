using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.CheckerTests;

public static class UnaryExpressionsData
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