using System;

namespace Common.Resources.Units.Exceptions
{
    /// <summary>
    /// An exception thrown when a given UnitType has not been set up in the game
    /// </summary>
    public class UnitNotFoundException : Exception
    {
        #region Properties

        /// <summary>
        /// The type of the unit which was not found
        /// </summary>
        public UnitType UnitType
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of UnitNotFoundException, based on the unit type which was not found
        /// </summary>
        /// <param name="unitType">The type of the unit</param>
        public UnitNotFoundException(UnitType unitType)
        {
            UnitType = unitType;
        }

        #endregion
    }
}
