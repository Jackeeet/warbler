using System;
using NUnit.Framework;
using Tests.Mocks;
using Warbler.Environment;
using Warbler.Expressions;
using Warbler.TypeChecker;

namespace Tests.CheckerTests;

[TestFixture]
public class CheckerShould
{
    private TestReporter _errorReporter = null!;
    private WarblerEnvironment _environment = null!;
    private WarblerChecker? _checker;

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
        _checker = new WarblerChecker(_errorReporter, _environment);
    }

    private void PredefineVariables()
    {
        _environment.Define("intVar", ExpressionType.Integer);
        _environment.Define("reassignInt", ExpressionType.Integer);
        _environment.Define("reassignDouble", ExpressionType.Double);
        _environment.Define("reassignBool", ExpressionType.Boolean);
        _environment.Define("reassignChar", ExpressionType.Char);
        _environment.Define("reassignString", ExpressionType.String);
        _environment.Define("reassignExpression", ExpressionType.Integer);
        _environment.Define("reassignVariable", ExpressionType.Integer);
        _environment.Define("stringOnly", ExpressionType.Integer);
    }

    [Test]
    [TestCaseSource(typeof(UnaryExpressionsData), nameof(UnaryExpressionsData.ValidNames))]
    public void CheckValidUnaryExpressions(string inputName)
    {
        var expectedExpression = UnaryExpressionsData.Outputs[inputName];

        var actualExpression = UnaryExpressionsData.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }

    [Test]
    [TestCaseSource(typeof(BinaryExpressionsData), nameof(BinaryExpressionsData.ValidNames))]
    public void CheckValidBinaryExpressions(string inputName)
    {
        var expectedExpression = BinaryExpressionsData.Outputs[inputName];

        var actualExpression = BinaryExpressionsData.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }


    [Test]
    [TestCaseSource(typeof(TernaryExpressionsData), nameof(TernaryExpressionsData.ValidNames))]
    public void CheckValidTernaryExpressions(string inputName)
    {
        var expectedExpression = TernaryExpressionsData.Outputs[inputName];

        var actualExpression = TernaryExpressionsData.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }

    [Test]
    [TestCaseSource(typeof(VariableExpressionsData), nameof(VariableExpressionsData.ValidNames))]
    public void CheckValidVariableExpressions(string inputName)
    {
        var expectedExpression = VariableExpressionsData.Outputs[inputName];

        var actualExpression = VariableExpressionsData.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }

    [Test]
    [TestCaseSource(typeof(UnaryExpressionsData), nameof(UnaryExpressionsData.InvalidNames))]
    public void CheckInvalidUnaryExpressions(string inputName)
    {
        var expression = UnaryExpressionsData.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(expression);

        Assert.IsFalse(checkResult);
        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    [TestCaseSource(typeof(BinaryExpressionsData), nameof(BinaryExpressionsData.InvalidNames))]
    public void CheckInvalidBinaryExpressions(string inputName)
    {
        var expression = BinaryExpressionsData.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(expression);

        Assert.IsFalse(checkResult);
        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    [TestCaseSource(typeof(TernaryExpressionsData), nameof(TernaryExpressionsData.InvalidNames))]
    public void CheckInvalidTernaryExpressions(string inputName)
    {
        var expression = TernaryExpressionsData.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(expression);

        Assert.IsFalse(checkResult);
        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    [TestCaseSource(typeof(VariableExpressionsData), nameof(VariableExpressionsData.InvalidNames))]
    public void CheckInvalidVariableExpressions(string inputName)
    {
        var expression = VariableExpressionsData.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(expression);

        Assert.IsFalse(checkResult);
        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    public void ThrowOnUntypedLiteralExpression()
    {
        Assert.Throws<ArgumentException>(() => _checker!.CheckTypes(new LiteralExpression(2) { Line = 1 }));
    }

    [Test]
    public void ThrowOnUnexpectedUnaryOp()
    {
        Assert.Throws<ArgumentException>(() => _checker!.CheckTypes(
                new UnaryExpression(
                    new Token(TokenKind.Plus, "+", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            )
        );
    }

    [Test]
    public void ThrowOnUnexpectedBinaryOp()
    {
        Assert.Throws<ArgumentException>(() => _checker!.CheckTypes(
                new BinaryExpression(
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 },
                    new Token(TokenKind.Question, "?", null, 1),
                    new LiteralExpression(2) { Type = ExpressionType.Integer, Line = 1 }
                ) { Line = 1 }
            )
        );
    }
}