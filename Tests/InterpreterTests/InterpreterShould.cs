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
        DefineGlobals();
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
    private void DefineGlobals()
    {
        _environment.Define("intVar", ExpressionType.Integer);

        foreach (var name in Variable.DeclarationNames)
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

        foreach (var name in Variable.AssignmentNames)
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
    [TestCaseSource(typeof(Literal), nameof(Literal.ValidNames))]
    public void EvaluateValidLiteralExpressions(string inputName)
    {
        var expected = Literal.Outputs[inputName];

        var actual = _interpreter.Interpret(Literal.Inputs[inputName]);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Unary), nameof(Unary.ValidNames))]
    public void EvaluateValidUnaryExpressions(string inputName)
    {
        var expected = Unary.Outputs[inputName];

        var actual = _interpreter.Interpret(Unary.Inputs[inputName]);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Unary), nameof(Unary.InvalidNames))]
    public void ThrowOnInvalidUnaryExpressions(string inputName)
    {
        Assert.Throws<ArgumentException>(() => _interpreter.Interpret(Unary.Inputs[inputName]));
    }

    [Test]
    [TestCaseSource(typeof(Binary), nameof(Binary.ValidNames))]
    [DefaultFloatingPointTolerance(0.000001)]
    public void EvaluateValidBinaryExpressions(string inputName)
    {
        var expected = Binary.Outputs[inputName];

        var actual = _interpreter.Interpret(Binary.Inputs[inputName]);

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
    [TestCaseSource(typeof(Binary), nameof(Binary.InvalidNames))]
    public void ThrowOnInvalidBinaryExpressions(string inputName)
    {
        Assert.Throws<ArgumentException>(() => _interpreter.Interpret(Binary.Inputs[inputName]));
    }

    [Test]
    [TestCaseSource(typeof(Ternary), nameof(Ternary.ValidNames))]
    public void EvaluateValidTernaryExpressions(string inputName)
    {
        var expected = Ternary.Outputs[inputName];

        var actual = _interpreter.Interpret(Ternary.Inputs[inputName]);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    [TestCaseSource(typeof(Ternary), nameof(Ternary.InvalidNames))]
    public void ThrowOnInvalidTernaryExpressions(string inputName)
    {
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
        Assert.True(_environment.Defined(inputName));
        var expected = Variable.Outputs[inputName];

        var returnValue = _interpreter.Interpret(Variable.Inputs[inputName]);
        Assert.True(_environment.Assigned(inputName));
        var (_, storedValue) = _environment.Get(
            new Token(TokenKind.Identifier, inputName, null, 1));

        Assert.AreEqual(expected, storedValue);
        Assert.AreEqual(expected, returnValue);
    }

    [Test]
    [TestCaseSource(typeof(Variable), nameof(Variable.AssignmentNames))]
    public void EvaluateValidVariableAssignmentExpressions(string inputName)
    {
        Assert.True(_environment.Assigned(inputName));
        var variableToken = new Token(TokenKind.Identifier, inputName, null, 1);

        var initialValue = _environment.Get(variableToken);
        var expected = Variable.Outputs[inputName];

        var returnValue = _interpreter.Interpret(Variable.Inputs[inputName]);
        Assert.True(_environment.Assigned(inputName));
        var (_, storedValue) = _environment.Get(variableToken);

        Assert.AreEqual(expected, storedValue);
        Assert.AreNotEqual(initialValue, storedValue);
        Assert.AreEqual(expected, returnValue);
    }

    [Test]
    [TestCaseSource(typeof(Variable), nameof(Variable.InvalidNames))]
    public void ThrowOnUndefinedVariable(string inputName)
    {
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

        _environment.NewSubEnvironment(outerBlockId);
        _environment
            .GetSubEnvironment(outerBlockId)
            .Define("block", ExpressionType.Integer);
        _environment
            .GetSubEnvironment(outerBlockId)
            .Define("block2", ExpressionType.Integer);

        _environment.GetSubEnvironment(outerBlockId).NewSubEnvironment(innerBlockId);
        _environment
            .GetSubEnvironment(outerBlockId)
            .GetSubEnvironment(innerBlockId)
            .Define("block", ExpressionType.Integer);

        var expected = Block.Outputs[inputName];

        var actual = _interpreter.Interpret(Block.Inputs[inputName]);

        Assert.AreEqual(expected, actual);
    }
}