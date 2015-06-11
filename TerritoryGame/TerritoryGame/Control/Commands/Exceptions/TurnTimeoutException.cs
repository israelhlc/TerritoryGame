using System;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception to be sent to the player's turn thread when it is being aborted due to a timeout (too much time processing)
    /// </summary>
    internal class TurnTimeoutException : Exception
    {
    }
}
