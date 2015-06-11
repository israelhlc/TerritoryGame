using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common.General;
using Common.Settings;

[assembly: InternalsVisibleTo("TerritoryGame")]

namespace Common.Resources.Units
{
    #region Enums

    /// <summary>
    /// The name of the unit types
    /// </summary>
    public enum UnitType
    {
        /// <summary>
        /// Unknown unit type - may be used in some board visibility restrictions
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The unit Priest
        /// </summary>
        Priest,
        /// <summary>
        /// The unit Soldier
        /// </summary>
        Soldier,
        /// <summary>
        /// The unit Sergeant
        /// </summary>
        Sergeant,
        /// <summary>
        /// The unit Lieutenant
        /// </summary>
        Lieutenant,
        /// <summary>
        /// The unit Captain
        /// </summary>
        Captain,
        /// <summary>
        /// Major
        /// </summary>
        Major,
        /// <summary>
        /// The unit Colonel
        /// </summary>
        Colonel,
        /// <summary>
        /// The unit General
        /// </summary>
        General,
        /// <summary>
        /// The unit Marshal
        /// </summary>
        Marshal
    }

    #endregion

    /// <summary>
    /// Class representing a game unit.
    /// It has a type (which indicates its attributes), a state and its items
    /// </summary>
    public class Unit : GameElement
    {
        #region Members

        private uint _remainingMovements;

        #endregion

        #region Properties

        /// <summary>
        /// The ID of the player who owns the unit
        /// </summary>
        public int Owner
        {
            get;
            internal set;
        }

        /// <summary>
        /// The type of the unit
        /// </summary>
        public UnitType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// The items owned by the unit
        /// </summary>
        public HashSet<ItemType> Items
        {
            get;
            private set;
        }

        /// <summary>
        /// The current unit position on the board
        /// </summary>
        public Position Position
        {
            get;
            internal set;
        }

        /// <summary>
        /// The Current Health of the unit
        /// </summary>
        public double CurrentHealth
        {
            get;
            set;
        }

        /// <summary>
        /// The turn of the last movement made by the unit
        /// </summary>
        public ulong LastMovementTurn
        {
            get;
            private set;
        }

        /// <summary>
        /// The Remaining Movements in the turn
        /// It also affects the LastMovementTurn
        /// </summary>
        public uint RemainingMovements
        {
            get
            {
                if (LastMovementTurn < GameState.Turn)
                    _remainingMovements = Movements;

                return _remainingMovements;
            }
            internal set
            {
                LastMovementTurn = GameState.Turn;
                _remainingMovements = value;
            }
        }

        #region UnitAttributes Extension

        /// <summary>
        /// The max health (HP) of the unit, considering its type and items
        /// </summary>
        public double MaxHealth
        {
            get
            {
                //adds MaxHealth from all the items of the unit
                double itemsMaxHealth = 0;
                foreach (ItemType itemType in Items)
                {
                    itemsMaxHealth += UnitItems.Get(itemType).MaxHealth;
                }

                //returns the sum of the base attributes and the attached by items
                return Units.Get(Type).MaxHealth + itemsMaxHealth;
            }
        }

        /// <summary>
        /// The number of movements the unit can use each turn, considering its type and items
        /// </summary>
        public uint Movements
        {
            get
            {
                //adds Movements from all the items of the unit
                uint itemsMovements = 0;
                foreach (ItemType itemType in Items)
                {
                    itemsMovements += UnitItems.Get(itemType).Movements;
                }

                //returns the sum of the base attributes and the attached by items
                return Units.Get(Type).Movements + itemsMovements;
            }
        }

        /// <summary>
        /// The attack power of the unit, considering its type and items
        /// </summary>
        public uint Attack
        {
            get
            {
                //adds Attack from all the items of the unit
                uint itemsAttack = 0;
                foreach (ItemType itemType in Items)
                {
                    itemsAttack += UnitItems.Get(itemType).Attack;
                }

                //returns the sum of the base attributes and the attached by items
                return Units.Get(Type).Attack + itemsAttack;
            }
        }

        /// <summary>
        /// The defense power of the unit, considering its type and items
        /// </summary>
        public uint Defense
        {
            get
            {
                //adds Defense from all the items of the unit
                uint itemsDefense = 0;
                foreach (ItemType itemType in Items)
                {
                    itemsDefense += UnitItems.Get(itemType).Defense;
                }

                //returns the sum of the base attributes and the attached by items
                return Units.Get(Type).Defense + itemsDefense;
            }
        }

        /// <summary>
        /// The Range within the unit can perform an action, considering its type and items
        /// </summary>
        public uint Range
        {
            get
            {
                //adds Range from all the items of the unit
                uint itemsRange = 0;
                foreach (ItemType itemType in Items)
                {
                    itemsRange += UnitItems.Get(itemType).Range;
                }

                //returns the sum of the base attributes and the attached by items
                return Units.Get(Type).Range + itemsRange;
            }
        }

        /// <summary>
        /// The requiredRange within the unit can see the tiles, considering its type and items
        /// </summary>
        public uint Sight
        {
            get
            {
                //adds Sight from all the items of the unit
                uint itemsSight = 0;
                foreach (ItemType itemType in Items)
                {
                    itemsSight += UnitItems.Get(itemType).Sight;
                }

                //returns the sum of the base attributes and the attached by items
                return Units.Get(Type).Sight + itemsSight;
            }
        }

        /// <summary>
        /// The territory influence factor of the unit, considering its type and items
        /// </summary>
        public double InfluenceFactor
        {
            get
            {
                //adds InfluenceFactor from all the items of the unit
                double itemsInfluenceFactor = 0;
                foreach (ItemType itemType in Items)
                {
                    itemsInfluenceFactor += UnitItems.Get(itemType).InfluenceFactor;
                }

                //returns the sum of the base attributes and the attached by items
                return Units.Get(Type).InfluenceFactor + itemsInfluenceFactor;
            }
        }

        /// <summary>
        /// The minimum random factor of the unit, used to generate the odds in a battle, considering its type and items
        /// </summary>
        public double MinimumRandomFactor
        {
            get
            {
                //adds MinimumRandomFactor from all the items of the unit
                double itemsMinimumRandomFactor = 0;
                foreach (ItemType itemType in Items)
                {
                    itemsMinimumRandomFactor += UnitItems.Get(itemType).MinimumRandomFactor;
                }

                //returns the sum of the base attributes and the attached by items
                return Units.Get(Type).MinimumRandomFactor + itemsMinimumRandomFactor;
            }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new unit
        /// </summary>
        /// <param name="owner">The ID of the owner of the unit</param>
        /// <param name="type">The type of the unit</param>
        /// <param name="position">The initial position of the unit</param>
        public Unit(int owner, UnitType type, Position position)
        {
            Owner = owner;
            Type = type;
            Items = new HashSet<ItemType>();
            Position = (Position)position.Clone();
            CurrentHealth = Units.Get(type).MaxHealth;
            RemainingMovements = Units.Get(type).Movements;
        }

        /// <summary>
        /// Creates a new unit based on another one (clone)
        /// </summary>
        /// <param name="baseUnit">The base unit</param>
        protected Unit(Unit baseUnit)
            : base(baseUnit)
        {
            //copies the attributes
            Owner = baseUnit.Owner;
            Type = baseUnit.Type;
            Position = (Position)baseUnit.Position.Clone();
            CurrentHealth = Units.Get(baseUnit.Type).MaxHealth;
            RemainingMovements = Units.Get(baseUnit.Type).Movements;
            RemainingMovements = baseUnit.RemainingMovements;

            //copies the item types hash
            Items = new HashSet<ItemType>();
            foreach (ItemType unitItem in baseUnit.Items)
            {
                Items.Add(unitItem);
            }
        }

        #endregion

        #region Methods

        #region ICloneable

        /// <summary>
        /// Clones the unit, return a new deep copy
        /// </summary>
        /// <returns>A deep copy of the unit</returns>
        public override Object Clone()
        {
            return new Unit(this);
        }

        #endregion

        #endregion
    }
}
