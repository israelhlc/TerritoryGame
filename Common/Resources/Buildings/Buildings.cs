using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common.Resources.Buildings.Exceptions;

[assembly: InternalsVisibleTo("TerritoryGame")]

namespace Common.Resources.Buildings
{
    /// <summary>
    /// The class containing all the buildings' attributes.
    /// It must be set up from a class in TerritoryGame assembly while loading the game for the first time
    /// </summary>
    public static class Buildings
    {
        #region Members

        private static Dictionary<BuildingType, BuildingAttributes> _buildings = new Dictionary<BuildingType, BuildingAttributes>();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the attributes for a given building type
        /// </summary>
        /// <param name="type">The type of the building</param>
        /// <returns>The attributes of the building</returns>
        public static BuildingAttributes Get(BuildingType type)
        {
            //tries to get the value in the dictionary
            BuildingAttributes building;
            if (_buildings.TryGetValue(type, out building))
                return building;
            else
                //if the key is not in the dictionary, throws an exception
                throw new BuildingNotFoundException(type);
        }

        /// <summary>
        /// Sets the building for a given type
        /// </summary>
        /// <param name="type">The building type</param>
        /// <param name="attributes">The building attributes</param>
        internal static void Set(BuildingType type, BuildingAttributes attributes)
        {
            //Removes the old attributes, if there is any
            if (_buildings.ContainsKey(type))
                _buildings.Remove(type);

            //Adds the new attributes to the dictionary
            _buildings.Add(type, attributes);
        }

        #endregion
    }
}
