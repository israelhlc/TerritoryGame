using System;

namespace Common.General
{
    /// <summary>
    /// Abstract class for all the game components, i. e. every instantiable component during the game
    /// </summary>
    public abstract class GameElement : ICloneable
    {
        #region Properties

        /// <summary>
        /// The unique ID for the component
        /// </summary>
        public ulong ID
        {
            get;
            private set;
        }

        /// <summary>
        /// The last used ID, for the elements ID increment
        /// </summary>
        private static ulong LastUsedID
        {
            get;
            set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructs a new game component generating its unique ID
        /// </summary>
        protected GameElement(GameElement gameElement = null)
        {
            if (gameElement == null || (!gameElement.GetType().Equals(this.GetType())))
            {
                LastUsedID++;
                ID = LastUsedID;
            }
            else
            {
                ID = gameElement.ID;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Implements the Equals Comparator for the GameComponent class
        /// </summary>
        /// <param name="obj">Object to compare</param>
        /// <returns>true iif the ID is the same</returns>
        public override bool Equals(object obj)
        {
            //verifies if parameter is null
            if (obj == null)
            {
                return false;
            }

            //casts the object
            GameElement gameElement = obj as GameElement;
            if (gameElement == null)
            {
                return false;
            }

            //returns the comparator between the keys
            return ID.Equals(gameElement.ID);
        }

        /// <summary>
        /// Implements the GetHashCode method
        /// </summary>
        /// <returns>The hash code for the object, which will be its unique ID</returns>
        public override int GetHashCode()
        {
            return (int)ID;
        }

        #endregion

        #region ICloneable

        /// <summary>
        /// Clones a GameElement. 
        /// All overrides should call base(GameElement) in the constructor so it always copies the ID instead of generating a new one
        /// </summary>
        /// <returns>The cloned (deep copy) game element</returns>
        public abstract Object Clone();

        #endregion
    }
}
