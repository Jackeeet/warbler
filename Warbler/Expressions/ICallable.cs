using Warbler.Interpreter;

namespace Warbler.Expressions;

public interface ICallable
{
    public object Call(WarblerInterpreter interpreter, List<object> args);
}