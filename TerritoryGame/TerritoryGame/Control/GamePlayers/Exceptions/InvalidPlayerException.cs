using System;

namespace TerritoryGame.Control.GamePlayers.Exceptions
{
    /// <summary>
    /// Exception thrown when a received object was expected to be a Player, but the cast failed
    /// </summary>
    internal class InvalidPlayerException : Exception
    {

        #region Properties

        /// <summary>
        /// The object which was not able to be cast to Player
        /// </summary>
        public object InvalidPlayer
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new InvalidPlayerException instance
        /// </summary>
        /// <param name="invalidPlayer">The object which was not able to be cast to Player</param>
        public InvalidPlayerException(object invalidPlayer)
        {
            InvalidPlayer = invalidPlayer;
        }

        #endregion
    }
}
