using System.Collections.Generic;

namespace Common.Util
{
    /// <summary>
    /// Class with util methods for manipulating lists
    /// </summary>
    public static class ListUtil
    {
        #region Methods

        /// <summary>
        /// Gets a list based on the given list, but with no duplicates
        /// </summary>
        /// <typeparam name="T">The type of list</typeparam>
        /// <param name="list">The list with duplicates</param>
        /// <returns>The list without duplicates</returns>
        public static List<T> GetListWithoutDuplicates<T>(List<T> list)
        {
            //creates a hash with the collection to remove duplicates
            HashSet<T> hashWithoutDuplicates = new HashSet<T>(list);

            //returns a list with the elements of the hash
            return new List<T>(hashWithoutDuplicates);
        }

        #endregion
    }
}
