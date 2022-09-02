using System.Collections.Generic;
using NUnit.Framework;
using Tests.Mocks;
using Tests.Utils;
using Warbler.Expressions;
using Warbler.Resolver;
using Warbler.Utils.Type;

namespace Tests.ResolverTests;

[TestFixture]
public class ResolverShould
{
    private TestReporter _errorReporter = null!;
    private WarblerResolver _resolver = null!;
    private InputOutputData<List<Expression>, Dictionary<Expression, int?>> _basicData = null!;
    private InputOutputData<List<Expression>, Dictionary<Expression, int?>> _varDeclData = null!;
    private InputOutputData<List<Expression>, Dictionary<Expression, int?>> _variableData = null!;

    [OneTimeSetUp]
    public void BeforeFixture()
    {
        _errorReporter = new TestReporter();
        _basicData = new Basic().GetInputsOutputs();
        _varDeclData = new VarDecl().GetInputsOutputs();
        _variableData = new Variable().GetInputsOutputs();
    }

    [SetUp]
    public void BeforeTest()
    {
        _errorReporter.Reset();
        _resolver = new WarblerResolver(_errorReporter);
    }

    [Test]
    public void ResolveLiteralExpressions()
    {
        var exprs = new List<Expression>()
        {
            new LiteralExpression(4) { Line = 1, Type = new WarblerType(ExpressionType.Integer) }
        };
        var expected = new Dictionary<Expression, int?>();

        var actual = _resolver.Resolve(exprs);
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Basic), nameof(Basic.ValidNames))]
    public void ResolveBasicExpressions(string inputName)
    {
        var expected = _basicData.Output(inputName);
        var actual = _resolver.Resolve(_basicData.Input(inputName));
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(VarDecl), nameof(VarDecl.ValidNames))]
    public void ResolveVarDeclExpressions(string inputName)
    {
        var expected = _varDeclData.Output(inputName);
        var actual = _resolver.Resolve(_varDeclData.Input(inputName));
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Variable), nameof(Variable.ValidNames))]
    public void ResolveVariableExpressions(string inputName)
    {
        var expected = _variableData.Output(inputName);
        var actual = _resolver.Resolve(_variableData.Input(inputName));
        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void ResolveAssignmentExpressions()
    {
        Assert.Fail();
    }

    [Test]
    public void ResolveBlockExpressions()
    {
        Assert.Fail();
    }

    [Test]
    public void ResolveConditionalExpressions()
    {
        Assert.Fail();
    }

    [Test]
    public void ResolveWhileLoopExpressions()
    {
        Assert.Fail();
    }

    [Test]
    public void ResolveFunctionDeclarationExpressions()
    {
        Assert.Fail();
    }

    [Test]
    public void ResolveCallExpressions()
    {
        Assert.Fail();
    }
}