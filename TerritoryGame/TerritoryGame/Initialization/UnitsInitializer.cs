using Common.Resources.Units;

namespace TerritoryGame.Initialization
{
    /// <summary>
    /// Class to initialize the units of the game and its resources
    /// </summary>
    internal static class UnitsInitializer
    {
        #region Methods

        /// <summary>
        /// Initializes the units of the game
        /// </summary>
        public static void InitializeUnits()
        {
            #region Priest

            //sets the attributes for the Priest
            Units.Set(UnitType.Priest,
                new UnitAttributes(
                    maxHealth: 3,
                    movements: 2,
                    attack: 0,
                    defense: 1,
                    range: 1,
                    sight: 2,
                    influenceFactor: 0.5,
                    minimumRandomFactor: 0.6,
                    buildingCost: 10
                )
            );

            #endregion

            #region Soldier

            //sets the attributes for the Soldier
            Units.Set(UnitType.Soldier,
                new UnitAttributes(
                    maxHealth: 1,
                    movements: 2,
                    attack: 1,
                    defense: 1,
                    range: 1,
                    sight: 1,
                    influenceFactor: 0.0,
                    minimumRandomFactor: 0.45,
                    buildingCost: 2
                )
            );

            #endregion

            #region Sergeant

            //sets the attributes for the Sergeant
            Units.Set(UnitType.Sergeant,
                new UnitAttributes(
                    maxHealth: 2,
                    movements: 1,
                    attack: 1,
                    defense: 1,
                    range: 2,
                    sight: 2,
                    influenceFactor: 0.0,
                    minimumRandomFactor: 0.45,
                    buildingCost: 3
                )
            );

            #endregion

            #region Lieutenant

            //sets the attributes for the Lieutenant
            Units.Set(UnitType.Lieutenant,
                new UnitAttributes(
                    maxHealth: 4,
                    movements: 1,
                    attack: 1,
                    defense: 2,
                    range: 1,
                    sight: 1,
                    influenceFactor: 0,
                    minimumRandomFactor: 0.5,
                    buildingCost: 5
                )
            );

            #endregion

            #region Captain

            //sets the attributes for the Captain
            Units.Set(UnitType.Captain,
                new UnitAttributes(
                    maxHealth: 5,
                    movements: 1,
                    attack: 2,
                    defense: 1,
                    range: 1,
                    sight: 1,
                    influenceFactor: 0,
                    minimumRandomFactor: 0.5,
                    buildingCost: 8
                )
            );

            #endregion

            #region Major

            //sets the attributes for the Major
            Units.Set(UnitType.Major,
                new UnitAttributes(
                    maxHealth: 6,
                    movements: 2,
                    attack: 4,
                    defense: 2,
                    range: 1,
                    sight: 1,
                    influenceFactor: 0,
                    minimumRandomFactor: 0.55,
                    buildingCost: 13
                )
            );

            #endregion

            #region Colonel

            //sets the attributes for the Colonel
            Units.Set(UnitType.Colonel,
                new UnitAttributes(
                    maxHealth: 10,
                    movements: 1,
                    attack: 6,
                    defense: 5,
                    range: 1,
                    sight: 1,
                    influenceFactor: 0,
                    minimumRandomFactor: 0.55,
                    buildingCost: 21
                )
            );

            #endregion

            #region General

            //sets the attributes for the General
            Units.Set(UnitType.General,
                new UnitAttributes(
                    maxHealth: 15,
                    movements: 2,
                    attack: 9,
                    defense: 7,
                    range: 2,
                    sight: 3,
                    influenceFactor: 0.05,
                    minimumRandomFactor: 0.6,
                    buildingCost: 30
                )
            );

            #endregion

            #region Marshal

            //sets the attributes for the Marshal
            Units.Set(UnitType.Marshal,
                new UnitAttributes(
                    maxHealth: 20,
                    movements: 3,
                    attack: 12,
                    defense: 9,
                    range: 2,
                    sight: 2,
                    influenceFactor: 0.1,
                    minimumRandomFactor: 0.7,
                    buildingCost: 40
                )
            );

            #endregion
        }

        #endregion
    }
}
