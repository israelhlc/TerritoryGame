using System;
using Common.Resources;

namespace Common.Commands
{
    #region Enums

    /// <summary>
    /// The possible results of a command
    /// </summary>
    public enum PlayerCommandResult
    {
        /// <summary>
        /// The command was executed with no errors
        /// </summary>
        OK,
        /// <summary>
        /// The command had errors and was not executed
        /// </summary>
        NOK
    }

    #endregion

    /// <summary>
    /// The class for the response sent back to the player after a command.
    /// It contains the command result and the error that may have occurred
    /// </summary>
    public class PlayerCommandResponse
    {
        #region Properties

        /// <summary>
        /// The result of the command
        /// </summary>
        public PlayerCommandResult Result
        {
            get;
            private set;
        }

        /// <summary>
        /// The board resulted after executing the command
        /// </summary>
        public Board ResultBoard
        {
            get;
            private set;
        }

        /// <summary>
        /// The error thrown, when them result was a NOK
        /// </summary>
        public Exception Error
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new command response with an OK result
        /// </summary>
        /// <param name="resultBoard">The board resulted from the command execution</param>
        public PlayerCommandResponse(Board resultBoard) : this(PlayerCommandResult.OK, resultBoard, null) { }

        /// <summary>
        /// Creates a new command response with a NOK result
        /// </summary>
        /// <param name="resultBoard">The board resulted from the command execution</param>
        /// <param name="error">The error which caused the NOK result</param>
        public PlayerCommandResponse(Board resultBoard, Exception error) : this(PlayerCommandResult.NOK, resultBoard, error) { }

        /// <summary>
        /// Creates a new command response with a result and an error
        /// </summary>
        /// <param name="result">The command response result (OK / NOK)</param>
        /// <param name="resultBoard">The board resulted from the command execution</param>
        /// <param name="error">The error which caused the NOK result</param>
        private PlayerCommandResponse(PlayerCommandResult result, Board resultBoard, Exception error)
        {
            Result = result;
            ResultBoard = resultBoard;
            Error = error;
        }

        #endregion
    }
}
