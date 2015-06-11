using System.Collections.Generic;
using Common.AIInterface;
using Common.Settings;
using TerritoryGame.Control;
using TerritoryGame.Control.Commands;
using TerritoryGame.Control.GamePlayers;
using TerritoryGame.Settings;

namespace TerritoryGame.Initialization
{
    /// <summary>
    /// Class to initialize the game contents
    /// </summary>
    internal static class GameInitializer
    {
        #region Resources and Settings

        /// <summary>
        /// Initializes all the game resources
        /// </summary>
        public static void InitializeGameResources()
        {
            //initialize units
            UnitsInitializer.InitializeUnits();

            //initialize unit items
            UnitItemsInitializer.InitializeUnitItems();

            //initialize buildings
            BuildingsInitializer.InitializeBuildings();

            //initializa terrains
            TerrainsInitializer.InitializeTerrains();
        }

        /// <summary>
        /// Initializes the game settings
        /// </summary>
        public static void InitializeGameSettings()
        {
            GameSettings.Initialize(
                Rules.Default.DefaultNumberOfPlayers,
                Rules.Default.DefaultBoardWidth,
                Rules.Default.DefaultBoardHeight,
                Rules.Default.GroupingUnitsBonus,
                Rules.Default.GroupingUnitsThreshold,
                Rules.Default.HealthRecoverPerTurn
            );
        }

        #endregion

        #region Controls

        /// <summary>
        /// Initialize the game controls and all the logic needed to run the game
        /// </summary>
        public static void InitializeGameControls()
        {
            CommandProcessor.Initialize();
            GameManager.Initialize();
        }

        #endregion

        #region Players

        /// <summary>
        /// Initialize the basePlayers with the default number of basePlayers and the given callback method
        /// </summary>
        public static void InitializePlayers(AIBasePlayer.PlayerCommandCallBack callback)
        {
            InitializePlayers(GameSettings.NumberOfPlayers, callback);
        }

        /// <summary>
        /// Initialize the players with the given number of basePlayers and callback method
        /// </summary>
        /// <param name="numberOfPlayers">The number of basePlayers</param>
        /// <param name="callback">The callback function for the players commands</param>
        public static void InitializePlayers(int numberOfPlayers, AIBasePlayer.PlayerCommandCallBack callback)
        {
            Players.Initialize(numberOfPlayers, callback);
        }

        /// <summary>
        /// Initialize the players with the given basePlayers list
        /// </summary>
        /// <param name="basePlayers"></param>
        public static void InitializePlayers(List<AIBasePlayer> basePlayers)
        {
            Players.Initialize(basePlayers);
        }

        #endregion
    }
}
