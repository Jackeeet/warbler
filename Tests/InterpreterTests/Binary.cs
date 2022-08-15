using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.InterpreterTests;

public static class Binary
{
    public static readonly List<string> ValidNames = new()
    {
        "addition",
        "intDoubleAddition",
        "subtraction",
        "intDoubleSubtraction",
        "multiplication",
        "intDoubleMultiplication",
        "division",
        "intDoubleDivision",
        "modulo",
        "intDoubleModulo",
        "power",
        "intDoublePower",

        "concatenation",

        "lessThan",
        "intDoubleLessThan",
        "lessEqual",
        "intDoubleLessEqual",
        "greaterThan",
        "intDoubleGreaterThan",
        "greaterEqual",
        "intDoubleGreaterEqual",
        "equal",
        "intDoubleEqual",
        "notEqual",
        "intDoubleNotEqual",
    };

    public static readonly List<string> InvalidNames = new()
    {
        "unexpectedOperator",
        "nullLeftExpression",
        "nullRightExpression"
    };

    public static readonly Dictionary<string, Expression> Inputs = new()
    {
        #region numeric

        {
            "addition", new BinaryExpression(
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "intDoubleAddition", new BinaryExpression(
                new LiteralExpression(1.0d) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.Plus, "+", null, 1),
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
        },
        {
            "subtraction", new BinaryExpression(
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "intDoubleSubtraction", new BinaryExpression(
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(0.5d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
        },
        {
            "multiplication", new BinaryExpression(
                new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Asterisk, "*", null, 1),
                new LiteralExpression(12) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "intDoubleMultiplication", new BinaryExpression(
                new LiteralExpression(12) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.Asterisk, "*", null, 1),
                new LiteralExpression(0.5d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
        },
        {
            "division", new BinaryExpression(
                new LiteralExpression(12) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Slash, "/", null, 1),
                new LiteralExpression(3) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "intDoubleDivision", new BinaryExpression(
                new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.Slash, "/", null, 1),
                new LiteralExpression(3d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
        },
        {
            "modulo", new BinaryExpression(
                new LiteralExpression(12) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Percent, "%", null, 1),
                new LiteralExpression(3) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "intDoubleModulo", new BinaryExpression(
                new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.Percent, "%", null, 1),
                new LiteralExpression(3d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
        },
        {
            "power", new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Hat, "^", null, 1),
                new LiteralExpression(3) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "intDoublePower", new BinaryExpression(
                new LiteralExpression(3) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.Hat, "^", null, 1),
                new LiteralExpression(3d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
        },

        #endregion

        {
            "concatenation", new BinaryExpression(
                new LiteralExpression("warb") { Type = new WarblerType(ExpressionType.String), Line = 1 },
                new Token(TokenKind.DoublePlus, "++", null, 1),
                new LiteralExpression("ler") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.String), Line = 1 }
        },

        #region relational

        {
            "lessThan", new BinaryExpression(
                new LiteralExpression('a') { Type = new WarblerType(ExpressionType.Char), Line = 1 },
                new Token(TokenKind.LessThan, "<", null, 1),
                new LiteralExpression('b') { Type = new WarblerType(ExpressionType.Char), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "intDoubleLessThan", new BinaryExpression(
                new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.LessThan, "<", null, 1),
                new LiteralExpression(2.3d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "lessEqual", new BinaryExpression(
                new LiteralExpression("abc") { Type = new WarblerType(ExpressionType.String), Line = 1 },
                new Token(TokenKind.LessEqual, "<=", null, 1),
                new LiteralExpression("cba") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "intDoubleLessEqual", new BinaryExpression(
                new LiteralExpression(10d) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.LessEqual, "<=", null, 1),
                new LiteralExpression(20) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "greaterThan", new BinaryExpression(
                new LiteralExpression(3) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.GreaterThan, ">", null, 1),
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "intDoubleGreaterThan", new BinaryExpression(
                new LiteralExpression(10d) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.GreaterThan, ">", null, 1),
                new LiteralExpression(20) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "greaterEqual", new BinaryExpression(
                new LiteralExpression(3) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.GreaterEqual, ">=", null, 1),
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "intDoubleGreaterEqual", new BinaryExpression(
                new LiteralExpression(10d) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.GreaterEqual, ">=", null, 1),
                new LiteralExpression(20) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "equal", new BinaryExpression(
                new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new Token(TokenKind.DoubleEqual, "==", null, 1),
                new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "intDoubleEqual", new BinaryExpression(
                new LiteralExpression(20) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.DoubleEqual, "==", null, 1),
                new LiteralExpression(20d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "notEqual", new BinaryExpression(
                new LiteralExpression(20d) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.NotEqual, "!=", null, 1),
                new LiteralExpression(20d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },
        {
            "intDoubleNotEqual", new BinaryExpression(
                new LiteralExpression(20d) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new Token(TokenKind.NotEqual, "!=", null, 1),
                new LiteralExpression(25) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 }
        },

        #endregion

        #region invalid

        {
            "unexpectedOperator", new BinaryExpression(
                new LiteralExpression("warb") { Type = new WarblerType(ExpressionType.String), Line = 1 },
                new Token(TokenKind.Question, "?", null, 1),
                new LiteralExpression("ler") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.String), Line = 1 }
        },
        {
            "nullLeftExpression", new BinaryExpression(
                null!,
                new Token(TokenKind.DoublePlus, "++", null, 1),
                new LiteralExpression("ler") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.String), Line = 1 }
        },
        {
            "nullRightExpression", new BinaryExpression(
                new LiteralExpression("warb") { Type = new WarblerType(ExpressionType.String), Line = 1 },
                new Token(TokenKind.DoublePlus, "++", null, 1),
                null!
            ) { Type = new WarblerType(ExpressionType.String), Line = 1 }
        }

        #endregion
    };

    public static readonly Dictionary<string, object?> Outputs = new()
    {
        #region numeric

        { "addition", 2 },
        { "intDoubleAddition", 2.0d },
        { "subtraction", 0 },
        { "intDoubleSubtraction", 0.5d },
        { "multiplication", 120 },
        { "intDoubleMultiplication", 6.0d },
        { "division", 4 },
        { "intDoubleDivision", 3.333333d },
        { "modulo", 0 },
        { "intDoubleModulo", 1.0d },
        { "power", 8 },
        { "intDoublePower", 27.0d },

        #endregion

        { "concatenation", "warbler" },

        { "lessThan", true },
        { "intDoubleLessThan", false },
        { "lessEqual", true },
        { "intDoubleLessEqual", true },
        { "greaterThan", true },
        { "intDoubleGreaterThan", false },
        { "greaterEqual", true },
        { "intDoubleGreaterEqual", false },
        { "equal", true },
        { "intDoubleEqual", true },
        { "notEqual", false },
        { "intDoubleNotEqual", true },
    };
}