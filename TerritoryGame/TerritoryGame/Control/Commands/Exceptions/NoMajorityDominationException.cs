using System;
using Common.Resources;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception thrown when the player don't have enough domain on a territory
    /// to reach majority domination (greater than 50%) 
    /// </summary>
    internal class NoMajorityDominationException : Exception
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

        /// <summary>
        /// The current domain of the player on the tile
        /// </summary>
        public double CurrentDomain
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new NoMajorityDominationException
        /// </summary>
        /// <param name="position">The position of the tile</param>
        /// <param name="currentDomain">The current domain of the player on the tile</param>
        public NoMajorityDominationException(Position position, double currentDomain)
        {
            Position = position;
            CurrentDomain = currentDomain;
        }

        #endregion
    }
}
