namespace BusinessLogicLayer.Helper;

public static class ICollectionHelper
{
    public static void RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> predicate)
    {
        var itemsToRemove = collection.Where(predicate).ToList();

        foreach (var item in itemsToRemove)
        {
            collection.Remove(item);
        }
    }
}