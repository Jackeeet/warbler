using System;
using NUnit.Framework;
using Tests.Mocks;
using Warbler.Environment;
using Warbler.Expressions;
using Warbler.TypeChecker;
using Warbler.Utils.Exceptions;
using Warbler.Utils.Token;
using Warbler.Utils.Type;

namespace Tests.CheckerTests;

[TestFixture]
public class CheckerShould
{
    private TestReporter _errorReporter = null!;
    private TestIdProvider _idProvider = null!;
    private WarblerEnvironment _environment = null!;
    private WarblerChecker? _checker;

    [OneTimeSetUp]
    public void BeforeFixture()
    {
        _errorReporter = new TestReporter();
        _idProvider = new TestIdProvider();
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
        _environment.Define("intVar", new WarblerType(ExpressionType.Integer));
        _environment.Define("reassignInt", new WarblerType(ExpressionType.Integer));
        _environment.Define("reassignDouble", new WarblerType(ExpressionType.Double));
        _environment.Define("reassignBool", new WarblerType(ExpressionType.Boolean));
        _environment.Define("reassignChar", new WarblerType(ExpressionType.Char));
        _environment.Define("reassignString", new WarblerType(ExpressionType.String));
        _environment.Define("reassignExpression", new WarblerType(ExpressionType.Integer));
        _environment.Define("reassignVariable", new WarblerType(ExpressionType.Integer));
        _environment.Define("stringOnly", new WarblerType(ExpressionType.Integer));
    }

    [Test]
    [TestCaseSource(typeof(Unary), nameof(Unary.ValidNames))]
    public void CheckValidUnaryExpressions(string inputName)
    {
        var expectedExpression = Unary.Outputs[inputName];

        var actualExpression = Unary.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }

    [Test]
    [TestCaseSource(typeof(Binary), nameof(Binary.ValidNames))]
    public void CheckValidBinaryExpressions(string inputName)
    {
        var expectedExpression = Binary.Outputs[inputName];

        var actualExpression = Binary.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }


    [Test]
    [TestCaseSource(typeof(Ternary), nameof(Ternary.ValidNames))]
    public void CheckValidTernaryExpressions(string inputName)
    {
        var expectedExpression = Ternary.Outputs[inputName];

        var actualExpression = Ternary.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }

    [Test]
    [TestCaseSource(typeof(Variable), nameof(Variable.ValidNames))]
    public void CheckValidVariableExpressions(string inputName)
    {
        var expectedExpression = Variable.Outputs[inputName];

        var actualExpression = Variable.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(actualExpression);

        Assert.IsTrue(checkResult);
        Assert.AreEqual(expectedExpression, actualExpression);
    }

    [Test]
    [TestCaseSource(typeof(Unary), nameof(Unary.InvalidNames))]
    public void CheckInvalidUnaryExpressions(string inputName)
    {
        var expression = Unary.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(expression);

        Assert.IsFalse(checkResult);
        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    [TestCaseSource(typeof(Binary), nameof(Binary.InvalidNames))]
    public void CheckInvalidBinaryExpressions(string inputName)
    {
        var expression = Binary.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(expression);

        Assert.IsFalse(checkResult);
        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    [TestCaseSource(typeof(Ternary), nameof(Ternary.InvalidNames))]
    public void CheckInvalidTernaryExpressions(string inputName)
    {
        var expression = Ternary.Inputs[inputName];
        var checkResult = _checker!.CheckTypes(expression);

        Assert.IsFalse(checkResult);
        Assert.IsTrue(_errorReporter.HadError);
    }

    [Test]
    [TestCaseSource(typeof(Variable), nameof(Variable.InvalidNames))]
    public void CheckInvalidVariableExpressions(string inputName)
    {
        var expression = Variable.Inputs[inputName];
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
                    new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Line = 1 }
            )
        );
    }

    [Test]
    public void ThrowOnUnexpectedBinaryOp()
    {
        Assert.Throws<UnreachableException>(() => _checker!.CheckTypes(
                new BinaryExpression(
                    new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 },
                    new Token(TokenKind.Question, "?", null, 1),
                    new LiteralExpression(2) { Type = new WarblerType(ExpressionType.Integer), Line = 1 }
                ) { Line = 1 }
            )
        );
    }
}