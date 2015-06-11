using System;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception to be sent to a thread when the command processor was aborted and thus all the threads it created are being aborted too
    /// </summary>
    internal class CommandProcessorAbortedException : Exception
    {
    }
}
