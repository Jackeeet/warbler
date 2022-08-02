using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tests.Mocks;
using Warbler.Environment;
using Warbler.Expressions;
using Warbler.Interpreter;

namespace Tests.InterpreterTests;

[TestFixture]
public class InterpreterShould
{
    private TestReporter _errorReporter = null!;
    private WarblerEnvironment _environment = null!;
    private WarblerInterpreter _interpreter = null!;

    [OneTimeSetUp]
    public void BeforeFixture()
    {
        _errorReporter = new TestReporter();
    }

    [SetUp]
    public void BeforeTest()
    {
        _errorReporter.Reset();
        _environment = new WarblerEnvironment();
        PredefineVariables();
        _interpreter = new WarblerInterpreter(_errorReporter, _environment);
    }

    private void PredefineVariables()
    {
        Define();
        _environment.Assign(
            new Token(TokenKind.Identifier, "intVar", null, 1), 10);
        _environment.Assign(
            new Token(TokenKind.Identifier, "reassignInt", null, 1), 0);
        _environment.Assign(
            new Token(TokenKind.Identifier, "reassignDouble", null, 1), 0.0d);
        _environment.Assign(
            new Token(TokenKind.Identifier, "reassignBool", null, 1), false);
        _environment.Assign(
            new Token(TokenKind.Identifier, "reassignChar", null, 1), '0');
        _environment.Assign(
            new Token(TokenKind.Identifier, "reassignString", null, 1), "");
        _environment.Assign(
            new Token(TokenKind.Identifier, "reassignExpression", null, 1), 0);
        _environment.Assign(
            new Token(TokenKind.Identifier, "reassignVariable", null, 1), 0);
    }

    // this is one of the dumbest methods i've ever written
    private void Define()
    {
        _environment.Define("intVar", ExpressionType.Integer);

        foreach (var name in VariableExpressionsData.DeclarationNames)
        {
            var type = name[0] == 'i'
                ? ExpressionType.Integer
                : name.Substring(0, name.IndexOf('D')) switch
                {
                    "double" => ExpressionType.Double,
                    "bool" => ExpressionType.Boolean,
                    "char" => ExpressionType.Char,
                    "string" => ExpressionType.String,
                    _ => ExpressionType.Integer
                };

            _environment.Define(name, type);
        }

        foreach (var name in VariableExpressionsData.AssignmentNames)
        {
            var type = name.Substring(8).ToLower() switch
            {
                "double" => ExpressionType.Double,
                "bool" => ExpressionType.Boolean,
                "char" => ExpressionType.Char,
                "string" => ExpressionType.String,
                _ => ExpressionType.Integer
            };

            _environment.Define(name, type);
        }
    }

    [Test]
    [TestCaseSource(typeof(LiteralExpressionsData), nameof(LiteralExpressionsData.ValidNames))]
    public void EvaluateValidLiteralExpressions(string inputName)
    {
        var expected = LiteralExpressionsData.Outputs[inputName];

        var actual = _interpreter.Interpret(LiteralExpressionsData.Inputs[inputName]);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(UnaryExpressionsData), nameof(UnaryExpressionsData.ValidNames))]
    public void EvaluateValidUnaryExpressions(string inputName)
    {
        var expected = UnaryExpressionsData.Outputs[inputName];

        var actual = _interpreter.Interpret(UnaryExpressionsData.Inputs[inputName]);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(UnaryExpressionsData), nameof(UnaryExpressionsData.InvalidNames))]
    public void ThrowOnInvalidUnaryExpressions(string inputName)
    {
        Assert.Throws<ArgumentException>(() => _interpreter.Interpret(UnaryExpressionsData.Inputs[inputName]));
    }

    [Test]
    [TestCaseSource(typeof(BinaryExpressionsData), nameof(BinaryExpressionsData.ValidNames))]
    [DefaultFloatingPointTolerance(0.000001)]
    public void EvaluateValidBinaryExpressions(string inputName)
    {
        var expected = BinaryExpressionsData.Outputs[inputName];

        var actual = _interpreter.Interpret(BinaryExpressionsData.Inputs[inputName]);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void HandleDivisionByZero()
    {
        var divByZero = new BinaryExpression(
            new LiteralExpression(39) { Type = ExpressionType.Integer, Line = 1 },
            new Token(TokenKind.Slash, "/", null, 1),
            new BinaryExpression(
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                new Token(TokenKind.Minus, "-", null, 1),
                new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
            ) { Type = ExpressionType.Integer, Line = 1 }
        ) { Type = ExpressionType.Integer, Line = 1 };

        var value = _interpreter.Interpret(divByZero);

        Assert.IsTrue(_errorReporter.HadRuntimeError);
        Assert.IsNull(value);
    }

    [Test]
    [TestCaseSource(typeof(BinaryExpressionsData), nameof(BinaryExpressionsData.InvalidNames))]
    public void ThrowOnInvalidBinaryExpressions(string inputName)
    {
        Assert.Throws<ArgumentException>(() => _interpreter.Interpret(BinaryExpressionsData.Inputs[inputName]));
    }

    [Test]
    [TestCaseSource(typeof(TernaryExpressionsData), nameof(TernaryExpressionsData.ValidNames))]
    public void EvaluateValidTernaryExpressions(string inputName)
    {
        var expected = TernaryExpressionsData.Outputs[inputName];

        var actual = _interpreter.Interpret(TernaryExpressionsData.Inputs[inputName]);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(TernaryExpressionsData), nameof(TernaryExpressionsData.InvalidNames))]
    public void ThrowOnInvalidTernaryExpressions(string inputName)
    {
        Assert.Throws<ArgumentException>(() =>
            _interpreter.Interpret(TernaryExpressionsData.Inputs[inputName])
        );
    }

    [Test]
    [TestCaseSource(typeof(VariableExpressionsData), nameof(VariableExpressionsData.DeclarationNames))]
    public void EvaluateValidVariableDeclarationExpressions(string inputName)
    {
        // variable names get defined at type-checking stage so that 
        // it is possible to check whether the value being assigned
        // matches the declared type
        // thus before a vardecl expression is evaluated (and assigned), the name is already defined
        Assert.True(_environment.Defined(inputName));
        var expected = VariableExpressionsData.Outputs[inputName];

        var returnValue = _interpreter.Interpret(VariableExpressionsData.Inputs[inputName]);
        Assert.True(_environment.Assigned(inputName));
        var (_, storedValue) = _environment.Get(
            new Token(TokenKind.Identifier, inputName, null, 1));

        Assert.AreEqual(expected, storedValue);
        Assert.AreEqual(expected, returnValue);
    }

    [Test]
    [TestCaseSource(typeof(VariableExpressionsData), nameof(VariableExpressionsData.AssignmentNames))]
    public void EvaluateValidVariableAssignmentExpressions(string inputName)
    {
        Assert.True(_environment.Assigned(inputName));
        var variableToken = new Token(TokenKind.Identifier, inputName, null, 1);

        var initialValue = _environment.Get(variableToken);
        var expected = VariableExpressionsData.Outputs[inputName];

        var returnValue = _interpreter.Interpret(VariableExpressionsData.Inputs[inputName]);
        Assert.True(_environment.Assigned(inputName));
        var (_, storedValue) = _environment.Get(variableToken);

        Assert.AreEqual(expected, storedValue);
        Assert.AreNotEqual(initialValue, storedValue);
        Assert.AreEqual(expected, returnValue);
    }

    [Test]
    [TestCaseSource(typeof(VariableExpressionsData), nameof(VariableExpressionsData.InvalidNames))]
    public void ThrowOnUndefinedVariable(string inputName)
    {
        Assert.Throws<ArgumentException>(() =>
            _interpreter.Interpret(VariableExpressionsData.Inputs[inputName])
        );
    }

    [Test]
    [TestCaseSource(typeof(BlockExpressionsData), nameof(BlockExpressionsData.ValidNames))]
    public void EvaluateValidBlockExpressions(string inputName)
    {
        var expected = BlockExpressionsData.Outputs[inputName];

        var actual = _interpreter.Interpret(BlockExpressionsData.Inputs[inputName]);

        Assert.AreEqual(expected, actual);
    }
}