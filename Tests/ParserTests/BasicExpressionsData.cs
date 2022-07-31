using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.ParserTests;

public static class BasicExpressionsData
{
    public static readonly List<string> ValidNames = new()
    {
        "empty",
        "intLiteral",
        "doubleLiteral",
        "stringLiteral",
        "charLiteral",
        "true",
        "false",
        "grouping",
        "raiseToPower",
        "raiseGroupingToPower",
        "raiseToGrouping",
        "chainPower",
        "unaryMinus",
        "unaryNot",
        "unaryGrouping",
        "multiplication",
        "chainMultiplication",
        "division",
        "chainDivision",
        "modulo",
        "chainModulo",
        "addition",
        "chainAddition",
        "subtraction",
        "chainSubtraction",
        "unarySubtraction",
        "concatenation",
        "chainConcatenation",
        "arithmeticPrecedence",
        "greaterThan",
        "lessThan",
        "greaterEqual",
        "lessEqual",
        "relationalPrecedence",
        "equality",
        "inequality",
        "equalityPrecedence",
        "ternary",
        "nestedTernary",
        "ternaryPrecedence"
    };

    public static readonly List<string> InvalidNames = new()
    {
        "chainComparison",
        "chainEquality",
        "unsupportedUnary",
        // "unsupportedBinary",
        "unsupportedTernaryCondition",
        "unsupportedTernaryBranch",
        "noBinaryArg",
        "noBinaryArgs",
        "noTernaryCondition",
        "noTernaryThen",
        "noTernaryElse",
        "noClosingBracket",
        "noOpeningBracket"
    };

    public static readonly Dictionary<string, string> Inputs = new()
    {
        #region validInputs

        { "empty", "" },
        { "intLiteral", "2" },
        { "doubleLiteral", "3.4" },
        { "stringLiteral", "\"hello birds\"" },
        { "charLiteral", "'a'" },
        { "true", "true" },
        { "false", "false" },
        { "grouping", "(2 + 2)" },
        { "raiseToPower", "2 ^ 2" },
        { "raiseGroupingToPower", "(2 + 2) ^ 2" },
        { "raiseToGrouping", "2 ^ (2 + 2)" },
        { "chainPower", "2 ^ 2 ^ 2 ^ 2" },
        { "unaryMinus", "-123" },
        { "unaryNot", "!false" },
        { "unaryGrouping", "-(2 + 2)" },
        { "multiplication", "2 * 2" },
        { "chainMultiplication", "1 * 2 * 3" },
        { "division", "2 / 2" },
        { "chainDivision", "1 / 2 / 3" },
        { "modulo", "2 % 2" },
        { "chainModulo", "1 % 2 % 3" },
        { "addition", "2 + 2" },
        { "chainAddition", "1 + 2 + 3" },
        { "subtraction", "2 - 2" },
        { "chainSubtraction", "1 - 2 - 3" },
        { "unarySubtraction", "-1 - 2" },
        { "concatenation", "\"a\" ++ \"b\"" },
        { "chainConcatenation", "\"a\" ++ \"b\" ++ \"c\"" },
        { "arithmeticPrecedence", "-5 + (1 + 2) ^ 3 * 4" },
        { "greaterThan", "3 > 2" },
        { "lessThan", "2 < 3 " },
        { "greaterEqual", "3 >=2" },
        { "lessEqual", "\"a\" <= \"b\"" },
        { "relationalPrecedence", "2 + 3 <= 5" },
        { "equality", "2 == 2" },
        { "inequality", "2 != 3" },
        { "equalityPrecedence", "2 < 3 == \"a\" < \"b\"" },
        { "ternary", "true ? 1 : 0" },
        { "nestedTernary", "true ? false ? 1 : 2 : true ? 3 : 4" },
        { "ternaryPrecedence", "2 != 3 ? 2 + 3 : 2 - 3" },

        #endregion

        { "chainComparison", "x < y <= z" },
        { "chainEquality", "x == y != z" },
        { "unsupportedUnary", "/2" },
        // { "unsupportedBinary", "2 _ 2" }, // this gets parsed as <int var int>
        // which is correct if we allow single expressions
        // which should be removed once I add the print() function
        { "unsupportedTernaryCondition", "x # y : z" },
        { "unsupportedTernaryBranch", "x ? y < z" },
        { "noBinaryArg", "+" },
        { "noBinaryArgs", "2 +" },
        { "noTernaryCondition", "? y : z" },
        { "noTernaryThen", "x ? : z" },
        { "noTernaryElse", "x ? y :" },
        { "noClosingBracket", "(2 + 3" },
        { "noOpeningBracket", "2 + 3)" },
    };

    public static readonly Dictionary<string, List<Expression?>> Outputs = new()
    {
        // { "empty", new List<Expression?>() },
        // { "chainComparison", new List<Expression?>() },
        // { "chainEquality", new List<Expression?>() },
        // { "unsupportedUnary", new List<Expression?>() },
        // { "unsupportedBinary", new List<Expression?>() },
        // { "unsupportedTernaryCondition", new List<Expression?>() },
        // { "unsupportedTernaryBranch", new List<Expression?>() },
        // { "noBinaryArg", new List<Expression?>() },
        // { "noBinaryArgs", new List<Expression?>() },
        // { "noTernaryCondition", new List<Expression?>() },
        // { "noTernaryThen", new List<Expression?>() },
        // { "noTernaryElse", new List<Expression?>() },
        // { "noClosingBracket", new List<Expression?>() },
        // { "noOpeningBracket", new List<Expression?>() },

        #region validOutputs

        {
            "empty",
            new List<Expression?>()
        },
        {
            "ternaryPrecedence",
            new List<Expression?>
            {
                new TernaryExpression(
                    new BinaryExpression(
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.NotEqual, "!=", null, 1),
                        new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 },
                    new BinaryExpression(
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Plus, "+", null, 1),
                        new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 },
                    new BinaryExpression(
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Minus, "-", null, 1),
                        new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },

        {
            "nestedTernary",
            new List<Expression?>
            {
                new TernaryExpression(
                    new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 },
                    new TernaryExpression(
                        new LiteralExpression(false) { Type = ExpressionType.Boolean, Line = 1 },
                        new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 },
                    new TernaryExpression(
                        new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 },
                        new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 },
                        new LiteralExpression(4) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },

        {
            "ternary",
            new List<Expression?>
            {
                new TernaryExpression(
                    new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 },
                    new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                    new LiteralExpression(0) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "equalityPrecedence",
            new List<Expression?>
            {
                new BinaryExpression(
                    new BinaryExpression(
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.LessThan, "<", null, 1),
                        new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 },
                    new Token(TokenKind.DoubleEqual, "==", null, 1),
                    new BinaryExpression(
                        new LiteralExpression("a") { Type = ExpressionType.String, Line = 1 },
                        new Token(TokenKind.LessThan, "<", null, 1),
                        new LiteralExpression("b") { Type = ExpressionType.String, Line = 1 }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "inequality",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.NotEqual, "!=", null, 1),
                    new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "equality",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.DoubleEqual, "==", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "relationalPrecedence",
            new List<Expression?>
            {
                new BinaryExpression(
                    new BinaryExpression(
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Plus, "+", null, 1),
                        new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 },
                    new Token(TokenKind.LessEqual, "<=", null, 1),
                    new LiteralExpression(5) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "lessEqual",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression("a") { Type = ExpressionType.String, Line = 1 },
                    new Token(TokenKind.LessEqual, "<=", null, 1),
                    new LiteralExpression("b") { Type = ExpressionType.String, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "greaterEqual",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.GreaterEqual, ">=", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "lessThan",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.LessThan, "<", null, 1),
                    new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "greaterThan",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.GreaterThan, ">", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "arithmeticPrecedence",
            new List<Expression?>
            {
                new BinaryExpression(
                    new UnaryExpression(
                        new Token(TokenKind.Minus, "-", null, 1),
                        new LiteralExpression(5) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new BinaryExpression(
                        new BinaryExpression(
                            new BinaryExpression(
                                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                                new Token(TokenKind.Plus, "+", null, 1),
                                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                            ) { Line = 1 },
                            new Token(TokenKind.Hat, "^", null, 1),
                            new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                        ) { Line = 1 },
                        new Token(TokenKind.Asterisk, "*", null, 1),
                        new LiteralExpression(4) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "chainConcatenation",
            new List<Expression?>
            {
                new BinaryExpression(
                    new BinaryExpression(
                        new LiteralExpression("a") { Type = ExpressionType.String, Line = 1 },
                        new Token(TokenKind.DoublePlus, "++", null, 1),
                        new LiteralExpression("b") { Type = ExpressionType.String, Line = 1 }
                    ) { Line = 1 },
                    new Token(TokenKind.DoublePlus, "++", null, 1),
                    new LiteralExpression("c") { Type = ExpressionType.String, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "concatenation",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression("a") { Type = ExpressionType.String, Line = 1 },
                    new Token(TokenKind.DoublePlus, "++", null, 1),
                    new LiteralExpression("b") { Type = ExpressionType.String, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "unarySubtraction",
            new List<Expression?>
            {
                new BinaryExpression(
                    new UnaryExpression(
                        new Token(TokenKind.Minus, "-", null, 1),
                        new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 },
                    new Token(TokenKind.Minus, "-", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "chainSubtraction",
            new List<Expression?>
            {
                new BinaryExpression(
                    new BinaryExpression(
                        new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Minus, "-", null, 1),
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 },
                    new Token(TokenKind.Minus, "-", null, 1),
                    new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "subtraction",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Minus, "-", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "chainAddition",
            new List<Expression?>
            {
                new BinaryExpression(
                    new BinaryExpression(
                        new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Plus, "+", null, 1),
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "addition",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "chainModulo",
            new List<Expression?>
            {
                new BinaryExpression(
                    new BinaryExpression(
                        new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Percent, "%", null, 1),
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 },
                    new Token(TokenKind.Percent, "%", null, 1),
                    new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "modulo",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Percent, "%", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "chainDivision",
            new List<Expression?>
            {
                new BinaryExpression(
                    new BinaryExpression(
                        new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Slash, "/", null, 1),
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 },
                    new Token(TokenKind.Slash, "/", null, 1),
                    new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "division",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Slash, "/", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "chainMultiplication",
            new List<Expression?>
            {
                new BinaryExpression(
                    new BinaryExpression(
                        new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Asterisk, "*", null, 1),
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 },
                    new Token(TokenKind.Asterisk, "*", null, 1),
                    new LiteralExpression(3) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "multiplication",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Asterisk, "*", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        /* ------------------ */
        {
            "intLiteral", new List<Expression?> { new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 } }
        },
        {
            "doubleLiteral",
            new List<Expression?> { new LiteralExpression(3.4d) { Type = ExpressionType.Double, Line = 1 } }
        },
        {
            "stringLiteral",
            new List<Expression?> { new LiteralExpression("hello birds") { Type = ExpressionType.String, Line = 1 } }
        },
        {
            "charLiteral", new List<Expression?> { new LiteralExpression('a') { Type = ExpressionType.Char, Line = 1 } }
        },
        {
            "true", new List<Expression?> { new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 } }
        },
        {
            "false", new List<Expression?> { new LiteralExpression(false) { Type = ExpressionType.Boolean, Line = 1 } }
        },
        {
            "grouping",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "raiseToPower", new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Hat, "^", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "raiseGroupingToPower",
            new List<Expression?>
            {
                new BinaryExpression(
                    new BinaryExpression(
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Plus, "+", null, 1),
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 },
                    new Token(TokenKind.Hat, "^", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "raiseToGrouping",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Hat, "^", null, 1),
                    new BinaryExpression(
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Plus, "+", null, 1),
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "chainPower",
            new List<Expression?>
            {
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Hat, "^", null, 1),
                    new BinaryExpression(
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Hat, "^", null, 1),
                        new BinaryExpression(
                            new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                            new Token(TokenKind.Hat, "^", null, 1),
                            new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                        ) { Line = 1 }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "unaryMinus",
            new List<Expression?>
            {
                new UnaryExpression(
                    new Token(TokenKind.Minus, "-", null, 1),
                    new LiteralExpression(123) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "unaryNot",
            new List<Expression?>
            {
                new UnaryExpression(
                    new Token(TokenKind.Not, "!", null, 1),
                    new LiteralExpression(false) { Type = ExpressionType.Boolean, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "unaryGrouping",
            new List<Expression?>
            {
                new UnaryExpression(
                    new Token(TokenKind.Minus, "-", null, 1),
                    new BinaryExpression(
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Plus, "+", null, 1),
                        new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        }

        #endregion
    };
}