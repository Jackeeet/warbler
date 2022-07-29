using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.CheckerTests;

public static class VariableExpressionsData
{
    public static readonly List<string> ValidNames = new()
    {
        "intDeclaration",
        "doubleDeclaration",
        "boolDeclaration",
        "charDeclaration",
        "stringDeclaration",
        "implicitTypeDeclaration",
        "initializeWithExpression",
        "initializeWithVariable",
        "assignInt",
        "assignDouble",
        "assignBoolean",
        "assignChar",
        "assignString",
        "assignExpression",
        "assignVariable"
    };

    public static readonly List<string> InvalidNames = new()
    {
        "initIntWithDouble",
        "assignBoolToString",
    };

    public static readonly Dictionary<string, Expression> Inputs = new()
    {
        #region varDecl

        {
            "intDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Int, "int", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "doubleDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Double, "double", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new LiteralExpression(1.0d) { Type = ExpressionType.Double, Line = 1 }
            ) { Line = 1 }
        },
        {
            "boolDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Bool, "bool", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 }
            ) { Line = 1 }
        },
        {
            "charDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Char, "char", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new LiteralExpression('a') { Type = ExpressionType.Char, Line = 1 }
            ) { Line = 1 }
        },
        {
            "stringDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.String, "string", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new LiteralExpression("abc") { Type = ExpressionType.String, Line = 1 }
            ) { Line = 1 }
        },
        {
            "implicitTypeDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Def, "def", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "initializeWithExpression",
            new VariableDeclarationExpression(
                new Token(TokenKind.Int, "int", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new BinaryExpression(
                    new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            ) { Line = 1 }
        },
        {
            "initializeWithVariable",
            new VariableDeclarationExpression(
                new Token(TokenKind.Int, "int", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new VariableExpression(
                    new Token(TokenKind.Identifier, "intVar", null, 1)
                ) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },

        # endregion

        #region varAssign

        {
            "assignInt",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignInt", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },
        {
            "assignDouble",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignDouble", null, 1),
                new LiteralExpression(2.0d) { Type = ExpressionType.Double, Line = 1 }
            ) { Line = 1 }
        },
        {
            "assignBoolean",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignBool", null, 1),
                new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 }
            ) { Line = 1 }
        },
        {
            "assignChar",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignChar", null, 1),
                new LiteralExpression('2') { Type = ExpressionType.Char, Line = 1 }
            ) { Line = 1 }
        },
        {
            "assignString",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignString", null, 1),
                new LiteralExpression("2") { Type = ExpressionType.String, Line = 1 }
            ) { Line = 1 }
        },
        {
            "assignExpression",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignExpression", null, 1),
                new BinaryExpression(
                    new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            ) { Line = 1 }
        },
        {
            "assignVariable",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignVariable", null, 1),
                new VariableExpression(
                    new Token(TokenKind.Identifier, "intVar", null, 1)
                ) { Type = ExpressionType.Integer, Line = 1 }
            ) { Line = 1 }
        },

        #endregion

        {
            "initIntWithDouble",
            new VariableDeclarationExpression(
                new Token(TokenKind.Int, "int", null, 1),
                new Token(TokenKind.Identifier, "sdhhfiuw", null, 1),
                new LiteralExpression(2.3d) { Type = ExpressionType.Double, Line = 1 }
            ) { Line = 1 }
        },
        {
            "assignBoolToString",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "stringOnly", null, 1),
                new LiteralExpression(false) { Type = ExpressionType.Boolean, Line = 1 }
            ) { Line = 1 }
        }
    };

    public static readonly Dictionary<string, Expression> Outputs = new()
    {
        #region varDecl

        {
            "intDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Int, "int", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },

        {
            "doubleDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Double, "double", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new LiteralExpression(1.0d) { Type = ExpressionType.Double, Line = 1 }
            ) { Type = ExpressionType.Double, Line = 1 }
        },
        {
            "boolDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Bool, "bool", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 }
            ) { Type = ExpressionType.Boolean, Line = 1 }
        },
        {
            "charDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Char, "char", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new LiteralExpression('a') { Type = ExpressionType.Char, Line = 1 }
            ) { Type = ExpressionType.Char, Line = 1 }
        },
        {
            "stringDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.String, "string", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new LiteralExpression("abc") { Type = ExpressionType.String, Line = 1 }
            ) { Type = ExpressionType.String, Line = 1 }
        },
        {
            "implicitTypeDeclaration",
            new VariableDeclarationExpression(
                new Token(TokenKind.Def, "def", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
                new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "initializeWithExpression",
            new VariableDeclarationExpression(
                new Token(TokenKind.Int, "int", null, 1),
                new Token(TokenKind.Identifier, "a", null, 1),
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
                new Token(TokenKind.Identifier, "a", null, 1),
                new VariableExpression(
                    new Token(TokenKind.Identifier, "intVar", null, 1)
                ) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },

        #endregion

        #region varAssign

        {
            "assignInt",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignInt", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },
        {
            "assignDouble",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignDouble", null, 1),
                new LiteralExpression(2.0d) { Type = ExpressionType.Double, Line = 1 }
            ) { Type = ExpressionType.Double, Line = 1 }
        },
        {
            "assignBoolean",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignBool", null, 1),
                new LiteralExpression(true) { Type = ExpressionType.Boolean, Line = 1 }
            ) { Type = ExpressionType.Boolean, Line = 1 }
        },
        {
            "assignChar",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignChar", null, 1),
                new LiteralExpression('2') { Type = ExpressionType.Char, Line = 1 }
            ) { Type = ExpressionType.Char, Line = 1 }
        },
        {
            "assignString",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignString", null, 1),
                new LiteralExpression("2") { Type = ExpressionType.String, Line = 1 }
            ) { Type = ExpressionType.String, Line = 1 }
        },
        {
            "assignExpression",
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
            "assignVariable",
            new AssignmentExpression(
                new Token(TokenKind.Identifier, "reassignVariable", null, 1),
                new VariableExpression(
                    new Token(TokenKind.Identifier, "intVar", null, 1)
                ) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        },

        #endregion
    };
}