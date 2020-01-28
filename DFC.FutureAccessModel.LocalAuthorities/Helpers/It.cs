using System.Collections.Generic;
using System.Linq;

namespace DFC.FutureAccessModel.LocalAuthorities.Helpers
{
    /// <summary>
    /// a static helper class to increase code readability
    /// </summary>
    public static class It
    {
        /// <summary>
        /// is null...
        /// </summary>
        /// <typeparam name="TEntity">the entity type</typeparam>
        /// <param name="theItem">the item</param>
        /// <returns>true if null</returns>
        public static bool IsNull<TEntity>(TEntity theItem)
            where TEntity : class => theItem is null;

        /// <summary>
        /// is empty
        /// </summary>
        /// <param name="theItem">the item</param>
        /// <returns>true if null or whitespace</returns>
        public static bool IsEmpty(string theItem) =>
            string.IsNullOrWhiteSpace(theItem);

        /// <summary>
        /// is empty
        /// </summary>
        /// <typeparam name="TEntity">the entity type</typeparam>
        /// <param name="theItems">the items (instance)</param>
        /// <returns></returns>
        public static bool IsEmpty<TItem>(IEnumerable<TItem> theItems) =>
            !theItems.Any();

        /// <summary>
        /// has...
        /// </summary>
        /// <typeparam name="TEntity">the entity type</typeparam>
        /// <param name="theItem">the item (instance)</param>
        /// <returns>true if not null</returns>
        public static bool Has<TEntity>(TEntity theItem)
            where TEntity : class =>
            !IsNull(theItem);

        /// <summary>
        /// has...
        /// </summary>
        /// <typeparam name="TEntity">the entity type</typeparam>
        /// <param name="theItem">the item (instance)</param>
        /// <returns>true if not null</returns>
        public static bool Has<TEntity>(TEntity? theItem)
            where TEntity : struct =>
            theItem != null;
    }
}
