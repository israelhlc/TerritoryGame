using System;
using System.Collections.Generic;
using Common.AIInterface;
using Common.Commands.Exceptions;
using Common.Resources;
using Common.Resources.Buildings;
using Common.Resources.Units;

namespace Common.Commands
{
    #region Enums

    /// <summary>
    /// The commands available in the game
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// The command for ending the current player's turn
        /// </summary>
        EndOfTurn,
        /// <summary>
        /// The command for moving an unit somewhere
        /// </summary>
        MoveTo,
        /// <summary>
        /// The command for attacking with an unit
        /// </summary>
        Attack,
        /// <summary>
        /// The command for influencing a territory with an unit
        /// </summary>
        InfluenceAt,
        /// <summary>
        /// The command for equipping an unit with an item
        /// </summary>
        EquipUnit,
        /// <summary>
        /// The command for setting the production on a building
        /// </summary>
        SetProduction,
        /// <summary>
        /// The command for creating a new building
        /// </summary>
        CreateBuilding
    }

    #endregion

    /// <summary>
    /// The command a player (AI or human) will send to the game
    /// It can be anything a player can do in a turn, buth will
    /// be validated by the games rules before being processed
    /// </summary>
    public sealed class PlayerCommand
    {
        #region Properties

        /// <summary>
        /// The command type
        /// </summary>
        public CommandType Command
        {
            get;
            private set;
        }

        /// <summary>
        /// The unit which the command referes to
        /// </summary>
        public Unit Unit
        {
            get;
            private set;
        }

        /// <summary>
        /// The destination (Position) of the command
        /// </summary>
        public Position Destination
        {
            get;
            private set;
        }

        /// <summary>
        /// The ID of the player who is sending the command
        /// </summary>
        public int PlayerID
        {
            get;
            private set;
        }

        /// <summary>
        /// The item to be equipped in the unit
        /// </summary>
        public ItemType? UnitItem
        {
            get;
            private set;
        }

        /// <summary>
        /// The building to set the production on
        /// </summary>
        public Building Building
        {
            get;
            private set;
        }

        /// <summary>
        /// The production to be set on the building
        /// </summary>
        public ProduceableElement Production
        {
            get;
            private set;
        }

        /// <summary>
        /// The list of invalid resources which will be returned in the exception
        /// </summary>
        private List<Object> InvalidResources
        {
            get;
            set;
        }

        /// <summary>
        /// The type of the building being created
        /// </summary>
        public BuildingType? BuildingCreating
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new PlayerCommand for the following Commands:
        /// End of Turn
        /// </summary>
        /// <param name="player">The player creating the command</param>
        /// <param name="command">The type of the command</param>
        private PlayerCommand(AIBasePlayer player, CommandType command) :
            this(player, command, null, null, null, null, null, null) { }

        /// <summary>
        /// Creates a new PlayerCommand for the following Commands:
        /// End of Turn / Move / AttackAt / InfluenceAt / CreateBuilding
        /// </summary>
        /// <param name="player">The player creating the command</param>
        /// <param name="command">The type of the command</param>
        /// <param name="unit">The unit which will execute the command</param>
        /// <param name="position">The target position for the command</param>
        private PlayerCommand(AIBasePlayer player, CommandType command, Unit unit, Position position) :
            this(player, command, unit, position, null, null, null, null) { }

        /// <summary>
        /// Creates a new PlayerCommand for the following Commands:
        /// EndOfTurn, EquipUnit
        /// </summary>
        /// <param name="player">The player creating the command</param>
        /// <param name="command">The type of the command</param>
        /// <param name="unit">The unit which will execute the command</param>
        /// <param name="itemType">The item to be equipped</param>
        private PlayerCommand(AIBasePlayer player, CommandType command, Unit unit, ItemType itemType) :
            this(player, command, unit, null, itemType, null, null, null) { }

        /// <summary>
        /// Creates a new PlayerCommand for the following commands:
        /// CreateBuilding
        /// </summary>
        /// <param name="player">The player creating the command</param>
        /// <param name="command">The type of the command</param>
        /// <param name="unit">The unit which will execute the command</param>
        /// <param name="buildingType">The type of the building being created</param>
        private PlayerCommand(AIBasePlayer player, CommandType command, Unit unit, BuildingType buildingType) :
            this(player, command, unit, null, null, null, null, buildingType) { }

        /// <summary>
        /// Creates a new PlayerCommand for the following Commands:
        /// EndOfTurn / SetProduction
        /// </summary>
        /// <param name="player">The player creating the command</param>
        /// <param name="command">The type of the command</param>
        /// <param name="building">The building to set the production on</param>
        /// <param name="production">The production to be set on the building</param>
        private PlayerCommand(AIBasePlayer player, CommandType command, Building building, ProduceableElement production) :
            this(player, command, null, null, null, building, production, null) { }

        /// <summary>
        /// Creates a new PlayerCommand for the following Commands:
        /// End of Turn / Move / AttackAt / InfluenceAt / EquipUnit / SetProduction / CreateBuilding
        /// </summary>
        /// <param name="player">The player creating the command</param>
        /// <param name="command">The type of the command</param>
        /// <param name="unit">The unit which will execute the command</param>
        /// <param name="position">The target position for the command</param>
        /// <param name="itemType">The item to be equipped</param>
        /// <param name="building">The building to set the production on</param>
        /// <param name="production">The production to be set on the building</param>
        /// <param name="buildingType">The type of the building being created</param>
        private PlayerCommand(AIBasePlayer player, CommandType command, Unit unit, Position position, ItemType? itemType, Building building, ProduceableElement production, BuildingType? buildingType)
        {
            //sets the attributes
            PlayerID = player.PlayerID;
            Command = command;
            Unit = unit;
            Destination = position;
            UnitItem = itemType;
            Building = building;
            Production = production;
            BuildingCreating = buildingType;

            //intializes the validation list
            InvalidResources = new List<Object>();

            //validates the command - it will raise an exception if it is invalid
            switch (command)
            {
                case CommandType.EndOfTurn:
                    ValidateEndOfTurnCommand();
                    break;
                case CommandType.Attack:
                    ValidateAttackCommand();
                    break;
                case CommandType.MoveTo:
                    ValidateMoveToCommand();
                    break;
                case CommandType.InfluenceAt:
                    ValidateInfluenceAtCommand();
                    break;
                case CommandType.EquipUnit:
                    ValidateEquipUnitCommand();
                    break;
                case CommandType.SetProduction:
                    ValidateSetProductionCommand();
                    break;
                case CommandType.CreateBuilding:
                    ValidateCreateBuildingCommand();
                    break;
                default:
                    //No suitable command type called - adds the command to the validation List
                    InvalidResources.Add(Command);
                    break;
            }

            //if the validation list is not empty, throw the exception
            if (InvalidResources.Count > 0)
                throw new InvalidPlayerCommandException(InvalidResources);
        }

        #endregion

        #region Validation Methods

        #region Commands

        /// <summary>
        /// Validates the CommandType EndOfTurn
        /// It must be have the EndOfTurn CommandType
        /// </summary>
        private void ValidateEndOfTurnCommand()
        {
            //validates the CommandType
            ValidateCommandType(CommandType.EndOfTurn);
        }

        /// <summary>
        /// Validates the AttackAt with the following attributes:
        /// CommandType / Unit / Destination
        /// </summary>
        private void ValidateAttackCommand()
        {
            //validates the CommandType
            ValidateCommandType(CommandType.Attack);

            //validates the Unit
            ValidateUnit();

            //validates the Destination
            ValidateDestination();
        }

        /// <summary>
        /// Validates the Move with the following attributes:
        /// CommandType / Unit / Destination
        /// </summary>
        private void ValidateMoveToCommand()
        {
            //validates the CommandType
            ValidateCommandType(CommandType.MoveTo);

            //validates the Unit
            ValidateUnit();

            //validates the Destination
            ValidateDestination();
        }

        /// <summary>
        /// Validates the InfluenceAt with the following attributes:
        /// CommandType / Unit / Destination
        /// </summary>
        private void ValidateInfluenceAtCommand()
        {
            //validates the CommandType
            ValidateCommandType(CommandType.InfluenceAt);

            //validates the Unit
            ValidateUnit();

            //validates the Destination
            ValidateDestination();
        }

        /// <summary>
        /// Validates the EquipUnit with the following attributes:
        /// CommandType / Unit / UnitItem
        /// </summary>
        private void ValidateEquipUnitCommand()
        {
            //validates the CommandType
            ValidateCommandType(CommandType.EquipUnit);

            //validates the Unit
            ValidateUnit();

            //validates the Item
            ValidateUnitItem();
        }

        /// <summary>
        /// Validates the SetProduction with the following attributes:
        /// CommandType / Building / Production
        /// </summary>
        private void ValidateSetProductionCommand()
        {
            //validates the CommandType
            ValidateCommandType(CommandType.SetProduction);

            //validates the building
            ValidateBuilding();

            //validates the production
            ValidateProduction();
        }

        /// <summary>
        /// Validates the CreateBuilding with the following attributes:
        /// CommandType / Unit
        /// </summary>
        private void ValidateCreateBuildingCommand()
        {
            //validates the CommandType
            ValidateCommandType(CommandType.CreateBuilding);

            //validates the Unit
            ValidateUnit();

            //validates unit type
            ValidateUnitType(UnitType.Priest);
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Validates if the command type is the same as the given one
        /// </summary>
        /// <param name="commandType">The Command Type wanted</param>
        private void ValidateCommandType(CommandType commandType)
        {
            //checks if the CommandType is the same of the parameter
            if (Command != commandType)
                InvalidResources.Add(Command);
        }

        /// <summary>
        /// Validates if the unit is valid and belongs to the owner of the command
        /// </summary>
        private void ValidateUnit()
        {
            //checks if the Unit is null
            if (Unit == null)
            {
                //Adds the unit full name to return in the exception
                InvalidResources.Add(typeof(Unit).FullName);
            }
            //checks if the Unit does not belong to the
            else if (Unit.Owner != PlayerID)
            {
                //adds the invalid unit to the exception return
                InvalidResources.Add(Unit);
            }
        }

        /// <summary>
        /// Validates if the destination is valid, i. e. is not null and 
        /// with its coordinates greater than or equal to 0
        /// </summary>
        private void ValidateDestination()
        {
            //checks if the Destination is null
            if (Destination == null)
            {
                //Adds the position full name to return in the exception
                InvalidResources.Add(typeof(Position).FullName);
            }
            else if (Destination.X < 0 || Destination.Y < 0)
            {
                //the destination is not null but one or both of its coordinates are invalid
                InvalidResources.Add(Destination);
            }
        }

        /// <summary>
        /// Validates if the unit item is valid, i. e. is not null and
        /// the unit doesn't have it already
        /// </summary>
        private void ValidateUnitItem()
        {
            //checks if the ItemType is null
            if (UnitItem == null)
            {
                //Adds the itemtype full name to return in the exception
                InvalidResources.Add(typeof(ItemType).FullName);
            }
            //checks if the Unit already has this itemtype
            else if (Unit != null && Unit.Items.Contains((ItemType)UnitItem))
            {
                //adds the invalid item type to the exception return
                InvalidResources.Add(UnitItem);
            }
        }

        /// <summary>
        /// Validates if the building is valid and belongs to the owner of the command
        /// </summary>
        private void ValidateBuilding()
        {
            //checks if the Building is null
            if (Building == null)
            {
                //Adds the building full name to return in the exception
                InvalidResources.Add(typeof(Building).FullName);
            }
            //checks if the Building does not belong to the
            else if (Building.Owner != PlayerID)
            {
                //adds the invalid building to the exception return
                InvalidResources.Add(Building);
            }
        }

        /// <summary>
        /// Validates if the production is valid
        /// </summary>
        private void ValidateProduction()
        {
            //checks if the Production is null
            if (Production == null)
            {
                //Adds the ProduceableElement full name to return in the exception
                InvalidResources.Add(typeof(ProduceableElement).FullName);
            }
        }

        /// <summary>
        /// Validates if the unit has the given type
        /// </summary>
        private void ValidateUnitType(UnitType unitType)
        {
            //checks if the Unit has the given type
            if (Unit != null && Unit.Type != unitType)
            {
                //adds the invalid unit to the exception return
                InvalidResources.Add(Unit);
            }
        }

        #endregion

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Creates a new EndOfTurn Command
        /// </summary>
        /// <param name="player">The player sending the command</param>
        /// <returns>A new EndOfTurn PlayerCommand</returns>
        /// <exception cref="InvalidPlayerCommandException">Thrown when the Command has an invalid combination of parameters</exception>
        public static PlayerCommand EndOfTurn(AIBasePlayer player)
        {
            return new PlayerCommand(player, CommandType.EndOfTurn);
        }

        /// <summary>
        /// Creates a new MoveTo Command
        /// </summary>
        /// <param name="player">The player sending the command</param>
        /// <param name="unit">The Unit which will make the movement</param>
        /// <param name="destination">The destination of the movement</param>
        /// <returns>A new MoveTo PlayerCommand with the given parameters</returns>
        /// <exception cref="InvalidPlayerCommandException">Thrown when the Command has an invalid combination of parameters</exception>
        public static PlayerCommand MoveTo(AIBasePlayer player, Unit unit, Position destination)
        {
            return new PlayerCommand(player, CommandType.MoveTo, unit, destination);
        }

        /// <summary>
        /// Creates a new AttackAt Command
        /// </summary>
        /// <param name="player">The player sending the command</param>
        /// <param name="unit">The Unit which will make the attack</param>
        /// <param name="destination">The destination (tile position) of the attack</param>
        /// <returns>A new AttackAt PlayerCommand with the given parameters</returns>
        /// <exception cref="InvalidPlayerCommandException">Thrown when the Command has an invalid combination of parameters</exception>
        public static PlayerCommand AttackAt(AIBasePlayer player, Unit unit, Position destination)
        {
            return new PlayerCommand(player, CommandType.Attack, unit, destination);
        }

        /// <summary>
        /// Creates a new InfluenceAt command
        /// </summary>
        /// <param name="player">The player sending the command</param>
        /// <param name="unit">The Unit which will perform the territory influence</param>
        /// <param name="destination">The destination (tile position) of the influence</param>
        /// <returns>A new InfluenceAt PlayerCommand with the given parameters</returns>
        /// <exception cref="InvalidPlayerCommandException">Thrown when the Command has an invalid combination of parameters</exception>
        public static PlayerCommand InfluenceAt(AIBasePlayer player, Unit unit, Position destination)
        {
            return new PlayerCommand(player, CommandType.InfluenceAt, unit, destination);
        }

        /// <summary>
        /// Creates a new EquipUnit command
        /// </summary>
        /// <param name="player">The player sending the command</param>
        /// <param name="unit">The Unit to equip</param>
        /// <param name="itemType">The item to equip the unit</param>
        /// <returns>A new EquipUnit PlayerCommand with the given parameters</returns>
        public static PlayerCommand EquipUnit(AIBasePlayer player, Unit unit, ItemType itemType)
        {
            return new PlayerCommand(player, CommandType.EquipUnit, unit, itemType);
        }

        /// <summary>
        /// Creates a new SetProduction command
        /// </summary>
        /// <param name="player">The player sending the command</param>
        /// <param name="building">The building to set production on</param>
        /// <param name="production">The production to be set on the building</param>
        /// <returns>A new SetProduction PlayerCommand with the given parameters</returns>
        public static PlayerCommand SetProduction(AIBasePlayer player, Building building, ProduceableElement production)
        {
            return new PlayerCommand(player, CommandType.SetProduction, building, production);
        }

        /// <summary>
        /// Creates a new CreateBuilding command
        /// </summary>
        /// <param name="player">The player sending the command</param>
        /// <param name="unit">The Unit using to create the building</param>
        /// <param name="buildingType">The type of the building being created</param>
        /// <returns>A new CreateBuilding PlayerCommand with the given parameters</returns>
        public static PlayerCommand CreateBuilding(AIBasePlayer player, Unit unit, BuildingType buildingType)
        {
            return new PlayerCommand(player, CommandType.CreateBuilding, unit, buildingType);
        }

        #endregion
    }
}
