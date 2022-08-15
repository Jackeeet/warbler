using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.CheckerTests;

public static class Binary
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

    public static readonly List<string> InvalidNames = new()
    {
        "addStrings",
        "mismatchedAddition",
        "booleanPower",
        "mismatchedPower",
        "mismatchedMultiplication",
        "mismatchedDivision",
        "mismatchedConcatenation",
        "concatIntegers",
        "concatDoubles",
        "concatBooleans",
        "concatChars",
        "mismatchedComparison",
        "mismatchedEquality",
    };

    public static readonly Dictionary<string, Expression> Inputs = new()
    {
        #region valid

        {
            "addIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Line = 1 }
        },
        {
            "subtractIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Line = 1 }
        },
        {
            "addDoubles",
            new BinaryExpression(
                new LiteralExpression(2.2) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2.2) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Line = 1 }
        },
        {
            "addIntAndDouble",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2.2) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Line = 1 }
        },
        {
            "chainAddition",
            new BinaryExpression(
                new BinaryExpression(
                    new LiteralExpression(2.2) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(2.2) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
                ) { Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2.2) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Line = 1 }
        },
        {
            "power",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Hat, "^", null, 1),
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Line = 1 }
        },
        {
            "multiplyIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Asterisk, "*", null, 1),
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Line = 1 }
        },
        {
            "divideIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Slash, "/", null, 1),
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Line = 1 }
        },
        {
            "doubleModulo",
            new BinaryExpression(
                new LiteralExpression(22.3d) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.Percent, "%", null, 1),
                new LiteralExpression(2.2d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Line = 1 }
        },
        {
            "concatenation",
            new BinaryExpression(
                new LiteralExpression("hello ") { Type = new WarblerType(ExpressionType.String), Line = 1 },
                new Token(TokenKind.DoublePlus, "++", null, 1),
                new LiteralExpression("birds") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Line = 1 }
        },
        {
            "compareIntegers",
            new BinaryExpression(
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.LessThan, "<", null, 1),
                new LiteralExpression(3) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Line = 1 }
        },
        {
            "compareDoubles",
            new BinaryExpression(
                new LiteralExpression(1.1d) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.GreaterThan, ">", null, 1),
                new LiteralExpression(3.2d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Line = 1 }
        },
        {
            "compareIntAndDouble",
            new BinaryExpression(
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.LessEqual, "<=", null, 1),
                new LiteralExpression(3.2d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Line = 1 }
        },
        {
            "compareStrings",
            new BinaryExpression(
                new LiteralExpression("asdf") { Type = new WarblerType(ExpressionType.String), Line = 1 },
                new Token(TokenKind.GreaterEqual, ">=", null, 1),
                new LiteralExpression("3.2d") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Line = 1 }
        },
        {
            "compareChars",
            new BinaryExpression(
                new LiteralExpression('a') { Type = new WarblerType(ExpressionType.Char), Line = 1 },
                new Token(TokenKind.DoubleEqual, "==", null, 1),
                new LiteralExpression('e') { Type = new WarblerType(ExpressionType.Char), Line = 1 }
            ) { Line = 1 }
        },
        {
            "compareBooleans",
            new BinaryExpression(
                new LiteralExpression(false) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new Token(TokenKind.NotEqual, "!=", null, 1),
                new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
            ) { Line = 1 }
        },

        #endregion

        {
            "addStrings",
            new BinaryExpression(
                new LiteralExpression("a") { Type = new WarblerType(ExpressionType.String), Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression("b") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Line = 1 }
        },
        {
            "mismatchedAddition",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression("b") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Line = 1 }
        },
        {
            "booleanPower",
            new BinaryExpression(
                new LiteralExpression(false) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new Token(TokenKind.Hat, "^", null, 1),
                new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
            ) { Line = 1 }
        },
        {
            "mismatchedPower",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Hat, "^", null, 1),
                new LiteralExpression("a") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Line = 1 }
        },
        {
            "mismatchedMultiplication",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Asterisk, "*", null, 1),
                new LiteralExpression("a") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Line = 1 }
        },
        {
            "mismatchedDivision",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Slash, "/", null, 1),
                new LiteralExpression("2") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Line = 1 }
        },
        {
            "mismatchedConcatenation",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.DoublePlus, "++", null, 1),
                new LiteralExpression("a") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Line = 1 }
        },
        {
            "concatIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.DoublePlus, "++", null, 1),
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Line = 1 }
        },
        {
            "concatDoubles",
            new BinaryExpression(
                new LiteralExpression(2.2d) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.DoublePlus, "++", null, 1),
                new LiteralExpression(2.2d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Line = 1 }
        },
        {
            "concatBooleans",
            new BinaryExpression(
                new LiteralExpression(false) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new Token(TokenKind.DoublePlus, "++", null, 1),
                new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
            ) { Line = 1 }
        },
        {
            "concatChars",
            new BinaryExpression(
                new LiteralExpression('s') { Type = new WarblerType(ExpressionType.Char), Line = 1 },
                new Token(TokenKind.DoublePlus, "++", null, 1),
                new LiteralExpression('k') { Type = new WarblerType(ExpressionType.Char), Line = 1 }
            ) { Line = 1 }
        },
        {
            "mismatchedComparison",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.LessEqual, "<=", null, 1),
                new LiteralExpression("a") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Line = 1 }
        },
        {
            "mismatchedEquality",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.DoubleEqual, "==", null, 1),
                new LiteralExpression("a") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Line = 1 }
        },
    };

    public static readonly Dictionary<string, Expression> Outputs = new()
    {
        {
            "addIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "subtractIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "addDoubles",
            new BinaryExpression(
                new LiteralExpression(2.2) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2.2) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
        },
        {
            "addIntAndDouble",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2.2) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
        },
        {
            "chainAddition",
            new BinaryExpression(
                new BinaryExpression(
                    new LiteralExpression(2.2) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(2.2) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(2.2) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
        },
        {
            "power",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Hat, "^", null, 1),
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "multiplyIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Asterisk, "*", null, 1),
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "divideIntegers",
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Slash, "/", null, 1),
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "doubleModulo",
            new BinaryExpression(
                new LiteralExpression(22.3d) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.Percent, "%", null, 1),
                new LiteralExpression(2.2d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
        },
        {
            "concatenation",
            new BinaryExpression(
                new LiteralExpression("hello ") { Type = new WarblerType(ExpressionType.String), Line = 1 },
                new Token(TokenKind.DoublePlus, "++", null, 1),
                new LiteralExpression("birds") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.String), Line = 1 }
        },
        {
            "compareIntegers",
            new BinaryExpression(
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.LessThan, "<", null, 1),
                new LiteralExpression(3) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "compareDoubles",
            new BinaryExpression(
                new LiteralExpression(1.1d) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.GreaterThan, ">", null, 1),
                new LiteralExpression(3.2d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "compareIntAndDouble",
            new BinaryExpression(
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.LessEqual, "<=", null, 1),
                new LiteralExpression(3.2d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "compareStrings",
            new BinaryExpression(
                new LiteralExpression("asdf") { Type = new WarblerType(ExpressionType.String), Line = 1 },
                new Token(TokenKind.GreaterEqual, ">=", null, 1),
                new LiteralExpression("3.2d") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "compareChars",
            new BinaryExpression(
                new LiteralExpression('a') { Type = new WarblerType(ExpressionType.Char), Line = 1 },
                new Token(TokenKind.DoubleEqual, "==", null, 1),
                new LiteralExpression('e') { Type = new WarblerType(ExpressionType.Char), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "compareBooleans",
            new BinaryExpression(
                new LiteralExpression(false) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new Token(TokenKind.NotEqual, "!=", null, 1),
                new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
    };
}