using Common.AIInterface;

namespace TerritoryGame.Control.GamePlayers
{
    /// <summary>
    /// Class representing a basePlayer, in the Game view
    /// </summary>
    internal class Player
    {
        #region Properties

        /// <summary>
        /// The basePlayer ID
        /// </summary>
        public int ID
        {
            get
            {
                return Instance.PlayerID;
            }
        }

        /// <summary>
        /// Indicates if the basePlayer is still alive
        /// </summary>
        public bool IsAlive
        {
            get;
            internal set;
        }

        /// <summary>
        /// The instance of the player, which will be used to call the turn
        /// </summary>
        public AIBasePlayer Instance
        {
            get;
            internal set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a new Player
        /// </summary>
        /// <param name="instance">The instance of the AI Player</param>
        public Player(AIBasePlayer instance)
        {
            IsAlive = true;
            Instance = instance;
        }

        #endregion

        #region override

        /// <summary>
        /// Compares the Player to another Player and return true when they are the same
        /// </summary>
        /// <param name="obj">The other Player instance</param>
        /// <returns>true iif the objects are equals (deep comparison)</returns>
        public override bool Equals(object obj)
        {
            //verifies if playerAsObject is null
            if (obj == null)
            {
                return false;
            }

            //casts the object
            Player player = obj as Player;
            if (player == null)
            {
                return false;
            }

            //returns the comparator between the keys
            return ID.Equals(player.ID);
        }

        /// <summary>
        /// Implements the GetHashCode method
        /// </summary>
        /// <returns>The hash code for the object, which will be its unique ID</returns>
        public override int GetHashCode()
        {
            return ID;
        }

        #endregion
    }
}
