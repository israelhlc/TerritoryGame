using System;
using Common.Resources.Units;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception thrown when the unit already has the item trying to be equipped on it
    /// </summary>
    internal class UnitAlreadyHasItemException : Exception
    {
        #region Properties

        /// <summary>
        /// The unit which was trying to equip
        /// </summary>
        public Unit Unit
        {
            get;
            private set;
        }

        /// <summary>
        /// The item which the unit already has
        /// </summary>
        public ItemType Item
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new UnitAlreadyHasItemException with the given parameters
        /// </summary>
        /// <param name="unit">The unit which was trying to equip</param>
        /// <param name="item">The item which the unit already has</param>
        public UnitAlreadyHasItemException(Unit unit, ItemType item)
        {
            Unit = unit;
            Item = item;
        }

        #endregion
    }
}
