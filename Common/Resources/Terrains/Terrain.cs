using System;
using Common.Util;

namespace Common.Resources.Terrains
{
    #region Enums

    /// <summary>
    /// The types of terrains in the game
    /// </summary>
    public enum TerrainType
    {
        /// <summary>
        /// An unknown terrain - used when a player has no visibility of the terrain
        /// </summary>
        Unknown,
        /// <summary>
        /// The terrain type Grass
        /// </summary>
        Grass,
        /// <summary>
        /// The terrain type Sand
        /// </summary>
        Sand,
        /// <summary>
        /// The terrain type Forest
        /// </summary>
        Forest,
        /// <summary>
        /// The terrain type Water
        /// </summary>
        Water
    }

    #endregion

    /// <summary>
    /// The class for a tile terrain. It contains a terrain type and the properties associated to it
    /// </summary>
    public class Terrain
    {
        #region Properties

        /// <summary>
        /// The type of the terrain
        /// As the key of the object, it cannot be modified
        /// </summary>
        public TerrainType TerrainType
        {
            get;
            private set;
        }

        /// <summary>
        /// The cost for a unit to move to this terrain
        /// </summary>
        public uint MovementCost
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether the terrain can receive ground units
        /// </summary>
        public bool CanReceiveGroundUnits
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether the terrain can receive water units
        /// </summary>
        public bool CanReceiveWaterUnits
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether the terrain can receive aerial units
        /// </summary>
        public bool CanReceiveAerialUnits
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether the terrain can receive a building
        /// </summary>
        public bool CanReceiveBuilding
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a random known terrain type, i. e. all the types in TerrainType except the Unknown
        /// </summary>
        public static TerrainType RandomTerrainType
        {
            get
            {
                return (TerrainType)RandomUtil.Random.Next(Enum.GetValues(typeof(TerrainType)).Length - 1) + 1;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a terrain type, which will define some rules of the game
        /// </summary>
        /// <param name="terrainType">The type of the terrain</param>
        /// <param name="movementCost">The cost for a unit to move to this terrain</param>
        /// <param name="canReceiveGroundUnits">Indicates whether the terrain can receive ground units</param>
        /// <param name="canReceiveWaterUnits">Indicates whether the terrain can receive water units</param>
        /// <param name="canReceiveAerialUnits">Indicates whether the terrain can receive aerial units</param>
        /// <param name="canReceiveBuilding">Indicates whether the terrain can receive a building</param>
        public Terrain(TerrainType terrainType, uint movementCost, bool canReceiveGroundUnits,
            bool canReceiveWaterUnits, bool canReceiveAerialUnits, bool canReceiveBuilding)
        {
            TerrainType = terrainType;
            MovementCost = movementCost;
            CanReceiveGroundUnits = canReceiveGroundUnits;
            CanReceiveWaterUnits = canReceiveWaterUnits;
            CanReceiveAerialUnits = canReceiveAerialUnits;
            CanReceiveBuilding = canReceiveBuilding;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Implements the Equals Comparator for the TileTerrain class
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>true iif the terrain type is the same</returns>
        public override bool Equals(object obj)
        {
            //verifies if parameter is null
            if (obj == null)
            {
                return false;
            }

            //casts the object
            Terrain terrain = obj as Terrain;
            if (terrain == null)
            {
                return false;
            }

            //returns the comparator between the keys
            return TerrainType.Equals(terrain.TerrainType);
        }

        /// <summary>
        /// Implements the GetHashCode method to keep consistency with Equals
        /// </summary>
        /// <returns>The hash code for the object</returns>
        public override int GetHashCode()
        {
            return (int)TerrainType;
        }

        #endregion
    }
}
