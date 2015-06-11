using System;
using Common.Resources.Units;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to perform an action with an unit wich has no remaining movements left
    /// </summary>
    internal class NoRemainingMovementsException : Exception
    {
        #region Properties

        /// <summary>
        /// The unit wich has no more movements
        /// </summary>
        public Unit Unit
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new NoRemainingMovementsException with the given parameters
        /// </summary>
        /// <param name="unit">The unit wich has no more movements</param>
        public NoRemainingMovementsException(Unit unit)
        {
            Unit = unit;
        }

        #endregion
    }
}
