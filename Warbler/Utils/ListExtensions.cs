namespace Warbler.Utils;

public static class ListExtensions
{
    public static bool AllEquals<T>(this List<T> list, List<T> other)
    {
        if (list.Count != other.Count)
            return false;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] is null && other[i] is null || !list[i]!.Equals(other[i]))
                return false;
        }

        return true;
    }
}