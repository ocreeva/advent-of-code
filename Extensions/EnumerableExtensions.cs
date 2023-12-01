namespace System.Collection.Generic
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> AssertAll<T>(this IEnumerable<T> enumerable, Func<T, bool> assert, Func<T, string> error)
        {
            foreach (var item in enumerable)
            {
                if (!assert(item)) throw new Exception(error(item));
                yield return item;
            }
        }
    }
}
