using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.CheckerTests;

public static class BinaryExpressionsData
{
    public static readonly List<string> ValidNames = new()
    {
        "addIntegers",
        "subtractIntegers",
        "addDoubles",
        "addIntAndDouble",
        "chainAddition",
        "power",
        "multiplyIntegers",
        "divideIntegers",
        "doubleModulo",
        "concatenation",
        "compareIntegers",
        "compareDoubles",
        "compareIntAndDouble",
        "compareStrings",
        "compareChars",
        "compareBooleans",
    };

    public static readonly List<string> InvalidNames = new() { };

    public static readonly Dictionary<string, Expression> Inputs = new()
    {
        {
            "addIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "subtractIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "addDoubles",
            new BinaryExpression(
                new LiteralExpression(2.2) { Type = ExpressionType.Double, Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2.2) { Type = ExpressionType.Double, Line = 1 }
            ) { Line = 1 }
        },
        {
            "addIntAndDouble",
            new BinaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2.2) { Type = ExpressionType.Double, Line = 1 }
            ) { Line = 1 }
        },
        {
            "chainAddition",
            new BinaryExpression(
                new BinaryExpression(
                    new LiteralExpression(2.2) { Type = ExpressionType.Double, Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(2.2) { Type = ExpressionType.Double, Line = 1 }
                ) { Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2.2) { Type = ExpressionType.Double, Line = 1 }
            ) { Line = 1 }
        },
        {
            "power",
            new BinaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.Hat, "^", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "multiplyIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.Asterisk, "*", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "divideIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.Slash, "/", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "doubleModulo",
            new BinaryExpression(
                new LiteralExpression(22.3d) { Type = ExpressionType.Double, Line = 1 },
                new Token(TokenKind.Modulo, "%", null, 1),
                new LiteralExpression(2.2d) { Type = ExpressionType.Double, Line = 1 }
            ) { Line = 1 }
        },
        {
            "concatenation",
            new BinaryExpression(
                new LiteralExpression("hello ") { Type = ExpressionType.String, Line = 1 },
                new Token(TokenKind.DoublePlus, "++", null, 1),
                new LiteralExpression("birds") { Type = ExpressionType.String, Line = 1 }
            ) { Line = 1 }
        },
        {
            "compareIntegers",
            new BinaryExpression(
                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.LessThan, "<", null, 1),
                new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "compareDoubles",
            new BinaryExpression(
                new LiteralExpression(1.1d) { Type = ExpressionType.Double, Line = 1 },
                new Token(TokenKind.GreaterThan, ">", null, 1),
                new LiteralExpression(3.2d) { Type = ExpressionType.Double, Line = 1 }
            ) { Line = 1 }
        },
        {
            "compareIntAndDouble",
            new BinaryExpression(
                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.LessEqual, "<=", null, 1),
                new LiteralExpression(3.2d) { Type = ExpressionType.Double, Line = 1 }
            ) { Line = 1 }
        },
        {
            "compareStrings",
            new BinaryExpression(
                new LiteralExpression("asdf") { Type = ExpressionType.String, Line = 1 },
                new Token(TokenKind.GreaterEqual, ">=", null, 1),
                new LiteralExpression("3.2d") { Type = ExpressionType.String, Line = 1 }
            ) { Line = 1 }
        },
        {
            "compareChars",
            new BinaryExpression(
                new LiteralExpression('a') { Type = ExpressionType.Char, Line = 1 },
                new Token(TokenKind.DoubleEqual, "==", null, 1),
                new LiteralExpression('e') { Type = ExpressionType.Char, Line = 1 }
            ) { Line = 1 }
        },
        {
            "compareBooleans",
            new BinaryExpression(
                new LiteralExpression(false) { Type = ExpressionType.Boolean, Line = 1 },
                new Token(TokenKind.NotEqual, "!=", null, 1),
                new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 }
            ) { Line = 1 }
        },
    };

    public static readonly Dictionary<string, Expression> Outputs = new()
    {
        {
            "addIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "subtractIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "addDoubles",
            new BinaryExpression(
                new LiteralExpression(2.2) { Type = ExpressionType.Double, Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2.2) { Type = ExpressionType.Double, Line = 1 }
            ) { Type = ExpressionType.Double, Line = 1 }
        },
        {
            "addIntAndDouble",
            new BinaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Double, Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2.2) { Type = ExpressionType.Double, Line = 1 }
            ) { Type = ExpressionType.Double, Line = 1 }
        },
        {
            "chainAddition",
            new BinaryExpression(
                new BinaryExpression(
                    new LiteralExpression(2.2) { Type = ExpressionType.Double, Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(2.2) { Type = ExpressionType.Double, Line = 1 }
                ) { Type = ExpressionType.Double, Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2.2) { Type = ExpressionType.Double, Line = 1 }
            ) { Type = ExpressionType.Double, Line = 1 }
        },
        {
            "power",
            new BinaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.Hat, "^", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "multiplyIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.Asterisk, "*", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "divideIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.Slash, "/", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "doubleModulo",
            new BinaryExpression(
                new LiteralExpression(22.3d) { Type = ExpressionType.Double, Line = 1 },
                new Token(TokenKind.Modulo, "%", null, 1),
                new LiteralExpression(2.2d) { Type = ExpressionType.Double, Line = 1 }
            ) { Type = ExpressionType.Double, Line = 1 }
        },
        {
            "concatenation",
            new BinaryExpression(
                new LiteralExpression("hello ") { Type = ExpressionType.String, Line = 1 },
                new Token(TokenKind.DoublePlus, "++", null, 1),
                new LiteralExpression("birds") { Type = ExpressionType.String, Line = 1 }
            ) { Type = ExpressionType.String, Line = 1 }
        },
        {
            "compareIntegers",
            new BinaryExpression(
                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.LessThan, "<", null, 1),
                new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Boolean, Line = 1 }
        },
        {
            "compareDoubles",
            new BinaryExpression(
                new LiteralExpression(1.1d) { Type = ExpressionType.Double, Line = 1 },
                new Token(TokenKind.GreaterThan, ">", null, 1),
                new LiteralExpression(3.2d) { Type = ExpressionType.Double, Line = 1 }
            ) { Type = ExpressionType.Boolean, Line = 1 }
        },
        {
            "compareIntAndDouble",
            new BinaryExpression(
                new LiteralExpression(1) { Type = ExpressionType.Double, Line = 1 },
                new Token(TokenKind.LessEqual, "<=", null, 1),
                new LiteralExpression(3.2d) { Type = ExpressionType.Double, Line = 1 }
            ) { Type = ExpressionType.Boolean, Line = 1 }
        },
        {
            "compareStrings",
            new BinaryExpression(
                new LiteralExpression("asdf") { Type = ExpressionType.String, Line = 1 },
                new Token(TokenKind.GreaterEqual, ">=", null, 1),
                new LiteralExpression("3.2d") { Type = ExpressionType.String, Line = 1 }
            ) { Type = ExpressionType.Boolean, Line = 1 }
        },
        {
            "compareChars",
            new BinaryExpression(
                new LiteralExpression('a') { Type = ExpressionType.Char, Line = 1 },
                new Token(TokenKind.DoubleEqual, "==", null, 1),
                new LiteralExpression('e') { Type = ExpressionType.Char, Line = 1 }
            ) { Type = ExpressionType.Boolean, Line = 1 }
        },
        {
            "compareBooleans",
            new BinaryExpression(
                new LiteralExpression(false) { Type = ExpressionType.Boolean, Line = 1 },
                new Token(TokenKind.NotEqual, "!=", null, 1),
                new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 }
            ) { Type = ExpressionType.Boolean, Line = 1 }
        },
    };
}