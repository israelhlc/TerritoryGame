using System;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception thrown when a command sent to the game is not allowed in the rules, 
    /// for example when trying to move to a tile with an enemy unit
    /// </summary>
    internal class TileNotOwnedByException : Exception
    {
    }
}
