using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common.Resources.Terrains.Exceptions;

[assembly: InternalsVisibleTo("TerritoryGame")]

namespace Common.Resources.Terrains
{
    /// <summary>
    /// The class containing all the Terrains' attributes.
    /// It must be set up from a class in TerritoryGame assembly while loading the game for the first time
    /// </summary>
    public static class Terrains
    {
        #region Members

        private static Dictionary<TerrainType, Terrain> _terrains = new Dictionary<TerrainType, Terrain>();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the attributes for a given terrain type
        /// </summary>
        /// <param name="type">The type of the terrain</param>
        /// <returns>The attributes of the terrain</returns>
        public static Terrain Get(TerrainType type)
        {
            //tries to get the value in the dictionary
            Terrain terrain;
            if (_terrains.TryGetValue(type, out terrain))
                return terrain;
            else
                //if the key is not in the dictionary, throws an exception
                throw new TerrainNotFoundException(type);
        }

        /// <summary>
        /// Sets the attributes for a given terrain type
        /// </summary>
        /// <param name="type">The terrain type</param>
        /// <param name="attributes">The attributes</param>
        internal static void Set(TerrainType type, Terrain attributes)
        {
            //Removes the old attributes, if there is any
            if (_terrains.ContainsKey(type))
                _terrains.Remove(type);

            //Adds the attributes to the dictionary
            _terrains.Add(type, attributes);
        }

        #endregion
    }
}
