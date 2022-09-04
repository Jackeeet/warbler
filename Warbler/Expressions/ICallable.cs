using Warbler.Environment;
using Warbler.Interpreter;

namespace Warbler.Expressions;

public interface ICallable
{
    public object Call(WarblerInterpreter interpreter, WarblerEnvironment callerEnvironment, List<object> args);
}