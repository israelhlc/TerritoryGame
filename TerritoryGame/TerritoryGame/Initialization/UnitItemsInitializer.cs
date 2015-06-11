using Common.Resources.Units;

namespace TerritoryGame.Initialization
{
    /// <summary>
    /// Class to initialize the unit items of the game and its resources
    /// </summary>
    internal static class UnitItemsInitializer
    {
        #region Methods

        /// <summary>
        /// Initializes the unit items of the game
        /// </summary>
        public static void InitializeUnitItems()
        {
            #region Heart

            //sets the attributes for the Heart
            UnitItems.Set(ItemType.Heart,
                new UnitAttributes(
                    maxHealth: 3,
                    movements: 0,
                    attack: 0,
                    defense: 0,
                    range: 0,
                    sight: 0,
                    influenceFactor: 0.0,
                    minimumRandomFactor: 0.0,
                    buildingCost: 15
                )
            );

            #endregion

            #region Sandal

            //sets the attributes for the Sandal
            UnitItems.Set(ItemType.Sandal,
                new UnitAttributes(
                    maxHealth: 0,
                    movements: 1,
                    attack: 0,
                    defense: 0,
                    range: 0,
                    sight: 0,
                    influenceFactor: 0.0,
                    minimumRandomFactor: 0.0,
                    buildingCost: 10
                )
            );

            #endregion

            #region Armor

            //sets the attributes for the Armor
            UnitItems.Set(ItemType.Armor,
                new UnitAttributes(
                    maxHealth: 0,
                    movements: 0,
                    attack: 0,
                    defense: 0,
                    range: 0,
                    sight: 0,
                    influenceFactor: 0.0,
                    minimumRandomFactor: 0.2,
                    buildingCost: 15
                )
            );

            #endregion

            #region Sword

            //sets the attributes for the Sword
            UnitItems.Set(ItemType.Sword,
                new UnitAttributes(
                    maxHealth: 0,
                    movements: 0,
                    attack: 3,
                    defense: 0,
                    range: 0,
                    sight: 0,
                    influenceFactor: 0.0,
                    minimumRandomFactor: 0.0,
                    buildingCost: 25
                )
            );

            #endregion

            #region Shield

            //sets the attributes for the Shield
            UnitItems.Set(ItemType.Shield,
                new UnitAttributes(
                    maxHealth: 0,
                    movements: 0,
                    attack: 0,
                    defense: 2,
                    range: 0,
                    sight: 0,
                    influenceFactor: 0.0,
                    minimumRandomFactor: 0.0,
                    buildingCost: 20
                )
            );

            #endregion

            #region Belt

            //sets the attributes for the Belt
            UnitItems.Set(ItemType.Belt,
                new UnitAttributes(
                    maxHealth: 0,
                    movements: 0,
                    attack: 0,
                    defense: 0,
                    range: 1,
                    sight: 0,
                    influenceFactor: 0.0,
                    minimumRandomFactor: 0.0,
                    buildingCost: 20
                )
            );

            #endregion

            #region Glasses

            //sets the attributes for the Glasses
            UnitItems.Set(ItemType.Glasses,
                new UnitAttributes(
                    maxHealth: 0,
                    movements: 0,
                    attack: 0,
                    defense: 0,
                    range: 0,
                    sight: 1,
                    influenceFactor: 0.0,
                    minimumRandomFactor: 0.0,
                    buildingCost: 10
                )
            );

            #endregion

            #region Helm

            //sets the attributes for the Helm
            UnitItems.Set(ItemType.Helm,
                new UnitAttributes(
                    maxHealth: 0,
                    movements: 0,
                    attack: 0,
                    defense: 0,
                    range: 0,
                    sight: 0,
                    influenceFactor: 0.25,
                    minimumRandomFactor: 0.0,
                    buildingCost: 30
                )
            );

            #endregion
        }

        #endregion
    }
}
