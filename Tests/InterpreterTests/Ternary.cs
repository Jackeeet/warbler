using System.Collections.Generic;
using Warbler.Expressions;
using Warbler.Utils.Token;
using Warbler.Utils.Type;

namespace Tests.InterpreterTests;

public static class Ternary
{
    public static readonly List<string> ValidNames = new()
    {
        "equalityCondition",
        "inequalityCondition",
        "comparisonCondition",
        "basicBooleanCondition",
        "binaryUnaryBranches",
        "unevaluatedNullBranch"
    };

    public static readonly List<string> InvalidNames = new()
    {
        "nonBooleanCondition",
        "nullThenBranch",
        "nullElseBranch"
    };

    public static readonly Dictionary<string, Expression> Inputs = new()
    {
        #region valid

        {
            "equalityCondition", new TernaryExpression(
                new BinaryExpression(
                    new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new Token(TokenKind.DoubleEqual, "==", null, 1),
                    new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new LiteralExpression(3) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "inequalityCondition", new TernaryExpression(
                new BinaryExpression(
                    new LiteralExpression(0) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new Token(TokenKind.NotEqual, "!=", null, 1),
                    new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new LiteralExpression(3) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "comparisonCondition", new TernaryExpression(
                new BinaryExpression(
                    new LiteralExpression('a') { Type = new WarblerType(ExpressionType.Char), Line = 1 },
                    new Token(TokenKind.LessThan, "<", null, 1),
                    new LiteralExpression('b') { Type = new WarblerType(ExpressionType.Char), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new LiteralExpression("true") { Type = new WarblerType(ExpressionType.String), Line = 1 },
                new LiteralExpression("false") { Type = new WarblerType(ExpressionType.String), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.String), Line = 1 }
        },
        {
            "basicBooleanCondition", new TernaryExpression(
                new LiteralExpression(false) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new LiteralExpression(2.3d) { Type = new WarblerType(ExpressionType.Double), Line = 1 },
                new LiteralExpression(3.4d) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Double), Line = 1 }
        },
        {
            "binaryUnaryBranches", new TernaryExpression(
                new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new BinaryExpression(
                    new LiteralExpression(0) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new UnaryExpression(
                    new Token(TokenKind.Minus, "-", null, 1),
                    new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "unevaluatedNullBranch", new TernaryExpression(
                new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new LiteralExpression(null!) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },

        #endregion

        {
            "nonBooleanCondition", new TernaryExpression(
                new LiteralExpression("hehe") { Type = new WarblerType(ExpressionType.String), Line = 1 },
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "nullThenBranch", new TernaryExpression(
                new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new LiteralExpression(null!) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
        {
            "nullElseBranch", new TernaryExpression(
                new LiteralExpression(false) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new LiteralExpression(null!) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        },
    };

    public static readonly Dictionary<string, object?> Outputs = new()
    {
        { "equalityCondition", 2 },
        { "inequalityCondition", 2 },
        { "comparisonCondition", "true" },
        { "basicBooleanCondition", 3.4d },
        { "binaryUnaryBranches", 1 },
        { "unevaluatedNullBranch", 1 },
        { "nonBooleanCondition", new object() },
        { "nullThenBranch", new object() },
        { "nullElseBranch", new object() },
    };
}