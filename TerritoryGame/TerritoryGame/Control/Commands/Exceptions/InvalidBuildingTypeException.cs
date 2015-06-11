using System;
using Common.Resources.Buildings;

namespace TerritoryGame.Control.Commands.Exceptions
{
    /// <summary>
    /// Exception thrown when an invalid building type was given in a command
    /// </summary>
    internal class InvalidBuildingTypeException : Exception
    {
        #region Properties

        /// <summary>
        /// The building type given
        /// </summary>
        public BuildingType? BuildingType
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new InvalidBuildingTypeException with the given parameter
        /// </summary>
        /// <param name="buildingType">The building type given</param>
        public InvalidBuildingTypeException(BuildingType? buildingType)
        {
            BuildingType = buildingType;
        }

        #endregion
    }
}
