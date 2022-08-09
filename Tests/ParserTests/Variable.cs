using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.ParserTests;

public static class Variable
{
    public static readonly List<string> ValidNames = new()
    {
        // declaration
        "declareInt",
        "declareDouble",
        "declareBool",
        "declareChar",
        "declareString",
        "declareImplicit",
        "initializeWithExpression",
        "initializeWithVariable",

        // assignment
        "assignInt",
        "assignDouble",
        "assignBool",
        "assignChar",
        "assignString",
        "assignExpression",
        "assignVariable",
        "declarationThenAssignment",
    };

    public static readonly List<string> InvalidNames = new()
    {
        "uninitializedVariable",
        "noEquals",
        "chainDeclaration",
        "commaDeclaration",
        "chainAssignment",
        "commaAssignment",
        "assignToExpression"
    };

    public static readonly Dictionary<string, string> Inputs = new()
    {
        // declaration
        { "declareInt", "int a = 10" },
        { "declareDouble", "double a = 20.3" },
        { "declareBool", "bool a = false" },
        { "declareChar", "char a = 'a'" },
        { "declareString", "string a = \"abc\"" },
        { "declareImplicit", "def a = 10" },
        { "initializeWithExpression", "int a = 1 + 1" },
        { "initializeWithVariable", "int a = intVar" },

        // assignment
        { "assignInt", "a = 10" },
        { "assignDouble", "a = 20.3" },
        { "assignBool", "a = false" },
        { "assignChar", "a = 'a'" },
        { "assignString", "a = \"abc\"" },
        { "assignExpression", "a = 1 + 1" },
        { "assignVariable", "a = intVar" },
        { "declarationThenAssignment", "int b = 30 int c = 2 \n b = b + c" },

        // invalid
        { "uninitializedVariable", "int x" },
        { "noEquals", "int x 20" },
        { "chainDeclaration", "int x = int y = 20" },
        { "commaDeclaration", "int x, y = 20" },
        { "chainAssignment", "x = y = 20" },
        { "commaAssignment", "x, y = 20" },
        { "assignToExpression", "(2 + 3) = 10"}
    };

    public static readonly Dictionary<string, List<Expression?>> Outputs = new()
    {
        #region invalid

        { "uninitializedVariable", new List<Expression?>() },
        { "noEquals", new List<Expression?>() },
        { "chainDeclaration", new List<Expression?>() },
        { "commaDeclaration", new List<Expression?>() },
        { "chainAssignment", new List<Expression?>() },
        { "commaAssignment", new List<Expression?>() },

        #endregion

        #region varDecl

        {
            "declareInt", new List<Expression?>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Int, "int", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "declareDouble", new List<Expression?>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Double, "double", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(20.3d) { Type = ExpressionType.Double, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "declareBool", new List<Expression?>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Bool, "bool", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(false) { Type = ExpressionType.Boolean, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "declareChar", new List<Expression?>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Char, "char", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression('a') { Type = ExpressionType.Char, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "declareString", new List<Expression?>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.String, "string", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression("abc") { Type = ExpressionType.String, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "declareImplicit", new List<Expression?>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Def, "def", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "initializeWithExpression", new List<Expression?>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Int, "int", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new BinaryExpression(
                        new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Plus, "+", null, 1),
                        new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "initializeWithVariable", new List<Expression?>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Int, "int", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "intVar", null, 1)
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },

        #endregion

        #region assignment

        {
            "assignInt", new List<Expression?>
            {
                new AssignmentExpression(
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "assignDouble", new List<Expression?>
            {
                new AssignmentExpression(
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(20.3d) { Type = ExpressionType.Double, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "assignBool", new List<Expression?>
            {
                new AssignmentExpression(
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(false) { Type = ExpressionType.Boolean, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "assignChar", new List<Expression?>
            {
                new AssignmentExpression(
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression('a') { Type = ExpressionType.Char, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "assignString", new List<Expression?>
            {
                new AssignmentExpression(
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression("abc") { Type = ExpressionType.String, Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "assignExpression", new List<Expression?>
            {
                new AssignmentExpression(
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new BinaryExpression(
                        new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 },
                        new Token(TokenKind.Plus, "+", null, 1),
                        new LiteralExpression(1) { Type = ExpressionType.Integer, Line = 1 }
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            "assignVariable", new List<Expression?>
            {
                new AssignmentExpression(
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new VariableExpression(
                        new Token(TokenKind.Identifier, "intVar", null, 1)
                    ) { Line = 1 }
                ) { Line = 1 }
            }
        },
        {
            // "int b = 30 int c = 2 \n b = b + c" 
            "declarationThenAssignment", new List<Expression?>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Int, "int", null, 1),
                    new Token(TokenKind.Identifier, "b", null, 1),
                    new LiteralExpression(30) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 },
                new VariableDeclarationExpression(
                    new Token(TokenKind.Int, "int", null, 1),
                    new Token(TokenKind.Identifier, "c", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 },
                new AssignmentExpression(
                    new Token(TokenKind.Identifier, "b", null, 2),
                    new BinaryExpression(
                        new VariableExpression(
                            new Token(TokenKind.Identifier, "b", null, 2)
                        ) { Line = 2 },
                        new Token(TokenKind.Plus, "+", null, 2),
                        new VariableExpression(
                            new Token(TokenKind.Identifier, "c", null, 2)
                        ) { Line = 2 }
                    ) { Line = 2 }
                ) { Line = 2 }
            }
        },

        #endregion
    };
}