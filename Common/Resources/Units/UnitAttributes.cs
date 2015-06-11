using System;
using Common.General;

namespace Common.Resources.Units
{
    /// <summary>
    /// The fixed attributes of a unit in the game
    /// </summary>
    public class UnitAttributes : GameElement
    {
        #region Properties

        /// <summary>
        /// The max health (HP) of the unit
        /// </summary>
        public double MaxHealth
        {
            get;
            protected set;
        }

        /// <summary>
        /// The number of movements the unit can use each turn
        /// </summary>
        public uint Movements
        {
            get;
            protected set;
        }

        /// <summary>
        /// The attack power of the unit
        /// </summary>
        public uint Attack
        {
            get;
            protected set;
        }

        /// <summary>
        /// The defense power of the unit
        /// </summary>
        public uint Defense
        {
            get;
            protected set;
        }

        /// <summary>
        /// The requiredRange within the unit can perform an action
        /// </summary>
        public uint Range
        {
            get;
            protected set;
        }

        /// <summary>
        /// The requiredRange within the unit can see the tiles
        /// </summary>
        public uint Sight
        {
            get;
            protected set;
        }

        /// <summary>
        /// The territory influence factor of the unit
        /// </summary>
        public double InfluenceFactor
        {
            get;
            protected set;
        }

        /// <summary>
        /// The minimum random factor of the unit, used to generate the odds in a battle
        /// </summary>
        public double MinimumRandomFactor
        {
            get;
            protected set;
        }

        /// <summary>
        /// The building cost of the unit attribute
        /// </summary>
        public uint BuildingCost
        {
            get;
            protected set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Intantiate a new Unit Attributes Class, with the attributes for a unit type
        /// </summary>
        /// <param name="maxHealth">The max health (HP) of the unit</param>
        /// <param name="movements">The number of movements the unit can use each turn</param>
        /// <param name="attack">The attack power of the unit</param>
        /// <param name="defense">The defense power of the unit</param>
        /// <param name="range">The requiredRange within the unit can perform an attack</param>
        /// <param name="sight">The requiredRange within the unit can see the tiles</param>
        /// <param name="influenceFactor">The territory influence factor of the unit</param>
        /// <param name="minimumRandomFactor">The minimum random factor of the unit, used to generate the odds in a battle</param>
        /// <param name="buildingCost">The building cost of the unit attribute</param>
        public UnitAttributes(double maxHealth, uint movements, uint attack, uint defense, uint range, uint sight, double influenceFactor, double minimumRandomFactor, uint buildingCost)
            : this(maxHealth, movements, attack, defense, range, sight, influenceFactor, minimumRandomFactor, buildingCost, null) { }

        /// <summary>
        /// Intantiate a new Unit Attributes Class, with the attributes for a unit type, cloning the base ID
        /// </summary>
        /// <param name="maxHealth">The max health (HP) of the unit</param>
        /// <param name="movements">The number of movements the unit can use each turn</param>
        /// <param name="attack">The attack power of the unit</param>
        /// <param name="defense">The defense power of the unit</param>
        /// <param name="range">The requiredRange within the unit can perform an attack</param>
        /// <param name="sight">The requiredRange within the unit can see the tiles</param>
        /// <param name="influenceFactor">The territory influence factor of the unit</param>
        /// <param name="minimumRandomFactor">The minimum random factor of the unit, used to generate the odds in a battle</param>
        /// <param name="buildingCost">The building cost of the unit attribute</param>
        /// <param name="unitAttributes">The base UnitAttributes, to clone the ID</param>
        protected UnitAttributes(double maxHealth, uint movements, uint attack, uint defense, uint range, uint sight, double influenceFactor, double minimumRandomFactor, uint buildingCost, UnitAttributes unitAttributes)
            : base(unitAttributes)
        {
            MaxHealth = maxHealth;
            Movements = movements;
            Attack = attack;
            Defense = defense;
            Range = range;
            Sight = sight;
            InfluenceFactor = influenceFactor;
            MinimumRandomFactor = minimumRandomFactor;
            BuildingCost = buildingCost;
        }

        /// <summary>
        /// Creates a new unit attributes based on another one (clone)
        /// </summary>
        /// <param name="unitAttributes">The base UnitAttributes</param>
        protected UnitAttributes(UnitAttributes unitAttributes)
            : this(unitAttributes.MaxHealth, unitAttributes.Movements, unitAttributes.Attack,
                unitAttributes.Defense, unitAttributes.Range, unitAttributes.Sight, unitAttributes.InfluenceFactor, unitAttributes.MinimumRandomFactor, unitAttributes.BuildingCost, unitAttributes) { }

        #endregion

        #region Operators

        /// <summary>
        /// Adds two unit attributes, what is the same of adding all its values
        /// </summary>
        /// <param name="firstUnitAttributes">The first position (operator left side)</param>
        /// <param name="secondUnitAttributes">The second position (operator right side)</param>
        /// <returns>The new attributes - the sum of all its properties</returns>
        public static UnitAttributes operator +(UnitAttributes firstUnitAttributes, UnitAttributes secondUnitAttributes)
        {
            return new UnitAttributes(
                firstUnitAttributes.MaxHealth + secondUnitAttributes.MaxHealth,
                firstUnitAttributes.Movements + secondUnitAttributes.Movements,
                firstUnitAttributes.Attack + secondUnitAttributes.Attack,
                firstUnitAttributes.Defense + secondUnitAttributes.Defense,
                firstUnitAttributes.Range + secondUnitAttributes.Range,
                firstUnitAttributes.Sight + secondUnitAttributes.Sight,
                firstUnitAttributes.InfluenceFactor + secondUnitAttributes.InfluenceFactor,
                firstUnitAttributes.MinimumRandomFactor + secondUnitAttributes.MinimumRandomFactor,
                firstUnitAttributes.BuildingCost + secondUnitAttributes.BuildingCost
                );
        }

        #endregion

        #region Methods

        #region ICloneable

        /// <summary>
        /// Clones the unit attributes, return a new deep copy
        /// </summary>
        /// <returns>A deep copy of the unit attributes</returns>
        public override Object Clone()
        {
            return new UnitAttributes(this);
        }

        #endregion

        #endregion
    }
}
