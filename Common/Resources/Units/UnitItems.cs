using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common.Resources.Units.Exceptions;

[assembly: InternalsVisibleTo("TerritoryGame")]

namespace Common.Resources.Units
{
    #region Enums

    /// <summary>
    /// The type of the item to be held by a unit
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// The item Heart
        /// </summary>
        Heart,
        /// <summary>
        /// The item Sandal
        /// </summary>
        Sandal,
        /// <summary>
        /// The item Armor
        /// </summary>
        Armor,
        /// <summary>
        /// The item Sword
        /// </summary>
        Sword,
        /// <summary>
        /// The item Shield
        /// </summary>
        Shield,
        /// <summary>
        /// The item Belt
        /// </summary>
        Belt,
        /// <summary>
        /// The item Glasses
        /// </summary>
        Glasses,
        /// <summary>
        /// The item Helm
        /// </summary>
        Helm
    }

    #endregion

    /// <summary>
    /// The class containing all the units' items.
    /// An unit item is basically a unit attributes modifier
    /// It must be set up from a class in TerritoryGame assembly while loading the game for the first time
    /// </summary>
    public static class UnitItems
    {
        #region Members

        private static Dictionary<ItemType, UnitAttributes> _items = new Dictionary<ItemType, UnitAttributes>();

        #endregion

        #region Methods

        /// <summary>
        /// Gets the attributes for a given unit type
        /// </summary>
        /// <param name="type">The type of the unit</param>
        /// <returns>The attributes of the unit</returns>
        public static UnitAttributes Get(ItemType type)
        {
            //tries to get the value in the dictionary
            UnitAttributes attributes;
            if (_items.TryGetValue(type, out attributes))
                return attributes;
            else
                //if the key is not in the dictionary, throws an exception
                throw new UnitItemNotFoundException(type);
        }

        /// <summary>
        /// Sets the attributes for a given item type
        /// </summary>
        /// <param name="type">The unit type</param>
        /// <param name="attributes">The attributes</param>
        internal static void Set(ItemType type, UnitAttributes attributes)
        {
            //Removes the old attributes, if there is any
            if (_items.ContainsKey(type))
                _items.Remove(type);

            //Adds the attributes to the dictionary
            _items.Add(type, attributes);
        }

        #endregion
    }
}
