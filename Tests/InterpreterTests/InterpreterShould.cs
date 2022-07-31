using System;
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
        _interpreter = new WarblerInterpreter(_errorReporter, new WarblerEnvironment());
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
    [TestCaseSource(typeof(VariableExpressionsData), nameof(VariableExpressionsData.ValidNames))]
    public void EvaluateValidVariableExpressions(string inputName)
    {
        var expected = VariableExpressionsData.Outputs[inputName];

        var actual = _interpreter.Interpret(VariableExpressionsData.Inputs[inputName]);

        Assert.AreEqual(expected, actual);
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