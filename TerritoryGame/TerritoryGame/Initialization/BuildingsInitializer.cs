using Common.Resources.Buildings;

namespace TerritoryGame.Initialization
{
    /// <summary>
    /// Class to initialize the buildings of the game and its resources
    /// </summary>
    internal static class BuildingsInitializer
    {
        #region Methods

        /// <summary>
        /// Initializes the buildings of the game
        /// </summary>
        public static void InitializeBuildings()
        {
            #region City

            //Sets the attributes for the City
            Buildings.Set(BuildingType.City,
                new BuildingAttributes(
                    productionRate: 2,
                    influenceFactor: 0.5
                )
            );

            #endregion

            #region Barrack

            //Sets the attributes for the Barrack
            Buildings.Set(BuildingType.Barrack,
                new BuildingAttributes(
                    productionRate: 3,
                    influenceFactor: 0.0
                )
            );

            #endregion
        }

        #endregion
    }
}
