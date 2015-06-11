using System;

namespace TerritoryGame.Control.Commands.UnitActions.Exceptions
{
    /// <summary>
    /// Exception thrown when a given destination in a MoveTo command is unreachable by the unit in the turn
    /// </summary>
    internal class UnreacheableDestinationException : Exception
    {
        #region Properties

        /// <summary>
        /// The required movements for the command
        /// </summary>
        public uint RequiredMovements
        {
            get;
            private set;
        }

        /// <summary>
        /// The remaining movements of the unit on the turn
        /// </summary>
        public uint RemainingMovements
        {
            get;
            private set;
        }

        /// <summary>
        /// The required requiredRange for the command
        /// </summary>
        public uint RequiredRange
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new UnreacheableDestinationException
        /// </summary>
        /// <param name="requiredMovements">The required movements for the command</param>
        /// <param name="remainingMovements">The remaining movements of the unit on the turn</param>
        public UnreacheableDestinationException(uint requiredMovements, uint remainingMovements)
        {
            RequiredMovements = requiredMovements;
            RemainingMovements = remainingMovements;
        }

        /// <summary>
        /// Creates a new UnreacheableDestinationException
        /// </summary>
        /// <param name="requiredRange">The required requiredRange for the attack</param>
        public UnreacheableDestinationException(uint requiredRange)
        {
            RequiredRange = requiredRange;
        }

        #endregion
    }
}
