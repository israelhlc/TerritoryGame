using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common.Resources.Units.Exceptions;

[assembly: InternalsVisibleTo("TerritoryGame")]

namespace Common.Resources.Units
{
    /// <summary>
    /// The class containing all the units' attributes.
    /// It must be set up from a class in TerritoryGame assembly while loading the game for the first time
    /// </summary>
    public static class Units
    {
        #region Members

        private static Dictionary<UnitType, UnitAttributes> _units = new Dictionary<UnitType, UnitAttributes>();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the attributes for a given unit type
        /// </summary>
        /// <param name="type">The type of the unit</param>
        /// <returns>The attributes of the unit</returns>
        public static UnitAttributes Get(UnitType type)
        {
            //tries to get the value in the dictionary
            UnitAttributes attributes;
            if (_units.TryGetValue(type, out attributes))
                return attributes;
            else
                //if the key is not in the dictionary, throws an exception
                throw new UnitNotFoundException(type);
        }

        /// <summary>
        /// Sets the attributes for a given unit type
        /// </summary>
        /// <param name="type">The unit type</param>
        /// <param name="attributes">The attributes</param>
        internal static void Set(UnitType type, UnitAttributes attributes)
        {
            //Removes the old attributes, if there is any
            if (_units.ContainsKey(type))
                _units.Remove(type);

            //Adds the attributes to the dictionary
            _units.Add(type, attributes);
        }

        #endregion
    }
}
