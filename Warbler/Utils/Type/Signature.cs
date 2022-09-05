using Warbler.Utils.General;

namespace Warbler.Utils.Type;

public record Signature(List<WarblerType> Parameters, WarblerType ReturnType)
{
    public virtual bool Equals(Signature? other)
    {
        if (other is null)
            return false;

        return ReturnType == other.ReturnType &&
               Parameters.AllEquals(other.Parameters);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ReturnType, Parameters);
    }
}