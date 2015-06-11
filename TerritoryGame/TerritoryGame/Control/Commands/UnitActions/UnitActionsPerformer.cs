using System;
using System.Collections.Generic;
using System.Linq;
using Common.Resources;
using Common.Resources.Buildings;
using Common.Resources.Terrains;
using Common.Resources.Units;
using Common.Settings;
using Common.Util;
using TerritoryGame.Control.Commands.Exceptions;
using TerritoryGame.Control.Commands.UnitActions.Exceptions;
using TerritoryGame.Log;

namespace TerritoryGame.Control.Commands.UnitActions
{
    /// <summary>
    /// Class used to execute the commands which involve units' action
    /// </summary>
    internal static class UnitActionsPerformer
    {
        #region Common

        /// <summary>
        /// Removes the unit from the game
        /// </summary>
        /// <param name="board">The game board</param>
        /// <param name="unit">The unit being removed</param>
        /// <param name="unitTile">The tile of the unit</param>
        private static void RemoveUnit(Board board, Unit unit, Tile unitTile)
        {
            //removes from the tile
            unitTile.Units.Remove(unit);

            //removes from the GameElementsManager
            GameElementsManager.Remove(unit);

            //logs the unit death
            GameLogger.Log(LogMessages.UnitDied, new List<String>() { unit.ID.ToString() });

            //checks if the player was eliminated
            if (!board.ExistsPlayer(unit.Owner))
            {
                //if the player was eliminated, removes it from the game
                GameManager.EliminatePlayerFromGame(unit.Owner);
            }
        }

        #endregion

        #region MoveTo

        /// <summary>
        /// Moves the unit to the desired destination.
        /// It also affect the tiles, removing the unit from the original tile 
        /// and adding it to the destination tile
        /// </summary>
        /// <param name="board">The board on which the unit will be moved</param>
        /// <param name="unit">The unit to perform the movement</param>
        /// <param name="destination">Destination of the unit</param>
        internal static void MoveTo(Board board, Unit unit, Position destination)
        {
            //validates the movement
            uint movementsCost = ValidateMoveTo(board, unit, destination);

            //keeps the start and end so it does not lose reference
            Tile startTile = board.GetTile(unit.Position);
            Tile endTile = board.GetTile(destination);

            //changes the unit's position and remaining movements
            unit.Position = (Position)destination.Clone();
            unit.RemainingMovements = unit.RemainingMovements > movementsCost ? unit.RemainingMovements - movementsCost : 0;

            //removes the unit from the start tile
            startTile.Units.Remove(unit);

            //adds the unit to the end tile units' list
            endTile.Units.Add(unit);

            //gets the building in the destination tile
            Building buildingOnTile = endTile.Building;

            //if there is a building on the endTile belonging to an enemy, remove it
            if (buildingOnTile != null && buildingOnTile.Owner != unit.Owner)
            {
                //removes the building on the tile
                GameElementsManager.Remove(buildingOnTile);
                endTile.Building = null;

                //logs the building destruction
                GameLogger.Log(LogMessages.UnitDestroyedBuilding,
                    new List<String>() { 
                    unit.ID.ToString(),
                    buildingOnTile.ID.ToString(),
                    buildingOnTile.Position.ToString()
                });

                //checks if the player was eliminated
                if (!board.ExistsPlayer(buildingOnTile.Owner))
                {
                    //if the player was eliminated, removes it from the game
                    GameManager.EliminatePlayerFromGame(buildingOnTile.Owner);
                }
            }
        }

        /// <summary>
        /// Validates the unit movement
        /// </summary>
        /// <param name="board">The board on which the unit will be moved</param>
        /// <param name="unit">The unit to perform the movement</param>
        /// <param name="destination">Destination of the unit</param>
        /// <returns>The cost of the movement</returns>
        private static uint ValidateMoveTo(Board board, Unit unit, Position destination)
        {
            //if destination is the current position, thrown an impossible route exception
            if (destination.Equals(unit.Position))
                throw new ImpossibleGotoException(board, unit.Position, destination);

            //if there is no more movements remaining in the turn, don't allow the movement
            if (unit.RemainingMovements <= 0)
                throw new NoRemainingMovementsException((Unit)unit.Clone());

            //calculates the best route
            List<Position> route = MovementUtil.CalculateBestRouteTo(board, unit.Position, destination, unit.Owner);

            //calculates the movement cost
            uint movementsCost = MovementUtil.CalculateRouteCost(board, route);

            //if trying to move more than one tile and the unit does not have enough movements in the turn, throw an exception
            if (route.Count > 2 && unit.RemainingMovements < movementsCost)
                throw new UnreacheableDestinationException(movementsCost, unit.RemainingMovements);

            //returns the cost of the movement
            return movementsCost;
        }

        #endregion

        #region AttackAt

        /// <summary>
        /// Attacks the given target using the given unit
        /// </summary>
        /// <param name="board">The board on which the unit will attack</param>
        /// <param name="unit">The unit to perform the attack</param>
        /// <param name="target">Target position of the unit</param>
        internal static void AttackAt(Board board, Unit unit, Position target)
        {
            //gets the current and destination tiles
            Tile currentTile = board.GetTile(unit.Position);
            Tile targetTile = board.GetTile(target); ;

            //validates the attack
            ValidateAttackAt(unit, target, targetTile);

            //gets the attack of the unit
            double attack = unit.Attack;
            if (currentTile.Units.Count > GameSettings.GroupingUnitsThreshold)
                attack *= GameSettings.GroupingUnitsBonus;
            double attackerDomain;
            currentTile.Domain.TryGetValue(unit.Owner, out attackerDomain);
            attack += attack * attackerDomain;

            //gets the unit with the best enemyDefense on the destination tile
            Unit attackedUnit = targetTile.Units.OrderByDescending(u => u.Defense).First();

            //gets the enemyDefense of the enemy unit
            double enemyDefense = attackedUnit.Defense;
            if (targetTile.Units.Count > GameSettings.GroupingUnitsThreshold)
                enemyDefense *= GameSettings.GroupingUnitsBonus;
            double defensorDomain;
            targetTile.Domain.TryGetValue(attackedUnit.Owner, out defensorDomain);
            enemyDefense += enemyDefense * defensorDomain;

            //gets the real attack and enemyDefense
            double realAttack = attack * RandomUtil.NextNormalizedDouble(unit.MinimumRandomFactor);
            double realEnemyDefense = enemyDefense * RandomUtil.NextNormalizedDouble(attackedUnit.MinimumRandomFactor);

            //gets the damages inflicted
            double attackerDamage = (realAttack / (realAttack + realEnemyDefense)) * attack;
            double defensorDamage = (realEnemyDefense / (realAttack + realEnemyDefense)) / enemyDefense;

            //reduces de remaining movements of the attacking unit
            uint movementCost = Terrains.Get(targetTile.Terrain).MovementCost;
            unit.RemainingMovements = unit.RemainingMovements > movementCost ? unit.RemainingMovements - movementCost : 0;

            //process the damage inflicted on each unit
            ProcessAttackDamage(board, unit, currentTile, defensorDamage);
            ProcessAttackDamage(board, attackedUnit, targetTile, attackerDamage);

            //logs the attack damage
            GameLogger.Log(LogMessages.UnitAttackedAndDamaged,
                new List<String>() { 
                    unit.ID.ToString(),
                    attackedUnit.ID.ToString(),
                    target.ToString(),
                    attackerDamage.ToString(),
                    defensorDamage.ToString()
                });
        }

        /// <summary>
        /// Processes the attack damage in the damaged unit.
        /// If the unit is killed, it will be removed from the game currentBoard and GameElementsManager
        /// </summary>
        /// <param name="board">The board on which the unit will attack</param>
        /// <param name="damagedUnit">The damaged unit</param>
        /// <param name="damagedUnitTile">The tile of the damaged unit</param>
        /// <param name="damage">The damage suffered by the unit</param>
        private static void ProcessAttackDamage(Board board, Unit damagedUnit, Tile damagedUnitTile, double damage)
        {
            //if the damage is negative or zero, the damage is not considered
            if (damage <= 0)
                return;

            //damages the unit
            damagedUnit.CurrentHealth -= damage;

            //removes the unit if it died, checking also if the player was eliminated
            if (damagedUnit.CurrentHealth <= 0)
                RemoveUnit(board, damagedUnit, damagedUnitTile);
        }

        /// <summary>
        /// Validates the unit attack
        /// </summary>
        /// <param name="unit">The unit which is attacking</param>
        /// <param name="target">The target of the attack</param>
        /// <param name="targetTile">The target tile</param>
        private static void ValidateAttackAt(Unit unit, Position target, Tile targetTile)
        {
            //if there is no more movements remaining in the turn, don't allow the movement
            if (unit.RemainingMovements <= 0)
                throw new NoRemainingMovementsException((Unit)unit.Clone());

            //gets the requiredRange distance to the destination
            uint requiredRange = MovementUtil.CalculateManhattanDistance(unit.Position, target);

            //check if the destination is not in the range of the unit
            if (requiredRange > unit.Range)
                throw new UnreacheableDestinationException(requiredRange);

            //gets the destination tile
            if (targetTile.Units.Exists(u => u.Owner == unit.Owner))
                throw new ImpossibleAttackException();
        }

        #endregion

        #region InfluenceAt

        /// <summary>
        /// Influences the tile in the target using the given unit
        /// </summary>
        /// <param name="board">The board on which the unit will perform the influence</param>
        /// <param name="unit">The unit to perform the influence</param>
        /// <param name="target">Target position of the unit</param>
        internal static void InfluenceAt(Board board, Unit unit, Position target)
        {
            //gets the current and destination tiles
            Tile currentTile = board.GetTile(unit.Position);
            Tile targetTile = board.GetTile(target);

            //validates the influence command
            ValidateInfluenceAt(unit, target);

            //gets the influence factor of the unit
            double influence = unit.InfluenceFactor;
            if (currentTile.Units.Count > GameSettings.GroupingUnitsThreshold)
                influence *= GameSettings.GroupingUnitsBonus;
            double currentTileDomain;
            currentTile.Domain.TryGetValue(unit.Owner, out currentTileDomain);
            influence += influence * currentTileDomain;

            //gets the real influence that will be added to the tile
            double realInfluence = influence * RandomUtil.NextNormalizedDouble(unit.MinimumRandomFactor);

            //reduces de remaining movements of the influencing unit
            uint movementCost = Terrains.Get(targetTile.Terrain).MovementCost;
            unit.RemainingMovements = unit.RemainingMovements > movementCost ? unit.RemainingMovements - movementCost : 0;

            //adds the domain to the tile
            targetTile.AddDomain(unit.Owner, influence);

            //logs the influence factor
            GameLogger.Log(LogMessages.UnitInfluencedTerritory,
                new List<String>() { 
                    unit.ID.ToString(),
                    target.ToString(),
                    influence.ToString()
                });
        }

        /// <summary>
        /// Validates the unit territory influence
        /// </summary>
        /// <param name="unit">The unit which is performing the influence</param>
        /// <param name="target">The target of the influence</param>
        private static void ValidateInfluenceAt(Unit unit, Position target)
        {
            //if there is no more movements remaining in the turn, don't allow the movement
            if (unit.RemainingMovements <= 0)
                throw new NoRemainingMovementsException((Unit)unit.Clone());

            //gets the requiredRange distance to the destination
            uint requiredRange = MovementUtil.CalculateManhattanDistance(unit.Position, target);

            //check if the destination is not in the range of the unit
            if (requiredRange > unit.Range)
                throw new UnreacheableDestinationException(requiredRange);
        }

        #endregion

        #region CreateBuilding

        /// <summary>
        /// Creates a building in the tile of the given unit, consuming it at the the end of the command
        /// </summary>
        /// <param name="board">The board on which the unit will create the building</param>
        /// <param name="unit">The unit to create the building</param>
        /// <param name="buildingType">The type of the building being created</param>
        internal static void CreateBuilding(Board board, Unit unit, BuildingType buildingType)
        {
            //gets the unit's tile
            Tile unitTile = board.GetTile(unit.Position);

            //creates the building
            Building newBuilding = new Building(unit.Owner, buildingType, unit.Position);
            GameElementsManager.Add(newBuilding);

            //adds the building to the tile
            unitTile.Building = newBuilding;

            //removes the unit from the game
            RemoveUnit(board, unit, unitTile);

            //don't need to log the building creation - already logged on CommandProcessor
        }

        #endregion
    }
}
