using System;
using System.Collections.Generic;
using Common.General;
using Common.Resources;
using Common.Resources.Buildings;
using Common.Resources.Terrains;
using Common.Resources.Units;
using Common.Settings;
using Common.Util;
using TerritoryGame.Control.GamePlayers;

namespace TerritoryGame.Control
{
    /// <summary>
    /// Class which manages the currentBoard being used in the game.
    /// It provides methods to obtain the currentBoard viewed by any player.
    /// </summary>
    internal static class BoardManager
    {
        #region Properties

        /// <summary>
        /// The full game currentBoard, with everything
        /// </summary>
        internal static Board Board
        {
            get;
            private set;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the currentBoard manager (and the currentBoard)
        /// </summary>
        internal static void Initialize()
        {
            //creates a new empty and random currentBoard
            Board = new Board(
                GameSettings.NumberOfPlayers,
                GameSettings.BoardWidth,
                GameSettings.BoardHeight
            );

            //initialize currentBoard elements
            InitializeRandomBoardElements();
        }

        /// <summary>
        /// Initializes the random currentBoard elements - currently available only for up to 4 players
        /// </summary>
        private static void InitializeRandomBoardElements()
        {
            //gets the list of playersID
            List<int> playersIDs = Players.PlayersIDs;

            //checks if the game has more than 4 players
            if (playersIDs.Count > 4)
                throw new NotImplementedException("The board initialization has not been implemented for more than 4 players.");

            //sorts the list so the players are "shuffled"
            playersIDs.Sort();

            //gets the starting list
            List<Position> startingPositions = new List<Position>() { 
                new Position(0,0),
                new Position(0, Board.Height-1),
                new Position(Board.Width-1, Board.Height-1),
                new Position(Board.Width-1, 0)
            };

            //iterates the players IDs
            foreach (int playerID in playersIDs)
            {
                //gets a random starting position
                int positionIndex = RandomUtil.NextInt % startingPositions.Count;

                //adds the initial building and unit on the position
                Tile initialTile = Board.GetTile(startingPositions[positionIndex]);
                Building initialBuilding = new Building(playerID, BuildingType.City, startingPositions[positionIndex]);
                Unit initialUnit = new Unit(playerID, UnitType.Soldier, startingPositions[positionIndex]);
                initialTile.Building = initialBuilding;
                initialTile.Units.Add(initialUnit);

                //grants the initial tile is grass, so it will be possible to move to it
                initialTile.Terrain = TerrainType.Grass;

                //adds the initial resources to the game elements manager
                GameElementsManager.Add(initialUnit);
                GameElementsManager.Add(initialBuilding);

                //removes the position from the list
                startingPositions.RemoveAt(positionIndex);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the currentBoard as viewed by the player with the given ID
        /// </summary>
        /// <param name="playerID">The player ID</param>
        /// <returns>the currentBoard, as viewed by the given player</returns>
        internal static Board GetPlayerBoard(int playerID)
        {
            //gets the visible tiles for the player
            bool[,] visibleTiles = GetVisibleTiles(playerID);

            //clones the currentBoard
            Board playerBoard = (Board)Board.Clone();

            //iterate through the tiles
            for (int x = 0; x < playerBoard.Width; x++)
            {
                for (int y = 0; y < playerBoard.Height; y++)
                {
                    //if the tile is not visible, clear it so the player cannot see
                    if (!visibleTiles[x, y])
                    {
                        //gets the tile
                        Tile tile = playerBoard.GetTile(x, y);

                        //clears the tile
                        tile.Terrain = TerrainType.Unknown;
                        tile.Building = null;
                        tile.Domain.Clear();
                        tile.Units.Clear();
                    }
                }
            }

            //return the player currentBoard
            return playerBoard;
        }

        /// <summary>
        /// Get the visible tiles for the given player
        /// </summary>
        /// <param name="playerID">The player ID</param>
        /// <returns>The currentBoard visibility map, where true means the tile is visible for the player</returns>
        private static bool[,] GetVisibleTiles(int playerID)
        {
            //creates a boolean matrix to fill the visible tiles
            bool[,] visibleTiles = new bool[Board.Width, Board.Height];

            //gets the player's game elements
            List<GameElement> playerElements = Board.GetGameElements(playerID);

            //the neighbours list
            List<Position> neighbours = new List<Position>();

            //iterate through the player's elements to get the visible tiles
            foreach (GameElement gameElement in playerElements)
            {
                //clears the neighbours list
                neighbours.Clear();

                //if it is an unit, must get the visibility
                if (gameElement is Unit)
                {
                    //gets the unit
                    Unit unit = gameElement as Unit;

                    //gets the unit's neighbours in sight
                    neighbours = unit.Position.GetNeighbours(unit.Sight);
                    neighbours.Add(unit.Position);
                }
                //if it is a building
                else if (gameElement is Building)
                {
                    //gets the building
                    Building building = gameElement as Building;

                    //gets the neighbours for the building
                    neighbours = building.Position.GetNeighbours();
                    neighbours.Add(building.Position);
                }

                //sets all the list in neighbours as visible
                foreach (Position neighbour in neighbours)
                {
                    visibleTiles[neighbour.X, neighbour.Y] = true;
                }
            }

            //returns the visible tiles
            return visibleTiles;
        }

        #endregion
    }
}
