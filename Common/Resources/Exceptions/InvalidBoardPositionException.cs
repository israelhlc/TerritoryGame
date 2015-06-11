using System;

namespace Common.Resources.Exceptions
{
    /// <summary>
    /// Exception to be thrown when trying to retrieve a position out of the board limits
    /// </summary>
    class InvalidBoardPositionException : Exception
    {
        #region Properties

        /// <summary>
        /// The invalid position
        /// </summary>
        public Position Position
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new InvalidBoardPositionException for a given position
        /// </summary>
        /// <param name="position">The invalid position</param>
        public InvalidBoardPositionException(Position position)
        {
            Position = position;
        }

        #endregion
    }
}
