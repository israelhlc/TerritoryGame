using System;
using Common.Resources;
using Common.Resources.Units;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception thrown when a given item type was not found in the building's inventory
    /// </summary>
    internal class NoSuchItemInInventory : Exception
    {
        #region Properties

        /// <summary>
        /// The position where the building was required
        /// </summary>
        public Position Position
        {
            get;
            private set;
        }

        /// <summary>
        /// The ItemType which was not found in the Inventory of the building in the given position
        /// </summary>
        public ItemType Item
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new NoSuchItemInInventory with the given parameters
        /// </summary>
        /// <param name="position">The position where the building is located</param>
        /// <param name="itemType">The ItemType which was not found in the building's inventory</param>
        public NoSuchItemInInventory(Position position, ItemType itemType)
        {
            Position = position;
            Item = itemType;
        }

        #endregion
    }
}
