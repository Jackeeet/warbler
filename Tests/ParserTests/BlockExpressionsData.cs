﻿using System;
using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.ParserTests;

public static class BlockExpressionsData
{
    public static readonly List<string> ValidNames = new()
    {
        "simpleBlock",
        "multiExpressionBlock",
        "twoBlocks",
        "blockBetweenExpressions",
    };

    public static readonly List<string> InvalidNames = new()
    {
        "assignBlock",
    };

    public static readonly Dictionary<string, string> Inputs = new()
    {
        { "simpleBlock", ":> int a = 10 <:" },
        { "multiExpressionBlock", ":> int a = 10 a = 15 <:" },
        { "twoBlocks", ":> int a = 10 <: \n :> int x = 10 <:" },
        { "blockBetweenExpressions", "int a = 20 \n :> int a = 10 <: \n a = 15" },
        { "assignBlock", "int a = :> int x = 15 <:" }
    };

    public static readonly Dictionary<string, List<Expression?>> Outputs = new()
    {
        #region valid

        {
            "simpleBlock", new List<Expression?>
            {
                new BlockExpression(new Guid(),
                    new List<Expression?>
                    {
                        new VariableDeclarationExpression(
                            new Token(TokenKind.Int, "int", null, 1),
                            new Token(TokenKind.Identifier, "a", null, 1),
                            new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 1 }
                        ) { Line = 1 }
                    }
                ) { Line = 1 }
            }
        },
        {
            "multiExpressionBlock", new List<Expression?>
            {
                new BlockExpression(new Guid(),
                    new List<Expression?>
                    {
                        new VariableDeclarationExpression(
                            new Token(TokenKind.Int, "int", null, 1),
                            new Token(TokenKind.Identifier, "a", null, 1),
                            new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 1 }
                        ) { Line = 1 },
                        new AssignmentExpression(
                            new Token(TokenKind.Identifier, "a", null, 1),
                            new LiteralExpression(15) { Type = ExpressionType.Integer, Line = 1 }
                        ) { Line = 1 }
                    }
                ) { Line = 1 }
            }
        },
        {
            "twoBlocks", new List<Expression?>
            {
                new BlockExpression(new Guid(),
                    new List<Expression?>
                    {
                        new VariableDeclarationExpression(
                            new Token(TokenKind.Int, "int", null, 1),
                            new Token(TokenKind.Identifier, "a", null, 1),
                            new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 1 }
                        ) { Line = 1 }
                    }
                ) { Line = 1 },
                new BlockExpression(new Guid(),
                    new List<Expression?>
                    {
                        new VariableDeclarationExpression(
                            new Token(TokenKind.Int, "int", null, 2),
                            new Token(TokenKind.Identifier, "x", null, 2),
                            new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 2 }
                        ) { Line = 2 }
                    }
                ) { Line = 2 }
            }
        },
        {
            "blockBetweenExpressions", new List<Expression?>
            {
                new VariableDeclarationExpression(
                    new Token(TokenKind.Int, "int", null, 1),
                    new Token(TokenKind.Identifier, "a", null, 1),
                    new LiteralExpression(20) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 },
                new BlockExpression(new Guid(),
                    new List<Expression?>
                    {
                        new VariableDeclarationExpression(
                            new Token(TokenKind.Int, "int", null, 2),
                            new Token(TokenKind.Identifier, "a", null, 2),
                            new LiteralExpression(10) { Type = ExpressionType.Integer, Line = 2 }
                        ) { Line = 2 }
                    }
                ) { Line = 2 },
                new AssignmentExpression(
                    new Token(TokenKind.Identifier, "a", null, 3),
                    new LiteralExpression(15) { Type = ExpressionType.Integer, Line = 3 }
                ) { Line = 3 }
            }
        },
        {
            "assignBlock", new List<Expression?>()
        },

        #endregion
    };
}