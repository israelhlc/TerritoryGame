using System.Text;
using Common.General;
using Common.Resources.Units;

namespace Common.Resources
{
    /// <summary>
    /// Defines the game object which may be built in a building
    /// </summary>
    public class ProduceableElement : GameElement
    {
        #region Properties

        /// <summary>
        /// The type of the produceable element - it may be an UnitType or an ItemType
        /// </summary>
        public object ProduceableElementType
        {
            get;
            private set;
        }

        /// <summary>
        /// The building cost of the resource
        /// </summary>
        public uint BuildingCost
        {
            get
            {
                if (ProduceableElementType is UnitType)
                {
                    return Units.Units.Get((UnitType)ProduceableElementType).BuildingCost;
                }
                else //if (ProduceableElementType is ItemType)
                {
                    return UnitItems.Get((ItemType)ProduceableElementType).BuildingCost;
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new ProduceableElement for an unit
        /// </summary>
        /// <param name="unitType">The UnitType which will be used for the ProduceableElement</param>
        public ProduceableElement(UnitType unitType)
        {
            ProduceableElementType = unitType;
        }

        /// <summary>
        /// Creates a new ProduceableElement for an item
        /// </summary>
        /// <param name="unitItemType">The ItemType which will be used for the ProduceableElement</param>
        public ProduceableElement(ItemType unitItemType)
        {
            ProduceableElementType = unitItemType;
        }

        /// <summary>
        /// Creates a new ProduceableElement based on another one (clone)
        /// </summary>
        /// <param name="baseProduceableElement">The base produceable element</param>
        private ProduceableElement(ProduceableElement baseProduceableElement)
            : base(baseProduceableElement)
        {
            ProduceableElementType = baseProduceableElement.ProduceableElementType;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Implements the Equals Comparator for the ProduceableElement class
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>true iif the produceable elements are of the same type AND have the same value</returns>
        public override bool Equals(object obj)
        {
            //verifies if parameter is null
            if (obj == null)
            {
                return false;
            }

            //casts the object
            ProduceableElement produceableElement = obj as ProduceableElement;
            if (produceableElement == null)
            {
                return false;
            }

            //verifies if they have different types (i. e. one is for na unit and other is for an item)
            if (!ProduceableElementType.GetType().Equals(produceableElement))
                return false;

            //if they are UnitType
            if (ProduceableElementType is UnitType)
            {
                return ((UnitType)ProduceableElementType).Equals((UnitType)produceableElement.ProduceableElementType);
            }
            //if they are ItemType
            else //if (ProduceableElementType is ItemType)
            {
                return ((ItemType)ProduceableElementType).Equals((ItemType)produceableElement.ProduceableElementType);
            }
        }

        /// <summary>
        /// Implements the GetHashCode for the ProduceableElement class
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            //if they are UnitType
            if (ProduceableElementType is UnitType)
            {
                return (int)((UnitType)ProduceableElementType);
            }
            //if they are ItemType
            else //if (ProduceableElementType is ItemType)
            {
                return (int)((ItemType)ProduceableElementType) * 1000;
            }
        }

        /// <summary>
        /// Creates a string representation
        /// </summary>
        /// <returns>The string representation of the produceable element</returns>
        public override string ToString()
        {
            //the string to be returned
            StringBuilder stringRepresentation = new StringBuilder();

            //appends the enum identifier
            stringRepresentation.Append(ProduceableElementType.ToString());

            //returns the string created
            return stringRepresentation.ToString();
        }

        /// <summary>
        /// Clones the ProduceableElement, return a new deep copy
        /// </summary>
        /// <returns>A deep copy of the ProduceableElement</returns>
        public override object Clone()
        {
            return new ProduceableElement(this);
        }

        #endregion
    }
}
