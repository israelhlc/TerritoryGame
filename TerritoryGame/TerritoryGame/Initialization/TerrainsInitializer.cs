using System;
using Common.Resources.Terrains;

namespace TerritoryGame.Initialization
{
    /// <summary>
    /// Class to initialize the terrains of the game and its resources
    /// </summary>
    internal static class TerrainsInitializer
    {
        #region Methods

        /// <summary>
        /// Initializes the terrains of the game
        /// </summary>
        public static void InitializeTerrains()
        {
            #region Unknown

            //Sets the attributes for an unknown terrain
            Terrains.Set(TerrainType.Unknown,
                new Terrain(
                    terrainType: TerrainType.Unknown,
                    movementCost: Int32.MaxValue,
                    canReceiveGroundUnits: true,
                    canReceiveWaterUnits: true,
                    canReceiveAerialUnits: true,
                    canReceiveBuilding: false
                )
            );

            #endregion

            #region Grass

            //Sets the attributes for the Grass
            Terrains.Set(TerrainType.Grass,
                new Terrain(
                    terrainType: TerrainType.Grass,
                    movementCost: 1,
                    canReceiveGroundUnits: true,
                    canReceiveWaterUnits: false,
                    canReceiveAerialUnits: true,
                    canReceiveBuilding: true
                )
            );

            #endregion

            #region Sand

            //Sets the attributes for the Sand
            Terrains.Set(TerrainType.Sand,
                new Terrain(
                    terrainType: TerrainType.Sand,
                    movementCost: 2,
                    canReceiveGroundUnits: true,
                    canReceiveWaterUnits: false,
                    canReceiveAerialUnits: true,
                    canReceiveBuilding: true
                )
            );

            #endregion

            #region Forest

            //Sets the attributes for the Forest
            Terrains.Set(TerrainType.Forest,
                new Terrain(
                    terrainType: TerrainType.Forest,
                    movementCost: 3,
                    canReceiveGroundUnits: true,
                    canReceiveWaterUnits: false,
                    canReceiveAerialUnits: true,
                    canReceiveBuilding: false
                )
            );

            #endregion

            #region Water

            //Sets the attributes for the Water
            Terrains.Set(TerrainType.Water,
                new Terrain(
                    terrainType: TerrainType.Water,
                    movementCost: 1,
                    canReceiveGroundUnits: false,
                    canReceiveWaterUnits: true,
                    canReceiveAerialUnits: true,
                    canReceiveBuilding: false
                )
            );

            #endregion
        }

        #endregion
    }
}
