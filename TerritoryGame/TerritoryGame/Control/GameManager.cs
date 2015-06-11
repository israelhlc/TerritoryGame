using System;
using System.Collections.Generic;
using Common.Resources;
using Common.Resources.Buildings;
using Common.Resources.Units;
using Common.Settings;
using TerritoryGame.Control.GamePlayers;
using TerritoryGame.Log;

namespace TerritoryGame.Control
{
    /// <summary>
    /// Class for general controls of the game, like turn ending
    /// </summary>
    internal static class GameManager
    {
        #region Properties

        /// <summary>
        /// Indicates when the game has ended
        /// </summary>
        public static bool GameOver
        {
            get;
            private set;
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the game manager
        /// </summary>
        public static void Initialize()
        {
            GameOver = false;
        }

        #endregion

        #region Turns

        /// <summary>
        /// Executes the end of turn rules and sets up the game for the next turn
        /// </summary>
        public static void NextTurn()
        {
            //iterates through the tiles to reset the attributes for the new turn
            for (int i = 0; i < BoardManager.Board.Width; i++)
            {
                for (int j = 0; j < BoardManager.Board.Height; j++)
                {
                    //gets the current position
                    Position currentPosition = new Position(i, j);

                    //gets the tile
                    Tile tile = BoardManager.Board.GetTile(currentPosition);

                    //process building turn end
                    ProcessBuildingTurnEnd(tile, currentPosition);

                    //reset units turn end
                    ProcessUnitsTurnEnd(tile, currentPosition);
                }
            }

            //normalizes currentBoard influence factor
            BoardManager.Board.NormalizeDomains();

            //sets the next turn in the game state
            GameState.Turn++;
        }

        /// <summary>
        /// Finishes the game
        /// </summary>
        /// <param name="ex">An exception which was the cause of the game over</param>
        public static void EndGame(Exception ex = null)
        {
            GameOver = true;
            GameLogger.Log(LogMessages.GameWonBy, new List<String>() { Players.CurrentPlayer.ID.ToString() });
            GameLogger.Log(LogMessages.GameOver);
        }

        #region Buildings

        /// <summary>
        /// Processess the tile's building end of turn
        /// </summary>
        /// <param name="tile">The tile with the building</param>
        /// <param name="position">The tile position</param>
        private static void ProcessBuildingTurnEnd(Tile tile, Position position)
        {
            //if there is no building in the tile or the building is not producing anything, do nothing
            if (tile.Building == null || tile.Building.CurrentProduction == null)
                return;

            //adds the production rate to the current production progres
            tile.Building.ProductionProgress += Buildings.Get(tile.Building.Type).ProductionRate;

            //if the production progress is less than the building cost, the production is not over so there is nothing more to do
            if (tile.Building.ProductionProgress < tile.Building.CurrentProduction.BuildingCost)
                return;

            //creates the newly finished ProduceableElement
            CreateFinishedProduction(tile, position);

            //resets the progres and current production
            tile.Building.ProductionProgress = 0;
            tile.Building.CurrentProduction = null;

            //spreads the influence factor
            List<Position> neighbours = position.GetNeighbours();
            tile.AddDomain(tile.Building.Owner, Buildings.Get(tile.Building.Type).InfluenceFactor, false);
            foreach (Position neighbour in neighbours)
            {
                //adds domain without normalizing - it will be done once after all the processing
                BoardManager.Board.GetTile(neighbour).AddDomain(tile.Building.Owner, Buildings.Get(tile.Building.Type).InfluenceFactor, false);
            }
        }

        /// <summary>
        /// Creates the newly finished ProduceableElement
        /// </summary>
        /// <param name="tile">The tile with the building</param>
        /// <param name="position">The tile position</param>
        private static void CreateFinishedProduction(Tile tile, Position position)
        {
            //the production is a unit
            if (tile.Building.CurrentProduction.ProduceableElementType is UnitType)
            {
                //creates a new unit
                UnitType unitType = (UnitType)tile.Building.CurrentProduction.ProduceableElementType;
                Unit newUnit = new Unit(tile.Building.Owner, unitType, position);
                tile.Units.Add(newUnit);
                GameElementsManager.Add(newUnit);
            }
            //the production is an item
            else if (tile.Building.CurrentProduction.ProduceableElementType is ItemType)
            {
                //creates a new unit item
                ItemType unitItemType = (ItemType)tile.Building.CurrentProduction.ProduceableElementType;
                tile.Building.UnitItemsInventory.Add(unitItemType);
            }

            //logs the finished production
            GameLogger.Log(LogMessages.BuildingProducedGameElement,
                new List<String>() { 
                    tile.Building.ID.ToString(), 
                    tile.Building.CurrentProduction.ToString() 
                });
        }

        #endregion

        #region Units

        /// <summary>
        /// Processes the tile's units end of turn
        /// </summary>
        /// <param name="tile">The tile being processed</param>
        /// <param name="position">The tile position</param>
        private static void ProcessUnitsTurnEnd(Tile tile, Position position)
        {
            //gets the position's neighbours
            List<Position> neighbours = position.GetNeighbours();

            //iterates through the units in the tile
            foreach (Unit unit in tile.Units)
            {
                //if the unit made no move - let it recover
                if (unit.RemainingMovements == unit.Movements && unit.CurrentHealth < unit.MaxHealth)
                {
                    //recovers the unit's health
                    unit.CurrentHealth += (unit.MaxHealth * GameSettings.HealthRecoverPerTurn);

                    //it's not possible to have more health than the MaxHealth
                    if (unit.CurrentHealth > unit.MaxHealth)
                        unit.CurrentHealth = unit.MaxHealth;
                }

                //spreads the influence factor
                tile.AddDomain(unit.Owner, unit.InfluenceFactor, false);
                foreach (Position neighbour in neighbours)
                {
                    //adds domain without normalizing - it will be done once after all the processing
                    BoardManager.Board.GetTile(neighbour).AddDomain(unit.Owner, unit.InfluenceFactor, false);
                }

                //resets the movements
                unit.RemainingMovements = unit.Movements;
            }
        }

        #endregion

        #endregion

        #region Players

        /// <summary>
        /// Eliminates the player from the game, removing all its elements and setting it as not alive anymore
        /// </summary>
        /// <param name="playerID">The ID of the player to be eliminated</param>
        public static void EliminatePlayerFromGame(int playerID)
        {
            //remove the player elements from the game
            RemovePlayerElements(playerID);

            //sets the player as dead on Players management
            Players.SetPlayerNotAlive(playerID);

            //log the player elimination
            GameLogger.Log(LogMessages.PlayerEliminated, new List<String>() { playerID.ToString() });
        }

        /// <summary>
        /// Removes everything related to the player
        /// </summary>
        public static void RemovePlayerElements(int playerID)
        {
            //iterates through the tiles of the currentBoard
            for (int x = 0; x < BoardManager.Board.Width; x++)
            {
                for (int y = 0; y < BoardManager.Board.Height; y++)
                {
                    //gets the tile
                    Tile tile = BoardManager.Board.GetTile(x, y);

                    //checks for units owned by the player
                    if (tile.Units.Exists(unit => unit.Owner == playerID))
                    {
                        //removes units from the GameElementsManager
                        foreach (Unit unit in tile.Units)
                        {
                            GameElementsManager.Remove(unit);
                        }

                        //resets the units list on the tile
                        tile.Units.Clear();
                    }

                    //remove the building if it belongs to the player
                    if (tile.Building != null && tile.Building.Owner == playerID)
                    {
                        //removes it from the GameElementsManager
                        GameElementsManager.Remove(tile.Building);

                        //removes the building from the tile
                        tile.Building = null;
                    }

                    //remove the domain
                    tile.RemoveDomain(playerID);
                }
            }
        }

        #endregion
    }
}
