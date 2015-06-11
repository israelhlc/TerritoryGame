using System;
using Common.Resources;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception thrown when a building is already in a tile
    /// and it is attempted to create a new building on it
    /// </summary>
    internal class BuildingAlreadyInTileException : Exception
    {
        #region Properties

        /// <summary>
        /// The position of the tile
        /// </summary>
        public Position Position
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new BuildingAlreadyInTileException with the given parameter
        /// </summary>
        /// <param name="position">The position of the tile</param>
        public BuildingAlreadyInTileException(Position position)
        {
            Position = position;
        }

        #endregion
    }
}
