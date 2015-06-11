using System;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception to be throws when a command executer (inside CommandProcessor) 
    /// was required but has not been defined
    /// </summary>
    internal class CommandExecuterNotDefinedException : Exception
    {
    }
}
