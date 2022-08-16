using Warbler.Environment;
using Warbler.Interpreter;
using Warbler.Utils.Type;

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
        // var functionEnvironment = SetupCallEnvironment(callerEnvironment, args);
        var functionEnvironment = callerEnvironment
            .GetFunctionEnvironment(_declaration.Name.Lexeme)
            .Copy();

        for (int i = 0; i < _declaration.Parameters.Count; i++)
        {
            var (_, name) = _declaration.Parameters[i];
            functionEnvironment.Assign(name, args[i]);
        }
        
        return interpreter.InterpretBlock(_declaration.Body, functionEnvironment) ?? throw new ArgumentException();
    }

    private WarblerEnvironment SetupCallEnvironment(WarblerEnvironment callerEnvironment, List<object> args)
    {
        var functionEnvironment = new WarblerEnvironment(callerEnvironment);
        foreach (var (typeData, name) in _declaration.Parameters)
            functionEnvironment.Define(name.Lexeme, WarblerTypeUtils.ToWarblerType(typeData));

        functionEnvironment.AddSubEnvironment(_declaration.Body.EnvironmentId);

        var signature = _declaration.Type.Signature;
        if (signature is null)
            throw new ArgumentException();

        for (int i = 0; i < _declaration.Parameters.Count; i++)
        {
            var (_, name) = _declaration.Parameters[i];
            functionEnvironment.Define(name.Lexeme, signature.Parameters[i], args[i]);
        }

        return functionEnvironment;
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