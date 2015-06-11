using System;
using Common.General;

namespace Common.Resources.Buildings
{
    /// <summary>
    /// Class for the buildings' attributes
    /// </summary>
    public class BuildingAttributes : GameElement
    {
        #region Properties

        /// <summary>
        /// The production rate (how much it produces per turn)
        /// </summary>
        public uint ProductionRate
        {
            get;
            private set;
        }

        /// <summary>
        /// The territory influence factor of the building
        /// </summary>
        public double InfluenceFactor
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new attributes for a building
        /// </summary>
        /// <param name="productionRate">The production rate (how much it produces per turn)</param>
        /// <param name="influenceFactor">The territory influence factor of the building</param>
        public BuildingAttributes(uint productionRate, double influenceFactor)
            : this(productionRate, influenceFactor, null) { }

        /// <summary>
        /// Creates new attributes for a building
        /// </summary>
        /// <param name="productionRate">The production rate (how much it produces per turn)</param>
        /// <param name="influenceFactor">The territory influence factor of the building</param>
        /// <param name="baseBuildingAttributes">The base buildingAttributes</param>
        protected BuildingAttributes(uint productionRate, double influenceFactor, BuildingAttributes baseBuildingAttributes)
            : base(baseBuildingAttributes)
        {
            ProductionRate = productionRate;
            InfluenceFactor = influenceFactor;
        }

        /// <summary>
        /// Creates a new BuildingAttributes based on another one (deep copy)
        /// </summary>
        /// <param name="baseBuildingAttributes">The base buildingAttributes</param>
        protected BuildingAttributes(BuildingAttributes baseBuildingAttributes)
            : this(baseBuildingAttributes.ProductionRate, baseBuildingAttributes.InfluenceFactor, baseBuildingAttributes) { }

        #endregion

        #region ICloneable

        /// <summary>
        /// Clones the building attributes, return a new deep copy
        /// </summary>
        /// <returns>A deep copy of the building attributes</returns>
        public override Object Clone()
        {
            return new BuildingAttributes(this);
        }

        #endregion
    }
}
