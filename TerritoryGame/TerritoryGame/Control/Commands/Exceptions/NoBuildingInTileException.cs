using System;
using Common.Resources;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception thrown when a building was required in the tile but it was not found
    /// </summary>
    internal class NoBuildingInTileException : Exception
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

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new NoBuildingInTileException
        /// </summary>
        /// <param name="position"></param>
        public NoBuildingInTileException(Position position)
        {
            Position = position;
        }

        #endregion
    }
}
