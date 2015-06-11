using System.Collections.Generic;
using Common.General;
using TerritoryGame.Control.Exceptions;

namespace TerritoryGame.Control
{
    /// <summary>
    /// The GameElements Manager, on which every creation and removal must be persisted
    /// </summary>
    internal static class GameElementsManager
    {
        #region Members

        private static Dictionary<ulong, GameElement> _gameElements;

        #endregion

        #region Properties

        /// <summary>
        /// The GameElements stored in the manager
        /// </summary>
        private static Dictionary<ulong, GameElement> GameElements
        {
            get
            {
                //if the dictionary has not been initialized, do it
                if (_gameElements == null)
                    _gameElements = new Dictionary<ulong, GameElement>();

                return _gameElements;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a game element by its ID
        /// </summary>
        /// <param name="gameElementID">The Game Element ID</param>
        /// <returns>The Game Element</returns>
        /// <exception cref="GameElementNotFoundException">Thrown when the game element was not found</exception>
        internal static GameElement Get(ulong gameElementID)
        {
            //tries to get the game element
            GameElement gameElement;
            if (GameElements.TryGetValue(gameElementID, out gameElement))
                return gameElement;

            //the game element was not found
            throw new GameElementNotFoundException(gameElementID);
        }

        /// <summary>
        /// Gets a game element of the given type by its ID
        /// </summary>
        /// <typeparam name="T">The type of the game element</typeparam>
        /// <param name="gameElementID">The game element ID</param>
        /// <returns>The Game Element</returns>
        /// <exception cref="GameElementNotFoundException">Thrown when the game element was not found</exception>
        internal static T Get<T>(ulong gameElementID) where T : GameElement
        {
            GameElement gameElement = Get(gameElementID);
            if (gameElement is T)
                return gameElement as T;

            //the game element with the ID and type was not found
            throw new GameElementNotFoundException(gameElementID);
        }

        /// <summary>
        /// Adds the given game element to the manager
        /// </summary>
        /// <param name="gameElement">The game element</param>
        /// <exception cref="NullGameElementException">Thrown when null is given instead of a GameElement</exception>
        /// <exception cref="DuplicateGameElementException">Thrown when the given gameElement's ID is already defined in the manager</exception>
        internal static void Add(GameElement gameElement)
        {
            //validates if gameElement is null
            if (gameElement == null)
                throw new NullGameElementException();

            //checks if the ID is already in use
            if (GameElements.ContainsKey(gameElement.ID))
                throw new DuplicateGameElementException(gameElement.ID);

            //adds the gameElement
            GameElements.Add(gameElement.ID, gameElement);
        }

        /// <summary>
        /// Removes the Game Element from the manager
        /// </summary>
        /// <param name="gameElement">The game element</param>
        /// <returns>true iif the gameElement has been successfully removed</returns>
        internal static bool Remove(GameElement gameElement)
        {
            return Remove(gameElement.ID);
        }

        /// <summary>
        /// Removes the Game Element with the given ID from the manager
        /// </summary>
        /// <param name="gameElementID">The game element ID</param>
        /// <returns>true iif the gameElement has been successfully removed</returns>
        internal static bool Remove(ulong gameElementID)
        {
            return GameElements.Remove(gameElementID);
        }

        #endregion
    }
}
