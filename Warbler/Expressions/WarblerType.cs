namespace Warbler.Expressions;

public record struct WarblerType(ExpressionType BaseType, Signature? Signature = null);