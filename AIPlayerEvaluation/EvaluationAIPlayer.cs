using System;
using System.Collections.Generic;
using System.Linq;
using Common.AIInterface;
using Common.Resources;

namespace AIPlayerEvaluation
{
    /// <summary>
    /// This is the class where the implementation of the AI shall begin
    /// As it is, the player just don't do anything on its turn
    /// If needed, see reference at AIPlayerExample.AIPlayer
    /// </summary>
    public class EvaluationAIPlayer : AIBasePlayer
    {
        #region Interface AIBasePlayer

        /// <summary>
        /// Initializes the AIPlayerExample player
        /// </summary>
        /// <param name="playerID">The ID of the player in the game - cannot be changed</param>
        /// <param name="callBack">The callback function that must be called to send the commands</param>
        public EvaluationAIPlayer(int playerID, AIBasePlayer.PlayerCommandCallBack callBack)
            : base(playerID, callBack) { }

        /// <summary>
        /// This method is called by the game to invoke the commands of the players' turn.
        /// The player must send all the commands to the callback and then finish its turn.
        /// It is a simple example of AI, capable only of executing the basics of the game, no strategy:
        /// </summary>
        /// <param name="board">The game board as visible to the player</param>
        /// <param name="turnValidationCode">The validation code to be sent back to the game in with the commands</param>
        public override void PlayTurn(Board board, long turnValidationCode)
        {
            //TODO the code of the AI must be implemented here
        }

        #endregion
    }
}
