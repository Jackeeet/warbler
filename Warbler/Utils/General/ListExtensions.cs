namespace Warbler.Utils.General;

public static class ListExtensions
{
    public static bool AllEquals<T>(this List<T> list, List<T>? other)
    {
        if (other is null)
            return false;

        if (list.Count != other.Count)
            return false;

        return list
            .Zip(other)
            .All(zp =>
                zp.First is null && zp.Second is null ||
                zp.First is not null && zp.First.Equals(zp.Second)
            );
    }
}