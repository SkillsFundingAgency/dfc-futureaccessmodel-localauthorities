using System.Collections.ObjectModel;

namespace DFC.FutureAccessModel.LocalAuthorities.Helpers
{
    public static class Collection
    {
        /// <summary>
        /// returns an empty collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ICollection<T> Empty<T>() => new List<T>();

        /// <summary>
        /// For each, to safe list and conducts the action
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            It.IsNull(action)
                .AsGuard<ArgumentNullException>();

            var items = collection.SafeList();
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// For each, to safe list and conducts the action
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="action">The action.</param>
        public static void ForEach<T>(this IReadOnlyCollection<T> collection, Action<T> action)
        {
            It.IsNull(action)
                .AsGuard<ArgumentNullException>();

            var items = collection.SafeList();
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// As a safe readonly list.
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>
        /// a readonly safe collection
        /// </returns>
        public static IReadOnlyCollection<T> AsSafeReadOnlyList<T>(this IEnumerable<T> list)
        {
            return list.SafeReadOnlyList();
        }

        /// <summary>
        /// Safe list, the private implementation of null coalescing
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>
        /// a safe list
        /// </returns>
        private static List<T> SafeList<T>(this IEnumerable<T> list)
        {
            return (list ?? new List<T>()).ToList();
        }

        /// <summary>
        /// Safe read only list.
        /// </summary>
        /// <typeparam name="T">of type</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>
        /// a safe readonly list
        /// </returns>
        private static IReadOnlyCollection<T> SafeReadOnlyList<T>(this IEnumerable<T> list)
        {
            return new ReadOnlyCollection<T>(list.SafeList());
        }
    }
}
