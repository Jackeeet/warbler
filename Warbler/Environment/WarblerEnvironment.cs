using Warbler.Errors;
using Warbler.Expressions;
using Warbler.Resources.Errors;

namespace Warbler.Environment;

public class WarblerEnvironment
{
    private readonly WarblerEnvironment? _enclosing;
    private readonly Dictionary<Guid, WarblerEnvironment> _subEnvironments;
    private Dictionary<string, Tuple<ExpressionType, object?>> _values = new();

    public WarblerEnvironment()
    {
        _enclosing = null;
        _subEnvironments = new Dictionary<Guid, WarblerEnvironment>();
    }

    public WarblerEnvironment(WarblerEnvironment enclosing)
    {
        _enclosing = enclosing;
        _subEnvironments = new Dictionary<Guid, WarblerEnvironment>();
    }

    public void Reset()
    {
        _values = new Dictionary<string, Tuple<ExpressionType, object?>>();
    }

    public void NewSubEnvironment(Guid environmentId)
    {
        if (_subEnvironments.ContainsKey(environmentId))
            throw new ArgumentException($"a subenvironment with id {environmentId} already exists");
        _subEnvironments[environmentId] = new WarblerEnvironment();
    }

    public WarblerEnvironment GetSubEnvironment(Guid environmentId)
    {
        if (!_subEnvironments.ContainsKey(environmentId))
            throw new ArgumentException($"no subenvironment with id {environmentId}");
        return _subEnvironments[environmentId];
    }

    public void Define(string name, ExpressionType type, object? value = null)
    {
        _values[name] = Tuple.Create(type, value);
    }

    public bool Defined(string name)
    {
        return _values.ContainsKey(name);
    }

    public bool Assigned(string name)
    {
        return Defined(name) && _values[name].Item2 is not null;
    }

    public Tuple<ExpressionType, object?> Get(Token name, bool typeOnly = false)
    {
        if (_values.ContainsKey(name.Lexeme))
            return _values[name.Lexeme];

        if (_enclosing is not null)
            return _enclosing.Get(name);

        throw new RuntimeError(name, string.Format(Runtime.UndefinedVariable, name.Lexeme));
    }

    public void Assign(Token name, object? value)
    {
        if (Defined(name.Lexeme))
        {
            var type = _values[name.Lexeme].Item1;
            _values[name.Lexeme] = Tuple.Create(type, value);
            return;
        }

        if (_enclosing is not null)
        {
            _enclosing.Assign(name, value);
            return;
        }

        throw new RuntimeError(name, Runtime.UndefinedVariable);
    }
}