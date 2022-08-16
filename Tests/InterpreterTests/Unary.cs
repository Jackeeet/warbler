using System.Collections.Generic;
using Warbler.Expressions;
using Warbler.Utils.Token;
using Warbler.Utils.Type;

namespace Tests.InterpreterTests;

public static class Unary
{
    public static readonly List<string> ValidNames = new()
    {
        "minusInteger",
        "minusDouble",
        "notBoolean",
    };

    public static readonly List<string> InvalidNames = new()
    {
        "notNumber",
        "minusBoolean",
        "unknownOperator",
        "nullExpression",
    };

    public static readonly Dictionary<string, Expression?> Inputs = new()
    {
        {
            "minusInteger",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "minusDouble",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(1.0d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "notBoolean",
            new UnaryExpression(
                new Token(TokenKind.Not, "!", null, 1),
                new LiteralExpression(false) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },

        {
            "notNumber",
            new UnaryExpression(
                new Token(TokenKind.Not, "!", null, 1),
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "minusBoolean",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "unknownOperator",
            new UnaryExpression(
                new Token(TokenKind.Question, "?", null, 1),
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "nullExpression",
            new UnaryExpression(
                new Token(TokenKind.Minus, "-", null, 1),
                null!
            ) { Line = 1 }
        }
    };

    public static readonly Dictionary<string, object?> Outputs = new()
    {
        { "minusInteger", -1 },
        { "minusDouble", -1.0 },
        { "notBoolean", true },
        { "notNumber", new object() },
        { "minusBoolean", new object() },
        { "unknownOperator", new object() },
        { "nullExpression", new object() },
    };
}