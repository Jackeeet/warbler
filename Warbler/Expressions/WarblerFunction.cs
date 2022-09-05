using Warbler.Environment;
using Warbler.Interpreter;

namespace Warbler.Expressions;

public class WarblerFunction : ICallable
{
    private readonly FunctionDeclarationExpression _declaration;
    private readonly WarblerEnvironment _environmentModel;

    public WarblerFunction(FunctionDeclarationExpression declaration, WarblerEnvironment environmentModel)
    {
        _declaration = declaration;
        _environmentModel = environmentModel;
    }

    public object Call(WarblerInterpreter interpreter, List<object> args)
    {
        var localEnvironment = _environmentModel.Copy();
        for (int i = 0; i < _declaration.Parameters.Count; i++)
        {
            var (_, name) = _declaration.Parameters[i];
            localEnvironment.Assign(name, args[i]);
        }

        return interpreter.InterpretBlock(_declaration.Body, localEnvironment) ?? throw new ArgumentException();
    }

    public override string ToString()
    {
        return $"\\ {_declaration.Name.Lexeme} ({_declaration.ReturnType.BaseType})";
    }
}