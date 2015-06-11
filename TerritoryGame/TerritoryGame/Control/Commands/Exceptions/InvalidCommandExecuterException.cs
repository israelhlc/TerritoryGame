using System;
using Common.Commands;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception thrown when the an invalid (different command commandType) command executer is called to execute a command.
    /// If this exception is thrown there is an error in the game command processing core.
    /// The stack trace must be used to find out the error and fix it.
    /// </summary>
    internal class InvalidCommandExecuterException : Exception
    {
        #region Properties

        /// <summary>
        /// The command commandType needed by the command executer
        /// </summary>
        public CommandType CommandType
        {
            get;
            private set;
        }

        /// <summary>
        /// The command sent to the command executer
        /// </summary>
        public PlayerCommand Command
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new exception based on the command type and the player command
        /// </summary>
        /// <param name="commandType">The command commandType needed by the command executer</param>
        /// <param name="command">The command sent to the command executer</param>
        public InvalidCommandExecuterException(CommandType commandType, PlayerCommand command)
        {
            CommandType = commandType;
            Command = command;
        }

        #endregion
    }
}
