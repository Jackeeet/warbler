﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tests.Mocks;
using Warbler.Expressions;
using Warbler.Interpreter;
using Warbler.Resolver;
using Warbler.Utils.Exceptions;
using Warbler.Utils.Id;
using Warbler.Utils.Token;
using Warbler.Utils.Type;

namespace Tests.InterpreterTests;

[TestFixture]
public class InterpreterShould
{
    private TestReporter _errorReporter = null!;

    private TestIdProvider _idProvider = null!;

    private WarblerResolver _resolver = null!;

    private WarblerInterpreter _interpreter = null!;

    private Stack<Dictionary<string, bool>> _scopes = null!;

    [OneTimeSetUp]
    public void BeforeFixture()
    {
        _errorReporter = new TestReporter();
        _idProvider = new TestIdProvider();
        _scopes = new Stack<Dictionary<string, bool>>();
    }

    [SetUp]
    public void BeforeTest()
    {
        _errorReporter.Reset();
        // _interpreter = new WarblerInterpreter(_errorReporter, new WarblerEnvironment());
        SetupGlobalScope();
        _resolver = new WarblerResolver(_errorReporter);
        SetupGlobalEnvironment();
    }

    private void SetupGlobalScope()
    {
        var globalScope = new Dictionary<string, bool>
        {
            ["intVar"] = true,
            ["reassignInt"] = true,
            ["reassignDouble"] = true,
            ["reassignBool"] = true,
            ["reassignChar"] = true,
            ["reassignString"] = true,
            ["reassignExpression"] = true,
            ["reassignVariable"] = true,
            ["i"] = true,
            ["j"] = true,
            ["undef"] = false,
        };
        _scopes.Push(globalScope);
    }

    private void SetupGlobalEnvironment()
    {
        DefineGlobals();

        _interpreter.GlobalEnvironment.Assign(
            new Token(TokenKind.Identifier, "intVar", null, 1), 10);
        _interpreter.GlobalEnvironment.Assign(
            new Token(TokenKind.Identifier, "reassignInt", null, 1), 0);
        _interpreter.GlobalEnvironment.Assign(
            new Token(TokenKind.Identifier, "reassignDouble", null, 1), 0.0d);
        _interpreter.GlobalEnvironment.Assign(
            new Token(TokenKind.Identifier, "reassignBool", null, 1), false);
        _interpreter.GlobalEnvironment.Assign(
            new Token(TokenKind.Identifier, "reassignChar", null, 1), '0');
        _interpreter.GlobalEnvironment.Assign(
            new Token(TokenKind.Identifier, "reassignString", null, 1), "");
        _interpreter.GlobalEnvironment.Assign(
            new Token(TokenKind.Identifier, "reassignExpression", null, 1), 0);
        _interpreter.GlobalEnvironment.Assign(
            new Token(TokenKind.Identifier, "reassignVariable", null, 1), 0);
        _interpreter.GlobalEnvironment.Assign(
            new Token(TokenKind.Identifier, "i", null, 1), 0);
        _interpreter.GlobalEnvironment.Assign(
            new Token(TokenKind.Identifier, "j", null, 1), 0);
    }

    // this is one of the dumbest methods i've ever written
    private void DefineGlobals()
    {
        _interpreter.GlobalEnvironment.Define("intVar", new WarblerType(ExpressionType.Integer));
        _interpreter.GlobalEnvironment.Define("i", new WarblerType(ExpressionType.Integer));
        _interpreter.GlobalEnvironment.Define("j", new WarblerType(ExpressionType.Integer));

        foreach (var name in Variable.DeclarationNames)
        {
            var type = name[0] == 'i'
                ? new WarblerType(ExpressionType.Integer)
                : name.Substring(0, name.IndexOf('D')) switch
                {
                    "double" => new WarblerType(ExpressionType.Double),
                    "bool" => new WarblerType(ExpressionType.Boolean),
                    "char" => new WarblerType(ExpressionType.Char),
                    "string" => new WarblerType(ExpressionType.String),
                    _ => new WarblerType(ExpressionType.Integer)
                };

            _interpreter.GlobalEnvironment.Define(name, type);
        }

        foreach (var name in Variable.AssignmentNames)
        {
            var type = name.Substring(8).ToLower() switch
            {
                "double" => new WarblerType(ExpressionType.Double),
                "bool" => new WarblerType(ExpressionType.Boolean),
                "char" => new WarblerType(ExpressionType.Char),
                "string" => new WarblerType(ExpressionType.String),
                _ => new WarblerType(ExpressionType.Integer)
            };

            _interpreter.GlobalEnvironment.Define(name, type);
        }
    }

    [Test]
    [TestCaseSource(typeof(Literal), nameof(Literal.ValidNames))]
    public void EvaluateValidLiteralExpressions(string inputName)
    {
        var expr = Literal.Inputs[inputName];
        _resolver.Resolve(new List<Expression> { expr });
        var expected = Literal.Outputs[inputName];

        var actual = _interpreter.Interpret(expr);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Unary), nameof(Unary.ValidNames))]
    public void EvaluateValidUnaryExpressions(string inputName)
    {
        var expr = Unary.Inputs[inputName];
        _resolver.Resolve(new List<Expression> { expr! });
        var expected = Unary.Outputs[inputName];

        var actual = _interpreter.Interpret(expr!);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Unary), nameof(Unary.InvalidNames))]
    public void ThrowOnInvalidUnaryExpressions(string inputName)
    {
        var expr = Unary.Inputs[inputName];
        _resolver.Resolve(new List<Expression> { expr! });
        Assert.Throws<ArgumentException>(() =>
            _interpreter.Interpret(expr!)
        );
    }

    [Test]
    public void ThrowOnUnknownUnaryOperator()
    {
        var expr = Unary.Inputs["unknownOperator"];
        _resolver.Resolve(new List<Expression> { expr! });

        Assert.Throws<UnreachableException>(() =>
            _interpreter.Interpret(expr!)
        );
    }

    [Test]
    [TestCaseSource(typeof(Binary), nameof(Binary.ValidNames))]
    [DefaultFloatingPointTolerance(0.000001)]
    public void EvaluateValidBinaryExpressions(string inputName)
    {
        var expr = Binary.Inputs[inputName];
        _resolver.Resolve(new List<Expression> { expr });
        var expected = Binary.Outputs[inputName];

        var actual = _interpreter.Interpret(Binary.Inputs[inputName]);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void HandleDivisionByZero()
    {
        // todo move to Binary.cs
        var expr = new BinaryExpression(
            new LiteralExpression(39) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
            new Token(TokenKind.Slash, "/", null, 1),
            new BinaryExpression(
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
            ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
        ) { Type = new WarblerType(ExpressionType.Integer), Line = 1 };

        _resolver.Resolve(new List<Expression> { expr });
        var value = _interpreter.Interpret(expr);

        Assert.IsTrue(_errorReporter.HadRuntimeError);
        Assert.IsNull(value);
    }


    [Test]
    [TestCaseSource(typeof(Binary), nameof(Binary.InvalidNames))]
    public void ThrowOnInvalidBinaryExpressions(string inputName)
    {
        var expr = Binary.Inputs[inputName];
        _resolver.Resolve(new List<Expression> { expr });
        Assert.Throws<ArgumentException>(() =>
            _interpreter.Interpret(Binary.Inputs[inputName]));
    }

    [Test]
    public void ThrowOnUnknownBinaryOperator()
    {
        // todo move to Binary.cs
        var expr = new BinaryExpression(
            new LiteralExpression("warb") { Type = new WarblerType(ExpressionType.String), Line = 1 },
            new Token(TokenKind.Question, "?", null, 1),
            new LiteralExpression("ler") { Type = new WarblerType(ExpressionType.String), Line = 1 }
        ) { Type = new WarblerType(ExpressionType.String), Line = 1 };

        _resolver.Resolve(new List<Expression> { expr });

        Assert.Throws<UnreachableException>(() =>
            _interpreter.Interpret(expr)
        );
    }

    [Test]
    [TestCaseSource(typeof(Ternary), nameof(Ternary.ValidNames))]
    public void EvaluateValidTernaryExpressions(string inputName)
    {
        var expr = Ternary.Inputs[inputName];
        _resolver.Resolve(new List<Expression> { expr });
        var expected = Ternary.Outputs[inputName];

        var actual = _interpreter.Interpret(Ternary.Inputs[inputName]);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Ternary), nameof(Ternary.InvalidNames))]
    public void ThrowOnInvalidTernaryExpressions(string inputName)
    {
        var expr = Ternary.Inputs[inputName];
        _resolver.Resolve(new List<Expression> { expr });
        Assert.Throws<ArgumentException>(() =>
            _interpreter.Interpret(Ternary.Inputs[inputName])
        );
    }

    [Test]
    [TestCaseSource(typeof(Variable), nameof(Variable.DeclarationNames))]
    public void EvaluateValidVariableDeclarationExpressions(string inputName)
    {
        // variable names get defined at type-checking stage so that 
        // it is possible to check whether the value being assigned
        // matches the declared type
        // thus before a vardecl expression is evaluated (and assigned), the name is already defined
        Assert.True(_interpreter.GlobalEnvironment.Defined(inputName));

        var expr = Variable.Inputs[inputName];
        _resolver.Resolve(new List<Expression> { expr });
        var expected = Variable.Outputs[inputName];

        var returnValue = _interpreter.Interpret(expr);
        Assert.True(_interpreter.GlobalEnvironment.Assigned(inputName));
        var (_, storedValue) = _interpreter.GlobalEnvironment.GetDefined(
            new Token(TokenKind.Identifier, inputName, null, 1));

        Assert.AreEqual(expected, storedValue);
        Assert.AreEqual(expected, returnValue);
    }

    [Test]
    [TestCaseSource(typeof(Variable), nameof(Variable.AssignmentNames))]
    public void EvaluateValidVariableAssignmentExpressions(string inputName)
    {
        Assert.True(_interpreter.GlobalEnvironment.Assigned(inputName));
        var variableToken = new Token(TokenKind.Identifier, inputName, null, 1);

        var initialValue = _interpreter.GlobalEnvironment.GetDefined(variableToken);
        var expected = Variable.Outputs[inputName];
        var expr = Variable.Inputs[inputName];
        _resolver.Resolve(new List<Expression> { expr });

        var returnValue = _interpreter.Interpret(expr);
        Assert.True(_interpreter.GlobalEnvironment.Assigned(inputName));
        var (_, storedValue) = _interpreter.GlobalEnvironment.GetDefined(variableToken);

        Assert.AreEqual(expected, storedValue);
        Assert.AreNotEqual(initialValue, storedValue);
        Assert.AreEqual(expected, returnValue);
    }

    [Test]
    [TestCaseSource(typeof(Variable), nameof(Variable.InvalidNames))]
    public void ThrowOnUndefinedVariable(string inputName)
    {
        var expr = Variable.Inputs[inputName];
        _resolver.Resolve(new List<Expression> { expr });
        Assert.Throws<ArgumentException>(() =>
            _interpreter.Interpret(Variable.Inputs[inputName])
        );
    }

    [Test]
    [TestCaseSource(typeof(Block), nameof(Block.ValidNames))]
    public void EvaluateValidBlockExpressions(string inputName)
    {
        var outerBlockId = Block.OuterBlockId;
        var innerBlockId = Block.InnerBlockId;

        _interpreter.GlobalEnvironment.AddSubEnvironment(outerBlockId);
        _interpreter.GlobalEnvironment
            .GetSubEnvironment(outerBlockId)
            .Define("block", new WarblerType(ExpressionType.Integer));
        _interpreter.GlobalEnvironment
            .GetSubEnvironment(outerBlockId)
            .Define("block2", new WarblerType(ExpressionType.Integer));

        _interpreter.GlobalEnvironment.GetSubEnvironment(outerBlockId).AddSubEnvironment(innerBlockId);
        _interpreter.GlobalEnvironment
            .GetSubEnvironment(outerBlockId)
            .GetSubEnvironment(innerBlockId)
            .Define("block", new WarblerType(ExpressionType.Integer));

        var expr = Block.Inputs[inputName];
        _resolver.Resolve(new List<Expression> { expr });
        var expected = Block.Outputs[inputName];

        var actual = _interpreter.Interpret(expr);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(WhileLoop), nameof(WhileLoop.ValidNames))]
    public void EvaluateValidWhileLoops(string inputName)
    {
        _interpreter.GlobalEnvironment.AddSubEnvironment(_idProvider.GetEnvironmentId());
        _interpreter.GlobalEnvironment.GetSubEnvironment(_idProvider.GetEnvironmentId())
            .AddSubEnvironment(new EnvId(new Guid("00000000-0000-0000-0000-000000000001")));

        var expr = WhileLoop.Inputs[inputName];
        _resolver.Resolve(new List<Expression> { expr });
        var expected = WhileLoop.Outputs[inputName];

        var actual = _interpreter.Interpret(expr);

        Assert.AreEqual(expected, actual);
    }
}