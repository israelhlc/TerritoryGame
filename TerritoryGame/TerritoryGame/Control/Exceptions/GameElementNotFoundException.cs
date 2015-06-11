using System;

namespace TerritoryGame.Control.Exceptions
{
    internal class GameElementNotFoundException : Exception
    {
        #region Properties

        /// <summary>
        /// The Game Element ID
        /// </summary>
        public ulong GameElementID
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new GameElementNotFoundException
        /// </summary>
        /// <param name="gameElementID">The game element ID</param>
        public GameElementNotFoundException(ulong gameElementID)
        {
            GameElementID = gameElementID;
        }

        #endregion
    }
}
