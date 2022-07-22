using Warbler.Errors;
using Warbler.Expressions;
using Warbler.Resources.Errors;

namespace Warbler.Environment;

public class WarblerEnvironment
{
    private Dictionary<string, Tuple<ExpressionType, object?>> _values = new();

    public void Reset()
    {
        _values = new Dictionary<string, Tuple<ExpressionType, object?>>();
    }

    public void Define(string name, ExpressionType type, object? value = null)
    {
        _values[name] = Tuple.Create(type, value);
    }

    public bool Defined(string name)
    {
        return _values.ContainsKey(name);
    }

    public bool DefinedValue(string name)
    {
        return Defined(name) && _values[name].Item2 is not null;
    }

    public Tuple<ExpressionType, object?> Get(Token name, bool typeOnly = false)
    {
        if (!_values.ContainsKey(name.Lexeme))
            throw new RuntimeError(name, string.Format(Runtime.UndefinedVariable, name.Lexeme));

        // technically this should not be an error that ever gets seen by the user
        // so maybe remove this
        if (!typeOnly && _values[name.Lexeme].Item2 is null)
            throw new RuntimeError(name, string.Format(Runtime.UninitializedVariable, name.Lexeme));
        return _values[name.Lexeme];
    }

    public void Assign(Token name, object? value)
    {
        if (!Defined(name.Lexeme))
            throw new RuntimeError(name, Runtime.UndefinedVariable);

        var type = _values[name.Lexeme].Item1;
        _values[name.Lexeme] = Tuple.Create(type, value);
    }
}