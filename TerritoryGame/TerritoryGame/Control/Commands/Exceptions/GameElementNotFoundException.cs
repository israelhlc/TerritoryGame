using System;
using Common.General;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception thrown when a game element was not found
    /// </summary>
    internal class GameElementNotFoundException : Exception
    {
        #region Properties

        /// <summary>
        /// The game element which was not found in the real game objects
        /// </summary>
        public GameElement GameElement
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new GameElementNotFoundException
        /// </summary>
        /// <param name="gameElement">The game element which was not found in the real game objects</param>
        public GameElementNotFoundException(GameElement gameElement)
        {
            GameElement = gameElement;
        }

        #endregion
    }
}
