using System;

namespace Common.Resources.Terrains.Exceptions
{
    /// <summary>
    /// An exception thrown when a given TerrainType has not been set up in the game
    /// </summary>
    class TerrainNotFoundException : Exception
    {
        #region Properties

        /// <summary>
        /// The type of the terrain which was not found
        /// </summary>
        public TerrainType TerrainType
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of TerrainNotFoundException, based on the terrain type which was not found
        /// </summary>
        /// <param name="terrainType">The type of the terrain</param>
        public TerrainNotFoundException(TerrainType terrainType)
        {
            TerrainType = terrainType;
        }

        #endregion
    }
}
