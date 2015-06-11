using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TerritoryGame")]

namespace Common.Settings
{
    /// <summary>
    /// Class with the rules of the game
    /// It can be read by all the players, but can only be set by the game control
    /// </summary>
    public static class GameSettings
    {
        #region Properties

        /// <summary>
        /// The number of players in the game
        /// </summary>
        public static int NumberOfPlayers
        {
            get;
            internal set;
        }

        /// <summary>
        /// The board width (number of tiles in the x axis)
        /// </summary>
        public static int BoardWidth
        {
            get;
            internal set;
        }

        /// <summary>
        /// The board height (number of tiles in the y axis)
        /// </summary>
        public static int BoardHeight
        {
            get;
            internal set;
        }

        /// <summary>
        /// The bonus which will be applied if there is more than the units threshold count in the tile
        /// </summary>
        public static double GroupingUnitsBonus
        {
            get;
            internal set;
        }

        /// <summary>
        /// The number of units since when the GroupingUnitsBonus will be applied
        /// </summary>
        public static uint GroupingUnitsThreshold
        {
            get;
            internal set;
        }

        /// <summary>
        /// The Percentage of the health recovered each turn.
        /// It is based in the max health of the unit.
        /// </summary>
        public static float HealthRecoverPerTurn
        {
            get;
            internal set;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the game settings
        /// </summary>
        /// <param name="numberOfPlayers">The number of players</param>
        /// <param name="boardWidth">The board width</param>
        /// <param name="boardHeight">The board height</param>
        /// <param name="groupingUnitsBonus">Bonus applied to units if there is a number of units in the tile according to the threshold</param>
        /// <param name="groupingUnitsThreshold">Threshold to define the minimum of units in the tile to start applying the bonus</param>
        /// <param name="healthRecoverPerTurn">The health recovered by the unit when it has not acted in the turn</param>
        internal static void Initialize(int numberOfPlayers, int boardWidth, int boardHeight,
            double groupingUnitsBonus, uint groupingUnitsThreshold,
            float healthRecoverPerTurn)
        {
            NumberOfPlayers = numberOfPlayers;
            BoardWidth = boardHeight;
            BoardHeight = boardHeight;
            GroupingUnitsBonus = groupingUnitsBonus;
            GroupingUnitsThreshold = groupingUnitsThreshold;
            HealthRecoverPerTurn = healthRecoverPerTurn;
        }

        #endregion
    }
}
