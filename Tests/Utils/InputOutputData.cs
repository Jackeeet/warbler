using System;
using System.Collections.Generic;

namespace Tests.Utils;

public class InputOutputData<TI, TO>
{
    private readonly Dictionary<string, Tuple<TI, TO>> _data;

    public InputOutputData()
    {
        _data = new Dictionary<string, Tuple<TI, TO>>();
    }

    public void Add(string key, TI input, TO output)
    {
        this[key] = Tuple.Create(input, output);
    }

    public Tuple<TI, TO> this[string key]
    {
        get => _data[key];
        set => _data[key] = value;
    }

    public TI Input(string key)
    {
        return this[key].Item1;
    }

    public TO Output(string key)
    {
        return this[key].Item2;
    }
}