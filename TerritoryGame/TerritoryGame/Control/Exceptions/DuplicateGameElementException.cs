using System;

namespace TerritoryGame.Control.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to add a duplicate (same ID) Game Element in the GameElementsManager
    /// </summary>
    internal class DuplicateGameElementException : Exception
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
        /// Creates a new DuplicateGameElementException
        /// </summary>
        /// <param name="gameElementID">The Game Element ID</param>
        public DuplicateGameElementException(ulong gameElementID)
        {
            GameElementID = gameElementID;
        }

        #endregion
    }
}
