using System.Collections.Generic;
using System.Linq;
using AIPlayerEvaluation;
using AIPlayerExample;
using Common.AIInterface;
using Common.Util;
using TerritoryGame.Control.GamePlayers.Exceptions;

namespace TerritoryGame.Control.GamePlayers
{
    /// <summary>
    /// Class for the basePlayers' manipulation
    /// </summary>
    internal static class Players
    {
        #region Members

        private static List<Player> _players = new List<Player>();

        #endregion

        #region Properties

        /// <summary>
        /// The current index of the basePlayers' array
        /// </summary>
        private static int CurrentIndex
        {
            get;
            set;
        }

        /// <summary>
        /// The current basePlayer
        /// </summary>
        public static Player CurrentPlayer
        {
            get
            {
                //if there is no player, throw an exception
                if (NumberOfPlayers == 0)
                    throw new PlayersNotInitializedException();

                return _players[CurrentIndex];
            }
        }

        /// <summary>
        /// The next basePlayer (cyclical)
        /// It has side effects, as after this property the CurrentPlayer is changed
        /// </summary>
        public static Player NextPlayer
        {
            get
            {
                //if there is no player, throw an exception
                if (NumberOfPlayers == 0)
                    throw new PlayersNotInitializedException();

                //searches next alive player
                int lastPlayer = CurrentIndex;
                do
                {
                    //gets next player
                    CurrentIndex = (CurrentIndex + 1) % _players.Count;

                    //if reaches the last player again, throw the exception and do not enter a loop
                    if (CurrentIndex == lastPlayer)
                        throw new NoNextPlayerAvailableException();

                } while (!CurrentPlayer.IsAlive);

                //if the player search restarted the index, increments the turn counter in the game
                if (CurrentIndex < lastPlayer)
                    GameManager.NextTurn();

                //returns the found alive player
                return CurrentPlayer;
            }
        }

        /// <summary>
        /// The number of players in the game
        /// </summary>
        public static int NumberOfPlayers
        {
            get
            {
                return _players.Count;
            }
        }

        /// <summary>
        /// Returns the Players's IDs
        /// </summary>
        public static List<int> PlayersIDs
        {
            get
            {
                //selects the ids of the players
                IEnumerable<int> ids = from player in _players select player.ID;

                //return the ids as a list
                return ids.ToList();
            }
        }

        #endregion

        #region Initializer

        /// <summary>
        /// Initialize the players using the AIPlayer class
        /// </summary>
        /// <param name="numberOfPlayers">The number of players</param>
        /// <param name="callback">The callback function for the players commands</param>
        public static void Initialize(int numberOfPlayers, AIBasePlayer.PlayerCommandCallBack callback)
        {
            //Creates the first players - it will be the evaluation player
            AIBasePlayer player = new EvaluationAIPlayer(RandomUtil.NextInt, callback);
            _players.Add(new Player(player));

            //Adds the remaining players - they will be the example player
            for (int i = 1; i < numberOfPlayers; i++)
            {
                //Creates the new player
                player = new AIPlayer(RandomUtil.NextInt, callback);
                _players.Add(new Player(player));
            }
        }

        /// <summary>
        /// Initialize the players using a list of base players
        /// </summary>
        /// <param name="basePlayers">The list with the players</param>
        public static void Initialize(List<AIBasePlayer> basePlayers)
        {
            //Checks if the list is empty
            if (basePlayers == null)
            {
                return;
            }

            //Adds the players to the list
            _players = new List<Player>(basePlayers.Count);
            foreach (AIBasePlayer basePlayer in basePlayers)
            {
                _players.Add(new Player(basePlayer));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the given player as not alive in the game anymore
        /// </summary>
        /// <param name="playerID">The ID of the eliminated player</param>
        public static void SetPlayerNotAlive(int playerID)
        {
            //finds the player in the list
            Player player = _players.Find(p => p.ID == playerID);
            if (player != null)
            {
                //sets the player as not alive anymore
                player.IsAlive = false;
            }
        }

        #endregion
    }
}
