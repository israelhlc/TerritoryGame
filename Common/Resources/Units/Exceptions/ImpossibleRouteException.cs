using System;

namespace Common.Resources.Units.Exceptions
{
    /// <summary>
    /// Exception thrown when no route can be traced from the start to the end in the given board
    /// </summary>
    class ImpossibleRouteException : Exception
    {
        #region Properties

        /// <summary>
        /// The game board used to trace the route
        /// </summary>
        public Board Board
        {
            get;
            private set;
        }

        /// <summary>
        /// The route start position
        /// </summary>
        public Position Start
        {
            get;
            private set;
        }

        /// <summary>
        /// The route end (goal) position
        /// </summary>
        public Position End
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new ImpossibleRouteException
        /// </summary>
        /// <param name="board">The game board used to trace the route</param>
        /// <param name="start">The route start position</param>
        /// <param name="end">The route end (goal) position</param>
        public ImpossibleRouteException(Board board, Position start, Position end)
        {
            Board = board;
            Start = start;
            End = end;
        }

        #endregion
    }
}
