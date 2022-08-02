using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.InterpreterTests;

public static class VariableExpressionsData
{
    public static readonly List<string> DeclarationNames = new()
    {
        "intDeclaration",
        "doubleDeclaration",
        "boolDeclaration",
        "charDeclaration",
        "stringDeclaration",
        "implicitTypeDeclaration",
        "initializeWithExpression",
        "initializeWithVariable",
    };

    public static readonly List<string> AssignmentNames = new()
    {
        "reassignInt",
        "reassignDouble",
        "reassignBool",
        "reassignChar",
        "reassignString",
        "reassignExpression",
        "reassignVariable",
    };

    public static readonly List<string> InvalidNames = new()
    {
        "initUndefined", // undefined in the environment on type-checking stage
        "initWithUndefined",
        "assignUndefined",
        "assignToUndefined",
        "initWithNull",
        "assignNull"
    };

    public static readonly Dictionary<string, Expression> Inputs = new()
    {
        #region declaration

        {
            "intDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Int, "int", null, 1),
                new Token(TokenKind.Identifier, "intDeclaration", null, 1),
                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },

        {
            "doubleDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Double, "double", null, 1),
                new Token(TokenKind.Identifier, "doubleDeclaration", null, 1),
                new LiteralExpression(1.0d) { Type = ExpressionType.Double, Line = 1 }
            ) { Type = ExpressionType.Double, Line = 1 }
        },
        {
            "boolDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Bool, "bool", null, 1),
                new Token(TokenKind.Identifier, "boolDeclaration", null, 1),
                new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 }
            ) { Type = ExpressionType.Boolean, Line = 1 }
        },
        {
            "charDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Char, "char", null, 1),
                new Token(TokenKind.Identifier, "charDeclaration", null, 1),
                new LiteralExpression('a') { Type = ExpressionType.Char, Line = 1 }
            ) { Type = ExpressionType.Char, Line = 1 }
        },
        {
            "stringDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.String, "string", null, 1),
                new Token(TokenKind.Identifier, "stringDeclaration", null, 1),
                new LiteralExpression("abc") { Type = ExpressionType.String, Line = 1 }
            ) { Type = ExpressionType.String, Line = 1 }
        },
        {
            "implicitTypeDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Def, "def", null, 1),
                new Token(TokenKind.Identifier, "implicitTypeDeclaration", null, 1),
                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "initializeWithExpression",
            new VariableDeclarationExpression(
                new Token(TokenKind.Int, "int", null, 1),
                new Token(TokenKind.Identifier, "initializeWithExpression", null, 1),
                new BinaryExpression(
                    new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
                ) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "initializeWithVariable",
            new VariableDeclarationExpression(
                new Token(TokenKind.Int, "int", null, 1),
                new Token(TokenKind.Identifier, "initializeWithVariable", null, 1),
                new VariableExpression(
                    new Token(TokenKind.Identifier, "intVar", null, 1)
                ) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },

        #endregion

        #region assignment

        {
            "reassignInt",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignInt", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "reassignDouble",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignDouble", null, 1),
                new LiteralExpression(2.0d) { Type = ExpressionType.Double, Line = 1 }
            ) { Type = ExpressionType.Double, Line = 1 }
        },
        {
            "reassignBool",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignBool", null, 1),
                new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 }
            ) { Type = ExpressionType.Boolean, Line = 1 }
        },
        {
            "reassignChar",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignChar", null, 1),
                new LiteralExpression('2') { Type = ExpressionType.Char, Line = 1 }
            ) { Type = ExpressionType.Char, Line = 1 }
        },
        {
            "reassignString",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignString", null, 1),
                new LiteralExpression("2") { Type = ExpressionType.String, Line = 1 }
            ) { Type = ExpressionType.String, Line = 1 }
        },
        {
            "reassignExpression",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignExpression", null, 1),
                new BinaryExpression(
                    new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
                ) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "reassignVariable",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignVariable", null, 1),
                new VariableExpression(
                    new Token(TokenKind.Identifier, "intVar", null, 1)
                ) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },

        #endregion

        #region invalid

        {
            "initUndefined", new VariableDeclarationExpression(
                new Token(TokenKind.Int, "int", null, 1),
                new Token(TokenKind.Identifier, "undef", null, 1),
                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "initWithUndefined", new VariableDeclarationExpression(
                new Token(TokenKind.Int, "int", null, 1),
                new Token(TokenKind.Identifier, "intDeclaration", null, 1),
                new VariableExpression(
                    new Token(TokenKind.Identifier, "undef", null, 1)
                ) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "assignUndefined", new AssignmentExpression(
                new Token(TokenKind.Identifier, "intVar", null, 1),
                new VariableExpression(
                    new Token(TokenKind.Identifier, "undef", null, 1)
                ) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "assignToUndefined", new AssignmentExpression(
                new Token(TokenKind.Identifier, "undef", null, 1),
                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "initWithNull", new VariableDeclarationExpression(
                new Token(TokenKind.Int, "int", null, 1),
                new Token(TokenKind.Identifier, "intDeclaration", null, 1),
                new LiteralExpression(null!) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "assignNull", new AssignmentExpression(
                new Token(TokenKind.Identifier, "intVar", null, 1),
                new LiteralExpression(null!) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        }

        #endregion
    };

    public static readonly Dictionary<string, object?> Outputs = new()
    {
        { "intDeclaration", 1 },
        { "doubleDeclaration", 1.0d },
        { "boolDeclaration", true },
        { "charDeclaration", 'a' },
        { "stringDeclaration", "abc" },
        { "implicitTypeDeclaration", 1 },
        { "initializeWithExpression", 2 },
        // this is set in InterpreterShould -> PredefineVariables, which isn't the best place
        // but it works for now
        { "initializeWithVariable", 10 },

        { "reassignInt", 2 },
        { "reassignDouble", 2.0d },
        { "reassignBool", true },
        { "reassignChar", '2' },
        { "reassignString", "2" },
        { "reassignExpression", 2 },
        { "reassignVariable", 10 },
    };
}