using System.Collections.Generic;
using Warbler.Expressions;

namespace Tests.InterpreterTests;

public static class BlockExpressionsData
{
	public static readonly List<string> ValidNames = new() { };

	public static readonly List<string> InvalidNames = new() {};

	public static readonly Dictionary<string, Expression> Inputs = new() {};

	public static readonly Dictionary<string, Expression> Outputs = new() {};
}
