using System.Collections.Generic;
using Tests.Mocks;
using Warbler.Expressions;
using Expression = Warbler.Expressions.Expression;
using ExpressionType = Warbler.Expressions.ExpressionType;

namespace Tests.ParserTests;

public static class Conditional
{
    public static readonly List<string> ValidNames = new()
    {
        "if",
        "ifElse",
        "ifElseIf",
        "ifElseIfElse",
        "nestedIf",
        "nestedElse",
        // todo add oneLine
    };

    public static readonly List<string> InvalidNames = new()
    {
    };

    public static readonly Dictionary<string, string> Inputs = new()
    {
        #region valid

        { "if", "if true then :> int a = 10 <:" },
        { "ifElse", "if false then :> int a = 10 <: else :> int a = 12 <:" },
        {
            "ifElseIf", "if 1 > 2 then :> \n" +
                        "   int a = 1 <:\n " +
                        "else if 1 == 2 then :> \n" +
                        "   int a = 2" +
                        "<: \n"
        },
        {
            "ifElseIfElse", "if 1 > 2 then :> \n" +
                            "   int a = 1 <:\n " +
                            "else if 1 == 2 then :> \n" +
                            "   int a = 2 <: \n" +
                            "else :> int a = 0 <:"
        },
        {
            "nestedIf", "if 1 < 2 then :>\n" +
                        "   int a = 1\n" +
                        "   if a != 2 then :>\n" +
                        "       a = 2 " +
                        "   <: " +
                        "<:"
        },
        {
            "nestedElse", "if false then :> <:\n" +
                          "else :>\n" +
                          "    int a = 1\n" +
                          "    if a == 1 then :>\n" +
                          "        a = 2 " +
                          "    <: " +
                          "<:"
        },

        #endregion
    };

    public static readonly Dictionary<string, List<Expression?>> Outputs = new()
    {
        {
            "if", new List<Expression?>
            {
                new ConditionalExpression(
                    new LiteralExpression(true) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                    new BlockExpression(new TestGuidProvider().Get(),
                        new List<Expression?>
                        {
                            new VariableDeclarationExpression(
                                new Token(TokenKind.Int, "int", null, 1),
                                new Token(TokenKind.Identifier, "a", null, 1),
                                new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                            ) { Line = 1 }
                        }
                    ) { Line = 1 },
                    null
                ) { Line = 1 },
            }
        },
        {
            "ifElse", new List<Expression?>
            {
                new ConditionalExpression(
                    new LiteralExpression(false) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                    new BlockExpression(new TestGuidProvider().Get(),
                        new List<Expression?>
                        {
                            new VariableDeclarationExpression(
                                new Token(TokenKind.Int, "int", null, 1),
                                new Token(TokenKind.Identifier, "a", null, 1),
                                new LiteralExpression(10) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                            ) { Line = 1 }
                        }
                    ) { Line = 1 },
                    new BlockExpression(new TestGuidProvider().Get(),
                        new List<Expression?>
                        {
                            new VariableDeclarationExpression(
                                new Token(TokenKind.Int, "int", null, 1),
                                new Token(TokenKind.Identifier, "a", null, 1),
                                new LiteralExpression(12) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                            ) { Line = 1 }
                        }
                    ) { Line = 1 }
                ) { Line = 1 },
            }
        },
        {
            "ifElseIf", new List<Expression?>
            {
                new ConditionalExpression(
                    new BinaryExpression(
                        new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                        new Token(TokenKind.GreaterThan, ">", null, 1),
                        new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                    ) { Line = 1 },
                    new BlockExpression(new TestGuidProvider().Get(),
                        new List<Expression?>
                        {
                            new VariableDeclarationExpression(
                                new Token(TokenKind.Int, "int", null, 2),
                                new Token(TokenKind.Identifier, "a", null, 2),
                                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 2 }
                            ) { Line = 2 }
                        }
                    ) { Line = 1 },
                    new ConditionalExpression(
                        new BinaryExpression(
                            new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 3 },
                            new Token(TokenKind.DoubleEqual, "==", null, 3),
                            new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 3 }
                        ) { Line = 3 },
                        new BlockExpression(new TestGuidProvider().Get(),
                            new List<Expression?>
                            {
                                new VariableDeclarationExpression(
                                    new Token(TokenKind.Int, "int", null, 4),
                                    new Token(TokenKind.Identifier, "a", null, 4),
                                    new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 4 }
                                ) { Line = 4 }
                            }
                        ) { Line = 3 },
                        null
                    ) { Line = 3 }
                ) { Line = 1 },
            }
        },
        {
            "ifElseIfElse", new List<Expression?>
            {
                new ConditionalExpression(
                    // if 1 > 2 then
                    new BinaryExpression(
                        new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                        new Token(TokenKind.GreaterThan, ">", null, 1),
                        new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                    ) { Line = 1 },
                    // int a = 1
                    new BlockExpression(new TestGuidProvider().Get(),
                        new List<Expression?>
                        {
                            new VariableDeclarationExpression(
                                new Token(TokenKind.Int, "int", null, 2),
                                new Token(TokenKind.Identifier, "a", null, 2),
                                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 2 }
                            ) { Line = 2 }
                        }
                    ) { Line = 1 },
                    // else if 1 == 2 then
                    new ConditionalExpression(
                        new BinaryExpression(
                            new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 3 },
                            new Token(TokenKind.DoubleEqual, "==", null, 3),
                            new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 3 }
                        ) { Line = 3 },
                        // int a = 2
                        new BlockExpression(new TestGuidProvider().Get(),
                            new List<Expression?>
                            {
                                new VariableDeclarationExpression(
                                    new Token(TokenKind.Int, "int", null, 4),
                                    new Token(TokenKind.Identifier, "a", null, 4),
                                    new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 4 }
                                ) { Line = 4 }
                            }
                        ) { Line = 3 },
                        // else int a = 0
                        new BlockExpression(new TestGuidProvider().Get(),
                            new List<Expression?>
                            {
                                new VariableDeclarationExpression(
                                    new Token(TokenKind.Int, "int", null, 5),
                                    new Token(TokenKind.Identifier, "a", null, 5),
                                    new LiteralExpression(0) { Type = new WarblerType(ExpressionType.Integer), Line = 5 }
                                ) { Line = 5 }
                            }
                        ) { Line = 5 }
                    ) { Line = 3 }
                ) { Line = 1 },
            }
        },
        {
            "nestedIf", new List<Expression?>
            {
                new ConditionalExpression(
                    new BinaryExpression(
                        new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                        new Token(TokenKind.LessThan, "<", null, 1),
                        new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                    ) { Line = 1 },
                    new BlockExpression(new TestGuidProvider().Get(),
                        new List<Expression?>
                        {
                            new VariableDeclarationExpression(
                                new Token(TokenKind.Int, "int", null, 2),
                                new Token(TokenKind.Identifier, "a", null, 2),
                                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 2 }
                            ) { Line = 2 },
                            new ConditionalExpression(
                                new BinaryExpression(
                                    new VariableExpression(
                                        new Token(TokenKind.Identifier, "a", null, 3)
                                    ) { Line = 3 },
                                    new Token(TokenKind.NotEqual, "!=", null, 3),
                                    new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 3 }
                                ) { Line = 3 },
                                new BlockExpression(new TestGuidProvider().Get(),
                                    new List<Expression?>
                                    {
                                        new AssignmentExpression(
                                            new Token(TokenKind.Identifier, "a", null, 4),
                                            new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 4 }
                                        ) { Line = 4 }
                                    }
                                ) { Line = 3 },
                                null
                            ) { Line = 3 }
                        }
                    ) { Line = 1 },
                    null
                ) { Line = 1 },
            }
        },

        {
            "nestedElse", new List<Expression?>
            {
                new ConditionalExpression(
                    new LiteralExpression(false) { Type = new WarblerType(ExpressionType.Boolean), Line = 1 },
                    new BlockExpression(new TestGuidProvider().Get(), 
                        new List<Expression?>()) {Line = 1},
                    new BlockExpression(new TestGuidProvider().Get(),
                        new List<Expression?>
                        {
                            new VariableDeclarationExpression(
                                new Token(TokenKind.Int, "int", null, 3),
                                new Token(TokenKind.Identifier, "a", null, 3),
                                new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 3 }
                            ) { Line = 3},
                            new ConditionalExpression(
                                new BinaryExpression(
                                    new VariableExpression(
                                        new Token(TokenKind.Identifier, "a", null, 4)
                                    ) { Line = 4 },
                                    new Token(TokenKind.DoubleEqual, "==", null, 4),
                                    new LiteralExpression(1) { Type = new WarblerType(ExpressionType.Integer), Line = 4 }
                                ) { Line = 4 },
                                new BlockExpression(new TestGuidProvider().Get(),
                                    new List<Expression?>
                                    {
                                        new AssignmentExpression(
                                            new Token(TokenKind.Identifier, "a", null, 5),
                                            new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 5 }
                                        ) { Line = 5 }
                                    }
                                ) { Line = 4 },
                                null
                            ) { Line = 4 }
                        }
                    ) { Line = 2 }
                ) { Line = 1 },
            }
        },
    };
}