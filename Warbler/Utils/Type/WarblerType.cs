using Warbler.Expressions;

namespace Warbler.Utils.Type;

public record struct WarblerType(ExpressionType BaseType, Signature? Signature = null);