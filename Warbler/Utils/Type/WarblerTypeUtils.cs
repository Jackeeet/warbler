using System.Diagnostics;
using Warbler.Expressions;

namespace Warbler.Utils.Type;

public static class WarblerTypeUtils
{
    public static WarblerType ToWarblerType(TypeData data)
    {
        if (data.BaseType != ExpressionType.Function)
            return new WarblerType(data.BaseType);

        var parameterTypes = new List<WarblerType>();
        Debug.Assert(data.Parameters != null, "expression.Parameters != null");
        foreach (var te in data.Parameters)
        {
            parameterTypes.Add(ToWarblerType(te));
        }

        Debug.Assert(data.ReturnType != null, "expression.ReturnType != null");
        var returnType = ToWarblerType(data.ReturnType);

        return new WarblerType(ExpressionType.Function, new Signature(parameterTypes, returnType));
    }
}