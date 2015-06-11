using System;

namespace Common.Resources.Units.Exceptions
{
    /// <summary>
    /// An exception thrown when a given ItemType has not been set up in the game
    /// </summary>
    public class UnitItemNotFoundException : Exception
    {
        #region Constructor

        /// <summary>
        /// Constructor of UnitItemNotFoundException, based on the item type which was not found
        /// </summary>
        /// <param name="itemType">The type of the item</param>
        public UnitItemNotFoundException(ItemType itemType)
        {
            ItemType = itemType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type of the item which was not found
        /// </summary>
        public ItemType ItemType
        {
            get;
            private set;
        }

        #endregion
    }
}
