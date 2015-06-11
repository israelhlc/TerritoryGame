using System.Runtime.CompilerServices;
using Common.Commands;
using Common.Resources;

[assembly: InternalsVisibleTo("TerritoryGame")]

namespace Common.AIInterface
{
    /// <summary>
    /// The base class for an AI Player
    /// It has a simple interface so it is simple to receive
    /// the Game State and send Commands to the Game
    /// </summary>
    public abstract class AIBasePlayer
    {
        #region Public Delegates

        /// <summary>
        /// Delegate for the player turn callback, where the player will send the commands to
        /// </summary>
        /// <param name="command">The command the player wants to do</param>
        /// <param name="turnValidationCode">The validation code received in the beginning of the turn</param>
        /// <returns>The result of the command after processed by the game core</returns>
        public delegate PlayerCommandResponse PlayerCommandCallBack(PlayerCommand command, long turnValidationCode);

        #endregion

        #region Properties

        /// <summary>
        /// The Callback which will be used to send the commands to the game
        /// </summary>
        protected PlayerCommandCallBack CallBack
        {
            get;
            private set;
        }

        /// <summary>
        /// The Player ID
        /// </summary>
        public int PlayerID
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the AIBasePlayer setting up the callback function that will be called to send commands
        /// </summary>
        /// <param name="playerID">
        /// The Player ID. 
        /// It must be sent by the game, because it will be used to validate a command.
        /// Changing it will cause the commands of the Player to be rejected by the game.
        /// </param>
        /// <param name="callBack">The callback function that must be called to send the commands</param>
        protected AIBasePlayer(int playerID, PlayerCommandCallBack callBack)
        {
            PlayerID = playerID;
            CallBack = callBack;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// This method is called by the game to invoke the commands of the players' turn.
        /// </summary>
        /// <param name="board">The game state as visible to the player</param>
        /// <param name="turnValidationCode">The validation code to be sent back to the game in with the commands</param>
        public abstract void PlayTurn(Board board, long turnValidationCode);

        #endregion
    }
}
