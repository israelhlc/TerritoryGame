using System;

namespace Common.Resources.Buildings.Exceptions
{
    /// <summary>
    /// An exception thrown when a given BuildingType has not been set up in the game
    /// </summary>
    public class BuildingNotFoundException : Exception
    {
        #region Properties

        /// <summary>
        /// The type of the building which was not found
        /// </summary>
        public BuildingType BuildingType
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of BuildingNotFoundException, based on the building type which was not found
        /// </summary>
        /// <param name="buildingType">The type of the unit</param>
        public BuildingNotFoundException(BuildingType buildingType)
        {
            BuildingType = buildingType;
        }

        #endregion
    }
}
