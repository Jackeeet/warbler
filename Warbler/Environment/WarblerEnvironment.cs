using System.Diagnostics;
using Warbler.Errors;
using Warbler.Resources.Errors;
using Warbler.Utils.Exceptions;
using Warbler.Utils.Id;
using Warbler.Utils.Token;
using Warbler.Utils.Type;

namespace Warbler.Environment;

public class WarblerEnvironment
{
    private readonly WarblerEnvironment? _enclosing;
    private readonly Dictionary<EnvId, WarblerEnvironment> _subEnvironments = new();
    private readonly Dictionary<string, WarblerEnvironment> _functionEnvironments = new();
    private readonly Dictionary<string, Tuple<WarblerType, object?>> _values = new();

    public WarblerEnvironment()
    {
        _enclosing = null;
    }

    public WarblerEnvironment(WarblerEnvironment enclosing)
    {
        _enclosing = enclosing;
    }

    private WarblerEnvironment(
        WarblerEnvironment? enclosing,
        Dictionary<EnvId, WarblerEnvironment> subEnvironments,
        Dictionary<string, WarblerEnvironment> functionEnvironments,
        Dictionary<string, Tuple<WarblerType, object?>> values)
    {
        _enclosing = enclosing;

        foreach (var env in subEnvironments)
            _subEnvironments[env.Key] = env.Value.Copy(this);

        foreach (var env in functionEnvironments)
            _functionEnvironments[env.Key] = env.Value.Copy(this);

        foreach (var (name, value) in values)
            _values[name] = value;
    }

    public WarblerEnvironment Copy(WarblerEnvironment? enclosing = null)
    {
        return new WarblerEnvironment(
            enclosing ?? _enclosing,
            _subEnvironments,
            _functionEnvironments,
            _values
        );
    }

    public bool HasSubEnvironment(EnvId environmentId)
    {
        return _subEnvironments.ContainsKey(environmentId);
    }

    public WarblerEnvironment AddSubEnvironment(EnvId environmentId)
    {
        if (_subEnvironments.ContainsKey(environmentId))
            throw new EnvironmentException($"a subenvironment with id {environmentId} already exists");
        _subEnvironments[environmentId] = new WarblerEnvironment(this);
        return _subEnvironments[environmentId];
    }

    public WarblerEnvironment AddFunctionEnvironment(string functionName)
    {
        if (_functionEnvironments.ContainsKey(functionName))
            throw new EnvironmentException($"a function environment with name {functionName} already exists");
        _functionEnvironments[functionName] = new WarblerEnvironment(this);
        return _functionEnvironments[functionName];
    }

    public WarblerEnvironment GetSubEnvironment(EnvId environmentId)
    {
        if (!_subEnvironments.ContainsKey(environmentId))
            throw new EnvironmentException($"no subenvironment with id {environmentId}");
        return _subEnvironments[environmentId];
    }

    public WarblerEnvironment GetFunctionEnvironment(string functionName)
    {
        if (_functionEnvironments.ContainsKey(functionName))
            return _functionEnvironments[functionName];

        if (_enclosing is not null)
            return _enclosing.GetFunctionEnvironment(functionName);

        throw new EnvironmentException($"no function environment with name {functionName}");
    }

    public void Define(string name, WarblerType type, object? value = null)
    {
        _values[name] = Tuple.Create(type, value);
    }

    public bool Defined(string name)
    {
        return DefinedLocal(name) || _enclosing is not null && _enclosing.Defined(name);
    }

    private bool DefinedLocal(string name)
    {
        return _values.ContainsKey(name);
    }

    public void Assign(Token name, object? value)
    {
        if (DefinedLocal(name.Lexeme))
        {
            var type = _values[name.Lexeme].Item1;
            _values[name.Lexeme] = Tuple.Create(type, value);
            return;
        }

        if (_enclosing is null)
            throw new RuntimeError(name, Runtime.UndefinedVariable);

        _enclosing.Assign(name, value);
    }

    public bool Assigned(string name)
    {
        var assignedEnclosing = _enclosing is not null && _enclosing.Assigned(name);
        return Defined(name) && (AssignedLocal(name) || assignedEnclosing);
    }

    private bool AssignedLocal(string name)
    {
        return _values.ContainsKey(name) && _values[name].Item2 is not null;
    }

    public Tuple<WarblerType, object?> GetDefined(Token name)
    {
        if (DefinedLocal(name.Lexeme))
        {
            return _values[name.Lexeme];
        }

        if (_enclosing is not null)
        {
            return _enclosing.GetDefined(name);
        }

        // an undefined variable really should not be a runtime error
        // because we can find undefined variables at checking stage
        // todo throw a more appropriate error
        throw new RuntimeError(name, string.Format(Runtime.UndefinedVariable, name.Lexeme));
    }

    public Tuple<WarblerType, object?> GetAssigned(Token name)
    {
        if (AssignedLocal(name.Lexeme))
        {
            return _values[name.Lexeme];
        }

        if (_enclosing is not null)
        {
            return _enclosing.GetAssigned(name);
        }

        throw new RuntimeError(name, string.Format(Runtime.UnassignedVariable, name.Lexeme));
    }

    public Tuple<WarblerType, object?> GetAt(int level, string name)
    {
        return Ancestor(level)._values[name];
    }

    public void AssignAt(int level, Token name, object? value)
    {
        var env = Ancestor(level);
        var type = env._values[name.Lexeme].Item1;
        env._values[name.Lexeme] = Tuple.Create(type, value);
    }

    public WarblerEnvironment Ancestor(int level)
    {
        var env = this;
        for (int i = 0; i < level - 1; i++)
        // for (int i = 0; i < level; i++)
        {
            Debug.Assert(env != null, nameof(env) + " != null");
            env = env._enclosing;
        }

        Debug.Assert(env != null, nameof(env) + " != null");
        return env;
    }
}