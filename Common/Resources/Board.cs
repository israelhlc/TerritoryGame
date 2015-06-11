using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common.General;
using Common.Resources.Exceptions;

[assembly: InternalsVisibleTo("TerritoryGame")]

namespace Common.Resources
{
    /// <summary>
    /// The board (map) of the game, which is basically a bidimensional array of Tile
    /// </summary>
    public class Board : GameElement
    {
        #region Constants

        /// <summary>
        /// The width dimension
        /// </summary>
        private const int WIDTH_DIMENSION = 0;

        /// <summary>
        /// The height dimension
        /// </summary>
        private const int HEIGHT_DIMENSION = 1;

        #endregion

        #region Properties

        /// <summary>
        /// The array of tiles in the Board
        /// </summary>
        protected Tile[,] Tiles
        {
            get;
            set;
        }

        /// <summary>
        /// The board width (X dimension)
        /// </summary>
        public int Width
        {
            get
            {
                return Tiles.GetLength(WIDTH_DIMENSION);
            }
        }

        /// <summary>
        /// The board height (Y dimension)
        /// </summary>
        public int Height
        {
            get
            {
                return Tiles.GetLength(HEIGHT_DIMENSION);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an empty squared board.
        /// All the tiles will be of the same type,
        /// as specified in Tile constructor
        /// </summary>
        /// <param name="numberOfPlayers">The number of players in the game</param>
        /// <param name="size">Size of the board</param>
        public Board(int numberOfPlayers, int size) : this(numberOfPlayers, size, size) { }

        /// <summary>
        /// Creates an empty board with the specified width and height.
        /// All the tiles will be of the same type,
        /// as specified in Tile constructor
        /// </summary>
        /// <param name="numberOfPlayers">The number of players in the game</param>
        /// <param name="width">Board width</param>
        /// <param name="height">Board height</param>
        public Board(int numberOfPlayers, int width, int height)
        {
            //initialize tiles array
            Tiles = new Tile[width, height];

            //initialize every tile
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Tiles[i, j] = new Tile(numberOfPlayers);
                }
            }
        }

        /// <summary>
        /// Creates a new board based on another one (clone)
        /// </summary>
        /// <param name="baseBoard">The base board</param>
        protected Board(Board baseBoard)
            : base(baseBoard)
        {
            //initialize tiles array
            Tiles = new Tile[baseBoard.Width, baseBoard.Height];

            //clones every tile
            for (int i = 0; i < baseBoard.Width; i++)
            {
                for (int j = 0; j < baseBoard.Height; j++)
                {
                    Tiles[i, j] = (Tile)baseBoard.GetTile(i, j).Clone();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the tile at the specified coordinates
        /// </summary>
        /// <param name="position">The position of the desired tile</param>
        /// <returns>The tile at the given position</returns>
        public Tile GetTile(Position position)
        {
            return GetTile(position.X, position.Y);
        }

        /// <summary>
        /// Gets the tile at the specified coordinates
        /// </summary>
        /// <param name="xCoordinate">The X coordinate position</param>
        /// <param name="yCoordinate">The Y coordinate position</param>
        /// <returns>The tile at the given coordinates</returns>
        public Tile GetTile(int xCoordinate, int yCoordinate)
        {
            try
            {
                return Tiles[xCoordinate, yCoordinate];
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidBoardPositionException(new Position(xCoordinate, yCoordinate));
            }
        }

        /// <summary>
        /// Normalizes the tile's domains
        /// </summary>
        public void NormalizeDomains()
        {
            //iterates through the tiles to reset the attributes for the new turn
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    //normalize tile's domains
                    Tiles[i, j].NormalizeDomains();
                }
            }
        }

        /// <summary>
        /// Checks if the given player has units or building in the board
        /// </summary>
        /// <param name="playerID">The player ID</param>
        /// <returns>true iif the player has units or building in the board</returns>
        public bool ExistsPlayer(int playerID)
        {
            //iterates through the tiles to call ExistsPlayer
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    //checks if the player is in the tile
                    if (Tiles[i, j].ExistsPlayer(playerID))
                        return true;
                }
            }

            //if the player was not found on any tile, it is not in the board
            return false;
        }

        /// <summary>
        /// Gets the game element with the given ID in the board
        /// </summary>
        /// <param name="gameElementID">The game element ID</param>
        /// <returns>The element from the board with the given ID, null otherwise</returns>
        public GameElement GetGameElement(ulong gameElementID)
        {
            //iterates through the tiles
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    //searches within the units
                    GameElement gameElement = Tiles[i, j].Units.Find(unit => unit.ID == gameElementID);
                    if (gameElement != null)
                        return gameElement;

                    //compares the building
                    if (Tiles[i, j].Building != null && Tiles[i, j].Building.ID == gameElementID)
                        return Tiles[i, j].Building;
                }
            }

            //the game element was not found - return null
            return null;
        }

        /// <summary>
        /// Gets the game elements (units and buildings) of a given player
        /// </summary>
        /// <param name="playerID">The player ID</param>
        /// <returns>The list containing all player's elements in the board</returns>
        public List<GameElement> GetGameElements(int playerID)
        {
            //creates the list of player elements
            List<GameElement> playerElements = new List<GameElement>();

            //iterates through the tiles
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    //adds all the game elements of the tile
                    playerElements.AddRange(Tiles[i, j].GetGameElements(playerID));
                }
            }

            //return the list with all the elements
            return playerElements;
        }

        #region ICloneable

        /// <summary>
        /// Clones the board, return a new deep copy
        /// </summary>
        /// <returns>A deep copy of the board</returns>
        public override Object Clone()
        {
            return new Board(this);
        }

        #endregion

        #endregion
    }
}
