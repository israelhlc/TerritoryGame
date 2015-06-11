using System;
using System.Collections.Generic;
using Common.General;
using Common.Resources.Units;

namespace Common.Resources.Buildings
{
    #region Enums

    /// <summary>
    /// The types of buildings in the game
    /// </summary>
    public enum BuildingType
    {
        /// <summary>
        /// The building type City
        /// </summary>
        City,
        /// <summary>
        /// The building type Barrack
        /// </summary>
        Barrack
    }

    #endregion

    /// <summary>
    /// Class for the buildings of the game.
    /// A building is a unmovable resource which produces resources
    /// </summary>
    public class Building : GameElement
    {
        #region Members

        private ProduceableElement _currentProduction;

        #endregion

        #region Properties

        /// <summary>
        /// The owner (player) ID
        /// </summary>
        public int Owner
        {
            get;
            private set;
        }

        /// <summary>
        /// The type of the building
        /// </summary>
        public BuildingType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// The position of the building in the map
        /// </summary>
        public Position Position
        {
            get;
            private set;
        }

        /// <summary>
        /// The progress of the corrent resource being produced
        /// </summary>
        public uint ProductionProgress
        {
            get;
            internal set;
        }

        /// <summary>
        /// The current resource being produced
        /// </summary>
        public ProduceableElement CurrentProduction
        {
            get
            {
                return _currentProduction;
            }
            internal set
            {
                _currentProduction = value;
                ProductionProgress = 0;
            }
        }

        /// <summary>
        /// The unitItems stored in the building, to be equiped in a unit
        /// </summary>
        public List<ItemType> UnitItemsInventory
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new building
        /// </summary>
        /// <param name="owner">The ID of the owner of the unit</param>
        /// <param name="type">The type of the building</param>
        /// <param name="position">The initial position of the unit</param>
        public Building(int owner, BuildingType type, Position position) : this(owner, type, position, null) { }

        /// <summary>
        /// Creates a new building
        /// </summary>
        /// <param name="owner">The ID of the owner of the unit</param>
        /// <param name="type">The type of the building</param>
        /// <param name="position">The initial position of the unit</param>
        /// <param name="baseBuilding">The base building</param>
        protected Building(int owner, BuildingType type, Position position, Building baseBuilding)
            : base(baseBuilding)
        {
            Owner = owner;
            Type = type;
            Position = (Position)position.Clone();
            ProductionProgress = 0;
            CurrentProduction = null;
            UnitItemsInventory = new List<ItemType>();
        }

        /// <summary>
        /// Creates a new building based on another one (clone)
        /// </summary>
        /// <param name="baseBuilding">The base building</param>
        protected Building(Building baseBuilding)
            : this(baseBuilding.Owner, baseBuilding.Type, baseBuilding.Position, baseBuilding)
        {
            //copies the other attributes
            ProductionProgress = baseBuilding.ProductionProgress;
            CurrentProduction = (ProduceableElement)baseBuilding.CurrentProduction.Clone();

            //copies the inventory
            foreach (ItemType itemType in baseBuilding.UnitItemsInventory)
            {
                UnitItemsInventory.Add(itemType);
            }
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Clones the building, return a new deep copy
        /// </summary>
        /// <returns>A deep copy of the building</returns>
        public override Object Clone()
        {
            return new Building(this);
        }

        #endregion
    }
}
