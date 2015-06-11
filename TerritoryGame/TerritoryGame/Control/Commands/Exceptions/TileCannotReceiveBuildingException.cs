using System;
using Common.Resources;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to create a building on a tile which does not allow building creation
    /// </summary>
    internal class TileCannotReceiveBuildingException : Exception
    {
        #region Properties

        /// <summary>
        /// The position of the tile where the building cannot be created
        /// </summary>
        public Position Position
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new TileCannotReceiveBuildingException with the given parameters
        /// </summary>
        /// <param name="position">The position of the tile where the building cannot be created</param>
        public TileCannotReceiveBuildingException(Position position)
        {
            Position = position;
        }

        #endregion
    }
}
