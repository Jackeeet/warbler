﻿using Warbler.Expressions;

namespace Warbler.Errors;

public class RuntimeError : Exception
{
    public readonly Token Token;

    public RuntimeError()
    {
    }

    public RuntimeError(Token token, string message) : base(message)
    {
        Token = token;
    }
}