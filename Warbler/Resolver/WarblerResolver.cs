using System.Diagnostics;
using Warbler.ErrorReporting;
using Warbler.Expressions;
using Warbler.Utils.Token;

namespace Warbler.Resolver;

public class WarblerResolver : IExpressionVisitor<object?>
{
    private readonly IErrorReporter _errorReporter;
    private readonly Stack<Dictionary<string, bool>> _scopes;
    private readonly Dictionary<Expression, int?> _locals = new();

    public WarblerResolver(IErrorReporter errorReporter)
    {
        _errorReporter = errorReporter;
        _scopes = new Stack<Dictionary<string, bool>>();
    }

    public Dictionary<Expression, int?> Resolve(List<Expression> expressions)
    {
        foreach (var expression in expressions)
        {
            Debug.Assert(expression != null, nameof(expression) + " != null");
            Resolve(expression);
        }

        return _locals;
    }

    private void Resolve(Expression expression)
    {
        expression.Accept(this);
    }

    private void ResolveLocal(Expression expression, Token name)
    {
        var scopes = _scopes.Reverse().ToArray();
        for (int i = scopes.Length - 1; i >= 0; i--)
        {
            if (scopes[i].ContainsKey(name.Lexeme))
            {
                _locals[expression] = scopes.Length - i - 1;
                return;
            }
        }

        // this means that the variable is global
        _locals[expression] = null;
    }

    private void BeginScope()
    {
        _scopes.Push(new Dictionary<string, bool>());
    }

    private void EndScope()
    {
        _scopes.Pop();
    }

    private void Declare(Token name)
    {
        if (_scopes.Count != 0)
        {
            var scope = _scopes.Peek();
            if (scope.ContainsKey(name.Lexeme))
            {
                _errorReporter.ErrorAtToken(name,
                    $"A variable with the name {name.Lexeme} is already declared in this scope");
            }

            scope[name.Lexeme] = false;
        }
    }

    private void Define(Token name)
    {
        if (_scopes.Count != 0)
        {
            _scopes.Peek()[name.Lexeme] = true;
        }
    }

    public object? VisitUnaryExpression(UnaryExpression expression)
    {
        Resolve(expression.Expression);
        return null;
    }

    public object? VisitBinaryExpression(BinaryExpression expression)
    {
        Resolve(expression.Left);
        Resolve(expression.Right);
        return null;
    }

    public object? VisitTernaryExpression(TernaryExpression expression)
    {
        Resolve(expression.Condition);
        Resolve(expression.ThenBranch);
        Resolve(expression.ElseBranch);
        return null;
    }

    public object? VisitLiteralExpression(LiteralExpression expression)
    {
        return null;
    }

    public object? VisitVariableDeclarationExpression(VariableDeclarationExpression expression)
    {
        Declare(expression.Name);
        Resolve(expression.Initializer);
        Define(expression.Name);

        return null;
    }

    public object? VisitVariableExpression(VariableExpression expression)
    {
        ResolveLocal(expression, expression.Name);
        return null;
    }

    public object? VisitAssignmentExpression(AssignmentExpression expression)
    {
        Resolve(expression.Value);
        ResolveLocal(expression, expression.Name);
        return null;
    }

    public object? VisitBlockExpression(BlockExpression expression)
    {
        BeginScope();
        Resolve(expression.Expressions!);
        EndScope();
        return null;
    }

    public object? VisitConditionalExpression(ConditionalExpression expression)
    {
        Resolve(expression.Condition);
        ResolveInnerBlock(expression.ThenBranch);
        if (expression.ElseBranch is not null)
            ResolveInnerBlock(expression.ElseBranch);
        return null;
    }

    public object? VisitWhileLoopExpression(WhileLoopExpression expression)
    {
        Resolve(expression.Condition);
        ResolveInnerBlock(expression.Actions);
        return null;
    }

    private void ResolveInnerBlock(Expression expression)
    {
        if (expression is BlockExpression blockExpression)
        {
            foreach (var expr in blockExpression.Expressions)
            {
                Debug.Assert(expr != null, nameof(expr) + " != null");
                Resolve(expr);
            }
        }
        else
        {
            Resolve(expression);
        }
    }

    public object? VisitFunctionDeclarationExpression(FunctionDeclarationExpression expression)
    {
        Declare(expression.Name);
        Define(expression.Name);
        ResolveFunction(expression);
        return null;
    }

    private void ResolveFunction(FunctionDeclarationExpression expression)
    {
        BeginScope();
        foreach (var (_, paramName) in expression.Parameters)
        {
            Declare(paramName);
            Define(paramName);
        }

        foreach (var expr in expression.Body.Expressions)
        {
            Debug.Assert(expr != null, nameof(expr) + " != null");
            Resolve(expr);
        }

        EndScope();
    }

    public object? VisitCallExpression(CallExpression expression)
    {
        Resolve(expression.Called);
        foreach (var arg in expression.Args)
            Resolve(arg);
        return null;
    }
}