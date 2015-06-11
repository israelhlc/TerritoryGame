using System.Collections.Generic;
using Common.AIInterface;
using Common.Commands;
using Common.General;
using Common.Resources;
using Common.Resources.Buildings;
using Common.Resources.Terrains;
using Common.Resources.Units;
using Common.Util;

namespace AIPlayerExample
{
    /// <summary>
    /// This is an example of a simple AI Player
    /// It implements the AIBasePlayer interface
    /// </summary>
    public class AIPlayer : AIBasePlayer
    {
        #region Properties

        /// <summary>
        /// The current turn validation code
        /// </summary>
        private long CurrentTurnValidationCode
        {
            get;
            set;
        }

        /// <summary>
        /// The last board received from the game
        /// </summary>
        private Board CurrentBoard
        {
            get;
            set;
        }

        #endregion

        #region Interface AIBasePlayer

        /// <summary>
        /// Initializes the AIPlayerExample player
        /// </summary>
        /// <param name="playerID">The ID of the player in the game - cannot be changed</param>
        /// <param name="callBack">The callback function that must be called to send the commands</param>
        public AIPlayer(int playerID, AIBasePlayer.PlayerCommandCallBack callBack)
            : base(playerID, callBack) { }

        /// <summary>
        /// This method is called by the game to invoke the commands of the players' turn.
        /// The player must send all the commands to the callback and then finish its turn.
        /// It is a simple example of AI, capable only of executing the basics of the game, no strategy:
        /// </summary>
        /// <param name="board">The game board as visible to the player</param>
        /// <param name="turnValidationCode">The validation code to be sent back to the game in with the commands</param>
        public override void PlayTurn(Board board, long turnValidationCode)
        {
            //stores the current turn validation code
            CurrentTurnValidationCode = turnValidationCode;

            //stores the current board
            CurrentBoard = board;

            //get the list of game elements from the player
            List<GameElement> myGameElements = CurrentBoard.GetGameElements(PlayerID);

            //iterates through my game elements to execute the actions
            foreach (GameElement gameElement in myGameElements)
            {
                //if it is a unit
                if (gameElement is Unit)
                {
                    //create building with the unit
                    PerformCreateBuilding(gameElement.ID);

                    //equip the unit
                    PerformEquipUnit(gameElement.ID);

                    //perform influence with the unit
                    PerformInfluenceAt(gameElement.ID);

                    //attack with the unit
                    PerformAttackAt(gameElement.ID);

                    //move the unit
                    PerformMoveTo(gameElement.ID);
                }
                //if it is a building
                else if (gameElement is Building)
                {
                    //gets the building as a Building
                    Building building = gameElement as Building;

                    //sets the next building
                    PerformSetProduction(building);
                }
            }

            //ends the turn
            SendCommand(PlayerCommand.EndOfTurn(this));
        }

        #endregion

        #region Methods

        #region Common

        /// <summary>
        /// Sends the command to the game
        /// </summary>
        /// <param name="command">The command to be sent</param>
        /// <returns>The response received from the game</returns>
        private PlayerCommandResponse SendCommand(PlayerCommand command)
        {
            //sends the command and gets the response
            PlayerCommandResponse commandResponse = CallBack(command, CurrentTurnValidationCode);

            //updates the current board
            if (commandResponse.ResultBoard != null)
                CurrentBoard = commandResponse.ResultBoard;

            //returns the command response
            return commandResponse;
        }

        #endregion

        #region Commands Logic

        /// <summary>
        /// Creates a building with the unit if it is a priest and is not in a building or adjacent to one
        /// </summary>
        /// <param name="unitID">The ID of the unit</param>
        private void PerformCreateBuilding(ulong unitID)
        {
            //gets the unit
            Unit unit = CurrentBoard.GetGameElement(unitID) as Unit;
            if (unit == null)
                return;

            //create a building only with a priest
            if (unit.Type == UnitType.Priest)
            {
                //gets its position and neighbours
                List<Position> positions = unit.Position.GetNeighbours();
                positions.Add(unit.Position);

                //iterates through the positions
                foreach (Position position in positions)
                {
                    //if there is a building, the unit will not create another one
                    if (CurrentBoard.GetTile(position).Building != null)
                        return;
                }

                //there is no building - create a new one (a city, just for example)
                SendCommand(PlayerCommand.CreateBuilding(this, unit, BuildingType.City));
            }
        }

        /// <summary>
        /// Equips the unit with all the items in the inventory if it is in a bulding's tile
        /// </summary>
        /// <param name="unitID">The ID of the unit</param>
        private void PerformEquipUnit(ulong unitID)
        {
            //gets the unit
            Unit unit = CurrentBoard.GetGameElement(unitID) as Unit;
            if (unit == null)
                return;

            //gets the unit's tile
            Tile tile = CurrentBoard.GetTile(unit.Position);

            //if there is a building and it has items, equip one of them on the unit
            if (tile.Building != null && tile.Building.UnitItemsInventory.Count > 0)
            {
                //send the equipUnit command
                SendCommand(PlayerCommand.EquipUnit(this, unit, tile.Building.UnitItemsInventory[0]));
            }
        }

        /// <summary>
        /// Performs the influence over territories with the unit
        /// </summary>
        /// <param name="unitID">The ID of the unit</param>
        private void PerformInfluenceAt(ulong unitID)
        {
            //gets the unit
            Unit unit = CurrentBoard.GetGameElement(unitID) as Unit;
            if (unit == null)
                return;

            //gets the unit's neighbours within range
            List<Position> neighbours = unit.Position.GetNeighbours(unit.Range);

            //the unit may influence a territory - the chances are proportional to its influence factor
            if (RandomUtil.NextDouble <= unit.InfluenceFactor)
            {
                //searches for a neighbour where it does not have 100% domain
                foreach (Position neighbour in neighbours)
                {
                    //verifies the tile's domain
                    double currentDomain;
                    CurrentBoard.GetTile(neighbour).Domain.TryGetValue(PlayerID, out currentDomain);

                    //if the domain is not full, influence it
                    if (currentDomain < Tile.NORMALIZED_DOMAIN_SUM)
                    {
                        //sends the command to influence at
                        SendCommand(PlayerCommand.InfluenceAt(this, unit, neighbour));

                        //updates unit's status
                        unit = CurrentBoard.GetGameElement(unitID) as Unit;
                    }

                    //if the unit has no movements left, stop influencing
                    if (unit.RemainingMovements == 0)
                        break;
                }
            }
        }

        /// <summary>
        /// Perform attacks with the unit
        /// </summary>
        /// <param name="unitID">The ID of the unit</param>
        private void PerformAttackAt(ulong unitID)
        {
            //gets the unit
            Unit unit = CurrentBoard.GetGameElement(unitID) as Unit;
            if (unit == null)
                return;

            //if the unit does not have remaining movements, return
            if (unit.RemainingMovements == 0)
                return;

            //gets the unit's neighbours within range
            List<Position> neighbours = unit.Position.GetNeighbours(unit.Range);

            //the response received from the game
            PlayerCommandResponse commandResponse = null;

            //may attack more than once in the turn
            do
            {
                //finds an enemy in a neighbour
                Position attackDestination = neighbours.Find(p => CurrentBoard.GetTile(p).Units.Exists(u => u.Owner != PlayerID));

                //if there is an attack available to a neighbour, attack it
                if (attackDestination != null)
                {
                    //send the attack command
                    commandResponse = SendCommand(PlayerCommand.AttackAt(this, unit, attackDestination));

                    //updates unit's status
                    unit = CurrentBoard.GetGameElement(unitID) as Unit;
                }
                //no attack is available - stop attacking
                else
                {
                    break;
                }
            } while (commandResponse != null && commandResponse.Result != PlayerCommandResult.NOK && unit != null && unit.RemainingMovements > 0);
        }

        /// <summary>
        /// Performs the movements with the unit
        /// </summary>
        /// <param name="unitID">The ID of the unit</param>
        private void PerformMoveTo(ulong unitID)
        {
            //gets the unit
            Unit unit = CurrentBoard.GetGameElement(unitID) as Unit;
            if (unit == null)
                return;

            //if the unit does not have remaining movements, return
            if (unit.RemainingMovements == 0)
                return;

            //the response received from the game
            PlayerCommandResponse commandResponse = null;

            //may move more than once in the turn
            do
            {
                //gets the unit's neighbours
                List<Position> neighbours = unit.Position.GetNeighbours();

                //removes neighbours with enemies
                neighbours.RemoveAll(p => CurrentBoard.GetTile(p).Units.Exists(u => u.Owner != PlayerID));

                //removes neighbours where it cannot move to
                neighbours.RemoveAll(p => !Terrains.Get(CurrentBoard.GetTile(p).Terrain).CanReceiveGroundUnits);

                //if no neighbour is available, don't move
                if (neighbours.Count == 0)
                    return;

                //if there is an enemy building in a neighbour, go to it
                Position destination = neighbours.Find(p => CurrentBoard.GetTile(p).Building != null && CurrentBoard.GetTile(p).Building.Owner != PlayerID);
                if (destination == null)
                    destination = neighbours[RandomUtil.NextInt % neighbours.Count];

                //moves randomly to a neighbour
                commandResponse = SendCommand(PlayerCommand.MoveTo(this, unit, destination));

                //updates unit's status
                unit = CurrentBoard.GetGameElement(unitID) as Unit;

            } while (commandResponse != null && commandResponse.Result != PlayerCommandResult.NOK && unit != null && unit.RemainingMovements > 0);
        }

        /// <summary>
        /// Sets the production of a building
        /// </summary>
        /// <param name="building">The building to set the production on</param>
        private void PerformSetProduction(Building building)
        {
            //gets the position
            Position position = building.Position;

            //gets the Tile
            Tile tile = CurrentBoard.GetTile(position);

            //if I have a building not producing, set its production
            if (tile.Building != null && tile.Building.Owner == PlayerID && tile.Building.CurrentProduction == null)
            {
                //sets the production on the building
                SendCommand(PlayerCommand.SetProduction(this, tile.Building, new ProduceableElement(UnitType.Lieutenant)));
            }
        }

        #endregion

        #endregion
    }
}
