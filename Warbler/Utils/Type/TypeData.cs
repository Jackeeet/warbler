using Warbler.Expressions;

namespace Warbler.Utils.Type;

public record TypeData(ExpressionType BaseType, List<TypeData>? Parameters, TypeData? ReturnType);