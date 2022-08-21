using Warbler.Environment;
using Warbler.Interpreter;

namespace Warbler.Expressions;

public class WarblerFunction : ICallable
{
    private readonly FunctionDeclarationExpression _declaration;

    public WarblerFunction(FunctionDeclarationExpression declaration)
    {
        _declaration = declaration;
    }

    public object Call(WarblerInterpreter interpreter, WarblerEnvironment callerEnvironment, List<object> args)
    {
        var functionEnvironment = callerEnvironment
            .GetFunctionEnvironment(_declaration.Name.Lexeme)
            .Copy();

        for (int i = 0; i < _declaration.Parameters.Count; i++)
        {
            var (_, name) = _declaration.Parameters[i];
            functionEnvironment.Assign(name, args[i]);
        }

        var functionBodyEnvironment = functionEnvironment.GetSubEnvironment(_declaration.Body.EnvironmentId);
        return interpreter.InterpretBlock(_declaration.Body, functionBodyEnvironment) ?? throw new ArgumentException();
    }

    public int Arity()
    {
        return _declaration.Parameters.Count;
    }

    public override string ToString()
    {
        return $"\\ {_declaration.Name.Lexeme} ({_declaration.ReturnType.BaseType})";
    }
}